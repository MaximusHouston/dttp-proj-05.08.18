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
   
   public partial class TestBusinessServices : TestAdmin
   {
      UserSessionModel model = new UserSessionModel();

      BusinessServices service;

      public TestBusinessServices()
      {
         service = new BusinessServices(this.TContext);
      }

      [Test]
      public void TestBusinessServices_Super_Admin_Can_See_All_Businesses()
      {
         var sa = GetUserSessionModel("USSA0@Somewhere.com");

         var search = new SearchBusiness();

         search.PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL;

         var response = service.GetBusinessListModel(null, search);

         var result = response.Model as List<BusinessListModel>;

         // this where clause is necessary becuase the daikin test import currently doesnt have country so canadian businesses
         // have null states
         int count = db.Businesses.Count();

         Assert.AreEqual(result.Count(), count);

      }

      [Test]
      public void TestBusinessServices_Can_See_All_Businesses_Under_User_Groups()
      {
         // Test regional manager1
         var rm1 = GetUserSessionModel("USRM1@Somewhere.com");

         var search = new SearchBusiness();

         var response = service.GetBusinessListModel(rm1, search);

         var results = response.Model as List<BusinessListModel>;

         Assert.AreEqual(results.Count(), 5);

         // Test regional manager2
         var rm2 = GetUserSessionModel("USRM2@Somewhere.com");

         response = service.GetBusinessListModel(rm2, search);

         results = response.Model as List<BusinessListModel>;

         Assert.AreEqual(results.Count(), 5);

      }

      [Test]
      public void TestBusinessServices_Search_Businesses_And_Reorder()
      {
         // Test regional manager1
         var rm1 = GetUserSessionModel("USRM1@Somewhere.com");

         var search = new SearchBusiness
         {
            SortColumn = "BusinessName",
            Page = 1,
            PageSize = 2,
            ReturnTotals = true
         };

         var response = service.GetBusinessListModel(rm1, search);

         var results = response.Model as List<BusinessListModel>;

         Assert.AreEqual(results.Count(), 2);

         Assert.IsTrue(string.Compare(results[0].BusinessName, results[1].BusinessName) < 0); 

         search.IsDesc = true;

         response = service.GetBusinessListModel(rm1, search);

         results = response.Model as List<BusinessListModel>;

         Assert.IsTrue(string.Compare(results[0].BusinessName, results[1].BusinessName) > 0);

      }

      [Test]
      public void TestBusinessServices_Check_Unrelated_Manager_Cannot_View_Business()
      {
         var USAM4 = GetUserSessionModel("USAM4@Somewhere.com");

         var response = service.GetBusinessListModel(USAM4, new SearchBusiness());

         var  results = response.Model as List<BusinessListModel>;

         foreach (var business in results)
         {
            Assert.IsTrue(business.BusinessName != "USB1");
         }

      }

      [Test]
      public void TestBusinessServices_business_can_be_DeActivated()
      {
          var USAM4 = GetUserSessionModel("USAM4@Somewhere.com");

          var US4 = db.Businesses.Where(u => u.BusinessName.Contains("USB4")).FirstOrDefault();

          US4.Enabled = true;

          var response = service.EnableDisable(USAM4, new BusinessModel { BusinessId = US4.BusinessId, Enabled = false });

          US4 = db.Businesses.Where(u => u.BusinessName.Contains("USB4")).FirstOrDefault();

          Assert.IsTrue(response.IsOK);

          Assert.IsFalse(US4.Enabled);

      }

      [Test]
      public void TestBusinessServices_Get_BusinessModel_ToEdit()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var business = db.Businesses.Where(u => u.BusinessName.Contains("USB4")).FirstOrDefault();

          var response = service.GetBusinessModel(sa, business.BusinessId,true);

          var model = response.Model as BusinessModel;

          Assert.IsTrue(response.IsOK);

          Assert.AreEqual(business.BusinessName, model.BusinessName);

      }

      [Test]
      public void TestBusinessServices_Get_BusinessModel_FromAccountId()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var business = db.Businesses.Where(u => string.IsNullOrEmpty(u.AccountId) == false).FirstOrDefault();

          var response = service.GetBusinessModelByAccountId(sa, business.AccountId);

          var model = response.Model as BusinessModel;

          Assert.IsTrue(response.IsOK);

          Assert.AreEqual(business.BusinessName, model.BusinessName);

      }

      [Test]
      public void TestBusinessServices_Distributor_And_ManufacturerRep_Businesses_Must_Have_AccountId()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var business = db.Businesses.Where(u => u.BusinessName.Contains("USB4")).FirstOrDefault();

          var response = service.GetBusinessModel(sa, business.BusinessId, true);

          var model = response.Model as BusinessModel;

          model.AccountId = "";
          model.BusinessTypeId = (int)BusinessTypeEnum.Other;

          service.Response.Messages.Clear();
          service.PostModel(sa, model);

          Assert.IsTrue(response.IsOK);

          model.BusinessTypeId = (int)db.BusinessTypes.Where(t => t.BusinessTypeId == BusinessTypeEnum.Distributor).FirstOrDefault().BusinessTypeId;

          service.Response.Messages.Clear();
          service.PostModel(sa, model);

          Assert.IsFalse(response.IsOK);

          model.BusinessTypeId = (int)db.BusinessTypes.Where(t => t.BusinessTypeId == BusinessTypeEnum.ManufacturerRep).FirstOrDefault().BusinessTypeId;

          service.Response.Messages.Clear();
          service.PostModel(sa, model);

          Assert.IsFalse(response.IsOK);

      }

      [Test]
      public void TestBusinessServices_business_can_be_Added()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var model = new BusinessModel
          {
              BusinessName = "new1",
               
              BusinessTypeId = (int)BusinessTypeEnum.Dealer,

              Address = new AddressModel
              {
                  AddressLine1 = "newline1",
                  AddressLine2 = "newline2",
                  AddressLine3 = "newline3",
                  PostalCode = "12345",
                  Location = "new",
                  StateId = 1,
                  CountryCode = "US"
              },
              Contact = new ContactModel
              {
                  MobileNumber = "020 111 3456",
                  OfficeNumber = "020 111 3456",
                  WebAddress = "www.dfdf.com"
              }
          };


          var response = service.PostModel(sa, model);

          Assert.IsTrue(response.IsOK);

          var business = db.Context.Businesses.Where(u => u.BusinessName == model.BusinessName).Include(u => u.Address).FirstOrDefault();

          Assert.IsNotNull(business);

          Assert.AreEqual(business.BusinessName, model.BusinessName);
          Assert.AreEqual(business.Contact.ContactEmail, model.Contact.ContactEmail);
          Assert.AreEqual(business.Address.AddressLine1, model.Address.AddressLine1);
          Assert.AreEqual(business.Address.AddressLine2, model.Address.AddressLine2);
          Assert.AreEqual(business.Address.AddressLine3, model.Address.AddressLine3);
          Assert.AreEqual(business.Address.Location, model.Address.Location);
          Assert.AreEqual(business.Address.StateId, model.Address.StateId);
          Assert.AreEqual(business.Contact.Mobile, model.Contact.MobileNumber);
          Assert.AreEqual(business.Contact.Phone, model.Contact.OfficeNumber);
          Assert.AreEqual(business.Address.PostalCode, model.Address.PostalCode);
          Assert.AreEqual(business.Contact.Website, model.Contact.WebAddress);
           Assert.AreEqual(business.BusinessTypeId, BusinessTypeEnum.Dealer);
          Assert.IsTrue(business.Enabled);
      }

      [Test]
      public void TestBusinessServices_business_Can_Be_Edited()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var u1 = GetUserSessionModel("USAM4@Somewhere.com");

          var response = service.GetBusinessModel(sa, u1.BusinessId, false);

          var model = response.Model as BusinessModel;

          model.BusinessName = "Testdfg";

          model.Address.AddressLine1 = "sadsa";
          model.BusinessTypeId = (int)BusinessTypeEnum.ManufacturerRep;

          response = service.PostModel(sa, model);

          Assert.IsTrue(response.IsOK);

          var business = db.Context.Businesses.Where(u => u.BusinessId == model.BusinessId).Include(u => u.Address).FirstOrDefault();

          Assert.IsNotNull(business);

          Assert.AreEqual(business.BusinessName, model.BusinessName);
          Assert.AreEqual(business.Address.AddressLine1, model.Address.AddressLine1);
      }

      [Test]
      public void TestBusinessServices_business_Cant_Edit_If_Not_Scope()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var u1 = GetUserSessionModel("USAM3@Somewhere.com");

          var u2 = GetUserSessionModel("USAM4@Somewhere.com");

          var response = service.GetBusinessModel(sa, u2.BusinessId, true);

          var model = response.Model as BusinessModel;

          model.BusinessName = "fdsdfd";

          response = service.PostModel(u1, model);

          Assert.IsFalse(response.IsOK);
      }




   }
}