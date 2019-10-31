using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TranslatorHelper
{
    class TranslatorHelper
    {
        // https://stackoverflow.com/questions/826777/how-to-have-an-auto-incrementing-version-number-visual-studio
        // https://stackoverflow.com/questions/53782085/visual-studio-assemblyversion-with-dont-work
        static readonly Version version = Assembly.GetExecutingAssembly().GetName().Version;
        static DateTime buildDate;

        const string logs = @"..\TranslatorHelper\MockLog.txt";
        static long position = GetLastReadPosition();

        static void Main()
        {
            OnLoad();

            Console.WriteLine($"Translator: v{version}");
            Console.WriteLine($"Last Updated: {buildDate}");
            Console.WriteLine();

            MockCreateLog();
            MockWriterAsync();

            var fileInfo = new FileInfo(logs);
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
        }

        static void MessagesReceiver(object sender, FileSystemEventArgs args)
        {
            try
            {
                using (StreamReader sr = new StreamReader(logs))
                {
                    using (var file = File.Open(logs, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (position >= file.Length)
                            return;

                        file.Position = position;

                        using (var reader = new StreamReader(file))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                Console.WriteLine(Translate(line).Text);
                            }

                            position = file.Position;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private static Message Translate(string line)
        {
            Message msg = new Message(DateTime.Now, "Sender", line);
            return msg;
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

            using (FileStream file = File.Create(logs))
            {
                // Just create file
            }

            Console.WriteLine($"Created MockLog here: {new FileInfo(logs).FullName}");
            Console.WriteLine();
        }

        private static void OnExit(object sender, EventArgs args)
        {
            File.Delete(logs);
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
                    $"Translator: ({DateTime.Now}) TranslatorSender:((NickName)), TranslatorMessage:(({MockRandomString(10)}))"
                };

                // Console.WriteLine($"Writing string: {str.First<string>()}");
                File.AppendAllLines(logs, str);
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
