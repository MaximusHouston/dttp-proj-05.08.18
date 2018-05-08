 
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class CitySystemModel
    {
        public int id { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterNameValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string name { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterDescription", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string description { get; set; }

        public string image { get; set; }
        public string icon { get; set; }
    }
}
