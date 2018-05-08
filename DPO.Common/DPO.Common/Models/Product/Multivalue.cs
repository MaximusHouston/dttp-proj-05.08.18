 
using System.Collections.Generic; 

namespace DPO.Common.Models
{
    public class Multivalue
    {
        public string AttributeID { get; set; }
        public string title { get; set; }
        public List<string> Value { get; set; } 
    }
}
