using DPO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
    /// <summary>
    /// Contains Commission Multiplier Queries
    /// </summary>
    public partial class Repository
    {
        public IQueryable<CommissionMultiplier> CommissionMultipliers
        {
            get { return this.GetDbSet<CommissionMultiplier>(); }
        }

        public IQueryable<CommissionMultiplier> QueryCommissionMultipliersViewableBySearch(UserSessionModel user, SearchCommissionMultiplier search)
        {
            var query = this.CommissionMultipliers;

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            query = Paging(user, query, search); // Must be Last

            return query;
        }

        

        private IQueryable<CommissionMultiplier> Sort(IQueryable<CommissionMultiplier> query, SearchCommissionMultiplier search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "multipliercategorytype":
                    query = (desc) ? query.OrderByDescending(s => s.MultiplierCategoryType.Name) : query.OrderBy(s => s.MultiplierCategoryType.Name);
                    break;
                case "multiplier":
                    query = (desc) ? query.OrderByDescending(s => s.Multiplier) : query.OrderBy(s => s.Multiplier);
                    break;
                case "commissionpercentage":
                    query = (desc) ? query.OrderByDescending(s => s.CommissionPercentage) : query.OrderBy(s => s.CommissionPercentage);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.CommissionMultiplierId) : query.OrderBy(s => s.CommissionMultiplierId);
                    break;
            }

            return query;
        }

        public IQueryable<CommissionMultiplier> Filter(IQueryable<CommissionMultiplier> query, SearchCommissionMultiplier search)
        {
            if (search == null) return query;

            if (search.MultiplierCategoryType.HasValue)
            {
                query = query.Where(w => w.MultiplierCategoryTypeId == search.MultiplierCategoryType.Value);
            }

            if (search.Multiplier.HasValue)
            {
                query = query.Where(w => w.Multiplier == search.Multiplier.Value);
            }

            return query;
        }

        public List<CommissionMultiplier> GetCommissionMultipliers()
        {
            var query = this.Context.CommissionMultipliers
                        .Select(c => c)
                        .Cache();
            return query.ToList();
        }
    }
}
