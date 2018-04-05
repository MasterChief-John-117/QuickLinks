using Microsoft.AspNetCore.Mvc;
using System;

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
                Program.database.GetCollection<Entities.Link>("links").Insert(link);
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
                    return new JsonResult("NOT FOUND");
                }
            }
        }
    }
}
