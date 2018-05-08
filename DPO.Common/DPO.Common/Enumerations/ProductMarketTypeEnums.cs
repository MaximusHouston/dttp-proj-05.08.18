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

   public enum ProductMarketTypeEnum : int  
   {
        [Obsolete("This is now ResidentialSingleFamily", false)]
       Residential = 111038,
       ResidentialSingleFamily = 111038,
       ResidentialMultiFamily = 111041,
       LightCommercial = 111039,
       Commercial = 111040,
       Export = 111042,
       Other = 1
   }

   
}
