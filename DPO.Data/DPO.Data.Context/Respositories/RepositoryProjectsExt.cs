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
using DPO.Model.Light;
using System.Reflection;
using System.Data.Entity.Core.Objects.DataClasses;
using Newtonsoft.Json;

namespace DPO.Data
{
    public partial class Repository
    {
        public IQueryable<Project> Projects
        {
            get { return this.GetDbSet<Project>(); }
        }

        public IQueryable<ProjectOpenStatusType> ProjectOpenStatusTypes
        {
            get { return this.GetDbSet<ProjectOpenStatusType>(); }
        }

        public IQueryable<ProjectStatusType> ProjectStatusTypes
        {
            get { return this.GetDbSet<ProjectStatusType>(); }
        }

        public IQueryable<ProjectType> ProjectTypes
        {
            get { return this.GetDbSet<ProjectType>(); }
        }

        public IQueryable<ProjectExportType> ProjectExportTypes
        {
            get { return this.GetDbSet<ProjectExportType>(); }
        }

        public Project GetProjectOwnerAndBusiness(long? projectId)
        {
            return this.Context.Projects.Where(p => p.ProjectId == projectId)
                .Select(p => p).Include(p => p.Owner).Include(u => u.Owner.Business).FirstOrDefault();
        }

        public Project GetProjectByProjectId(long? projectId)
        {
            return this.Context.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
        }

        //################################################################
        // Get all project in groups below(or inclusive) user and users own projects
        //################################################################
        private IQueryable<Project> QueryProjectsViewableByUser(UserSessionModel user, SearchProject searchProject = null)
        {
            IQueryable<Project> query;

            if (user == null)
            {
                query = this.Projects;
            }
            else
            {
                // Does user have rights to see projects in his/her own group ?
                bool inclusive = user.HasAccess(SystemAccessEnum.ViewProjectsInGroup);

                query = from project in this.Projects
                        join owner in this.Context.Users on project.OwnerId equals owner.UserId
                        join groups in this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value, inclusive) on owner.GroupId equals groups.GroupId into Lg
                        from groups in Lg.DefaultIfEmpty()

                            // Return any in group tree or the users own projects
                        where (owner.GroupId == groups.GroupId) || project.OwnerId == user.UserId
                        select project;

                // Get all projects which have been transferred
                var transfersQuery =
                        from project in this.Projects
                        join transfers in this.Context.ProjectTransfers on new { user.UserId, project.ProjectId } equals new { transfers.UserId, transfers.ProjectId }
                        select project;

                // join the two
                query = query.Union(transfersQuery);

                bool removeDeletedProjects = false;

                //if user cannot see deleted projects to start with, filter them out
                if (!user.HasAccess(SystemAccessEnum.UndeleteProject))
                {
                    removeDeletedProjects = true;
                }
                else
                {
                    //I have permission, but no searchproject to override this
                    //if (searchProject == null) removeDeletedProjects = false;

                    //I have permission, and a search project, and I chose to show deleted projects
                    //if (searchProject != null && searchProject.ShowDeletedProjects == true) removeDeletedProjects = false;

                    //I have permission, and a search project, and I chose to NOT show deleted projects
                    if (searchProject != null && searchProject.ShowDeletedProjects == false) removeDeletedProjects = true;
                }

                if (removeDeletedProjects)
                {
                    query = query.Where(p => p.Deleted == false);
                }
            }

            return query;
        }

