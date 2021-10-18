using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace MBADCases.Models
{
    public class CaseTypeResponse
    {
        public CaseTypeResponse(CaseType ocase, Message oms)
        {
            if (ocase != null) { _id = ocase._id; } else { oms.Messagecode = "417"; }
            
            //Casenumber = ocase.Casenumber;
            Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id , Messagedesc=oms.MessageDesc};
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public MessageResponse Message { get; set; }
    }
}
