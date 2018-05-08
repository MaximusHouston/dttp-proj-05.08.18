using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPO.Resources;
using DPO.Common;
using DPO.Data;
using DPO.Model.Light;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Web;
using System.Net;
using Renci.SshNet;

namespace DPO.Domain
{
    public partial class OrderServices : BaseServices, IOrderServices
    {
        public ServiceResponse Delete(UserSessionModel user, OrderViewModelLight model)
        {
            //return ChangeStatus(user, model, OrderStatusTypeEnum.Deleted);
            return null;
        }

        public ServiceResponse Approve(UserSessionModel user, OrderViewModelLight model)
        {
            return ChangeStatus(user, model, OrderStatusTypeEnum.InProcess);
        }

        public ServiceResponse Reject(UserSessionModel user, OrderViewModelLight model)
        {
            return ChangeStatus(user, model, OrderStatusTypeEnum.Canceled);
        }

        /*this service function will be calling by the UpdateOrderStatus API ONLY. It have no business logic checkin at all.
        We can call this as Exception function to Update the Order Status and bypass all the business logic at this time. */
        public ServiceResponse ChangeOrderStatus(UserSessionModel user, OrderViewModelLight model, OrderStatusTypeEnum orderStatus)
        {
            this.Db.ReadOnly = false;

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                entity.OrderStatusTypeId = (byte)orderStatus;
                Entry = Db.Entry(entity);
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, "Order Status updated");
            }

            var newModel = GetOrderModel(user, model).Model as OrderViewModelLight;
            this.Response.Model = newModel;

