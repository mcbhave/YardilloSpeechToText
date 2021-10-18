using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MBADCases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WixDBController : ControllerBase
    {
        // GET: api/<WixDBController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WixDBController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WixDBController>
        [HttpPost]
        public string provision(string value)
        {
            return "{}";
        }

        // PUT api/<WixDBController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WixDBController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
