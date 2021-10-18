using MBADCases.Authentication;
using MBADCases.Services;
using MBADCases.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MBADCases.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class SchedulerController : Controller
    {
        //private const string V = "1.0";
        private readonly ScheduleService _scheduleService;
        public SchedulerController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

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
                _scheduleService.Gettenant(tenantid);

                Schedule  ocase = _scheduleService.Get(id);

                if (ocase == null)
                {
                    ocase = new Schedule();
                    sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);

                    var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status404NotFound", Messagecode = "404", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                }
                else
                {
                    var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }

            }
            catch (Exception ex)
            {
                var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));

            }
        }
        public IActionResult Get()
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            //string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                //srequest = id;
                _scheduleService.Gettenant(tenantid);

                List< Schedule> ocase = _scheduleService.Get();

                if (ocase == null)
                {
                    ocase = new List<Schedule>();
                    //sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);

                    //var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status404NotFound", Messagecode = "404", Callerid = "all", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, null);
                }
                else
                {
                    var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = "all", Callerrequest = "", Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }

            }
            catch (Exception ex)
            {
                var oms = _scheduleService.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = "all", Callerrequest = "", Callresponse = sresponse, Callerrequesttype = "GET", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));

            }
        }
    }
}
