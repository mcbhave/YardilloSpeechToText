using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MBADCases.Models
{
    public class TenantUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string Tenantname { get; set; }
        public string Userid { get; set; }

        public string Createdate { get; set; }

        public string Createuserid { get; set; }

        public string Source { get; set; }

        public string Role { get; set; }

        public string Type { get; set; }

        public string YAuthSource { get; set; }

        public string RapidAPIkey { get; set; }

        public string Installationid { get; set; }
    }
}
