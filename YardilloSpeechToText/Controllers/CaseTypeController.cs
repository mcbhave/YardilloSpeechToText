using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MBADCases.Models;
using MBADCases.Authentication;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Http;
namespace MBADCases.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class CaseTypeController : ControllerBase
    {
        private readonly CaseTypeService _casetypeservice;
        public CaseTypeController(CaseTypeService casetypeservice)
        {
            _casetypeservice = casetypeservice;
        }
        [HttpGet("{id:length(24)}", Name = "GetCaseType")]
        public IActionResult Get(string id)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = id;
                _casetypeservice.Gettenant(tenantid);

                CaseType ocase = _casetypeservice.Get(id);

                if (ocase == null)
                {
                    ocase = new CaseType();
                    sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);

                    var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status404NotFound", Messagecode = "404", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                }
                else
                {
                    var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }

            }
            catch (Exception ex)
            {
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));

            }
        }

        [HttpGet("{name}", Name = "GetCaseTypeByName")]
        public IActionResult GetByName(string name)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = name;
                _casetypeservice.Gettenant(tenantid);

                CaseType ocase = _casetypeservice.GetByName(name);
               
                if (ocase == null)
                {
                    ocase = new CaseType();
                    sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);

                    var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status404NotFound", Messagecode = "404", Callerid = name, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });                   
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                }
                else
                {
                    var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = name, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }

            }
            catch (Exception ex)
            {
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = name, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));

            }
        }
        [HttpPost("{id:length(24)}", Name = "UpdateCaseType")]
        public IActionResult Post(string id, CaseType ocasetype)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(ocasetype);
                _casetypeservice.Gettenant(tenantid);
                ocasetype._id = id;
                _casetypeservice.Update(id, ocasetype);

                sresponse = "Status200OK";
                //var oms = _casetypeservice.SetMessage(oretcase._id, CaseTypeName, "PUT", "200", "Case type insert", usrid, null);
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = ocasetype._id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseTypeResponse(ocasetype, oms));
            }
            catch (Exception ex)
            {
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(ocasetype, oms));
            }
        }

        [HttpGet("search/{filter}", Name = "GetCasetypesbyfilter")]
        public IActionResult Search(string filter)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = filter;
                _casetypeservice.Gettenant(tenantid);
                if (filter.ToLower() == "all") { filter = ""; }
                List<CaseType> ocase = _casetypeservice.Searchcasetypes(filter,true);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);

            }
            catch (Exception ex)
            {
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = "yardillo", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "PUT", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));

            }
        }

        [HttpPut("{CaseTypeName}")]
        public IActionResult Put(string CaseTypeName,CaseType ocasetype)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(ocasetype);
                _casetypeservice.Gettenant(tenantid);
                ocasetype.Createuser = usrid;
                ocasetype.Updateuser = usrid;
                ocasetype.Createdate = DateTime.UtcNow.ToString();
                ocasetype.Updatedate = DateTime.UtcNow.ToString();
                var oretcase = _casetypeservice.Create(CaseTypeName,ocasetype);

                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oretcase);
                var oms= _casetypeservice.SetMessage(new Message() {Messageype= "Status200OK", Messagecode="200", Callerid = oretcase._id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "PUT", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseTypeResponse(ocasetype, oms));
            }
            catch (Exception ex)
            {
               var oms= _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = "yardillo", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "PUT", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(ocasetype, oms));

            }
        }
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = id;
                _casetypeservice.Gettenant(tenantid);
                 _casetypeservice.Remove(id);

                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "DELETE", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseTypeResponse(null, oms));
            }
            catch (Exception ex)
            {
                var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "DELETE", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));
            }
        }
    }
}
