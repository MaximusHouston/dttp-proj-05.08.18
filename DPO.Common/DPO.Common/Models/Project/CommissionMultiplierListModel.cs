 

namespace DPO.Common
{
    public class CommissionMultiplierListModel : PageModel
    {
        public int? CommissionMultiplierId { get; set; }

        public MultiplierCategoryTypeEnum MultiplierCategoryType { get; set; }

        public decimal? Multiplier { get; set; }

        public decimal? CommissionPercentage { get; set; }
    }
}
