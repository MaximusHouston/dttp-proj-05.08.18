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

   public enum ConstructionTypeEnum : byte
   {
       [Description("New")]
        New = 1,

       [Description("Refurbished")]
        Refurbished = 2,

       [Description("Replacement")]
        Replacement = 3,
   }

   
}
