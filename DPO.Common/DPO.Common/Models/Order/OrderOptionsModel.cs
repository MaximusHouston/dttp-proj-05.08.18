

namespace DPO.Common
{
    public class OrderOptionsModel
    {
        public bool CanSubmitOrder { get; set; }
        public bool CanViewSubmittedOrder { get; set; } // Can see Submitted Order in Quote
        public bool CanViewOrders { get; set; }// Can see Orders Tab
    }
}
