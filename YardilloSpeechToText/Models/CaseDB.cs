using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MBADCases.Models
{
    public class CaseDB
    {
        public CaseDB()
        {
            Fields = new List<CaseDBfield>();
            //Activities = new List<CaseActivity>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int Casenumber { get; set; }
        public string Casetitle { get; set; }
        public string Casetype { get; set; }
        public string Casestatus { get; set; }
        public string Currentactivityid { get; set; }
        public string Currentactionid { get; set; }
        public string Casedescription { get; set; }
        public string Createdate { get; set; }
        public string Createuser { get; set; }

        public string Updatedate { get; set; }
        public string Updateuser { get; set; }
        //public string Sladate { get; set; }
        public string itemId { get; set; }
        public List<CaseDBfield> Fields { get; set; }
        //public List<CaseActivity> Activities { get; set; }
        public string Blob { get; set; }

    }

    public class CaseDBfield
    {
        public string Fieldid { get; set; }
        public string Value { get; set; }
    }
    public class CaseDBAction
    {
        public string Actionid { get; set; }
        public string Value { get; set; }
    }
}