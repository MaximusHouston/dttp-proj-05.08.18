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

        public IQueryable<User> Users
        {
            get { return this.GetDbSet<User>(); }
        }

        public IQueryable<UserType> UserTypes
        {
            get { return this.GetDbSet<UserType>(); }
        }
        //internal IQueryable<User> UserQueryAccessibleBusinessUsersForUserId(long? userId)
        //{
        //   return from user in this.Users

        //          join users in this.Users on user.BusinessId equals users.BusinessId

        //          where user.UserId == userId && users.UserType <= user.UserType

        //          select users;

        //}

        //internal IQueryable<User> UserQueryByAccessibleGroupUsersForUserId(long? userId)
        //{
        //   var result = from admin in this.Users

        //                // Join to get all the groups for the user
        //                join adminGroups in this.GroupOwnerLinks on admin.UserId equals adminGroups.UserId

        //                // Join to get all the business linked to the groups
        //                join groupBusinesses in this.GroupBusinessLinks on adminGroups.GroupId equals groupBusinesses.GroupId

        //                // Join all the users found in those businesses
        //                join users in this.Users on groupBusinesses.BusinessId equals users.BusinessId

        //                // make sure access level allows users to be returned
        //                where admin.UserId == userId && users.UserType <= admin.UserType

        //                select users;

        //   return result;

        //}

        internal bool IsSuperUser(long? adminId)
        {
            return this.Users.Any(a => a.UserId == adminId && a.UserTypeId >= UserTypeEnum.DaikinSuperUser);
        }

        public bool IsUser(string email)
        {
            return this.Users.Where(u => u.Email == email).Any();
        }

        public void SaveDisplaySettings(UserSessionModel model)
        {
            var settings = string.Format("pagesize={0}", model.DisplaySettingsPageSize);

            this.Users.Where(u => u.UserId == u.UserId).Update(u => new User { DisplaySettings = settings });
        }

        //################################################################
        // View all unallocated users under the users grouping tree
        //################################################################
        public IQueryable<User> QueryUnallocatedUsersViewableByUser(UserSessionModel user)
        {
            IQueryable<User> query;

            if (user.UserTypeId >= UserTypeEnum.DaikinSuperUser)
            {
                query = from unallocated in this.Users
                        where unallocated.GroupId == 0
                        select unallocated;
            }
            else
            {
                query = //get all business in the group tree for user
                             from g in this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value, true)
                             join u in this.Users on g.GroupId equals u.GroupId
                             join b in this.Businesses on u.BusinessId equals b.BusinessId
                             join unallocated in this.Users on b.BusinessId equals unallocated.BusinessId
                             where unallocated.GroupId == 0
                             select unallocated;

                if (user.HasAccess(SystemAccessEnum.AdminAccessRights))
                {
                    query = query.Where(u => u.UserTypeId <= user.UserTypeId);
                }
                else
                {
                    query = query.Where(u => u.UserTypeId < user.UserTypeId);
                }
            }


            return query.Distinct();
        }

        //################################################################
        // Get all users linked to groups under the user
        //################################################################
        public IQueryable<User> QueryUsersViewableByUser(UserSessionModel user, bool includeUnallocated)
        {
            IQueryable<User> query = null;

            // log
            if (user == null || user.UserId == 0 || user.UserTypeId == UserTypeEnum.Systems)
            {
                query = this.Users;
            }
            else
            {
                query = from u in this.Users
                        join g in this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value) on u.GroupId equals g.GroupId
                        select u;

                if (user.HasAccess(SystemAccessEnum.AdminAccessRights))
                {
                    query = query.Where(u => u.UserTypeId <= user.UserTypeId);
                }
                else
                {
                    query = query.Where(u => u.UserTypeId < user.UserTypeId);
                }

                query = query.Where(u => u.GroupId != user.GroupId);

                if (includeUnallocated)
                {
                    query = (from u in query select u)
                            .Union
                            (from u in QueryUnallocatedUsersViewableByUser(user) select u);
                }
            }

            return query;

        }

        //################################################################
        // Get all users in business linked to groups attached to the user
        ////################################################################
        //private IQueryable<User> QueryUsersViewableByUser_old(UserSessionModel user)
        //{
        //   IQueryable<User> query = null;

        //   if (user==null || user.UserTypeId >= UserTypeEnum.DaikinSuperUser)
        //   {
        //       query = this.Users;
        //   }
        //   else
        //   {
        //       query = from users in this.Users
        //               join link in this.QueryGroupsViewableBelowByGroupId(user.GroupId.Value) on users.GroupId equals link.GroupId
        //               where users.UserTypeId <= user.UserTypeId
        //               select users;  
        //   }

        //   return query;

        //}

        public IQueryable<User> QueryUserViewableByUserId(UserSessionModel admin, long? userId)
        {
            // Request their own record
            if (admin.UserId == userId)
            {
                return this.Users.Where(u => u.UserId == userId);
            }
            else
            {
                return this.QueryUsersViewableByUser(admin, true).Where(u => u.UserId == userId);
            }

        }

        public IQueryable<User> GetSuperUser()
        {
            return this.Users.Where(u => u.UserTypeId == UserTypeEnum.DaikinSuperUser);
        }

        public IQueryable<User> UserQueryByUserId(long userId)
        {
            return this.Users.Where(u => u.UserId == userId);
        }

        public IQueryable<User> UserQueryByEmail(string email)
        {
            return this.Users.Where(u => u.Email == email);
        }

        // #################################################
        // Get all parent owners
        // #################################################
        public IQueryable<User> QueryGroupParentOwnersByGroupId(long? groupId, bool inclusive)
        {
            var query = (from own in this.Users
                         join grp in this.QueryGroupsViewableAboveByGroupId(groupId.Value, inclusive) on own.GroupId equals grp.GroupId
                         where own.IsGroupOwner == true && own.Enabled == true
                         select own);
            return query;
        }

        // #################################################
        // Get all enable owners of a group.
        // #################################################
        public IQueryable<User> QueryGroupOwnersByGroupId(long? groupId)
        {
            var query = this.Users.Where(u => u.GroupId == groupId && u.IsGroupOwner == true && u.Enabled == true);
            return query;
        }

        // #################################################
        // Get first enabled parent
        // #################################################
        public IQueryable<User> QueryDaikinGroupOwnersByAccountId(string accountId)
        {
            var group = QueryGroupsByAccountId(accountId).FirstOrDefault();

            return QueryDaikinGroupOwnersByGroup(group);
        }

        public IQueryable<User> QueryDaikinGroupOwnersByBusinessName(string businessName)
        {
            var group = QueryGroupsByBusinessName(businessName).FirstOrDefault();

            return QueryDaikinGroupOwnersByGroup(group);
        }

        private IQueryable<User> QueryDaikinGroupOwnersByGroup(Group group)
        {
            var groupId = (group == null) ? 0 : group.GroupId;

            // Get first owner of a parent group with the lower or equal user type value from the passed user type.

            var user = QueryGroupParentOwnersByGroupId(groupId, false)
                       .Where(u =>
                              u.UserTypeId == UserTypeEnum.DaikinEmployee ||
                              u.UserTypeId == UserTypeEnum.DaikinAdmin ||
                              u.UserTypeId == UserTypeEnum.DaikinSuperUser
                              )
                       .OrderBy(u => u.UserTypeId)
                       .Take(1)
                       .FirstOrDefault();

            // return all other group owners in that group
            IQueryable<User> query = null;

            if (user == null)
            {
                query = QueryGroupOwnersByGroupId(0);
            }
            else
            {
                query = QueryGroupOwnersByGroupId(user.GroupId).Where(u => u.UserTypeId >= user.UserTypeId);
            }

            return query;
        }

        // #################################################
        // Search for super user for business
        // #################################################

        public IQueryable<User> QueryUsersApprovedBusinessSuperUser(string accountId)
        {
            IQueryable<User> query = null;

            if (!string.IsNullOrEmpty(accountId))
            {
                if (accountId.ToLower().StartsWith("dc"))
                {
                    query = from u in this.Users
                            where u.Business.DaikinCityId == accountId
                            select u;
                } else
                {
                    query = from u in this.Users
                            where u.Business.AccountId == accountId
                            select u;
                }

                query = query
                    .Where(u =>
                        u.Approved == true
                            && u.UserTypeId == UserTypeEnum.CustomerSuperUser
                            && u.Enabled == true);
            }

            return query;
        }

        // #################################################
        // Search for users under user tree
        // #################################################

        public IQueryable<User> QueryUsersViewableBySearch(UserSessionModel admin, SearchUser search, bool includeUnallocated, bool includeCurrentUser = false)
        {

            var query = QueryUsersViewableByUser(admin, includeUnallocated);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            if (includeCurrentUser == true)
            {
                query = UserQueryByUserId(admin.UserId).Concat(query);
            }

            query = Paging(admin, query, search); // Must be Last

            return query;
        }

        public IQueryable<User> QueryUsersViewableByProjectSearch(UserSessionModel admin, SearchUser search, bool includeUnallocated, bool includeCurrentUser = false)
        {
            IQueryable<User> query = null;

            // log
            if (admin == null || admin.UserId == 0 || admin.UserTypeId == UserTypeEnum.Systems)
            {
                query = this.Users;
            }
            else
            {
                query = from u in this.Users
                        join g in this.QueryGroupsViewableBelowByGroupId(admin.GroupId.Value, true) 
                        on u.GroupId equals g.GroupId
                        select u;

                if (!admin.HasAccess(SystemAccessEnum.ViewProjectsInGroup))
                {
                    query = query.Where(u => u.UserId == admin.UserId);
                }

                if (includeUnallocated)
                {
                    query = (from u in query select u)
                            .Union
                            (from u in QueryUnallocatedUsersViewableByUser(admin) select u);
                }
            }

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            if (!includeCurrentUser)
            {
                query = query.Where(u => u.UserId != admin.UserId);
            }

            query = Paging(admin, query, search); // Must be Last

            return query;
        }


        private IQueryable<User> Filter(IQueryable<User> query, SearchUser search)
        {
            if (search == null) return query;

            if (search.UserId.HasValue)
            {
                query = query.Where(s => s.UserId == search.UserId);
            }

            if (!string.IsNullOrEmpty(search.Email))
            {
                query = query.Where(s => s.Email == search.Email);
            }

            if (search.Enabled.HasValue)
            {
                query = query.Where(s => s.Enabled == search.Enabled);
            }

            if (search.GroupId.HasValue)
            {
                query = query.Where(s => s.GroupId == search.GroupId);
            }

            if (search.Approved.HasValue)
            {
                query = query.Where(s => s.Approved == search.Approved);
            }

            if (search.Rejected.HasValue)
            {
                query = query.Where(s => s.Rejected == search.Rejected);
            }

            if (search.UserTypeId.HasValue)
            {
                query = query.Where(s => s.UserTypeId <= search.UserTypeId);
            }

            if (!string.IsNullOrEmpty(search.Filter))
            {
                query = query.Where(s => s.FirstName.Contains(search.Filter) ||
                                         s.MiddleName.Contains(search.Filter) ||
                                         s.LastName.Contains(search.Filter) ||
                                         s.Business.BusinessName.Contains(search.Filter) ||
                                         s.Email.Contains(search.Filter));
            }

            return query;
        }

        private IQueryable<User> Sort(IQueryable<User> query, SearchUser search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "displayname":
                    query = (desc) ?
                    query.OrderByDescending(s => s.FirstName).ThenByDescending(s => s.MiddleName).ThenByDescending(s => s.LastName) :
                    query.OrderBy(s => s.FirstName).ThenBy(s => s.MiddleName).ThenBy(s => s.LastName);
                    break;
                case "businessname":
                    query = (desc) ? query.OrderByDescending(s => s.Business.BusinessName) : query.OrderBy(s => s.Business.BusinessName);
                    break;
                case "businesstype":
                    query = (desc) ? query.OrderByDescending(s => s.Business.BusinessType.Description) : query.OrderBy(s => s.Business.BusinessType.Description);
                    break;
                case "email":
                    query = (desc) ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email);
                    break;
                case "registeredon":
                    query = (desc) ? query.OrderByDescending(s => s.RegisteredOn) : query.OrderBy(s => s.RegisteredOn);
                    break;
                case "usertype":
                    query = (desc) ? query.OrderByDescending(s => s.UserType.Description) : query.OrderBy(s => s.UserType.Description);
                    break;
                case "active":
                    query = (desc) ? query.OrderByDescending(s => s.Enabled) : query.OrderBy(s => s.Enabled);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.Business.BusinessName) : query.OrderBy(s => s.Business.BusinessName);
                    break;
            }

            return query;
        }


        //################################################################
        // Create
        //################################################################
        public User UserCreate(Business business, Group group, UserTypeEnum userTypeId)
        {
            var entity = this.Context.Users.Create();

            entity.UserId = this.Context.GenerateNextLongId();

            entity.Group = group;

            entity.Business = business;

            entity.UserTypeId = userTypeId;

            entity.Address = AddressCreate();

            entity.Contact = ContactCreate();

            this.Context.Users.Add(entity);

            return entity;
        }

        public User UserCreate(long businessId, long groupId, UserTypeEnum userType)
        {
            var entity = this.Context.Users.Create();

            entity.UserId = this.Context.GenerateNextLongId();

            entity.GroupId = groupId;

            entity.BusinessId = businessId;

            entity.UserTypeId = userType;

            entity.Address = AddressCreate();

            entity.Contact = ContactCreate();

            this.Context.Users.Add(entity);

            return entity;
        }



    }
}