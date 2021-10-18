using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class PostmarkEmail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Tag { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public string ReplyTo { get; set; }
        public List<PostmarkEmailHeader> Headers { get; set; }
        public bool TrackOpens { get; set; }
        public string TrackLinks { get; set; }
        public List<PostmarkEmailAttachment> Attachments { get; set; }
        public PostmarkEmailMetadata Metadata { get; set; }
        public string MessageStream { get; set; }
        public class PostmarkEmailHeader
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class PostmarkEmailAttachment
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public string ContentType { get; set; }
            public string ContentID { get; set; }
        }

        public class PostmarkEmailMetadata
        {
            public string color { get; set; }

            [JsonProperty("client-id")]
            public string ClientId { get; set; }
        }

         
           
         

    }
}
