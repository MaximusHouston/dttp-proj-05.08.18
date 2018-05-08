using System;

namespace DPO.Domain 
{
    public class ERPInvoiceInfo
    {
        public long ProjectID { get; set; }
        public long QuoteID { get; set; }
        public int CompanyNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        public int InvoiceSeq { get; set; }
        public string OrderNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Status { get; set; }
    }
}
