using MBADCases.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MBADCases.Models
{
    public class VaultResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Name { get; set; }
        public string Macroname { get; set; }

        public string Encryptwithkey { get; set; }
        public string Safekeeptext { get; set; }
        public MessageResponse Message { get; set; }
    }
}
