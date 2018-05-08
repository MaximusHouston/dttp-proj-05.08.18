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

   public partial class Repository 
   {

      public IQueryable<Contact> Contacts
      {
         get { return this.GetDbSet<Contact>(); }
      }

      public Contact ContactCreate()
      {
          var entity = new Contact();

         entity.ContactId = this.Context.GenerateNextLongId();

         this.Context.Contacts.Add(entity);

         return entity;
      }

      public IQueryable<Contact> GetContactByContactId(long? id)
      {
          return this.Contacts.Where(u => u.ContactId == id);
      }

      public IQueryable<Contact> GetContactQueryByContactId(long? id)
      {
          return this.Contacts.Where(u => u.ContactId == id);
      }
   }
}