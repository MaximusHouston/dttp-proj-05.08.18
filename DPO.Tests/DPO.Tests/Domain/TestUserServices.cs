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

namespace DPO.Tests
{

   [TestFixture]
   public class TestUserServices : TestAdmin
   {
      UserSessionModel model = new UserSessionModel();

      UserServices service;

      public TestUserServices()
      {
         service = new UserServices(this.TContext);
      }

      [Test]
      public void TestUserServices_Super_Admin_Can_See_All_Requiring_Approval()
      {
          var sa = GetUserSessionModel("daikincity@daikincomfort.com");

         var search = new SearchUser
         {
            Approved = false,PageSize = 0
         };
         var response = service.GetUserListModel(null, search);
         var result = response.Model as List<UserListModel>;

         var count = this.TContext.Users.Where(u => u.Approved == false).Count();

         Assert.That(result.Count(), Is.EqualTo(count));
      }


      [Test]
      public void TestUserServices_Is_Search_For_WildCard_User_Name_Working()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var search = new SearchUser
          {
              Filter = "SSA0@Somewhere",
              PageSize = 9999
          };

          var response = service.GetUserListModel(null, search);

          var result = response.Model as List<UserListModel>;

          result = response.Model as List<UserListModel>;
          Assert.AreEqual(result.Count(), 1);

      }

      [Test]
      public void TestUserServices_Search_Approvals_And_Reorder()
      {
         // Test regional manager1
         var admin = GetUserSessionModel("USRM2@Somewhere.com");

         var search = new SearchUser
         {
            SortColumn = "DisplayName",
            Page = 1,
            PageSize = 2,
            ReturnTotals = true
         };

         var response = service.GetUserListModel(admin, search);

         var results = response.Model as List<UserListModel>;

         Assert.AreEqual(results.Count(), 2);

         Assert.IsTrue(string.Compare(results[0].DisplayName, results[1].DisplayName) < 0);

         search.IsDesc = true;

         response = service.GetUserListModel(admin, search);

         results = response.Model as List<UserListModel>;

         Assert.IsTrue(string.Compare(results[0].DisplayName, results[1].DisplayName) > 0);

         foreach (var user in results)
         {
            Assert.IsTrue(admin.UserTypeId >= user.UserTypeId);
         }

      }

      [Test]
      public void TestUserServices_Check_Unrelated_Manager_Cannot_Approve()
      {
         var USAM4 = GetUserSessionModel("USAM4@Somewhere.com");

         var US1 = db.Users.Where(u => u.Email == "US1@Somewhere.com").FirstOrDefault();

         var response = service.ChangeUserStatus(USAM4, new UserModel { UserId = US1.UserId, Approved = true });

         Assert.IsFalse(response.IsOK);

         Assert.IsTrue(response.Messages.Items[0].Text == Resources.DataMessages.DM004);

      }

      [Test]
      public void TestUserServices_Check_Lower_Access_User_Cannot_Approve_higher_access_user()
      {
          var US2 = GetUserSessionModel("US2@Somewhere.com");

          var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").FirstOrDefault();

          var response = service.ChangeUserStatus(US2, new UserModel { UserId = USRM1.UserId, Approved = true });

         Assert.IsFalse(response.IsOK);

         Assert.IsTrue(response.Messages.Items[0].Text == Resources.DataMessages.DM004);

      }


      [Test]
      public void TestUserServices_Ignore_if_user_already_approved()
      {
         var admin = GetUserSessionModel("USRM2@Somewhere.com");

         var US6 = db.Users.Where(u => u.Email == "US6@Somewhere.com").FirstOrDefault();

         var response = service.ChangeUserStatus(admin, new UserModel { UserId = US6.UserId,UserTypeId = UserTypeEnum.CustomerUser,  Approved = true });

         Assert.IsTrue(response.IsOK);
      }

      [Test]
      public void TestUserServices_user_can_be_approved()
      {
          var admin = GetUserSessionModel("USRM2@Somewhere.com");

         var US6 = db.Users.Where(u => u.Email == "US6@Somewhere.com").FirstOrDefault();
           
         Assert.IsFalse(US6.Approved);

         Assert.IsNull(US6.ApprovedOn);

         var response = service.ChangeUserStatus(admin, new UserModel { UserId = US6.UserId,UserTypeId = UserTypeEnum.CustomerUser, Approved = true });

         Assert.IsTrue(response.IsOK);

         Assert.IsTrue(US6.Approved);

         Assert.IsNotNull(US6.ApprovedOn);

      }

      [Test]
      public void TestUserServices_user_can_be_DeActivated()
      {
          var admin = GetUserSessionModel("USRM2@Somewhere.com");

         var US4 = db.Users.Where(u => u.Email == "US4@Somewhere.com").FirstOrDefault();

         US4.Enabled= true;

         db.SaveChanges();

         US4 = db.Users.Where(u => u.Email == "US4@Somewhere.com").FirstOrDefault();

         var response = service.ChangeUserStatus(admin, new UserModel { UserId = US4.UserId, UserTypeId = US4.UserTypeId, Enabled = false });

         US4 = db.Users.Where(u => u.Email == "US4@Somewhere.com").FirstOrDefault();

         Assert.IsTrue(response.IsOK);

         Assert.IsFalse(US4.Enabled);

      }

      [Test]
      public void TestUserServices_Add_User_and_Use_Own_business_Address()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var business = db.Businesses.Where(b => b.AccountId != "").FirstOrDefault();

