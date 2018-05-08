 

namespace DPO.Common
{

    public class SearchCommissionRequests : Search
    {
        public SearchCommissionRequests() : base() { }

        public SearchCommissionRequests(Search search) : base(search) { }

        public long? ProjectId { get; set; }

        public long? QuoteId { get; set; }

        public bool PendingRequests { get; set; }

    }

}