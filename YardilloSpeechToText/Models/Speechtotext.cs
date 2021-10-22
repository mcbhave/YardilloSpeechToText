using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBADCases.Models
{
    public class Speechtotext
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string TranId { get; set; }
        public string audio_url { get; set; }
        public int audio_duration { get; set; }

        public int usage_count_audio { get; set; }

        public int usage_count_feedback { get; set; }
        public object webhook_url { get; set; }
        public string highlite_start_tag { get; set; }

        public string highlite_end_tag { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public string Createdate { get; set; }
        public string Createuser { get; set; }
        public string Updatedate { get; set; }
        public string Updateuser { get; set; }
        public AAIResponse AIResponse { get; set; }

        public string error { get; set; }

    }
    
    public class Speechtotextattr
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string TranId { get; set; }
        public string highlite_start_tag { get; set; }
        public string highlite_end_tag { get; set; }
        public string Createdate { get; set; }
        public string Createuser { get; set; }
        public string Updatedate { get; set; }
        public string Updateuser { get; set; }
    }
    public class Speechtotextmap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Labeltext { get; set; }
        public string Phrasetext { get; set; }
        public string Feedbacktext { get; set; }

        public string Createdate { get; set; }
        public string Createuser { get; set; }

        public string Updatedate { get; set; }
        public string Updateuser { get; set; }
    }
     
    public class AAIRequest
    {
        public string audio_url { get; set; }
        public string webhook_url { get; set; }
      
    }

    public class AAITranscriptResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        //public string acoustic_model { get; set; }
        //public object audio_duration { get; set; }
        //public string audio_url { get; set; }
        //public object confidence { get; set; }
        //public object dual_channel { get; set; }
        //public bool format_text { get; set; }
        //public string language_model { get; set; }
        //public bool punctuate { get; set; }
        //public object text { get; set; }
        //public object utterances { get; set; }
        //public object webhook_status_code { get; set; }
        //public object webhook_url { get; set; }
        //public object words { get; set; }
    }
    public class AAIErrorResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public string  error { get; set; }
    }

        public class AAIResponse
    {
       
        public string id { get; set; }
        public string language_model { get; set; }
        public string acoustic_model { get; set; }
        public string status { get; set; }
        public string audio_url { get; set; }
        public string text { get; set; }

        public string highlitedtext { get; set; }
        public List<owords> words { get; set; }
        public object utterances { get; set; }
        public object confidence { get; set; }
        public int audio_duration { get; set; }
        public bool punctuate { get; set; }
        public bool format_text { get; set; }
        public object dual_channel { get; set; }
        public object webhook_url { get; set; }
        public object webhook_status_code { get; set; }
        public bool speed_boost { get; set; }
        public object auto_highlights_result { get; set; }
        public bool auto_highlights { get; set; }
        public object audio_start_from { get; set; }
        public object audio_end_at { get; set; }
        public List<object> word_boost { get; set; }
        public object boost_param { get; set; }
        public bool filter_profanity { get; set; }
        public bool redact_pii { get; set; }
        public bool redact_pii_audio { get; set; }
        public object redact_pii_audio_quality { get; set; }
        public object redact_pii_policies { get; set; }
        public object redact_pii_sub { get; set; }
        public bool speaker_labels { get; set; }
        public bool content_safety { get; set; }
        public bool iab_categories { get; set; }
        public ContentSafetyLabels content_safety_labels { get; set; }
        public IabCategoriesResult iab_categories_result { get; set; }
        public List<phrase> phrases { get; set; }

        public List<feedback> feedback { get; set; }
        public string error { get; set; }
        public List<commonname> commonnames { get; set; }
        public int feedbackcount { get; set; }
        public int totalfeedbackcount { get; set; }

       
    }

    public class commonname
    {
        public string name { get; set; }
        public string tip { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public int occurances { get; set; }
    }
    public class phrase
    {
        public object confidence { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public string text { get; set; }
        public string feedback { get; set; }

        public int occurances { get; set; }
    }
    public class feedback
    {
        public object confidence { get; set; }
        public int end { get; set; }
        public int start { get; set; }

        public string phrase { get; set; }
        public string text { get; set; }
        
        public int occurances { get; set; }
    }
    public class ContentSafetyLabels
    {
    }

    public class IabCategoriesResult
    {
    }

    public class owords
    {
        public object confidence { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public string text { get; set; }
        public string feedback { get; set; }
    }

    public class Speechwebhook
    {
        public string status { get; set; }
        public string transcript_id { get; set; }
    }
    public class SppechToTextResponse
    {
        public string _id { get; set; }
        public string tenantid { get; set; }

        public string userid { get; set; }
        public string status { get; set; }
        public string error { get; set; }
        public int audio_duration { get; set; }

        public int usage_count_audio { get; set; }

        public int usage_count_feedback { get; set; }
        public int feedbackcount { get; set; }
        public int totalfeedbackcount { get; set; }
        public object confidence { get; set; }
        public string text { get; set; }

        public string highlitedtext { get; set; }
        public int total_words { get; set; }
        public int total_phrases { get; set; }
        public int total_commonnames { get; set; }
        public List<owords> words { get; set; }
        public List<phrase> phrases { get; set; }
        public List<commonname> commonnames { get; set; }

        public List<feedback> feedback { get; set; }



    }

}
