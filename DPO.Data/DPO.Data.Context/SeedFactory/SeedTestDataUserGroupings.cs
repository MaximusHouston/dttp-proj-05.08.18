//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;

using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;

namespace DPO.Data
{

   /* 
          *    # = Needing approval
          * 
          * USSA0 System Admin
          *    USB0 - Diakin business
          *       USRM1 Regional Manager 1
          *             USGP1 Group 1
          *                   USB1  Business 1
          *                         USAM1 Account Manager 1
          *                         USUA1 User Admin 1
          *                         USU1  User 1 #
          *                   USB2  Business 2
          *                         USAM2 Account Manager 2
          *                         USUA2 User Admin 2 #
          *                         USU2  User 2 #
          *             USGP2 Group 2
          *                   USB3  Business 3
          *                         
          *       USRM2 Regional Manager 2
          *             USGP3 Group 3
          *                   USB3  Business 3
          *                         USAM3 Account Manager 3
          *                         USUA3 User Admin 3
          *                         USU3  User 3 #
          *                   USB4  Business 4
          *                         USAM4 Account Manager 4
          *                         USUA4 User Admin 4 #
          *                         USU4  User 4 #
          *                         USU5  User 5 

          *             USGP5 Group 4
          *                   USB2  Business 2  
          *                   
          *      USRM99 Regional Manager 99 ( Pap data)
          *             USGP99 Group 99
          *                   USB99  Business 99
          *                         USU99 - 999  User 99 - 999
          */

   public partial class SeedFactory 
   {

