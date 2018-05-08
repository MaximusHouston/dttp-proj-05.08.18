using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{
    public partial class Repository
    {
        public IQueryable<Link> Links
        {
            get
            {
                return this.GetDbSet<Link>();
            }
        }

        public IQueryable<Link> GetActiveLinks(string category)
        {
            if (category == null)
            {
                category = String.Empty;
            }

            return this.Links
                .Where(l => l.Enabled == true && l.LinkCategory.ToLower() == category.ToLower())
                .OrderBy(l => l.Order).ThenBy(l => l.LinkName);
        }
    }
}
