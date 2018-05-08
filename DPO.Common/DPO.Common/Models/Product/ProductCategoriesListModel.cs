 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class ProductCategoriesModel : PageModel
    {
        public int ProductFamilyId { get; set; }

        public string ProductFamilyName { get; set; }

        public List<TabModel> ProductFamilyTabs { get; set; }

        public List<CatelogListModel> Items { get; set; }
    }
}
