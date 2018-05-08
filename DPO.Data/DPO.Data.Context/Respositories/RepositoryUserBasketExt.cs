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
using System.Reflection;


namespace DPO.Data
{

   public partial class Repository 
   {

       public IQueryable<UserBasketItem> UserBasketItems
      {
          get { return this.GetDbSet<UserBasketItem>(); }
      }

      public List<UserBasketItem> UserBasketItemsByUser(UserSessionModel user)
      {
         var basketRecord = this.UserBasketItems.Where(u => u.UserId == user.UserId).ToList();

         return basketRecord;
      }

      public void BasketItemRemove(UserSessionModel user, BasketItemModel item)
      {
          var entity = this.UserBasketItems.Where(u => u.UserId == user.UserId && u.ItemId == item.ItemId).FirstOrDefault();

          if (entity != null)
          {
              this.Context.UserBasketItems.Remove(entity);
          }

          return;
      }

      public UserBasketItem BasketItemCreate(long userId,long itemId,string description, decimal quantity)
      {
          var entity = this.Context.UserBasketItems.Create();

          entity.BasketItemId = this.Context.GenerateNextLongId();

          entity.UserId = userId;

          entity.Description = description;

          entity.ItemId = itemId;

          entity.Quantity = quantity;

          this.Context.UserBasketItems.Add(entity);

          return entity;
      }



   }
}