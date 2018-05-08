using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DPO.Common
{
    public enum OrderStatusTypeEnum: byte
    {
        [Description("New Record")]
        NewRecord = 1,

        [Description("Submitted")]
        Submitted = 2,

        [Description("Awaiting CSR")]
        AwaitingCSR = 3,

        [Description("Accepted")]
        Accepted = 4,

        [Description("In Process")]
        InProcess = 5,

        [Description("Picked")]
        Picked = 6,

        [Description("Shipped")]
        Shipped = 7,

        [Description("Canceled")]
        Canceled = 8
    }
}
