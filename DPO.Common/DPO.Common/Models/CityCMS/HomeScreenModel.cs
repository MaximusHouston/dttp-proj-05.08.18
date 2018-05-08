 
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class HomeScreenModel : PageModel
    {
        [Required(ErrorMessageResourceName = "PleaseEnterTitleValue", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string title { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterDescription", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string bodytext { get; set; }

        [Required(ErrorMessageResourceName = "PleaseEnterPrivacyPolicy", ErrorMessageResourceType = typeof(Resources.ResourceUI))]
        public string privacypolicy { get; set; }

        public BillboardModel BillboardItems { get; set; }
    }
}
