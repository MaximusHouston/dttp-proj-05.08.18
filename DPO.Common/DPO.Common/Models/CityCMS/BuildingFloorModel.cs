 
namespace DPO.Common
{
    public class BuildingFloorModel : PageModel
    {
        public int? buildingId { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int? typeId { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public string floorImage { get; set; }
        public int? applicationId { get; set; }
        public string backgroundImage { get; set; }
        public string icon { get; set; }

        public FloorConfigurationsModel configurations { get; set; }
        public FloorConfigurationsModel alternativeConfigurations { get; set; }

        //web floors
        public BuildingLinksModel links { get; set; }


        //only used for breadcrumbs
        public string buildingName { get; set; }
    }
}
