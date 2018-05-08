using DPO.Common;
using DPO.Data;
using DPO.Model.Light;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain 
{
    public interface IOrderServices
    {
        ServiceResponse GetOrderModel(UserSessionModel user, OrderViewModelLight qto); 
        ServiceResponse GetOrderModel(long orderId);  
        ServiceResponse GetOrdersForGrid(UserSessionModel user, SearchOrders search); 
        ServiceResponse Approve(UserSessionModel user, OrderViewModelLight model); 
        ServiceResponse Reject(UserSessionModel user, OrderViewModelLight model); 
        ServiceResponse ChangeOrderStatus(UserSessionModel user, OrderViewModelLight model, OrderStatusTypeEnum orderStatus); 
        ServiceResponse ChangeStatus(UserSessionModel user, OrderViewModelLight model, OrderStatusTypeEnum status);
        ServiceResponse PostModel(UserSessionModel admin, OrderViewModelLight model); 
    }
}
