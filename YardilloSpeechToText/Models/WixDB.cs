using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MBADCases.Models.WixDB;

namespace MBADCases.Models
{
    public class WixDB
    {
        public partial class Schema
        {
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("Id")]
            public string Id { get; set; }

            [JsonProperty("allowedOperations")]
            public string[] AllowedOperations { get; set; }

            [JsonProperty("maxPageSize")]
            public long MaxPageSize { get; set; }

            [JsonProperty("ttl")]
            public long ttl { get; set; }

            [JsonProperty("fields")]
            public IDictionary<string, FieldValue> Fields { get; set; }

            [JsonProperty("defaultSort")]
            public defaultSort defaultSort { get; set; }



        }
        public class DBSchemas
        {
            [JsonProperty("Schemas")]
            public List<Schema> Schemas { get; set; }


        }
        public partial class RequestContext
        {
            public RequestContext(){
                Settings = new Settings();
            }

            [JsonProperty("settings")]
            public Settings Settings { get; set; }

            [JsonProperty("instanceId")]
            public Guid InstanceId { get; set; }

            [JsonProperty("installationId")]
            public Guid InstallationId { get; set; }

            [JsonProperty("memberId")]
            public Guid MemberId { get; set; }

            [JsonProperty("role")]
            public string Role { get; set; }
        }
        public class provision
        {
            public provision()
            {
                RequestContext = new RequestContext();
            }
            public RequestContext RequestContext { get; set; }

        }

        public class  data 
        {
            public data()
            {
                RequestContext = new RequestContext();
            }
            public RequestContext RequestContext { get; set; }
            public string collectionName { get; set; }
           
            public dynamic item  { get; set; }

            public string itemId { get; set; }

        }

       

public class DataItem<T>
    {
        private T _value;

        public T item
        {
            get
            {
                // insert desired logic here
                return _value;
            }
            set
            {
                // insert desired logic here
                _value = value;
            }
        }

        public static implicit operator T(DataItem<T> value)
        {
            return value.item;
        }

        public static implicit operator DataItem<T>(T value)
        {
            return new DataItem<T> { item = value };
        }
            
    }
    public class DataCount
        {
            public int totalCount { get; set; }
           
            
        }
        public class DataItems
        {
       
            public List<item> item {get;set;}
            public DataItems()
            {
                item = new List<item>();
            }
             
            public int totalCount { get; set; }
            
        }
       
        public class FindItems<T>
        {

            public List<T> items { get; set; }
            public FindItems()
            {
                items = new List<T>();
            }

            public int totalCount { get; set; }

        }
        public class item
        {
            public string _id { get; set; }
            public string _owner { get; set; }
        }
        public class DateAdded
        {
            [JsonProperty("$date")]
            public string date { get; set; }
        }
        public class find
        {
            public find()
            {
                RequestContext = new RequestContext();
            }
            public RequestContext RequestContext { get; set; }
            public string[] schemaIds { get; set; }
        }
        public class Settings
        {
            [JsonProperty("authorization")]
            public string Authorization { get; set; }
            [JsonProperty("yauthtenantname")]
            public string Yauthtenantname { get; set; }
            
        }
    }

    public class defaultSort
    {
        public string fieldName { get; set; }
        public string direction { get; set; }
    }
    public class FieldValue
    {
        
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("queryOperators")]
            public string[] QueryOperators { get; set; }
         

    }

}
