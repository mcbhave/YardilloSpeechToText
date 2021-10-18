using System;
using System.Collections.Generic;
using System.Linq;
using MBADCases.Models;
using MBADCases.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MBADCases.Services
{
    public class VaultService
    {
        private IMongoCollection<Vault> _Vaultcollection;
        private IMongoCollection<Vault> _Vaultcollectionlist;
        //private IMongoCollection<Vault> _masterVaultcollection;
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        public string _tenantid;
        public VaultService(ICasesDatabaseSettings settings)
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
                _tenantid = tenantid;
               TenantDatabase = helperservice.Gettenant(tenantid, _client, MBADDatabase, _settings);
                _Vaultcollection = MBADDatabase.GetCollection<Vault>(_settings.Vaultcollection);
                _Vaultcollectionlist = MBADDatabase.GetCollection<Vault>(_settings.Vaultcollection);


            }
            catch { throw; };
        }
        public VaultResponse Get(string id)
        {
            try {
                VaultResponse ovr=null;
                Vault ov = _Vaultcollection.Find<Vault>(book => book._id == id && book.Tenantid==_tenantid).FirstOrDefault();
                if (ov != null) {
                    ovr = new VaultResponse();
                    ovr._id = ov._id;
                    ovr.Name = ov.Name;
                    ovr.Macroname = ov.Macroname;
                   
                    helperservice.VaultCrypt ovrcr=  new helperservice.VaultCrypt(helperservice.Gheparavli(_Vaultcollection));
                    ovr.Encryptwithkey = ovrcr.Decrypt(ov.Encryptwithkey);
                    ovr.Safekeeptext = ovrcr.Decrypt(ov.Safekeeptext);
                }
                return ovr;
            } catch { throw; };
        }
        public VaultResponse GetByName(string name)
        {
            try {
                VaultResponse ovr = null;
                Vault ov = _Vaultcollection.Find<Vault>(book => book.Name.ToLower() == name.ToLower() && book.Tenantid == _tenantid).FirstOrDefault();
                if (ov != null)
                {
                    ovr = new VaultResponse();
                    ovr._id = ov._id;
                    ovr.Name = ov.Name;
                    ovr.Macroname = ov.Macroname;
                    helperservice.VaultCrypt ovrcr = new helperservice.VaultCrypt(helperservice.Gheparavli(_Vaultcollection));
                    ovr.Encryptwithkey = ovrcr.Decrypt(ov.Encryptwithkey);
                    ovr.Safekeeptext = ovrcr.Decrypt(ov.Safekeeptext);
                }
                 
                return ovr;

            } catch { throw; };
        }
        public List<Vault> Searchvault(string sfilter)
        {
            
            try { 
                return  _Vaultcollectionlist.Find(f=>f.Tenantid==_tenantid).ToList();
            }

 
            catch
            {
                throw;
            }
        }
        public Vault Create(Vault oadapter)
        {
            try
            {
                
                //encrypt the password and key
              if(oadapter.Encryptwithkey==null || oadapter.Encryptwithkey == "")
                {
                    oadapter.Encryptwithkey = Guid.NewGuid().ToString();
                }
                if (oadapter.Safekeeptext == null || oadapter.Safekeeptext == "")
                {
                    oadapter.Safekeeptext = Guid.NewGuid().ToString();
                }
         
                helperservice.VaultCrypt ovault = new helperservice.VaultCrypt(helperservice.Gheparavli(_Vaultcollection));
                oadapter.Safekeeptext= ovault.Encrypt(oadapter.Safekeeptext);
                oadapter.Encryptwithkey = ovault.Encrypt(oadapter.Encryptwithkey);

                oadapter.Tenantid = _tenantid;
                _Vaultcollection.InsertOne(oadapter);
                return oadapter;
            }
            catch
            {
                throw;
            }

        }
        public void Update(string id, Vault VaultIn)
        {
            try
            {
                Vault ov = _Vaultcollection.Find<Vault>(book => book._id == id && book.Tenantid == _tenantid).FirstOrDefault();
                if (ov != null)
                { 

                    if (VaultIn.Encryptwithkey == null || VaultIn.Encryptwithkey == "")
                    {
                        VaultIn.Encryptwithkey = Guid.NewGuid().ToString();
                    }
                    if (VaultIn.Safekeeptext == null || VaultIn.Safekeeptext == "")
                    {
                        VaultIn.Safekeeptext = Guid.NewGuid().ToString();
                    }

                    helperservice.VaultCrypt ovault = new helperservice.VaultCrypt(helperservice.Gheparavli(_Vaultcollection));
                    ov.Safekeeptext = ovault.Encrypt(VaultIn.Safekeeptext);
                    ov.Encryptwithkey = ovault.Encrypt(VaultIn.Encryptwithkey);
                    if(ov.Name.ToLower() != VaultIn.Name.ToLower())
                    {
                        //check unique name
                        Vault ovnchk = _Vaultcollection.Find<Vault>(book => book.Name.ToLower() == VaultIn.Name.ToLower() && book.Tenantid == _tenantid && book._id != VaultIn._id).FirstOrDefault();
                        if (ovnchk != null) { throw new Exception(VaultIn.Name + " name already exists. Please send blank and let system generate one or pass unique name"); }
                    }
                    ov.Name = VaultIn.Name;
                    _Vaultcollection.ReplaceOne(ocase => ocase._id == id, ov);
                }
            }
            catch { throw; }
        }
        public void Remove(string id)
        {
            try
            {
               Vault v= _Vaultcollection.Find<Vault>(f => f._id == id && f.Tenantid == _tenantid).FirstOrDefault();
                if (v != null) { 
                _Vaultcollection.DeleteOne(book => book._id == id);
                }
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
                Callertype = ICallerType.VAULT,
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
     
}
}
