using System;
using System.Collections.Generic;
using System.Linq;
using MBADCases.Services;
using MongoDB.Driver;
namespace MBADCases.Models
{
    public class AdapterService
    {
        private IMongoCollection<Adapter> _Adapterscollection;
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        private string _tenantid;
        public AdapterService(ICasesDatabaseSettings settings)
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
                _Adapterscollection = TenantDatabase.GetCollection<Adapter>(_settings.Adapterscollection);
                _tenantid = tenantid;

            }
            catch { throw; };
        }
        public Adapter Get(string id)
        {
            try { 
                return _Adapterscollection.Find<Adapter>(book => book._id == id).FirstOrDefault();
                
            } catch 
            { throw; };
        }
        public List<Adapter> Get()
        {
            try
            {                
                return _Adapterscollection.Find<Adapter>(book => book.Name != null).ToList();
            }
            catch
            { throw; };
        }
        public Adapter GetByName(string name)
        {
            try { 
                return _Adapterscollection.Find<Adapter>(book => book.Name.ToLower() == name.ToLower()).FirstOrDefault();
                
            } catch { throw; };
        }
        
        public Adapter Create(Adapter oadapter)
        {
            try
            {
                _Adapterscollection.InsertOne(oadapter);
                return oadapter;
            }
            catch
            {
                throw;
            }

        }
        public void Update(string id, Adapter AdapterIn)
        {
            try
            {
                Adapter ov = _Adapterscollection.Find<Adapter>(book => book.Name.ToLower() == AdapterIn.Name.ToLower() && book._id !=AdapterIn._id).FirstOrDefault();
                if (ov != null) { 
                 
                    //check unique name
                    if (ov != null) { throw new Exception(AdapterIn.Name + " name already exists. Please send blank and let system generate one or pass unique name"); }
                }
                  
              
                _Adapterscollection.ReplaceOne(ocase => ocase._id == id, AdapterIn);

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
                Callertype = ICallerType.ADAPTER,
                Messagecode = _MessageCode,
                Messageype = _MessageType,
                MessageDesc = _MessageDesc,
                Callerrequest = srequest,
                Callerrequesttype = srequesttype,
                Userid = userid,
                Messagedate = DateTime.UtcNow.ToString()
            };

            MessageService omesssrv = new MessageService(_settings, TenantDatabase,MBADDatabase);
            oms = omesssrv.Create(oms);

            return oms;

        }
    }
}
