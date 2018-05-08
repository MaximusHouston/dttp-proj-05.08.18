//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Data;
using DPO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DPO.Domain
{

    public partial class UserGroupsServices : BaseServices 
    {
        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            var group = entity as Group;

            if (group == null)
            {
                throw new ArgumentException("Group entity not loaded");
            }

            RulesCommon(admin, group);

            if (this.Response.IsOK)
            {
                // have to use parent group as the current has not been added to the database
                var parentGroups = Db.QueryGroupsViewableAboveByGroupId(group.ParentGroupId.Value,true).ToList();

                parentGroups.ForEach(g => g.ChildrenCountDeep++);

                // need to get parent because we are add a new record
                var parentGroup = parentGroups.Where(g => g.GroupId == group.ParentGroupId.Value).FirstOrDefault();

                parentGroup.ChildrenCount++;

                group.Path = parentGroup.Path + "\\" + group.GroupId.ToString();
            }

        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var group = entity as Group;

            if (group == null)
            {
                throw new ArgumentException("Group entity not loaded");
            }

            // Make sure operation allowed
            if (!IsUserParentOwner(admin, group.GroupId))
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG005);
                return;
            }

            RulesCommon(admin, group);

        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
            var group = entity as Group;

            if (group == null)
            {
                throw new ArgumentException("Group entity not loaded");
            }
            // Make sure operation allowed
            if (!IsUserParentOwner(admin, group.GroupId))
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG005);
            }

            // Make group has no members
            if (group.MemberCount != 0)
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG010);
            }

            // Make group has no members
            if (group.ChildrenCount != 0)
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG011);
            }

        }

        // #################################################
        // Rules to calculate the actual project total
        // #################################################
        private void RulesCommon(UserSessionModel admin, Group group)
        {
            var entry = Db.Entry(group);

            // Make sure operation allowed
            if (string.IsNullOrWhiteSpace(group.Name))
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG001);
            }

            if (group.GroupId == 0)
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG017);
            }

            // You can be a parent of yourself. Also valid data needed
            if (group.GroupId == group.ParentGroupId)
            {
                this.Response.AddError(Resources.ResourceModelUserGroups.UG013);
                return;

            }
            // Make sure operation allowed
            if (entry.HasChanged("ParentGroupId"))
            {
                if (!group.ParentGroupId.HasValue || group.ParentGroupId == 0)
                {
                    this.Response.AddError(Resources.ResourceModelUserGroups.UG012);
                    return;
                }

                // Check user has permissions on parent group
                if (!IsUserParentOwner(admin, group.ParentGroupId.Value))
                {
                    this.Response.AddError(Resources.ResourceModelUserGroups.UG007);
                    return;
                }

                // Check that the parent group is not a child of the current group.
                if (this.Db.QueryGroupsViewableBelowByGroupId(group.GroupId).Any(g => g.GroupId == group.ParentGroupId))
                {
                    this.Response.AddError(Resources.ResourceModelUserGroups.UG006);
                    return;
                }
            }

        }


    }

}