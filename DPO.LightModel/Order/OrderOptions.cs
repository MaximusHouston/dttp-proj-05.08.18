using System;

namespace DPO.Model.Light
{
    public class OrderOptions
    {
        public bool canSubmitOrder { get; set; }
        public bool canViewSubmittedOrder { get; set; } // Can see Submitted Order in Quote
        public bool canViewOrders { get; set; }// Can see Orders Tab
    }
}
