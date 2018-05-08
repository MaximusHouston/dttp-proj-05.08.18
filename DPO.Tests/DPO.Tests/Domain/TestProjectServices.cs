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
using System.Net.Mail;

namespace DPO.Tests
{

   [TestClass]
   public partial class TestProjectServices : TestAdmin
   {
      UserSessionModel sa = new UserSessionModel();

      ProjectServices service;

      SystemTestDataServices systemService;

      BusinessServices businessService;

      ProjectType projecttypes;
      ProjectOpenStatusType ProjectOpenStatus;
      ProjectStatusType projectstatus;
      VerticalMarketType verticaltype;



      public TestProjectServices()
      {
          systemService = new SystemTestDataServices(this.TContext);

          service = new ProjectServices(this.TContext);

          businessService = new BusinessServices(this.TContext);

          //sa = GetUserSessionModel("USSA0@Somewhere.com");
          sa = GetUserSessionModel("User15@test.com");

          projecttypes = db.ProjectTypes.FirstOrDefault();
          ProjectOpenStatus = db.ProjectOpenStatusTypes.FirstOrDefault();
          projectstatus = db.ProjectStatusTypes.FirstOrDefault();
          verticaltype = db.VerticalMarketTypes.FirstOrDefault();

      }

       //[TestMethod]
       //public void TestProjectServices_Rules_Once_Created_Project_Date_Cannot_Be_Changed()
       //{
       //    var model = GetProjectModel(sa,"Project 1");

       //    var newdate = new DateTime();

       //    model.ProjectDate = newdate;

       //    var response = service.PostModel(sa, model);

       //    Assert.IsTrue(response.Messages.Items.Any(m=>m.Text == Resources.ResourceModelProject.MP011));

       //}

      [TestMethod]
      public void TestProjectServices_Rules_Project_Duplicate()
      {
          var model = GetProjectModel(sa, "Project 1");

          var response = service.Duplicate(sa,model);

          Assert.IsTrue(response.IsOK);

          var newProject = GetProjectModel(sa, (response.Model as Project).Name);

          AssertPropertiesThatMatchAreEqual(newProject, model, false, new string[] { "Timestamp", "Name", "OwnerId","Concurrency", "ProjectId", "ProjectDate" });

          AssertPropertiesThatMatchAreEqual(newProject.ActiveQuoteSummary, model.ActiveQuoteSummary, false, new string[] { "Timestamp", "Revision", "Concurrency", "ProjectId", "QuoteId" });

      }


