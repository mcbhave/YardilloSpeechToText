using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace MBADCases.Models
{
    public class CaseTypeFieldnotused
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Tenantid { get; set; }
        public string Casetypeid { get; set; }
        public string Fieldname { get; set; }
        public string Fielddesc { get; set; }

        public string Fieldtype { get; set; }
    }
}
