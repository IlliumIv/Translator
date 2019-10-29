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
        static Version version = Assembly.GetExecutingAssembly().GetName().Version;
        static DateTime buildDate;

        const string logs = @"..\..\..\testLog.txt";
        static long position = GetMyLastReadPosition();

        static readonly Random _rng = new Random();
        static string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        static void Main(string[] args)
        {
            OnLoad();
            CreateExampleLog();


            Console.WriteLine($"Translator: v{version}");
            Console.WriteLine($"Last Updated: {buildDate}");
            Console.WriteLine();

            ExampleWriterAsync();

            MessagesReceiver();

            Console.ReadLine();
        }

        static void OnLoad()
        {
            buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        }

        static void CreateExampleLog()
        {
            if (!File.Exists(logs))
                using (FileStream file = File.Create(logs)) ;
        }

        static async void MessagesReceiver()
        {

        }

        static void ReadingLogs()
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
                                Console.WriteLine(line);
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

        private static long GetMyLastReadPosition()
        {
            return 0;
        }

        static async void ExampleWriterAsync()
        {
            await Task.Run(() => WritingRandomLines());
        }

        static void WritingRandomLines()
        {
            while (true)
            {
                string[] str = new string[]
                {
                    RandomString(10)
                };

                Console.WriteLine($"Writing string: {str.First<string>()}");
                File.AppendAllLines(logs, str);
                // Thread.Sleep(new Random().Next(10000));
                Thread.Sleep(500);
            }
        }

        static string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }

            return new string(buffer);
        }
    }
}
