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

   public enum SubmittalSheetTypeEnum : int
   {
        Other = 1,
        AlthermaIndoor = 111503,
        AlthermaOutdoor = 111504,
        AlthermaTank= 111505, 
        MultiSplitIndoor = 111506, 
        MultiSplitOutdoor = 111507, 
        SystemCooling = 111508, 
        SystemHP = 111509, 
        VRVIIIAirCooled= 111510, 
        VRVIIIWaterCooled = 111511, 
        VRVIndoor= 111512, 
        Controllers= 111513, 
        Accessories= 111514, 
        RTU = 111515, 
        Packaged= 111516,
        ACAndHP = 111517,
        CoilsAndAirHandler = 111499,
        GasFurnace= 111502,
        CommercialACAndHP = 111500,
        CommercialAH = 111501,
        PackagedACAndHP = 111518,
        PackagedDualFuel = 111519,
        PackagedGasElectric = 111520
    }

}
