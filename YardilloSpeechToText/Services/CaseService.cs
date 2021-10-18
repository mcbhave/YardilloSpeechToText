using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MBADCases.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace MBADCases.Services
{
    public class CaseService  
    {
        private  IMongoCollection<Case> _casecollection;
        private IMongoCollection<BsonDocument> _casecollectionlist;
        private IMongoCollection<CaseDB> _casedbcollection;
        private IMongoCollection<CaseType> _casetypecollection;
        private IMongoCollection<CaseActivityHistory> _caseactivityhistorycollection;
        private IMongoCollection<ActionAuthLogs> _ActionAuthLogscollection;
        private IMongoCollection<AdapterMapLog> _AdapterLogscollection;
        private IMongoCollection<Adapter> _Adapterscollection;
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        private string _tenantid;
        public CaseService(ICasesDatabaseSettings settings)
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
            try {  
                TenantDatabase = helperservice.Gettenant(tenantid, _client, MBADDatabase, _settings);
                _casecollection =  TenantDatabase.GetCollection<Case>(_settings.CasesCollectionName);
                _casetypecollection = TenantDatabase.GetCollection<CaseType>(_settings.CaseTypesCollectionName);
                _casedbcollection= TenantDatabase.GetCollection<CaseDB>(_settings.CasesCollectionName);
                _caseactivityhistorycollection = TenantDatabase.GetCollection<CaseActivityHistory>(_settings.Caseactivityhistorycollection);
                _ActionAuthLogscollection = TenantDatabase.GetCollection<ActionAuthLogs>(_settings.ActionAuthLogscollection);
                _Adapterscollection = TenantDatabase.GetCollection<Adapter>(_settings.Adapterscollection);
                _AdapterLogscollection = TenantDatabase.GetCollection<AdapterMapLog>(_settings.AdapterLogscollection);
                _tenantid = tenantid;

                _casecollectionlist = TenantDatabase.GetCollection<BsonDocument>(_settings.CasesCollectionName);
            }
            catch { throw; };
        }
        //public List<Case> Get() =>
        //    _case.Find(book => true).ToList();
        public List<Case> Searchcases(string sfilter)
        {
            FilterDefinition<BsonDocument> oFilterDoc  ;
            //FilterDefinitionBuilder<BsonDocument> ofd = new FilterDefinitionBuilder<BsonDocument>();
            var clauses = new List<FilterDefinition<BsonDocument>>();
            var ofd = Builders<BsonDocument>.Filter;
            string scasetype=string.Empty;
            List<BsonDocument> colC = new List<BsonDocument>();
            //IDictionary<string, string> sdir = new Dictionary<string, string>(); ;
            //oFilterDoc = ofd.Eq("Casestatus", scasetype);
            oFilterDoc = ofd.Ne("Casetype", "");
            try
            {
                if(sfilter!=null || sfilter != "")
                {
                    var sfl = sfilter.Split("&");
                    foreach(string s in sfl)
                    {
                       var sparam= s.Split("=");
                        if (sparam.Length == 2)
                        {
                            var filenameQuery = new BsonRegularExpression(sparam[1].ToString(), "i");
                            switch (sparam[0].ToString().ToLower())
                            {
                                case "casetype":
                                    clauses.Add(ofd.Eq("Casetype", filenameQuery));
                                    break;
                                case "itemid":
                                    clauses.Add(ofd.Eq("itemId", filenameQuery));
                                    break;
                                case "casestatus":                                 
                                    clauses.Add(ofd.Eq("Casestatus", filenameQuery)); // "{'$regex' : 'Open', '$options' : 'i'}"
                                    break;
                                case "currentactivityid":
                                    clauses.Add(ofd.Eq("Currentactivityid", filenameQuery));
                                    break;
                                case "createdate":
                                    clauses.Add(ofd.Eq("Createdate", filenameQuery));
                                    break;
                                case "createuser":
                                    clauses.Add(ofd.Eq("Createuser", filenameQuery));
                                    break;
                                case "updatedate":
                                    clauses.Add(ofd.Eq("Updatedate", filenameQuery));
                                    break;
                                case "updateuser":
                                    clauses.Add(ofd.Eq("Updateuser", filenameQuery));
                                    break;
                                default:
                                    clauses.Add(ofd.ElemMatch<BsonValue>(
                                                   "Fields", new BsonDocument
                                                               { { "Fieldid", new BsonRegularExpression(sparam[0].ToString(), "i") },
                                                                                { "Value", new BsonDocument { { "$eq", sparam[1].ToString() } } }
                                                               }));
                                    //new BsonRegularExpression(sparam[0].ToString(), "i")
                                    //colC = _casecollectionlist.Find(Builders<BsonDocument>.Filter.Eq("fields[?(@.fieldid=='" + sparam[0].ToString() + "')]", sparam[1].ToString())).ToList();
                                    break;
                            };
                        }                           
                    }
                }

                CaseTypeService octsr = new CaseTypeService(_casetypecollection);
                oFilterDoc = ofd.And(clauses);
              colC = _casecollectionlist.Find(oFilterDoc ).Limit(10).ToList();
                List<Case> oretcase = new List<Case>();
                if (colC != null)
                {

                    foreach (BsonDocument b in colC)
                    {
                        Case ocas = BsonSerializer.Deserialize<Case>(b.ToJson());
                        ocas= Get(ocas._id);
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
        public List<Case> SearchcasesfromRapidapi(string sfilter)
        {
            IMongoCollection<Tenant> _tenantcoll = MBADDatabase.GetCollection<Tenant>("Tenants");
            Tenant tenant = _tenantcoll.Find(t => t.Tenantname.ToLower() == _tenantid.ToLower()).FirstOrDefault();
            string rapidkey = tenant.Rapidapikey;

            if (rapidkey != null && rapidkey != "")
            {

                var client = new HttpClient();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,

                    RequestUri = new Uri("https://mongodb-wix.p.rapidapi.com/case/" + sfilter),
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
                        List<Case> oretcase = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Case>>(body);
                        return oretcase;
                    }
                    else
                    {
                        List<Case> oretcase = new List<Case>();
                        return oretcase;
                    }
                }
            }
            else
            {
                return Searchcases(sfilter);
            }

        }

        public Case Get(string id) {
            try { 
                Case ocase= _casecollection.Find<Case>(book => book._id == id).FirstOrDefault();

                if (ocase == null) { return null; }

                //get case type
                CaseTypeService octsr = new CaseTypeService(_casetypecollection);
                //get case type definations
                CaseType oct = octsr.GetByName(ocase.Casetype);
                if (oct == null)
                {
                    oct = new CaseType();
                    oct.Casetype = ocase.Casetype;
                    //Register new case type
                    oct = octsr.Create(ocase.Casetype, oct);
                }
                Activity oCurrentActivity;
                if (ocase.Currentactivityid != null && ocase.Currentactivityid!="") 
                { 
                    oCurrentActivity = oct.Activities.Where(f => f.Activityid == ocase.Currentactivityid).FirstOrDefault();
                    if (oCurrentActivity != null)
                    {
                        CaseActivity ocasactiv = new CaseActivity();
                        CaseActivityHistory oacthist= _caseactivityhistorycollection.Find<CaseActivityHistory>(f => f.Activityid == oCurrentActivity.Activityid && f.Caseid == id).FirstOrDefault();
                            if (oacthist != null && oacthist.Activitycomplete==false) 
                            {

                                ocasactiv.Activityid = oCurrentActivity.Activityid;
                                ocasactiv.Activityname = oCurrentActivity.Activityname;
                                ocasactiv.Activitycomplete = oacthist.Activitycomplete;
                                ocasactiv.Activitycompletedate = oacthist.Activitycompletedate;
                                //get current action
                                Models.Action oact = oCurrentActivity.Actions.Where(o => o.Actionid == ocase.Currentactionid).FirstOrDefault();
                                if (oact != null)
                                {
                                    List<Models.Action> colacts = oCurrentActivity.Actions.Where(o => o.Actionseq >= oact.Actionseq && o.Actiontype == "TASK").ToList();
                                    foreach (Models.Action o in colacts)
                                    {
                                        o.Activityid = oCurrentActivity.Activityid;
                                        o.Activityseq = oCurrentActivity.Activityseq;
                                        o.Caseid = id;
                                        if (helperservice.GetCompareResults(ocase, o, _ActionAuthLogscollection))
                                        {
                                            CaseAction ocurract = new CaseAction();
                                            ocurract.Actionid = o.Actionid;
                                            ocurract.Actiontype = o.Actiontype;
                                            ocurract.Actionname = o.Actionname;
                                            ocurract.Actionseq = o.Actionseq;
                                            ocasactiv.Actions.Add(ocurract);
                                        }

                                    }
                                }
                                ocase.Activities.Add(ocasactiv);
                            }
                            else
                            {
                                //if (oacthist != null && oacthist.Activitycomplete == true)
                                //{
                                //    ocase.Casestatus = "Closed";
                                //    ocase.Updatedate = DateTime.UtcNow.ToString();  
                                //}
                            }
                        }
                }


                //set all field attributes
                foreach(Casefield f in ocase.Fields)
                {
                    foreach(Models.Activity aa in oct.Activities)
                    {
                        foreach(Models.Action  aat in aa.Actions)
                        {
                            Casetypefield cf = aat.Fields.Where(fl => fl.Fieldid == f.Fieldid).FirstOrDefault();
                            if (cf != null){
                                f.Fieldname= cf.Fieldname;
                                f.Required = cf.Required;
                                f.Seq = cf.Seq;
                                f.Options = cf.Options;
                                f.Type = cf.Type;
                            }
                        }
                    }
                }

                if (oct.Fields != null) { 
                foreach (Casefield f in ocase.Fields)
                {
                    Casetypefield cf = oct.Fields.Where(fl => fl.Fieldid == f.Fieldid).FirstOrDefault();
                    if (cf != null)
                    {
                        f.Fieldname = cf.Fieldname;
                        f.Required = cf.Required;
                        f.Seq = cf.Seq;
                        f.Options = cf.Options;
                        f.Type = cf.Type;
                    }
                }
                }
                //get top one that is not yet complete
                //if (colact != null)
                //{
                //    Activity a = colact.FirstOrDefault();
                //    if (a != null)
                //    {
                //        CaseActivity ocasactiv = new CaseActivity();
                //        ocasactiv.Activityid = a.Activityid;
                //        ocase.Activities.Add(a);
                //    }

                //}


                return ocase;
            } catch { throw; };
        }
         
        public CaseDB Create(Case ocase)
        {
            try
            {
                bool caseclosed = true;
                //create a case _id first
                var oc = ocase.ToJson();
                CaseDB ocasedb = Newtonsoft.Json.JsonConvert.DeserializeObject<CaseDB>(oc);
                _casedbcollection.InsertOne(ocasedb);

                //first get the case type
                //get case type details
                CaseTypeService octsr = new CaseTypeService(_casetypecollection);
                CaseType oct = octsr.GetByName(ocasedb.Casetype);
                if (oct == null) {
                    oct = new CaseType();
                    oct.Casetype = ocasedb.Casetype;
                    //Register new case type
                    oct= octsr.Create(ocasedb.Casetype, oct);
                }

                if (oct.Activities != null) { 
                IComparer<Activity> comparer = new MyActivityOrder();
                oct.Activities.Sort(comparer);
                foreach (Activity odact in oct.Activities)
                {
                    //set this as current activity
                    ocasedb.Currentactivityid = odact.Activityid;
                    CaseActivityHistory icaseActivity = new CaseActivityHistory();
                    icaseActivity.Caseid = ocasedb._id;
                    icaseActivity.Activityid = odact.Activityid;
                    icaseActivity.Activityseq = odact.Activityseq;
                    icaseActivity.Activitydisabled = odact.Isdisabled;
                    
                    int totalactionsfin = 0;
                    if (odact.Isdisabled == false)
                    {
                        //if reaches this point then check if the first activity action is an EVENT OR TASK
                        //execute EVENT TYPE actions until it reaches a task
                        IComparer<Models.Action> compareract = new MyActionOrder();
                        odact.Actions.Sort(compareract);

                        foreach (Models.Action iAct in odact.Actions)
                        {
                            //regardless of its a TASK OR EVENT check if its authorized
                            CaseAction iCaseActn = new CaseAction();
                            iCaseActn.Actionid = iAct.Actionid;
                            iCaseActn.Actiontype = iAct.Actiontype;
                            string sFieldValue=string.Empty;
                            
                            iAct.Caseid = ocasedb._id;
                            iAct.Activityid = odact.Activityid;
                            iAct.Activityseq = odact.Activityseq;
                            if (helperservice.GetCompareResults(ocase,iAct, _ActionAuthLogscollection) == true)
                            {
                                ocasedb.Currentactionid = iAct.Actionid;

                                if (iAct.Actiontype.ToUpper() == "EVENT")
                                {
                                    if (iAct.Fields != null)
                                    {
                                        List<Casetypefield> oflds = iAct.Fields.Where(o => o.Required == true).ToList();

                                        foreach (Casetypefield ofl in oflds)
                                        {
                                            CaseDBfield casefield;
                                            if ((casefield = ocasedb.Fields.Where(o => o.Fieldid.ToUpper() == ofl.Fieldid.ToUpper()).FirstOrDefault()) != null)
                                            {
                                                if (casefield.Value == null || casefield.Value == "")
                                                {
                                                    throw new Exception("Field and its value is required, {\"Fieldid\":\"" + ofl.Fieldid + "\",\"Value\":\"100000\"}");
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Field and its value is missing, {\"Fieldid\":\"" + ofl.Fieldid + "\",\"Value\":\"100000\"}" );
                                            }
                                        }

                                        //if all fields are present then add them
                                        foreach (Casetypefield ofl in iAct.Fields)
                                        {
                                            CaseDBfield odbsetfld;
                                            if ((odbsetfld = ocasedb.Fields.Where(F => F.Fieldid.ToLower() == ofl.Fieldid.ToLower()).FirstOrDefault()) != null)
                                            {
                                                odbsetfld.Value = ofl.Value;
                                            }
                                            else
                                            {
                                                ocasedb.Fields.Add(new CaseDBfield() { Fieldid = ofl.Fieldid, Value = ofl.Value });
                                            }
                                        }
                                    }

                                    Adapterresponse oAdpResp = new Adapterresponse();
                                    List<SetCasetypefield> colfld; 
                                    if (iAct.Adapterresponsemaps != null && iAct.Adapterresponsemaps.Count > 0)
                                    {
                                      foreach(Adapterresponsemap oad in iAct.Adapterresponsemaps)
                                        {
                                            Adapter oadp;
                                            if ((oadp = _Adapterscollection.Find(a => a.Name.ToLower() == oad.Adapterid.ToLower()).FirstOrDefault()) != null)
                                            {
                                                StringBuilder slog = new StringBuilder();
                                                try
                                                {
                                                    slog.Append("Action:" + iAct.Actionid );
                                                    slog.Append("Adapter:" + oad.Adapterid);
                                                    colfld = helperservice.ExecuteAdapter(oadp, oad, slog);
                                                    AdapterMapLog olog = new AdapterMapLog() { Caseid = ocasedb._id,Log=slog.ToString(), Casetype=ocasedb.Casetype, Actionid=iAct.Actionid,Adapterid=oadp.Name,Createdate=  DateTime.UtcNow.ToString() };

                                                    _AdapterLogscollection.InsertOne(olog);

                                                    foreach (Models.SetCasetypefield ofl in colfld)
                                                    {
                                                        CaseDBfield ocasesetfld;
                                                        if ((ocasesetfld = ocasedb.Fields.Where(F => F.Fieldid.ToLower() == ofl.Fieldid.ToLower()).FirstOrDefault()) != null)
                                                        {
                                                            ocasesetfld.Value = ofl.Value;
                                                        }
                                                        else
                                                        {
                                                            //add field
                                                            ocasesetfld = new CaseDBfield();
                                                            ocasesetfld.Fieldid = ofl.Fieldid;
                                                            ocasesetfld.Value = ofl.Value;
                                                            ocasedb.Fields.Add(ocasesetfld);
                                                        }
                                                    }

                                                }
                                                catch
                                                {
                                                    //log
                                                    AdapterMapLog olog = new AdapterMapLog() { Caseid = ocasedb._id, Log = slog.ToString(), Casetype = ocasedb.Casetype, Actionid = iAct.Actionid, Adapterid = oadp.Name, Createdate = DateTime.UtcNow.ToString() };

                                                    _AdapterLogscollection.InsertOne(olog);
                                                }

                                            };
                                        }
                                       
                                      
                                        //execute adapter
                                        oAdpResp = iAct.Adapterresponses.Where(a => a.Actionresponse.ToUpper() == "TRUE").FirstOrDefault();
                                    }
                                    else
                                    {
                                        //Adapterresponseattr is also null in this case
                                        //use TRUE
                                        oAdpResp = iAct.Adapterresponses.Where(a => a.Actionresponse.ToUpper() == "TRUE").FirstOrDefault();
                                    }

                                    if (oAdpResp != null)
                                    {
                                        iCaseActn.Adapterresponse = oAdpResp.Actionresponse;
                                        //no adapter associated
                                        //Translate and assign fields
                                        foreach (Models.SetCasetypefield ofl in oAdpResp.Fields)
                                        {
                                            CaseDBfield ocasesetfld;
                                            if ((ocasesetfld = ocasedb.Fields.Where(F => F.Fieldid.ToLower() == ofl.Fieldid.ToLower()).FirstOrDefault()) != null)
                                            {
                                                ocasesetfld.Value = ofl.Value;
                                            }
                                            else
                                            {
                                                //add field
                                                ocasesetfld = new CaseDBfield();
                                                ocasesetfld.Fieldid = ofl.Fieldid;
                                                ocasesetfld.Value = ofl.Value;
                                                ocasedb.Fields.Add(ocasesetfld);
                                            }
                                        }

                                        //at this point activity action is complete
                                        //set Actioncomplete = true and  Actionstatus = "COMPLETE"

                                        iCaseActn.Actioncomplete = true;
                                        iCaseActn.Actioncompletedate = DateTime.UtcNow.ToString();
                                        iCaseActn.Actionstatus = "SUCCESS";
                                        icaseActivity.Actions.Add(iCaseActn);
                                    }
                                    
                                }
                                else
                                {
                                    // you hit a TASK
                                    //STOP
                                    caseclosed = false;
                                    break;
                                }
                                
                            }
                            else //action condition is false
                            {
                                iCaseActn.Actioncomplete = true;
                                iCaseActn.Actioncompletedate = DateTime.UtcNow.ToString();
                                iCaseActn.Actionstatus = "SKIPPED";
                                icaseActivity.Actions.Add(iCaseActn);
                            }
                                totalactionsfin = totalactionsfin + 1;
                                if (totalactionsfin == odact.Actions.Count)
                                {
                                    icaseActivity.Activitycomplete = true;
                                    icaseActivity.Activitycompletedate = DateTime.UtcNow.ToString();
                                    _caseactivityhistorycollection.InsertOneAsync(icaseActivity);
                                }
                             

                        }
                        
                         
                    }
                    else
                    {
                        icaseActivity.Activitycomplete = true;
                        icaseActivity.Activitycompletedate = DateTime.UtcNow.ToString();
                        _caseactivityhistorycollection.InsertOneAsync(icaseActivity);
                    }

                }
                }

              
                if (caseclosed)
                {
                    ocasedb.Casestatus = "Closed";
                    ocasedb.Updatedate = DateTime.UtcNow.ToString();
                    ocasedb.Currentactivityid = "";
                    ocasedb.Currentactionid  = "";
                }
                _casedbcollection.ReplaceOne(ocase => ocase._id == ocasedb._id, ocasedb);
                return ocasedb;
            }
            catch
            {
                throw;
            }
          
        }
        public void Update(string id, Case CaseIn)
        {
            try
            {
                _casecollection.ReplaceOne(ocase => ocase._id == id, CaseIn); ;
            }
            catch { throw; }
        }
        public void Update1(string id,Case CaseIn) 
        {
            try {
            //https://wimsupapp.bubbleapps.io/version-test/api/1.1/obj/mpclubusers
                foreach (Casefield csat in CaseIn.Fields)
            {
                var arrayFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))
                        & Builders<BsonDocument>.Filter.Eq("Fields.Fieldid", csat.Fieldid);

                    UpdateDefinition<BsonDocument> arrayUpdate=null;

                    if (csat.Value != null)
                    {
                        arrayUpdate = Builders<BsonDocument>.Update.Set("Fields.$.Value", csat.Value);
                    }
                    if (csat.Type != null) {
                        if (arrayUpdate == null)
                        {
                            arrayUpdate = Builders<BsonDocument>.Update.Set("Fields.$.Type", csat.Type);
                        }
                        else
                        {
                            arrayUpdate.AddToSet("Fields.$.Type", csat.Type);
                        }
                   
                    }

                    var casecoll = TenantDatabase.GetCollection<BsonDocument>(_settings.CasesCollectionName);
                casecoll.UpdateOne(arrayFilter, arrayUpdate);
            }
            }
            catch { throw; }
        }
        //public void Update(string id, Case caseIn) =>
        //    _case.ReplaceOne(ocase => ocase._id == id, caseIn);

        //public void Remove(Case caseIn) =>
        //    _case.DeleteOne(ocase => ocase._id == caseIn._id);

        public void Remove(string id)
        {
            try {
                _casecollection.DeleteOne(book => book._id == id);
            }
            catch { throw; }
        }

        public Message SetMessage(string  callrtype,string caseid, string srequest,string srequesttype, string sMessageCode, string sMessagedesc,string userid, Exception ex)
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
            Message oms = new Message {
                Tenantid = _tenantid,
                Callerid = caseid,
                Callertype = callrtype,
                Messagecode = _MessageCode, 
                Messageype = _MessageType,
                MessageDesc= _MessageDesc,
                Callerrequest=srequest,
                Callerrequesttype=srequesttype,
                Userid= userid,
                Messagedate=DateTime.UtcNow.ToString()
        };
            
            MessageService omesssrv = new MessageService(_settings,TenantDatabase, MBADDatabase);
            oms= omesssrv.Create(oms);
           
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
