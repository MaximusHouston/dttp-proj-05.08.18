 
namespace DPO.Common
{
    public class SearchDiscountRequests : Search
    {
        public SearchDiscountRequests() : base() { }

        public SearchDiscountRequests(Search search) : base(search) { }

        public long? ProjectId { get; set; }

        public long? QuoteId { get; set; }

        public bool PendingRequests { get; set; }
    }
}