using MBADCases.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using YardilloSpeechToText.Models;

namespace MBADCases.Services
{
    public class SpeechtotextService
    {
        private IMongoCollection<Speechtotext> _speechtotextcollection;
        private IMongoCollection<Speechtotextattr> _speechtotextattrcollection;
        private IMongoCollection<Speechtotextmap> _Speechtotextmap;
        private IMongoCollection<commonnamesmap> _commonnamesmap;
        private IMongoCollection<commonaispeechid> _commonaispeechid;
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        private string _tenantid;
        private const string _assemblyai_server_token = "5553a2488b5a4d8db6b18a055143a880";
        private const string _assemblyai_server_url = "https://api.assemblyai.com/v2/transcript";
        public SpeechtotextService(ICasesDatabaseSettings settings)
        {
            try
            {
                _settings = settings;
                _client = new MongoClient(settings.ConnectionString);
                MBADDatabase = _client.GetDatabase(settings.DatabaseName);
            }
            catch { throw; }
        }
        public void Gettenant(string tenantid)
        {
            try
            {
                TenantDatabase = helperservice.Gettenant(tenantid, _client, MBADDatabase, _settings);
                _speechtotextcollection = TenantDatabase.GetCollection<Speechtotext>(_settings.SpeechToTextCollection);
                _speechtotextattrcollection = TenantDatabase.GetCollection<Speechtotextattr>(_settings.SpeechToTextAttrCollection);
                _Speechtotextmap = TenantDatabase.GetCollection<Speechtotextmap>(_settings.SpeechToTextMAPCollection);
                _commonnamesmap = MBADDatabase.GetCollection<commonnamesmap>(_settings.CommonnamesCollection);
                _commonaispeechid = MBADDatabase.GetCollection<commonaispeechid>(_settings.CommonaispeechidCollection);
                _tenantid = tenantid;

                var indexKeysDefinition = Builders<Speechtotextmap>.IndexKeys.Ascending(hamster => hamster.Phrasetext);
                _Speechtotextmap.Indexes.CreateOneAsync(new CreateIndexModel<Speechtotextmap>(indexKeysDefinition));  


            }
            catch { throw; };
        }
       
