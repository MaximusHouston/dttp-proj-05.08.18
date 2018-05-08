using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DPO.Common
{
    public enum ProjectOpenStatusTypeEnum : byte
    {
        [Description("1.Budgeting")]
        Budgeting = 1,

       [Description("2.Design")]
        Design = 2,

       [Description("3.Quote")]
        Quote = 3,

       [Description("4.Submittal")]
       Submittal = 4,

       [Description("5.Rep/Distributor has PO")]
       RepHasPO = 5,

       [Description("6.Daikin has PO")]
       DaikinHasPO = 6
    }
    }
