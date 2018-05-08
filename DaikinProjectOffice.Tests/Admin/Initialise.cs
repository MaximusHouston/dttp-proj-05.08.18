
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using DPO.Common;
using DPO.Data;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.Entity.Core.EntityClient;
using DPO.Domain;
using NUnit.Framework;
using NUnit.Common;
using System.Web.Mvc;

namespace DaikinProjectOffice.Tests
{
  
   public partial class TestAdmin
   {
      public DPOContext TContext = null;

      public Repository db;

      public TestAdmin()
      {
          CreateNewTestContext();
          
      }
      
     
      public void CreateNewTestContext()
      {
          if (TContext != null && TContext.TransactionScope != null)
          {
              TContext.Rollback();
          }
          
          Console.WriteLine("Start before any Test");
          TContext = TestAdmin.TestContext();

          TContext.SetTransactional(System.Data.IsolationLevel.ReadUncommitted);

          db = new Repository(this.TContext);

          this.TContext.ReadOnly = false;
      }
      
     
      public static void InitialiseTests(TestContext test)
      {
         string rootpath = ( AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory) + @"\\..\\..\\..\\..\\";
         bool recreateSchema = DPOContext.GenerateSQLCEedmx(rootpath, @"DPO.Tests\\DPO.Tests\\Context\\DPOContextTest.edmx");
          // file path of the database to create
         var dbTestFilePath = TestDatabasePath();

         if (recreateSchema)
         {
            throw new Exception("New Edmx Test file found and copied. Please publish the new database and compile code");
         }
          var context = TestAdmin.TestContext();

          //new SystemServices().SeedSystemDataDefaults();
         // new SystemTestDataServices().SeedSystemTestData();
      }
     
   
      public void Finish()
      {
         if (this.TContext != null)
         {
            this.TContext.Rollback(); //incase in transaction

            this.TContext.Dispose();
         }

         Console.WriteLine("Start after all tests");
      }
     
   }
}