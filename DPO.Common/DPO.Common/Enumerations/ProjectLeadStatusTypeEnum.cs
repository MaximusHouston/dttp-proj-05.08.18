using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public enum ProjectLeadStatusTypeEnum : byte
    {
        Lead = 1,
        Opportunity = 2,
        OpenOrder = 3,
        Shipped = 4,
        Disqualified = 5
    }
}
