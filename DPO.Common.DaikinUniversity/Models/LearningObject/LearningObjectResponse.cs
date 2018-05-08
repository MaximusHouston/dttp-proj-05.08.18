using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common.DaikinUniversity
{
    public class LearningObjectResponse
    {
        [JsonProperty("trainingItem")]
        public LearningObject Object { get; set; }

        public string Result { get; set; }

        public string Reason { get; set; }
    }
}
