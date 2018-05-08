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

    public enum DiscountRequestStatusTypeEnum : int  
   {
        NewRecord = 0,
       Pending = 2,
       Rejected = 4,
       Approved = 6,
       Deleted = 8,
       
   }

   
}
