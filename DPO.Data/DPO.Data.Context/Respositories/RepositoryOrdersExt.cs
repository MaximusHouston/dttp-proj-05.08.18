using EntityFramework.Extensions;
using DPO.Common;
using System.Reflection;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace DPO.Data
{
    public partial class Repository
    {
        public IQueryable<Order> Orders
        {
            get { return this.GetDbSet<Order>(); }
        }

        public IQueryable<Order> QueryOrdersViewableByUser(UserSessionModel user)
        {
            IQueryable<Order> query;


            query = from order in this.Context.Orders
                    join project in QueryProjectsViewableByUser(user) on order.Quote.ProjectId equals project.ProjectId
                    select order;

            return query;

        }

        public Order OrderCreate(long projectId, long quoteId)
        {
            var entity = new Order();

            entity.OrderId = this.Context.GenerateNextLongId();

            entity.OrderStatusTypeId = (byte)OrderStatusTypeEnum.NewRecord;

            entity.QuoteId = quoteId;

            this.Context.Orders.Add(entity);

            return entity;
        }

        public OrderItem OrderItemCreate(long orderId)
        {
            var entity = new OrderItem();

            entity.OrderItemId = this.Context.GenerateNextLongId();

            entity.OrderId = orderId;

            return entity;
        }

        public OrderAttachment OrderAttachmentCreate(long orderId)
        {
            var entity = new OrderAttachment();
            entity.OrderAttachmentId = this.Context.GenerateNextLongId();
            entity.OrderId = orderId;
            return entity;
        }

        public IQueryable<Order> QueryOrderViewableBySearch(UserSessionModel user, SearchOrders search)
        {
            IQueryable<Order> query;

            if (user.UserTypeId == UserTypeEnum.DaikinSuperUser)
            {
                query = from order in this.Context.Orders
                        select order;
            }
            else
            {
                query = QueryOrdersViewableByUser(user);
            }
                        

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            //query = Paging(user, query, search); // Must be Last

            return query;
        }


        private IQueryable<Order> Filter(IQueryable<Order> query, SearchOrders search)
        {
            if (search == null) return query;


            if (search.ProjectId.HasValue)
            {
                query = query.Where(s => s.Quote.ProjectId == search.ProjectId);

            }
            if (search.QuoteId.HasValue)
            {
                query = query.Where(s => s.QuoteId == search.QuoteId);
            }

            if (search.SubmittedOrders)
            {
                query = query.Where(s => s.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted);
            }

            if (!string.IsNullOrWhiteSpace(search.Filter))
            {
                query = query.Where(s => s.Comments.Contains(search.Filter) || s.Comments.Contains(search.Filter));
            }

            return query;
        }

        private IQueryable<Order> Sort(IQueryable<Order> query, Search search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "orderstatustype":
                    query = (desc) ? query.OrderByDescending(s => s.OrderStatusType.Name) : query.OrderBy(s => s.OrderStatusType.Name);
                    break;
                case "quotetitle":
                    query = (desc) ? query.OrderByDescending(s => s.Quote.Title) : query.OrderBy(s => s.Quote.Title);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.SubmitDate).ThenByDescending(s => s.Quote.Title) : query.OrderBy(s => s.SubmitDate).ThenBy(s => s.Quote.Title);
                    break;
            }

            return query;
        }

        //TODO: Is this being used?
        public bool ValidateOrderEmails(List<string> emails)
        {

            List<bool> result = new List<bool>();

            foreach (string email in emails)
            {
                var query = (from e in this.Context.Users
                             where e.Email == email
                             select e.Email
                             ).Count();

                if (query > 0)
                {
                    result.Add(true);
                }
                else
                {
                    result.Add(false);
                }
            }

            if (result.Contains(false))
            {
                return false;
            }

            return true;
        }

        //TODO: Is this being used?
        public List<string> GetInvalidOrderEmails(List<string> emails)
        {
            List<string> InvalidEmails = new List<string>();


            foreach (string str in emails)
            {
                var email = str.Trim();

                var query = (from e in this.Context.Users
                             select e.Email
                             ).Where(em => em.Contains(email)).Count();

                if (query <= 0)
                {
                    InvalidEmails.Add(email);
                }
            }

            return InvalidEmails;

        }
    }
}
