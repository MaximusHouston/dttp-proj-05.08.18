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
   public enum ProductNoteTypeEnum : int  
   {
       StandardFeature = 100000000,
       Benefit = 100000001,
       Note = 100000002,
        //TODO: Delete after Sep 01, 2017
        //sStandardAndFeature = 100000003,
       CabinetFeature = 100000004,
       Other = 1
   }

   
}
