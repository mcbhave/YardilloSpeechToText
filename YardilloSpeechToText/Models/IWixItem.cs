using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MBADCases.Models
{
    interface IWixItem
    {
        
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string _id { get; set; }


            public string _owner { get; set; }

 


        
    }
}
