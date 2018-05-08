//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DPO.Resources;
using System.Net.Mail;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Text;

namespace DPO.Domain
{

    public partial class UserGroupsServices : BaseServices
    {
        public UserGroupsServices() : base() { }

        public UserGroupsServices(DPOContext context) : base(context) { }


        public ServiceResponse GroupsListModel(UserSessionModel user, string filter)
        {
            this.Response = new ServiceResponse();

            var groups = this.Db.QueryGroupsViewableBelowByUser(user, filter).ToList();

            // Return top level groups by selecting all groups without a parent.
            var toplevelgroups = groups.Where(g => !groups.Any(p => p.GroupId == g.ParentGroupId)).ToList();

            var items = (from g in groups
                         select new UserGroupItemModel
                         {
                             GroupId = g.GroupId,
                             GroupName = g.Name,
                             Level = g.RelativePath(toplevelgroups),
                             ChildCountDeep = g.ChildrenCountDeep,
                             ViewableChildCount = g.Users.Count(),
                             MemberCount = g.MemberCount
                         }).ToList();

            var model = new UserGroupsModel();

            model.UserGroups = items;

            model.UserGroupId = user.GroupId;

            // Make sure group id zero is never sent.
            if (model.UserGroupId.HasValue && model.UserGroupId.Value == 0)
            {
                model.UserGroupId = null;
            }

            model.UnAllocatedGroup = new UserGroupItemModel { ChildCountDeep = 0, GroupId = 0, GroupName = "Unallocated", Level = 1 };

            model.UnAllocatedGroup.MemberCount = this.Db.QueryGroupUsersUnallocatedViewableByUser(user).Count();

            this.Response.Model = model;

            return this.Response;

        }

       
        public ServiceResponse GroupMove(UserSessionModel admin, long groupId, long toParentGroupId)
        {
            this.Response = new ServiceResponse();

            this.Db.ReadOnly = false;

            // We check later if allowed to action group
            var group = this.Db.QueryGroupByGroupId(groupId).FirstOrDefault();

            group.ParentGroupId = toParentGroupId;

            ApplyBusinessRules(admin, group);

            if (this.Response.IsOK)
            {
                // Need all Groups to recalculate totals

                Db.Groups.ToList();

                Db.UpdateGroupInformation();

                base.SaveToDatabase("Users moved to group");

                Db.SystemRoutinesUpdateMembersCountForGroups();
            }

            return this.Response;

        }


        public ServiceResponse GroupUsersListModel(UserSessionModel user, string filter, long? groupId)
        {
            // do not return yourself.
            var search = new SearchUser { Filter = filter, Page = 1, PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL, GroupId = groupId };
            var query = from u in this.Db.QueryGroupUsersViewableByByUser(user, search)
                        select u;

            var model = (from u in query
                         select new UserListModel
                         {
                             UserId = u.UserId,
                             FirstName = u.FirstName,
                             MiddleName = u.MiddleName,
                             LastName = u.LastName,
                             BusinessName = u.Business.BusinessName,
                             BusinessTypeDescription = u.Business.BusinessType.Description,
                             UserTypeDescription = u.UserType.Description,
                             Email = u.Email,
                             IsGroupOwner = u.IsGroupOwner ?? false,
                             GroupName = u.Group.Name,
                             GroupId = u.GroupId,
                             Enabled = u.Enabled
                         }).ToList();

            this.Response.Model = model;

            return this.Response;

        }

        public ServiceResponse GroupCreate(UserSessionModel admin, string groupName, long? parentGroupId)
        {
            this.Response = new ServiceResponse();

            this.Db.ReadOnly = false;

            if (parentGroupId == null)
            {
                parentGroupId = admin.GroupId;
            }

            if (parentGroupId == null)
            {
                return this.Response;
            }

            var group = this.Db.GroupCreate(groupName, parentGroupId);

            ApplyBusinessRules(admin, group);

            if (this.Response.IsOK)
            {
                base.SaveToDatabase("Group created");
            }

            return this.Response;

        }

