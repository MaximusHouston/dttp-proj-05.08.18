using Newtonsoft.Json;

namespace DPO.Common.DaikinUniversity
{
    public class AvailabilityItem
    {
        public string Id { get; set; }

        public bool IncludeSubs { get; set; }

        public string SubType { get; set; }

        /// <summary>
        /// Multiple types of availabilities
        /// </summary>
        [JsonProperty("__type")]
        public string Type { get; set; }
    }
}