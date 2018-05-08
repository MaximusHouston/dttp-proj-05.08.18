

namespace DPO.Common
{
    public class QuoteOptionsModel
    {
        public bool CanEditQuote { get; set; }
        public bool CanDeleteQuote { get; set; }
        public bool CanUnDeleteQuote { get; set; }
        public bool CanSetActive { get; set; } 
        public bool CanRequestDiscount { get; set; }
        public bool CanRequestCommission { get; set; }
        public bool CanCalculateCommission { get; set; }
        public bool CanAddProducts { get; set; }
        public bool CanEditTags { get; set; }
        public bool CanDuplicate { get; set; }
        
    }
}