        private IQueryable<Project> QueryProjectsExportViewableByUser(UserSessionModel user, ProjectExportParameter exportParam = null)
        {
            IQueryable<Project> query;

            if (user == null)
            {
                query = this.Projects;
            }
            else
            {
                // Does user have rights to see projects in his/her own group ?
                bool inclusive = user.HasAccess(SystemAccessEnum.ViewProjectsInGroup);

                query = from project in this.Projects
                        join owner in this.Context.Users on project.OwnerId equals owner.UserId
                        join groups in this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value, inclusive) on owner.GroupId equals groups.GroupId into Lg
                        from groups in Lg.DefaultIfEmpty()

                            // Return any in group tree or the users own projects
                        where (owner.GroupId == groups.GroupId) || project.OwnerId == user.UserId
                        select project;

                // Get al projects which have been transferred
                var transfersQuery =
                        from project in this.Projects
                        join transfers in this.Context.ProjectTransfers on new { user.UserId, project.ProjectId } equals new { transfers.UserId, transfers.ProjectId }
                        select project;

                // join the two
                query = query.Union(transfersQuery);

                bool removeDeletedProjects = false;

                //if user cannot see deleted projects to start with, filter them out
                if (!user.HasAccess(SystemAccessEnum.UndeleteProject))
                {
                    removeDeletedProjects = true;
                }
                else
                {
                    //I have permission, and a search project, and I chose to NOT show deleted projects
                    if (exportParam != null && exportParam.ShowDeletedProjects == false) removeDeletedProjects = true;
                }

                if (removeDeletedProjects)
                {
                    query = query.Where(p => p.Deleted == false);
                }
            }

