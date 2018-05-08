 

namespace DPO.Common
{
    public class QuoteItemModel : SearchQuoteItem, IConcurrency
    {
        public QuoteItemModel()
        {
        }

        public long? ProjectId { get; set; }
        public long? ProductId { get; set; }
        public string ProductNumber { get; set; }
        public string Description { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ListPrice { get; set; }
        public decimal? NetPrice { get; set; }
        public long? AccountMultiplierId { get; set; }
        public QuoteModel Quote { get; set; }
        public decimal Multiplier { get; set; }
        public int LineSequence { get; set; }
        public decimal ExtendedNetPrice { get; set; }
        public string CodeString { get; set; }
        public byte? LineItemTypeId { get; set; }
    }
}

