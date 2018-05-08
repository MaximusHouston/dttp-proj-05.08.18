using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using DPO.Web.Areas.Apps.Models;

namespace DPO.Web.Areas.TradeShow.Models
{
    public class TradeShowOrder
    {
        public int ID { get; set; }
        public DateTime InsertedDate { get; set; }
        public int RequesterId { get; set; }
        public int EventId { get; set; }
        public int ShippingId { get; set; }
        //public virtual ICollection<RentingItem> Items { get; set; }
    }
}