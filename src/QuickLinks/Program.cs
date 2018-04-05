using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using LiteDB;
using System.IO;
using System;
using System.Diagnostics;

namespace QuickLinks
{
    public class Program
    {
        public static LiteDatabase database = new LiteDatabase(@"filename=lite.db; mode=exclusive; password=YOURPASSWORDHERE");
        public static void Main(string[] args)
        {
            InitDB();
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
        }

        private static void InitDB()
        {
            if (!database.CollectionExists("words"))
            {
                var wordDb = database.GetCollection<Word>("words");
                if (!File.Exists("words.txt"))
                {
                    Console.WriteLine("Word list does not exist. Please ensure it is named 'words.txt', and is placed in the root directory. If not please check the original repository to download the list");
                    Console.WriteLine("Press the any key to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string line;
                var file = new StreamReader("words.txt");
                while (!string.IsNullOrEmpty((line = file.ReadLine())))
                {
                    try
                    {
                        wordDb.Insert(new Word { Value = line });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.ReadKey();
                    }
                }
                file.Close();
                stopwatch.Stop();
                Console.WriteLine($"Created word list in {stopwatch.ElapsedMilliseconds} ms");
                database.GetCollection<Entities.Link>("links").EnsureIndex(l => l.AnalyticsTag);
            }
        }
        public class Word
        {
            [BsonId]
            public string Value { get; set; }
        }
    }
}
