 
using System.Collections.Generic;
 

namespace DPO.Common
{
    public class CommissionMultipliersModel : SearchCommissionMultiplier
    {
        public CommissionMultipliersModel()
        {
            Items = new PagedList<CommissionMultiplierListModel>(new List<CommissionMultiplierListModel>());
        }

        public PagedList<CommissionMultiplierListModel> Items { get; set; }
    }
}
