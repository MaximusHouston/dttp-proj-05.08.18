//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using NUnit.Framework;
using NUnit.Common;

namespace DaikinProjectOffice.Tests
{

   [TestFixture]
   public partial class TestPermissionsServices : TestAdmin
   {
      UserSessionModel model = new UserSessionModel();

      PermissionServices permService;

      public TestPermissionsServices()
      {
          permService = new PermissionServices(this.TContext);
      }


      private int CountSystemAccess(UserSessionModel user)
      {
          return db.Permissions.Where(p => p.ObjectId == (long)user.UserTypeId && p.PermissionTypeId == PermissionTypeEnum.SystemAccess).Count();

      }

      private int CountSystemAccessUser(UserSessionModel user)
      {
          return db.Permissions.Where(p => p.ObjectId == (long)user.UserId && p.PermissionTypeId == PermissionTypeEnum.SystemAccess).Count();

      }

      private int CountPermBusiness(UserSessionModel user, PermissionTypeEnum type)
      {
          return db.Permissions.Where(p => p.ObjectId == user.BusinessId && p.PermissionTypeId == type).Count();
      }

      private int CountPermUser(UserSessionModel user, PermissionTypeEnum type)
      {
          return db.Permissions.Where(p => p.ObjectId == user.UserId && p.PermissionTypeId == type).Count();
      }

      [Test]
      public void TestPermissionsServices_Check_User_CityArea_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.CityArea);
          Assert.IsTrue(list.Count() == CountPermUser(USAM1, PermissionTypeEnum.CityArea));
      }

      [Test]
      public void TestPermissionsServices_Check_Business_CityArea_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.CityArea);
          Assert.IsTrue(list.Count() == CountPermBusiness(USAM1, PermissionTypeEnum.CityArea));
      }

      [Test]
      public void TestPermissionsServices_Check_User_Brand_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForUser(USAM1,USAM1.UserId,PermissionTypeEnum.Brand);
          Assert.IsTrue(list.Count() == CountPermUser(USAM1, PermissionTypeEnum.Brand));
      }

      [Test]
      public void TestPermissionsServices_Check_Business_Brand_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.Brand);
          Assert.IsTrue(list.Count() == CountPermBusiness(USAM1, PermissionTypeEnum.Brand));
      }

      [Test]
      public void TestPermissionsServices_Check_User_Tool_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.Tool);
          Assert.IsTrue(list.Count() == CountPermUser(USAM1, PermissionTypeEnum.Tool));
      }

      [Test]
      public void TestPermissionsServices_Check_Business_Tool_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.Tool);
          Assert.IsTrue(list.Count() == CountPermBusiness(USAM1, PermissionTypeEnum.Tool));
      }

      [Test]
      public void TestPermissionsServices_Check_User_ProductFamily_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.ProductFamily);
          Assert.IsTrue(list.Count() == CountPermUser(USAM1, PermissionTypeEnum.ProductFamily));
      }

      [Test]
      public void TestPermissionsServices_Check_Business_ProductFamily_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  business
          var list = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.ProductFamily);
          Assert.IsTrue(list.Count() == CountPermBusiness(USAM1, PermissionTypeEnum.ProductFamily));
      }

      [Test]
      public void TestPermissionsServices_Check_User_SystemAccess_Permissions()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          // check  default
          var list = permService.GetPermissonsForUser(USAM1, (long)USAM1.UserId, PermissionTypeEnum.SystemAccess);
          Assert.IsTrue(list.Count() == CountSystemAccess(USAM1));

          // check  user
          list = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.SystemAccess);
          Assert.IsTrue(list.Where(p=>p.IsSelected).Count() == CountSystemAccessUser(USAM1));

      }


      [Test]
      public void TestPermissionsServices_Check_Remove_Parent_Permissions_Reduces_Child_List()
      {
          var USAM1 = GetUserSessionModel("USAM1@Somewhere.com");

          var model = new CheckBoxListModel();

          /// Business
          var busPerms = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.Tool);
          Assert.IsTrue(busPerms.Count() == CountPermBusiness(USAM1, PermissionTypeEnum.Tool));

          /// user
          var userPerms = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.Tool);
          Assert.IsTrue(busPerms.Count() == CountPermUser(USAM1, PermissionTypeEnum.Tool));

          // Remove 1
          var perms = new List<PermissionListModel>();
          perms.AddRange(busPerms);
          perms.Remove(busPerms.Where(b => b.IsSelected).FirstOrDefault());
          db.PermissionsUpdate(EntityEnum.Tool, (long)PermissionTypeEnum.Tool, EntityEnum.Business, USAM1.BusinessId, perms, PermissionTypeEnum.Tool);
          db.SaveChanges();

          /// Business defaults
          var busPerms2 = permService.GetPermissonsForBusiness(USAM1, USAM1.BusinessId.Value, PermissionTypeEnum.Tool);
          Assert.IsTrue(busPerms2.Where(p=>p.IsSelected).Count() == CountPermBusiness(USAM1, PermissionTypeEnum.Tool));

          /// user
          var userPerms2 = permService.GetPermissonsForUser(USAM1, USAM1.UserId, PermissionTypeEnum.Tool);
          Assert.IsTrue(userPerms2.Count() == CountPermUser(USAM1, PermissionTypeEnum.Tool));

          Assert.IsTrue(busPerms.Count() == busPerms2.Count);

          Assert.IsTrue(userPerms.Count - 1 == userPerms2.Count);


      }


   }
}