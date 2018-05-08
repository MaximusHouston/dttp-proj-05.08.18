 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class CommissionRequestListModel : SearchCommissionRequests
    {
        public CommissionRequestListModel()
        {
            Items = new PagedList<CommissionRequestModel>(new List<CommissionRequestModel>());
        }

        public PagedList<CommissionRequestModel> Items { get; set; }
    }
}
