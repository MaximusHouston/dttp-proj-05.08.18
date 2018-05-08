using System;
using System.Linq;
using DPO.Domain;
using DPO.Model.Light;
using DPO.Common;

namespace DPO.Services.Light
{
    public class OrderServiceLight : BaseServices, IOrderServiceLight
    {
        public OrderServiceLight() : base() { }

        public ProjectServices projectService = new ProjectServices();
        public QuoteServices quoteService = new QuoteServices();

        #region Get Order
        public ServiceResponse GetNewOrder(UserSessionModel user, long quoteId)
        {
            var queryERPAccountId = (from business in this.Context.Businesses
                                     select business).Where(b => b.BusinessId == user.BusinessId);

            var ERPAccountId = queryERPAccountId.FirstOrDefault().ERPAccountId;

            var query = from quote in this.Context.Quotes
                        join project in this.Context.Projects on quote.ProjectId equals project.ProjectId
                        where quote.QuoteId == quoteId
                        select new OrderViewModelLight
                        {
                            OrderStatusTypeId = OrderStatusTypeEnum.NewRecord,
                            ProjectId = project.ProjectId,
                            ProjectName = project.Name,
                            QuoteId = quote.QuoteId,
                            DiscountRequestId = quote.DiscountRequestId,
                            QuoteTitle = quote.Title,
                            BusinessId = project.Owner.BusinessId, 
                            BusinessName = project.Owner.Business.BusinessName,
                            AccountID = project.Owner.Business.AccountId,
                            ShipToAddressId = project.ShipToAddressId,
                            ShipToName = project.ShipToName,
                            PricingTypeId = quote.IsCommission ? (byte)2 : (byte)1,
                            TotalNetPrice = quote.TotalNet,
                            //TotalDiscountPercent = quote.DiscountPercentage,
                            TotalDiscountPercent = quote.ApprovedDiscountPercentage,
                            ERPAccountId = ERPAccountId,
                            EstimatedDeliveryDate = project.EstimatedDelivery,
                            SubmitDate = DateTime.Now,
                            SubmittedByUserId = user.UserId,
                            SubmittedByUserName = user.LastName + ", " + user.FirstName,
                            CreatedByUserId = user.UserId,
                            UpdatedByUserId = user.UserId,
                            Timestamp = quote.Timestamp
                        };

            var model = query.FirstOrDefault();
            model.EstimatedReleaseDate = model.EstimatedDeliveryDate.AddDays(-10);

            model.Project = projectService.GetProjectModel(user, model.ProjectId).Model as ProjectModel;
            model.HasConfiguredModel = quoteService.HasConfiguredModel(model.QuoteId);

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        public void InsertProjectInfoToMapics(OrderViewModelLight model)
        {
            var project = new ProjectInfo
            {
                BusinessId = model.BusinessId.GetValueOrDefault(),
                BusinessName = model.BusinessName,
                ERPAccountId = !string.IsNullOrEmpty(model.ERPAccountId) ?
                                    Convert.ToInt32(model.ERPAccountId) : 0,
                ProjectId = model.ProjectId,
                ProjectName = model.ProjectName,
                AccountId = model.AccountID,
                QuoteId = model.QuoteId
            };

            var erpClient = new ERPClient();
            erpClient.PostProjectsInfoToMapicsAsync(project);
        }

        #region Get Submited Order
        public ServiceResponse GetSubmittedOrder(UserSessionModel user, long quoteId)
        {
            var queryExistedOrder = from order in this.Context.Orders
                                    where order.QuoteId == quoteId
                                    select order;

            var existedOrder = queryExistedOrder.FirstOrDefault();

            if (existedOrder != null) // view 
            {
                var query = from order in this.Context.Orders
                            join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
                            join project in this.Context.Projects on quote.ProjectId equals project.ProjectId
                            join attachment in this.Context.OrderAttachments on order.OrderId equals attachment.OrderId
                            where order.QuoteId == quoteId
                            select new OrderViewModelLight
                            {
                                OrderId = order.OrderId,
                                ProjectId = project.ProjectId,
                                ProjectName = project.Name,
                                QuoteId = quote.QuoteId,
                                DiscountRequestId = quote.DiscountRequestId,
                                QuoteTitle = quote.Title,
                                BusinessId = project.Owner.BusinessId,
                                //ERPAccountId = project.Owner.Business.ERPAccountId,
                                ShipToAddressId = project.ShipToAddressId,
                                PricingTypeId = quote.IsCommission ? (byte)2 : (byte)1,
                                TotalNetPrice = quote.TotalNet,
                                //TotalDiscountPercent = quote.DiscountPercentage,
                                TotalDiscountPercent = quote.ApprovedDiscountPercentage,
                                Comments = order.Comments,
                                EstimatedDeliveryDate = project.EstimatedDelivery,
                                OrderReleaseDate = order.OrderReleaseDate,
                                PONumber = order.PONumber,
                                DeliveryAppointmentRequired = order.DeliveryAppointmentRequired,
                                DeliveryContactName = order.DeliveryContactName,
                                DeliveryContactPhone = order.DeliveryContactPhone,
                                SubmitDate = DateTime.Now,
                                SubmittedByUserId = user.UserId,
                                SubmittedByUserName = user.LastName + ", " + user.FirstName,
                                CreatedByUserId = user.UserId,
                                UpdatedByUserId = user.UserId,
                                ProjectDate = project.ProjectDate,
                                POAttachmentFileName = attachment.FileName,
                                OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                                RequestedDiscountPercentage = (!string.IsNullOrEmpty(quote.DiscountRequests.FirstOrDefault().RequestedDiscount.ToString())) ?
                                                               quote.DiscountRequests.FirstOrDefault().RequestedDiscount : 0,
                                ApprovedDiscountPercentage = (quote.DiscountRequests.FirstOrDefault().ApprovedDiscount != null) ?
                                                              quote.DiscountRequests.FirstOrDefault().ApprovedDiscount.Value : 0,
                                Timestamp = quote.Timestamp
                            };
                OrderViewModelLight model = query.FirstOrDefault();
                model.EstimatedReleaseDate = model.EstimatedDeliveryDate.AddDays(-10);
                model.Project = projectService.GetProjectModel(user, model.ProjectId).Model as ProjectModel;

                this.Response.Model = model;
            }

            return this.Response;
        }
        #endregion

        public ServiceResponse GetOrderInQuote(UserSessionModel user, long quoteId)
        {
            var query = from order in this.Context.Orders
                        join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
                        join project in this.Context.Projects on quote.ProjectId equals project.ProjectId
                        join orderAttachment in this.Context.OrderAttachments on order.OrderId equals orderAttachment.OrderId
                        where order.QuoteId == quoteId
                        select new OrderViewModelLight
                        {
                            OrderId = order.OrderId,
                            PONumber = order.PONumber,
                            OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                            ProjectId = project.ProjectId,
                            ProjectName = project.Name,
                            QuoteId = quote.QuoteId,
                            QuoteTitle = quote.Title,
                            BusinessId = project.Owner.BusinessId,
                            ShipToAddressId = project.ShipToAddressId,
                            ERPShipDate = (order.ERPShipDate != null && order.ERPShipDate != DateTime.MinValue) ? order.ERPShipDate : (System.DateTime?)null,
                            PricingTypeId = quote.IsCommission ? (byte)2 : (byte)1,
                            TotalNetPrice = quote.TotalNet,
                            //TotalDiscountPercent = quote.DiscountPercentage,
                            TotalDiscountPercent = quote.ApprovedDiscountPercentage,
                            Comments = project.ProjectStatusNotes,
                            EstimatedReleaseDate = order.EstimatedReleaseDate,
                            SubmitDate = DateTime.Now,
                            SubmittedByUserId = user.UserId,
                            SubmittedByUserName = user.LastName + ", " + user.FirstName,
                            CreatedByUserId = user.UserId,
                            UpdatedByUserId = user.UserId,
                            OrderReleaseDate = order.OrderReleaseDate,
                            POAttachmentFileName = order.OrderAttachments.FirstOrDefault().FileName,
                            Timestamp = quote.Timestamp
                        };

            this.Response.Model = query.ToList();

            return this.Response;
        }

        public ServiceResponse GetOrderStatusTypes(UserSessionModel user)
        {
            var query = from OrderStatus in this.Context.OrderStatusTypes
                        where OrderStatus.Name != "New Record"
                        select new LookupItem
                        {
                            KeyId = OrderStatus.OrderStatusTypeId,
                            DisplayText = OrderStatus.Name
                        };
            this.Response.Model = query.ToList();
            return this.Response;
        }


        public ServiceResponse GetOrderOptions(UserSessionModel user, long? projectId, long? currentQuoteId)
        {
            ServiceResponse responese = new ServiceResponse();
            OrderServices orderservices = new OrderServices();
            OrderOptionsModel orderOptions = new OrderOptionsModel();

            //orderOptions = GetOrderOptionsModel(user, projectId, currentQuoteId);
            orderOptions = orderservices.GetOrderOptionsModel(user, projectId, currentQuoteId);

            responese.Model = orderOptions;
            return responese;

            #region old code, remove after Dec/31/2017
            //OrderOptions orderOptions = new OrderOptions();

            ////========= No ShowPrices ================
            //if (user.ShowPrices == false)
            //{
            //    orderOptions.CanSubmitOrder = false;
            //    orderOptions.CanViewSubmittedOrder = false;
            //    orderOptions.CanViewOrders = false;

            //    responese.Model = orderOptions;
            //    return responese;

            //}

            //if (user.HasAccess(SystemAccessEnum.SubmitOrder))
            //{
            //    //============Submit Order is not allowed when there is no products in quote================
            //    var queryQuoteItems = from quoteItem in this.Context.QuoteItems
            //                          where quoteItem.QuoteId == currentQuoteId
            //                          select quoteItem;
            //    var productList = queryQuoteItems.ToList();
            //    if (productList == null || productList.Count == 0)
            //    {
            //        orderOptions.CanSubmitOrder = false;
            //        orderOptions.CanViewSubmittedOrder = false;

            //        responese.Model = orderOptions;
            //        return responese;
            //    }
            //    //==========================================================================================

            //    //============Hide Submit Order button when quote is Commission=================
            //    var queryQuote = from quote in this.Context.Quotes
            //                     where quote.QuoteId == currentQuoteId
            //                     select quote;

            //    var currentquote = queryQuote.FirstOrDefault();
            //    if (currentquote.IsCommission)
            //    {
            //        orderOptions.CanSubmitOrder = false;
            //        orderOptions.CanViewSubmittedOrder = false;
            //        orderOptions.CanViewOrders = true;// can see Orders Tab 


            //        responese.Model = orderOptions;
            //        return responese;
            //    }
            //    //==============================================================================

            //    //============Check Discount Request Status===============================
            //    var queryDARrequest = from dar in this.Context.DiscountRequests
            //                          where dar.QuoteId == currentQuoteId
            //                          select dar;
            //    var quoteDAR = queryDARrequest.FirstOrDefault();
            //    if (quoteDAR != null && quoteDAR.DiscountRequestStatusTypeId == 2)
            //    {//========= DAR pending======
            //        orderOptions.CanSubmitOrder = false;
            //        orderOptions.CanViewSubmittedOrder = false;

            //        responese.Model = orderOptions;
            //        return responese;
            //    }
            //    //========================================================================


            //    //===========Check Commission Request Status==============================
            //    var queryCOMrequest = from com in this.Context.CommissionRequests
            //                          where com.QuoteId == currentQuoteId
            //                          select com;
            //    var quoteCOM = queryCOMrequest.FirstOrDefault();

            //    if (quoteCOM != null && quoteCOM.CommissionRequestStatusTypeId == 2)
            //    {//========= COM pending======
            //        orderOptions.CanSubmitOrder = false;
            //        orderOptions.CanViewSubmittedOrder = false;

            //        responese.Model = orderOptions;
            //        return responese;
            //    }

            //    //========================================================================


            //    //======== DAR/COM approved  or  No DAR/COM ==============================
            //    var queryOrderInProject = from order in this.Context.Orders
            //                              join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
            //                              where quote.ProjectId == projectId
            //                              select order;
            //    var existedOrder = queryOrderInProject.FirstOrDefault();

            //    if (existedOrder != null)/*=== Order existed =====*/
            //    {
            //        if (existedOrder.OrderStatusTypeId == 1 || existedOrder.OrderStatusTypeId == 2 || existedOrder.OrderStatusTypeId == 3) // New Record/ Submitted/ Awaiting CSR
            //        {
            //            orderOptions.CanSubmitOrder = false;
            //        }
            //        else
            //        {
            //            if (existedOrder.OrderStatusTypeId == 4) // Accepted 
            //            {
            //                if (existedOrder.QuoteId == currentQuoteId)
            //                {
            //                    orderOptions.CanSubmitOrder = false;
            //                }
            //                else // this is to allow many orders in one project 
            //                {
            //                    orderOptions.CanSubmitOrder = true;
            //                }
            //            }
            //            else if (existedOrder.OrderStatusTypeId == 5) //InProcess
            //            {
            //                orderOptions.CanSubmitOrder = false;
            //            }
            //            else if (existedOrder.OrderStatusTypeId == 6) //Picked
            //            {
            //                orderOptions.CanSubmitOrder = false;
            //            }
            //            else if (existedOrder.OrderStatusTypeId == 7) //Shipped
            //            {
            //                orderOptions.CanSubmitOrder = false;
            //            }
            //            else if (existedOrder.OrderStatusTypeId == 8)// Canceled
            //            {
            //                orderOptions.CanSubmitOrder = true;
            //            }


            //        }
            //        orderOptions.CanViewSubmittedOrder = true;
            //    }
            //    else                  /*===== Order not existed ====*/
            //    {
            //        orderOptions.CanSubmitOrder = true;
            //        orderOptions.CanViewSubmittedOrder = false;
            //    }

            //    orderOptions.CanViewOrders = true;

            //    responese.Model = orderOptions;
            //    return responese;

            //    //========End of DAR/COM approved  or  No DAR/COM ========================
            //}
            //else /***** when user don't have permission to Submit Order *****/
            //{
            //    orderOptions.CanSubmitOrder = false;
            //    if (user.HasAccess(SystemAccessEnum.ViewOrder))
            //    {
            //        orderOptions.CanViewOrders = true;

            //        var queryOrderInProject = from order in this.Context.Orders
            //                                  join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
            //                                  where quote.ProjectId == projectId
            //                                  select order;
            //        var existedOrder = queryOrderInProject.FirstOrDefault();

            //        if (existedOrder != null)
            //        {
            //            orderOptions.CanViewSubmittedOrder = true;
            //        }
            //        else
            //        {
            //            orderOptions.CanViewSubmittedOrder = false;
            //        }

            //    }
            //    else
            //    { //=== user don't have permission to View Order
            //        orderOptions.CanViewSubmittedOrder = false;
            //        orderOptions.CanViewOrders = false;
            //    }

            //    responese.Model = orderOptions;
            //    return responese;
            //}

            #endregion
        }// end of GetOrderOptions

    }// end of class OrderServiceLight 
}
