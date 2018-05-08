//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DPO.Common
{

    public enum ProjectStatusTypeEnum : byte
    {
        [Description("Open")]
        Open = 1,

        [Description("Closed Won")]
        ClosedWon = 2,

        [Description("Closed Lost")]
        ClosedLost = 3,

        [Description("Inactive")]
        Inactive = 4
    }


}
