using Newtonsoft.Json;

namespace DPO.Common.DaikinUniversity
{
    public class CompetencyItem
    {
        [JsonProperty("Id")]
        public string ID { get; set; }

        public string Title { get; set; }
    }
}