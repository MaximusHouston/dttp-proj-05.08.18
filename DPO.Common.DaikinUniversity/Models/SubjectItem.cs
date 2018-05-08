using Newtonsoft.Json;

namespace DPO.Common.DaikinUniversity
{
    public class SubjectItem
    {
        [JsonProperty("Id")]
        public string ID { get; set; }

        [JsonProperty("ParentId")]
        public string ParentID { get; set; }

        public string ParentTitle { get; set; }

        public string Title { get; set; }
    }
}