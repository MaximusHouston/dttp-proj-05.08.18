using System;

namespace DPO.Domain
{
    public class ERPOrderInfo
    {
        public int POKey { get; set; }
        public int CustomerNumber { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public DateTime RequestDate { get; set; }
        public string TermsCode { get; set; }
        public string OrderType { get; set; }
        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToInstruction { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public Decimal TotalAmount { get; set; }
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public string ShipToNumber { get; set; }
        public int CompanyNo { get; set; }
        public long BusinessID { get; set; }
        public string BusinessName { get; set; }
        public string CRMAccountID { get; set; }
        public long ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectRefID { get; set; }
        public long QuoteID { get; set; }
        public string QuoteRefID { get; set; }
        public string Comments { get; set; }
        public Decimal DiscountPercent { get; set; }
        public OrderDetail[] Details { get; set; }
    }

    public class OrderDetail
    {
        public int POKey { get; set; }
        //public string CRMAccountID { get; set; }
        public int LineSeq { get; set; }
        public string ProductNumber { get; set; }
        public string CustomerProductNo { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal NetPrice { get; set; }
        public Decimal ExtendedNetPrice { get; set; }
        public string ProductDescription { get; set; }
        public Decimal DiscountPercent { get; set; }
        public int CompanyNo { get; set; }
    }

    public class OrderResult
    {
        public int POKey { get; set; }
        public bool isSaved { get; set; }
        public string Message { get; set; }
    }

    public class OrderResponse
    {
        public int CompanyNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal DiscountPercent { get; set; }
        public string OrderRefNum { get; set; }
        public int CustomerNumber { get; set; }
        public long ProjectID { get; set; }
        public long QuoteID { get; set; }
        public string OrderStatus { get; set; }
        public string HeaderCode { get; set; }
        public string OrderComment { get; set; }
    }
}
