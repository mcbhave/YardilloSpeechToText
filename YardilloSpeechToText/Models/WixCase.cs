using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class WixCase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string _owner { get; set; }

        
        public string casetitle { get; set; }
        public string casetype { get; set; }
        public string casestatus { get; set; }
        public string currentactivityid { get; set; }
        public string currentactionid { get; set; }

        public string casedescription { get; set; }
        public string createdate { get; set; }
        public string createuser { get; set; }

        public string updatedate { get; set; }
        public string updateuser { get; set; }


    }
}
