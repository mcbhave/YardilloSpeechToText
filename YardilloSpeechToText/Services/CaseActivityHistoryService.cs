using MBADCases.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Services
{
    public class CaseActivityHistoryService
    {
        private IMongoCollection<Case> _casecollection;
        private IMongoCollection<CaseDB> _casedbcollection;
        private IMongoCollection<CaseType> _casetypecollection;
        private IMongoCollection<CaseActivityHistory> _caseactivityhistorycollection;
        private IMongoCollection<ActionAuthLogs> _ActionAuthLogscollection;

        private IMongoDatabase MBADDatabase;
        private IMongoDatabase TenantDatabase;
        ICasesDatabaseSettings _settings;
        private MongoClient _client;
        public CaseActivityHistoryService(ICasesDatabaseSettings settings)
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
                _casecollection = TenantDatabase.GetCollection<Case>(_settings.CasesCollectionName);
                _casetypecollection = TenantDatabase.GetCollection<CaseType>(_settings.CaseTypesCollectionName);
                _casedbcollection = TenantDatabase.GetCollection<CaseDB>(_settings.CasesCollectionName);
                _caseactivityhistorycollection = TenantDatabase.GetCollection<CaseActivityHistory>(_settings.Caseactivityhistorycollection);
                _ActionAuthLogscollection = TenantDatabase.GetCollection<ActionAuthLogs>(_settings.ActionAuthLogscollection);


            }
            catch { throw; };
        }
        public CaseActivityHistory Get(string id)
        {
            try
            {
                CaseActivityHistory ocase = _caseactivityhistorycollection.Find<CaseActivityHistory>(book => book._id == id).FirstOrDefault();
                                

                return ocase;
            }
            catch { throw; };
        }
        public CaseActivityHistory Get(string id,string Caseid)
        {
            try
            {
                CaseActivityHistory ocase = _caseactivityhistorycollection.Find<CaseActivityHistory>(book => book._id == id && book.Caseid==Caseid).FirstOrDefault();

                return ocase;
            }
            catch { throw; };
        }
    }
}
