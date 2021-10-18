using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MBADCases.Models
{
    public class Tenant : IWixItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Tenantname { get; set; }
        public string Tenantdesc { get; set; }

        public string _owner { get; set; }
        public string Createdate { get; set; }
        public string Createuser { get; set; }

        public string Dbconnection { get; set; }
        public string Rapidapikey { get; set; }

        public string Rapidhost { get; set; }

        public string Rapidsubscription { get; set; }
        public MessageResponse Message { get; set; }
        public string YAuthSource { get; set; }

    }
}
