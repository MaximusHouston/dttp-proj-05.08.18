 
using System.Collections.Generic;

namespace DPO.Common
{
    public class FloorConfigurationLayoutsModel : PageModel
    {
        public List<FloorConfigurationLayoutModel> layout { get; set; }

        //not used in JSON
        public string buildingName { get; set; }
        public string floorName { get; set; }
        public long? floorId { get; set; }
        public long? buildingId { get; set; }
        public string configName { get; set; }
        public long configId { get; set; }

        public List<CitySystemModel> systems { get; set; }
        public List<DecisionTreeCellMap> dependancies { get; set; }
        public DecisionTreeMap decisionTree { get; set; }
    }
}
