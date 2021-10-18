using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class ActionAuthLogs
    {
        public ActionAuthLogs()
        {
            Logdate = DateTime.UtcNow.ToString();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string Caseid { get; set; }

        public string Activityid { get; set; }
        public int Activityseq { get; set; }
        public string Actionid { get; set; }
        public int Actionseq { get; set; }

        public bool Actionauthresult { get; set; }
        public string Logdate { get; set; }
        public string Logdesc { get; set; }

    }
}
