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
     
      public IQueryable<Quote> Quotes
      {
          get { return this.GetDbSet<Quote>(); }
      }

      //################################################################
      // QUOTES
      //################################################################
      public Quote QuoteCreate(long? projectId)
      {
          if (projectId == null) return null;

          var entity = new Quote();

          entity.QuoteId = this.Context.GenerateNextLongId();

          entity.ProjectId = projectId.Value;

          this.Context.Quotes.Add(entity);

          return entity;
      }

      public Quote QuoteCreate(Project project)
      {
          if (project == null) return null;

          var entity = new Quote();

          entity.QuoteId = this.Context.GenerateNextLongId();

          entity.Project = project;

          this.Context.Quotes.Add(entity);

          return entity;
      }

      public void QuoteRemove(Quote entity)
      {
          this.Context.Quotes.Remove(entity);
      }


      public IQueryable<Quote> QueryQuoteViewableByQuoteId(UserSessionModel admin, long? quoteId)
      {
          var query = this.QueryQuotesViewableByUser(admin).Where(u => u.QuoteId == quoteId);

          if (!admin.HasAccess(SystemAccessEnum.UndeleteProject))
          {
              query = query.Where(p => p.Deleted == false);
          }

          return query;
      }

      public int QuoteGetNextVersionNumber(long projectId)
      {
          var result = this   .Quotes
                        .Where(u => u.ProjectId == projectId)
                        .Max(q => (int?)q.Revision);

          return (result == null) ? 1 : result.Value + 1;
      }

      public IQueryable<Quote> QuoteGetCurrentActiveQuote(UserSessionModel user, long projectId)
      {
          var query = this.QueryQuotesViewableByUser(user).Where(q => q.ProjectId == projectId && q.Active == true);

          return query;
      }

      public IQueryable<Quote> QueryQuotesViewableBySearch(UserSessionModel user, SearchQuote search)
      {

          var query = QueryQuotesViewableByUser(user);

          query = Filter(query, search);

          if (search != null && search.ReturnTotals)
          {
              search.TotalRecords = query.Count();
          }

          query = Sort(query, search);

          query = Paging(user, query, search); // Must be Last

          return query;
      }

      private IQueryable<Quote> QueryQuotesViewableByUser(UserSessionModel user)
      {
          IQueryable<Quote> query;

          if (user == null)
          {
              query = this.Quotes;
          }
          else
          {
              query = from quote in this.Quotes
                      join project in this.QueryProjectsViewableByUser(user) on quote.ProjectId equals project.ProjectId
                      select quote;
          }

          if (!user.HasAccess(SystemAccessEnum.UndeleteProject))
          {
              query = query.Where(q => q.Deleted == false);
          }

          return query;

      }

      private IQueryable<Quote> Filter(IQueryable<Quote> query, SearchQuote search)
      {
          if (search == null) return query;


          if (search.ProjectId.HasValue)
          {
              query = query.Where(s => s.ProjectId == search.ProjectId);

          }
          if (search.QuoteId.HasValue)
          {
              query = query.Where(s => s.QuoteId == search.QuoteId);
          }

          return query;
      }

      private IQueryable<Quote> Sort(IQueryable<Quote> query, SearchQuote search)
      {
          if (search == null) return query;

          string sortcolumn = (search.SortColumn + "").ToLower();

          bool desc = search.IsDesc;

          switch (sortcolumn)
          {
              case "title":
                  query = (desc) ? query.OrderByDescending(s => s.Title) : query.OrderBy(s => s.Title);
                  break;
              default:
                  query = (desc) ? query.OrderByDescending(s => s.Revision) : query.OrderBy(s => s.Revision);
                  break;
          }

          return query;
      }




   }
}