        public commonaispeechid GettenantbyAIId(string tranid)
        {
            _commonaispeechid = MBADDatabase.GetCollection<commonaispeechid>(_settings.CommonaispeechidCollection);
            commonaispeechid c = _commonaispeechid.Find(c => c.Tranid == tranid).FirstOrDefault();
            if (c == null) { throw new Exception("Invalid _id"); }
            return c;
        }
        public Speechtotext GetTranscript(string id,Speechtotext otran,bool usecache)
        {
            StringBuilder slog = new StringBuilder();
            //Speechtotext osptotxt = new Speechtotext();
            string responseBody = string.Empty;
            int feedbackcount = 0;
            //int totalfeedbackcount = 0;
            try
            {
                Speechtotext c = _speechtotextcollection.Find(c => c._id== id).FirstOrDefault();
                if (c == null) { throw new Exception("Invalid _id"); }
                if (c.Status.ToLower() != "completed")
                {
                    AAIResponse oRet = new AAIResponse();

                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("authorization", _assemblyai_server_token);

                    //var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(osptxt), Encoding.UTF8, "application/json");
                    string surl = _assemblyai_server_url + "/" + c.TranId;

                    try
                    {

                    using (HttpResponseMessage response = _client.GetAsync(surl).GetAwaiter().GetResult())
                    {
                        response.EnsureSuccessStatusCode();
                        slog.Append("Response: ");
                        responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        slog.Append(responseBody);
                    }
                    }
                    catch(Exception eee)
                    {
                        oRet.feedback = new List<feedback>();
                        oRet.commonnames = new List<commonname>();
                        oRet.phrases = new List<phrase>();
                        oRet.words = new List<owords>();
                        otran.Duration = 0;
                        otran.Status = "error";
                        otran.TranId = c.TranId;
                        otran.audio_url = oRet.audio_url;
                        otran.error = eee.ToString();
                        oRet.status = "error";
                        oRet.error = eee.ToString();
                        return otran;
                    }
                    
                    if (responseBody!=null || responseBody != "")
                    {                      
                        try
                        {
                            oRet = Newtonsoft.Json.JsonConvert.DeserializeObject<AAIResponse>(responseBody);
                            otran.Duration = oRet.audio_duration;
                            otran.TranId = oRet.id;
                            otran.audio_url = oRet.audio_url;
                            otran.Status = oRet.status;
                            oRet.error = oRet.error;
                            oRet.confidence = oRet.confidence;
                        }
                        catch 
                        {
                            oRet.feedback = new List<feedback>();
                            oRet.commonnames = new List<commonname>();
                            oRet.phrases = new List<phrase>();
                            oRet.words = new List<owords>();
                            AAIErrorResponse oerr = Newtonsoft.Json.JsonConvert.DeserializeObject<AAIErrorResponse>(responseBody);
                            otran.Duration = 0;
                            otran.Status = oerr.status;
                            otran.TranId = c.TranId;
                            otran.audio_url = oRet.audio_url;
                            otran.error = oerr.error;
                            oRet.status = oerr.status;
                            oRet.error = oerr.error;
                            return otran;
                        }   
                    }
                    else
                    {
                        oRet.feedback = new List<feedback>();
                        oRet.commonnames = new List<commonname>();
                        oRet.phrases = new List<phrase>();
                        oRet.words = new List<owords>();
                        otran.Duration = 0;
                        otran.Status = "error";
                        otran.TranId = c.TranId;
                        otran.audio_url = oRet.audio_url;
                        otran.error = "No response from AI CODE:000001";
                        oRet.status = "error";
                        oRet.error = "No response from AI CODE:000001";
                        return otran;
                    }
                    if (oRet.status == "completed")
                    {
                        oRet.feedback = new List<feedback>();
                        oRet.commonnames = new List<commonname>();
                        oRet.phrases = new List<phrase>();
                        oRet.highlitedtext = oRet.text;
                        if (otran.usage_count_feedback == 0) { c.usage_count_feedback = -1; } //keep zero no charge first time
                         
                        if (otran.usage_count_audio == 0) { c.usage_count_audio = 1; }
                        Getsuggestions(oRet, feedbackcount);
                    }
                   
                    otran.AIResponse = oRet;
                    otran._id = c._id;
                    _speechtotextcollection.ReplaceOne(ocase => ocase._id == c._id, otran);
                    return otran;
                }

                if (usecache == false)
                {
                    AAIResponse oRet = c.AIResponse;
                    otran.Duration = oRet.audio_duration;
                    otran.TranId = c.TranId;
                    otran.audio_url = oRet.audio_url;
                    
                    if (oRet.status == "completed")
                    {
                        if (c.usage_count_feedback == 0) { c.usage_count_feedback = 1; }
                        otran.usage_count_feedback += c.usage_count_feedback;
                        if (c.usage_count_audio == 0) { c.usage_count_audio = 1; }
                        Getsuggestions(oRet, feedbackcount);
                        otran.usage_count_feedback  = c.usage_count_feedback +1;
                    }

                    otran.AIResponse = oRet;

                    otran._id = c._id;
                    otran.TranId = c.TranId;
                    otran.Createdate = c.Createdate;
                    otran.Createdate = c.Createuser;
                    otran.Status = c.Status;
                    otran.error = c.error;
                    otran.usage_count_audio = c.usage_count_audio;
                   
                    if (oRet.status == "completed")
                    {
                        _speechtotextcollection.ReplaceOne(ocase => ocase._id == c._id, otran);
                    }
                    return otran;
                }
                return c;
            }
            catch { throw; }
            finally
            {

            }
        }
        public void WriteDurationLog(Speechtotext slog)
        {
            Speechtotext c = _speechtotextcollection.Find(c => c.TranId.ToUpper() == slog.TranId.ToUpper()).FirstOrDefault();
            if (c != null)
            {
                //update
                slog._id = c._id;
                _speechtotextcollection.ReplaceOne(ocase => ocase._id == c._id, slog);

                
            }
            else
            {
                _speechtotextcollection.InsertOne(slog);
            }

         
           
        }
        public Speechtotext PostSpeech(AAIRequest osptxt,Speechtotext ocase, bool usecache, string usrid)
        {
            StringBuilder slog = new StringBuilder();
            Speechtotext osptotxt = new Speechtotext();
            string responseBody = string.Empty;
             
            try 
            {
                Speechtotext otran = new Speechtotext();
                if (usecache == true)
                {
                    Speechtotext c = _speechtotextcollection.Find(c => c.audio_url.ToUpper() == ocase.audio_url.ToUpper()).FirstOrDefault();
                    if (c == null) { usecache = false; }
                    else { otran = c; }
                }

                if (usecache == false)
                {

                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("authorization", _assemblyai_server_token);
                    if (osptxt.audio_url == null) { throw new Exception("audio_url can not be blank."); }
                    if (osptxt.audio_url.StartsWith("//") == true) { osptxt.audio_url = "https:" + osptxt.audio_url; }
                    var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(osptxt), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = _client.PostAsync(_assemblyai_server_url, serialized).GetAwaiter().GetResult())
                    {
                        response.EnsureSuccessStatusCode();
                        slog.Append("Response: ");
                        responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        slog.Append(responseBody);

                    }

                    AAITranscriptResponse oairesp = Newtonsoft.Json.JsonConvert.DeserializeObject<AAITranscriptResponse>(responseBody);
                    bool alldefault = false;
                    Speechtotextattr oattrb = new Speechtotextattr();
                    oattrb.highlite_start_tag = ocase.highlite_start_tag;
                    oattrb.highlite_end_tag = ocase.highlite_end_tag;
                    SetDefaultAttr(oattrb, alldefault);

                    otran.TranId = oairesp.id;
                    otran.Status = oairesp.status;
                    otran.AIResponse = new AAIResponse();
                    otran.audio_url = ocase.audio_url;
                    otran.webhook_url = ocase.webhook_url;
                    otran.highlite_start_tag = oattrb.highlite_start_tag;
                    otran.highlite_end_tag = oattrb.highlite_end_tag;
                    otran.usage_count_audio = 1;
                    otran.usage_count_feedback = 0;
                    _speechtotextcollection.InsertOneAsync(otran);

                    commonaispeechid commonspeechid = new commonaispeechid();
                    commonspeechid.Sourceid = "AssemblyAI";
                    commonspeechid.Speechid = otran._id;
                    commonspeechid.Tenantid = _tenantid;
                    commonspeechid.Tranid = otran.TranId;
                    commonspeechid.Createdate = DateTime.UtcNow.ToString();
                    commonspeechid.Createuser = usrid;
                    _commonaispeechid.InsertOneAsync(commonspeechid);



                }

                //oairesp._id = otran._id; ;

                //slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", EMAIL SENT : " + ouser.authentication.email.email);
                return otran;
            }

