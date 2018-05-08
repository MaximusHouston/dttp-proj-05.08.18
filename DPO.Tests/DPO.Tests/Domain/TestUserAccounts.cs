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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Threading;
using DPO.Resources;


namespace DPO.Tests
{

   [TestClass]
   public partial class TestsUserAccounts: TestAdmin
   {
      AccountServices service;

      UserRegistrationModel model = new UserRegistrationModel();

      public TestsUserAccounts()
      {
         service = new AccountServices(this.TContext);
      }


      [TestMethod]
      public void TestsUserAccounts_User_Login()
      {

         var model = new UserLoginModel();

         var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").FirstOrDefault();
         var lastLogin = USRM1.LastLoginOn;

         var USRM2 = db.Users.Where(u => u.Email == "USRM2@Somewhere.com").FirstOrDefault();

         // Fail
         model.Email = "dsfdsf";
         model.Password = USRM1.Password;
         var errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => m.Key == "Email"));
         Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU004));

         //fail
         model.Email = "dsfdsf";
         model.Password = "dfsd";
         errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => m.Key == "Email"));
         Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU004));

         //fail
         model.Email = USRM1.Email;
         model.Password = "dfsd";
         errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => m.Key == "Email"));
         Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU004));

         //fail
         model.Email = USRM2.Email;
         model.Password = USRM2.Password;
         errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => m.Key == "Email"));
         Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU004));

         //pass
         model.Email = USRM1.Email;
         model.Password = "test";
         errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => m.Key == "Email"));
         Assert.IsFalse(errors.Any(m => m.Text == ResourceModelUser.MU004));

         var user = db.Users.Where(u=>u.UserId == USRM1.UserId).FirstOrDefault();

         Thread.Sleep(10);
         Assert.AreNotEqual(lastLogin, user.LastLoginOn);

      }

      [TestMethod]
      public void TestsUserAccounts_User_Login_Disabled_User_Cant_Login()
      {

          var model = new UserLoginModel();

          var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").FirstOrDefault();
          
          USRM1.Enabled = false;

          db.SaveChanges();

          //pass
          model.Email = USRM1.Email;
          model.Password = "test";
          var errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

          Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU005));

      }

      [TestMethod]
      public void TestsUserAccounts_User_Login_Disabled_Business_Cant_Login()
      {

          var model = new UserLoginModel();

          var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").Include(u=>u.Business).FirstOrDefault();

          USRM1.Business.Enabled = false;

          db.SaveChanges();

          //pass
          model.Email = USRM1.Email;
          model.Password = "test";
          var errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

          Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU022));

      }

      [TestMethod]
      public void TestsUserAccounts_User_Login_Unapproved_User_Cant_Login()
      {

          var model = new UserLoginModel();

          var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").Include(u => u.Business).FirstOrDefault();

          USRM1.Approved = false;

          db.SaveChanges();

          //pass
          model.Email = USRM1.Email;
          model.Password = "test";
          var errors = service.Login(model).GetMessages(MessageTypeEnum.Error);

          Assert.IsTrue(errors.Any(m => m.Text == ResourceModelUser.MU006));

      }

      [TestMethod]
      public void TestsUserAccounts_User_RequestNewPassword()
      {

         var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").FirstOrDefault();

         var model = new UserResetPasswordModel();

         // Fail
         model.Email = "dsfdsf";
         var errors = service.RequestNewPassword(model).GetMessages(MessageTypeEnum.Error);
         Assert.IsTrue(errors.Any(m => m.Key == "Email"));

         //fail
         model.Email = USRM1.Email;
         errors = service.RequestNewPassword(model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => m.Key == "Email"));
       

      }

      [TestMethod]
      public void TestsUserAccounts_Security_Encrypt_Decrypt()
      {

         var code = Crypto.Encrypt("rubbish");
         var dec = Crypto.Decrypt(code);

         Assert.IsTrue("rubbish" == dec);
      }

      [TestMethod]
      public void TestsUserAccounts_Security_TimeOutCheck()
      {

         UserResetPasswordModel model = new UserResetPasswordModel { Email = "asdsad@asdasd.com" };

         model.GenerateSecurityKey(DateTime.UtcNow);

         var response = service.ValidateSecurityKey(new UserResetPasswordModel { SecurityKey = model.SecurityKey });

         Assert.IsTrue(response.IsOK);

         model.GenerateSecurityKey(DateTime.UtcNow.AddMinutes(-29));

         response = service.ValidateSecurityKey(new UserResetPasswordModel { SecurityKey = model.SecurityKey });

         Assert.IsTrue(response.IsOK);

         model.GenerateSecurityKey(DateTime.UtcNow.AddMinutes(-31));

         response = service.ValidateSecurityKey(new UserResetPasswordModel { SecurityKey = model.SecurityKey });

         Assert.IsFalse(response.IsOK);

      }


      [TestMethod]
      public void TestsUserAccounts_User_Request_Password_Reset()
      {
         var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").FirstOrDefault();

         var model = new UserResetPasswordModel();

         // Fail wrong security key
         model.Email = USRM1.Email;
         model.SecurityKey = Crypto.Encrypt("rubbish");
         model.NewPassword = "abcde12345"; ;
         model.ConfirmPassword = "abcde12345";

         var response = service.ResetPassword(model);

         Assert.IsFalse(response.IsOK);

         //pass
         model.GenerateSecurityKey();
         model.Email = "";

         response = service.ResetPassword(model);

         Assert.IsTrue(response.IsOK);

         Assert.AreEqual(model.Email, USRM1.Email);

         db = new Repository(this.TContext); // save took place

         var user = db.Users.Where(u => u.Email == model.Email).FirstOrDefault();

         Assert.IsNotNull(user);

         Assert.IsTrue(user.Password == Crypto.Hash(model.NewPassword, user.Salt));

      }
   }
}