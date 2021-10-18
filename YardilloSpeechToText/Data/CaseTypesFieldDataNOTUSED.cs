using MBADCases.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace MBADCases.Data
{
    public class CaseTypesFieldDataNOTUSED
    {
        private string _id { get; set; }
        public CaseTypesFieldDataNOTUSED(string id)
        {
            _id = id;
        }
        public List<CaseTypeFieldnotused> GetCaseTypeFields()
        {
            List<CaseTypeFieldnotused> oten = GetAllCaseTypeFieldsfromDB();

            return oten;
        }
        public CaseTypeFieldnotused GetCaseTypeField(string _id)
        {
            List<CaseTypeFieldnotused> oten = GetAllCaseTypeFieldsfromDB();
            return (CaseTypeFieldnotused)oten.SingleOrDefault(x => x._id == _id);

        }
        private static List<CaseTypeFieldnotused> GetAllCaseTypeFieldsfromDB()
        {
            string spath = "Data/casetypefieldsdata.json";
            string sSchemas = System.IO.File.ReadAllText(spath);
            List<CaseTypeFieldnotused> lsch = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CaseTypeFieldnotused>>(sSchemas);

            return lsch;
        }
    }
}
