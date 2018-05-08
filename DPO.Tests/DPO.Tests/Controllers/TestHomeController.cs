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
   public partial class TestHomeControllers : TestAdmin
   {
      AccountServices service;

      UserRegistrationModel model = new UserRegistrationModel();

      HomeController controller = new HomeController();

      public TestHomeControllers()
      {
         service = new AccountServices(this.TContext);
      }


   }
}