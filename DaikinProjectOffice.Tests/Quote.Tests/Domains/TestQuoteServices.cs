
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using System.Configuration;
using NUnit.Framework;
using NUnit.Common;
using Resources = DPO.Resources; 

namespace DaikinProjectOffice.Tests
{

   [TestFixture]
   public partial class TestQuoteServices : TestAdmin
   {
      QuoteServices quoteService;
      BasketServices serviceBasket;

        UserSessionModel user = new UserSessionModel();

        ProjectServices projectService;
        SystemTestDataServices systemService;
        BusinessServices businessService;

        ServiceResponse Response = new ServiceResponse();
        ProjectModel projectModel = new ProjectModel();

        long _projectId;
        long _quoteId;
        int _quoteCount = 0;


        public TestQuoteServices()
        {
          
            systemService = new SystemTestDataServices(this.TContext);
            projectService = new ProjectServices(this.TContext);
            businessService = new BusinessServices(this.TContext);
            quoteService = new QuoteServices(this.TContext);

            user = GetUserSessionModel("User15@test.com");

            _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            projectModel = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            _quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == _projectId)
                           .OrderByDescending(q => q.QuoteId)
                           .Select(q => q.QuoteId).FirstOrDefault();

            _quoteCount = this.db.Context.Quotes.Where(q => q.ProjectId == _projectId)
                           .OrderByDescending(q => q.QuoteId)
                           .Count();
        }

        /// <summary>
        /// Test the GetQuotelistModel function on QuoteService Domain
        /// make sure it return the list of QuoteListModels
        /// </summary>
        [Test]
        [Category("QuoteService_GET")]
        [TestCase("quoteId")]
        [TestCase("projectId")]
        public void TestQuoteServices_GetQuoteListModel(string testValue)
        {
            if (testValue == "quoteId")
            {
                SearchQuote searchQuote = new SearchQuote();
                searchQuote.QuoteId = _quoteId;

                this.Response = quoteService.GetQuoteListModel(user, searchQuote);
                Assert.That(Response.HasError, Is.EqualTo(false));

                List<QuoteListModel> quoteListModels = Response.Model as List<QuoteListModel>;
                Assert.That(quoteListModels.Count, Is.EqualTo(1));
                Assert.That(quoteListModels.FirstOrDefault().Title, Is.Not.EqualTo(string.Empty));
            }
            else
            {
                SearchQuote searchQuote = new SearchQuote();
                searchQuote.ProjectId = _projectId;

                this.Response = quoteService.GetQuoteListModel(user, searchQuote);
                Assert.That(Response.HasError, Is.EqualTo(false));

                List<QuoteListModel> quoteListModels = Response.Model as List<QuoteListModel>;
                Assert.That(quoteListModels.Count, Is.GreaterThanOrEqualTo(1));
                Assert.That(_quoteCount, Is.EqualTo(quoteListModels.Count));
                Assert.That(quoteListModels.FirstOrDefault().Title, Is.Not.EqualTo(string.Empty));

            }
        }

        /// <summary>
        /// </summary>
        /// <param name="testValue">
        /// We need to test these cases: 
        ///  one is it return existed QuoteModel 
        ///  other is it return new QuoteModel
        /// </param>
        [Test]
        [Category("QuoteService_GET")]
        [TestCase("GetQuoteModel")]
        [TestCase("CreateQuoteModel")]
        public void TestQuoteServices_GetQuoteModel_ShouldReturnQuoteModel(string testValue)
        {
            if(testValue == "GetQuoteModel")
            {
                //make sure it return existed QuoteModel based on ProjectId and Quoteid
                this.Response = quoteService.GetQuoteModel(user, _projectId, _quoteId);
                Assert.That(this.Response.HasError, Is.EqualTo(false));
                QuoteModel model = this.Response.Model as QuoteModel;
                Assert.That(model, Is.Not.EqualTo(null));
                Assert.That(model.Title, Is.Not.EqualTo(string.Empty));
                Assert.That(model.ProjectId, Is.EqualTo(_projectId));
                Assert.That(model.QuoteId, Is.EqualTo(_quoteId));
            }
            else if( testValue == "CreateQuoteModel")
            {
                //make sure if return new QuoteModel when the quoteId is null or not existed
                this.Response = quoteService.GetQuoteModel(user, _projectId, null);
                Assert.That(this.Response.HasError, Is.EqualTo(false));
                QuoteModel model = this.Response.Model as QuoteModel;
                Assert.That(model.QuoteId, Is.EqualTo(null));
                Assert.That(model.ProjectId, Is.EqualTo(_projectId));
                Assert.That(model.Title, Is.EqualTo(null));
            }   
        }

