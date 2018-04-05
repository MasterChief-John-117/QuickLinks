using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace QuickLinks.Controllers
{
    [Route("/")]
    public class ValuesController : Controller
    {
        // GET /
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return null;
        }

        // GET /5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return null;
        }

        // POST /
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT /5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE /5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
