using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MBADCases.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MBADCases.Services
{
    public class CaseTypeService
    {
        private IMongoCollection<CaseType> _casetypecollection;
        private IMongoCollection<BsonDocument> _casetypecollectionlist;
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        private string _tenantid;
        public CaseTypeService(ICasesDatabaseSettings settings)
        {
            try
            {
                _settings = settings;
                _client = new MongoClient(settings.ConnectionString);
                MBADDatabase = _client.GetDatabase(settings.DatabaseName);       
            }
            catch { throw; }
        }
        public CaseTypeService(IMongoCollection<CaseType> casetypecollection)
        {
            try
            {
                _casetypecollection = casetypecollection;
            }
            catch { throw; }
        }
        public void Gettenant(string tenantid)
        {
            try
            {
                TenantDatabase = helperservice.Gettenant(tenantid, _client, MBADDatabase, _settings);
                _casetypecollection = TenantDatabase.GetCollection<CaseType>(_settings.CaseTypesCollectionName);
                _casetypecollectionlist = TenantDatabase.GetCollection<BsonDocument>(_settings.CaseTypesCollectionName);
                _tenantid = tenantid;
            }
            catch { throw; };
        }
        public CaseType Get(string id)
        {
            try { return _casetypecollection.Find<CaseType>(book => book._id == id).FirstOrDefault(); } catch { throw; };
        }
        public CaseType GetByName(string name)
        {
            try{ return _casetypecollection.Find<CaseType>(book => book.Casetype.ToLower() == name.ToLower()).FirstOrDefault(); } catch { throw; };
        }

        public List<CaseType> Searchcasetypes(string sfilter, bool bLocal)
        {
            IMongoCollection<Tenant> _tenantcoll = MBADDatabase.GetCollection<Tenant>("Tenants");
            Tenant tenant = _tenantcoll.Find(t => t.Tenantname.ToLower() == _tenantid.ToLower()).FirstOrDefault();
            string rapidkey = tenant.Rapidapikey;

            if (rapidkey != null && rapidkey != "" && bLocal == false)
            {

                var client = new HttpClient();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,

                    RequestUri = new Uri("https://mongodb-wix.p.rapidapi.com/casetype/search/all"),
                    Headers =
                                {
                                    { "x-rapidapi-host", "mongodb-wix.p.rapidapi.com" },
                                    { "x-rapidapi-key", rapidkey },
                                },
                };
                using (var response = client.SendAsync(request).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();
                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (body != null)
                    {
                        List<CaseType> oretcase = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CaseType>>(body);
                        return oretcase;
                    }
                    else
                    {
                        List<CaseType> oretcase = new List<CaseType>();
                        return oretcase;
                    }
                }
            }
            else
            {
                FilterDefinition<BsonDocument> oFilterDoc;
                FilterDefinitionBuilder<BsonDocument> ofd = new FilterDefinitionBuilder<BsonDocument>();
                var clauses = new List<FilterDefinition<BsonDocument>>();
                //var ofd = Builders<BsonDocument>.Filter;
                string scasetype = string.Empty;
                List<BsonDocument> colC = new List<BsonDocument>();
                //IDictionary<string, string> sdir = new Dictionary<string, string>(); ;
                if (sfilter == "" || sfilter.ToLower()=="all")
                {
                    oFilterDoc = ofd.And(ofd.Ne("Casetype", ""));
                }
                else
                {
                    oFilterDoc = ofd.And(ofd.Eq("Casetype", sfilter));
                }
                try
                {

                    colC = _casetypecollectionlist.Find(oFilterDoc).ToList();

                    List<CaseType> oretcase = new List<CaseType>();
                    if (colC != null)
                    {

                        foreach (BsonDocument b in colC)
                        {
                            CaseType ocas = BsonSerializer.Deserialize<CaseType>(b.ToJson());
                            oretcase.Add(ocas);
                        }
                    }

                    return oretcase;
                }
                catch
                {
                    throw;
                }
            } 

        }
        
        public CaseType Create(string CaseTypeName,CaseType ocasetype)
        {
            try
            {
                //check if unique
                CaseType c = _casetypecollection.Find(c => c.Casetype.ToUpper() == CaseTypeName.ToUpper()).FirstOrDefault();
                if (c != null) { ocasetype = c; return ocasetype; }
                if (ocasetype.Casetype != CaseTypeName) { ocasetype.Casetype = CaseTypeName; }
                //if (ocasetype.Updateuser == null) { ocasetype.Updateuser = createuserid; }
                if (ocasetype.Createdate == null || ocasetype.Createdate=="") { ocasetype.Createdate = DateTime.UtcNow.ToString(); }
                if (ocasetype.Updatedate == null || ocasetype.Createdate == "") { ocasetype.Updatedate = DateTime.UtcNow.ToString(); }

                _casetypecollection.InsertOne(ocasetype);
                return ocasetype;
            }
            catch
            {
                throw;
            }

        }

        public void Update(string id, CaseType CaseTypeIn)
        {
            try
            {
                _casetypecollection.ReplaceOne(ocase => ocase._id == id, CaseTypeIn);
               
            }
            catch { throw; }
        }
        public void Remove(string id)
        {
            try
            {
                _casetypecollection.DeleteOne(c => c._id == id);

            }
            catch { throw; }
        }
        public Message SetMessage(string casetypeid, string srequest, string srequesttype, string sMessageCode, string sMessagedesc, string userid, Exception ex)
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
                Callerid = casetypeid,
                Callertype = ICallerType.CASETYPE ,
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
   


}
 
