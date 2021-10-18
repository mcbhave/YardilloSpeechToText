using MBADCases.Models;
using System.Collections.Generic;
using System.Linq;

namespace MBADCases.Data
{
    public class TenantData
    {
        private string _id {get;set;}
        public TenantData(string id)
        {
            _id = id;
        }
        public List<Tenant> getTenants()
        {
            List<Tenant> oten =  GetAllTenantsfromDB();
           
            return oten;
        }
        public Tenant GetTenant(string _id)
        {
            List<Tenant> oten = GetAllTenantsfromDB();
            return (Tenant)oten.SingleOrDefault(x => x._id == _id); 
              
        }
        private static List<Tenant> GetAllTenantsfromDB()
        {
            string spath = "Data/tenants.json";
            string sSchemas = System.IO.File.ReadAllText(spath);
            List<Tenant> lsch = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tenant>>(sSchemas);

            return lsch;
        }
    }
}
