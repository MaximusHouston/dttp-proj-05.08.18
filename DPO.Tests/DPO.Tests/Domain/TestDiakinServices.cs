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
   public partial class TestsDaikinServices : TestAdmin
   {

       DaikinServices service = null;

      public TestsDaikinServices()
      {
          service = new DaikinServices(this.TContext);
      }

      [TestMethod]
      public void TestsDaikinServices_VerifyAccount()
      {
         var response =  service.VerifyAccount("A-DSFFSDSDFSD");

         Assert.IsFalse((bool)response.Model);


      }

   }
}