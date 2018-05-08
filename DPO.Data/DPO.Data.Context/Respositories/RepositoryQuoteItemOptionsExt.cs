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

        public IQueryable<QuoteItemOption> QuoteItemOptions
        {
            get { return this.GetDbSet<QuoteItemOption>(); }
        }

        //public QuoteItemOption QuoteItemOptionCreate()
        //{
        //    var entity = new QuoteItemOption();

        //    entity.QuoteItemOptionId = this.Context.GenerateNextLongId();

        //    this.Context.QuoteItemOptions.Add(entity);

        //    return entity;
        //}
        public QuoteItemOption QuoteItemOptionCreate(QuoteItem quoteItem)
        {
            var entity = new QuoteItemOption();

            entity.QuoteItemOptionId = this.Context.GenerateNextLongId();

            entity.QuoteItemId = quoteItem.QuoteItemId;
            entity.QuoteItem = quoteItem;

            entity.QuoteId = quoteItem.QuoteId;
            entity.BaseProductId = (long)quoteItem.ProductId;
            
            this.Context.QuoteItemOptions.Add(entity);

            return entity;
        }


        public IQueryable<QuoteItemOption> QuoteItemOptionsByQuoteItemId(UserSessionModel admin, long? quoteItemId)
        {
            var query = this.QuoteItemOptionsByQueryByUser(admin).Where(u => u.QuoteItemId == quoteItemId);
            return query;
        }

        public IQueryable<QuoteItemOption> QuoteItemOptionsByQueryByUser(UserSessionModel user)
        {
            IQueryable<QuoteItemOption> query;

            if (user == null)
            {
                query = this.QuoteItemOptions;
            }
            else
            {
                query = from items in this.QuoteItemOptions
                        join quote in this.QueryQuotesViewableByUser(user) on items.QuoteId equals quote.QuoteId
                        select items;
            }

            return query;

        }

    }
}
