using System;
using System.Collections.Generic;
 

namespace DPO.Common
{
    public class BuildingLinksModel
    {
        public List<BuildingLinkModel> link { get; set; }

        // not used in json
        public String buildingName { get; set; }
    }
}
