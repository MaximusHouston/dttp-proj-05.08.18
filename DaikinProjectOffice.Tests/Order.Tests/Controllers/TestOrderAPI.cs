using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using DPO.Domain;
using DPO.Common;
using DPO.Model.Light;
using DPO.Services.Light;
using Resources = DPO.Resources;
using DPO.Data;
using DPO.Web.Controllers;
using System.Web;
using System.Web.Http;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class TestOrderAPI : TestAdmin
    {
        OrderServices orderService = null;
        OrderViewModelLight orderVMLight = null;
        ServiceResponse Response = null;
        OrderServiceLight orderServiceLight = null;
        UserSessionModel user = null;
        AccountServices accountService = null;
        QuoteServices quoteService = null;
        QuoteModel quoteModel = null;
        SearchOrders searchOrders = null;

        OrderController orderController = null;

        List<Project> projects = new List<Project>();
        List<QuoteModel> quotesModelWithoutOrder = new List<QuoteModel>();
        List<QuoteModel> quotesModelWithOrder = new List<QuoteModel>();

        public TestOrderAPI() 
        {
            orderService = new OrderServices(this.TContext);
            orderServiceLight = new OrderServiceLight();
            searchOrders = new SearchOrders();

            accountService = new AccountServices();
            user = accountService.GetUserSessionModel("User15@test.com").Model as UserSessionModel;
            quoteService = new QuoteServices();

            orderVMLight = new OrderViewModelLight();
            orderVMLight.BusinessId = 206680765352640513;
            orderVMLight.ShipToAddressId = 479102086194151432;
            orderVMLight.PricingTypeId = 1;
            orderVMLight.PONumber = "AAPO0613201601";
            orderVMLight.TotalDiscountPercent = 0;
            orderVMLight.EstimatedReleaseDate = Convert.ToDateTime("2016-06-06 00:00:00.000");
            orderVMLight.DeliveryAppointmentRequired = false;
            orderVMLight.DeliveryContactName = "";
            orderVMLight.DeliveryContactPhone = "";
            orderVMLight.OrderReleaseDate = DateTime.Now.AddDays(2);
            orderVMLight.SubmittedByUserId = 392643416367824896;
            orderVMLight.SubmitDate = DateTime.Now;
            orderVMLight.CreatedByUserId = 392643416367824896;
            orderVMLight.UpdatedByUserId = 392643416367824896;
            orderVMLight.DiscountRequestId = 0;
            orderVMLight.CommissionRequestId = 0;
            orderVMLight.ERPOrderNumber = "";
            orderVMLight.ERPPOKey = 0;
            orderVMLight.ERPStatus = "Submitted";
            orderVMLight.Comments = "PLEASE CALL BEFORE SHIPPING";
            orderVMLight.ERPComments = "";
            orderVMLight.ERPOrderDate = null;
            orderVMLight.ERPInvoiceDate = null;
            orderVMLight.ERPShipDate = null;
            orderVMLight.ERPInvoiceNumber = null;
            orderVMLight.QuoteId = 479102111284477952;
            orderVMLight.ProjectId = 479102086194151424;
            orderVMLight.CurrentUser = user;
            var fileUpload = new FileUploadTest();
            orderVMLight.POAttachment = fileUpload;
            orderVMLight.POAttachmentFileName = fileUpload.FileUploadName;
            orderVMLight.ERPAccountId = "1234";

            orderController = new OrderController();

            projects = this.db.Projects.Where(p => p.OwnerId == user.UserId).ToList();
          
            foreach(var project in projects)
            {
                var result = this.db.Context.Quotes.Where(q => q.AwaitingOrder == null && q.ProjectId == project.ProjectId).OrderByDescending(q => q.QuoteId).Select(q => new { q.QuoteId, q.ProjectId }).FirstOrDefault();
                if(result != null)
                {
                    quoteModel = quoteService.GetQuoteModel(user, result.ProjectId, result.QuoteId).Model as QuoteModel;
                    if (quoteModel != null)
                        quotesModelWithoutOrder.Add(quoteModel);
                }
            }

            foreach (var project in projects)
            {
                var result = this.db.Context.Quotes.Where(q => q.AwaitingOrder == true && q.ProjectId == project.ProjectId).OrderByDescending(q => q.QuoteId).Select(q => new { q.QuoteId, q.ProjectId }).FirstOrDefault();
                if (result != null)
                {
                    quoteModel = quoteService.GetQuoteModel(user, result.ProjectId, result.QuoteId).Model as QuoteModel;
                    if (quoteModel != null)
                        quotesModelWithOrder.Add(quoteModel);
                }
            }

        }

        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetNewOrder_ReturnNewOrderViewModelLight()
        {
            Response = orderServiceLight.GetNewOrder(user, quoteModel.QuoteId.Value);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetSubmittedOrder_ReturnOrderHasSubmitted()
        {
            quoteModel = quotesModelWithOrder.First();
            Response = orderServiceLight.GetSubmittedOrder(user, quoteModel.QuoteId.Value);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetOrdersForGrid_ShouldReturnOrderGridViewModel()
        {
            Response = orderService.GetOrdersForGrid(user, searchOrders);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }
        
        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetOrderInQuote_ShouldReturnOrderBelongToQuote()
        {
            quoteModel = quotesModelWithOrder.Last();
            Response = orderServiceLight.GetOrderInQuote(user, quoteModel.QuoteId.Value);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetOrderStatusTypes_ShouldReturnOrderStatusType()
        {
            Response = orderServiceLight.GetOrderStatusTypes(user);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_GetOrderOptions_ShouldReturnOrderOptions()
        {
            quoteModel = quotesModelWithOrder.Last();
            Response = orderServiceLight.GetOrderOptions(user, quoteModel.ProjectId, quoteModel.QuoteId);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderAPI_POST")]
        public void TestOrderAPI_PostOrder_ShouldReturnSubmitOrder()
        {
            Response = orderService.PostModel(user, orderVMLight);
            Assert.That(Response.IsOK, Is.EqualTo(true));
        }

        [Test]
        [Category("OrderAPI_POST")]
        public void TestOrderAPI_ApproveOrder_ShouldReturnOrderStatusEqualToApprove()
        {
            Response = orderService.Approve(user, orderVMLight);
            OrderViewModelLight result = Response.Model as OrderViewModelLight;
            Assert.That(result.OrderStatusTypeId, Is.EqualTo(4));
        }
        
        [Test]
        [Category("OrderAPI_POST")]
        public void TestOrderAPI_RejectOrder_ShouldReturnOrderStatusEqualsToReject()
        {
            Response = orderService.Reject(user, orderVMLight);
            OrderViewModelLight result = Response.Model as OrderViewModelLight;
            Assert.That(result.OrderStatusTypeId, Is.EqualTo(8));
        }

        [Test]

        public void TestOrderAPI_UpdateOrderStatus_ShouldChangeOrderStatusPerRequested()
        {
            Response = orderService.ChangeOrderStatus(user, orderVMLight, (OrderStatusTypeEnum)orderVMLight.OrderStatusTypeId);
            Assert.That(Response.IsOK, Is.EqualTo(true));
        }

        [Test]
        [TestCase("20051700")]
        [TestCase("1234")]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_CheckAccountOnMapics_ShouldReturnValidationBasedOnERPAccountId(string ERPAccountId)
        {
            Response = orderController.CheckAccountOnMapics(ERPAccountId);

            if (ERPAccountId == "20051700")
            Assert.That(Response.IsOK, Is.EqualTo(true));

            if (ERPAccountId == "1234")
            Assert.That(Response.Messages.Items.Any(), Is.EqualTo("ERPAccount is invalid"));

            //TODO: need to test for ERP SUSPEND Account
            //need to add code in future.
        }

        [Test]
        [TestCase("20051700", "AA-PO0617201602")]
        [TestCase("20051700","Test-PONumber")]
        [Category("OrderAPI_GET")]
        public void TestOrderAPI_CheckPONumberMapics_ShouldReturnValidationBasedOnPoNumber(string ERPAccountId, string PONumber)
        {
            Response = orderController.CheckPONumberMapics(ERPAccountId, PONumber);

            if(ERPAccountId == "20051700" && PONumber == "AA-PO0617201602")
            Assert.That(Response.Messages.Items.Any(t => t.Text == "PONumber already exists."), Is.EqualTo(true));
            
            if(ERPAccountId == "20051700" && PONumber == "Test-PONumber")
            Assert.That(Response.IsOK, Is.EqualTo(true));
        }
    }
}
