//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;

using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;

namespace DPO.Data
{

   public partial class SeedFactory 
   {
      DPOContext context;
      Repository db = null;
      Repository Db { get { db = db ?? new Repository(context, false); return db; } }

      public void Seed(DPOContext context)
      {
         
         this.context = context;

          // Test
         SeedTestDataUserGroupings();
         SeedTestDataProjects();

      }



      public void CreateTestContact(DPOContext context,Contact contact, string name)
      {
         contact.ContactEmail = string.Format("{0}@somewhere.com", name);
         contact.Mobile = "+44 (0) 780 36545";
         contact.Phone = "+44 (0) 208 456 7895";
      }

      public void CreateTestAddress(DPOContext context,Address address, string name)
      {
        address.AddressLine1 = string.Format("{0} Line1", name);
        address.AddressLine2 = string.Format("{0} Line2", name);
        address.AddressLine3 = string.Format("{0} Line3", name);
        address.PostalCode = "SM7 2PG";
        address.RegionId = context.Regions.FirstOrDefault().RegionId;
        address.Location = "Banstead";
         
      }

      public Business CreateTestBusiness(DPOContext context, BusinessTypeEnum type, string name)
      {
          this.context = context;
        var bus = context.Businesses.Where(u => u.BusinessName == name).FirstOrDefault();

        if (bus == null)
        {
            bus = Db.BusinessCreate(type);
            bus.AccountId = name;
            bus.BusinessName = name;
            bus.Enabled = true;
            CreateTestAddress(context, bus.Address, name);
            CreateTestContact(context, bus.Contact, name);
         };

        return bus;
      }

 

   }
}