 
using System.ComponentModel.DataAnnotations;
 

namespace DPO.Common
{
    public class FloorConfigurationModel : PageModel
    {
        public int id { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterNameValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string name { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterSystemNameValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string systemName { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterSystemSizeValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string systemSize { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterSystemTypeValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string systemType { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterEnergyValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string energy { get; set; }
        
        public string overlayImage { get; set; }
        public string systemImage { get; set; }

        public int floorId { get; set; }

        public FloorConfigurationIndoorUnitsModel indoorUnits { get; set; }

        public FloorConfigurationLayoutsModel layouts { get; set; }

        public bool? isAlternate { get; set; }
        
        //not used in JSON
        public string buildingName { get; set; }
        public string floorName { get; set; }
        public long? buildingId { get; set; }
    }
}
