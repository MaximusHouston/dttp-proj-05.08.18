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
using System.Data.Entity.Core;
using System.Data.Common;
using DPO.Domain;

namespace DPO.Tests
{

   [TestClass]
   public partial class TestsDataContext : TestAdmin
   {

      /// <summary>
      // If any of these fail its maybe because the Concurrency mode properties option 
      // on the timestamp field of that entity hasn't been set to "Fixed"
      /// </summary>
      [TestMethod]
      public void TestsDataContext_Make_Sure_Entities_With_Timestamp_Fields_Have_Concurrency_Switched_On()
      {

           var ctx1 = TestAdmin.TestContext();
           var ctx2 = TestAdmin.TestContext();

           ctx1.ReadOnly = false;
           ctx2.ReadOnly = false;
           ConcurrencyCheck<Business>(ctx1, ctx1.Businesses, ctx2, ctx2.Businesses);

           ctx1 = TestAdmin.TestContext();
           ctx2 = TestAdmin.TestContext();
           ctx1.ReadOnly = false;
           ctx2.ReadOnly = false;
           ConcurrencyCheck<User>(ctx1, ctx1.Users, ctx2, ctx2.Users);

      }
      private void ConcurrencyCheck<T>(DPOContext user1, DbSet<T> set1, DPOContext user2, DbSet<T> set2) where T : class
      {
         try
         {
            user1.IgnoreTimestampChecking = true;
            user2.IgnoreTimestampChecking = true;

            var entity = set1.First() as IConcurrency;

            //The other user comes and performs an update
            var entity2 = set2.First() as IConcurrency;
            entity2.Timestamp = DateTime.Now;
            user2.SaveChanges();

            entity.Timestamp = DateTime.Now;
            user1.SaveChanges();

            Assert.Fail("Failed concurrency check for" + typeof(T).ToString());
         }
         catch (DbUpdateConcurrencyException e)
         {
            var ex = e;
         }

      }


      /// <summary>
      // If this fails its because DPOContextExt has changed
      /// </summary>
      [TestMethod]
      public void TestsDataContext_Entities_That_Have_A_Timestamp_If_Modified_Should_Have_Timestamp_Updated()
      {
         var entity = this.TContext.Users.First() as IConcurrency;

         this.TContext.Entry(entity).State = EntityState.Modified;

         var saveTimestamp = entity.Timestamp;

         this.TContext.SaveChanges();

         Assert.AreNotEqual(entity.Timestamp, saveTimestamp);
      }


      /// <summary>
      // If this fails its because DPOContextExt has changed
      /// </summary>
      [TestMethod]
      public void TestsDataContext_Timestamps_Of_Entities_Cannot_Be_Updated_Manually()
      {
         try
         {
            var entity = this.TContext.Users.First() as IConcurrency;

            entity.Timestamp = DateTime.Now;
            this.TContext.SaveChanges();

            Assert.Fail("Failed to prevent timestamp update. Check DPOContextExt.cs->SaveChanges");
         }
         catch (Exception e)
         {
            var ex = e;
         }
      }


      /// <summary>
      // If this fails its because DPOContextExt has changed
      /// </summary>
      [TestMethod]
      public void TestsDataContext_Entities_That_Have_A_Timestamp_If_Added_Should_Have_Timestamp_Updated()
      {
          var gRP = db.Groups.FirstOrDefault();
          var bus = db.Businesses.FirstOrDefault();

          var entity = new SystemTestDataServices().GetOrCreateTestUser(this.TContext, gRP, bus, "Added", UserTypeEnum.DaikinAdmin, true,false);

         this.TContext.Users.Add(entity);

         this.TContext.SaveChanges();

         Assert.AreNotEqual(entity.Timestamp, DateTime.MinValue);
      }



      [TestMethod]
      public void TestsDataContext_Group_TotalChildren_Count_Check()
      {
          

          this.TContext.Groups.Add(new Group { GroupId = 1, ParentGroupId = null });
          this.TContext.Groups.Add(new Group { GroupId = 11, ParentGroupId = 1 });
          this.TContext.Groups.Add(new Group { GroupId = 12, ParentGroupId = 1 });
          this.TContext.Groups.Add(new Group { GroupId = 121, ParentGroupId = 12 });
          this.TContext.Groups.Add(new Group { GroupId = 1211, ParentGroupId = 121 });
          this.TContext.Groups.Add(new Group { GroupId = 1212, ParentGroupId = 121 });
          this.TContext.Groups.Add(new Group { GroupId = 12121, ParentGroupId = 1212 });
          this.TContext.Groups.Add(new Group { GroupId = 12122, ParentGroupId = 1212 });
          this.TContext.Groups.Add(new Group { GroupId = 122, ParentGroupId = 12 });
          this.TContext.Groups.Add(new Group { GroupId = 1221, ParentGroupId = 122 });
          this.TContext.Groups.Add(new Group { GroupId = 2, ParentGroupId = null });
          this.TContext.Groups.Add(new Group { GroupId = 21, ParentGroupId = 2 });

          this.db.UpdateGroupInformation();

          var groups = this.TContext.Groups.Local.ToList();
          var grp1 = groups.Where(g => g.GroupId == 1).FirstOrDefault();
          var grp1212 = groups.Where(g => g.GroupId == 1212).FirstOrDefault();
          var grp2 = groups.Where(g => g.GroupId == 2).FirstOrDefault();

          Assert.IsTrue(grp1.ChildrenCountDeep == 9);
          Assert.IsTrue(grp1212.ChildrenCountDeep == 2);
          Assert.IsTrue(grp2.ChildrenCountDeep == 1);

          Assert.IsTrue(grp1.ParentGroup == null);
          Assert.IsTrue(grp1212.ParentGroup.GroupId == grp1212.ParentGroupId);

      }


   }
}