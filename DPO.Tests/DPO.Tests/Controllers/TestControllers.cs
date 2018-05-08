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
using DPO.Web.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using RazorGenerator.Testing;

namespace DPO.Tests
{

   [TestClass]
   public partial class TestControllers : TestAdmin
   {
      AccountServices service;

      UserRegistrationModel model = new UserRegistrationModel();

      HomeController controller = new HomeController();

      public TestControllers()
      {
         service = new AccountServices(this.TContext);
      }

      [TestMethod]
      public void TODO_TestController_Make_Sure_All_Action_Results_Have_Views()
      {
         var types = System.Reflection.Assembly.GetAssembly(typeof(DPO.Web.Controllers.BaseController)).GetTypes();

         var controllers = types.Where(t => t.Name.EndsWith("Controller")).ToList();

         var views = types.Where(t => t.BaseType.BaseType == typeof(WebViewPage)).ToList();

         var errors = new List<string>();

         foreach (var controller in controllers)
         {
            var controllerName = controller.Name.Replace("Controller", "");

            controller.GetMethods().Where(t => t.ReturnType == typeof(ActionResult) || t.ReturnType == typeof(ViewResult) || t.ReturnType == typeof(PartialViewResult)).ToList().ForEach(m =>
            {
                var view =  controllerName + "." + m.Name;

                if (!views.Any(t => t.FullName == "DPO.Web.Views." + view || t.FullName == "DPO.Web.Views.Shared." + m.Name))
                {
                      errors.Add(view);
                }
            });
         }

         if (errors.Count > 0)
         {
            Assert.Fail("\nViews for the following controller methods were not found.\nMaybe need to create/( or open and save) them or goto http://razorgenerator.codeplex.com/ and install Razor Generator on your machine:\n" + string.Join("\n", errors));
         }

      }

   }
}