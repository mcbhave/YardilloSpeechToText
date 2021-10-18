using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuth2 : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var osc = new
            {
                message = "OK computer",
                client = "12a345b6-7890-98d7-65e4-f321abc1de23"
            };
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osc);
        }
        [HttpPost]
        public IActionResult Post(object id)
        {
           var osc= new
            {
                message = "OK computer",
                client = "12a345b6-7890-98d7-65e4-f321abc1de23"
            };
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osc);
        }
    }
}