            return this.Response;
        }

        public ServiceResponse ChangeStatus(UserSessionModel user, OrderViewModelLight model, OrderStatusTypeEnum status)
        {
            this.Db.ReadOnly = false;
            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                entity.OrderStatusTypeId = (byte)status;
                Entry = Db.Entry(entity);

                RulesOnEdit(user, entity);
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, "Order Request");
            }

            var newModel = GetOrderModel(user, model).Model as OrderViewModelLight;
            this.Response.Model = newModel;

            return this.Response;
        }

        #region Post Orders
        public ServiceResponse PostModel(UserSessionModel admin, OrderViewModelLight model)
        {
            this.Db.ReadOnly = false;

            try
            {
                Order order = null;

                // Validate Model 
                RulesOnValidateModel(model);

                // Map to Entity
                if (this.Response.IsOK)
                {
                    order = ModelToEntity(admin, model);
                }

                // Validate Entity
                if (this.Response.IsOK)
                {
                    this.Response.PropertyReference = "";

                    ApplyBusinessRules(admin, order);
                }

                if (this.Response.IsOK)
                {
                    if (model.OrderId == 0)
                    {
                        model.OrderId = order.OrderId;
                    }

                    //== We could save to database at the end ==========
                    //SaveToDatabase(model, entity, string.Format("Order " + model.OrderId, ""));
                    //Order order = this.Db.Context.Orders.Where(o => o.QuoteId == model.QuoteId).OrderByDescending(o => o.OrderId).FirstOrDefault();
                    //==================================================
                    
                    //order Items break down 
                    var orderItemsVM = this.GetOrderItems(admin, model.QuoteId).Model as List<OrderItemsViewModel>;

                    var results = orderItemsVM.ToList();

                    ///Refactored - Anand
                    //Group by productId (and CodeString)  
                    var newResult = GetNewResultList(results);

                    if (order != null && newResult != null)
                    {
                        //main submit process - handles both configured and non-configured
                        this.Response = PersistOrdersAndOrderDetails(newResult, order, model, admin); 
                    }
                }
            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            finaliseModelSvc.FinaliseOrderModel(this.Response.Messages, admin, model);//TODO: do we need this here?
            this.Response.Model = model;

            return this.Response;
        }

        public List<OrderItemsViewModel> GetNewResultList(List<OrderItemsViewModel> results)
        {
            return (from result in results
                    group result by new { result.ProductId, result.CodeString } into temp
                    select new OrderItemsViewModel
                    {
                        OrderId = temp.First().OrderId,
                        ProductId = temp.Key.ProductId,
                        ProductNumber = temp.First().ProductNumber,
                        LineSequence = temp.First().LineSequence,
                        ParentProductId = temp.First().ParentProductId,
                        AccountMultiplierId = temp.First().AccountMultiplierId,
                        ParentProductNumber = temp.First().ParentProductNumber,
                        Quantity = temp.Sum(sm => sm.Quantity),
                        ListPrice = temp.First().ListPrice,
                        Multiplier = temp.First().Multiplier,
                        NetPrice = temp.First().NetPrice,
                        ExtendedPrice = temp.Sum(sm => sm.ExtendedPrice),
                        DiscountPercentage = temp.First().DiscountPercentage,
                        CodeString = temp.Key.CodeString,
                        LineItemTypeId = temp.First().LineItemTypeId,
                        ConfigType = temp.First().ConfigType,
                        QuoteItemId = temp.First().QuoteItemId
                    }).ToList();
        }

        public void IterateOrderItemsIntoTempList(List<OrderItemsViewModel> newResult, Order order, UserSessionModel admin)
        {
            for (var i = 0; i < newResult.Count(); i++)
            {
                var item = newResult[i];

                var orderItem = Db.OrderItemCreate(order.OrderId);
                orderItem.LineSequence = i + 1;
                orderItem.ParentProductId = item.ParentProductId;
                orderItem.ProductId = item.ProductId;
                orderItem.AccountMultiplierId = (item.AccountMultiplierId > 0) ? item.AccountMultiplierId : 0;
                orderItem.ParentProductNumber = item.ParentProductNumber;
                orderItem.ProductNumber = item.ProductNumber;
                orderItem.Quantity = item.Quantity;
                orderItem.ListPrice = item.ListPrice;
                orderItem.Multiplier = item.Multiplier;
                orderItem.NetPrice = item.NetPrice;
                orderItem.ExtendedNetPrice = item.ExtendedPrice;
                orderItem.DiscountPercentage = (item.DiscountPercentage > 0) ? item.DiscountPercentage : 0;
                orderItem.CodeString = item.CodeString;
                orderItem.LineItemTypeId = item.LineItemTypeId;
                orderItem.ConfigType = item.ConfigType;

                //Add OrderItemOptions
                if (orderItem.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                {
                    var quoteItemOptions = Db.QuoteItemOptionsByQuoteItemId(admin, item.QuoteItemId).ToList();

                    foreach (var quoteItemOption in quoteItemOptions)
                    {
                        //add orderItemOption entity
                        var orderItemOption = Db.OrderItemOptionCreate(orderItem);
                        orderItemOption.CodeString = quoteItemOption.CodeString;
                        orderItemOption.OptionProductId = quoteItemOption.OptionProductId;
                        orderItemOption.OptionProductNumber = quoteItemOption.OptionProductNumber;
                        orderItemOption.OptionProductDescription = quoteItemOption.OptionProductDescription;
                        orderItemOption.RequiredQuantity = quoteItemOption.RequiredQuantity;
                        orderItemOption.Quantity = quoteItemOption.Quantity;
                        orderItemOption.ListPrice = quoteItemOption.ListPrice;
                        //Multiplier?
                        orderItemOption.LineItemOptionTypeId = quoteItemOption.LineItemOptionTypeId;
                        orderItem.OrderItemOptions.Add(orderItemOption);

                        //add OrderItemOption View Model to build Submittal XML
                        var orderItemOptionModel = new OrderItemOptionViewModel
                        {
                            //OrderItemOptionId = quoteItemOption.OrderItemOptionId,
                            //OrderItemId = quoteItemOption.OrderItemId,
                            BaseProductId = quoteItemOption.BaseProductId,
                            OptionProductId = quoteItemOption.OptionProductId,
                            OptionProductNumber = quoteItemOption.OptionProductNumber,
                            Quantity = quoteItemOption.Quantity,
                            ListPrice = quoteItemOption.ListPrice,
                            CodeString = quoteItemOption.CodeString,
                            LineItemOptionTypeId = quoteItemOption.LineItemOptionTypeId
                        };
                        newResult[i].OrderItemOptions.Add(orderItemOptionModel);
                    }
                }

                this.Db.Context.OrderItems.Add(orderItem);
            }
        }

        public ServiceResponse PersistOrdersAndOrderDetails(List<OrderItemsViewModel> newResult, Order order, OrderViewModelLight model, UserSessionModel admin)
        {
            var mapicsSvcResp = new ServiceResponse();
            using (var quoteService = new QuoteServices())
            {
                if (!quoteService.HasConfiguredModel(model.QuoteId)) // Regular Order
                {
                    //create a json array and call mapics to receive ok or not ok response
                    mapicsSvcResp = erpServiceProvider.CheckWithMapicsBeforeSavingToDb(newResult, order, model);

                    IterateOrderItemsIntoTempList(newResult, order, admin);  //loop through newResult;

                    if (!mapicsSvcResp.IsOK)
                    {
                        this.Response.Messages.AddError($"Couldn't post order and order details in mapics for order item {order.OrderId}");
                        return this.Response;
                    }
                }
                else// Order Has Configured Model
                {
                    IterateOrderItemsIntoTempList(newResult, order, admin);

                    model.ConfigOrderNumber = GetConfiguredOrderNumber();

                    order.ConfigOrderNumber = model.ConfigOrderNumber;

                    //Upload PO File to FTP Server
                    var sftpResp = UploadPOAttachmentToFTPServer(model.QuoteId, model.ConfigOrderNumber, model.POAttachmentFileName);
                    model.POAttachmentFileLocation = sftpResp.Model as String;

                    //Send order request to Mapics
                    var xmlRequest = this.BuildSubmittalOrderXMLString(model, newResult);
                    var xmlResponse = erpServiceProvider.SendOrderRequestToMapics(xmlRequest);
                    mapicsSvcResp = erpServiceProvider.ProcessMapicsOrderSeriveResponse(xmlResponse);

                    if (!mapicsSvcResp.IsOK)
                    {
                        this.Response.AddError("Request to Mapics web service failed!");
                        return this.Response;
                    }
                }

                if (mapicsSvcResp.IsOK)
                {
                    var orderAttachmentVM = Db.OrderAttachmentCreate(order.OrderId);
                    orderAttachmentVM.FileName = model.POAttachmentFileName ?? "No File Name";
                    this.Db.Context.OrderAttachments.Add(orderAttachmentVM);
                    this.Db.SaveChanges();

                    this.Response.Messages.AddInformation(ResourceUI.OrderSubmitInformationMessage);
                }
            }

            return this.Response;
        }

        #endregion

        #region Post Model To Entity
        public Order ModelToEntity(UserSessionModel admin, OrderViewModelLight model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.BusinessId = (model.BusinessId != null) ? model.BusinessId.Value : 0;
            entity.ShipToAddressId = (model.ShipToAddressId != null) ? model.ShipToAddressId.Value : 0;
            entity.PricingTypeId = (byte)model.PricingTypeId;
            entity.PONumber = model.PONumber;
            entity.TotalDiscountPercent = 0;
            entity.EstimatedReleaseDate = model.EstimatedReleaseDate ?? DateTime.UtcNow;
            entity.DeliveryAppointmentRequired = (!string.IsNullOrEmpty(model.DeliveryAppointmentRequired.ToString())) && model.DeliveryAppointmentRequired;
            entity.DeliveryContactName = model.DeliveryContactName;
            entity.DeliveryContactPhone = model.DeliveryContactPhone;
            entity.OrderReleaseDate = model.OrderReleaseDate;
            entity.SubmittedByUserId = (!string.IsNullOrEmpty(model.SubmittedByUserId.ToString())) ?
                                         model.SubmittedByUserId : admin.UserId;
            //entity.SubmitDate = model.SubmitDate ?? DateTime.UtcNow;
            entity.SubmitDate = (DateTime)model.SubmitDate;
            entity.CreatedByUserId = (!string.IsNullOrEmpty(model.CreatedByUserId.ToString())) ?
                                       model.CreatedByUserId : admin.UserId;
            entity.UpdatedByUserId = (!string.IsNullOrEmpty(model.UpdatedByUserId.ToString())) ?
                                     model.UpdatedByUserId : admin.UserId;
            entity.DiscountRequestId = (model.DiscountRequestId > 0) ? model.DiscountRequestId : 0;
            entity.CommissionRequestId = (model.CommissionRequestId > 0) ? model.CommissionRequestId : 0;
            entity.ERPOrderNumber = (model.ERPOrderNumber != null) ? model.ERPOrderNumber : null;
            entity.ERPPOKey = (model.ERPPOKey != null) ? model.ERPPOKey : (int?)null;
            entity.ERPStatus = (model.ERPStatus != null) ? model.ERPStatus : null;
            entity.Comments = model.Comments;
            entity.ERPComment = (model.ERPComments != null) ? model.ERPComments : null;
            entity.ERPOrderDate = (model.ERPOrderDate != null & model.ERPOrderDate != DateTime.MinValue) ?
                                   model.ERPOrderDate : (System.DateTime?)null;
            entity.ERPInvoiceDate = (model.ERPInvoiceDate != null & model.ERPOrderDate != DateTime.MinValue) ?
                                   model.ERPInvoiceDate : (System.DateTime?)null;
            entity.ERPShipDate = (model.ERPShipDate != null & model.ERPOrderDate != DateTime.MinValue) ?
                                   model.ERPOrderDate : (System.DateTime?)null;
            entity.ERPInvoiceNumber = (model.ERPInvoiceNumber != null) ? model.ERPInvoiceNumber : null;

            if (entity.Quote == null)
            {
                entity.Quote = this.Context.Quotes.FirstOrDefault(q => q.QuoteId == entity.QuoteId);
            }

            if (entity.Quote.Project == null)
            {
                entity.Quote.Project = this.Context.Projects.FirstOrDefault(p => p.ProjectId == entity.Quote.ProjectId);
            }

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }
        #endregion

        #region AddOrderItems
        public ServiceResponse GetOrderItems(UserSessionModel user, long quoteId)
        {
            this.Response = new ServiceResponse();

            this.Context.ReadOnly = false;

            // Pull all products that have standard child items (system breakdown, multi-module breakdown)
            var childProductQuery = from q in this.Context.Quotes
                                    where q.QuoteId == quoteId
                                    join qi in this.Context.QuoteItems.Where(qi => qi.Quantity > 0)
                                    on q.QuoteId equals qi.QuoteId
                                    join pp in this.Context.Products
                                    on qi.ProductId equals pp.ProductId
                                    join pa in this.Context.ProductAccessories
                                    on pp.ProductId equals pa.ParentProductId
                                    where pa.RequirementTypeId == (int)RequirementTypeEnums.Standard
                                    join cp in this.Context.Products
                                    on pa.ProductId equals cp.ProductId
                                    where cp.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor || cp.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                                    #region Remove after 10/31/2017
                                    //join dar in this.Context.DiscountRequests
                                    //on q.QuoteId equals dar.QuoteId into rt
                                    //from r in rt.DefaultIfEmpty()
                                    #endregion Remove after 10/31/2017
                                    select new OrderItemsViewModel
                                    {
                                        ParentProductId = (!string.IsNullOrEmpty(pa.ParentProductId.ToString())) ?
                                                            pa.ParentProductId : 0,
                                        ProductId = (!string.IsNullOrEmpty(cp.ProductId.ToString())) ?
                                                      cp.ProductId : 0,
                                        AccountMultiplierId = (long)cp.MultiplierTypeId.Value,
                                        ParentProductNumber = pp.ProductNumber,
                                        ProductNumber = cp.ProductNumber,
                                        Quantity = pa.Quantity * qi.Quantity,
                                        ListPrice = cp.ListPrice,
                                        Multiplier = qi.Multiplier,
                                        NetPrice = qi.Multiplier * cp.ListPrice,
                                        ExtendedPrice = cp.ListPrice * qi.Multiplier * pa.Quantity * qi.Quantity,
                                        MultiplierCategoryTypeId = cp.MultiplierType.MultiplierTypesMultiplierCategoryTypes.Select(mct => mct.MultiplierCategoryTypeId).FirstOrDefault(),
                                        CodeString = qi.CodeString,
                                        LineItemTypeId = qi.LineItemTypeId,
                                        ConfigType = qi.ConfigType,
                                        QuoteItemId = qi.QuoteItemId

                                        #region Remove after 10/31/2017

                                        //DiscountPercentage = 
                                        //   (r != null) 
                                        //       ? (cp.MultiplierType.MultiplierTypesMultiplierCategoryTypes
                                        //           .Any(cpmt => cpmt.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV) 
                                        //               ? r.ApprovedDiscountVRV * 100 : r.ApprovedDiscountSplit * 100) 
                                        //       : 0

                                        #endregion Remove after 10/31/2017
                                    };

            // Pull all products that do not have standard child items (so no systems or multi-module units)
            var nonChildProductQuery = from q in this.Context.Quotes
                                       where q.QuoteId == quoteId
                                       join qi in this.Context.QuoteItems.Where(qi => qi.Quantity > 0)
                                       on q.QuoteId equals qi.QuoteId
                                       join pp in this.Context.Products
                                       on qi.ProductId equals pp.ProductId
                                       #region Remove after 10/31/2017
                                       //join dar in this.Context.DiscountRequests
                                       //on q.QuoteId equals dar.QuoteId into rt
                                       //from r in rt.DefaultIfEmpty()
                                       #endregion Remove after 10/31/2017
                                       where !childProductQuery.Any(a => a.ParentProductId == qi.ProductId)
                                       select new OrderItemsViewModel
                                       {
                                           ParentProductId = 0,
                                           ProductId = pp.ProductId,
                                           AccountMultiplierId = (long)pp.MultiplierTypeId.Value,
                                           ParentProductNumber = pp.ProductNumber,
                                           ProductNumber = pp.ProductNumber,
                                           Quantity = qi.Quantity,
                                           ListPrice = qi.ListPrice,
                                           Multiplier = qi.Multiplier,
                                           NetPrice = qi.Multiplier * qi.ListPrice,
                                           ExtendedPrice = qi.ListPrice * qi.Multiplier * qi.Quantity,
                                           MultiplierCategoryTypeId = pp.MultiplierType.MultiplierTypesMultiplierCategoryTypes.Select(mct => mct.MultiplierCategoryTypeId).FirstOrDefault(),
                                           CodeString = qi.CodeString,
                                           LineItemTypeId = qi.LineItemTypeId,
                                           ConfigType = qi.ConfigType,
                                           QuoteItemId = qi.QuoteItemId

                                           #region Remove after 10 / 31 / 2017

                                           //DiscountPercentage =
                                           //    (r != null)
                                           //            ? (pp.MultiplierType.MultiplierTypesMultiplierCategoryTypes
                                           //                .Any(cpmt => cpmt.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV)
                                           //                    ? r.ApprovedDiscountVRV * 100 : r.ApprovedDiscountSplit * 100)
                                           //            : 0

                                           #endregion Remove after 10/31/2017

                                       };

            var query = childProductQuery.Concat(nonChildProductQuery);
            query = ApplyOrderItemDiscounts(query, quoteId);
            this.Response.Model = query.OrderBy(x => x.ProductNumber).ToList();

            return this.Response;
        }

        public IQueryable<OrderItemsViewModel> GetOrderItemsBrokenDown(long orderId)
        {
            var orderItemQuery = from orderItem in this.Context.OrderItems
                                 where orderItem.OrderId == orderId
                                 select new OrderItemsViewModel
                                 {
                                     OrderItemId = orderItem.OrderItemId,
                                     OrderId = orderItem.OrderId,
                                     LineSequence = orderItem.LineSequence,
                                     ParentProductId = (long)orderItem.ParentProductId,
                                     ParentProductNumber = orderItem.ParentProductNumber,
                                     ProductId = orderItem.ProductId,
                                     ProductNumber = orderItem.ProductNumber,
                                     Quantity = orderItem.Quantity,
                                     ListPrice = orderItem.ListPrice,
                                     Multiplier = orderItem.Multiplier,
                                     NetPrice = orderItem.NetPrice,
                                     DiscountPercentage = orderItem.DiscountPercentage,
                                     CodeString = orderItem.CodeString,
                                     LineItemTypeId = orderItem.LineItemTypeId

                                 };
            return orderItemQuery;
        }

        public IQueryable<OrderItemOptionViewModel> GetOrderItemOptions(long orderId)
        {
            var orderItemOptionQuery = from optionItem in this.Context.OrderItemOptions
                                       where optionItem.OrderId == orderId
                                       select new OrderItemOptionViewModel
                                       {
                                           OrderItemOptionId = optionItem.OrderItemOptionId,
                                           OrderItemId = optionItem.OrderItemId,
                                           BaseProductId = optionItem.BaseProductId,
                                           OptionProductId = optionItem.OptionProductId,
                                           OptionProductNumber = optionItem.OptionProductNumber,
                                           Quantity = optionItem.Quantity,
                                           ListPrice = optionItem.ListPrice,
                                           CodeString = optionItem.CodeString,
                                           LineItemOptionTypeId = optionItem.LineItemOptionTypeId
                                       };
            return orderItemOptionQuery;
        }

        public IQueryable<OrderItemOptionViewModel> GetOrderItemOptionsByOrderItemId(long orderId, long orderItemId)
        {
            var orderItemOptionQuery = from optionItem in this.Context.OrderItemOptions
                                       where optionItem.OrderId == orderId && optionItem.OrderItemId == orderItemId
                                       select new OrderItemOptionViewModel
                                       {
                                           OrderItemOptionId = optionItem.OrderItemOptionId,
                                           OrderItemId = optionItem.OrderItemId,
                                           BaseProductId = optionItem.BaseProductId,
                                           OptionProductId = optionItem.OptionProductId,
                                           OptionProductNumber = optionItem.OptionProductNumber,
                                           Quantity = optionItem.Quantity,
                                           ListPrice = optionItem.ListPrice,
                                           CodeString = optionItem.CodeString,
                                           LineItemOptionTypeId = optionItem.LineItemOptionTypeId
                                       };
            return orderItemOptionQuery;
        }

        public IQueryable<OrderItemsViewModel> ApplyOrderItemDiscounts(IQueryable<OrderItemsViewModel> query, long quoteId)
        {
            // Apply appropriate DAR discounts for items if needed
            var currApprovedDar = (from d in this.Context.DiscountRequests
                                   where d.QuoteId == quoteId
                                     && d.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved
                                   select d).FirstOrDefault();


            if (currApprovedDar == null)
            {
                return query;
            }

            var orderItems = query.ToArray();

            // Apply line item discount
            foreach (var orderItem in orderItems)
            {
                if (orderItem.MultiplierCategoryTypeId != null)
                {

                    decimal lineItemDiscount = 0;
                    switch (orderItem.MultiplierCategoryTypeId)
                    {
                        case MultiplierCategoryTypeEnum.VRV:
                            lineItemDiscount = currApprovedDar.ApprovedDiscountVRV;
                            break;
                        case MultiplierCategoryTypeEnum.Unitary:
                            lineItemDiscount = currApprovedDar.ApprovedDiscountUnitary.HasValue ? currApprovedDar.ApprovedDiscountUnitary.Value : 0;
                            break;
                        case MultiplierCategoryTypeEnum.Split:
                            lineItemDiscount = currApprovedDar.ApprovedDiscountSplit;
                            break;
                        case MultiplierCategoryTypeEnum.LCPackage:
                            lineItemDiscount = currApprovedDar.ApprovedDiscountLCPackage.HasValue ? currApprovedDar.ApprovedDiscountLCPackage.Value : 0;
                            break;
                    }

                    // TODO: We need to standardize percentages... we should probably standard on decimals not percentage
                    // Convert decimal to a percentage value
                    orderItem.DiscountPercentage = lineItemDiscount * 100;
                }
            }

            return orderItems.AsQueryable();
        }

        public void AdjustOrderItems(UserSessionModel user, OrderViewModelLight model, List<OrderItem> OrderItemUpdates, bool isAdd, Order entity = null)
        {
            this.Context.ReadOnly = false;

            var order = entity;

            if (order == null)
            {
                order = GetEntity(user, model);
            }

            if (order != null && OrderItemUpdates != null && OrderItemUpdates.Count > 0)
            {

                this.Context.Entry(order).State = EntityState.Modified;

                ApplyBusinessRules(user, order);

                if (this.Response.IsOK)
                {
                    base.SaveToDatabase(model, order, string.Format("Order '{0}'", order.OrderId));
                }
            }

            return;
        }

        public OrderOptionsModel GetOrderOptionsModel(UserSessionModel user, long? projectId, long? currentQuoteId)
        {
            OrderOptionsModel orderOptions = new OrderOptionsModel();

            //========= No ShowPrices ================
            if (user.ShowPrices == false)
            {
                orderOptions.CanSubmitOrder = false;
                orderOptions.CanViewSubmittedOrder = false;
                orderOptions.CanViewOrders = false;
                return orderOptions;
            }

            if (user.HasAccess(SystemAccessEnum.SubmitOrder))
            {
                //============Submit Order is not allowed when there is no products in quote================
                var queryQuoteItems = from quoteItem in this.Context.QuoteItems
                                      where quoteItem.QuoteId == currentQuoteId
                                      select quoteItem;
                var productList = queryQuoteItems.ToList();
                if (productList == null || productList.Count == 0)
                {
                    orderOptions.CanSubmitOrder = false;
                    orderOptions.CanViewSubmittedOrder = false;

                    return orderOptions;
                }
                //==========================================================================================

                //============Hide Submit Order button when quote is Commission or quote is Deleted=================
                var queryQuote = from quote in this.Context.Quotes
                                 where quote.QuoteId == currentQuoteId
                                 select quote;

                var currentquote = queryQuote.FirstOrDefault();
                if (currentquote.IsCommission || currentquote.Deleted)
                {
                    orderOptions.CanSubmitOrder = false;
                    orderOptions.CanViewSubmittedOrder = false;
                    orderOptions.CanViewOrders = true;// can see Orders Tab 

                    return orderOptions;
                }
                //==============================================================================

                //============Check Discount Request Status===============================
                var queryDARrequest = from dar in this.Context.DiscountRequests
                                      where dar.QuoteId == currentQuoteId
                                      select dar;
                var quoteDAR = queryDARrequest.FirstOrDefault();
                if (quoteDAR != null && quoteDAR.DiscountRequestStatusTypeId == 2)
                {//========= DAR pending======
                    orderOptions.CanSubmitOrder = false;
                    orderOptions.CanViewSubmittedOrder = false;

                    return orderOptions;
                }
                //========================================================================

                //===========Check Commission Request Status==============================
                var queryCOMrequest = from com in this.Context.CommissionRequests
                                      where com.QuoteId == currentQuoteId
                                      select com;
                var quoteCOM = queryCOMrequest.FirstOrDefault();

                if (quoteCOM != null && quoteCOM.CommissionRequestStatusTypeId == 2)
                {//========= COM pending======
                    orderOptions.CanSubmitOrder = false;
                    orderOptions.CanViewSubmittedOrder = false;

                    return orderOptions;
                }
                //========================================================================

                //======== DAR/COM approved  or  No DAR/COM ==============================
                var queryOrderInProject = from order in this.Context.Orders
                                          join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
                                          where quote.ProjectId == projectId
                                          select order;
                var existedOrder = queryOrderInProject.FirstOrDefault();

                if (existedOrder != null)/*=== Order existed =====*/
                {
                    if (existedOrder.OrderStatusTypeId == 1 || existedOrder.OrderStatusTypeId == 2 || existedOrder.OrderStatusTypeId == 3) // New Record/ Submitted/ Awaiting CSR
                    {
                        orderOptions.CanSubmitOrder = false;
                    }
                    else
                    {
                        if (existedOrder.OrderStatusTypeId == 4) // Accepted 
                        {
                            if (existedOrder.QuoteId == currentQuoteId)
                            {
                                orderOptions.CanSubmitOrder = false;
                            }
                            else // this is to allow many orders in one project 
                            {
                                orderOptions.CanSubmitOrder = true;
                            }
                        }
                        else if (existedOrder.OrderStatusTypeId == 5) //InProcess
                        {
                            orderOptions.CanSubmitOrder = false;
                        }
                        else if (existedOrder.OrderStatusTypeId == 6) //Picked
                        {
                            orderOptions.CanSubmitOrder = false;
                        }
                        else if (existedOrder.OrderStatusTypeId == 7) //Shipped
                        {
                            orderOptions.CanSubmitOrder = false;
                        }
                        else if (existedOrder.OrderStatusTypeId == 8)// Canceled
                        {
                            orderOptions.CanSubmitOrder = true;
                        }
                    }
                    orderOptions.CanViewSubmittedOrder = true;
                }
                else                  /*===== Order not existed ====*/
                {
                    orderOptions.CanSubmitOrder = true;
                    orderOptions.CanViewSubmittedOrder = false;
                }

                orderOptions.CanViewOrders = true;

                return orderOptions;

                //========End of DAR/COM approved  or  No DAR/COM ========================
            }
            else /***** when user doesn't have permission to Submit Order *****/
            {
                orderOptions.CanSubmitOrder = false;
                if (user.HasAccess(SystemAccessEnum.ViewOrder))
                {
                    orderOptions.CanViewOrders = true;

                    var queryOrderInProject = from order in this.Context.Orders
                                              join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
                                              where quote.ProjectId == projectId
                                              select order;
                    var existedOrder = queryOrderInProject.FirstOrDefault();

                    if (existedOrder != null)
                    {
                        orderOptions.CanViewSubmittedOrder = true;
                    }
                    else
                    {
                        orderOptions.CanViewSubmittedOrder = false;
                    }

                }
                else
                { //=== user doesn't have permission to Submit Order  && View Order
                    orderOptions.CanViewSubmittedOrder = false;
                    orderOptions.CanViewOrders = false;
                }

                return orderOptions;
            }
        }// end of GetOrderOptionsModel

        public ServiceResponse SubmitOrderToMapics(long orderId)
        {
            return this.Response;
        }

        public MfgModel BuildMfgModel(OrderItemsViewModel configOrderItem)
        {
            MfgModel Model = new MfgModel()
            {
                ConfigType = configOrderItem.ConfigType,
                CodeString = configOrderItem.CodeString,
                ModelNumber = configOrderItem.CodeString,
                Quantity = (int)configOrderItem.Quantity,
                BaseModel = new MfgBaseModel()
                {
                    BaseModelNumber = configOrderItem.ProductNumber,
                    Efficiency = "Standard"
                },
                Accessories = new List<MfgAccessory>()
            };

            //Get FactoryInstalled Items
            foreach (var orderItemOption in configOrderItem.OrderItemOptions)
            {
                if (orderItemOption.LineItemOptionTypeId == (byte)LineItemOptionTypeEnum.FactoryInstalled)
                {
                    MfgAccessory accessory = new MfgAccessory()
                    {
                        AccessoryItemNumber = orderItemOption.OptionProductNumber
                    };
                    Model.Accessories.Add(accessory);
                }
            }

            return Model;
        }       

        public string BuildSubmittalOrderXMLString(OrderViewModelLight orderModel, List<OrderItemsViewModel> orderItems)
        {
            //var configOrderNumber = GetConfiguredOrderNumber();

            var ShipToAddress = this.Db.GetAddressByAddressId(orderModel.ShipToAddressId);

            //=======GET SUBMITTAL ORDER INFO======
            var SubmittalOrder = new SubmittalOrder()
            {
                CompanyNumber = "01",
                CustomerNumber = orderModel.ERPAccountId,
                ConfigOrder = orderModel.ConfigOrderNumber.ToString("D6"),
                SalesOrder = new SalesOrder()
                {
                    PONumber = orderModel.PONumber,
                    POFile = orderModel.POAttachmentFileLocation,
                    PODate = orderModel.SubmitDate.ToString("yyyy-MM-dd"),
                    RequestDate = orderModel.EstimatedDeliveryDate.ToString("yyyy-MM-dd"),
                    OrderType = "PC",// get from LCST
                    ShipTo = new ShipTo()
                    {
                        Name = orderModel.ShipToName,
                        Address1 = ShipToAddress.AddressLine1,
                        Address2 = ShipToAddress.AddressLine2,
                        Address3 = ShipToAddress.AddressLine3,
                        State = ShipToAddress.State.Code,
                        City = ShipToAddress.Location,
                        Zip = ShipToAddress.PostalCode
                    },
                    Comments = orderModel.Comments,
                    BusinessID = (long)orderModel.BusinessId,
                    ProjectID = orderModel.ProjectId,
                    QuoteID = orderModel.QuoteId,
                    LineItems = new List<LineItem>()
                },
                MfgOrder = new MfgOrder()
                {
                    //OrderType = "Customer",
                    Models = new List<MfgModel>()
                }
            };

            //=============BUILD SALESORDER LINE ITEMS=========

            var nextLineSequence = orderItems.Select(i => i.LineSequence).Max() + 1;

            foreach (var orderItem in orderItems)
            {
                //Get standard items
                if (orderItem.LineItemTypeId == (byte)LineItemTypeEnum.Standard)
                {
                    var lineItem = new LineItem()
                    {
                        LineSequence = orderItem.LineSequence * 10,
                        SKU = orderItem.ProductNumber,
                        Quantity = (int)orderItem.Quantity,
                        Price = orderItem.NetPrice,
                        Discount = orderItem.DiscountPercentage,
                        CodeString = orderItem.CodeString
                    };

                    SubmittalOrder.SalesOrder.LineItems.Add(lineItem);
                }
                //Get configured items (Base Model)
                if (orderItem.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
                {
                    var lineItem = new LineItem()
                    {
                        LineSequence = orderItem.LineSequence * 10,
                        SKU = orderItem.CodeString,
                        Quantity = (int)orderItem.Quantity,
                        Price = orderItem.NetPrice,
                        Discount = orderItem.DiscountPercentage,
                        CodeString = orderItem.CodeString,
                        BaseModel = orderItem.ProductNumber
                    };

                    SubmittalOrder.SalesOrder.LineItems.Add(lineItem);

                    //===Add MFG MODELS===
                    var Model = BuildMfgModel(orderItem);
                    SubmittalOrder.MfgOrder.Models.Add(Model);
                }
            }// end of foreach orderItems

            var serializer = new XmlSerializer(SubmittalOrder.GetType());
            var xmlStringWriter = new System.IO.StringWriter();
            serializer.Serialize(xmlStringWriter, SubmittalOrder);

            return xmlStringWriter.ToString();
        }

        public int GetConfiguredOrderNumber()
        {
            return this.Db.GetConfigOrderNumber();
        }
        #endregion

        public string GetEncodedPOAttachment(long quoteId, string fileName)
        {
            string base64FileStr = "";
            var directory = Utilities.GetPOAttachmentDirectory(quoteId);
            var fullFileName = directory + "\\" + fileName;
            try
            {
                var file = new FileInfo(fullFileName);
                if (file.Exists)
                {
                    Byte[] bytes = File.ReadAllBytes(fullFileName);
                    base64FileStr = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return base64FileStr;
        }

        public ServiceResponse UploadPOAttachmentToFTPServer(long quoteId, int configOrderNumber, string fileName)
        {
            var fileLocation = string.Empty;

            try
            {
                var directory = Utilities.GetPOAttachmentDirectory(quoteId);
                var sourceFileName = Path.Combine(directory, fileName);

                using (SftpClient sftpClient = CreatePIMSftp())
                {
                    sftpClient.Connect();
                    using (FileStream fileStream = new FileStream(sourceFileName, FileMode.Open))
                    {
                        sftpClient.BufferSize = 1024;
                        var newName = configOrderNumber.ToString("D6") + "-" + fileName;
                        var destination = Path.Combine(Utilities.Config("dpo.sys.sftp.daikincity.poAttachments"), newName);
                        sftpClient.UploadFile(fileStream, destination, null);
                        fileLocation = Utilities.Config("dpo.sys.sftp.host") + destination;
                    }
                    sftpClient.Dispose();
                }

                this.Response.Model = fileLocation;

            }
            catch (Exception e)
            {
                Log.Error("UploadPOAttachmentToFTPServer: " + e.Message);
            }

            return this.Response;
        }

        public SftpClient CreatePIMSftp()
        {
            var hostName = Utilities.Config("dpo.sys.sftp.host");
            var username = Utilities.Config("dpo.sys.sftp.username");
            var password = Utilities.Config("dpo.sys.sftp.password");
            var portString = Utilities.Config("dpo.sys.sftp.port");

            int port;
            if (!Int32.TryParse(portString, out port))
            {
                port = 22; // Default SSH Port
            }

            return new SftpClient(hostName, port, username, password);
        }

        ///removed to erpServiceProvicer -- delete later
        /// //public ServiceResponse CheckPONumber(string PONumber, string ERPAccountId)
        //{
        //    int count = (from o in this.Context.Orders
        //                 from b in this.Context.Businesses
        //                 where b.BusinessId == o.BusinessId && b.ERPAccountId == ERPAccountId
        //                 select o)
        //        .Where(o => o.PONumber == PONumber).Count();

        //    if (count > 0)
        //    {
        //        this.Response.AddError(Resources.ResourceModelBusiness.BM011);
        //    }

        //    return this.Response;
        //}

        //public ServiceResponse CheckPONumberExist(string ERPAccountId, string PONumber)
        //{
        //    //var count = (from o in this.Context.Orders select o).Count(o => o.PONumber == PONumber
        //    //                && o.Quote.Project.Owner.Business.ERPAccountId == ERPAccountId);

        //    //if (count > 0)
        //    //{
        //    //    Response.Messages.AddError("PONumber", "PO number already exists");
        //    //    return Response;
        //    //}
        //    //else
        //    //{
        //    //    Response = ERPClient.GetOrderInfoFromMapicsAsync(ERPAccountId, PONumber); //connect to mapics web api call
        //    //    return Response;
        //    //}

        //    var response = serviceHelper.CheckPONumber(PONumber, ERPAccountId);
        //    return response;
        //}

        //public ServiceResponse CheckWithMapicsBeforeSavingToDb(List<OrderItemsViewModel> orderItemsVm, Order order,
        //    OrderViewModelLight model)
        //{
        //    var orderDetailList = new List<OrderDetail>(); // array of order detail to send it to mapics
        //    var address = Db.Addresses.FirstOrDefault(x => x.AddressId == model.ShipToAddressId);
        //    var state = Db.States.FirstOrDefault(x => x.StateId == address.StateId);

        //    var increment = 1;
        //    foreach (var item in orderItemsVm)
        //    {
        //        var orderDetail = new OrderDetail()
        //        {
        //            LineSeq = increment,
        //            ProductNumber = item.ProductNumber,
        //            CustomerProductNo = "",
        //            Quantity = item.Quantity,
        //            NetPrice = item.NetPrice,
        //            ExtendedNetPrice = item.ExtendedPrice,
        //            ProductDescription = "",
        //            DiscountPercent = item.DiscountPercentage,
        //            CompanyNo = 1,
        //        };
        //        increment++;

        //        orderDetailList.Add(orderDetail);
        //    }

        //    //construct json array to post it to mapics
        //    var jsonData = new ERPOrderInfo
        //    {
        //        CustomerNumber = !string.IsNullOrWhiteSpace(model.ERPAccountId) ? Convert.ToInt32(model.ERPAccountId) : 0,
        //        PONo = model.PONumber,
        //        PODate = DateTime.Today,
        //        RequestDate = model.OrderReleaseDate,
        //        TermsCode = "",
        //        OrderType = "DK",
        //        ShipToName = model.ShipToName,
        //        ShipToAddress1 = address?.AddressLine1,
        //        ShipToAddress2 = address?.AddressLine2,
        //        ShipToCity = address?.Location,
        //        ShipToState = state?.Code,
        //        ShipToZip = address?.PostalCode,
        //        ShipToInstruction = order.Comments,  ///From Delivery notes
        //        ContactName = model.DeliveryContactName,
        //        ContactPhone = model.DeliveryContactPhone,
        //        TotalAmount = model.TotalNetPrice,
        //        OrderCode = "DK",
        //        Status = model.ERPStatus,
        //        ShipToNumber = null,
        //        CompanyNo = 1,
        //        BusinessID = model.BusinessId.GetValueOrDefault(),
        //        BusinessName = model.BusinessName,
        //        ProjectID = model.ProjectId,
        //        ProjectName = model.ProjectName,
        //        ProjectRefID = null,
        //        QuoteID = model.QuoteId,
        //        QuoteRefID = null,
        //        Comments = model.Comments,
        //        DiscountPercent = 0,
        //        Details = orderDetailList?.ToArray()
        //    };

        //    this.Response = ERPClient.PostOrderToMapicsAsync(jsonData);

        //    return this.Response;
        //}

        //public string SendOrderRequestToMapics(string xmlRequest)
        //{
        //    //Call Mapics Web Service - Maran
        //    MapicsOrderService.CFG001RInput input = new MapicsOrderService.CFG001RInput();
        //    input.INORDERREQ = new MapicsOrderService.INORDERREQ();
        //    input.INORDERREQ.@string = xmlRequest;
        //    input.INORDERREQ.length = input.INORDERREQ.@string.Length;

        //    MapicsOrderService.cfg001rRequest req = new MapicsOrderService.cfg001rRequest()
        //    {
        //        args0 = input
        //    };

        //    //Request
        //    MapicsOrderService.CFG001RPortType client = new MapicsOrderService.CFG001RPortTypeClient("CFG001RHttpSoap11Endpoint");
        //    MapicsOrderService.cfg001rResponse res = client.cfg001r(req);

        //    //Response
        //    var xmlResponseString = res.@return.INORDERREQ.@string;

        //    return xmlResponseString;

        //    //=============================

        //    //Call Mapics Web Service - Vinu
        //    //MapicsOrderService.cfg001RInput input = new MapicsOrderService.cfg001RInput();
        //    //input._INORDERREQ = new MapicsOrderService.inorderreq();
        //    //input._INORDERREQ._String = xmlRequest;
        //    //input._INORDERREQ._Length = input._INORDERREQ._String.Length;

        //    //MapicsOrderService.cfg001rRequest req = new MapicsOrderService.cfg001rRequest()
        //    //{
        //    //    arg0 = input
        //    //};

        //    ////Request
        //    //MapicsOrderService.CFG001RServices client = new MapicsOrderService.CFG001RServicesClient("CFG001RServicesPort");
        //    //MapicsOrderService.cfg001rResponse res = client.cfg001r(req);

        //    ////Response
        //    //var xmlResponseString = res.@return._INORDERREQ._String;

        //    //return xmlResponseString;

        //}

        //public ServiceResponse ProcessMapicsOrderSeriveResponse(string xmlResponse)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(xmlResponse);
        //    XmlNode root = xmlDoc.DocumentElement;

        //    var exception1 = root.SelectSingleNode("/OrderResponse/Exception");
        //    var exception2 = root.SelectSingleNode("/OrderResponse/MapicsModel/Exception");

        //    if (exception1 != null)
        //    {
        //        this.Response.AddError(exception1.InnerText);
        //    }
        //    else if (exception2 != null)
        //    {
        //        this.Response.AddError(exception2.InnerText);
        //    }
        //    else
        //    {
        //        var status = root.SelectSingleNode("/OrderResponse/MapicsModel/BomCreation/Status");
        //        if (status != null)
        //        {
        //            if (status.InnerText.ToLower() == "successful" || status.InnerText.ToLower() == "created" || status.InnerText.ToLower() == "pending")
        //            {
        //                this.Response.AddSuccess("Order has been submitted successfully");
        //            }
        //            else
        //            {
        //                var error = root.SelectSingleNode("/OrderResponse/MapicsModel/BomCreation/Error");
        //                this.Response.AddError(error.InnerText);
        //            }

        //        }
        //    }

        //    return this.Response;
        //}

        //TODO: deprecated, delete after May, 01, 2018 - Huy Nguyen
        //public string BuildSubmittalOrderXMLString(long orderId)
        //{
        //    var resp = GetOrderModel(orderId);
        //    var orderModel = resp.Model as OrderViewModelLight;
        //    var configOrderNumber = GetConfiguredOrderNumber();
        //    var ShipToAddress = this.Db.GetAddressByAddressId(orderModel.ShipToAddressId);

        //    //=======GET SUBMITTAL ORDER INFO======
        //    SubmittalOrder SubmittalOrder = new SubmittalOrder()
        //    {
        //        CompanyNumber = "01",
        //        CustomerNumber = orderModel.ERPAccountId,
        //        ConfigOrder = configOrderNumber.ToString(),
        //        SalesOrder = new SalesOrder()
        //        {
        //            PONumber = orderModel.PONumber,
        //            PODate = orderModel.SubmitDate.ToString("yyyy-MM-dd"),
        //            RequestDate = orderModel.EstimatedDeliveryDate.ToString("yyyy-MM-dd"),
        //            OrderType = "PC",// get from LCST
        //            ShipTo = new ShipTo()
        //            {
        //                Name = orderModel.ShipToName,
        //                Address1 = ShipToAddress.AddressLine1,
        //                Address2 = ShipToAddress.AddressLine2,
        //                Address3 = ShipToAddress.AddressLine3,
        //                State = ShipToAddress.State.Code,
        //                City = ShipToAddress.Location,
        //                Zip = ShipToAddress.PostalCode
        //            },
        //            Comments = orderModel.Comments,
        //            BusinessID = (long)orderModel.BusinessId,
        //            ProjectID = orderModel.ProjectId,
        //            QuoteID = orderModel.QuoteId,
        //            LineItems = new List<LineItem>()
        //        },
        //        MfgOrder = new MfgOrder()
        //        {
        //            //OrderType = "Customer",
        //            Models = new List<MfgModel>()
        //        }
        //    };

        //    //=============BUILD SALESORDER LINE ITEMS=========

        //    var orderItems = GetOrderItemsBrokenDown(orderId).ToList();
        //    var nextLineSequence = orderItems.Select(i => i.LineSequence).Max() + 1;

        //    foreach (var orderItem in orderItems)
        //    {
        //        //Get standard items
        //        if (orderItem.LineItemTypeId == (byte)LineItemTypeEnum.Standard)
        //        {
        //            LineItem lineItem = new LineItem()
        //            {
        //                LineSequence = orderItem.LineSequence * 10,
        //                SKU = orderItem.ProductNumber,
        //                Quantity = (int)orderItem.Quantity,
        //                Price = orderItem.NetPrice,
        //                Discount = orderItem.DiscountPercentage,
        //                CodeString = orderItem.CodeString
        //            };

        //            SubmittalOrder.SalesOrder.LineItems.Add(lineItem);
        //        }
        //        //Get configured items (Base Model)
        //        if (orderItem.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
        //        {
        //            LineItem lineItem = new LineItem()
        //            {
        //                LineSequence = orderItem.LineSequence * 10,
        //                SKU = orderItem.CodeString,
        //                Quantity = (int)orderItem.Quantity,
        //                Price = orderItem.NetPrice,
        //                Discount = orderItem.DiscountPercentage,
        //                CodeString = orderItem.CodeString,
        //                BaseModel = orderItem.ProductNumber
        //            };

        //            SubmittalOrder.SalesOrder.LineItems.Add(lineItem);

        //            //===Add MFG MODELS===
        //            MfgModel Model = BuildMfgModel(orderItem);
        //            SubmittalOrder.MfgOrder.Models.Add(Model);
        //        }
        //    }// end of foreach orderItems

        //    XmlSerializer serializer = new XmlSerializer(SubmittalOrder.GetType());

        //    var xmlStringWriter = new System.IO.StringWriter();
        //    serializer.Serialize(xmlStringWriter, SubmittalOrder);

        //    return xmlStringWriter.ToString();
        //}
    }
}