          var model = new UserModel
          {
              FirstName = "new1",
              MiddleName = "blabdsfla",
              LastName = "blabldfsa",
              Email = "new1@dfgfd.com",
               UserTypeId = UserTypeEnum.CustomerUser,
                
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
              },
              UseBusinessAddress = false,
              Business = new BusinessModel { BusinessId = business.BusinessId }
          };

          model.UserTypeId = UserTypeEnum.CustomerUser;
          var response = new UserServices(this.TContext).PostModel(sa, model);

          Assert.IsTrue(response.IsOK);

          var user = db.Context.Users.Where(u=>u.Email == model.Email).Include(u=>u.Business).Include(u=>u.Address).FirstOrDefault();

          Assert.IsNotNull(user);

          Assert.AreEqual(user.FirstName, model.FirstName);
          Assert.AreEqual(user.LastName, model.LastName);
          Assert.AreEqual(user.MiddleName, model.MiddleName);
          Assert.AreEqual(user.Email, model.Email);
          Assert.AreEqual(user.Contact.ContactEmail, model.Contact.ContactEmail);
          Assert.AreEqual(user.BusinessId, model.Business.BusinessId);
          Assert.AreEqual(user.Address.AddressLine1, model.Address.AddressLine1);
          Assert.AreEqual(user.Address.AddressLine2, model.Address.AddressLine2);
          Assert.AreEqual(user.Address.AddressLine3, model.Address.AddressLine3);
          Assert.AreEqual(user.Address.Location, model.Address.Location);
          Assert.AreEqual(user.Address.StateId, model.Address.StateId);
          Assert.AreEqual(user.Contact.Mobile, model.Contact.MobileNumber);
          Assert.AreEqual(user.Contact.Phone, model.Contact.OfficeNumber);
          Assert.AreEqual(user.Address.PostalCode, model.Address.PostalCode);
          Assert.AreEqual(user.Contact.Website, model.Contact.WebAddress);
          Assert.AreEqual(user.UserTypeId, UserTypeEnum.CustomerUser);
          Assert.IsTrue(user.Enabled);
      }

 
      [Test]
      public void TestUserServices_User_Can_Be_Edited()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var u1 = GetUserSessionModel("USAM4@Somewhere.com");

          var response = service.GetUserModel(sa, u1.UserId, false);

          var model = response.Model as UserModel;

          model.FirstName = "Testdfg";

          model.Address.AddressLine1 = "sadsa";
        model.Address.AddressLine2 = "sadsadfd";
        model.Address.AddressLine3 = "sadsdfgrega";
        model.Address.PostalCode = "12345";
        model.Address.Location = "sdfdfsg";
        model.Address.StateId = 1;
        model.Address.CountryCode = "US";

          model.UseBusinessAddress = false;

          response = new UserServices(this.TContext).PostModel(sa, model);

          Assert.IsTrue(response.IsOK);

          var user = db.Context.Users.Where(u => u.Email == model.Email).Include(u => u.Address).FirstOrDefault();

          Assert.IsNotNull(user);

          Assert.AreEqual(user.FirstName, model.FirstName);
          Assert.AreEqual(user.Address.AddressLine1, model.Address.AddressLine1);
      }

      [Test]
      public void TestUserServices_User_Cant_Edit_If_Not_Scope()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var u1 = GetUserSessionModel("USAM3@Somewhere.com");

          var u2 = GetUserSessionModel("USAM4@Somewhere.com");

          var response = service.GetUserModel(sa, u2.UserId, false);

          var model = response.Model as UserModel;

          model.UserTypeId = UserTypeEnum.CustomerUser;

          response = new UserServices(this.TContext).PostModel(u1, model);

          Assert.IsFalse(response.IsOK);

      }

      [Test]
      public void TestUserServices_User_Cant_Change_Business_Type_If_Own_Record()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var response = service.GetUserModel(sa, sa.UserId, false);

          var model = response.Model as UserModel;

          model.UserTypeId = UserTypeEnum.CustomerUser;

          response = new UserServices(this.TContext).PostModel(sa, model);

          var newmodel = service.GetUserModel(sa, sa.UserId, false).Model as UserModel;

          Assert.IsTrue(newmodel.UserTypeId.Value == sa.UserTypeId);

      }

      [Test]
      public void TestUserServices_User_Cant_Change_Business_If_Own_Record()
      {
          var db = new Repository(this.TContext);

          var sa = GetUserSessionModel("US1@Somewhere.com");

          var response = service.GetUserModel(sa, sa.UserId, false);

          var model = response.Model as UserModel;

          model.Business.BusinessId = db.Context.Businesses.Where(b => b.BusinessId != model.Business.BusinessId).FirstOrDefault().BusinessId;

          response = new UserServices(this.TContext).PostModel(sa, model);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == Resources.SystemMessages.SM007));

      }

      [Test]
      public void TestBusinessServices_Udser_Type_cant_Be_Changed_To_Higher_Level_Than_Scope()
      {
          var db = new Repository(this.TContext);

          var u1 = GetUserSessionModel("USRM1@Somewhere.com");

          var u2 = GetUserSessionModel("USAM3@Somewhere.com");

          var response = service.GetUserModel(u1, u2.UserId, false);

          var model = response.Model as UserModel;

          model.UserTypeId = u1.UserTypeId;

          response = new UserServices(this.TContext).PostModel(u1, model);

          Assert.IsTrue(response.Messages.Items.Any(m => m.Text == Resources.DataMessages.DM005));
      }

   }
}