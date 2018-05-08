 

namespace DPO.Common
{
    public class SearchOrders: Search
    {
       public SearchOrders() : base() { }

        public SearchOrders(Search search) : base(search) { }

        public long? ProjectId { get; set; }

        public long? QuoteId { get; set; }

        public bool SubmittedOrders { get; set; }
    }
}
