using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using MBADCases.Models;
using MBADCases.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MBADCases.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class TenantController : ControllerBase
    {
        //private const string V = "1.0";
        private readonly TenantService _tenantservice;
       
        public TenantController(TenantService tenantservice)
        {
            _tenantservice = tenantservice;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _tenantservice.Gettenant(tenantid);
           
                List<Tenant> ocase = _tenantservice.GetByUserid(usrid);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
            }
            catch { throw; }
         }

        // GET api/<CaseController>/5
        [MapToApiVersion("1.0")]
        [HttpGet("{id:length(24)}", Name = "GetTenant")]
        public IActionResult Get(string id)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _tenantservice.Gettenant(tenantid);

                Tenant ocase = _tenantservice.Get(id);
                oms = _tenantservice.SetMessage(id, id, "GET", "200", "Case type Search", usrid, null);
                if (ocase == null)
                {
                    ocase = new Tenant();
                    oms = _tenantservice.SetMessage(ocase._id, id, "GET", "400", "Not found", usrid, null);
                    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                }
                else
                {
                    oms = _tenantservice.SetMessage(ocase._id, id, "GET", "200", "Case type Search by name", usrid, null);
                    ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }
            }
            catch (Exception ex)
            {
                Tenant ocase = new Tenant();
                ocase._id = id;
                oms = _tenantservice.SetMessage(id, id, "GET", "501", "Case Type Search", usrid, ex);

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, ocase);
            }
        }
         
        // POST api/<CaseController>
        [MapToApiVersion("1.0")]
        [HttpPost("{id:length(24)}", Name = "UpdateTenant")]
        public IActionResult Post(string id, Tenant tenant)
        {
            Message oms;
            Tenant oten=null;
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
          
            try
            {
                smessage = "Database connection string updates not allowed for BASIC scubscription";

                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(tenant);
                _tenantservice.Gettenant(tenantid);
                oten = _tenantservice.Get(id);
                
                if (oten != null)
                {
                    oten._id = id;
                    oten.Tenantdesc = tenant.Tenantdesc;
                    if (tenant!=null && tenant.Rapidsubscription!=null)
                    {
                      if((tenant.Rapidsubscription.Contains("PRO")) || (tenant.Rapidsubscription.Contains("ULTRA")) || (tenant.Rapidsubscription.Contains("MEGA")))
                        {
                            oten.Dbconnection = tenant.Dbconnection;
                            smessage = "";
                        }
                        else
                        {
                            oten.Dbconnection = "";
                        }
                    }
                    else
                    {
                        oten.Dbconnection = "";
                    }
                   
                    _tenantservice.Update(id, oten);
                }
                
                
                oms = _tenantservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = tenant._id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "TENANT", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new TenantResponse(oten, oms));
            }
            catch (Exception ex)
            {
                smessage = ex.ToString();
                oms = _tenantservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = tenant._id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "TENANT", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new TenantResponse(oten, oms));
            }

        }

        [HttpPost("{id:length(24)}/{apikey}", Name = "UpdateTenantApiKey")]
        public IActionResult Post(string id,string apikey)
        {
            Message oms;
            Tenant oten = null;
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";

            try
            {


                srequest = id + ", key=" + apikey;
                _tenantservice.Gettenant(tenantid);

                  oten = _tenantservice.Get(id);
                if (oten != null)
                {
                    oten._id = id;
                    oten.Rapidapikey = apikey;
                    _tenantservice.Update(id, oten);
                }

                oms = _tenantservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "TENANT", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new TenantResponse(oten, oms));
            }
            catch (Exception ex)
            {
                smessage = ex.ToString();
                oms = _tenantservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "POST", Callertype = "TENANT", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new TenantResponse(null, oms));
            }

        }

        // PUT api/<CaseController>/5
        [HttpPut]
        public IActionResult Put(Tenant tenant)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
           

            try
            {
                               
                _tenantservice.Gettenant(tenantid);
                //do not allow to set a database while tenant creation
                tenant.Dbconnection = "";
                //check if tenant with user name exists
                tenant.Createuser = usrid;
                tenant._owner = usrid;
                tenant.Createdate = DateTime.UtcNow.ToString();
                Tenant ocase = _tenantservice.GetByName(tenantid);
                tenant.YAuthSource = ocase.YAuthSource;
                if (tenant.YAuthSource == null){ tenant.YAuthSource = "yardillo"; }
              
                var oretcase = _tenantservice.Create(tenant);
                oms = _tenantservice.SetMessage(oretcase._id, "", "PUT", "200", "Tenant insert", usrid, null);

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, tenant);
            }
            catch (Exception ex)
            {
                oms = _tenantservice.SetMessage(null, "", "PUT", "", "Tenant insert", usrid, ex);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, tenant);

            }

        }

        // DELETE api/<CaseController>/5
        [HttpDelete("{id}")]
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
                _tenantservice.Gettenant(tenantid);
                _tenantservice.Remove(id);
                oms = _tenantservice.SetMessage( id, id, "DELETE", "200", "Case delete", usrid, null);
            
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, id);
            }
            catch (Exception ex)
            {
                oms = _tenantservice.SetMessage(  id, id, "DELETE", "200", "Case delete", usrid, ex);
                 
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, ocase);

            }
        }

    }
}
