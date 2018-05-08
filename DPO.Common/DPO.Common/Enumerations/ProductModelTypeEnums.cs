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

    public enum ProductModelTypeEnum : int
    {
        #region Delete after 10/01/2017

        //Accessory = 100000000,
        //Indoor = 100000151,
        //Outdoor = 100000301,
        //System = 100000451,

        #endregion 

        Other = 1,
        All = 100000999,
        Indoor = 111531,
        Outdoor = 111532,
        System = 111533,
        Accessory = 112553,
    }


}
