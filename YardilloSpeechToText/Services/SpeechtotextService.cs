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
        private IMongoCollection<Speechtotextmap> _Speechtotextmap;
        private IMongoCollection<commonnamesmap> _commonnamesmap;
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
                _Speechtotextmap = TenantDatabase.GetCollection<Speechtotextmap>(_settings.SpeechToTextMAPCollection);
                _commonnamesmap = TenantDatabase.GetCollection<commonnamesmap>(_settings.CommonnamesCollection);

                _tenantid = tenantid;

                var indexKeysDefinition = Builders<Speechtotextmap>.IndexKeys.Ascending(hamster => hamster.Phrasetext);
                _Speechtotextmap.Indexes.CreateOneAsync(new CreateIndexModel<Speechtotextmap>(indexKeysDefinition));  


            }
            catch { throw; };
        }
       
        public Speechtotext GetTranscript(string id,Speechtotext otran,bool usecache)
        {
            StringBuilder slog = new StringBuilder();
            //Speechtotext osptotxt = new Speechtotext();
            string responseBody = string.Empty;
            int feedbackcount = 0;
            try
            {
                Speechtotext c = _speechtotextcollection.Find(c => c.TranId == id).FirstOrDefault();
                if (c == null)
                {
                    HttpClient _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("authorization", _assemblyai_server_token);

                    //var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(osptxt), Encoding.UTF8, "application/json");
                    string surl = _assemblyai_server_url + "/" + id;
                    using (HttpResponseMessage response = _client.GetAsync(surl).GetAwaiter().GetResult())
                    {
                        response.EnsureSuccessStatusCode();
                        slog.Append("Response: ");
                        responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        slog.Append(responseBody);
                    }


                    AAIResponse oRet = Newtonsoft.Json.JsonConvert.DeserializeObject<AAIResponse>(responseBody);
                    otran.Duration = oRet.audio_duration;
                    otran.TranId = id;
                    otran.audio_url = oRet.audio_url;

                    Getsuggestions(oRet, feedbackcount);
                    

                    otran.AIResponse = oRet;

                    _speechtotextcollection.InsertOneAsync(otran);

                    return otran;
                }

                if (usecache == false)
                {
                    AAIResponse oRet = c.AIResponse;
                    otran.Duration = oRet.audio_duration;
                    otran.TranId = id;
                    otran.audio_url = oRet.audio_url;
                    oRet.phrases = new List<phrase>();
                    Getsuggestions(oRet, feedbackcount);

                    otran.AIResponse = oRet;

                    otran._id = c._id;
                    _speechtotextcollection.ReplaceOne(ocase => ocase._id == c._id, otran);
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
        public AAIResponse PostSpeech(AAIRequest osptxt)
        {
            StringBuilder slog = new StringBuilder();
            Speechtotext osptotxt = new Speechtotext();
            string responseBody = string.Empty;
            try 
            {
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("authorization", _assemblyai_server_token);

                var serialized = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(osptxt), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = _client.PostAsync(_assemblyai_server_url, serialized).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    slog.Append("Response: ");
                      responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    slog.Append(responseBody);

                }

                AAIResponse oairesp = Newtonsoft.Json.JsonConvert.DeserializeObject<AAIResponse>(responseBody);
                //slog.AppendLine(",Message:" + omess._id + " " + omess.message_text + ", EMAIL SENT : " + ouser.authentication.email.email);
                return oairesp;
            }

            catch { throw; }
            finally {
               
            }

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

        public void Getsuggestions(AAIResponse oRet, int feedbackcount)
        {

            try
            {
                //sanatize the word
                string smatchword;//= oRet.text.Replace(".", "");

                //string sbegin = "\\b(?:";
                //string swords=string.Empty;
                foreach (owords ow in oRet.words)
                {
                    smatchword = ow.text.Replace(".","").TrimStart().TrimEnd();
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
                                owords sword= oRet.words.Find(x => x.text.ToUpper() == w.Phrasetext.ToUpper());
                                if (sword != null)
                                {
                                    //its a word
                                    if(sword.feedback != w.Feedbacktext)
                                    {
                                        sword.feedback = w.Feedbacktext;
                                        feedbackcount += 1;
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
                                    }
                                    
                                   
                                }

                                //get the common names
                                if (oRet.commonnames == null) { oRet.commonnames = new List<commonname>(); }
                                commonnamesmap commonname = _commonnamesmap.Find(c => c.name.ToUpper() == smatchword.ToUpper()).FirstOrDefault();
                                if (commonname != null)
                                {
                                    var ocomname = new commonname() { name = commonname.name, tipnote = commonname.tipnote};
                                    oRet.commonnames.Add(ocomname);
                                }

                            }
                        }
                    
                    }
                }

                oRet.feedbackcount = feedbackcount;
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
