using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QuickLinks.Entities
{
    public class Link
    {
        public class Visitor
        {
            public string UserAgent;
            public string Ip;
        }
        public class Analytics
        {
            public int AllVisitorCount { get; private set; }
            public int UniqueVisitorCount { get; private set; }
            public Analytics(IEnumerable<Visitor> AllVisitors, IEnumerable<Visitor> UniqueVisitors)
            {
                this.AllVisitorCount = AllVisitors == null ? 0 : AllVisitors.Count();
                this.UniqueVisitorCount = UniqueVisitors == null ? 0 : UniqueVisitors.Count();
            }
        }
        public string OriginalUrl { get; private set; }
        [BsonId]
        public string ShortUrl { get; private set; }
        public string AnalyticsTag { get; private set; }
        private List<Visitor> AllVisitors { get; set; }
        private HashSet<Visitor> UniqueVisitors { get; set; }

        public Link()
        {

        }
        public Link(string url)
        {
            OriginalUrl = url;
            Parallel.Invoke
            (
                () => ShortUrl = GetNewShortLink(),
                () => AnalyticsTag = GetNewAnalyticsTag()
            );
            AllVisitors = new List<Visitor>();
            UniqueVisitors = new HashSet<Visitor>();
        }

        public Analytics GetAnalytics()
        {
            return new Analytics(this.AllVisitors, this.UniqueVisitors);
        }

        private string GetNewShortLink()
        {
            var linkDb = Program.database.GetCollection<Link>("links");
            while (true)
            {
                var words = Program.database.GetCollection<Program.Word>("words").FindAll().Select(w => w.Value);
                string val = "";
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                val += words.ElementAt((new Random()).Next(0, words.Count()));
                if (!linkDb.Exists(l => l.ShortUrl == val))
                {
                    return val;
                }
                Console.WriteLine($"{DateTime.Now}: Generating new shortlink.... Collision found, trying again");
            }
        }

        private string GetNewAnalyticsTag()
        {
            var linkdb = Program.database.GetCollection<Link>("links");
            while (true)
            {
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
                Console.WriteLine($"{DateTime.Now}: Generating new Analytics tag.... Collision found, trying again");
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
