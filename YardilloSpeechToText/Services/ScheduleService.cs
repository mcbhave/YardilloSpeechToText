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
    public class ScheduleService
    {
        private IMongoCollection<Schedule> _schedulesCollection;
         
        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        private string _tenantid;
        public ScheduleService(ICasesDatabaseSettings settings)
        {
            try
            {
                _settings = settings;
                _client = new MongoClient(settings.ConnectionString);
                MBADDatabase = _client.GetDatabase(settings.DatabaseName);
            }
            catch { throw; }
        }
        public ScheduleService(IMongoCollection<Schedule> schedulesCollection)
        {
            try
            {
                _schedulesCollection = schedulesCollection;
            }
            catch { throw; }
        }
        public void Gettenant(string tenantid)
        {
            try
            {
                TenantDatabase = helperservice.Gettenant(tenantid, _client, MBADDatabase, _settings);
                _schedulesCollection = TenantDatabase.GetCollection<Schedule>(_settings.SchedulesCollectionName);
                _tenantid = tenantid;
            }
            catch { throw; };
        }
        public List<Schedule> Get()
        {
            try { return _schedulesCollection.Find(f => f.Tenantid == _tenantid).ToList(); } catch { throw; };
        }
        public  Schedule  Get(string id)
        {
            try { return _schedulesCollection.Find(f => f._id == id).FirstOrDefault(); } catch { throw; };
        }
        public Message SetMessage(Message oms)
        {

            MessageService omesssrv = new MessageService(_settings, TenantDatabase, MBADDatabase);
            oms = omesssrv.Create(oms);

            return oms;

        }
        

    }

}
