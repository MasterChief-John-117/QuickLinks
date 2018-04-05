using Microsoft.AspNetCore.Mvc;

namespace QuickLinks.Controllers
{
    [Route("/")]
    public class ValuesController : Controller
    {
        // GET /anything
        [HttpGet("{*url}")]
        public IActionResult Get(string url)
        {
        }
    }
}
