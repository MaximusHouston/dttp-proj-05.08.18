using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using DPO.Domain;
using DPO.Data;
using DPO.Common;
using DPO.Model.Light;
using DPO.Services.Light;
using Resources = DPO.Resources;
using Moq;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public partial class TestOrderServices: TestAdmin
    {
        public OrderServices orderService = new OrderServices();
        public UserSessionModel user = new UserSessionModel();
        public ServiceResponse response = new ServiceResponse();
        public AccountServices accountService = new AccountServices();
        public OrderViewModelLight orderVMLight = null;
        public QuoteServices quoteService = new QuoteServices();
        public ERPServiceProvider erpSvcPrvdr = new ERPServiceProvider();

        public TestOrderServices() {
            orderService = new OrderServices(this.TContext);
            user = accountService.GetUserSessionModel("User15@test.com").Model as UserSessionModel;
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
        }

        [Test]
        [Category("OrderServices")]
        public void TestOrderService_GetOrderById_ShouldReturnOrderViewModelLight()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            long Id = 479113661118431232;
            response = orderService.GetOrderModel(Id);
            Assert.That(response.IsOK, Is.EqualTo(true));
        }

        [Test]
        [Category("OrderServices")]
        public void TestOrderService_GetOrderModelByQuoteId_ShouldReeturnOrderViewModelLight()
        {
            //Arrange
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            long quoteId = 479102111284477952;
            long orderId = 479113661118431232;

            OrderViewModelLight orderVMLight = new OrderViewModelLight();
            orderVMLight.QuoteId = quoteId;
            orderVMLight.OrderId = orderId;
            //Act
            response = orderService.GetOrderModel(user, orderVMLight);

            //Response
            Assert.That(response.IsOK, Is.EqualTo(true));
        }

        //not working
        //[Test]
        //[Category("OrderServices")]
        //public void TestOrderService_GetOrderViewModel_ShouldReturnOrderViewModel()
        //{
        //    //Arrange
        //    this.response.Messages.Items.Clear();
        //    this.response.Messages.HasErrors = false;

        //    OrderViewModel orderVM = new OrderViewModel();
        //    long quoteId = 479102111284477952;
        //    long orderId = 479113661118431232;
        //    long projectId = 479102086194151424;

        //    orderVM.QuoteId = quoteId;
        //    orderVM.OrderId = orderId;
        //    orderVM.ProjectId = projectId;
        //    //Act
        //    response = orderService.GetOrderViewModel(user, orderVM);

        //    //Assert
        //    Assert.That(response.IsOK, Is.EqualTo(true));
        //}

        [Test]
        [Category("OrderServices")]
        public void TestOrderService_GetOrderInQuote_ShouldReturnOrderViewModel()
        {
            //Arrange
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            long quoteId = 479102111284477952;
            long projectId = 479102086194151424;

            //Act
            response = orderService.GetOrderInQuote(user, projectId, quoteId);
            
            //Assert
            Assert.That(response.IsOK, Is.EqualTo(true));
        }

        [Test]
        [Category("OrderServices_SubmitOrder")]
        public void TestOrderService_SubmitOrder_ShouldPerformModelValidation()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            OrderViewModelLight orderVMLight = new OrderViewModelLight();

            response = orderService.PostModel(user, orderVMLight);

            Assert.That(response.Messages.HasErrors, Is.EqualTo(true));

            if(orderVMLight.ERPAccountId == null)
            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelBusiness.BM010), Is.EqualTo(true));

            if (orderVMLight.CurrentUser == null)
            {
                Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelUser.MU024), Is.EqualTo(true));
            }
            else if (orderVMLight.CurrentUser.Email == null)
            {
                Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP124), Is.EqualTo(true));
            }

            if (orderVMLight.CurrentUser != null && !orderVMLight.CurrentUser.ShowPrices)
            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelBusiness.BM008), Is.EqualTo(true));

            if (orderVMLight.POAttachmentFileName == null || orderVMLight.POAttachmentFileName == string.Empty)
            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceUI.POAttachmentRequired), Is.EqualTo(true));

            if (orderVMLight.OrderReleaseDate == null || orderVMLight.OrderReleaseDate == DateTime.MinValue)
            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP133), Is.EqualTo(true));

            if (orderVMLight.OrderReleaseDate < orderVMLight.SubmitDate)
            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP134), Is.EqualTo(true));

            if (orderVMLight.DeliveryAppointmentRequired)
            {
                if (orderVMLight.DeliveryContactName == null || orderVMLight.DeliveryContactName.Length == 0)
                Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP139), Is.EqualTo(true));
                   
                if (orderVMLight.DeliveryContactPhone == null || orderVMLight.DeliveryContactPhone.Length == 0)
                Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP138), Is.EqualTo(true));   
            }
        }

        [Test]
        [Category("OrderServices_SubmitOrder")]
        public void TestOrderService_SubmitOrder_ModelToEntity_ShouldBeMatched()
        {
            this.response.Messages.HasErrors = false;
            this.response.Messages.Items.Clear();

            Order entity = orderService.ModelToEntity(user, orderVMLight);

            Assert.That(entity.BusinessId, Is.EqualTo(orderVMLight.BusinessId));
            Assert.That(entity.ShipToAddressId, Is.EqualTo(orderVMLight.ShipToAddressId));
            Assert.That(entity.PricingTypeId, Is.EqualTo(orderVMLight.PricingTypeId));
            Assert.That(entity.PONumber, Is.EqualTo(orderVMLight.PONumber));
            Assert.That(entity.TotalDiscountPercent, Is.EqualTo(orderVMLight.TotalDiscountPercent));
            Assert.That(entity.EstimatedReleaseDate, Is.EqualTo(orderVMLight.EstimatedReleaseDate));
            Assert.That(entity.DeliveryAppointmentRequired, Is.EqualTo(orderVMLight.DeliveryAppointmentRequired));
            Assert.That(entity.DeliveryContactName, Is.EqualTo(orderVMLight.DeliveryContactName));
            Assert.That(entity.DeliveryContactPhone, Is.EqualTo(orderVMLight.DeliveryContactPhone));
            Assert.That(entity.OrderReleaseDate, Is.EqualTo(orderVMLight.OrderReleaseDate));
            Assert.That(entity.SubmittedByUserId, Is.EqualTo(orderVMLight.SubmittedByUserId));
            Assert.That(entity.SubmitDate, Is.EqualTo(orderVMLight.SubmitDate));
            Assert.That(entity.CreatedByUserId, Is.EqualTo(orderVMLight.CreatedByUserId));
            Assert.That(entity.UpdatedByUserId, Is.EqualTo(orderVMLight.UpdatedByUserId));
            Assert.That(entity.DiscountRequestId, Is.EqualTo(orderVMLight.DiscountRequestId));
            Assert.That(entity.CommissionRequestId, Is.EqualTo(orderVMLight.CommissionRequestId));
            Assert.That(entity.ERPOrderNumber, Is.EqualTo(orderVMLight.ERPOrderNumber));
            Assert.That(entity.ERPPOKey, Is.EqualTo(orderVMLight.ERPPOKey));
            Assert.That(entity.ERPStatus, Is.EqualTo(orderVMLight.ERPStatus));
            Assert.That(entity.Comments, Is.EqualTo(orderVMLight.Comments));
            Assert.That(entity.ERPComment, Is.EqualTo(orderVMLight.ERPComments));
            Assert.That(entity.ERPOrderDate, Is.EqualTo(orderVMLight.ERPOrderDate));
            Assert.That(entity.ERPInvoiceDate, Is.EqualTo(orderVMLight.ERPInvoiceDate));
            Assert.That(entity.ERPShipDate, Is.EqualTo(orderVMLight.ERPShipDate));
            Assert.That(entity.ERPInvoiceNumber, Is.EqualTo(orderVMLight.ERPInvoiceNumber));
        }

        [Test]
        [Category("OrderServiceRules_RuleOnAdd")]
        public void TestOrderService_RuleOnAdd_CannotSubmitOrderIfAwaitingDiscountRequestIsTrue()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            Order entity = orderService.ModelToEntity(user, orderVMLight);
            entity.Quote.AwaitingDiscountRequest = true;
          
            orderService.RulesOnAdd(user, entity, out response);

            Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP103), Is.EqualTo(true));
       
        }
        
        //[Test]
        //public void TestOrderService_RuleOnAdd_CannotSubmitOrderIfAwaitingCommissionRequestIsTrue()
        //{
        //    OrderViewModelLight orderVMLight = new OrderViewModelLight();
        //    orderVMLight.BusinessId = 206680765352640513;
        //    orderVMLight.ShipToAddressId = 479102086194151432;
        //    orderVMLight.PricingTypeId = 1;
        //    orderVMLight.PONumber = "AAPO0613201601";
        //    orderVMLight.TotalDiscountPercent = 0;
        //    orderVMLight.EstimatedReleaseDate = Convert.ToDateTime("2016-06-06 00:00:00.000");
        //    orderVMLight.DeliveryAppointmentRequired = false;
        //    orderVMLight.DeliveryContactName = "";
        //    orderVMLight.DeliveryContactPhone = "";
        //    orderVMLight.OrderReleaseDate = DateTime.Now.AddDays(2);
        //    orderVMLight.SubmittedByUserId = 392643416367824896;
        //    orderVMLight.SubmitDate = DateTime.Now;
        //    orderVMLight.CreatedByUserId = 392643416367824896;
        //    orderVMLight.UpdatedByUserId = 392643416367824896;
        //    orderVMLight.DiscountRequestId = 0;
        //    orderVMLight.CommissionRequestId = 0;
        //    orderVMLight.ERPOrderNumber = "";
        //    orderVMLight.ERPPOKey = 0;
        //    orderVMLight.ERPStatus = "Submitted";
        //    orderVMLight.Comments = "PLEASE CALL BEFORE SHIPPING";
        //    orderVMLight.ERPComments = "";
        //    orderVMLight.ERPOrderDate = null;
        //    orderVMLight.ERPInvoiceDate = null;
        //    orderVMLight.ERPShipDate = null;
        //    orderVMLight.ERPInvoiceNumber = null;
        //    orderVMLight.QuoteId = 479102111284477952;
        //    orderVMLight.ProjectId = 479102086194151424;
        //    orderVMLight.CurrentUser = user;
        //    var fileUpload = new FileUploadTest();
        //    orderVMLight.POAttachment = fileUpload;
        //    orderVMLight.POAttachmentFileName = fileUpload.FileUploadName;
        //    orderVMLight.ERPAccountId = "1234";

        //    Order entity = orderService.ModelToEntity(user, orderVMLight);
        //    entity.Quote.AwaitingCommissionRequest = true;

        //    orderService.RulesOnAdd(user, entity, out response);

        //    Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP103), Is.EqualTo(true));
        //}

        [Test]
        [Category("OrderServiceRules_RuleOnAdd")]
        public void TestOrderService_RuleOnAdd_CannotSubmitOrderIfQuoteHasNoItem()
        {
            this.response.Messages.HasErrors = false;
            this.response.Messages.Items.Clear();

            var fileUpload = new FileUploadTest();
            Order entity = new Order();
            entity = orderService.ModelToEntity(user, orderVMLight);
            entity.Quote.AwaitingCommissionRequest = false;
            entity.Quote.AwaitingDiscountRequest = false;
            List<OrderItem> orderItems = new List<OrderItem>();
            entity.OrderItems = orderItems;

            orderService.RulesOnAdd(user, entity, out response);

            Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP135), Is.EqualTo(true));
          
        }

        [Test]
        [Category("OrderServiceRules_RuleOnAdd")]
        public void TestOrderService_RuleOnAdd_CannotSubmitOrderIfOrderItemsHasZeroQuantity()
        {
            this.response.Messages.HasErrors = false;
            this.response.Messages.Items.Clear();

            Order entity = new Order();
            entity = orderService.ModelToEntity(user, orderVMLight);
            entity.Quote.AwaitingCommissionRequest = false;
            entity.Quote.AwaitingDiscountRequest = false;
            List<OrderItem> orderItems = new List<OrderItem>();
            orderItems.Add(new OrderItem
            {
            
             AccountMultiplierId = 1,
             DiscountPercentage = 0,
             ExtendedNetPrice = 0,
             LineSequence = 1,
             ListPrice = 0,
             Multiplier = 0,
             Quantity = 0
            });

            entity.OrderItems = orderItems;

            orderService.RulesOnAdd(user, entity, out response);

            Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP136), Is.EqualTo(true));
           
        }

        [Test]
        [Category("OrderServices_SubmitOrder")]
        public void TestOrderService_SubmitOrder_ShouldSaveOrderIntoDatabase()
        {
            this.response.Messages.HasErrors = false;
            this.response.Messages.Items.Clear();
          
            var query = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).ToList();
            Quote quote = null;

            foreach (Project project in query)
            {
                quote = this.db.Context.Quotes.Where(q => q.ProjectId == project.ProjectId && q.Active == false && q.Orders.Count() == 0).FirstOrDefault();
                break;
            }

            orderVMLight.QuoteId = quote.QuoteId;
            orderVMLight.ProjectId = quote.ProjectId;

            orderService.PostModel(user, orderVMLight);

            Order order = this.db.Context.Orders.Where(o => o.QuoteId == orderVMLight.QuoteId).FirstOrDefault();

            Assert.That(order, Is.Not.Null);
            Assert.That(order.OrderItems.Count(), Is.GreaterThan(0));
            Assert.That(order.OrderAttachments.Count(), Is.GreaterThan(0));
        }
      
        [Test]
        [Category("OrderServiceRules_RuleCommon")]
        public void TestOrderService_RuleCommon_CannotSubmitOrderWithoutPONumber()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;
            orderVMLight.PONumber = null;

            Order entity = new Order();
            entity = orderService.ModelToEntity(user, orderVMLight);
            entity.Quote.AwaitingCommissionRequest = false;
            entity.Quote.AwaitingDiscountRequest = false;
            List<OrderItem> orderItems = new List<OrderItem>();
            orderItems.Add(new OrderItem
            {

                AccountMultiplierId = 1,
                DiscountPercentage = 0,
                ExtendedNetPrice = 0,
                LineSequence = 1,
                ListPrice = 0,
                Multiplier = 0,
                Quantity = 1
            });

            entity.OrderItems = orderItems;

            orderService.RulesOnAdd(user, entity, out response);

            Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP125), Is.EqualTo(true));

        }

        [Test]
        [Category("OrderServiceRules_RuleCommon")]
        public void TestOrderService_RuleCommon_CannotSubmitOrderIfQuoteIsACommissionRequest()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            Order entity = new Order();
            entity = orderService.ModelToEntity(user, orderVMLight);
            entity.Quote.AwaitingCommissionRequest = false;
            entity.Quote.AwaitingDiscountRequest = false;
            List<OrderItem> orderItems = new List<OrderItem>();
            orderItems.Add(new OrderItem
            {

                AccountMultiplierId = 1,
                DiscountPercentage = 0,
                ExtendedNetPrice = 0,
                LineSequence = 1,
                ListPrice = 0,
                Multiplier = 0,
                Quantity = 1
            });

            entity.OrderItems = orderItems;
            entity.Quote.IsCommission = true;

            orderService.RulesOnAdd(user, entity, out response);

            Assert.That(this.response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP131), Is.EqualTo(true));
        }

        [Test]
        [Category("OrderServiceRules_RulesOnStatusChange")]
        public void TestOrderService_RulesOnStatusChange_SetQuoteToActiveWhenSubmitOrder()
        {
            this.response.Messages.Items.Clear();
            this.response.Messages.HasErrors = false;

            var query = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).ToList();
            Quote quote = null;

            foreach(Project project in query)
            {
                quote = this.db.Context.Quotes.Where(q => q.ProjectId == project.ProjectId && q.Active == false && q.Orders.Count() == 0).FirstOrDefault();
                break;
            }

            orderVMLight.QuoteId = quote.QuoteId;
            orderVMLight.ProjectId = quote.ProjectId;
            
            orderService.PostModel(user, orderVMLight);

            QuoteModel quoteModel = quoteService.GetQuoteModel(user, orderVMLight.ProjectId, orderVMLight.QuoteId).Model as QuoteModel;

            Assert.That(quoteModel.Active, Is.EqualTo(true));
        }

        [Test]
        [Category("OrderServices_Email")]
        public void TestOrderService_ValidateEmail_ShouldReturnTrue()
        {
            List<string> _emails = new List<string> { "Aaron.Nguyen@daikincomfort.com", "Charles.Teel@goodmanmfg.com" };
            var result = orderService.ValidateEmails(_emails);
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        [Category("OrderServices_Email")]
        public void TestOrderService_ValidateEmail_ShouldReturnFalse()
        {
            List<string> _emails = new List<string> { "Huy.Nguyen@daikincomfort.com", "1234@yahoo.com" };
            var result = orderService.ValidateEmails(_emails);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        [Category("OrderServices_Email")]
        public void TestOrderService_GetInvalidEmails_ShouldReturnListOfInValiadEmails()
        {
            List<string> _InvalidEmails = new List<string> { "Aaron.Nguyen@daikincomfort.com", 
                                                             "Charles.Teel@goodmanmfg.com", 
                                                             "1234@gmail.com" };
            var result = orderService.GetInvalidEmails(_InvalidEmails);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        [Category("OrderServices_Email")]
        public void TestOrderService_GetOrderSendEmailModel_ShouldReturnOrderSendEmailModelLight()
        {
            var result = orderService.getOrderSendEmailModel(orderVMLight);
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.IsInstanceOf<DPO.Model.Light.OrderSendEmailModel>(result);
        }

        [Test]
        [Category("OrderServices_GetOrder")]
        public void TestOrderService_GetOrderModelByOrderViewModelLight_ShouldReturnOrderViewModelLight()
        {
            orderVMLight.OrderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            var result = orderService.GetOrderModel(user, orderVMLight);
            Assert.That(result.IsOK, Is.EqualTo(true));
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.IsInstanceOf<OrderViewModelLight>(result.Model);
            OrderViewModelLight model = result.Model as OrderViewModelLight;
            Assert.That(model.OrderId, Is.EqualTo(orderVMLight.OrderId));

            orderVMLight.OrderId = 0;
        }

        [Test]
        [Category("OrderServices_GetOrder")]
        public void TestOrderService_GetOrderModelByOrderId_ShouldReturnOrderViewModelLight()
        {
            long _orderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            var result = orderService.GetOrderModel(_orderId);
            Assert.That(result.IsOK, Is.EqualTo(true));
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.IsInstanceOf<OrderViewModelLight>(result.Model);
            OrderViewModelLight model = result.Model as OrderViewModelLight;
            Assert.That(model.OrderId, Is.EqualTo(_orderId));
        }

        [Test]
        [Category("OrderServices_ChangeStatus")]
        public void TestOrderService_Approve_ShouldChangeOrderStatusAsAccepted()
        {
            long _orderId = this.db.Context.Orders.Where(o => o.OrderStatusTypeId != 4).OrderByDescending(o => o.OrderId).Select( o => o.OrderId).FirstOrDefault();
            OrderViewModelLight model = orderService.GetOrderModel(_orderId).Model as OrderViewModelLight;
            var result = orderService.Approve(user, model);
            Assert.That(result.IsOK, Is.EqualTo(true));
            Order order = this.db.Context.Orders.Where(o => o.OrderId == model.OrderId).FirstOrDefault();
            Assert.That(order.OrderStatusTypeId, Is.EqualTo(4));
        }

        [Test]
        [Category("OrderServices_ChangeStatus")]
        public void TestOrderService_Delete_ShouldReturnOrderStatusAsCanceled()
        {
            long _orderId = this.db.Context.Orders.Where(o => o.OrderStatusTypeId != 4).OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            OrderViewModelLight model = orderService.GetOrderModel(_orderId).Model as OrderViewModelLight;
            var result = orderService.Delete(user, model);
            Assert.That(result.IsOK, Is.EqualTo(true));
            Order order = this.db.Context.Orders.Where(o => o.OrderId == model.OrderId).FirstOrDefault();
            Assert.That(order.OrderStatusTypeId, Is.EqualTo(8));
        }

        [Test]
        [Category("OrderServices_ChangStatus")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void TestOrderService_ChangeOrderStatus_ShouldChangeOrderStatusPerRequest(int OrderStatusTypeId)
        {
            long _orderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            OrderViewModelLight model = orderService.GetOrderModel(_orderId).Model as OrderViewModelLight;
            var result = orderService.ChangeOrderStatus(user, model, (OrderStatusTypeEnum)OrderStatusTypeId);
            Assert.That(result.IsOK, Is.EqualTo(true));
            Order order = this.db.Context.Orders.Where(o => o.OrderId == model.OrderId).FirstOrDefault();
            Assert.That(order.OrderStatusTypeId, Is.EqualTo(OrderStatusTypeId));
        }

        [Test]
        [Category("OrderServices")]
        public void TestOrderService_GetEntity_ShouldReturnOrderEntity()
        {
            long _orderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            OrderViewModelLight model = orderService.GetOrderModel(_orderId).Model as OrderViewModelLight;
            var result = orderService.GetEntity(user, model);
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.IsInstanceOf<Order>(result);
            Assert.That(result.OrderId, Is.EqualTo(model.OrderId));
        }

        [Test]
        [Category("OrderServices_OrderItems")]
        public void TestOrderService_GetOrderItem_ShouldReturnProductsInOrder()
        {
            var result = orderService.GetOrderItems(user, orderVMLight.QuoteId);
            Assert.That(result, Is.Not.EqualTo(null));
            List<OrderItemsViewModel> model = result.Model as List<OrderItemsViewModel>;
            Assert.That(model.First().OrderId, Is.EqualTo(orderVMLight.OrderId));
            Assert.That(model.Count(), Is.GreaterThan(0));
        }

        [Test]
        [Category("OrderServices_OrderItems")]
        [TestCase(false)]
        [TestCase(true)]
        public void TestOrderService_AdjustOrderItems_ShouldUpdatedOrderItemsQuantity(bool isAdd)
        {
            long _orderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
           
            List<OrderItem> orderItems = this.db.Context.OrderItems.Where(oi => oi.OrderId == _orderId).ToList();
            int orderItemsCount = orderItems.Count();

            OrderViewModelLight model = orderService.GetOrderModel(_orderId).Model as OrderViewModelLight;

            if (isAdd)
            {
                OrderItem newOrderItem = orderItems.Last();
                orderItems.Add(newOrderItem);
                orderService.AdjustOrderItems(user, model, orderItems, isAdd, null);
                orderItems = this.db.Context.OrderItems.Where(oi => oi.OrderId == model.OrderId).ToList();
                Assert.That(orderItems.Count(), Is.GreaterThanOrEqualTo(orderItemsCount));
            }
            else
            {
                OrderItem orderItem = orderItems.First();
                orderItem.Quantity = 10;
                orderService.AdjustOrderItems(user, model, orderItems, isAdd, null);
                orderItems = this.db.Context.OrderItems.Where(oi => oi.OrderId == model.OrderId).ToList();
                Assert.That(orderItems.First().Quantity, Is.EqualTo(10));
            }
        }

        [Test]
        [Category("OrderServices_CheckPONumber")]
        [TestCase("AAPO0613201601", "20051700")]
        [TestCase("1234", "20051700")]
        [TestCase("1234","1234")]
        public void TestOrderService_CheckPONumber_ShouldCheckforExitingPONumberOnOrder(string PONumber, string ERPAccountId)
        {
            response = erpSvcPrvdr.CheckPONumber(PONumber, ERPAccountId);

            if (PONumber == "AAPO0613201601")
            {
              Assert.That(response.Messages.HasErrors, Is.EqualTo(true));
              Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelBusiness.BM011), Is.EqualTo(true));
            }

            if (PONumber == "1234" && ERPAccountId == "20051700")
            Assert.That(response.Model, Is.EqualTo(null));

            if (PONumber == "1234" && ERPAccountId == "1234")
            Assert.That(response.Model, Is.EqualTo(null));
        }

        [Test]
        [Category("OrderServices_CheckPONumber")]
        [TestCase("20051700", "AAPO0613201601")]
        [TestCase("20051700","1234")]
        public void TestOrderService_CheckPONumberExists_ShouldReturnTrueOrFalse(string ERPAccountId, string PONumber)
        {
            var result = erpSvcPrvdr.CheckPONumberExist(ERPAccountId, PONumber);

            if (PONumber == "AAPO0613201601")
                Assert.That(result, Is.EqualTo(true));
            
            if (PONumber == "1234")
                Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        [Category("OrderServices")]
        public void TestOrderService_GetQuoteIdByOrder_ShouldReturnQuoteId()
        {
            long _orderId = this.db.Context.Orders.OrderByDescending(o => o.OrderId).Select(o => o.OrderId).FirstOrDefault();
            long quoteId = orderService.GetQuoteIdByOrder(_orderId);
            Assert.That(quoteId, Is.Not.EqualTo(0));
        }
    }

    public class FileUploadTest : System.Web.HttpPostedFileBase
    {
        public string FileUploadName;

        public FileUploadTest()
        {
            FileUploadName = "Test.txt";
        }
    }   
}
