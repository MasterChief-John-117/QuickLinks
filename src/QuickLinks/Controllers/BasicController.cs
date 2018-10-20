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
            if(url == null || url.ToLower() == "help")
            {
                return RedirectPermanent("https://mastrchef.rocks/quicklinks");
            }
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var link = new Entities.Link(url);
                new Thread(() => {
                    Thread.CurrentThread.IsBackground = true;
                    SaveLink(link);
                }).Start();
                return new JsonResult(link);
            }
            else if (url.ToLower().StartsWith("custom/"))
            {
                url = url.Substring("custom/".Length);
                string sLink = url.Split("/")[0];
                url = url.Substring($"{sLink}/".Length);
                var testLink = Program.database.GetCollection<Entities.Link>("links").FindOne(l => l.ShortUrl.ToLower() == sLink.ToLower());
                if (testLink == null)
                {
                    if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    {
                        var link = new Entities.Link(url, sLink);
                        new Thread(() => {
                            Thread.CurrentThread.IsBackground = true;
                            SaveLink(link);
                        }).Start();
                        return new JsonResult(link);
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = 400;
                        return new JsonResult("Invalid URL");
                    }
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;
                    return new JsonResult("ShortLink Already in Use");
                }
            }
            else
            {
                var link = Program.database.GetCollection<Entities.Link>("links").FindOne(l => l.ShortUrl.ToLower() == url.ToLower());
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
            Console.WriteLine($"New Link:\n  Original URL: {link.OriginalUrl}\n  Shortened URL: {link.ShortUrl}\n  Analytics Tag: {link.AnalyticsTag}");
        }

        [HttpGet("analytics/{key}")]
        public IActionResult Analytics(string key)
        {
            var link = Program.database.GetCollection<Entities.Link>("links").FindOne(l => l.AnalyticsTag == key);
            if (link != null)
            {
                return new JsonResult(link.GetAnalytics());
            }
            else return new JsonResult("Not Found");
        }
    }
}
