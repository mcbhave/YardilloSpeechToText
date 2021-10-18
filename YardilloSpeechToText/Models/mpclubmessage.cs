using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class mpclubmessage
    {
        public MpclubmessageResponse response { get; set; } 
    }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class mpclubmessageResult
        {
        public string message_text { get; set; }
        public string type_of_message_option_os_message_types { get; set; }

        [JsonProperty("Created By")]
        public string CreatedBy { get; set; }

        [JsonProperty("Created Date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("Modified Date")]
        public DateTime ModifiedDate { get; set; }
        public string publish_text { get; set; }
        public bool sent_to_all_boolean { get; set; }
        public string subscription_type_option_os_subscription_types { get; set; }
        public string _id { get; set; }
        public string sun_signs_option_os_astro_signs { get; set; }
        public string life_path_option_os_numerology_life__ { get; set; }
        public string enneagram_type_option_os_enneagram_types { get; set; }
        public string mb_type1_option_myers_briggs_types { get; set; }

        public string yardillo_location { get; set; }
    }

    public class MpclubmessageResponse
    {
            public int cursor { get; set; }
            public List<mpclubmessageResult> results { get; set; }
            public int remaining { get; set; }
            public int count { get; set; }
        }

      


     
}