            catch { throw; }
            finally {
               
            }

        }
        private void SetDefaultAttr(Speechtotextattr oattr, bool alldefault)
        {
            if (oattr.highlite_start_tag == null || oattr.highlite_start_tag == "") { oattr.highlite_start_tag = "[color =#FF5733][b][i]"; alldefault = true; }
            if (oattr.highlite_end_tag == null || oattr.highlite_end_tag == "") { oattr.highlite_end_tag = "[/i][/b][/color]"; alldefault = true; }
        }
        public Speechtotextmap Create(Speechtotextmap ocasetype)
        {
            try
            {
                //check if unique
                Speechtotextmap c = _Speechtotextmap.Find(c => c.Phrasetext.ToUpper() == ocasetype.Phrasetext.ToUpper()).FirstOrDefault();
                if (c != null) {
                    //update
                    ocasetype._id = c._id;
                    _Speechtotextmap.ReplaceOne(ocase => ocase._id == c._id, ocasetype); 
                    
                    return ocasetype;
                }
             
                if (ocasetype.Createdate == null || ocasetype.Createdate == "") { ocasetype.Createdate = DateTime.UtcNow.ToString(); }
                if (ocasetype.Updatedate == null || ocasetype.Createdate == "") { ocasetype.Updatedate = DateTime.UtcNow.ToString(); }

                _Speechtotextmap.InsertOne(ocasetype);
                return ocasetype;
            }
            catch
            {
                throw;
            }

        }

