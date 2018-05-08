
using System.Collections.Generic;

namespace DPO.Common
{
    public class QuotePackageSelectedItemModel
    {
        public QuotePackageSelectedItemModel()
        {
            this.Items = new List<int>();
        }
        public string ProductNumber { get; set; }
        public List<int> Items { get; set; }
    }
}
