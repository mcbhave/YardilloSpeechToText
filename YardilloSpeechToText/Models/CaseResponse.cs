using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace MBADCases.Models
{
    public class CaseResponse
    {
        public CaseResponse(string  id, Message oms)
        {
            _id = id;
            //Casenumber = ocase.Casenumber;
            Message = new MessageResponse() { Messagecode = oms.Messagecode,   Messageype = oms.Messageype, _id = oms._id ,Messagedesc = oms.MessageDesc};
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

      
        public MessageResponse Message { get; set; }
    }
}
