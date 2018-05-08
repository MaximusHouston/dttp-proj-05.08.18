using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;

namespace DPO.Data
{

    public partial class Repository
    {

        public IQueryable<OrderItemOption> OrderItemOptions
        {
            get { return this.GetDbSet<OrderItemOption>(); }
        }

     
        public OrderItemOption OrderItemOptionCreate(OrderItem orderItem)
        {
            var entity = new OrderItemOption();

            entity.OrderItemOptionId = this.Context.GenerateNextLongId();

            entity.OrderItemId = orderItem.OrderItemId;
            entity.OrderId = orderItem.OrderId;
            entity.BaseProductId = (long)orderItem.ProductId;

            this.Context.OrderItemOptions.Add(entity);

            return entity;
        }


        public IQueryable<OrderItemOption> OrderItemOptionsByOrderItemId(UserSessionModel user, long? orderItemId)
        {
            var query = this.OrderItemOptionsByQueryByUser(user).Where(u => u.OrderItemId == orderItemId);
            return query;
        }

        public IQueryable<OrderItemOption> OrderItemOptionsByQueryByUser(UserSessionModel user)
        {
            IQueryable<OrderItemOption> query;

            if (user == null)
            {
                query = this.OrderItemOptions;
            }
            else
            {
                query = from items in this.OrderItemOptions
                        join order in this.QueryOrdersViewableByUser(user) on items.OrderId equals order.OrderId
                        select items;
            }

            return query;

        }

    }
}
