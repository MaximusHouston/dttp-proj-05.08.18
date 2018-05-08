
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
using DPO.Resources;
using NUnit.Framework;
using NUnit.Common;
using Resources = DPO.Resources;

namespace DaikinProjectOffice.Tests
{

   [TestFixture]
   public partial class TestsUserRegistration : TestAdmin
   {
      UserRegistrationModel model = new UserRegistrationModel();

      UserServices service = null;

      public TestsUserRegistration()
      {
         service = new UserServices(this.TContext);
      }

      [Test]
      public void TestsUserRegistration_Make_Sure_Basic_Validation_Occurs_on_the_Model()
      {
          model.IsRegistering = true;
          model.ExistingBusiness = ExistingBusinessEnum.Existing;
          // Fail
          var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => m.Key == "FirstName"));
         Assert.IsTrue(errors.Any(m => m.Key == "LastName"));
         Assert.IsTrue(errors.Any(m => m.Key.Contains("BusinessName")));
         Assert.IsTrue(errors.Any(m => m.Key == "Email"));

         model.FirstName = "blabla";
         model.LastName = "blabla";
         model.Business.BusinessName = "blabla";
         model.Email = "blabla@ssda.com";
        
         model.UseBusinessAddress = true;
         model.IsRegistering = true;

         // Pass
         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => m.Key == "FirstName"));
         Assert.IsFalse(errors.Any(m => m.Key == "LastName"));
         Assert.IsFalse(errors.Any(m => m.Key.Contains("BusinessName")));
         Assert.IsFalse(errors.Any(m => m.Key == "Email"));

      }
      [Test]
      public void TestsUserRegistration_Make_Sure_Company_Name_Exists_If_No_AccountId()
      {
         model.FirstName = "blabla";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Email = "blabla@ssda.com";
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.UseBusinessAddress = true;
         model.Business.BusinessName = "";
         model.IsRegistering = true;
         model.ExistingBusiness = ExistingBusinessEnum.Existing;

         // Fail
         var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);
         Assert.IsTrue(errors.Any(m => m.Key.Contains("BusinessName")));


         // Pass
         model.Business.BusinessName = "dddd";
         model.IsRegistering = true;
         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);
         Assert.IsFalse(errors.Any(m => m.Key.Contains("BusinessName")));

         //pass
         model.Business.BusinessName = "";
         model.IsRegistering = true;
         model.Business.AccountId = this.TContext.Businesses.First().AccountId;
         errors = service.PostModel(null, model).GetMessages(MessageTypeEnum.Error);
         Assert.IsTrue(errors.Any(m => m.Key.Contains("BusinessName")));

      }

      [Test]
      public void TestsUserRegistration_Make_Password_Confirm_Password_Is_Checked()
      {
         // Fail
         model.FirstName = "blabla";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Email = "blabla@ssda.com";
         model.Business.BusinessName = "US4";
         model.Business.AccountId = "A222";
         model.Business.BusinessTypeId = (int)TContext.BusinessTypes.FirstOrDefault().BusinessTypeId;
         model.UseBusinessAddress = true;
         model.Password = "blabla";
         model.ConfirmPassword = "wrongpassword";
         model.IsRegistering = true;
         model.UserTypeId = UserTypeEnum.CustomerUser;
         var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => string.Compare(m.Text, ResourceModelUser.MU010) == 0));

         // Pass
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.IsRegistering = true;
         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => string.Compare(m.Text, ResourceModelUser.MU010) == 0));
  
      }

      [Test]
      public void TestsUserRegistration_Error_If_Invalid_AccountId()
      {
         var db = new Repository(this.TContext);

         model.FirstName = "blabla";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Email = "blabla@ssda.com";
         model.Password = "blabla";
         model.Business.BusinessName = "dffdgd";
         model.Business.BusinessTypeId = (int)BusinessTypeEnum.Daikin;
         model.ConfirmPassword = "blabla";
         model.UseBusinessAddress = true;
         model.UserTypeId = UserTypeEnum.CustomerUser;

         // Fail
         model.Business.AccountId = "fakeaccount";
         model.IsRegistering = true;
         var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => string.Compare(m.Text,Resources.ResourceModelBusiness.BM001) == 0));

         //Pass
         model.Business.AccountId = "A222";
         model.IsRegistering = true;
         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => string.Compare(m.Text, Resources.ResourceModelBusiness.BM001) == 0));
      }

      [Test]
      public void TestsUserRegistration_Check_Email_User_Isnt_Already_In_Use()
      {

         model.FirstName = "blabla";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.Business.BusinessName = "US4";
         model.Business.AccountId = "A222";
         model.UseBusinessAddress = true;
         model.Business.BusinessTypeId = (int)BusinessTypeEnum.Dealer;
         // User from seed data
         model.Email = "USRM1@somewhere.com";
         model.IsRegistering = true;
         model.UserTypeId = UserTypeEnum.CustomerUser;
         // Fail
         var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => string.Compare(m.Text, ResourceModelUser.MU001) == 0));

        
         //Pass
         model.Email = "newuser3453@somewhere.com";

         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => string.Compare(m.Text, ResourceModelUser.MU001) == 0));
      }

      [Test]
      public void TestsUserRegistration_Valid_Account_Id_Must_Exist_And_Cannot_Reg_Same_business_Name()
      {

         var db = new Repository(this.TContext);

         model.FirstName = "blabla";
         model.LastName = "blabla";
         model.Email = "blabla@dfgfd.com";
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.Business.BusinessName = "US4";
         model.UseBusinessAddress = true;
         model.IsRegistering = true;
         var business = db.Businesses.FirstOrDefault();

         model.UseBusinessAddress = true;
         model.Business.AccountId = "dsfdsfssf";
         model.Business.BusinessTypeId = (int)BusinessTypeEnum.Daikin;
         model.UserTypeId = UserTypeEnum.CustomerUser;

         // Fail
         var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsTrue(errors.Any(m => string.Compare(m.Text, Resources.ResourceModelBusiness.BM001) == 0));

         // pass
         db = new Repository(this.TContext);
         model.Business.AccountId = business.AccountId;
         model.IsRegistering = true;
         errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

         Assert.IsFalse(errors.Any(m => string.Compare(m.Text, Resources.ResourceModelBusiness.BM001) == 0));

      }


      [Test]
      public void TestsUserRegistration_Make_Sure_Account_Id_Not_Used()
      {
          var db = new Repository(this.TContext);

          model.FirstName = "new1";
          model.MiddleName = "blabla";
          model.LastName = "blabla";
          model.Email = "new1@dfgfd.com";
          model.Password = "blabla";
          model.ConfirmPassword = "blabla";
          model.Business.BusinessName = "NewBusiness";
          model.Business.AccountId = "1234";
          model.Address = new AddressModel
          {
              StateId = 1
          };
          model.Business.BusinessTypeId = (int)db.BusinessTypes.First().BusinessTypeId;
          model.IsRegistering = true;
          var response = new UserServices(this.TContext).PostModel(null, model);

          Assert.IsFalse(response.IsOK);

          var USRM1 = db.Users.Where(u => u.Email == "USRM1@Somewhere.com").Include(u=>u.Business).FirstOrDefault();

          Assert.IsNotNull(USRM1);

          model.Business.AccountId = USRM1.Business.AccountId;
          model.IsRegistering = true;
          var errors = new UserServices(this.TContext).PostModel(null, model).GetMessages(MessageTypeEnum.Error);

          Assert.IsFalse(errors.Any(m => string.Compare(m.Text, Resources.ResourceModelBusiness.BM003) == 0));

      }

      [Test]
      public void TestsUserRegistration_Unspecified_Account_Id_Means_New_Business_Object_Needed_With_Address_And_Contact()
      {
         var db = new Repository(this.TContext);

         model.FirstName = "new1";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Email = "new1@dfgfd.com";
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.Business.BusinessName = "NewBusiness";

         model.Business.BusinessTypeId = (int)db.BusinessTypes.First().BusinessTypeId;
         model.IsRegistering = true;
         var response = new UserServices(this.TContext).PostModel(null, model);

         var errors = response.Messages.Items;

         Assert.IsFalse(errors.Any(m => m.Key == "AddressLine1"));
         Assert.IsFalse(errors.Any(m => m.Key == "AddressLine2"));
         Assert.IsFalse(errors.Any(m => m.Key == "PostalCode"));
         Assert.IsFalse(errors.Any(m => m.Key == "Location"));
         Assert.IsFalse(errors.Any(m => m.Key == "StateId"));
         Assert.IsFalse(errors.Any(m => m.Key == "CountryCode"));
         Assert.IsFalse(errors.Any(m => m.Key == "OfficeNumber"));

      }

      [Test]
      public void TestsUserRegistration_Check_New_User_Is_Awaiting_Approval_And_Reg_Date_Stamped()
      {
         var db = new Repository(this.TContext);

         model.FirstName = "new1";
         model.MiddleName = "blabla";
         model.LastName = "blabla";
         model.Email = "new1@dfgfd.com";
         model.Password = "blabla";
         model.ConfirmPassword = "blabla";
         model.Business.BusinessName = "US4";
         model.Business.AccountId = "A222";
         model.UseBusinessAddress = true;
         model.UserTypeId = UserTypeEnum.CustomerUser;
         model.Address = new AddressModel
         {
             StateId = 1,
             CountryCode = "US"
         };
         model.Business.BusinessTypeId = (int)db.BusinessTypes.First().BusinessTypeId;
         model.IsRegistering = true;
         var response = new UserServices(this.TContext).PostModel(null, model);

         Assert.IsTrue(response.IsOK);

         var user = db.UserQueryByEmail(model.Email).Include(i => i.Business).FirstOrDefault();

         Assert.IsNotNull(user);

         Assert.IsFalse(user.Approved);

         Assert.IsFalse(user.ApprovedOn.HasValue);

         Assert.IsTrue(user.RegisteredOn.HasValue);
      }

      [Test]
      public void TestsUserRegistration_Confirm_Model_Maps_With_Entity_Properties()
      {
         var db = new Repository(this.TContext);
         var model = new UserRegistrationModel
         {
             FirstName = "new1",
             MiddleName = "blabdsfla",
             LastName = "blabldfsa",
             Email = "new1@dfgfd.com",
             Password = "blablsdfa",
             ConfirmPassword = "blablsdfa",
             UserTypeId = UserTypeEnum.CustomerUser,
             ExistingBusiness = ExistingBusinessEnum.Existing,

             Business = new BusinessModel{
                 BusinessTypeId = (int)BusinessTypeEnum.Dealer,
                 BusinessName = "sdfdsf"
             },
             Address = new AddressModel
             {
                 AddressLine1 = "sadsa",
                 AddressLine2 = "sadsadfd",
                 AddressLine3 = "sadsdfgrega",
                 PostalCode = "12345",
                 Location = "sdfdfsg",
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
         };
         model.IsRegistering = true;
         var response = new UserServices(this.TContext).PostModel(null, model);

         Assert.IsTrue(response.IsOK);

         var user = db.UserQueryByEmail(model.Email).Include(i => i.Address).Include(i => i.Contact).FirstOrDefault();

         Assert.IsNotNull(user);

         Assert.AreEqual(user.FirstName,model.FirstName);
         Assert.AreEqual(user.LastName, model.LastName);
         Assert.AreEqual(user.MiddleName, model.MiddleName);
         Assert.AreEqual(user.Email, model.Email);
         Assert.AreEqual(user.Contact.ContactEmail, model.Contact.ContactEmail);
         Assert.AreEqual(user.Password, Crypto.Hash(model.Password,user.Salt));
         Assert.AreEqual(user.Business.BusinessName, model.Business.BusinessName);
         Assert.AreEqual(user.Address.AddressLine1, model.Address.AddressLine1);
         Assert.AreEqual(user.Address.AddressLine2, model.Address.AddressLine2);
         Assert.AreEqual(user.Address.AddressLine3, model.Address.AddressLine3);
         Assert.AreEqual((int)user.Business.BusinessTypeId, (int)model.Business.BusinessTypeId);
         Assert.AreEqual(user.Address.Location, model.Address.Location);
         Assert.AreEqual(user.Address.StateId, model.Address.StateId);
         Assert.AreEqual(user.Contact.Mobile, model.Contact.MobileNumber);
         Assert.AreEqual(user.Contact.Phone, model.Contact.OfficeNumber);
         Assert.AreEqual(user.Address.PostalCode, model.Address.PostalCode);
         Assert.AreEqual(user.Contact.Website, model.Contact.WebAddress);
         Assert.AreEqual(user.UserTypeId, UserTypeEnum.NotSet);
      }

      [Test]
      public void TestsUserRegistration_Confirm_Model_Maps_With_Entity_And_Business_User_Share_Same_Address_Properties()
      {
          var db = new Repository(this.TContext);
          var model = new UserRegistrationModel
          {
              FirstName = "new1",
              MiddleName = "blabdsfla",
              LastName = "blabldfsa",
              Email = "new1@dfgfd.com",
              Password = "blablsdfa",
              ConfirmPassword = "blablsdfa",
              UserTypeId = UserTypeEnum.CustomerUser,
              ExistingBusiness = ExistingBusinessEnum.Existing,

              Business = new BusinessModel
              {
                  BusinessTypeId = (int)BusinessTypeEnum.Dealer,
                  BusinessName = "sdfdsf"
              },
              Address = new AddressModel
              {
                  AddressLine1 = "line1",
                  AddressLine2 = "line2",
                  AddressLine3 = "line3",
                  PostalCode = "12345",
                  Location = "sdfdfsg",
                  StateId = 1,
                  CountryCode = "US"
              },
              Contact = new ContactModel
              {
                  MobileNumber = "020 111 3456",
                  OfficeNumber = "020 111 3456",
                  WebAddress = "www.dfdf.com"
              },
              UseBusinessAddress = true,
          };
          model.IsRegistering = true;
          var response = new UserServices(this.TContext).PostModel(null, model);

          Assert.IsTrue(response.IsOK);

          var user = db.UserQueryByEmail(model.Email).Include(i => i.Address).Include(i => i.Contact).FirstOrDefault();

          Assert.IsNotNull(user);

          Assert.AreEqual(user.FirstName, model.FirstName);
          Assert.AreEqual(user.LastName, model.LastName);
          Assert.AreEqual(user.MiddleName, model.MiddleName);
          Assert.AreEqual(user.Email, model.Email);
          Assert.AreEqual(user.Contact.ContactEmail, model.Contact.ContactEmail);
          Assert.AreEqual(user.Password, Crypto.Hash(model.Password, user.Salt));
          Assert.AreEqual(user.Business.BusinessName, model.Business.BusinessName);
          Assert.AreEqual(user.Address.AddressLine1, model.Address.AddressLine1);
          Assert.AreEqual(user.Address.AddressLine2, model.Address.AddressLine2);
          Assert.AreEqual(user.Address.AddressLine3, model.Address.AddressLine3);
          Assert.AreEqual((int)user.Business.BusinessTypeId, (int)user.Business.BusinessTypeId);
          Assert.AreEqual(user.Address.Location, model.Address.Location);
          Assert.AreEqual(user.Address.StateId, model.Address.StateId);
          Assert.AreEqual(user.Contact.Mobile, model.Contact.MobileNumber);
          Assert.AreEqual(user.Contact.Phone, model.Contact.OfficeNumber);
          Assert.AreEqual(user.Address.PostalCode, model.Address.PostalCode);
          Assert.AreEqual(user.Contact.Website, model.Contact.WebAddress);
          Assert.AreEqual(user.UserTypeId, UserTypeEnum.NotSet);

          var business = db.Context.Businesses.Where(b=>b.BusinessId == user.BusinessId).Include(i=>i.Address).FirstOrDefault();

          Assert.AreEqual(business.Address.AddressLine1, model.Address.AddressLine1);
          Assert.AreEqual(business.Address.AddressLine2, model.Address.AddressLine2);
          Assert.AreEqual(business.Address.AddressLine3, model.Address.AddressLine3);
      }


      [Test]
      // If a look up take place to get the account id business then its important business details are not updated, for security.
      // The business details are disbaled on the front end so nothing should be updated, but lets make sure
      public void TestsUserRegistration_Confirm_Model_Business_Address_does_not_Update_Existing_Business_Details()
      {
          var db = new Repository(this.TContext);

          var business = db.Businesses.Where(b => b.AccountId != "").FirstOrDefault();

          var model = new UserRegistrationModel
          {
              FirstName = "new1",
              MiddleName = "blabdsfla",
              LastName = "blabldfsa",
              Email = "new1@dfgfd.com",
              Password = "blablsdfa",
              ConfirmPassword = "blablsdfa",

              Business = new BusinessModel
              {
                  BusinessTypeId = (int)BusinessTypeEnum.Daikin,
                  AccountId = business.AccountId,
                  BusinessName = "sdfdsf"
              },
              Address = new AddressModel
              {
                  AddressLine1 = "line1",
                  AddressLine2 = "line2",
                  AddressLine3 = "line3",
                  PostalCode = "12345",
                  Location = "sdfdfsg",
                  StateId = 1,
                  CountryCode = "US"
              },
              Contact = new ContactModel
              {
                  MobileNumber = "020 111 3456",
                  OfficeNumber = "020 111 3456",
                  WebAddress = "www.dfdf.com"
              },
              UseBusinessAddress = true,
          };
          model.IsRegistering = true;
          model.UserTypeId = UserTypeEnum.CustomerUser;
          var response = new UserServices(this.TContext).PostModel(null, model);

          Assert.IsTrue(response.IsOK);

          business = db.Businesses.Where(b => b.AccountId == business.AccountId).FirstOrDefault();

          Assert.AreNotEqual(business.BusinessName, model.Business.BusinessName);
      }

    
   }
}