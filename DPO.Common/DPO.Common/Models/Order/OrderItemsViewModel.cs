using System;
using System.Collections.Generic; 

namespace DPO.Common
{
    public class OrderItemsViewModel
    {
        public OrderItemsViewModel() {
            this.OrderItemOptions = new List<OrderItemOptionViewModel>();
        }
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public int LineSequence { get; set; }
        public long ParentProductId { get; set; }
        public long ProductId { get; set; }
        public long AccountMultiplierId { get; set; }
        public string ParentProductNumber { get; set; }
        public string ProductNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Multiplier { get; set; }
        public decimal NetPrice { get; set; }
        public decimal ExtendedPrice { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal DiscountPercentage { get; set; }
        public MultiplierCategoryTypeEnum MultiplierCategoryTypeId { get; set; }

        public string CodeString { get; set; }
        public byte? LineItemTypeId { get; set; }
        public string ConfigType { get; set; }

        public long QuoteItemId { get; set; }
        public List<OrderItemOptionViewModel> OrderItemOptions { get; set; }
    }
}
