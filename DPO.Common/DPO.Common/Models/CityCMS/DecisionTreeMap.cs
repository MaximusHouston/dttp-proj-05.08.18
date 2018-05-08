 
using System.Collections.Generic;

namespace DPO.Common
{
    public class DecisionTreeMap
    {
        public long ConfigId { get; set; }
        public List<int> Systems { get; set; }
        public List<int> Dependancies { get; set; }
    }
}
