using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;

namespace DPO.Data
{
    public partial class Repository
    {
        public IQueryable<BusinessLink> BusinessLinks
        {
            get { return this.GetDbSet<BusinessLink>(); }
        }
        public BusinessLink BusinessLinkCreate(long BusinessId, long ParentBusinessId) {
            var entity = new BusinessLink()
            {
                BusinessLinkId = this.Context.GenerateNextLongId(),
                BusinessId = BusinessId,
                ParentBusinessId = ParentBusinessId
            };

            this.Context.BusinessLinks.Add(entity);

            return entity;
        }

        public IQueryable<BusinessLink> BusinessLinkQueryByBusinessLinkId(long? BusinessLinkId) {
            var query = from bl in this.BusinessLinks
                         where bl.BusinessLinkId == BusinessLinkId
                         select bl;

            return query;
        }

        public IQueryable<BusinessLink> BusinessLinkQueryByBusinessId(long? BusinessId)
        {
            var query = from bl in this.BusinessLinks
                        where bl.BusinessId == BusinessId
                        select bl;

            return query;
        }

    }
}
