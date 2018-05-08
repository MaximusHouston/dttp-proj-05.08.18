 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class FloorConfigurationLayoutNodeModel
    {
        //id = system id
        public int id { get; set; }
        public List<FloorConfigurationLayoutNodeModel> node { get; set; }
    }
}