        public void Getsuggestions(AAIResponse oRet, int feedbackcount )
        {
            int totaloccurances = 0;
            try
            {
               
                if (oRet.words == null) { throw new Exception("Transcription failed or file is empty. " + oRet.error); };
                
                bool alldefault = false;
                //string highlite_start_tag = "";
                //string highlite_end_tag = "";
                Speechtotextattr objattr = _speechtotextattrcollection.Find(a => a.TranId == oRet.id).FirstOrDefault();
                if (objattr == null) { objattr = new Speechtotextattr(); }
                
                    SetDefaultAttr(objattr, alldefault);
               
                //objattr.highlite_start_tag = highlite_end_tag;
                //objattr.highlite_end_tag = highlite_end_tag;

                //sanatize the word
                string smatchword;//= oRet.text.Replace(".", "");
                //string sbegin = "\\b(?:";
                //string swords=string.Empty;
                foreach (owords ow in  oRet.words)
                {
                    smatchword = ow.text.Replace(".","").Replace(",","").TrimStart().TrimEnd();
               //     swords =   swords + "|" + ow.text;              
               //string  sregex = sbegin + swords + ")\\b";
                //var gmailFilter = Builders<Speechtotextmap>.Filter.Regex(u => u.Phrasetext, new BsonRegularExpression(sregex));
                //var c = _Speechtotextmap.Find(u => u.Phrasetext.ToUpper()==ow.text.ToUpper()).ToList<Speechtotextmap>();
             List<Speechtotextmap> c = _Speechtotextmap.Find(c => c.Phrasetext.ToUpper().Contains( smatchword.ToUpper())).ToList<Speechtotextmap>();
                    if (c != null)
                    {
                        foreach(var w in c)
                        {
                            if(oRet.text.Contains(w.Phrasetext, StringComparison.OrdinalIgnoreCase))
                            {
                               List<owords> tempwords =  oRet.words;
                                owords sword= tempwords.Find(x => x.text.ToUpper() == w.Phrasetext.ToUpper());
                                if (sword != null)
                                {
                                    //its a word
                                    if(sword.feedback != w.Feedbacktext)
                                    {
                                        sword.feedback = w.Feedbacktext;
                                        feedbackcount += 1;
                                        oRet.highlitedtext = oRet.highlitedtext.Replace(sword.text, objattr.highlite_start_tag + sword.text + objattr.highlite_end_tag);

                                        oRet.feedback.Add(new feedback { confidence=sword.confidence, start=sword.start,end=sword.end, phrase= sword.text, text= sword.feedback });
                                    }                                 
                                }
                                else
                                {
                                    //phrase and add it
                                    if (oRet.phrases == null) { oRet.phrases = new List<phrase>(); }
                                    int startpos =oRet.text.IndexOf(w.Phrasetext);
                                    int endpos = startpos + w.Phrasetext.Length;
                                    var ophrase = new phrase() { text = w.Phrasetext, feedback = w.Feedbacktext, confidence = 0.9998, start =  startpos,end=endpos, occurances=1 };
                                   var oexistingphrase= oRet.phrases.Find(f => f.text.ToUpper() == w.Phrasetext.ToUpper());
                                    if (oexistingphrase== null)
                                    {
                                        oRet.phrases.Add(ophrase);
                                        feedbackcount +=1;
                                        totaloccurances += 1;
                                        oRet.highlitedtext = oRet.highlitedtext.Replace(w.Phrasetext, objattr.highlite_start_tag + w.Phrasetext + objattr.highlite_end_tag);
                                        oRet.feedback.Add(new feedback { text = w.Feedbacktext, phrase = w.Phrasetext, confidence = 0.9998, start = startpos, end = endpos, occurances = 1 });
                                    }
                                    else
                                    {
                                        oexistingphrase.occurances += 1;
                                        totaloccurances += 1;
                                    } 
                                }
                            }                           
                        }
                        //get the common names
                        if (oRet.commonnames == null) { oRet.commonnames = new List<commonname>(); }
                        commonnamesmap commonname = _commonnamesmap.Find(c => c.name.ToUpper() == smatchword.ToUpper()).FirstOrDefault();
                        if (commonname != null)
                        {
                            var ocomname = new commonname() { name = commonname.name, tip = commonname.tip,occurances=1 };
                            var oexistingcomname = oRet.commonnames.Find(f => f.name.ToUpper() == smatchword.ToUpper());
                            if (oexistingcomname == null)
                            {
                                oRet.commonnames.Add(ocomname);
                                feedbackcount += 1;
                                totaloccurances += 1;

                            }
                            else
                            {
                                oexistingcomname.occurances += 1;
                                feedbackcount += 1;
                                totaloccurances += 1;
                            }
                            
                        }
                    }
                }

                
                oRet.feedbackcount  = feedbackcount;
             
                oRet.totalfeedbackcount = totaloccurances;
            }
            catch
            {
                throw;
            }
        }
        public void Getsuggestions( owords ow,int feedbackcount)
        {
            
            try
            {
                //sanatize the word
                string smatchword = ow.text.Replace(".","");
                Speechtotextmap c = _Speechtotextmap.Find(c => c.Phrasetext.ToUpper() == smatchword.ToUpper()).FirstOrDefault();
                if (c != null)
                {
                   ow.feedback= c.Feedbacktext;
                    feedbackcount = +1;
                }                
            }
            catch
            {
                throw;
            }
        }
        public Message SetMessage(string callrtype, string caseid, string srequest, string srequesttype, string sMessageCode, string sMessagedesc, string userid, Exception ex)
        {

            var _MessageType = string.Empty;
            var _MessageCode = string.Empty;
            var _MessageDesc = string.Empty;
            if (ex != null)
            {
                _MessageType = "ERROR";
                _MessageCode = ex.Message;
                _MessageDesc = ex.ToString();
            }
            else
            {
                _MessageType = "INFO";
                _MessageCode = sMessageCode;
                _MessageDesc = sMessagedesc;
            }
            Message oms = new Message
            {
                Tenantid = _tenantid,
                Callerid = caseid,
                Callertype = callrtype,
                Messagecode = _MessageCode,
                Messageype = _MessageType,
                MessageDesc = _MessageDesc,
                Callerrequest = srequest,
                Callerrequesttype = srequesttype,
                Userid = userid,
                Messagedate = DateTime.UtcNow.ToString()
            };

            MessageService omesssrv = new MessageService(_settings, TenantDatabase, MBADDatabase);
            oms = omesssrv.Create(oms);

            return oms;

        }
        public Message SetMessage(Message oms)
        {

            MessageService omesssrv = new MessageService(_settings, TenantDatabase, MBADDatabase);
            oms = omesssrv.Create(oms);

            return oms;

        }
      
    }
    public class SpeechtotextmapResponse
    {
        public SpeechtotextmapResponse(Speechtotextmap ocase, Message oms)
        {
            if (ocase != null) { _id = ocase._id; } else { oms.Messagecode = "417"; }

            //Casenumber = ocase.Casenumber;
            Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id, Messagedesc = oms.MessageDesc };
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public MessageResponse Message { get; set; }
    }
}
