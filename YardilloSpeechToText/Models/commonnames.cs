using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YardilloSpeechToText.Models
{
    public class commonnamesmap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string name { get; set; }
        public string tip { get; set; }

    }

    public class commonaispeechid
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Sourceid { get; set; }
        public string Tranid { get; set; }
        public string Tenantid { get; set; }
        public string Speechid { get; set; }
        public string Createdate { get; set; }
        public string Createuser { get; set; }
    }
}
