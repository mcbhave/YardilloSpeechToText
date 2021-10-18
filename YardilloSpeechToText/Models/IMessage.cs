using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
namespace MBADCases.Models
{
    interface IMessage
    {

        public Message Message { get; set; }
      
    }
}
