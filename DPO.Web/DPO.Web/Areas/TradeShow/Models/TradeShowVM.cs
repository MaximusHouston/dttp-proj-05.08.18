using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
namespace DPO.Web.Areas.Apps.Models
{
    public partial class TradeShowVM
    {
        
        public Requester Requester { get; set; }
        public Event Event { get; set; }
        public Shipping Shipping { get; set; }
        public List<RentingItem> RentingItems { get; set; }
        public int[] SelectedRentingItems { get; set; }
        public int Quantity1 { get; set; }
        public int Quantity2 { get; set; }
        public int Quantity3 { get; set; }
        public int Size { get; set; }

    }
}