using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MBADCases.Models;
using MBADCases.Authentication;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Http;
namespace MBADCases.Authentication
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class SearchCasesController : ControllerBase
    {
        private readonly CaseService _caseservice;
        public SearchCasesController(CaseService caseservice)
        {
            _caseservice = caseservice;
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{filter}", Name = "SearchCases")]
        public IActionResult Get(string filter)
        {

            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _caseservice.Gettenant(tenantid);

                List<Case> ocase = _caseservice.Searchcases(filter);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                //if (ocase == null)
                //{
                //    oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "404", "Case Search", usrid, null);
                //    ocase = new Case();
                //    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };

                //    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                //}
                //else
                //{
                //    oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "200", "Case Search", usrid, null);
                //    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
                //    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                //}
            }
            catch {  
                Case ocase = new Case();
                //ocase._id = id;
                //oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "", "", usrid, ex);

                //return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
                throw;
            }
        }
    }
}
