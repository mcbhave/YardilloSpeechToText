using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MBADCases.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MBADCases.Services
{
    public class TenantService
    {
        private readonly IMongoCollection<Tenant> _tenant;
        private readonly IMongoCollection<TenantUser> _tenantusercoll;
        private IMongoDatabase MBADDatabase;
        ICasesDatabaseSettings _settings;
        private string _tenantid;
        public TenantService(ICasesDatabaseSettings settings)
        {
            try
            {
                _settings = settings;
                var client = new MongoClient(settings.ConnectionString);
                MBADDatabase = client.GetDatabase(settings.DatabaseName);
                _tenant = MBADDatabase.GetCollection<Tenant>("Tenants");
                _tenantusercoll = MBADDatabase.GetCollection<TenantUser>("TenantUsers");
            }
            catch { throw; }
        }
        public void Gettenant(string tenantid)
        {
            try
            {
                 
                _tenantid = tenantid;
            }
            catch { throw; };
        }
        public List<Tenant> Get() =>
            _tenant.Find<Tenant>(book => book.Tenantname == _tenantid).ToList();

        public List<Tenant> GetByUserid(string id) {
            //TenantUser ou = _tenantusercoll.Find<TenantUser>(x => x.Userid == id).FirstOrDefault();
           return _tenant.Find<Tenant>(book => book.Createuser == id).ToList();

        }

        public Tenant Get(string id)
        {
            try { return _tenant.Find<Tenant>(book => book._id == id).FirstOrDefault(); } catch { throw; };
        }

        public Tenant GetByName(string name)
        {
            try { return _tenant.Find<Tenant>(book => book.Tenantname == name).FirstOrDefault(); } catch { throw; };
        }
       
        public Tenant Create(Tenant ocase)
        {
            try
            {
                string snewtenname = ocase.Tenantname;
                if (snewtenname == null) { ocase.Tenantname = "Tenant"; }
                snewtenname = ocase.Tenantname + "_" + helperservice.RandomString(7, false);
                if (GetByName(snewtenname) != null)
                {
                    snewtenname = ocase.Tenantname + "_" + helperservice.RandomString(7, false);
                    while ((GetByName(snewtenname) != null))
                    {
                        //name must be unique assing a random string
                        snewtenname = ocase.Tenantname + "_" + helperservice.RandomString(7, false);
                    }
                };
                ocase.Tenantname = snewtenname;
                _tenant.InsertOne(ocase);

                if (ocase._id != "")
                {
                    //and register as tenantuser
                    TenantUser ousr = new TenantUser();
                    ousr.Userid = ocase.Createuser;
                    ousr.Tenantname = ocase.Tenantname.ToUpper();
                    ousr.Createdate = DateTime.UtcNow.ToString();
                    ousr.Createuserid = ocase.Createuser;
                    ousr.YAuthSource = ocase.YAuthSource;
                    ousr.Role = "ADMIN";
                    IMongoCollection<TenantUser> _tenantusercoll;
                    _tenantusercoll = MBADDatabase.GetCollection<TenantUser>("TenantUsers");
                    _tenantusercoll.InsertOne(ousr);
                    

                }

                return ocase;
            }
            catch
            {
                throw;
            }
        }
        
        //public Tenant GetByName(string name)
        //{
        //    try
        //    {
        //        VaultResponse ovr = null;
        //        Vault ov = _tenant.Find<Tenant>(book => book.Tenantname.ToLower() == name.ToLower() && book.TEN == _tenantid).FirstOrDefault();
        //        if (ov != null)
        //        {
        //            ovr = new VaultResponse();
        //            ovr._id = ov._id;
        //            ovr.Name = ov.Name;
        //            ovr.Macroname = ov.Macroname;
        //            helperservice.VaultCrypt ovrcr = new helperservice.VaultCrypt(helperservice.Gheparavli(_Vaultcollection));
        //            ovr.Encryptwithkey = ovrcr.Decrypt(ov.Encryptwithkey);
        //            ovr.Safekeeptext = ovrcr.Decrypt(ov.Safekeeptext);
        //        }

        //        return ovr;

        //    }
        //    catch { throw; };
        //}
        public void Update(string id, Tenant tenantIn)
        {
            try
            {
                _tenant.ReplaceOne(ocase => ocase._id == id, tenantIn);

            }
            catch { throw; }
        }
      

        public void Remove(string id)
        {
            try
            {
                _tenant.DeleteOne(book => book._id == id);
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
                Callertype = ICallerType.CASETYPE,
                Messagecode = _MessageCode,
                Messageype = _MessageType,
                MessageDesc = _MessageDesc,
                Callerrequest = srequest,
                Callerrequesttype = srequesttype,
                Userid = userid,
                Messagedate = DateTime.UtcNow.ToString()
            };
             
            MessageService omesssrv = new MessageService(_settings, MBADDatabase, MBADDatabase);
            oms = omesssrv.Create(oms);
            
            return oms;

        }
        public Message SetMessage(Message oms)
        {
            MessageService omesssrv = new MessageService(_settings, MBADDatabase, MBADDatabase);
            oms = omesssrv.Create(oms);

            return oms;
        }
    }

    public class TenantResponse
    {
        public TenantResponse(Tenant ocase, Message oms)
        {
            if (ocase != null) { _id = ocase._id; } else { oms.Messagecode = "417"; }

            //Casenumber = ocase.Casenumber;
            Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public MessageResponse Message { get; set; }
    }
}
