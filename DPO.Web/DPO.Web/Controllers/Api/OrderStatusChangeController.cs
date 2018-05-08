using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DPO.Common;
using DPO.Domain;
using DPO.Model.Light;

namespace DPO.Web.Controllers.Api
{
    public class OrderStatusChangeController : BaseApiController
    {
        public OrderServices orderService = new OrderServices();
        public ServiceResponse response = new ServiceResponse();

        [ActionName("UpdateOrderStatus")]
        [HttpPut]
        public ServiceResponse UpdateOrderStatus([FromUri] Int32 orderId, int orderStatus )
        {
#pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
           if(orderId == null)
#pragma warning restore CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
           {
               response.Messages.AddError("OrderId is null");
           }
#pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
           if(orderStatus == null)
#pragma warning restore CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
           {
               response.Messages.AddError("OrderStatus is null");
           }
          
           if(response.HasError)
           {
               return response;
           }
           else
           {
               OrderViewModelLight orderVMLight = orderService.GetOrderModel(orderId).Model as OrderViewModelLight;
               this.response = orderService.ChangeStatus(this.CurrentUser,orderVMLight, (OrderStatusTypeEnum)orderStatus);
           }
           return response;
        }

    }
}