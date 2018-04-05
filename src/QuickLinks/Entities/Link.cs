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
        public string ShortUrl { get; private set; }
        public string AnalyticsTag { get; private set; }
        public List<Visitor> AllVisitors { get; private set; }
        public HashSet<Visitor> UniqueVisitors { get; private set; }

        public Link(string url)
        {
            OriginalUrl = url;
            AnalyticsTag = GetNewAnalyticsTag();
            ShortUrl = GetNewShortLink();
            AllVisitors = new List<Visitor>();
            UniqueVisitors = new HashSet<Visitor>();
        }

        private string GetNewShortLink()
        {
            var words = Program.database.GetCollection<Program.Word>("words").FindAll().Select(w => w.Value);
            string val = "";
            val += words.ElementAt((new Random()).Next(0, words.Count()));
            val += words.ElementAt((new Random()).Next(0, words.Count()));
            val += words.ElementAt((new Random()).Next(0, words.Count()));
            return val;
        }

        private string GetNewAnalyticsTag()
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            string tag = "";
            for(int i = 0; i < 16; i++)
            {
                tag += chars[NextInt()];
            }
            return tag;
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
