

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
        public IQueryable<Business> Businesses
        {
            get { return this.GetDbSet<Business>(); }
        }

        public IQueryable<BusinessType> BusinessTypes
        {
            get { return this.GetDbSet<BusinessType>(); }
        }

        //#####################################################################
        // Get all business in businesses linked to groups attached to the user
        //#####################################################################
        public IQueryable<Business> QueryBusinessViewableByUser(UserSessionModel user, bool showUnallocated)
        {
            IQueryable<Business> query;

            if (user == null || user.UserId == 0 || user.UserTypeId >= UserTypeEnum.DaikinSuperUser)
            {
                query = this.Businesses;
            }
            else
            {
                //#####################################################################
                // Show all businesses user can see including his/her own
                //#####################################################################

                //var results = this.QueryUsersViewableByUser(user, showUnallocated).ToList();

                //var businesses = (from b in this.Businesses
                //                  select b).ToList();

                //var test = (from b in businesses
                //            join u in results
                //            on b.BusinessId equals u.BusinessId into ub
                //            from us in ub.DefaultIfEmpty()
                //            select b).ToList();

                //var test2 = (from u in test
                //             join b in businesses
                //             on u.BusinessId equals b.BusinessId
                //             select b).Distinct().ToList();

                //query = (from b in businesses
                //         join u in results
                //         on b.BusinessId equals u.BusinessId into ub
                //         from u in ub.DefaultIfEmpty()
                //         where  u.BusinessId == b.BusinessId || b.BusinessId == user.BusinessId
                //         select b
                //         ).Distinct().AsQueryable();


                query = (from b in this.Businesses
                         join u in QueryUsersViewableByUser(user, showUnallocated)
                         on b.BusinessId equals u.BusinessId into Lb
                         from u in Lb.DefaultIfEmpty()
                         where b.BusinessId == u.BusinessId || b.BusinessId == user.BusinessId
                         select b).Distinct();

            }

            return query;

        }

        public bool IsBusiness(string businessName)
        {
            return this.Businesses.Where(b => b.BusinessName == businessName).Any();
        }

        /// <summary>
        /// This will retrieve based on either CRM ID (Starts with A) 
        /// or Daikin City ID (just a number)
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Business GetBusinessByAccountId(string accountId)
        {
            IQueryable<Business> query = null;

            if (!string.IsNullOrEmpty(accountId))
            {
                if (accountId.ToLower().StartsWith("dc"))
                {
                    query = this.Businesses
                        .Where(b => b.DaikinCityId == accountId);
                }
                else
                {
                    query = this.Businesses
                        .Where(b => b.AccountId == accountId);
                }
            }
            else
            {
                return null;
            }

            return query
                  .Include(b => b.Contact)
                  .Include(b => b.Address)
                  .FirstOrDefault();
        }

        public Business GetBusinessByBusinessId(long businessId)
        {
            return this.Businesses.Where(b => b.BusinessId == businessId)
                  .Include(b => b.Contact)
                  .Include(b => b.Address)
                  .FirstOrDefault();
        }

        public IQueryable<Business> BusinessQueryByBusinessId(UserSessionModel admin, long? businessId)
        {
            return this.QueryBusinessViewableByUser(admin, true).Where(u => u.BusinessId == businessId);
        }

        public IQueryable<Business> BusinessQueryBySearch(UserSessionModel admin, SearchBusiness search)
        {
            var query = QueryBusinessViewableByUser(admin, true);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(admin, query, search); // Must be Last

            return query;
        }

        public IQueryable<Business> BusinessQueryBySearch(SearchBusiness search)
        {
            var query = QueryBusinessViewableByUser(null, true);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(new UserSessionModel(), query, search); // Must be Last

            return query;
        }


        public bool IsAccountIdInUse(string accountId)
        {
            return this.Businesses.Any(o => o.AccountId == accountId);
        }

        public bool IsBusinessNameInUse(UserSessionModel admin, string businessName)
        {
            return this.QueryBusinessViewableByUser(admin, true).Any(u => u.BusinessName == businessName);
        }

        private IQueryable<Business> Filter(IQueryable<Business> query, SearchBusiness search)
        {
            if (search == null) return query;

            if (search.BusinessId.HasValue)
            {
                query = query.Where(s => s.BusinessId == search.BusinessId);
            }

            if (!string.IsNullOrEmpty(search.BusinessName))
            {
                query = query.Where(s => s.BusinessName.Contains(search.BusinessName));
            }

            if (!string.IsNullOrEmpty(search.ExactBusinessName))
            {
                query = query.Where(s => s.BusinessName == search.ExactBusinessName);
            }

            if (!string.IsNullOrEmpty(search.AccountId))
            {
                query = query.Where(s => s.AccountId == search.AccountId);
            }

            if (!string.IsNullOrEmpty(search.Address))
            {
                query = query.Where(s => s.Address.AddressLine1.Contains(search.Address));
            }

            if (search.StateId != null)
            {
                query = query.Where(s => s.Address.StateId == search.StateId);
            }

            if (!string.IsNullOrEmpty(search.StateCode))
            {
                query = query.Where(s => s.Address.State.Code == search.StateCode);
            }

            if (!string.IsNullOrEmpty(search.PostalCode))
            {
                query = query.Where(s => s.Address.PostalCode == search.PostalCode);
            }

            if (!string.IsNullOrEmpty(search.CountryCode))
            {
                query = query.Where(s => s.Address.State.CountryCode == search.CountryCode);
            }

            if (!string.IsNullOrEmpty(search.Filter))
            {
                query = query.Where(s => s.BusinessName.Contains(search.Filter));
            }

            if (search.Enabled != null)
            {
                query = query.Where(s => s.Enabled == search.Enabled.Value);
            }

            if (search.IsDaikinComfortPro.GetValueOrDefault())
                query = query.Where(s => s.IsDaikinComfortPro == search.IsDaikinComfortPro);

            if (search.IsVRVPro.GetValueOrDefault())
                query = query.Where(s => s.IsVRVPro == search.IsVRVPro);
           
            if (search.IsDaikinBranch.GetValueOrDefault())
                query = query.Where(s => s.BusinessTypeId == (BusinessTypeEnum)200002);

            return query;
        }

        private IQueryable<Business> Sort(IQueryable<Business> query, SearchBusiness search)
        {
            if (search == null) return query;

            bool desc = search.IsDesc;

            switch ((search.SortColumn ?? "").ToLower())
            {
                case "accountid":
                    query = (desc) ? query.OrderByDescending(s => s.AccountId) : query.OrderBy(s => s.AccountId);
                    break;
                case "businesstype":
                    query = (desc) ? query.OrderByDescending(s => s.BusinessType.Description) : query.OrderBy(s => s.BusinessType.Description);
                    break;
                case "location":
                case "city":
                    query = (desc) ? query.OrderByDescending(s => s.Address.Location) : query.OrderBy(s => s.Address.Location);
                    break;
                case "state":
                    query = (desc) ? query.OrderByDescending(s => s.Address.State.Name) : query.OrderBy(s => s.Address.State.Name);
                    break;
                case "country":
                    query = (desc) ? query.OrderByDescending(s => s.Address.State.Country.Name) : query.OrderBy(s => s.Address.State.Country.Name);
                    break;
                case "webaddress":
                    query = (desc) ? query.OrderByDescending(s => s.Contact.Website) : query.OrderBy(s => s.Contact.Website);
                    break;
                case "enabled":
                    query = (desc) ? query.OrderByDescending(s => s.Enabled) : query.OrderBy(s => s.Enabled);
                    break;
                case "commissionable":
                    query = (desc) ? query.OrderByDescending(s => s.CommissionSchemeAllowed) : query.OrderBy(s => s.CommissionSchemeAllowed);
                    break;
                case "daikinbranch":
                    query = (desc) ? query.OrderByDescending(s => s.BusinessTypeId == (BusinessTypeEnum)200002) : query.OrderBy(s => s.BusinessTypeId);
                    break;
                case "vrvpro":
                    query = (desc) ? query.OrderByDescending(s => s.IsVRVPro) : query.OrderBy(s => s.IsVRVPro);
                    break;
                case "daikincomfortpro":
                    query = (desc) ? query.OrderByDescending(s => s.IsDaikinComfortPro) : query.OrderBy(s => s.IsDaikinComfortPro);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.BusinessName) : query.OrderBy(s => s.BusinessName);
                    break;
            }

            return query;
        }

        public Business GetBusinessByProjectOwner(long? projectId)
        {
            var business = this.Context.Projects.Where(p => p.ProjectId == projectId).Select(p => p.Owner.Business).FirstOrDefault();

            return business;
        }

        public Business BusinessCreate(BusinessTypeEnum type)
        {
            var entity = new Business();

            entity.BusinessId = this.Context.GenerateNextLongId();

            entity.BusinessTypeId = type;

            entity.Address = AddressCreate();

            entity.Contact = ContactCreate();

            this.Context.Businesses.Add(entity);

            return entity;
        }

    }
}