 

namespace DPO.Common
{
    public class CommissionMultiplierModel : PageModel
    {
        public int? CommissionMultiplierId { get; set; }

        public MultiplierCategoryTypeEnum MultiplierCategoryType { get; set; }

        public decimal? Multiplier { get; set; }

        public decimal? CommissionPercentage { get; set; }
    }

    public class CommissionMultiplierModelV2 
    {
        public int CommissionMultiplierId { get; set; }

        public MultiplierCategoryTypeEnum MultiplierCategoryTypeId { get; set; }

        public decimal Multiplier { get; set; }

        public decimal CommissionPercentage { get; set; }
    }
}
