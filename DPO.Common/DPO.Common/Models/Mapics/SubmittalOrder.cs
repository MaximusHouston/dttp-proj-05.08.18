 
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DPO.Common
{
    public class SubmittalOrder
    {
        public string CompanyNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string ConfigOrder { get; set; }
        public SalesOrder SalesOrder {get;set;}
        public MfgOrder MfgOrder { get; set; }
    }

    public class SalesOrder
    {
        public string PONumber { get; set; }
        public string POFile { get; set; }
        public string PODate { get; set; }
        public string RequestDate { get; set; }
        public string OrderType { get; set; }
        public ShipTo ShipTo { get; set; }
        public string ShippingInstructions { get; set; }
        public OrderContact Contact { get; set; }
        public string Comments { get; set; }
        public long BusinessID { get; set; }
        public long ProjectID { get; set; }
        public long QuoteID { get; set; }
        [XmlArray(ElementName = "Lines")]
        public List<LineItem> LineItems { get; set; }
    }

    public class ShipTo
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class OrderContact
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }

    [XmlType(TypeName = "Line")]
    public class LineItem
    {
        public int LineSequence { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string CodeString { get; set; }
        public string BaseModel { get; set; }
    }

    public class MfgOrder
    {
        public List <MfgModel> Models { get; set; }
    }

    [XmlType(TypeName = "Model")]
    public class MfgModel
    {
        public string ConfigType { get; set; }
        public string CodeString { get; set; }
        public string ModelNumber { get; set; }
        public int Quantity { get; set; }
        public MfgBaseModel BaseModel { get; set; }
        public List<MfgAccessory> Accessories { get; set; }
    }

    [XmlType(TypeName = "BaseModel")]
    public class MfgBaseModel
    {
        public string BaseModelNumber { get; set; }
        public string Efficiency { get; set; }
    }

    [XmlType(TypeName = "Accessory")]
    public class MfgAccessory
    {
        public string AccessoryItemNumber { get; set; }
    }
}
