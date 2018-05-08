using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public enum CommissionRequestStatusTypeEnum: int
    {
        NewRecord = 0,
        Pending = 2,
        Rejected = 4,
        Approved = 6,
        Deleted = 8,
       
    }
}
