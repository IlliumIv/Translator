// Copyright 2019 IlliumIv
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GoogleTranslateFreeApi;

namespace TranslatorHelper
{
    class TranslatorHelper
    {
        // https://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
        // https://stackoverflow.com/questions/53782085/visual-studio-assemblyversion-with-dont-work
        static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;
        static DateTime buildDate;

        const string MockLog = @"..\TranslatorHelper\MockLog.txt";
        const string ErrorLog = @"Error.log";
        const string TranslatedMessagesDir = @"C:\ProgramData\AOTranslator";
        const string TranslatedMessagesLua = @"C:\ProgramData\AOTranslator\messages.lua";

        static long position = GetLastReadPosition();
        static long messagesCount = 1;

        // Translator: (31.10.2019 17:46:47) (NickName):(XWXPIDRNSL) TranslatorEndMessage
        // Translator\:.*TranslatorEndMessage
        static readonly Regex lineCatch = new Regex(@"Translator\:.*TranslatorEndMessage");
        // \((.*?)\)
        static readonly Regex paramCatch = new Regex(@"\((.*?)\)");

        static void Main()
        {
            OnLoad();

            Console.WriteLine($"Translator: v{version}");
            Console.WriteLine($"Last Updated: {buildDate}");
            Console.WriteLine();

            MockCreateLog();
            MockWriterAsync();

            var fileInfo = new FileInfo(MockLog);
#pragma warning disable IDE0067 // Dispose objects before losing scope
            var logsWatcher = new FileSystemWatcher(fileInfo.DirectoryName,
                                                    fileInfo.Name)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
#pragma warning restore IDE0067 // Dispose objects before losing scope
            logsWatcher.Changed += new FileSystemEventHandler(MessagesReceiver);
            logsWatcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void OnLoad()
        {
            buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

            if (!Directory.Exists(TranslatedMessagesDir))
                Directory.CreateDirectory(TranslatedMessagesDir);

            using (FileStream file = File.Create(TranslatedMessagesLua))
            {
                // Just create file
            }
        }

        static void MessagesReceiver(object sender, FileSystemEventArgs args)
        {
            using (StreamReader sr = new StreamReader(MockLog))
            {
                using (var file = File.Open(MockLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (position >= file.Length)
                        return;

                    file.Position = position;

                    using (var reader = new StreamReader(file))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            Match match = lineCatch.Match(line);
                            if (match.Success)
                                TranslateAsync(match.Value, messagesCount++);
                        }

                        position = file.Position;
                    }
                }
            }
        }

        private static async void TranslateAsync(string line, long msgCount)
        {
            var translator = new GoogleTranslator();

            Language from = Language.Auto;
            Language to = Language.Russian;

            MatchCollection matches = paramCatch.Matches(line);

            Message msg = new Message(matches[0].Value, matches[1].Value, matches[2].Value);

            bool success = false;
            int retry = 0;

            while (!success && retry < 3)
            {
                try
                {
                    TranslationResult result = await translator.TranslateLiteAsync(msg.Text, from, to);
                    msg.Text = result.MergedTranslation;

                    // messages[1]={["time"]="00:00:00",["nickname"]="Illium",["translatedMessage"]="Some string whith message"}
                    File.AppendAllText(TranslatedMessagesLua, $"messages[{msgCount}]=" + "{" +
                                                              $"[\"time\"]=\"{msg.TimeStamp}\"" +
                                                              $"[\"nickname\"]=\"{msg.Sender}\"" +
                                                              $"[\"translatedMessage\"]=\"{msg.Text}\"\n");

                    Console.WriteLine($"{msg.TimeStamp}[{msg.Sender}]:{msg.Text}");

                    success = true;
                }
                catch (Exception e)
                {
                    if (!File.Exists(ErrorLog))
                        using (FileStream file = File.Create(MockLog))
                        {
                            // Create file if not exist
                        }
                    File.AppendAllText(ErrorLog, $"{e.Message}\n" +
                                                 $"{e.StackTrace}");
                    retry++;
                }
            }

            if (!success)
                Console.WriteLine($"Could not translate message in {retry} attempts");
        }

        private static long GetLastReadPosition()
        {
            return 0;
        }

        #region Mock
        static readonly Random _rng = new Random();
        static readonly string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static void MockCreateLog()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            using (FileStream file = File.Create(MockLog))
            {
                // Just create file
            }

            Console.WriteLine($"Created MockLog here: {new FileInfo(MockLog).FullName}");
            Console.WriteLine();
        }

        private static void OnExit(object sender, EventArgs args)
        {
            File.Delete(MockLog);
        }

        static async void MockWriterAsync()
        {
            await Task.Run(() => MockWritingRandomLines());
        }

        static void MockWritingRandomLines()
        {
            while (true)
            {
                string[] str = new string[]
                {
                    $"Translator: ({DateTime.Now}) (NickName):({MockRandomString(10)}) TranslatorEndMessage"
                };

                File.AppendAllLines(MockLog, str);
                // Thread.Sleep(new Random().Next(10000));
                Thread.Sleep(10000);
            }
        }

        static string MockRandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }

            return new string(buffer);
        }
        #endregion
    }
}
