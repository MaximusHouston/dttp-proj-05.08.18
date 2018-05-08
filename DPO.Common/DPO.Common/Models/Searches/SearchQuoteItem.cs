 
namespace DPO.Common
{
    public class SearchQuoteItem : Search
    {
        public SearchQuoteItem() : base() { }

        public SearchQuoteItem(Search search) : base(search) { }

        public long? QuoteId { get; set; }

        public long? QuoteItemId { get; set; }
    }
}