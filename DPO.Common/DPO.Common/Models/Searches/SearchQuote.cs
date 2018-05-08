 
namespace DPO.Common
{

    public class SearchQuote : Search
    {
        public SearchQuote() : base() { }

        public SearchQuote(Search search) : base(search) { }

        public long? ProjectId { get; set; }

        public string ProjectIdStr
        {
            get { return (ProjectId!= null) ? ProjectId.ToString() : ""; }
        } 

        public long? QuoteId { get; set; }

        public string QuoteIdStr
        {
            get { return (QuoteId != null) ? QuoteId.ToString() : ""; }
        } 
         
        public string Title { get; set; }

        public bool Deleted { get; set; }

        public bool Active { get; set; }

        public int? Revision { get; set; }

        public bool ShowImportProductPopup { get; set; }

        // Loading options
        public bool LoadQuoteItems { get; set; }
        public bool LoadDiscountRequests { get; set; }

        public bool LoadCommissionRequests { get; set; }
        public bool LoadQuoteOrders { get; set; }

    }

}