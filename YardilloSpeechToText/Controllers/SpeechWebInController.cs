using MBADCases.Models;
using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YardilloSpeechToText.Models;

namespace YardilloSpeechToText.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class SpeechWebInController : Controller
    {
        private readonly SpeechtotextService _speechtotextservice;

        public SpeechWebInController(SpeechtotextService speechtotextserviceervice)
        {
            _speechtotextservice = speechtotextserviceervice;
        }
        [MapToApiVersion("1.0")]
        [HttpPost(Name = "Getwebin")]
        public IActionResult Post(Speechwebhook id )
        {
            try
            {
               if( id.status == "completed")
                {
                    commonaispeechid objid = _speechtotextservice.GettenantbyAIId(id.transcript_id);

                }

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "");
            }
            catch(Exception e)
            {
                Message oms;
                oms = new Message();
                oms.MessageDesc = e.Message;

               
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, "");
                
            }
            
        }
    }
    
}
