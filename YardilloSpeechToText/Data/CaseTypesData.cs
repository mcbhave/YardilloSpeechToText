using MBADCases.Models;
using System.Collections.Generic;
using System.Linq;

namespace MBADCases.Data
{
    public class CaseTypesData
    {
        private string _id { get; set; }
        public CaseTypesData(string id)
        {
            _id = id;
        }
        public List<CaseType> GetCaseTypes()
        {
            List<CaseType> oten = GetAllCaseTypesfromDB();

            return oten;
        }
        public CaseType GetCaseType(string _id)
        {
            List<CaseType> oten = GetAllCaseTypesfromDB();
            return (CaseType)oten.SingleOrDefault(x => x._id == _id);

        }
        private static List<CaseType> GetAllCaseTypesfromDB()
        {
            string spath = "Data/casetypesdata.json";
            string sSchemas = System.IO.File.ReadAllText(spath);
            List<CaseType> lsch = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CaseType>>(sSchemas);

            return lsch;
        }
    }
}