       [TestMethod]
       public void TestProjectServices_Rules_Project_Date_Before_Delivery_Date()
       {
           var model = GetProjectModel(sa, "Project 1");

           model.EstimatedDelivery = model.ProjectDate.Value.AddDays(-1);

           var response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text == Resources.ResourceModelProject.MP006));
       }


       [TestMethod]
       public void TestProjectServices_Rules_Project_Date_Before_Estimated_Close_Date()
       {
           var model = GetProjectModel(sa, "Project 1");

           model.BidDate = model.ProjectDate;

           model.EstimatedClose = model.BidDate.Value.AddDays(-1);

           var response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP002));
       }

       [TestMethod]
       public void TestProjectServices_Rules_Check_Project_Mandatory_Fields()
       {
           var model = GetProjectModel(sa, "Project 1");

           var response = service.PostModel(sa, model);

           Assert.IsTrue(response.IsOK);

           model.EstimatedClose = null;
           model.EstimatedDelivery = null;

           service.Response.Messages.Clear();

           response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Estimated Close")));
           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Estimated Delivery")));

           model.ProjectDate = DateTime.Now;
           model.BidDate = DateTime.Now;
           model.EstimatedClose = DateTime.Now;
           model.EstimatedDelivery = DateTime.Now;

           model.ConstructionTypeId = null;
           model.ProjectOpenStatusTypeId = null;
           model.ProjectStatusTypeId = null;
           model.ProjectTypeId = null;
           model.VerticalMarketTypeId = 0;

           service.Response.Messages.Clear();

           response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Project Open Status")));
           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Project Status")));
           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Project Type")));
           Assert.IsTrue(response.Messages.Items.Any(m=>m.Text.Contains("Vertical Market")));
           Assert.IsTrue(response.Messages.Items.Any(m => m.Text.Contains("Construction Type")));


       }

       [TestMethod]
       public void TestProjectServices_Rules_New_Project_Copies_Business_Address_To_Seller_Address()
       {
          var user = GetUserSessionModel("USAM1@Somewhere.com");

          var business = GetBusinessModel(sa, "USB1");

          var newProject = service.GetProjectModel(user, null).Model as ProjectModel;

        newProject.VerticalMarketTypeId = 1;
        newProject.ProjectDate = DateTime.Now;
        newProject.ConstructionTypeId = 1;
        newProject.ProjectStatusTypeId = 1;
        newProject.ProjectOpenStatusTypeId = 1;
        newProject.CustomerName = "alan";
        newProject.Name = "test";
        newProject.ProjectTypeId = 1;
        newProject.EstimatedDelivery = DateTime.Now.AddDays(1);
        newProject.EstimatedClose = DateTime.Now.AddDays(1);
        newProject.Expiration = DateTime.Now.AddDays(1);
        newProject.Description = "test";
        newProject.EngineerName = "engine" ;                    
         

          var model = service.PostModel(user,newProject).Model as ProjectModel;

          Assert.AreNotEqual(business.Address.AddressId, model.SellerAddress.AddressId);
          Assert.AreEqual(business.Address.AddressLine1, model.SellerAddress.AddressLine1);
          Assert.AreEqual(business.Address.AddressLine2, model.SellerAddress.AddressLine2);
          Assert.AreEqual(business.Address.AddressLine3, model.SellerAddress.AddressLine3);
          Assert.AreEqual(business.Address.Location, model.SellerAddress.Location);
          Assert.AreEqual(business.Address.PostalCode, model.SellerAddress.PostalCode);
          Assert.AreEqual(business.Address.CountryCode, model.SellerAddress.CountryCode);
       }

       [TestMethod]
       public void TestProjectServices_Create_New_Project_Check()
       {
           var newProject = service.GetProjectModel(sa, null).Model as ProjectModel;

           var saveDatetime = DateTime.Today;

           newProject.VerticalMarketTypeId = 2;
           newProject.ProjectDate = saveDatetime;
           newProject.ConstructionTypeId = 1;
           newProject.ProjectStatusTypeId = 2;
           newProject.ProjectOpenStatusTypeId = 2;
           newProject.CustomerName = "alan";
           newProject.Name = "test44354543";
           newProject.ProjectTypeId = 2;
           newProject.BidDate = saveDatetime.AddDays(1);
           newProject.EstimatedClose = saveDatetime.AddDays(2);
           newProject.EstimatedDelivery = saveDatetime.AddDays(3);
           newProject.Expiration = saveDatetime.AddDays(4);
           newProject.Description = "test";
           newProject.EngineerName = "engine";

           var response = service.PostModel(sa, newProject);

           Assert.IsTrue(response.IsOK);

           var model = response.Model as ProjectModel;

           var savedProject = GetProjectModel(sa, newProject.Name);

           AssertPropertiesThatMatchAreEqual(newProject, savedProject, false, new string[] { "Timestamp","Concurrency","ProjectId","ProjectDate","OwnerId", "ProjectTypeDescription", "ProjectOpenStatusDescription", "ProjectStatusDescription", "ConstructionTypeDescription", "VerticalMarketDescription" });
       }

       [TestMethod]
       public void TestProjectServices_Rules_When_Project_Is_Closed_No_Edits_Allowed_Only_When_Open()
       {
           var model = GetProjectModel(sa, "Project 1");

           model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.ClosedWon;

           model.Name = "dsdf";

           var response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items[0].Text == Resources.ResourceModelProject.MP012);

           model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.ClosedLost;

           service.Response.Messages.Clear();

           response = service.PostModel(sa, model);

           Assert.IsTrue(response.Messages.Items[0].Text == Resources.ResourceModelProject.MP012);

           service.Response.Messages.Clear();

           model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.Open;

           response = service.PostModel(sa, model);

           Assert.IsTrue(response.IsOK);

           service.Response.Messages.Clear();

           model.Name = "dsdf";

           response = service.PostModel(sa, model);

           Assert.IsTrue(response.IsOK);


       }


   }
}