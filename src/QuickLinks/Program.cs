using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using LiteDB;

namespace QuickLinks
{
    public class Program
    {
        public static LiteDatabase database = new LiteDatabase(@"filename=lite.db; mode=exclusive; password=YOURPASSWORDHERE");
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
        }
    }
}
