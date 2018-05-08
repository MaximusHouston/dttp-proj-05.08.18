

namespace DPO.Common
{ 
    public class QuoteItemOptionModel
    {
        public QuoteItemOptionModel()
        {
        }

        public long? QuoteItemOptionId { get; set; }
        public long? QuoteItemId { get; set; }
        public long? QuoteId { get; set; }
        public long? BaseProductId { get; set; }
        public long? OptionProductId { get; set; }
        public string OptionProductNumber { get; set; }
        public string OptionProductName { get; set; }
        public int RequiredQuantity { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public LineItemOptionTypeEnum LineItemOptionTypeId { get; set; }
        public string LineItemOptionTypeDescription {
            get {
                return LineItemOptionTypeId.GetDescription(); ;
            } set { }
        }
    }
}
