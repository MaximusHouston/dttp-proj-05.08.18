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
   
   public enum UserTypeEnum : byte  
   {
      Systems        = 255,

      DaikinSuperUser     = 190,
      DaikinAdmin = 170,
      DaikinEmployee = 150,
       
      CustomerSuperUser  = 90,
      CustomerAdmin       = 70,
      CustomerUser        = 50,

      NotSet         = 0,

      OtherType = 10
   }

   
}
