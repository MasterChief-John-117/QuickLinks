using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;

namespace QuickLinks.Controllers
{
    [Route("/")]
    public class ValuesController : Controller
    {
        // GET /anything
        [HttpGet("{*url}")]
        public IActionResult Get(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var link = new Entities.Link(url);
                new Thread(() => {
                    Thread.CurrentThread.IsBackground = true;
                    SaveLink(link);
                }).Start();
                return new JsonResult(link);
            }
            else
            {
                var link = Program.database.GetCollection<Entities.Link>("links").FindById(url);
                if (link != null)
                {
                    return RedirectPermanent(link.OriginalUrl);
                }
                else
                {
                    HttpContext.Response.StatusCode = 404;
                    return new JsonResult("Not Found");
                }
            }
        }
        private void SaveLink(Entities.Link link)
        {
            Program.database.GetCollection<Entities.Link>("links").Insert(link);
            Console.WriteLine($"New Link:\nOriginal URL: {link.OriginalUrl}\nShortened URL: {link.ShortUrl}\nAnalytics Tag: {link.AnalyticsTag}");
        }
    }
}