        public ServiceResponse GroupDelete(UserSessionModel user, long groupId)
        {
            this.Response = new ServiceResponse();

            this.Db.ReadOnly = false;

            // We check later if allowed to action group
            var group = this.Db.QueryGroupByGroupId(groupId).FirstOrDefault();

            if (this.Response.IsOK)
            {

                Db.Context.Groups.Remove(group);

                ApplyBusinessRules(user, group);

                var parentGroups = Db.QueryGroupsViewableAboveByGroupId(groupId, false).ToList();

                parentGroups.ForEach(g => g.ChildrenCountDeep--);

                base.SaveToDatabase("Group removed");
            }

            return this.Response;

        }

        public ServiceResponse GroupRename(UserSessionModel user, long groupId, string newName)
        {
            this.Response = new ServiceResponse();

            this.Db.ReadOnly = false;

            // We check later if allowed to action group
            var group = this.Db.QueryGroupByGroupId(groupId).FirstOrDefault();

            group.Name = newName;

            ApplyBusinessRules(user, group);

            if (this.Response.IsOK)
            {
                base.SaveToDatabase("Group renamed");
            }

            return this.Response;

        }

        public ServiceResponse GroupUsersMove(UserSessionModel admin, long[] userIds, long toGroupId)
        {
            this.Response = new ServiceResponse();

            // Make sure operation is allowed
            // Validate that group moving into is within scope of the current user, unless moving to unallocated.
            if (toGroupId != 0 && !IsUserParentOwner(admin, toGroupId))
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG003);
                return this.Response;
            }

            this.Db.ReadOnly = false;

            // Validate users are within scope of the current user.
            // and exclude current user from moving

            userIds = userIds.Where(u => admin.UserId != u).ToArray();

            var dbUsers = (
                            from u in this.Db.QueryGroupUsersViewableByByUser(admin, new SearchUser { PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL })
                            where userIds.Contains(u.UserId)
                            select new { UserId = u.UserId, GroupId = u.GroupId, Timestamp = u.Timestamp }).ToList();



            if (dbUsers.Count != userIds.Length)
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG003);
                return this.Response;
            }

            Db.ReadOnly = false;

            dbUsers.ForEach(u => Db.Context.Users.Attach(new User { UserId = u.UserId, GroupId = u.GroupId, Timestamp = u.Timestamp }));

            foreach (var user in this.Context.Users.Local.ToList())
            {
                user.GroupId = toGroupId;
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase("Users moved to group");

                Db.SystemRoutinesUpdateMembersCountForGroups(); // after save
            }

            return this.Response;


        }

        public ServiceResponse GroupUserMakeOwner(UserSessionModel user, long groupId, long userId, bool makeOwner)
        {
            this.Response = new ServiceResponse();

            // User cannot make changes to himself only user in above group can
            if (userId == user.UserId)
            {
                this.Response.AddError(ResourceModelUserGroups.UG014);
                return this.Response;
            }

            // User cannot make changes to himself only user in above group can
            if (groupId == 0)
            {
                this.Response.AddError(ResourceModelUserGroups.UG016);
                return this.Response;
            }

            Db.ReadOnly = false;

            // Make sure user has access to group
            var entity = (from u in this.Db.QueryUsersViewableByUser(user, false) where u.UserId == userId select u).FirstOrDefault();

            if (entity != null)
            {
                entity.IsGroupOwner = makeOwner;
                base.SaveToDatabase("User made owner");
            }
            else
            {
                this.Response.AddError(ResourceModelUserGroups.UG015);
            }

            return this.Response;


        }


        private bool IsUserParentOwner(UserSessionModel user, long groupId)
        {
            var inclusive = user.HasAccess(SystemAccessEnum.AdminAccessRights);

            var result = Db.QueryGroupParentOwnersByGroupId(groupId, inclusive).Any(g => g.UserId == user.UserId);

            // We are also owner if we have owner access 
            if (result == false)
            {

            }

            return result;
        }

    }

}