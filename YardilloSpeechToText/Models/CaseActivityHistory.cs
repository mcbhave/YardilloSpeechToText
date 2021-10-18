using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class CaseActivityHistory
    {
        public CaseActivityHistory()
        {

            Actions = new List<CaseAction>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Caseid { get; set; }
        public string Activityid { get; set; }
        public bool Activitycomplete { get; set; }
        public int Activityseq { get; set; }

        public string Activitycompletedate { get; set; }
        public bool Activitydisabled { get; set; }

        public List<CaseAction> Actions { get; set; }
        public MessageResponse Message { get; set; }
    }
}
