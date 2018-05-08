 
using System.Collections.Generic;

namespace DPO.Common
{
    public class FloorConfigurationIndoorUnitsModel : PageModel
    {
        public List<int> indoorUnit { get; set; }

        //not used in JSON
        public string buildingName { get; set; }
        public string configName { get; set; }
        public string floorName { get; set; }
        public long? floorId { get; set; }
        public long? buildingId { get; set; }
        public long? configId { get; set; }
        public List<CitySystemModel> systemsToPickFrom { get; set; }
        public List<FloorConfigurationIndoorUnitModel> indoorUnitsToEdit { get; set; }
    }
}
