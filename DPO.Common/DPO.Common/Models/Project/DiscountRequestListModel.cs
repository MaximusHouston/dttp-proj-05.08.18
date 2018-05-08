 
using System.Collections.Generic;
 
namespace DPO.Common
{
    public class DiscountRequestListModel : SearchDiscountRequests
    {
        public DiscountRequestListModel()
        {
            Items = new PagedList<DiscountRequestModel>(new List<DiscountRequestModel>());
        }

        public PagedList<DiscountRequestModel> Items { get; set; }
    }
}
