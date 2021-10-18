using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MBADCases.Models;
using MBADCases.Authentication;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Http;
using System.IO;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MBADCases.Controllers
{
  
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class CaseController : ControllerBase
    {
        //private const string V = "1.0";
        private readonly CaseService _caseservice;
        public CaseController(CaseService caseservice)
        {
            _caseservice = caseservice;
        }
        // GET: api/<CaseController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<CaseController>/5
        [MapToApiVersion("1.0")]
        [HttpGet("{id:length(24)}", Name = "GetCase")]
        public IActionResult Get(string id)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _caseservice.Gettenant(tenantid);

                Case ocase = _caseservice.Get(id);
                
                if (ocase == null)
                {
                    oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "404", "Case Search", usrid, null);
                    ocase = new Case();
                    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };

                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                }
                else
                {
                    oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "200", "Case Search", usrid, null);
                    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id };
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }
            }
            catch (Exception ex)
            {
                Case ocase = new Case();
                ocase._id = id;
                oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "", "", usrid, ex);
 
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
            }
        }
        [MapToApiVersion("2.0")]
        [HttpGet("{id:length(24)}", Name = "GetCase")]
        public IActionResult Get2(string id)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _caseservice.Gettenant(tenantid);

                Case ocase = _caseservice.Get(id);
                oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "200", "Case Searched", usrid, null);

                ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id };

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
            }
            catch (Exception ex)
            {
                Case ocase = new Case();
                ocase._id = id;
                oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "GET", "", "", usrid, ex);
 
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
            }

        }
        [MapToApiVersion("1.0")]
        [HttpGet("{filter}", Name = "GetCasesbyfilter")]
        public IActionResult Search(string filter)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _caseservice.Gettenant(tenantid);

                List<Case> ocase = _caseservice.Searchcases(filter);
               
                if (ocase != null)
                {
                 
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }
               else 
                {
                    oms = _caseservice.SetMessage(ICallerType.CASE_SEARCH, "", "", "GET", "404", "Case Search", usrid, null);
                    ocase = new List<Case>();
                   
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, ocase);
                }
                
            }
            catch  
            {
                throw;
            }
        }
            // POST api/<CaseController>
            [MapToApiVersion("1.0")]
        [HttpPost("{id:length(24)}", Name = "UpdateCase")]
        public IActionResult Post(string id,Case ocase)
        {
            Message oms;
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            var usrid = HttpContext.Session.GetString("mbaduserid");

            string sj = ocase.Fields.ToJson();
            ocase.Updateuser = usrid;
            //string id = ocase._id;
            ocase._id = id;
            try
            {
                _caseservice.Gettenant(tenantid);
                var oc = _caseservice.Get(id);
                if(oc != null)
                {
                    ocase._id = oc._id;
                    if (ocase.Casetitle!=null && ocase.Casetitle.ToLower() != oc.Casetitle.ToLower()) { oc.Casetitle = ocase.Casetitle; }
                    if (ocase.Casenumber != 0 && ocase.Casenumber != oc.Casenumber) { oc.Casenumber = ocase.Casenumber; }
                    if (ocase.Casedescription != null && ocase.Casedescription.ToLower() != oc.Casedescription.ToLower()) { oc.Casedescription = ocase.Casedescription; }
                    if (ocase.Casestatus != null && ocase.Casestatus.ToLower() != oc.Casestatus.ToLower()) { oc.Casestatus = ocase.Casestatus; }
                    if (ocase.Casetype != null && ocase.Casetype.ToLower() != oc.Casetype.ToLower()) { oc.Casetype = ocase.Casetype; }
                    if (ocase.Currentactionid != null && ocase.Currentactionid.ToLower() != oc.Currentactionid.ToLower()) { oc.Currentactionid = ocase.Currentactionid; }
                    if (ocase.Currentactivityid != null && ocase.Currentactivityid.ToLower() != oc.Currentactivityid.ToLower()) { oc.Currentactivityid = ocase.Currentactivityid; }
                    Casefield tmpf = null;
                       foreach (Casefield f in ocase.Fields)
                        {
                            tmpf= oc.Fields.Find(fo => fo.Fieldid.ToLower() == f.Fieldid.ToLower());
                            if (tmpf != null)
                            {
                                if (f.Value!=null && f.Value.ToLower() != tmpf.Value.ToLower()) { tmpf.Value = f.Value; }
                            if (f.Type != null && f.Type.ToLower() != tmpf.Type.ToLower()) { tmpf.Type = f.Type; }
                        }
                       
                        }
                }

                _caseservice.Update(id, oc);
                  oms = _caseservice.SetMessage(ICallerType.CASE, id, sj, "POST", "UPDATE", "Case update", usrid, null);
                 return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(ocase._id, oms));
            }
            catch(Exception ex)
            {
                 
                oms = _caseservice.SetMessage(ICallerType.CASE, id, sj, "POST", "UPDATE", "Case update", usrid, ex);
                 return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
            }
          
        }

        // PUT api/<CaseController>/5
        [HttpPut("{CaseType}")]
        public IActionResult Put(string CaseType, Case ocase)
        {
            Message oms;
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            var usrid = HttpContext.Session.GetString("mbaduserid");

            string sj = ocase.Fields.ToJson();
            string id = ocase._id;
            string createuserid = usrid;
            try
            {
                _caseservice.Gettenant(tenantid);
                ocase.Createuser = createuserid;

                if (ocase.Casetype != CaseType) { ocase.Casetype = CaseType; }
                if (ocase.Updateuser == null) { ocase.Updateuser = createuserid; }
                if (ocase.Createuser == null) { ocase.Createuser = createuserid; }
                if (ocase.Createdate == null) { ocase.Createdate = DateTime.UtcNow.ToString(); }
                if (ocase.Updatedate == null) { ocase.Updatedate = DateTime.UtcNow.ToString(); }

               
                var oretcase = _caseservice.Create(ocase);
                oms = _caseservice.SetMessage(ICallerType.CASE, oretcase._id, sj, "PUT", "200", "Case insert", createuserid, null);

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(oretcase._id, oms));
            }
            catch(Exception ex)
            {
                oms = _caseservice.SetMessage(ICallerType.CASE, id, sj, "PUT", "", "Case insert", createuserid, ex);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
                 
            }
           
        }

        // DELETE api/<CaseController>/5
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            Message oms;
            Case ocase = new Case();
            
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            //string sj = ocase.Caseattributes.ToJson();
            // string id = ocase._id;
            try
            {
                _caseservice.Gettenant(tenantid);

                _caseservice.Remove(id);
                oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "DELETE", "200", "Case delete", usrid, null);
                 return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
            }
            catch (Exception ex)
            {
                oms = _caseservice.SetMessage(ICallerType.CASE, id, id, "DELETE", "200", "Case delete", usrid, ex);
                 return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));

            }
        }
        
        [HttpDelete("{Casetype}/{filter}")]
        public IActionResult SearchAndDelete(string Casetype,string filter)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                _caseservice.Gettenant(tenantid);

                List<Case> cases = _caseservice.Searchcases("Casetype=" + Casetype + "&" + filter);


                foreach (Case c in cases)
                {
                    _caseservice.Remove(c._id);
                }

                var oms = _caseservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = filter, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(null, oms));
                
            }
            catch (Exception ex)
            {
                var oms = _caseservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = filter, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(null, oms));
            }
        }
    }
}
