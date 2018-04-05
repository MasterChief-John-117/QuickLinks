using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace QuickLinks.Entities
{
    public class Link
    {
        public class Visitor
        {
            public string UserAgent;
            public string Ip;
        }
        public string OriginalUrl { get; private set; }
        [BsonId]
        public string ShortUrl { get; private set; }
        public string AnalyticsTag { get; private set; }
        public List<Visitor> AllVisitors { get; private set; }
        public HashSet<Visitor> UniqueVisitors { get; private set; }

        public Link()
        {

        }
        public Link(string url)
        {
            OriginalUrl = url;
            ShortUrl = GetNewShortLink();
            AnalyticsTag = GetNewAnalyticsTag();
            AllVisitors = new List<Visitor>();
            UniqueVisitors = new HashSet<Visitor>();
        }

        private string GetNewShortLink()
        {
            var linkDb = Program.database.GetCollection<Link>("links");
            while (true)
            {
                Console.WriteLine("Generating new Shortlink");
                var words = Program.database.GetCollection<Program.Word>("words").FindAll().Select(w => w.Value);
                string val = "";
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                if (!linkDb.Exists(l => l.ShortUrl == val))
                {
                    return val;
                }
            }
        }

        private string GetNewAnalyticsTag()
        {
            var linkdb = Program.database.GetCollection<Link>("links");
            while (true)
            {
                Console.WriteLine("Generating new Analytics tag");
                char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
                string tag = "";
                for (int i = 0; i < 16; i++)
                {
                    tag += chars[NextInt()];
                }
                if(!linkdb.Exists(l => l.AnalyticsTag == tag))
                {
                    return tag;
                }
            }
        }

        private int NextInt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            return Math.Abs( BitConverter.ToInt32(buffer, 0) % 26);
        }
    }
}
