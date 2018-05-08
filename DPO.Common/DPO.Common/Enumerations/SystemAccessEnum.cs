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
   
   public enum SystemAccessEnum 
   {
        None = 1,
        ManageGroups = 20,

        ApproveUsers = 30,
        ViewUsers = 32,
        EditUser = 34,
        AdminAccessRights = 38,
        UndeleteUser = 36,

        ViewBusiness = 40,
        EditBusiness = 42,
        UndeleteBusiness = 44,

        ViewProject = 50,
        EditProject = 52,
        UndeleteProject = 54,     
        //ShareProject = 56,
        TransferProject = 58,
        ViewProjectsInGroup = 59,

        RequestDiscounts = 60,
        ApproveDiscounts = 62,
        
        ViewOrder = 67,
        SubmitOrder = 68,
        
        //CMS access permissions
        ContentManagementHomeScreen = 70,
        ContentManagementFunctionalBuildings = 71,
        ContentManagementApplicationBuildings = 72,
        ContentManagementApplicationProducts = 73,
        ContentManagementLibrary = 74,
        ContentManagementCommsCenter = 75,
        ContentManagementProductFamilies = 76,
        ContentManagementTools = 77,

       // Pipeline Access Permissions
       ViewPipelineData = 80,
       EditPipelineData = 82,

       //View Discount Request
       ViewDiscountRequest = 63,

       RequestCommission = 64,
       ApprovedRequestCommission = 65,
       ViewRequestedCommission = 66
      
   }
}