            return query;
        }

        //################################################################
        // Is project a unique name
        //################################################################
        public bool IsProjectNameUnique(UserSessionModel admin, string projectName)
        {
            bool isUnique = (from project in this.Projects
                         join owner in this.Context.Users on project.OwnerId equals owner.UserId
                         join bus in this.Context.Businesses on owner.BusinessId equals bus.BusinessId
                         where project.Name == projectName && !project.Deleted
                         select project.Name).Any();

            return isUnique;
        }

        //################################################################
        // Has project been transferred
        //################################################################
        public bool IsProjectTransferred(UserSessionModel admin, long projectId)
        {
            var query = this.Context.ProjectTransfers.Where(u => u.UserId == admin.UserId && u.ProjectId == projectId).Any();

            return query;
        }

        //################################################################
        // Get all project in groups below(or inclusive) user and users own projects
        //################################################################
        public IQueryable<Project> QueryProjectViewableByProjectId(UserSessionModel admin, long? id)
        {
            var query = this.QueryProjectsViewableByUser(admin).Where(u => u.ProjectId == id);

            return query;
        }

        //################################################################
        // Get all project in groups below(or inclusive) user and users own projects
        //################################################################
        public IQueryable<Project> QueryProjectsViewableBySearch(UserSessionModel user, SearchProject search)
        {
            return QueryProjectsViewableBySearch(user, search, false);
        }
        
        public IQueryable<Project> QueryProjectsExportViewableByParam(UserSessionModel user, ProjectExportParameter exportparam)
        {
            return QueryProjectsExportViewableByParam(user, exportparam, false);
        }

        public IQueryable<Project> QueryProjectsViewableBySearch(UserSessionModel user, SearchProject search, bool noPaging)
        {
            IQueryable<Project> query;

            //specifc override for person's permissions / daikin user status
            query = QueryProjectsViewableByUser(user, search);

            query = Filter(query, search);

            if (!noPaging)
            {
                if (search != null && search.ReturnTotals)
                {
                    search.TotalRecords = query.Count();
                }

                query = Paging(user, query, search);
            }

            return query;
        }

        private IQueryable<Project> Filter(IQueryable<Project> query, SearchProject search)
        {
            if (search == null) return query;

            if (search.ProjectId.HasValue)
            {
                query = query.Where(s => s.ProjectId == search.ProjectId);
            }

            if (search.ProjectStartDate.HasValue)
            {
                //"Registration Date"
                //"Bid Date"
                //"Estimated Close"
                //"Estimated Delivery"

                switch (search.DateTypeId)
                {
                    case 1:
                        query = query.Where(s => s.ProjectDate >= search.ProjectStartDate.Value);
                        break;
                    case 2:
                        query = query.Where(s => s.BidDate >= search.ProjectStartDate.Value);
                        break;
                    case 3:
                        query = query.Where(s => s.EstimatedClose >= search.ProjectStartDate.Value);
                        break;
                    case 4:
                        query = query.Where(s => s.EstimatedDelivery >= search.ProjectStartDate.Value);
                        break;
                }

                //  query = query.Where(s => s.ProjectDate >= search.ProjectStartDate.Value);
            }

            if (search.ProjectStartEnd.HasValue)
            {
                switch (search.DateTypeId)
                {
                    case 1:
                        query = query.Where(s => s.ProjectDate <= search.ProjectStartEnd.Value);
                        break;
                    case 2:
                        query = query.Where(s => s.BidDate <= search.ProjectStartEnd.Value);
                        break;
                    case 3:
                        query = query.Where(s => s.EstimatedClose <= search.ProjectStartEnd.Value);
                        break;
                    case 4:
                        query = query.Where(s => s.EstimatedDelivery <= search.ProjectStartEnd.Value);
                        break;
                }
                // query = query.Where(s => s.ProjectDate <= search.ProjectStartEnd.Value);
            }

            if (search.UserId.HasValue)
            {
                query = query.Where(s => s.OwnerId == search.UserId.Value);
            }

            if (search.BusinessId.HasValue)
            {
                query = query.Where(s => s.Owner.BusinessId == search.BusinessId.Value);
            }

            if (search.ProjectOpenStatusTypeId.HasValue)
            {
                query = query.Where(s => s.ProjectOpenStatusTypeId == search.ProjectOpenStatusTypeId.Value);
            }

            if (search.ProjectStatusTypeId.HasValue)
            {
                query = query.Where(s => s.ProjectStatusTypeId == (ProjectStatusTypeEnum)search.ProjectStatusTypeId.Value);
            }

            if (search.ProjectLeadStatusTypeId.HasValue)
            {
                query = query.Where(s => s.ProjectLeadStatusTypeId == search.ProjectLeadStatusTypeId);
            }

            if (search.ProjectDarComStatusTypeId.HasValue)
            {
                if (search.ProjectDarComStatusTypeId == ProjectDarComStatusTypeEnum.BuySell)
                {
                    query = query.Where(p => p.Quotes.Where(q => q.IsCommission == false && q.Active).Any());
                }
                if (search.ProjectDarComStatusTypeId == ProjectDarComStatusTypeEnum.Commission)
                {
                    query = query.Where(p => p.Quotes.Where(q => q.IsCommission == true && q.Active).Any());
                }
            }

            if (search.OnlyAlertedProjects)
            {
                var expireDate = DateTime.Now.AddDays(search.ExpirationDays ?? 0);
                query = query.Where(s =>
                    (s.Quotes.Where(q => q.RecalculationRequired && q.Active).Count() > 0
                        || s.Expiration <= expireDate));
            }

            if (!string.IsNullOrWhiteSpace(search.Filter))
            {
                query = query.Where(s => s.Name.Contains(search.Filter) || DPO.Data.DPOContext.ALMDecConvertToString(s.ProjectId).Contains(search.Filter));
            }

            return query;
        }

        public IQueryable<Project> QueryProjectsExportViewableByParam(UserSessionModel user, ProjectExportParameter exportparam, bool noPaging)
        {
            IQueryable<Project> query;

            //specifc override for person's permissions / daikin user status
            query = QueryProjectsExportViewableByUser(user, exportparam);

            query = Filter(user, query, exportparam);

            if (!noPaging)
            {
                if (exportparam != null && exportparam.ReturnTotals)
                {
                    exportparam.TotalRecords = query.Count();
                }

                //query = Paging(user, query, exportparam);
            }

            return query;
        }

        private IQueryable<Project> Filter(UserSessionModel currentUser, IQueryable<Project> query, ProjectExportParameter exportParam)
        {
            if (exportParam == null) return query;

            if (exportParam.ProjectId != 0)
            {
                query = query.Where(s => s.ProjectId == exportParam.ProjectId);
            }

            if (exportParam.Filter != null)
            {
                Filter filter = JsonConvert.DeserializeObject<Filter>(exportParam.Filter);
                query = Filter(currentUser, query, filter);
            }

            if (exportParam.Sort != null)
            {
                List<Sort> sort = JsonConvert.DeserializeObject<List<Sort>>(exportParam.Sort);
            }

            return query;
        }

        private IQueryable<Project> Filter(UserSessionModel currentUser, IQueryable<Project> query, Filter filter)
        {
            if (filter != null)
            {
                var rootFilters = filter.Filters;
                var rootLogic = filter.Logic;
                var rootFilterName = filter.Name;

                if (rootFilters != null && rootLogic == "and")
                {
                    query = ProcessFilterItemsAnd(query, rootFilters);
                }
                else if (rootFilters != null && rootLogic == "or" && rootFilterName != "search")// A or B --- (PairFilter)
                {
                    query = ProcessPairFilterOr(query, rootFilters);
                }
                else if (rootFilters != null && rootLogic == "or" && rootFilterName == "search")// A or B --- (PairFilter)
                {
                    query = ProcessSearchProjectFilter(query, rootFilters);// Project Search Box Filter
                }
            }

            if (!currentUser.HasAccess(SystemAccessEnum.RequestCommission) && !currentUser.HasAccess(SystemAccessEnum.ViewRequestedCommission) && !currentUser.HasAccess(SystemAccessEnum.ApprovedRequestCommission))
            {
                query = query.Where(p => p.Quotes.Any(a => a.Active == true && a.IsCommission == false));
            }

            return query;
        }

        private IQueryable<Project> ProcessSearchProjectFilter(IQueryable<Project> query, List<FilterItem> filters)
        {

            var value = filters[0].Value;

            query = query.Where(p => p.ProjectId.ToString().Contains(value) || p.Name.Contains(value) || (p.Owner.FirstName + " " + p.Owner.LastName).Contains(value) || p.Owner.Business.BusinessName.Contains(value));

            return query;
        }

        private IQueryable<Project> ProcessFilterItemsAnd(IQueryable<Project> query, List<FilterItem> filters)
        {
            foreach (var filterItem in filters)
            {
                var field = filterItem.Field;
                var op = filterItem.Operator;
                var val = filterItem.Value;//string
                var childLogic = filterItem.Logic;
                var childFilters = filterItem.Filters;
                var childFilterName = filterItem.Name;

                if (field != null) // A and B and C and ... (Single)
                {
                    query = ProcessSingleFilter(query, field, op, val);

                }
                if (childFilters != null && childFilters.Count > 0 && childLogic == "or" && childFilterName == "search") // A and (B or C) --- PairFilter Or
                {
                    query = ProcessSearchProjectFilter(query, childFilters);

                }
                else if (childFilters != null && childFilters.Count > 0 && childLogic == "or") // A and (B or C) --- PairFilter Or
                {
                    query = ProcessPairFilterOr(query, childFilters);

                }
                else if (childFilters != null && childFilters.Count > 0 && childLogic == "and") // A and (B and C) --- PairFilter And ==> same As 2 Single Filter Items And
                {
                    query = ProcessPairFilterAnd(query, childFilters);
                    //query = ProcessFilterItemsAnd(query,childFilters);
                }
            }

            return query;
        }

        private IQueryable<Project> ProcessPairFilterAnd(IQueryable<Project> query, List<FilterItem> filters)
        {
            query = ProcessFilterItemsAnd(query, filters);
            return query;
        }

        //ProcessSingleFilter
        private IQueryable<Project> ProcessSingleFilter(IQueryable<Project> query, string field, string op, string val)
        {
            switch (field)
            {
                case "projectTypeId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => p.ProjectTypeId == value);
                    }
                    break;

                case "projectStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => (int)p.ProjectStatusTypeId == value);
                    }
                    break;

                case "projectOpenStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => p.ProjectOpenStatusTypeId == value);
                    }
                    break;

                case "projectLeadStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => (int)p.ProjectLeadStatusTypeId == value);
                    }
                    break;

                case "businessName":
                    if (op == "eq")
                    {
                        var value = val;
                        query = query.Where(p => p.Owner.Business.BusinessName == value);
                    }
                    break;

                case "projectOwner":
                    if (op == "eq")
                    {
                        var value = val;
                        query = query.Where(p => (p.Owner.FirstName + " " + p.Owner.LastName) == value);
                    }
                    break;

                case "isCommission":
                    if (op == "eq")
                    {
                        var value = Boolean.Parse(val);
                        //query = query.Where(p => p.IsCommission == value);

                        query = query.Where(p => p.Quotes.Any(a => a.Active == true && a.IsCommission == value));

                        //query = from project in query
                        //        join quote in this.Context.Quotes on new { P = project.ProjectId, A = true } equals new { P = quote.ProjectId, A = quote.Active } into Lq
                        //        from quote in Lq.DefaultIfEmpty()
                        //        where quote.IsCommission == value
                        //        select project;
                    }
                    break;

                case "pricingStrategy":
                    if (op == "eq")
                    {
                        if (val != null)
                        {
                            if (val == "Commission")
                            {
                                //query = query.Where(p => p.IsCommission == true);
                                query = query.Where(p => p.Quotes.Any(a => a.Active == true && a.IsCommission == true));
                            }
                            else if (val == "Buy/Sell")
                            {
                                //query = query.Where(p => p.IsCommission == false);
                                query = query.Where(p => p.Quotes.Any(a => a.Active == true && a.IsCommission == false));
                            }
                        }
                    }
                    break;

                case "alert":
                    if (op == "eq")
                    {
                        var value = Boolean.Parse(val);
                        //query = query.Where(p => p.Alert == value);
                        if (value == true)
                        {
                            query = query.Where(p => p.Quotes.Any(q => q.Active == true && (q.RecalculationRequired == true || q.Project.Expiration < DateTime.Today)));
                        }
                        else
                        {
                            query = query.Where(p => p.Quotes.Any(q => q.Active == true && (q.RecalculationRequired == false && q.Project.Expiration >= DateTime.Today)));
                        }
                    }
                    break;

                case "bidDate":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate <= value);
                    }
                    break;

                case "estimatedClose":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose <= value);
                    }
                    break;

                case "estimatedDelivery":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery <= value);
                    }
                    break;

                case "projectDate":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate <= value);
                    }
                    break;
            }

            return query;
        }

        private IQueryable<Project> ProcessPairFilterOr(IQueryable<Project> query, List<FilterItem> filters)
        {

            var field1 = filters[0].Field;
            var field2 = filters[0].Field;
            var op1 = filters[0].Operator;
            var op2 = filters[1].Operator;


            if (field1 == "bidDate")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate >= value1 || p.BidDate == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate <= value1 || p.BidDate == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.BidDate <= value1 || p.BidDate >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.BidDate >= value1 || p.BidDate <= value2);
                }
                // ....
            }
            else if (field1 == "estimatedDelivery")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery >= value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery <= value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedDelivery <= value1 || p.EstimatedDelivery >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedDelivery >= value1 || p.EstimatedDelivery <= value2);
                }
                //....
            }

            else if (field1 == "estimatedClose")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose >= value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose <= value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedClose <= value1 || p.EstimatedClose >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedClose >= value1 || p.EstimatedClose <= value2);
                }
                //....
            }

            else if (field1 == "projectDate")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate >= value1 || p.ProjectDate == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate <= value1 || p.ProjectDate == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.ProjectDate <= value1 || p.ProjectDate >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.ProjectDate >= value1 || p.ProjectDate <= value2);
                }
            }
            //...
            return query;
        }

        private IQueryable<Project> Sort(IQueryable<Project> query, SearchProject search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "projectdate":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectDate) : query.OrderBy(s => s.ProjectDate);
                    break;
                case "estimatedclose":
                    query = (desc) ? query.OrderByDescending(s => s.EstimatedClose) : query.OrderBy(s => s.EstimatedClose);
                    break;
                case "projectid":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectId) : query.OrderBy(s => s.ProjectId);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                    break;
            }

            return query;
        }

        public Project ProjectCreate(UserSessionModel admin)
        {
            return ProjectCreate(admin.UserId);
        }

        public ProjectPipelineNote ProjectPipelineNoteCreate(UserSessionModel admin)
        {
            var entity = new ProjectPipelineNote();
            entity.ProjectPipelineNoteId = this.Context.GenerateNextLongId();

            this.Context.ProjectPipelineNotes.Add(entity);

            return entity;
        }

        public Project ProjectCreate(long ownerId)
        {
            var entity = new Project();

            entity.ProjectId = this.Context.GenerateNextLongId();

            entity.OwnerId = ownerId;

            //mass upload change - added this
            //if(entity.Owner == null)
            //{
            //    entity.Owner = Context.Users.Where(u => u.UserId == ownerId).FirstOrDefault();
            //}

            this.Context.Projects.Add(entity);

            return entity;
        }

        public Contact ContactDuplicate(long fromContactId)
        {
            var contact = this.Contacts.Where(a => a.ContactId == fromContactId).FirstOrDefault();

            Contact newContact = null;

            if (contact != null)
            {
                newContact = this.ContactCreate();
                Utilities.Copy<Contact>(newContact, contact, new string[] { "ContactId", "Timestamp" });
            }

            return newContact;
        }

        public Address AddressDuplicate(long fromAddressId)
        {
            var address = this.Context.Addresses.Where(a => a.AddressId == fromAddressId).FirstOrDefault();

            Address newAddress = null;

            if (address != null)
            {
                newAddress = this.AddressCreate();
                Utilities.Copy<Address>(address, newAddress, new string[] { "AddressId", "Timestamp" });
            }

            return newAddress;
        }

        public QuoteItem QuoteItemDuplicate(Quote newQuote, QuoteItem quoteItem)
        {
            var newItem = this.QuoteItemCreate(newQuote);
            Utilities.Copy<QuoteItem>(quoteItem, newItem, new string[] { "QuoteItemId", "QuoteId", "Timestamp" });
            return newItem;
        }

        public QuoteItemOption QuoteItemOptionDuplicate(QuoteItem quoteItem, QuoteItemOption quoteItemOption)
        {
            var newQuoteItemOption = this.QuoteItemOptionCreate(quoteItem);
            Utilities.Copy<QuoteItemOption>(quoteItemOption, newQuoteItemOption, new string[] { "QuoteItemOptionId", "QuoteItemId", "QuoteId", "Timestamp" });
            return newQuoteItemOption;
        }

        public Quote QuoteDuplicate(UserSessionModel user, long projectId, long quoteId)
        {
            var project = this.QueryProjectViewableByProjectId(user, projectId).Include(p => p.Owner).Include(p => p.Owner.Business).FirstOrDefault();

            var quote = this.QueryQuoteViewableByQuoteId(user, quoteId).FirstOrDefault();

            if (project != null && quote != null)
            {
                var newQuote = QuoteDuplicate(user, project, quote);

                return newQuote;
            }

            return null;
        }

        public Quote QuoteDuplicate(UserSessionModel user, Project toProject, long fromProjectId, int quoteRevision)
        {

            var quote = this.QueryQuotesViewableByUser(user)
                        .Where(q => q.ProjectId == fromProjectId && q.Revision == quoteRevision)
                        .FirstOrDefault();

            return QuoteDuplicate(user, toProject, quote);
        }

        public Quote QuoteDuplicate(UserSessionModel user, Project toProject, Quote quote)
        {
            Quote newQuote = null;

            if (quote != null)
            {
                newQuote = this.QuoteCreate(toProject);

                Utilities.Copy<Quote>(quote, newQuote, new string[] {
                    "QuoteId", "ProjectId", "Active", 

                    // Commission
                    "AwaitingCommissionRequest", "CommissionRequestId",
                    "CommissionCovertYes", "CommissionConvertNo", 

                    // Discount Request (Buy/Sell)
                    "AwaitingDiscountRequest", "DiscountRequestId",
                    "CommissionPercentage",  "ApprovedCommissionPercentage",
                    "DiscountPercentage", "ApprovedDiscountPercentage",
                    "DiscountPercentageSplit", "ApprovedDiscountPercentageSplit",
                    "DiscountPercentageVRV", "ApprovedDiscountPercentageVRV",
                    "Deleted", "CreatedDate", "Timestamp"
                });

                var quoteItems = this.QuoteItemsByQuoteId(user, quote.QuoteId).ToList();

                foreach (var item in quoteItems)
                {
                    var newQuoteItem = QuoteItemDuplicate(newQuote, item);

                    if (newQuoteItem.LineItemTypeId == (byte?)LineItemTypeEnum.Configured) {
                        var quoteItemOptions = this.QuoteItemOptionsByQuoteItemId(user, item.QuoteItemId).ToList();
                        foreach (var quoteItemOption in quoteItemOptions) {
                            QuoteItemOptionDuplicate(newQuoteItem, quoteItemOption);
                        }
                    }
                }
            }

            return newQuote;
        }

        public Project ProjectDuplicate(UserSessionModel user, long projectId)
        {
            var project = this.QueryProjectViewableByProjectId(user, projectId).FirstOrDefault();

            Project newProject = null;

            if (project != null)
            {
                newProject = this.ProjectCreate(project.OwnerId);

                Utilities.Copy<Project>(project, newProject, new string[] { "ProjectId", "Quotes", "SellerAddressId", "EngineerAddressId", "CustomerAddressId", "ShipToAddressId", "Timestamp" });

                var count = this.Projects.Where(p => p.Name.Contains(project.Name)).Count();

                newProject.Name = string.Format("{0} V{1}", project.Name, count + 1);

                var newQuote = QuoteDuplicate(user, newProject, project.ProjectId, project.ActiveVersion);

                newProject.ActiveVersion = newQuote.Revision;

                newQuote.Active = true;

                newProject.EngineerName = project.EngineerName;
                newProject.SellerName = project.SellerName;
                newProject.ShipToName = project.ShipToName;
                newProject.CustomerName = project.CustomerName;

                if (project.SellerAddressId.HasValue) newProject.SellerAddress = AddressDuplicate(project.SellerAddressId.Value);

                if (project.EngineerAddressId.HasValue) newProject.EngineerAddress = AddressDuplicate(project.EngineerAddressId.Value);

                if (project.CustomerAddressId.HasValue) newProject.CustomerAddress = AddressDuplicate(project.CustomerAddressId.Value);

                if (project.ShipToAddressId.HasValue) newProject.ShipToAddress = AddressDuplicate(project.ShipToAddressId.Value);

                newProject.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;

            }

            return newProject;
        }

        public Project ProjectTransfer(UserSessionModel user, long projectId, string email)
        {
            var project = this.QueryProjectViewableByProjectId(user, projectId).Include(t => t.ProjectTransfers).FirstOrDefault();

            if (project != null)
            {
                var toUser = UserQueryByEmail(email).FirstOrDefault();

                if (toUser != null)
                {
                    var transferTo = project.ProjectTransfers.Where(t => t.UserId == toUser.UserId).FirstOrDefault();

                    if (transferTo != null) // we dont need the project transfer record (if there is one) for the new project owner
                    {
                        project.ProjectTransfers.Remove(transferTo);
                    }

                    var fromUser = UserQueryByUserId(project.OwnerId).FirstOrDefault();

                    var transferFrom = project.ProjectTransfers.Where(t => t.UserId == toUser.UserId).FirstOrDefault();

                    // we need a project transfer record for the current project owner
                    project.ProjectTransfers.Add(new ProjectTransfer { UserId = project.OwnerId, ProjectId = project.ProjectId });

                    project.OwnerId = toUser.UserId;

                }
            }

            return project;

        }
    }
}