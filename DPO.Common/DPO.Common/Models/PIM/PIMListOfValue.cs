using System.Collections.Generic;

namespace DPO.Common
{
    public class PIMListOfValue
    {
        public string ID { get; set; }

        public List<KeyValuePair<string, string>> Values { get; set; }

        /// <summary>
        /// Identify whether or not this list of values uses IDs and Values, or just values
        /// </summary>
        public bool UseValueID { get; set; }

        public string IDPattern { get; set; }

        public PIMListOfValue()
        {
            Values = new List<KeyValuePair<string, string>>();
        }
    }
}