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
   public partial class TestBasketServices : TestAdmin
   {
      UserSessionModel model = new UserSessionModel();


      BasketServices service;

      public TestBasketServices()
      {
         service = new BasketServices(this.TContext);
      }

      [TestMethod]
      public void TestBasketServices_Basket_IsCreated_If_None_Exists_For_User()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var basket = service.GetUserBasketModel(sa).Model as UserBasketModel;

          Assert.IsNotNull(basket);

      }

      [TestMethod]
      public void TestBasketServices_Basket_IsCreated_different_Per_User()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var rm1 = GetUserSessionModel("USAM4@Somewhere.com");

          var basketsa = service.GetUserBasketModel(sa).Model as UserBasketModel;

          var basketrm1 = service.GetUserBasketModel(rm1).Model as UserBasketModel;

          Assert.AreNotEqual(basketsa, basketrm1);

      }


      //[TestMethod]
      //public void TestBasketServices_Add_Item_To_Basket()
      //{
      //    var sa = GetUserSessionModel("USSA0@Somewhere.com");

      //    var prod = this.TContext.Products.FirstOrDefault();

      //    var basket = service.GetUserBasketModel(sa).Model as UserBasketModel;

      //    Assert.AreEqual(basket.Items.Count, 0);

      //    service.UpdateBasketItem(sa, new BasketItemModel { ItemId = prod.ProductId, Quantity = 23 });

      //    basket = service.GetUserBasketModel(sa).Model as UserBasketModel; ;

      //    Assert.AreEqual(basket.Items.Count, 1);

      //    Assert.AreEqual(basket.Items.FirstOrDefault().Quantity, 23);

      //    service.UpdateBasketItem(sa, new BasketItemModel { ItemId = prod.ProductId, Quantity = -10 });

      //    basket = service.GetUserBasketModel(sa).Model as UserBasketModel; ;

      //    Assert.AreEqual(basket.Items.Count, 1);

      //    Assert.AreEqual(basket.Items.FirstOrDefault().Quantity, 13);

      //}


      //[TestMethod]
      //public void TestBasketServices_Update_Item_From_Basket_Removes_Item_If_Quantity_Is_Zero()
      //{
      //    var sa = GetUserSessionModel("USSA0@Somewhere.com");

      //    var prod = this.TContext.Products.FirstOrDefault();

      //    var basket = service.GetUserBasketModel(sa).Model as UserBasketModel; ;

      //    Assert.AreEqual(basket.Items.Count, 0);

      //    service.UpdateBasketItem(sa, new BasketItemModel { ItemId = prod.ProductId, Quantity = 23 });

      //    basket = service.GetUserBasketModel(sa).Model as UserBasketModel; ;

      //    Assert.AreEqual(basket.Items.Count, 1);

      //    Assert.AreEqual(basket.Items.FirstOrDefault().Quantity, 23);

      //    service.UpdateBasketItem(sa, new BasketItemModel { ItemId = prod.ProductId, Quantity = -23 });

      //    basket = service.GetUserBasketModel(sa).Model as UserBasketModel; ;

      //    Assert.AreEqual(basket.Items.Count, 0);

      //}


      //[TestMethod]
      //public void TestBasketServices_Import_Basket_Into_Quote()
      //{
      //    var sa = GetUserSessionModel("USSA0@Somewhere.com");

      //    var activequote = db.Quotes.Where(q => q.Active == true).FirstOrDefault();

      //    var basket = service.GetUserBasketModel(sa).Model as UserBasketModel;

      //    Assert.AreEqual(basket.Items.Count, 0);

      //    Assert.IsNotNull(activequote);

      //    var preQuoteItems = db.QuoteItems.Where(q => q.QuoteId == activequote.QuoteId).ToList();

      //    service.UpdateBasketItem(sa, new BasketItemModel { ItemId = preQuoteItems[0].ProductId.Value, Quantity = 5 });

      //    var productId = preQuoteItems[0].ProductId;

      //    var qtyBefore = db.QuoteItems.Where(q => q.QuoteId == activequote.QuoteId && q.ProductId == productId).Select(q => q.Quantity).FirstOrDefault();

      //    new QuoteServices(TContext).ImportProductsFromBasket(sa, new QuoteModel { QuoteId = activequote.QuoteId });

      //    var postQuoteItems = db.QuoteItems.Where(q => q.QuoteId == activequote.QuoteId).ToList();

      //    Assert.AreEqual(postQuoteItems.Count, preQuoteItems.Count);

      //    var qtyAfter = db.QuoteItems.Where(q => q.QuoteId == activequote.QuoteId && q.ProductId == productId).Select(q => q.Quantity).FirstOrDefault();

      //    Assert.AreEqual(qtyBefore + 5, qtyAfter);
      //}



   }
}