        [Test]
        [Category("QuoteService_GET")]
        public void TestQuoteServices_GetQuoteItemListModel_ShouldReturnListOfQuoteItemListModel()
        {
            this.Response = quoteService.GetQuoteItemListModel(user, _quoteId);
            List<QuoteItemListModel> model = this.Response.Model as List<QuoteItemListModel>;
            Assert.That(this.Response.HasError, Is.EqualTo(false));
            Assert.That(model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("QuoteService_GET")]
        [TestCase(206218813971103744)]
        public void TestQuoteServices_GetSingleQuoteItemListModel(long quoteItemId)
        {
            QuoteItemListModel model = quoteService.GetSingleQuoteItemListModel(quoteItemId);
            Assert.That(model, Is.Not.EqualTo(null));
            Assert.That(model.QuoteId, Is.EqualTo(206218687261179904));
            Assert.That(model.ProductId, Is.EqualTo(206023024573415804));
            Assert.That(model.ProductNumber, Is.EqualTo("REYQ72PBYD"));
        }

        [Test]
        [Category("QuoteService_GET")]
        [TestCase("HasQuoteItemId")]
        [TestCase("NoQuoteItemId")]
        public void TestQuoteServices_GetQuoteItemModel(string testValue)
        {
            if (testValue == "HasQuoteItemId")
            {
                //get the QuoteItemId 
                var quoteItemId = this.db.Context.QuoteItems.Where(qi => qi.QuoteId == _quoteId)
                                      .OrderByDescending(qi => qi.QuoteItemId)
                                      .Select(qi => qi.QuoteItemId)
                                      .FirstOrDefault();

                this.Response = quoteService.GetQuoteItemModel(user, _quoteId, quoteItemId);
                Assert.That(this.Response.HasError, Is.EqualTo(false));

                QuoteItemModel model = this.Response.Model as QuoteItemModel;
                Assert.That(model, Is.Not.EqualTo(null));
                Assert.That(model.QuoteItemId, Is.EqualTo(_quoteId));
                Assert.That(model.QuoteId, Is.EqualTo(_quoteId));

                //make sure the model has Quote 
                Assert.That(model.Quote, Is.Not.EqualTo(null));
            }
            else
            {
                this.Response = quoteService.GetQuoteItemModel(user, _quoteId, 123456);
                Assert.That(this.Response.HasError, Is.EqualTo(true));
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.DataMessages.DM023)), Is.EqualTo(true));
            }
        }

        [Test]
        [Category("QuoteService_GET")]
        public void TestQuoteServices_GetQuoteItemsModel()
        {
            QuoteItemsModel model = new QuoteItemsModel();
            model.QuoteId = _quoteId;
            this.Response = quoteService.GetQuoteItemsModel(user, model);
            Assert.That(this.Response.HasError, Is.EqualTo(false));
            model = this.Response.Model as QuoteItemsModel;
            Assert.That(model.QuoteId, Is.EqualTo(_quoteId));
            Assert.That(model.ProjectId, Is.EqualTo(_projectId));

            QuoteListModel activeQuote = model.ActiveQuoteSummary;
            Assert.That(activeQuote, Is.Not.EqualTo(null));
            Assert.That(activeQuote.ProjectId, Is.EqualTo(_projectId));
            Assert.That(activeQuote.QuoteId, Is.EqualTo(_quoteId));

            //make sure has the <QuoteItemListModel
            Assert.That(model.Items, Is.Not.EqualTo(null));
            //make sure it load discount request Model
            Assert.That(model.DiscountRequests, Is.Not.EqualTo(null));
            //maker sure it load commission reuqets
            Assert.That(model.CommissionRequests, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("QuoteService_GET")]
        public void TestQuoteServices_GetQuoteItems()
        {
            this.Response = quoteService.GetQuoteItems(user, _quoteId);
            Assert.That(this.Response.HasError, Is.EqualTo(false));
            List<QuoteItemModel> models = this.Response.Model as List<QuoteItemModel>;
            Assert.That(models.Any(qim => qim.ProjectId == _projectId), Is.EqualTo(true));
            Assert.That(models.Any(qim => qim.QuoteId == _quoteId), Is.EqualTo(true));
            Assert.That(models.Any(qim => qim.Quantity > 0), Is.EqualTo(true));
        }

        [Test]
        [Category("QuoteService_GET")]
        public void TestQuoteServices_GetQuoteQuotePackage()
        {
                QuoteItemsModel model = new QuoteItemsModel();
                model.QuoteId = _quoteId;

                this.Response = quoteService.GetQuoteQuotePackage(user, model);
                Assert.That(this.Response.HasError, Is.EqualTo(false));

                model = this.Response.Model as QuoteItemsModel;
                QuoteItemListModel quoteItemListModel = model.Items.FirstOrDefault();
                Assert.That(quoteItemListModel, Is.Not.EqualTo(null));

                //make sure it has the QuotePackage
                Assert.That(model.QuotePackage, Is.Not.EqualTo(null));
                if (model.QuotePackage.Count > 0)
                {
                    Assert.That(model.QuotePackage.First().DocumentTypeId, Is.Not.EqualTo(null));
                }

                if (model.QuotePackageAttachedFiles.Count > 0)
                {
                    Assert.That(model.QuotePackageAttachedFiles, Is.Not.EqualTo(null));
                    Assert.That(model.QuotePackageAttachedFiles.First().FileName,
                              Is.EqualTo(model.QuotePackage.First().FileName));
                    Assert.That(model.QuotePackageAttachedFiles.First().Type.ToLower(),
                              Is.EqualTo("quotepackageattachedfile"));
                    Assert.That(model.QuotePackageAttachedFiles.First().Description, Is.Not.EqualTo(string.Empty));
                }            
        }

        [Test]
       [Category("QuoteService")]
      public void TestQuoteServices_Make_sure_cannot_delete_active()
      {
           var sa = GetUserSessionModel("USSA0@Somewhere.com");

           var activequote = db.Quotes.Where(q=>q.Title == "Quote 2").FirstOrDefault();

           var response = quoteService.Delete(sa, new QuoteModel { ProjectId = activequote.ProjectId, QuoteId = activequote.QuoteId },true);
 
           Assert.IsTrue(response.Messages.Items.Any(i=>i.Text ==  Resources.ResourceModelProject.MP007));

           activequote = db.Quotes.Where(q=>q.Title == "Quote 2").FirstOrDefault();

           Assert.IsNotNull(activequote);
         
      }
       [Test]
      public void TestQuoteServices_Make_sure_can_delete_nonactive_with_items()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var nonactive = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          var response = quoteService.Delete(sa, new QuoteModel { ProjectId = nonactive.ProjectId, QuoteId = nonactive.QuoteId },false);
           
          Assert.IsFalse(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP007));

          nonactive = db.Quotes.Where(q => q.Title == "Quote 1" && q.Deleted == true).FirstOrDefault();

          Assert.IsTrue(nonactive.Deleted);
      }
       [Test]
      public void TestQuoteServices_make_quote_active_and_only_ever_one_active()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var inactivequote = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          Assert.IsFalse(inactivequote.Active);

          var response = quoteService.SetActive(sa, new QuoteModel { ProjectId = inactivequote.ProjectId, QuoteId = inactivequote.QuoteId });

          inactivequote = db.Quotes.Where(q => q.Title == "Quote 1").FirstOrDefault();

          Assert.IsTrue(inactivequote.Active);

          Assert.AreEqual(db.Quotes.Where(q => q.ProjectId == inactivequote.ProjectId && q.Active == true).Count(), 1);
      }
       [Test]
      public void TestQuoteServices_add_new_quote()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var activequote = db.Quotes.Where(q => q.Title == "Quote 2").FirstOrDefault();

          var response = quoteService.GetQuoteModel(sa, activequote);

          var quotemodel = response.Model as QuoteModel;

          quotemodel.Title = "New title";
          quotemodel.Description = "New desc";
          quotemodel.IsCommissionScheme = false;
          quotemodel.Multiplier = 11;
          quotemodel.TotalFreight = 22;
          quotemodel.Notes = "New notes";
          quotemodel.IsGrossMargin = true;
          quotemodel.CommissionPercentage = 123;

          response = quoteService.PostModel(sa, quotemodel);

          var newquote = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId  && q.Title == "New title").FirstOrDefault();

          Assert.IsNotNull(newquote);

          Assert.AreEqual(newquote.Title , quotemodel.Title);
          Assert.AreEqual(newquote.Description, quotemodel.Description);
          Assert.AreEqual(newquote.TotalFreight, quotemodel.TotalFreight);
          Assert.AreEqual(newquote.Notes, quotemodel.Notes);
          Assert.AreEqual(newquote.IsGrossMargin, quotemodel.IsGrossMargin);
          Assert.AreEqual(newquote.CommissionPercentage*100, quotemodel.CommissionPercentage);
      }
       [Test]
      public void TestQuoteServices_add_new_with_latest_version_number()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var activequote = db.Quotes.Where(q => q.Title == "Quote 2").FirstOrDefault();

          var lastestversion = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId).Max(q => q.Revision);

          var response = quoteService.GetQuoteModel(sa, activequote);

          var quotemodel = response.Model as QuoteModel;

          quotemodel.Title = "New title";
          quotemodel.Description = "New desc";
          quotemodel.Multiplier = 11;
          quotemodel.TotalFreight = 22;
          quotemodel.Notes = "New notes";
          quotemodel.QuoteId = null;

          response = quoteService.PostModel(sa, quotemodel);


          var newquote = db.Quotes.Where(q => q.ProjectId == activequote.ProjectId && q.Title == quotemodel.Title).FirstOrDefault();

          Assert.AreEqual(newquote.Revision, lastestversion + 1);

      }
       [Test]
      public void TestQuoteServices_edit_quote()
      {
          var sa = GetUserSessionModel("USSA0@Somewhere.com");

          var quotemodel = GetActiveQuoteModel(sa, "Project 4"); // Dealer

          quotemodel.Title = "edit title";
          quotemodel.Description = "edit desc";
          quotemodel.Multiplier = 33;
          quotemodel.TotalFreight = 44;
          quotemodel.Notes = "edit notes";

          var response = quoteService.PostModel(sa, quotemodel);

          var editquote = db.Quotes.Where(q => q.ProjectId == quotemodel.ProjectId && q.Title == quotemodel.Title).FirstOrDefault();

          Assert.IsNotNull(editquote);

          Assert.AreEqual(editquote.Title, quotemodel.Title);
          Assert.AreEqual(editquote.Description, quotemodel.Description);
          Assert.AreEqual(editquote.DiscountPercentage, quotemodel.DiscountPercentage);
          Assert.AreEqual(editquote.TotalFreight, quotemodel.TotalFreight);
          Assert.AreEqual(editquote.Notes, quotemodel.Notes);
      }


       [Test]
       public void TestQuoteServices_Rules_Commission_Based_Only_If_Business_Allows()
       {
           var dealer = GetUserSessionModel("US1@Somewhere.com");

           var dealerQuote = GetActiveQuoteModel(dealer, "Project 1");

           dealerQuote.IsCommissionScheme = true;

           var response = quoteService.PostModel(dealer, dealerQuote);

           Assert.IsFalse(response.IsOK);

           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP014));

           // distributor check

           var distributor = GetUserSessionModel("US4@Somewhere.com");

           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");

           distributorQuote.IsCommissionScheme = true;

           response.Messages.Clear();
           response = quoteService.PostModel(distributor, distributorQuote);

           Assert.IsFalse(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP014));

           distributorQuote.IsCommissionScheme = false;

           response.Messages.Clear();
           response = quoteService.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       [Test]
       public void TestQuoteServices_Rules_Commission_Negotiation_Multipliers_Allowed_Only_After_A_Specified_MinTotalList()
       {
           TestQuoteServices_Rules_Commission_Policy_Checker("negotiation");
       }

       [Test]
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

           var response = quoteService.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == string.Format(Resources.ResourceModelProject.MP016, totalListThreshold)));

           var quoteitem = this.db.Context.QuoteItems.Where(q => q.QuoteId == distributorQuote.QuoteId).Include(q=>q.Product).FirstOrDefault();
           quoteitem.Product.ListPrice = totalListThreshold + 1;
           this.db.Context.SaveChanges();

           response.Messages.Clear();
           distributorQuote = GetActiveQuoteModel(distributor, "Project 4");
           distributorQuote.IsCommissionScheme = true; //make a change
           distributorQuote.Multiplier = startMultiplier;
           response = quoteService.PostModel(distributor, distributorQuote);

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

           var response = quoteService.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == string.Format(Resources.ResourceModelProject.MP018, threshold)));

           quote.TotalListCommission = threshold - 1;
           this.db.SaveChanges();

           response.Messages.Clear();
           response = quoteService.PostModel(distributor, distributorQuote);

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
           var response = quoteService.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP018));

           // check large
           distributorQuote.Multiplier = 99999999;
           response.Messages.Clear();
           response =  quoteService.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP018));

           // check invlaid
           distributorQuote.Multiplier = startCMultiplier + 0.03342M;
           response.Messages.Clear();
           response = quoteService.PostModel(distributor, distributorQuote);
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
           var response = quoteService.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

           // check large
           quote.TotalListNonCommission = 0;
           this.db.SaveChanges();
           response.Messages.Clear();
           response = quoteService.PostModel(distributor, distributorQuote);

           Assert.IsTrue(response.IsOK);
       }

       [Test]
       public void TestQuoteServices_Rules_When_Project_Is_Closed_No_Edits_Allowed_Only_When_Open()
       {
           var distributor = GetUserSessionModel("US4@Somewhere.com");

           var distributorQuote = GetActiveQuoteModel(distributor, "Project 4");
           distributorQuote.Title = "make a change";

           var project = this.db.Context.Projects.Where(q => q.ProjectId == distributorQuote.ProjectId).FirstOrDefault();
           project.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedLost;
           this.db.SaveChanges();

           // check 0
           var response = quoteService.PostModel(distributor, distributorQuote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP020));

       }

       [Test]
       public void TODO_TestQuoteServices_Rules_BuySell_Quotes_Recalculation_Occurs_With_Any_Edit_Apart_From_Status_Change()
       {
           Assert.Fail();
       }
       [Test]
       public void TODO_TestQuoteServices_Calculations_Totals()
       {
           Assert.Fail();
       }
       [Test]
       public void TODO_TestQuoteProductsServices_Rules_Adding_Various_Product_Types_Calculates_Correct_Quote_Totals()
       {
           Assert.Fail();
       }

       [Test]
       public void TODO_TestQuoteServices_Calculations_BuySell_Commission_Based_Totals_Discount_MarkUp()
       {
           Assert.Fail();
       }
       [Test]
       public void TODO_TestQuoteServices_Calculations_BuySell_Commission_Based_Totals_Discount_GrossMargin()
       {
           Assert.Fail();
       }

       [Test]
       public void TODO_TestQuoteServices_Discount_remains_for_a_determined_number_of_days_from_approval_date()
       {
           Assert.Fail();
       }

       [Test]
       public void TestQuoteServices_Import_Items_Cannot_Import_Product_Which_Is_Non_Commissionable_Into_CommissionableQuote()
       {
           var user = GetUserSessionModel("US4@Somewhere.com");

           var quote = GetActiveQuoteModel(user, "Project 4");
           quote.IsCommissionScheme = true; //make a change to recalculate totals
           quote.Multiplier = 0.6M;
           var response = quoteService.PostModel(user, quote);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

           // Switch all products in quote to commission
           var items = this.db.QuoteItemsByQuoteId(user, quote.QuoteId).Include(q=>q.Product).Include(q=>q.Quote).ToList();
           items.ForEach(i => i.Product.AllowCommissionScheme = true);
           items[0].Quote.IsCommissionScheme = true; //set quote to commissinable
           this.db.SaveChanges();

           response.Messages.Clear();
           quote = GetActiveQuoteModel(user, "Project 4");
           response = quoteService.PostModel(user, quote);
           Assert.IsTrue(response.IsOK);

           //Fill basket
           var basket = serviceBasket.GetUserBasketModel(user).Model as UserBasketModel;
           var nonCommissionProduct = this.db.Products.Where(p => p.AllowCommissionScheme == false).FirstOrDefault();

           response = quoteService.AddProductToQuote(user, quote, nonCommissionProduct.ProductId, 2);
           Assert.IsTrue(response.Messages.Items.Any(i => i.Text == Resources.ResourceModelProject.MP019));

       }
               
        

    }
}