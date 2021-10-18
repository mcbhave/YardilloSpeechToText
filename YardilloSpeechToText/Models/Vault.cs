using MBADCases.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MBADCases
{
    public class Vault
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Name { get; set; }
        public string Macroname { get; set; }

        public string Encryptwithkey { get; set; }
        public string Safekeeptext { get; set; }
        public string Tenantid { get; set; }
        
    }
}
