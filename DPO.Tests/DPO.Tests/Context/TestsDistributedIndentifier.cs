using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DPO.Data;
using FlakeGen;

namespace DPO.Tests
{
   [TestClass]
   public class TestsDistributedIndentifier
   {
      public DistributedIndentifier DI;


      public TestsDistributedIndentifier()
      {
         DI = new DistributedIndentifier();
      }

      [TestMethod]
      public void TestsDistributedIndentifier_VerifyUnique32BitIdsGeneratedInOrder()
      {
          int[] ids = new int[1000];
         
         for(int i = 0; i < 1000;i++)
         {
            ids[i] = DI.GenerateSequential32Bit();
         }

         Assert.IsTrue(TestAdmin.AreSorted(ids), "Ids array needs to be ordered");

         Assert.IsTrue(TestAdmin.AreUnique(ids), "Ids array are not unique");
      }

      [TestMethod]
      public void TestsDistributedIndentifier_VerifyUnique64BitIdsGeneratedInOrder()
      {
          long[] ids = new long[1000];

          for (int i = 0; i < 1000; i++)
          {
              ids[i] = DI.GenerateSequential64Bit();
          }

          Assert.IsTrue(TestAdmin.AreSorted(ids), "Ids array needs to be ordered");

          Assert.IsTrue(TestAdmin.AreUnique(ids), "Ids array are not unique");
      }


      [TestMethod]
      public void TestsDistributedIndentifier_VerifyUniqueGuidsIdsGeneratedInOrder()
      {
         Guid[] ids = new Guid[1000];

         for (int i = 0; i < 1000; i++)
         {
            ids[i] = DI.GenerateSequentialGuid();
         }

         Assert.IsTrue(TestAdmin.AreSorted(ids), "Guids needs to be ordered");

         Assert.IsTrue(TestAdmin.AreUnique(ids), "Guids are not unique");
      }

   }
}
