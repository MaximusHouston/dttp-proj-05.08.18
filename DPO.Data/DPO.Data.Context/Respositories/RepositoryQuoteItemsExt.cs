//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;
using System.Reflection;

namespace DPO.Data
{

    public partial class Repository
    {

        public IQueryable<QuoteItem> QuoteItems
        {
            get { return this.GetDbSet<QuoteItem>(); }
        }

        //################################################################
        // QUOTE ITEMS
        //################################################################

        public QuoteItem QuoteItemCreate(Quote quote)
        {
            var entity = new QuoteItem();

            entity.QuoteItemId = this.Context.GenerateNextLongId();

            entity.Quote = quote;
            entity.QuoteId = quote.QuoteId;

            this.Context.QuoteItems.Add(entity);

            return entity;
        }

        public IQueryable<QuoteItem> QuoteItemsByQuoteId(UserSessionModel admin, long? quoteId)
        {
            var query = this.QuoteItemsQueryByUser(admin).Where(u => u.QuoteId == quoteId);
            return query;
        }



        public IQueryable<QuoteItem> QuoteItemQueryByQuoteItemId(UserSessionModel admin, long? quoteitemId)
        {
            return this.QuoteItemsQueryByUser(admin).Where(u => u.QuoteItemId == quoteitemId);
        }


        public IQueryable<QuoteItem> QuoteItemsQueryByUser(UserSessionModel user)
        {
            IQueryable<QuoteItem> query;

            if (user == null)
            {
                query = this.QuoteItems;
            }
            else
            {
                query = from items in this.QuoteItems
                        join quote in this.QueryQuotesViewableByUser(user) on items.QuoteId equals quote.QuoteId
                        select items;
            }

            return query;

        }

        public IQueryable<QuoteItem> QuoteItemsQueryBySearch(UserSessionModel admin, SearchQuoteItem search)
        {
            IQueryable<QuoteItem> query;

            query = QuoteItemsQueryByUser(admin);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(admin, query, search); // Must be Last

            return query;
        }


        private IQueryable<QuoteItem> Filter(IQueryable<QuoteItem> query, SearchQuoteItem search)
        {
            if (search == null) return query;


            if (search.QuoteId.HasValue)
            {
                query = query.Where(s => s.QuoteId == search.QuoteId);

            }
            if (search.QuoteItemId.HasValue)
            {
                query = query.Where(s => s.QuoteItemId == search.QuoteItemId);
            }

            return query;
        }

        private IQueryable<QuoteItem> Sort(IQueryable<QuoteItem> query, SearchQuoteItem search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "productclasscode":
                    query = (desc) ? query.OrderByDescending(s => s.Product.ProductClassCode) : query.OrderBy(s => s.Product.ProductClassCode);
                    break;
                case "productqty":
                    query = (desc) ? query.OrderByDescending(s => s.Quantity) : query.OrderBy(s => s.Quantity);
                    break;
                case "productnumber":
                    query = (desc) ? query.OrderByDescending(s => s.ProductNumber) : query.OrderBy(s => s.ProductNumber);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.QuoteItemId) : query.OrderBy(s => s.QuoteItemId);
                    break;
            }

            return query;
        }


    }
}