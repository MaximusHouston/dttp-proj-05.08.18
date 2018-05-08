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

   public enum ProjectTypeEnum : byte
   {
       [Description("Plan & Spec – Daikin Brand Flat Spec")]
        PlanSpecFlatSpec        = 1,

       [Description("Plan & Spec – Daikin Brand Preferred")]
        PlanSpecBaseBid           = 2,

       [Description("Plan & Spec – Daikin Brand Specified")]
        PlanSpecBasisOfDesign   = 3,

       [Description("Plan & Spec – Competitors Preferred")]
        PlanSpecEqual            = 4,

       [Description("Plan & Spec – Competitors Specified")]
        PlanSpecApprovalOnly    = 5,

       [Description("Design/Build")]
        DesignBuild             = 6

   }

   
}
