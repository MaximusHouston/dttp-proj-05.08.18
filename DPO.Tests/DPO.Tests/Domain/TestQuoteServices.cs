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
using System.Configuration;

namespace DPO.Tests
{

   [TestClass]
   public partial class TestQuoteServices : TestAdmin
   {
      QuoteServices service;
      BasketServices serviceBasket;

      public TestQuoteServices()
      {
          service = new QuoteServices(this.TContext);
          serviceBasket = new BasketServices(this.TContext);
      }

       [TestMethod]
      public void TestQuoteServices_Make_sure_cannot_delete_active()
      {
           var sa = GetUserSessionModel("USSA0@Somewhere.com");

           var activequote = db.Quotes.Where(q=>q.Title == "Quote 2").FirstOrDefault();

           var response = service.Delete(sa, new QuoteModel { ProjectId = activequote.ProjectId, QuoteId = activequote.QuoteId },true);
 
           Assert.IsTrue(response.Messages.Items.Any(i=>i.Text == Resources.ResourceModelProject.MP007));

           activequote = db.Quotes.Where(q=>q.Title == "Quote 2").FirstOrDefault();

           Assert.IsNotNull(activequote);
           
      }
       [TestMethod]
      public void TestQuoteServices_Make_sure_can_delete_nonactive_with_items()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var nonactive = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          var response = service.Delete(sa, new QuoteModel { ProjectId = nonactive.ProjectId, QuoteId = nonactive.QuoteId },false);
           
          Assert.IsFalse(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP007));

          nonactive = db.Quotes.Where(q => q.Title == "Quote 1" && q.Deleted == true).FirstOrDefault();

