

namespace DPO.Common
{
    public class QuotePrintModel 
    {
        public ProjectModel Project { get; set; }
        public QuoteModel Quote { get; set; }
        public QuoteItemsModel QuoteItems { get; set; }
        public CommissionRequestModel CommissionRequest { get; set; }

        public bool WithCostPrices { get; set; }
    }
}
