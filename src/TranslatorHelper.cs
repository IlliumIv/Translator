// Copyright 2019 IlliumIv
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GoogleTranslateFreeApi;
using TranslatorHelper.Message;

namespace TranslatorHelper
{
    class TranslatorHelper
    {
        // https://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
        // https://stackoverflow.com/questions/53782085/visual-studio-assemblyversion-with-dont-work
        static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;
        static DateTime buildDate;

        const string MockLog = @"..\TranslatorHelper\MockLog.txt";
        const bool Debug = false;

        static string ModsLog;
        private static readonly Encoding srcEncoding = Encoding.GetEncoding(1251);

        const string ErrorLog = @"Error.log";
        const string TranslatedMessagesDir = @"C:\ProgramData\AOTranslator";
        const string TranslatedMessagesLua = @"C:\ProgramData\AOTranslator\messages.lua";


        static long position;
        static long messagesCount = 1;

        // Info: addon Translator(0): {08:25:39} {2} {Illium}:{Some message} TranslatorEndMessage
        // Translator\:.*TranslatorEndMessage
        static readonly Regex lineCatch = new Regex(@"Translator\(.*\)\:.*TranslatorEndMessage");
        // \{(.*?)\}
        static readonly Regex paramCatch = new Regex(@"\{(.*?)\}");

        static void Main()
        {
            OnLoad();

            Console.WriteLine($"Translator: v{version}");
            Console.WriteLine($"Last Updated: {buildDate}");
            Console.WriteLine();

            AOgameSearcher();
            position = GetEndOfFilePosition(ModsLog);

            // MockCreateLog();
            // MockWriterAsync();


            var fileInfo = new FileInfo(ModsLog);

#pragma warning disable CA2000 // Dispose objects before losing scope
            var logsWatcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);
#pragma warning restore CA2000 // Dispose objects before losing scope

            logsWatcher.NotifyFilter = NotifyFilters.LastWrite;
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

        private static void AOgameSearcher()
        {
            Console.WriteLine("Starting AOgame searcher...");
            Process[] processes = Process.GetProcessesByName("AOgame");
            while (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("AOgame");
                Thread.Sleep(10000);
            }

            var path = processes.First<Process>().MainModule.FileName;
            var fileInfo = new FileInfo(path);
            path = fileInfo.Directory.Parent.FullName;
            ModsLog = Path.Combine(path, @"Personal\Logs\mods.txt");
            Console.WriteLine($"AOgame found! LogFile here: {ModsLog}");
            Console.WriteLine();
        }

        static void MessagesReceiver(object sender, FileSystemEventArgs args)
        {
            try
            {
                using (StreamReader sr = new StreamReader(ModsLog, encoding: srcEncoding))
                {
                    using (var file = File.Open(ModsLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (position >= file.Length)
                            return;

                        file.Position = position;

                        using (var reader = new StreamReader(file, encoding: srcEncoding))
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
            catch (Exception e)
            {
                if (!File.Exists(ErrorLog))
                    using (FileStream file = File.Create(ErrorLog))
                    {
                        // Create file if not exist
                    }
                File.AppendAllText(ErrorLog, DateTime.Now + $"{e.Message}\n" +
                                             $"{e.StackTrace}\n");
            }
        }

        private static async void TranslateAsync(string line, long msgCount)
        {
            var translator = new GoogleTranslator();

            Language from = Language.Auto;
            Language to = Language.Russian;

            MatchCollection matches = paramCatch.Matches(line);

            if (!int.TryParse(StringHelper.Replacer.Clean(matches[1].Value), out int parsedChatType))
                parsedChatType = 0;

            ChatType chatType = ChatType.LogColorYellow;

            bool knownMesType = true;

            if (Enum.IsDefined(typeof(ChatType), parsedChatType))
                chatType = (ChatType)parsedChatType;
            else
                knownMesType = false;

            IMessage msg = new IMessage(matches[0].Value,
                                      chatType,
                                      matches[2].Value,
                                      matches[3].Value);


            string newMessage = $"{msg.TimeStamp} [{msg.Sender}]: \"{msg.Text}\"";

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"<< {newMessage}");
            Console.ResetColor();

            if (!knownMesType)
            {
                string unknownMesType = $"Finded unexpected type of message: {parsedChatType}";

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"<< {unknownMesType}");
                Console.ResetColor();

                if (!File.Exists(ErrorLog))
                    using (FileStream file = File.Create(ErrorLog))
                    {
                        // Create file if not exist
                    }
                File.AppendAllText(ErrorLog, DateTime.Now +  $" {unknownMesType}\n   \"{newMessage}\"\n\n");
            }


            bool success = false;
            int retry = 0;

            while (!success && retry < 3)
            {
                try
                {
                    if (!Debug)
                    {
                        if (!(msg.Text.Length <= 2) & !(msg.ChatType == ChatType.LogColorYellow))
                        {
                            TranslationResult result = await translator.TranslateLiteAsync(msg.Text, from, to).ConfigureAwait(true);
                            msg.Text = result.MergedTranslation;
                        }
                    }

                    success = true;
                }
                catch (Exception e)
                {
                    if (!File.Exists(ErrorLog))
                        using (FileStream file = File.Create(ErrorLog))
                        {
                            // Create file if not exist
                        }
                    File.AppendAllText(ErrorLog, DateTime.Now + $"{e.Message}\n" +
                                                 $"{e.StackTrace}\n");
                    retry++;
                }
            }

            if (!success)
                Console.WriteLine($"Could not translate message in {retry} attempts");

            MessagesSender(msg, msgCount);
        }

        private static void MessagesSender(IMessage message, long count)
        {
            /*
                                                        messages[1]={
                                                        ["ChatType"]="LogColorYellow",
                                                        ["TimeStamp"]="08:55:12",
                                                        ["Sender"]="Illium",
                                                        ["Text"]="Message!"}
            */
            File.AppendAllText(TranslatedMessagesLua, $"messages[{count}]=" + "{" +
                                                      $"[\"ChatType\"]=\"{message.ChatType}\"," +
                                                      $"[\"TimeStamp\"]=\"{message.TimeStamp}\"," +
                                                      $"[\"Sender\"]=\"{message.Sender}\"," +
                                                      $"[\"Text\"]=\"{message.Text}\"" + "}\n", srcEncoding);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($">> {message.TimeStamp} [{message.Sender}]: \"{message.Text}\"");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static long GetEndOfFilePosition(string path)
        {
            long pos = 0;

            if (File.Exists(path))

                using (StreamReader sr = new StreamReader(path, encoding: srcEncoding))
                {
                    using (var file = File.Open(ModsLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (position >= file.Length)
                            return pos;

                        return file.Length;
                    }
                }
            return pos;
        }

        #region Mock
        static readonly Random _rng = new Random();
        const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
            await Task.Run(() => MockWritingRandomLines()).ConfigureAwait(false);
        }

        static void MockWritingRandomLines()
        {
            while (true)
            {
                int randomType = new Random().Next(Enum.GetValues(typeof(ChatType)).Length);

                string[] str = new string[]
                {
                    $"Translator: ({DateTime.Now.TimeOfDay}) ({randomType}) (NickName):({MockRandomString(10)}) TranslatorEndMessage"
                };

                File.AppendAllLines(MockLog, str);
                Thread.Sleep(new Random().Next(10000));
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
