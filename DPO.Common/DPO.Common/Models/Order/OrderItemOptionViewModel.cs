using System;
 

namespace DPO.Common
{
    public class OrderItemOptionViewModel
    {
        public long OrderItemOptionId { get; set; }
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        //public int LineSequence { get; set; }
        public long BaseProductId { get; set; }
        public long OptionProductId { get; set; }
        public string BaseProductNumber { get; set; }
        public string OptionProductNumber { get; set; }
        //public long AccountMultiplierId { get; set; }

        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        //public decimal Multiplier { get; set; }
        //public decimal NetPrice { get; set; }
        public DateTime TimeStamp { get; set; }
        //public decimal DiscountPercentage { get; set; }   

        public string CodeString { get; set; }
        public byte LineItemOptionTypeId { get; set; }
        
    }
}
