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

namespace DPO.Data
{

    public partial class Repository
    {
        public IQueryable<Group> Groups
        {
            get { return this.GetDbSet<Group>(); }
        }

        //################################################################
        //Create group
        //################################################################
        public Group GroupCreate(string name, long? parentGroupId)
        {
            var entity = new Group();

            entity.Name = name;

            entity.ParentGroupId = parentGroupId;

            entity.GroupId = this.Context.GenerateNextLongId();

            this.Context.Groups.Add(entity);

            return entity;
        }


        //################################################################
        //Get all groups above the supplied group id
        //################################################################
        public IQueryable<Group> QueryGroupsViewableAboveByGroupId(long groupId, bool inclusive)
        {
            var query = this.Context.FnGetGroupsAboveGroupId(groupId);

            if (!inclusive)
            {
                query = query = query.Where(g => g.GroupId != groupId);
            }

            return query;
        }

        //################################################################
        //Get all groups below the supplied group id
        //################################################################
        public IQueryable<Group> QueryGroupsViewableBelowByGroupId(long groupId)
        {
            return QueryGroupsViewableBelowByGroupId(groupId, false);
        }

        public IQueryable<Group> QueryGroupsViewableBelowByGroupId(long groupId, bool inclusive)
        {
            var query = this.Context.FnGetGroupsBelowGroupId(groupId);

            if (!inclusive)
            {
                query = query = query.Where(g => g.GroupId != groupId);
            }

            return query;
        }


        //################################################################
        //Get group for buiness account Id
        //################################################################
        public IQueryable<Group> QueryGroupByGroupId(long groupId)
        {
            var query = this.Groups.Where(b => b.GroupId == groupId);
            return query;
        }

        //################################################################
        //Get group for buiness account Id
        //################################################################
        public IQueryable<Group> QueryGroupsByAccountId(string accountId)
        {
            IQueryable<Group> query = null;

            if (!string.IsNullOrEmpty(accountId) && accountId.ToLower().StartsWith("dc"))
            {
                query = this.Users.Where(b => b.Business.DaikinCityId == accountId).Select(u => u.Group);
            }
            else
            {
                query = this.Users.Where(b => b.Business.AccountId == accountId).Select(u => u.Group);
            }

            return query;
        }


        //################################################################
        //Get group for buiness account Id
        //should not / is not in use.
        //################################################################
        public IQueryable<Group> QueryGroupsByBusinessName(string businessName)
        {
            var query = this.Users.Where(b => b.Business.BusinessName == businessName).Select(u => u.Group);
            return query;
        }

        //################################################################
        //Get all groups below the supplied user and filter users
        //################################################################
        public IQueryable<Group> QueryGroupsViewableBelowByUser(UserSessionModel user, string userFilter)
        {
            var query = this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value, false);

            if (!string.IsNullOrWhiteSpace(userFilter))
            {
                var search = new SearchUser { Filter = userFilter, ReturnTotals = false, PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL };
                query = from q in query
                        join u in this.QueryGroupUsersViewableByByUser(user, search) on q.GroupId equals u.GroupId
                        select q;
            }

            query = query.OrderBy(q => q.Path);

            return query;
        }

        public IQueryable<User> QueryGroupUsersViewableByByUser(UserSessionModel user, SearchUser search)
        {
            var query = this.QueryUsersViewableBySearch(user, search, true).Where(u => u.Approved == true);

            return query;
        }

        public IQueryable<User> QueryGroupUsersUnallocatedViewableByUser(UserSessionModel user)
        {
            var query = this.QueryUnallocatedUsersViewableByUser(user).Where(u => u.Approved == true && u.Enabled == true);

            return query;
        }

        //######################################################################
        // Group Management Procedures
        //######################################################################
        #region Group Management Procedures

        public void UpdateGroupInformation()
        {
            _log.Info("Enter UpdateGroupInformation()");

            var dpoGroups = this.Context.Groups.Local.ToDictionary(g => g.GroupId);

            _log.Info("Start UpdateGroupRelationship");
            UpdateGroupSetRelationships(dpoGroups);

            _log.Info("Start UpdateGroupChildCount");
            UpdateGroupSetChildCounts(dpoGroups);

            _log.Info("Start UpdateGroupChildDeepCount");
            UpdateGroupSetChildrenDeepCount(dpoGroups);

            _log.Info("Start UpdateGroupPath");
            UpdateGroupSetPath(dpoGroups);
        }

        private void UpdateGroupSetRelationships(Dictionary<long, DPO.Data.Group> lookup)
        {
            _log.Info("Enter UpdateGroupSetRelationships()");

            foreach (var groupItem in lookup)
            {
                var group = groupItem.Value;

                if (group.ParentGroupId.HasValue)
                {
                    DPO.Data.Group parent;

                    if (lookup.TryGetValue(group.ParentGroupId.Value, out parent) == false)
                    {
                        throw new Exception(string.Format(Resources.DataMessages.DM027, group.GroupId));
                    }

                    //if (@group.ParentGroup != null && @group.ParentGroup != parent)
                    //{
                    //    group.ParentGroup = parent;
                    //}

                    if (@group.ParentGroup != parent)
                    {
                        group.ParentGroup = parent;
                    }

                    if (parent != null)
                    {
                        parent.ChildGroups.Add(group);

                        parent.ChildrenCount++;
                    }

                }
            }

            _log.Info("Finished execute UpdateGroupSetRelationships()");
        }

        private void UpdateGroupSetChildCounts(Dictionary<long, DPO.Data.Group> lookup)
        {
            _log.Info("Enter UpdateGroupSetChildCounts()");

            foreach (var groupItem in lookup)
            {
                var group = groupItem.Value;

                if (group.ChildrenCount != group.ChildGroups.Count)
                {
                    group.ChildrenCount = (short)group.ChildGroups.Count;
                }
            }
            _log.Info("Finished Execute UpdateGroupSetChildCounts()");
        }

        private void UpdateGroupSetChildrenDeepCount(Dictionary<long, DPO.Data.Group> lookup)
        {
            _log.Info("Enter UpdateGroupSetChildDeepCount()");

            foreach (var groupItem in lookup)
            {
                var group = groupItem.Value;

                var count = (short)(group.CalcChildrenDeepCount(0) - 1); // -1 remove the group itself in count

                if (group.ChildrenCountDeep != count)
                {
                    group.ChildrenCountDeep = count;
                }
            }

            _log.Info("Finished Execute UpdateGroupSetChildrenDeepCount()");
        }

        private void UpdateGroupSetPath(Dictionary<long, DPO.Data.Group> lookup)
        {
            _log.Info("Enter UpdateGroupSetPath()");
            foreach (var groupItem in lookup)
            {
                var group = groupItem.Value;

                var path = group.GetPath;

                if (group.Path != path)
                {
                    group.Path = path;
                }
            }
            _log.Info("Finished Execuate UpdateGroupSetPath()");
        }


        #endregion
    }

}