          Assert.IsTrue(nonactive.Deleted);
      }
       [TestMethod]
      public void TestQuoteServices_make_quote_active_and_only_ever_one_active()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var inactivequote = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          Assert.IsFalse(inactivequote.Active);

          var response = service.SetActive(sa, new QuoteModel { ProjectId = inactivequote.ProjectId, QuoteId = inactivequote.QuoteId });

          inactivequote = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          Assert.IsTrue(inactivequote.Active);

          Assert.AreEqual(db.Quotes.Where(q => q.ProjectId == inactivequote.ProjectId && q.Active == true).Count(), 1);
      }
       [TestMethod]
      public void TestQuoteServices_add_new_quote()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var activequote = db.Quotes.Where(q => q.Title == "Quote 2").FirstOrDefault();

          var response = service.GetQuoteModel(sa, activequote);

          var quotemodel = response.Model as QuoteModel;

          quotemodel.Title = "New title";
          quotemodel.Description = "New desc";
          quotemodel.IsCommissionScheme = false;
          quotemodel.Multiplier = 11;
          quotemodel.TotalFreight = 22;
          quotemodel.Notes = "New notes";
          quotemodel.IsGrossMargin = true;
          quotemodel.CommissionPercentage = 123;

          response = service.PostModel(sa, quotemodel);

          var newquote = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId  && q.Title == "New title").FirstOrDefault();

          Assert.IsNotNull(newquote);

          Assert.AreEqual(newquote.Title , quotemodel.Title);
          Assert.AreEqual(newquote.Description, quotemodel.Description);
          Assert.AreEqual(newquote.TotalFreight, quotemodel.TotalFreight);
          Assert.AreEqual(newquote.Notes, quotemodel.Notes);
          Assert.AreEqual(newquote.IsGrossMargin, quotemodel.IsGrossMargin);
          Assert.AreEqual(newquote.CommissionPercentage*100, quotemodel.CommissionPercentage);
      }
       [TestMethod]
      public void TestQuoteServices_add_new_with_latest_version_number()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var activequote = db.Quotes.Where(q => q.Title == "Quote 2").FirstOrDefault();

          var lastestversion = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId).Max(q => q.Revision);

          var response = service.GetQuoteModel(sa, activequote);

          var quotemodel = response.Model as QuoteModel;

          quotemodel.Title = "New title";
          quotemodel.Description = "New desc";
          quotemodel.Multiplier = 11;
          quotemodel.TotalFreight = 22;
          quotemodel.Notes = "New notes";
          quotemodel.QuoteId = null;

          response = service.PostModel(sa, quotemodel);


          var newquote = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId && q.Title == quotemodel.Title).FirstOrDefault();

          Assert.AreEqual(newquote.Revision, lastestversion + 1);

      }
       [TestMethod]
      public void TestQuoteServices_edit_quote()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var quotemodel = GetActiveQuoteModel(sa, "Project 4"); // Dealer

          quotemodel.Title = "edit title";
          quotemodel.Description = "edit desc";
          quotemodel.Multiplier = 33;
          quotemodel.TotalFreight = 44;
          quotemodel.Notes = "edit notes";

          var response = service.PostModel(sa, quotemodel);

          var editquote = db.Quotes.Where(q => q.ProjectId == quotemodel.ProjectId && q.Title == quotemodel.Title).FirstOrDefault();

          Assert.IsNotNull(editquote);

          Assert.AreEqual(editquote.Title, quotemodel.Title);
          Assert.AreEqual(editquote.Description, quotemodel.Description);
          Assert.AreEqual(editquote.DiscountPercentage, quotemodel.DiscountPercentage);
          Assert.AreEqual(editquote.TotalFreight, quotemodel.TotalFreight);
          Assert.AreEqual(editquote.Notes, quotemodel.Notes);
      }


       [TestMethod]
       public void TestQuoteServices_Rules_Commission_Based_Only_If_Business_Allows()
       {
           var dealer = GetUserSessionModel("US1@Somewhere.com");

           var dealerQuote = GetActiveQuoteModel(dealer, "Project 1");

           dealerQuote.IsCommissionScheme = true;

           var response = service.PostModel(dealer, dealerQuote);

           Assert.IsFalse(response.IsOK);

           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP014));

           // distributor check

           var distributor = GetUserSessionModel("US4@Somewhere.com");

           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");

           distributorQuote.IsCommissionScheme = true;

           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);

           Assert.IsFalse(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP014));

           distributorQuote.IsCommissionScheme = false;

           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       [TestMethod]
       public void TestQuoteServices_Rules_Commission_Negotiation_Multipliers_Allowed_Only_After_A_Specified_MinTotalList()
       {
           TestQuoteServices_Rules_Commission_Policy_Checker("negotiation");
       }

       [TestMethod]
       public void TestQuoteServices_Rules_Commission_Competitive_Multipliers_Allowed_Only_After_A_Specified_ListPrice()
       {
           TestQuoteServices_Rules_Commission_Policy_Checker("competitive");
       }

       private void TestQuoteServices_Rules_Commission_Policy_Checker(string range)
       {

           var totalListThreshold = decimal.Parse(Utilities.Config("dpo.sales.commission." + range + ".totallist.threshold"));
           var startMultiplier = decimal.Parse(Utilities.Config("dpo.sales.commission." + range + ".multiplier"));

           var distributor = GetUserSessionModel("US4@Somewhere.com");


           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");
           distributorQuote.Multiplier = startMultiplier;
           distributorQuote.IsCommissionScheme = true; //make a change

           // Change products to commissionable and update prices
           var quoteitems = this.db.Context.QuoteItems.Where(q => q.QuoteId == distributorQuote.QuoteId).Include(q=>q.Product).ToList();
           quoteitems.ForEach(q=>{q.Product.AllowCommissionScheme = true; q.Product.ListPrice = 1;});
           this.db.SaveChanges();

           var response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == string.Format(Resources.ResourceModelProject.MP016, totalListThreshold)));

           var quoteitem = this.db.Context.QuoteItems.Where(q => q.QuoteId == distributorQuote.QuoteId).Include(q=>q.Product).FirstOrDefault();
           quoteitem.Product.ListPrice = totalListThreshold + 1;
           this.db.Context.SaveChanges();

           response.Messages.Clear();
           distributorQuote = GetActiveQuoteModel(distributor, "Project 4");
           distributorQuote.IsCommissionScheme = true; //make a change
           distributorQuote.Multiplier = startMultiplier;
           response = service.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       // currently 50000
       public void TestQuoteServices_Rules_Commission_Policy_Mandatory_At_Predefined_TotalList_Threshold()
       {
           var threshold = decimal.Parse(Utilities.Config("dpo.sales.commission.buysell.total.threshold"));

           var distributor = GetUserSessionModel("US4@Somewhere.com");

           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");

           var quote = this.db.Context.Quotes.Where(q => q.QuoteId == distributorQuote.QuoteId).FirstOrDefault();
           quote.IsCommissionScheme = false;
           quote.TotalListCommission = threshold;
           this.db.SaveChanges();

           var response = service.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == string.Format(Resources.ResourceModelProject.MP018, threshold)));

           quote.TotalListCommission = threshold - 1;
           this.db.SaveChanges();

           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       public void TestQuoteServices_Rules_Commission_Policy_Multiplier_Within_Permitted_Range()
       {
           var startCMultiplier = decimal.Parse(Utilities.Config("dpo.sales.commission.competitive.multiplier"));

           var distributor = GetUserSessionModel("US4@Somewhere.com");
           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");

           var quote = this.db.Context.Quotes.Where(q => q.QuoteId == distributorQuote.QuoteId).FirstOrDefault();
           quote.IsCommissionScheme = true;
           this.db.SaveChanges();

           // check 0
           distributorQuote.Multiplier = 0;
           var response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP018));

           // check large
           distributorQuote.Multiplier = 99999999;
           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP018));

           // check invlaid
           distributorQuote.Multiplier = startCMultiplier + 0.03342M;
           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP018));

           Assert.IsTrue(response.IsOK);
       }

       public void TestQuoteServices_Rules_BuySell_Quotes_Cant_Be_Switched_To_Commission_If_Non_Commission_Products_Exist_In_Quote()
       {
           var startCMultiplier = decimal.Parse(Utilities.Config("dpo.sales.commission.competitive.multiplier"));

           var distributor = GetUserSessionModel("US4@Somewhere.com");
           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");

           var quote = this.db.Context.Quotes.Where(q => q.QuoteId == distributorQuote.QuoteId).FirstOrDefault();
           quote.IsCommissionScheme = true;
           quote.TotalListNonCommission = 999;
           this.db.SaveChanges();

           // check 0
           var response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

           // check large
           quote.TotalListNonCommission = 0;
           this.db.SaveChanges();
           response.Messages.Clear();
           response = service.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       [TestMethod]
       public void TestQuoteServices_Rules_When_Project_Is_Closed_No_Edits_Allowed_Only_When_Open()
       {
           var distributor = GetUserSessionModel("US4@Somewhere.com");

           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");
           distributorQuote.Title = "make a change";

           var project = this.db.Context.Projects.Where(q => q.ProjectId == distributorQuote.ProjectId).FirstOrDefault();
           project.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedLost;
           this.db.SaveChanges();

           // check 0
           var response = service.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP020));

       }

       [TestMethod]
       public void TODO_TestQuoteServices_Rules_BuySell_Quotes_Recalculation_Occurs_With_Any_Edit_Apart_From_Status_Change()
       {
           Assert.Fail();
       }
       [TestMethod]
       public void TODO_TestQuoteServices_Calculations_Totals()
       {
           Assert.Fail();
       }
       [TestMethod]
       public void TODO_TestQuoteProductsServices_Rules_Adding_Various_Product_Types_Calculates_Correct_Quote_Totals()
       {
           Assert.Fail();
       }

       [TestMethod]
       public void TODO_TestQuoteServices_Calculations_BuySell_Commission_Based_Totals_Discount_MarkUp()
       {
           Assert.Fail();
       }
       [TestMethod]
       public void TODO_TestQuoteServices_Calculations_BuySell_Commission_Based_Totals_Discount_GrossMargin()
       {
           Assert.Fail();
       }

       [TestMethod]
       public void TODO_TestQuoteServices_Discount_remains_for_a_determined_number_of_days_from_approval_date()
       {
           Assert.Fail();
       }

       [TestMethod]
       public void TestQuoteServices_Import_Items_Cannot_Import_Product_Which_Is_Non_Commissionable_Into_CommissionableQuote()
       {
           var user = GetUserSessionModel("US4@Somewhere.com");

           var quote = GetActiveQuoteModel(user, "Project 4");
           quote.IsCommissionScheme = true; //make a change to recalculate totals
           quote.Multiplier = 0.6M;
           var response = service.PostModel(user, quote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

           // Switch all products in quote to commission
           var items = this.db.QuoteItemsByQuoteId(user, quote.QuoteId).Include(q=>q.Product).Include(q=>q.Quote).ToList();
           items.ForEach(i => i.Product.AllowCommissionScheme = true);
           items[0].Quote.IsCommissionScheme = true; //set quote to commissinable
           this.db.SaveChanges();

           response.Messages.Clear();
           quote = GetActiveQuoteModel(user, "Project 4");
           response = service.PostModel(user, quote);
           Assert.IsTrue(response.IsOK);

           //Fill basket
           var basket = serviceBasket.GetUserBasketModel(user).Model as UserBasketModel;
           var nonCommissionProduct = this.db.Products.Where(p => p.AllowCommissionScheme == false).FirstOrDefault();

           response = service.AddProductToQuote(user, quote, nonCommissionProduct.ProductId, 2);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

       }

   }
}