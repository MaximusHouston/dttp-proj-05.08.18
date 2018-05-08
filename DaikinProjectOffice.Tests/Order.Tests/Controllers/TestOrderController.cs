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
using System.Web.Mvc;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using System.IO;
using System.Web.Routing;
using Moq;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class TestOrderController : TestAdmin
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

        List<Project> projects = new List<Project>();
        List<QuoteModel> quotesModelWithoutOrder = new List<QuoteModel>();
        List<QuoteModel> quotesModelWithOrder = new List<QuoteModel>();
        
        OrderController orderControllerApi = null;
        ProjectDashboardController orderController = null;

        public TestOrderController()
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

            orderControllerApi = new OrderController();
            orderController = new DPO.Web.Controllers.ProjectDashboardController();
            orderController.CurrentUser = user;

            projects = this.db.Projects.Where(p => p.OwnerId == user.UserId).ToList();

        }

        [Test]
        [Category("OrderController_GET")]
        [TestCase("GET")]
        public void ShouldRenderOrdersView(string httpMethod)
        {
           var httpContextMock = FakeHttpContext(httpMethod);
           var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose); 

           var routeData = new RouteData();
           routeData.Values.Add("key1", "/ProjectDashboard/Orders");

           var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
           orderController.ControllerContext = controllerContext;

           var result = orderController.Orders()as ViewResult;
           Assert.That(result.ViewName, Is.EqualTo("Orders"));
        }

        [Test]
        [Category("OrderController_GET")]
        [TestCase("GET")]
        public void TestOrderController_OrderInQuote_ShouldRenderPartialViewOrderInQuote(string httpMethod)
        {
            QuoteItemsModel model = new QuoteItemsModel();
          
            var httpContextMock = FakeHttpContext(httpMethod);
            var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

            var routeData = new RouteData();
            routeData.Values.Add("key1", "/ProjectDashboard/Orders");
            
            var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
            orderController.ControllerContext = controllerContext;

            var result = orderController.OrderInQuote(model) as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("OrderInQuote"));
        }

        [Test]
        [Category("OrderController_GET")]
        public void TestOrderController_OrderInQuote_ShouldReturnQuoteItemsModel()
        {
            QuoteItemsModel model = new QuoteItemsModel();
            Response = quoteService.GetQuoteItemsModel(user, model);
            Assert.That(Response.Model, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("OrderController_GET")]
        [TestCase("GET")]
        public void TestOrderController_ConfirmEstDeliveryDate(string httpMethod)
        {
            var httpContextMock = FakeHttpContext(httpMethod);
            var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

            var routeData = new RouteData();
            routeData.Values.Add("key1", "/ProjectDashboard/Orders");

            var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
            orderController.ControllerContext = controllerContext;

            var result = orderController.ConfirmEstDeliveryDate(orderVMLight.ProjectId, orderVMLight.QuoteId) as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("ConfirmEstDeliveryDate"));
        }

        [Test]
        [Category("OrderController")]
        [TestCase("GET")]
        [Ignore("this not pass")]
        public void TestOrderController_SendEmailOrderSubmit_ShouldReturnNullIfSendEmailSuccessed(string httpMethod)
        {
            var httpContextMock = FakeHttpContext(httpMethod);
            var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "/ProjectDashboard/Orders");

            var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
            orderController.ControllerContext = controllerContext;

            var result = orderController.SendEmailOrderSubmit(orderVMLight) as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("ConfirmEstDeliveryDate"));
        }

        [Test]
        [Category("OrderController")]
        [TestCase(false)]
        [TestCase(true)]
        public void TestOrderController_OrderPrint_ShouldRenderTheOrderInPdf(bool ShowCostPricing)
        {
            var result = orderController.OrderPrint(orderVMLight.ProjectId,
                                                    orderVMLight.QuoteId,
                                                    orderVMLight.OrderId,
                                                    ShowCostPricing) as ViewResult;

            Assert.That(result.ViewName, Is.EqualTo("OrderPrint"));
        }

        [Test]
        [Category("OrderController")]
        public void TestOrderController_OrderPrintFooter()
        {
            var result = orderController.OrderPrintFooter() as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("OrderPrintFooter"));
        }

        [Test]
        [Category("OrderController")]
        public void TestOrderController_OrderPrintHeader()
        {
            var result = orderController.OrderPrintHeader() as ViewResult;
            Assert.That(result.ViewName, Is.EqualTo("OrderPrintHeader"));
        }

        [Test]
        [Category("OrderController_POST")]
        [Ignore("Will be test In Phase 2")]
        public void TestOrderController_OrderDelete_ShouldUpdatedOrderStatusToDelete()
        {

        }

        [Test]
        [Category("OrderController_POST")]
        [Ignore("Will be test in Phase 2")]
        public void TestOrderController_OrderReject_ShouldUpdatedOrderStatusToReject()
        {

        }

        [Test]
        [Category("OrderController_GET")]
        [Ignore("Will be test in Phase 2")]
        public void TestOrderController_SendApprovedRejectionEmail_shouldReturnNull()
        {

        }
        // Return Fake HttpContext object for Controller
        public HttpContextBase FakeHttpContext(string httpMethod)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new MockHttpSession();
            var server = new Mock<HttpServerUtilityBase>();
            
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Request.HttpMethod).Returns(httpMethod);
            return context.Object;
        }
    }

   
}
