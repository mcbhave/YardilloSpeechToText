using MBADCases.Authentication;
using MBADCases.Models;
using MBADCases.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class SpeechController : ControllerBase
    {
        private readonly SpeechtotextService _speechtotextservice;
        public SpeechController(SpeechtotextService  speechtotextserviceervice)
        {
            _speechtotextservice = speechtotextserviceervice;
        }

        [MapToApiVersion("1.0")]
        [HttpPost(Name = "SpeechToText")]
        public IActionResult Post(Speechtotext ocase)
        {
            Message oms;
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            var usrid = HttpContext.Session.GetString("mbaduserid");

            string sj = ocase.ToJson();
            ocase.Updateuser = usrid;
            //string id = ocase._id;
            //ocase._id = id;

            AAIRequest oaireq = new AAIRequest() { audio_url = ocase.audio_url };
            try
            {
                _speechtotextservice.Gettenant(tenantid);

              AAIResponse oRet=  _speechtotextservice.PostSpeech(oaireq);

                SppechToTextResponse osp = new SppechToTextResponse();
                osp._id = oRet.id;
                osp.status = oRet.status;
                
                oms = _speechtotextservice.SetMessage(ICallerType.CASE, "", sj, "POST", "UPDATE", "Case update", usrid, null);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osp);
            }
            catch (Exception ex)
            {
                oms = new Message();
                oms.MessageDesc = ex.ToString();
                oms = _speechtotextservice.SetMessage(oms);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ocase._id, oms));
            }

        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id}", Name = "getTranscript")]
        public IActionResult Get(string id )
        {
            string usecache = HttpContext.Request.Query["usecache"];

            Message oms;
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var yathuiden = HttpContext.Session.GetString("yathuid");
          
            string yathuidde = string.Empty;
            if (yathuiden != null)
            {
                helperservice.VaultCrypt ovrcr = new helperservice.VaultCrypt("Y-Auth-userid");
                yathuidde=  ovrcr.Decrypt(yathuiden) ;
            }
           
            //string sj = ocase.ToJson();
            //ocase.Updateuser = usrid;
            //string id = ocase._id;
            //ocase._id = id;

            try
            {
                _speechtotextservice.Gettenant(tenantid);

                Speechtotext otran = new Speechtotext();
                if (yathuidde == "")
                {
                    yathuidde = usrid;
                }
                otran.Createuser = yathuidde;
                otran.Createdate = DateTime.UtcNow.ToString();

                bool ucache=  true;
                if (usecache!=null && usecache.ToLower() == "false")
                {
                    ucache = false;
                }

                Speechtotext oRet = _speechtotextservice.GetTranscript(id, otran, ucache);
                              
                //_speechtotextservice.WriteDurationLog(otran);

                SppechToTextResponse osp = new SppechToTextResponse();
                osp._id = oRet.AIResponse.id;
                osp.status = oRet.AIResponse.status;
                osp.text = oRet.AIResponse.text;

                osp.words = oRet.AIResponse.words;
                osp.phrases = oRet.AIResponse.phrases;
                osp.feedbackcount = oRet.AIResponse.feedbackcount ;
                osp.audio_duration = oRet.AIResponse.audio_duration;
                oms = _speechtotextservice.SetMessage(ICallerType.CASE, "", "", "POST", "UPDATE", "SpeechtoText", usrid, null);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osp);
            }
            catch (Exception ex)
            {
                oms = new Message();
                oms.MessageDesc = ex.ToString();
                oms = _speechtotextservice.SetMessage(oms);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed,   oms );
            }

        }

        [HttpPut("{SpeechMap}")]
        public IActionResult Put(Speechtotextmap ocasetype)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {
                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(ocasetype);
                _speechtotextservice.Gettenant(tenantid);
                ocasetype.Createuser = usrid;
                ocasetype.Updateuser = usrid;
                ocasetype.Createdate = DateTime.UtcNow.ToString();
                ocasetype.Updatedate = DateTime.UtcNow.ToString();
                var oretcase = _speechtotextservice.Create(ocasetype);

                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oretcase);
                var oms = _speechtotextservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = oretcase._id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "PUT", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new SpeechtotextmapResponse(ocasetype, oms));
            }
            catch (Exception ex)
            {
                var oms = _speechtotextservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = "speechtotext", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "PUT", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new SpeechtotextmapResponse(ocasetype, oms));
            }
        }

        //[HttpDelete("{id:length(24)}")]
        //public IActionResult Delete(string id)
        //{
        //    string usrid = HttpContext.Session.GetString("mbaduserid");
        //    string tenantid = HttpContext.Session.GetString("mbadtanent");
        //    string srequest = "";
        //    string smessage = "";
        //    string sresponse = "";
        //    try
        //    {
        //        srequest = id;
        //        _casetypeservice.Gettenant(tenantid);
        //        _casetypeservice.Remove(id);

        //        var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status200OK", Messagecode = "200", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "DELETE", Callertype = "CASETYPE", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
        //        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseTypeResponse(null, oms));
        //    }
        //    catch (Exception ex)
        //    {
        //        var oms = _casetypeservice.SetMessage(new Message() { Messageype = "Status417ExpectationFailed", Messagecode = "417", Callerid = id, Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "DELETE", Callertype = "CASETYPE", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });
        //        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(null, oms));
        //    }
        //}
    }
   
}
