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

namespace DPO.Common
{
    public enum BusinessTypeEnum : int
    {
        Unknown = 1,
        Daikin = 100000,
        ManufacturerRep = 200003,
        Distributor = 200000,
        Dealer = 200001,
        EngineerArchitect = 200005,
        Other = 100000000
    }
}
