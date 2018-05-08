 

namespace DPO.Common
{
    public class SearchCommissionMultiplier: Search
    {
        public SearchCommissionMultiplier() : base() { }

        public SearchCommissionMultiplier(Search search) : base(search) { }

        public MultiplierCategoryTypeEnum? MultiplierCategoryType { get; set; }

        public decimal? Multiplier { get; set; }
    }
}
