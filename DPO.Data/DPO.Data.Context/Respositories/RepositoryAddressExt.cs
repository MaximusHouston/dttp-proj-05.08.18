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

      public IQueryable<Address> Addresses
      {
         get { return this.GetDbSet<Address>(); }
      }

      public IQueryable<Country> Countries
      {
         get { return this.GetDbSet<Country>(); }
      }

      public IQueryable<State> States
      {
          get { return this.GetDbSet<State>(); }
      }


      public Address AddressCreate()
      {
         var entity = new Address();

         entity.AddressId = this.Context.GenerateNextLongId();

         this.Context.Addresses.Add(entity);

         return entity;
      }

      public Address GetAddressByAddressId(long? id)
      {
          return this.Addresses.Where(u => u.AddressId == id)
                .Include(b => b.State)
                .Include("State.Country")
                .FirstOrDefault();
      }

      public IQueryable<Address> GetAddressQueryByAddressId(long? id)
      {
          return this.Addresses.Where(u => u.AddressId == id);
      }


      public Country CountryCreate(string CountryCode,string name,string ISO3,string code, int phone )
      {
         var entity = new Country
            {
               Name = name,
               CountryCode = CountryCode,
               ISO3 = ISO3,
               PhoneCode = phone

            };

         this.Context.Countries.Add(entity);

         return entity;
      }

      public State StateCreate(string name, string code, Country country)
      {
          var entity = new State
         {
            Country = country,
            Name = name,
            Code = code
         };

         this.Context.States.Add(entity);

         return entity;
      }

   }
}