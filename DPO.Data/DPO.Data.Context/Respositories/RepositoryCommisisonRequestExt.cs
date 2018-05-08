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


        //################################################################
        // Get all project in groups below(or inclusive) user and users own projects
        //################################################################
        public IQueryable<CommissionRequest> QueryCommissionRequestsViewable(UserSessionModel user)
        {
            IQueryable<CommissionRequest> query;

            
            query = from commission in this.Context.CommissionRequests
                    join project in QueryProjectsViewableByUser(user) on commission.ProjectId equals project.ProjectId
                    select commission;

            return query;

        }

        public CommissionRequest CommissionRequestCreate(long projectId, long quoteId)
        {
            var entity = new CommissionRequest();

            entity.CommissionRequestId = this.Context.GenerateNextLongId();

            entity.CommissionRequestStatusTypeId = (int)CommissionRequestStatusTypeEnum.NewRecord;

            entity.ProjectId = projectId;

            entity.QuoteId = quoteId;

            this.Context.CommissionRequests.Add(entity);

            return entity;
        }


        public IQueryable<CommissionRequest> QueryCommissionRequestsViewableBySearch(UserSessionModel user, SearchCommissionRequests search)
        {

            var query = QueryCommissionRequestsViewable(user);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(user, query, search); // Must be Last

            return query;
        }


        private IQueryable<CommissionRequest> Filter(IQueryable<CommissionRequest> query, SearchCommissionRequests search)
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
                query = query.Where(s => s.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending);
            }

            if (!string.IsNullOrWhiteSpace(search.Filter))
            {
                query = query.Where(s => s.Notes.Contains(search.Filter) || s.ResponseNotes.Contains(search.Filter));
            }

            return query;
        }

        private IQueryable<CommissionRequest> Sort(IQueryable<CommissionRequest> query, Search search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "commissionrequeststatustype":
                    query = (desc) ? query.OrderByDescending(s => s.CommissionRequestStatusType.Description) : query.OrderBy(s => s.CommissionRequestStatusType.Description);
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

        public bool ValidateCommissionEmails(List<string> emails)
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

        public List<string> GetInvalidCommissionEmails(List<string> emails)
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