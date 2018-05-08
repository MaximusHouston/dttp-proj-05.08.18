 
using System.Collections.Generic;

namespace DPO.Common
{
    public class BuildingFloorsModel
    {
        public BuildingFloorsModel()
        {
            this.floor = new List<BuildingFloorModel>();
        }
        public List<BuildingFloorModel> floor { get; set; }
    }
}
