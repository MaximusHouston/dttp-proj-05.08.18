using DPO.Common;
using DPO.Data;
using DPO.Model.Light;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain 
{
    public interface IERPServiceProvider
    {
        #region OrderServices
        ServiceResponse CheckPONumber(string poNumber, string erpAccountId);

        ServiceResponse CheckPONumberExist(string erpAccountId, string poNumber);

        ServiceResponse CheckWithMapicsBeforeSavingToDb(List<OrderItemsViewModel> orderItemsVm, Order order,
           OrderViewModelLight model);

        string SendOrderRequestToMapics(string xmlRequest);

        ServiceResponse ProcessMapicsOrderSeriveResponse(string xmlResponse);

        #endregion

    }
}
