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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Conversations.V1;
using Twilio.Types;
using TimeZoneConverter;

namespace MBADCases.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthenticationHandler()]
    public class mpclubController : Controller
    {
        //# MPCLUB DEV#
//        private const string _BubbleAPIHeaderAuth = "3b349a5c1fa7f1de7a8ec20e28886b6d";
//        private const string _BubbleAPIUserUrl = "https://alittlemore.love/api/1.1/obj/User"; //"https://alittlemore.love/version-test/api/1.1/obj/User";
//        private const string _BubbleAPIMessagesUrl = "https://alittlemore.love/api/1.1/obj/messages";
//        private const string _postmark_fromemail = "info@alittlemore.love";
//        private const string _postmark_subject = "Message with A Little More Love";
//        private const string _postmark_server_token = "69972700-ccae-45c5-ad98-0a76c5d436ab";//"9f7a3892-ec08-4f65-9597-0f4ba30dddc1";//
//        private const string _postmark_server_url = "https://api.postmarkapp.com/email";
//        private const string _Twilio_accountSid = "ACa7d5f9352223310828cefc8611ff9a49";//"AC19e6ab935a4a67ec96d0a22e946af13f";
//        private const string _Twilio_authToken = "bf1eacdcaec23034b3d9745a933da7ef";//"d3d3eb56bc448bd4be5be0c1472b89ad";
//        private const string _Twilio_MessagingServiceSid = "MG5caf3746ed0272abbf031d78ec399cda";//"MG5a5b683ce82dce34d0767afac46d665e"
//                                                                                                //#MPCLUB DEV END#
//        private const int _timer_minutes = 15;
//        ////#YARDILLO DEV#
//        //private const string _BubbleAPIHeaderAuth = "b37115857798bb37ef2e755833f51b26";
//        //private const string _BubbleAPIUserUrl = "https://wimsupapp.bubbleapps.io/version-test/api/1.1/obj/mpclubusers";
//        //private const string _BubbleAPIMessagesUrl = "https://wimsupapp.bubbleapps.io/version-test/api/1.1/obj/mpclubMessages";
//        //private const string _postmark_fromemail = "yardilloapi@gmail.com";
//        //private const string _postmark_server_token = "69972700-ccae-45c5-ad98-0a76c5d436ab";
//        //private const string _postmark_server_url = "https://api.postmarkapp.com/email";
//        //private const string _Twilio_accountSid = "AC19e6ab935a4a67ec96d0a22e946af13f"; 
//        //private const string _Twilio_authToken = "d3d3eb56bc448bd4be5be0c1472b89ad"; 
//        //private const string _Twilio_MessagingServiceSid = "MG5a5b683ce82dce34d0767afac46d665e"; 
//        ////#YARDILLO DEV END#
//        private mpclubresp omastermessage = new mpclubresp();
//        private int _totalusers;
//        private int _totalmessages;
//        private bool isexception = false;
//        [MapToApiVersion("1.0")]
//        [HttpPost()]
//        public IActionResult Post(string id)
//        {
           
//            StringBuilder slog = new StringBuilder();
//            slog.Append("Service started");
//            try
//            {
//               var timeZones = TimeZoneInfo.GetSystemTimeZones();

//                HttpClient _client = new HttpClient();
//                  _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _BubbleAPIHeaderAuth);
//                _client.DefaultRequestHeaders.Accept.Add(
//                  new MediaTypeWithQualityHeaderValue("application/json"));
//                //get users in the current timezone
               
//                StringBuilder slog1 = new StringBuilder();

//                string param = "";//"?constraints=[{\"key\":\"timezone_text\",\"constraint_type\": \"equals\", \"value\": \"" + timezone + "\"}]";
//                string susers = helperservice.GetRESTResponse("GET", _BubbleAPIUserUrl + param, "", _client, slog1);

//                 mpclubuser colusers = Newtonsoft.Json.JsonConvert.DeserializeObject<mpclubuser>(susers);

//                List<mpclubmessageResult> allmessages = new List<mpclubmessageResult>();

//                if (colusers != null) {

//                    if (colusers.response != null)
//                    {
//                        if (colusers.response.results != null)
//                        {
//                            if (colusers.response.count > 0)
//                            {
//                                List<mpclubuserResult> AllActiveUsers = colusers.response.results.FindAll(x => x.active1_boolean == true);

//                                var DistinctItems = AllActiveUsers.GroupBy(x => x.timezone_id_text).Select(y => y.First());
//                                foreach (var item in DistinctItems)
//                                {
                                    
//                                    string sFromdate = "";
//                                    TimeSpan start = new TimeSpan() ;
//                                    TimeSpan end =new TimeSpan();
//                                    // string sToDate = "";
//                                    string timezone = item.timezone_id_text;// "India Standard Time";
//                                    try
//                                    {
//                                        //if (timezone == "Eastern Daylight Time") { timezone = "America/New_York"; };
//                                        //if (timezone == "Brasilia Standard Time") { timezone = "America/Eirunepe"; };
//                                        //if (timezone == "Central Daylight Time") { timezone = "Central Standard Time"; };
//                                        //if (timezone == "Mountain Daylight Time") { timezone = "Mountain Standard Time"; };
//                                        //if (timezone == "Pacific Daylight Time") { timezone = "Pacific Standard Time"; };

//                                        TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo(timezone);
//                                        DateTime indianTime = TimeZoneInfo.ConvertTime(DateTime.Now,
//                                            TimeZoneInfo.FindSystemTimeZoneById(tzi.Id));
//                                        start = new TimeSpan(indianTime.Hour, indianTime.Minute  , 0);
//                                        end = new TimeSpan(indianTime.Hour, indianTime.Minute + _timer_minutes, 0);
//                                        slog.AppendLine(timezone);
//                                        slog.AppendLine(" current time:" + indianTime.ToString());
//                                        //var timezone2 = TimeZoneInfo.FindSystemTimeZoneById(timezone);

//                                        //var ctsDate = DateTime.Parse(indianTime.ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                                        //var offset = timezone2.GetUtcOffset(ctsDate);

//                                        //var utcDate = new DateTimeOffset(ctsDate, offset).ToUniversalTime().DateTime;
//                                        sFromdate = indianTime.ToString("MM/dd/yyyy hh:");
//                                        //sFromdate = indianTime.AddHours(4).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"); //DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")
//                                        // sToDate = indianTime.AddHours(4).AddMinutes(15).ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
//                                    }
//                                    catch (Exception e) { slog.AppendLine("Can not find timezone : " + timezone + " , exception:" + e.ToString()); }

//                                    if (sFromdate != "") {
//                                          _client = new HttpClient();
//                                        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _BubbleAPIHeaderAuth);
//                                        _client.DefaultRequestHeaders.Accept.Add(
//                                          new MediaTypeWithQualityHeaderValue("application/json"));
//                                        //get messages at this time
//                                        // param = "?constraints=[{\"key\":\"publish_text\",\"constraint_type\": \"greater than\", \"value\": \"" + sFromdate + "\"},{\"key\":\"publish_text\",\"constraint_type\": \"less than\", \"value\": \"" + sToDate + "\"}]";
//                                        param =   "?constraints=[{\"key\":\"publish_text\",\"constraint_type\": \"text contains\", \"value\": \"" + sFromdate + "\"}]";
//                                        slog.AppendLine(sFromdate);
//                                        string smessages = helperservice.GetRESTResponse("GET", _BubbleAPIMessagesUrl + param, "", _client, slog1);
//                                    mpclubmessage colmessages = Newtonsoft.Json.JsonConvert.DeserializeObject<mpclubmessage>(smessages);

//                                    if (colmessages!=null && colmessages.response.results != null)
//                                    {
//                                        foreach (mpclubmessageResult omess in colmessages.response.results)
//                                        { 
//                                                TimeSpan now = DateTime.Parse(omess.publish_text).TimeOfDay;
                                            
//                                              if(  comparetimespsn(start,end,now))
//                                                {
//                                                    omess.yardillo_location = timezone;
//                                                    allmessages.Add(omess);
//                                                }
//                                        }
//                                    }
//                                    }
//                                }
//                                //if messages are avaliable
//                                _totalmessages = allmessages.Count;
//                                if (allmessages.Count > 0)
//                                {
//                                    foreach (mpclubmessageResult omess in allmessages)
//                                    {
//                                       List<mpclubuserResult> allvalidmessageusers = AllActiveUsers.FindAll(x => x.timezone_id_text == omess.yardillo_location);
//                                        //all active users
//                                        foreach (mpclubuserResult u in allvalidmessageusers)
//                                        {
//                                            //get all messages if any
//                                            //this sends out message to paid user and message type is send to all paid
                                            
//                                            if (u.subscription_option_os_subscription_types.ToUpper() == "ALL THE LOVE" && (omess.sent_to_all_boolean==true))
//                                            {
                                             
//                                                slog.AppendLine(",Use case 1 : User : " + u._id + " " + u.subscription_option_os_subscription_types.ToUpper() + " , message send to all : " + omess.sent_to_all_boolean);
                                                
//                                                SendMessage(u, omess, slog);
//                                            }

//                                            else if (u.subscription_option_os_subscription_types.ToUpper() == "ALL THE LOVE" && (omess.sent_to_all_boolean == false))
//                                            {
//                                                slog.AppendLine(",Usecase 2 : User : " + u._id + " " + u.subscription_option_os_subscription_types.ToUpper() + " , message send to all : NULL, timezone : " + omess.yardillo_location);

//                                                //this is based on the sun 
//                                                if (omess.sun_signs_option_os_astro_signs != null ) {
//                                                    // slog.AppendLine(",Astro: " + u.astro_sign_text + " : " + omess.sun_sign_text);
//                                                    if (u.x_astro_sign_option_os_astro_signs != null) { 
//                                                        if (omess.sun_signs_option_os_astro_signs.ToUpper() == u.x_astro_sign_option_os_astro_signs.ToUpper())
//                                                        {
//                                                            slog.AppendLine(", Astro sign : " + u.x_astro_sign_option_os_astro_signs);
//                                                            SendMessage(u, omess, slog);
//                                                        }
//                                                    }
//                                                }
//                                                else if (omess.life_path_option_os_numerology_life__ != null ) 
//                                                {
//                                                    // slog.AppendLine(",LP: " + u.life_path_number_number + " : " + omess.life_path_number);
//                                                    if (u.life_path_number_option_os_numerology_life__ != null)
//                                                    {
//                                                        if (omess.life_path_option_os_numerology_life__ == u.life_path_number_option_os_numerology_life__)
//                                                        {
//                                                            slog.AppendLine(", LP : " + u.life_path_number_option_os_numerology_life__);
//                                                            SendMessage(u, omess, slog);
//                                                        }
//                                                    }
//                                                }
//                                              else if (omess.mb_type1_option_myers_briggs_types != null ) {
//                                                    //slog.AppendLine(",MB: " + u.myers_briggs_type_text + " : " + omess.message_text);
//                                                    if (u.myers_briggs_option_myers_briggs_types != null) { 
//                                                    if (u.myers_briggs_option_myers_briggs_types.ToUpper() == omess.mb_type1_option_myers_briggs_types.ToUpper())
//                                                        {
//                                                            slog.AppendLine(", MB : " + u.myers_briggs_option_myers_briggs_types);

//                                                            SendMessage(u, omess, slog);
//                                                        }
//                                                    }
//                                                }
//                                               else if (omess.enneagram_type_option_os_enneagram_types != null) {
//                                                    //slog.AppendLine(",Enn: " + u.enneagram_number_text + " : " + omess.enneagram_text);
//                                                    if (u.enneagram_number_option_os_enneagram_types != null) { 
//                                                       if (u.enneagram_number_option_os_enneagram_types.ToUpper() == omess.enneagram_type_option_os_enneagram_types.ToUpper())
//                                                        {
//                                                            slog.AppendLine(", En : " + u.enneagram_number_option_os_enneagram_types);
//                                                            SendMessage(u, omess, slog);
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    slog.AppendLine(", Alert : " + u.enneagram_number_option_os_enneagram_types);
//                                                    SendMessage(u, omess, slog);
//                                                }
//                                            }
//                                            else if (u.subscription_option_os_subscription_types.ToUpper() == "FREE LOVE" &&  (omess.sent_to_all_boolean==true))
//                                            {
//                                                slog.AppendLine(",Usecase 3 : User : " + u._id + " " + u.subscription_option_os_subscription_types.ToUpper() + " , message send to all : Free ");

//                                                SendMessage(u, omess, slog);
//                                            }
//                                        }
//                                    }
                                   
                                   
//                                }
                              
//                            }
//                        }
//                    }
//                }

//                slog.AppendLine(",Service Ended");
//                mpclubresp ompcl = new mpclubresp();
//                ompcl.slog = slog.ToString();
//                omastermessage.slog = slog.ToString();
//                //SendLogToMpclub();
//                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ompcl);
//            }
//            catch (Exception ex)
//            {
//                isexception = true;
//                slog.AppendLine(",Service Exception: ");
//                slog.AppendLine(ex.ToString());
//                omastermessage.slog = slog.ToString();
//                //SendLogToMpclub();
//                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, slog.ToString());
//            }
//        }
//       private bool comparetimespsn(TimeSpan start,TimeSpan end, TimeSpan now)
//        {
//            if (start < end) { return start <= now && now <= end; } else { return !(end <= now && now <= start); }
//        }
//        private void SendMessage(mpclubuserResult ouser, mpclubmessageResult omess, StringBuilder slog)
//        {
//            try
//            {
//                omastermessage.SendMessageTo.Add(new SendMessage() { user = ouser, message = omess });

//                if (ouser.delivery_method_option_os_delivery_method.ToUpper() == "EMAIL")
//                {
//                    //send an email this message to this user
//                    //string sbody;

//                    PostmarkEmail opostmaremail = new PostmarkEmail();
                                   

//                    opostmaremail.From = _postmark_fromemail;
//                    opostmaremail.To = ouser.authentication.email.email;
//                    opostmaremail.Subject = _postmark_subject;
//                    opostmaremail.TextBody = omess.message_text;
//                    opostmaremail.HtmlBody = "" + omess.message_text + "<br/><br/><br/>Sent from and with <a href='https://alittlemore.love/' target='_blank'>A Little More Love</a>";
//                    opostmaremail.MessageStream = "broadcast";
//                    opostmaremail.TrackOpens = true;

//                    HttpClient _client = new HttpClient();
//                    _client.DefaultRequestHeaders.Add("X-Postmark-Server-Token", _postmark_server_token);
                                   
//                    var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(opostmaremail), Encoding.UTF8, "application/json");

//                    using (HttpResponseMessage response = _client.PostAsync(_postmark_server_url, serialized).GetAwaiter().GetResult())
//                    {
//                        response.EnsureSuccessStatusCode();
//                        slog.Append("Response: ");
//                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
//                        slog.Append(responseBody);
                         
//                    }

//                    slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", EMAIL SENT : " + ouser.authentication.email.email);
//                }
//                else if (ouser.delivery_method_option_os_delivery_method.ToUpper().Contains("TEXT"))
//                {
//                    try
//                    {
//                        string tonumber = ouser.phone_for_text_delivery_text;
//                        if (ouser.phone_for_text_delivery_text.StartsWith("+"))
//                        {
//                            tonumber = ouser.phone_country_code_text + ouser.phone_for_text_delivery_text;
//                        }
//                        else
//                        {
//                            tonumber = "+" + ouser.phone_country_code_text +  ouser.phone_for_text_delivery_text;
//                        }
//                        // string fromnumber = "+13603585357"; //get the number from mpclub
//                        //var accountSid = "ACa7d5f9352223310828cefc8611ff9a49";//"AC19e6ab935a4a67ec96d0a22e946af13f";
//                        //var authToken = "bf1eacdcaec23034b3d9745a933da7ef";//"d3d3eb56bc448bd4be5be0c1472b89ad";
//                        TwilioClient.Init(_Twilio_accountSid, _Twilio_authToken);

//                        var messageOptions = new CreateMessageOptions(
//                            new PhoneNumber(tonumber));
//                        messageOptions.MessagingServiceSid = _Twilio_MessagingServiceSid;// "MG5caf3746ed0272abbf031d78ec399cda";// "MG5a5b683ce82dce34d0767afac46d665e";

//                        DateTime convertedDate = DateTime.Parse(omess.publish_text.ToString());
//                        DateTime localDate = convertedDate.ToLocalTime();

//                        messageOptions.Body = omess.message_text; // + ", Pub date: " + omess.publish_text + ", timezone: " + omess.yardillo_location;

//                        var message = MessageResource.Create(messageOptions);
                       
//                        slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", SMS SENT " + messageOptions.Body);
//                    }
//                    catch (Exception e)
//                    {
//                        slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", SMS FAILED " + ouser.phone_for_text_delivery_text + " ," + e.ToString());
//                    }


//                }
//                else if (ouser.delivery_method_option_os_delivery_method.ToUpper() == "WHATSAPP")
//                {
//                    try
//                    {
//                        //// Find your Account SID and Auth Token at twilio.com/console
//                        //// and set the environment variables. See http://twil.io/secure
//                        //string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
//                        //string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

//                        //TwilioClient.Init(accountSid, authToken);

//                        //var conversation = ConversationResource.Create(
//                        //    messagingServiceSid: "MGXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
//                        //    friendlyName: "Friendly Conversation"
//                        //);

//                        //var accountSid = "ACa7d5f9352223310828cefc8611ff9a49";//"AC19e6ab935a4a67ec96d0a22e946af13f";
//                        //var authToken = "bf1eacdcaec23034b3d9745a933da7ef";//"d3d3eb56bc448bd4be5be0c1472b89ad";
//                        TwilioClient.Init(_Twilio_accountSid, _Twilio_authToken);
//                        string tonumber = ouser.phone_for_text_delivery_text;
//                        if (ouser.phone_for_text_delivery_text.StartsWith("+"))
//                        {
//                            tonumber = ouser.phone_country_code_text + ouser.phone_for_text_delivery_text;
//                        }
//                        else
//                        {
//                            tonumber = "+" + ouser.phone_country_code_text + ouser.phone_for_text_delivery_text;
//                        }
//                        var messageOptions = new CreateMessageOptions(
//                            new PhoneNumber("whatsapp:" + tonumber));
//                        messageOptions.From = new PhoneNumber("whatsapp:" + "+17372379901");

//                        messageOptions.MessagingServiceSid = _Twilio_MessagingServiceSid;

//                        messageOptions.Body = omess.message_text;

//                        var message = MessageResource.Create(messageOptions);
//                    }
//                    catch (Exception e)
//                    {
//                        slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", WhatsAPP FAILED " + ouser.phone_for_text_delivery_text + " ," + e.ToString());
//                    }
//                }

                
//            }

//            catch (Exception ex)
//            {
//                isexception = true;
//                slog.AppendLine("Exception sending message: " + ex.ToString());
//            }
//        }

//        private void SendLogToMpclub()
//        {
//            try
//            {

//                string sblob = Newtonsoft.Json.JsonConvert.SerializeObject(omastermessage);
//                var _client = new HttpClient();

//                _client.DefaultRequestHeaders.Add("x-rapidapi-host", "yardillo.p.rapidapi.com");
//                _client.DefaultRequestHeaders.Add("x-rapidapi-key", "36d9952f5bmshdde26829370d654p1d5a5cjsnad7db9145569");
//                _client.DefaultRequestHeaders.Add("X-RapidAPI-Proxy-Secret", "6acc1280-fde1-11eb-b480-3f057f12dc26");
//                _client.DefaultRequestHeaders.Add("X-RapidAPI-User", "yardilloapi@gmail.com");
//                _client.DefaultRequestHeaders.Add("Y-Auth-Src", "yardillo");
//                _client.DefaultRequestHeaders.Add("Accept", "application/json");
//                //_client.DefaultRequestHeaders.Add("Content-Type", "application/json");
//                string Casetitle = "Success";
//                if(isexception == true) { Casetitle = "Exception"; }
                
//                Case ocase = new Case() { Casetype = "mpclub", Blob = sblob, Casetitle= Casetitle  };
//                //string strcase = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);
//                StringBuilder slog = new StringBuilder();
//               // helperservice.GetRESTResponse("put","https://yardillo.azurewebsites.net/V1/case/mpclub", strcase, _client,slog);

                       

//                var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(ocase), Encoding.UTF8, "application/json");

//                using (HttpResponseMessage response = _client.PutAsync("https://yardillo.azurewebsites.net/V1/case/mpclub", serialized).GetAwaiter().GetResult())
//                {
//                    response.EnsureSuccessStatusCode();
//                    slog.Append("Response: ");
//                    string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
//                    slog.Append(responseBody);

//                }
//            }
//            catch (Exception ex)
//            {
//                isexception = true;
//                string s = ex.ToString();
//            }

//        }
//}
//    public   class mpclubresp
//    {
//     public   mpclubresp()
//        {
//            SendMessageTo = new List<SendMessage>();
          
//        }
//        public   string slog { get; set; }
//        public List<SendMessage> SendMessageTo { get; set; }
  
       
//    }
//    public class SendMessage
//    {
//        public mpclubuserResult user { get; set; }
//        public mpclubmessageResult message { get; set; }
//    }
}
