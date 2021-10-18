using MBADCases.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Controllers
{
    public class GiftCode : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [MapToApiVersion("1.0")]
        [HttpPost("{id:length(24)}", Name = "Update Gift Code")]
        public IActionResult Post(string id, GiftCard ocase)
        {

            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, id);
        }
        [HttpPut()]
        public IActionResult Put(GiftCard ocase)
        {

            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
        }
    }
}
