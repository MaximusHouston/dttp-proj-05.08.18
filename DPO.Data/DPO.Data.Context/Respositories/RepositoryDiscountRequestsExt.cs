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
using System.Data.Entity.Core.Objects.DataClasses;

namespace DPO.Data
{

    public partial class Repository
    {

        public IQueryable<DiscountRequest> DiscountRequests
        {
            get { return this.GetDbSet<DiscountRequest>(); }
        }

        //################################################################
        // Get all project in groups below(or inclusive) user and users own projects
        //################################################################
        public IQueryable<DiscountRequest> QueryDiscountRequestsViewable(UserSessionModel user)
        {
            IQueryable<DiscountRequest> query;


            query = from discount in this.Context.DiscountRequests
                    join project in QueryProjectsViewableByUser(user) on discount.ProjectId equals project.ProjectId
                    select discount;

            return query;

        }

        public DiscountRequest DiscountRequestCreate(long projectId, long quoteId)
        {
            var entity = new DiscountRequest();

            entity.DiscountRequestId = this.Context.GenerateNextLongId();

            entity.DiscountRequestStatusTypeId = (int)DiscountRequestStatusTypeEnum.NewRecord;

            entity.ProjectId = projectId;

            entity.QuoteId = quoteId;

            this.Context.DiscountRequests.Add(entity);

            return entity;
        }


        public IQueryable<DiscountRequest> QueryDiscountRequestsViewableBySearch(UserSessionModel user, SearchDiscountRequests search)
        {

            var query = QueryDiscountRequestsViewable(user);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(user, query, search); // Must be Last

            return query;
        }


        private IQueryable<DiscountRequest> Filter(IQueryable<DiscountRequest> query, SearchDiscountRequests search)
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

            if (search.PendingRequests)
            {
                query = query.Where(s => s.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending);
            }

            if (!string.IsNullOrWhiteSpace(search.Filter))
            {
                query = query.Where(s => s.Notes.Contains(search.Filter) || s.ResponseNotes.Contains(search.Filter));
            }

            return query;
        }

        private IQueryable<DiscountRequest> Sort(IQueryable<DiscountRequest> query, Search search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "discountrequeststatustype":
                    query = (desc) ? query.OrderByDescending(s => s.DiscountRequestStatusType.Description) : query.OrderBy(s => s.DiscountRequestStatusType.Description);
                    break;
                case "quotetitle":
                    query = (desc) ? query.OrderByDescending(s => s.Quote.Title) : query.OrderBy(s => s.Quote.Title);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.RequestedOn).ThenByDescending(s => s.Quote.Title) : query.OrderBy(s => s.RequestedOn).ThenBy(s => s.Quote.Title);
                    break;
            }

            return query;
        }

        public bool ValidateEmails(List<string> emails)
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

            if(result.Contains(false))
            {
                return false;
            }

            return true;
        }

        public List<string> GetInvalidEmails(List<string> emails)
        {
            List<string> InvalidEmails = new List<string>();
           

            foreach (string str in emails)
            {
                var email = str.Trim();

                var query = (from e in this.Context.Users
                             select e.Email
                             ).Where(em => em == email).Count();

                if(query <= 0)
                {
                    InvalidEmails.Add(email);
                }
            }

            return InvalidEmails;
            
        }

        


    }

}