      public void SeedTestDataUserGroupings()
      {
          if (Db.Users.Any(u => u.Email == "USSA0@Somewhere.com")) return;

         // New registration awaiting approval
         var USSA0 = CreateTestApprovedUser(context,null, "USSA0", AccessLevelEnum.SuperAdmin);
         var USRM1 = CreateTestApprovedUser(context,USSA0, "USRM1", AccessLevelEnum.RegionalManager);
         var USRM2 = CreateTestApprovedUser(context,USSA0, "USRM2", AccessLevelEnum.RegionalManager);
         var USRM99 = CreateTestApprovedUser(context,USSA0, "USRM99", AccessLevelEnum.RegionalManager);

         var USAM1 = CreateTestApprovedUser(context,USRM1, "USAM1", AccessLevelEnum.AccountManager);
         var USAM2 = CreateTestApprovedUser(context,USRM1, "USAM2", AccessLevelEnum.AccountManager);
         var USAM3 = CreateTestApprovedUser(context, USRM2, "USAM3", AccessLevelEnum.AccountManager);
         var USAM4 = CreateTestApprovedUser(context, USRM2, "USAM4", AccessLevelEnum.AccountManager);


         var USUA1 = CreateTestApprovedUser(context, USAM1, "USUA1", AccessLevelEnum.CustomerAdmin);
         var USUA2 = CreateTestUser(context, USAM2, "USUA2", AccessLevelEnum.CustomerAdmin);
         var USUA3 = CreateTestApprovedUser(context, USAM3, "USUA3", AccessLevelEnum.CustomerAdmin);
         var USUA4 = CreateTestUser(context, USAM4, "USUA4", AccessLevelEnum.CustomerAdmin);
         var USU1 = CreateTestUser(context, USUA1, "USU1", AccessLevelEnum.CustomerUser);
         var USU2 = CreateTestUser(context, USUA2, "USU2", AccessLevelEnum.CustomerUser);
         var USU3 = CreateTestUser(context, USUA3, "USU3", AccessLevelEnum.CustomerUser);
         var USU4 = CreateTestUser(context, USUA4, "USU4", AccessLevelEnum.CustomerUser,true);
         var USU5 = CreateTestApprovedUser(context, USUA4, "USU5", AccessLevelEnum.CustomerUser);
         context.Users.Add(USSA0);
         context.Users.Add(USRM1);
         context.Users.Add(USRM2);
         context.Users.Add(USAM1);
         context.Users.Add(USAM2);
         context.Users.Add(USAM3);
         context.Users.Add(USAM4);
         context.Users.Add(USUA1);
         context.Users.Add(USUA2);
         context.Users.Add(USUA3);
         context.Users.Add(USUA4);
         context.Users.Add(USU1);
         context.Users.Add(USU2);
         context.Users.Add(USU3);
         context.Users.Add(USU4);
         context.Users.Add(USU5);

         var USB0 = CreateTestBusiness(context, "USB0");
         var USB1 = CreateTestBusiness(context, "USB1");
         var USB2 = CreateTestBusiness(context, "USB2");
         var USB3 = CreateTestBusiness(context, "USB3");
         var USB4 = CreateTestBusiness(context, "USB4");
         var USB99 = CreateTestBusiness(context, "USB99");
         context.Businesses.Add(USB0);
         context.Businesses.Add(USB1);
         context.Businesses.Add(USB2);
         context.Businesses.Add(USB3);
         context.Businesses.Add(USB4);

         var USGP1 = Db.GroupCreate("USGP1"); USGP1.Write = true; USGP1.GroupOwnerId = USSA0.UserId;
         var USGP2 = Db.GroupCreate("USGP2"); USGP2.Write = true; USGP2.GroupOwnerId = USSA0.UserId;
         var USGP3 = Db.GroupCreate("USGP3"); USGP3.Write = true;USGP3.GroupOwnerId = USSA0.UserId;
         var USGP4 = Db.GroupCreate("USGP4"); USGP4.Write = true;USGP4.GroupOwnerId = USSA0.UserId;
         var USGP99 = Db.GroupCreate("USGP99"); USGP99.Write = true;USGP99.GroupOwnerId = USSA0.UserId;
         context.Groups.Add(USGP1);
         context.Groups.Add(USGP2);
         context.Groups.Add(USGP3);
         context.Groups.Add(USGP4);

         context.SaveChanges();

         // Link Businesses to Groups
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB1, USGP1));
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB2, USGP1));
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB3, USGP2));
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB3, USGP3));
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB4, USGP3));
         context.GroupBusinessLinks.Add(this.Db.GroupLinkToBusiness(USB2, USGP4));

         //Unapproved Pap data
         for (int i = 99; i <=299; i++)
         {
            var user = CreateTestUser(context,USRM99, "USU"+i.ToString(), AccessLevelEnum.CustomerUser);
            context.Users.Add(user);
            user.Business = USB99;
         }

         //Approved Pap data
         for (int i = 500; i <= 299; i++)
         {
            var user = CreateTestApprovedUser(context,USRM99, "USU" + i.ToString(), AccessLevelEnum.CustomerUser);
            context.Users.Add(user);
            user.Business = USB99;
         }

         // Link Owners to Groups
         context.GroupOwnerLinks.Add(this.Db.GroupLinkToOwner(USRM1, USGP1));
         context.GroupOwnerLinks.Add(this.Db.GroupLinkToOwner(USRM1, USGP2));
         context.GroupOwnerLinks.Add(this.Db.GroupLinkToOwner(USRM2, USGP3));
         context.GroupOwnerLinks.Add(this.Db.GroupLinkToOwner(USRM2, USGP4));
         context.GroupOwnerLinks.Add(this.Db.GroupLinkToOwner(USRM99, USGP99));

         //Regional manager belong to super user company
         USSA0.Business = USB0;
         USRM1.Business = USB0;
         USRM2.Business = USB0;
         USRM99.Business = USB0;

         //Bus 1
         USAM1.Business = USB1;
         USUA1.Business = USB1;
         USU1.Business = USB1;

         //Bus 2
         USAM2.Business = USB2;
         USUA2.Business = USB2;
         USU2.Business = USB2;

         //Bus 3
         USAM3.Business = USB3;
         USUA3.Business = USB3;
         USU3.Business = USB3;

         //Bus 4
         USAM4.Business = USB4;
         USUA4.Business = USB4;
         USU4.Business = USB4;
         USU5.Business = USB4;

         context.SaveChanges();
      }

   }
}