using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPO.Common;
using DPO.Data;
using DPO.Model.Light;

namespace DPO.Domain
{
    public partial class OrderServices : BaseServices, IOrderServices
    {
        #region Constructor
        HtmlServices htmlService;

        public OrderServices() : base() {
            htmlService = new HtmlServices(this.Context);
        }

        public OrderServices(DPOContext context)
            : base(context)
        {
            htmlService = new HtmlServices(context);
        }
        #endregion 

        #region Emails
        public bool ValidateEmails(List<string> emails)
        {
            var result = this.Db.ValidateEmails(emails);
            return result;
        }

        public List<String> GetInvalidEmails(List<string> emails)
        {
            var InvalidEmails = this.Db.GetInvalidEmails(emails);
            return InvalidEmails;
        }

        public DPO.Model.Light.OrderSendEmailModel getOrderSendEmailModel(OrderViewModelLight orderVMLight)
        {
            var proj = this.Db.GetProjectOwnerAndBusiness(orderVMLight.ProjectId);
            var business = this.Db.GetBusinessByProjectOwner(orderVMLight.ProjectId);
            orderVMLight.ProjectOwner = proj.Owner.FirstName + " " + proj.Owner.LastName;
            orderVMLight.BusinessName = business.BusinessName;

            //hacking code for ERP Invoice Date and ERP Order Date.will need to discuss about these to know when we can get these two values
            if (orderVMLight.ERPOrderDate == null)
            {
                orderVMLight.ERPOrderDate = DateTime.Now;
            }
            else
            {
                if (orderVMLight.ERPOrderDate == DateTime.MinValue)
                {
                    orderVMLight.ERPOrderDate = DateTime.Now;
                }
            }

            if (orderVMLight.ERPInvoiceDate == null)
            {
                orderVMLight.ERPInvoiceDate = DateTime.Now;
            }

            return new DPO.Model.Light.OrderSendEmailModel
            {
                order = orderVMLight,
                AccountManagerEmail = business.AccountManagerEmail,
                AccountOwnerEmail = business.AccountOwnerEmail,
                RequestDiscountPercent = orderVMLight.RequestedDiscountPercentage,
                ApprovedDiscountPercent = orderVMLight.ApprovedDiscountPercentage,
                DARAttachmentFile = (orderVMLight.DiscountRequestId != null) ? orderVMLight.DARAttachmentFileName : null,
                COMAttachmentFile = (orderVMLight.CommissionRequestId != null) ? orderVMLight.COMAttachmentFileName : null
            };
        }
        #endregion Emails

        #region Get Orders
        public ServiceResponse GetOrderModel(UserSessionModel user, OrderViewModelLight qto)
        {
            OrderViewModelLight model = null;

            if (!string.IsNullOrEmpty(qto.QuoteId.ToString()))
            {
                var query = from order in this.Db.QueryOrdersViewableByUser(user)

                            join mod in this.Db.Users on order.UpdatedByUserId equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()
                            join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                            join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                            join owner in this.Db.Users on project.OwnerId equals owner.UserId
                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                            join ort in this.Context.OrderAttachments on order.OrderId equals ort.OrderId
                            where order.OrderId == qto.OrderId
                            select new OrderViewModelLight
                            {
                                OrderId = order.OrderId,
                                ProjectId = order.Quote.ProjectId,
                                QuoteId = order.QuoteId,
                                ProjectOwner = owner.FirstName + " " + owner.LastName,
                                ProjectOwnerId = owner.UserId,
                                BusinessId = owner.BusinessId.Value,
                                BusinessName = business.BusinessName,
                                OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                                CreatedByUserId = mod.UserId,
                                UpdatedByUserId = mod.UserId,
                                UpdatedByUser = mod.FirstName + " " + mod.LastName,
                                DiscountRequestId = order.DiscountRequestId,
                                CommissionRequestId = order.CommissionRequestId,
                                ShipToAddressId = order.ShipToAddressId,
                                PricingTypeId = order.PricingTypeId,
                                PONumber = order.PONumber,
                                TotalDiscountPercent = order.TotalDiscountPercent,
                                Comments = order.Comments,
                                //EstimatedReleaseDate = order.EstimatedReleaseDate,
                                EstimatedDeliveryDate = (project.EstimatedDelivery != null) ? project.EstimatedDelivery : DateTime.Now,
                                DeliveryAppointmentRequired = order.DeliveryAppointmentRequired,
                                DeliveryContactName = order.DeliveryContactName,
                                DeliveryContactPhone = order.DeliveryContactPhone,
                                SubmittedByUserId = mod.UserId,
                                //SubmitDate = (order.SubmitDate != null) ? order.SubmitDate : DateTime.Now,
                                SubmitDate = (DateTime)order.SubmitDate,

                                ProjectDate = (project.ProjectDate != null) ? project.ProjectDate : DateTime.Now,
                                ERPOrderDate = (order.ERPOrderDate != null) ? order.ERPOrderDate.Value : (System.DateTime?)null,
                                ERPInvoiceNumber = (order.ERPInvoiceNumber != null) ? order.ERPInvoiceNumber : null,
                                ERPComments = (order.ERPComment != null) ? order.ERPComment : null,
                                ERPPOKey = (order.ERPPOKey != null) ? order.ERPPOKey.Value : (int?)null,
                                ERPStatus = (order.ERPStatus != null) ? order.ERPStatus : null,
                                POAttachmentFileName = ort.FileName,
                                Timestamp = (order.Timestamp != null) ? order.Timestamp : DateTime.Now
                            };

                model = query.FirstOrDefault();
            }

            if (model == null)
            {
                model = new OrderViewModelLight
                {
                    OrderStatusTypeId = OrderStatusTypeEnum.NewRecord,
                    QuoteId = qto.QuoteId
                };
            }

            finaliseModelSvc.FinaliseOrderModel(this.Response.Messages, user, model);
            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetOrderModel(long orderId)
        {
            OrderViewModelLight model = null;

            if (!string.IsNullOrEmpty(orderId.ToString()))
            {
                var query = from order in this.Context.Orders

                            join mod in this.Db.Users on order.UpdatedByUserId equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()
                            join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                            join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                            join owner in this.Db.Users on project.OwnerId equals owner.UserId
                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                            join ort in this.Context.OrderAttachments on order.OrderId equals ort.OrderId
                            where order.OrderId == orderId
                            select new OrderViewModelLight
                            {
                                OrderId = order.OrderId,
                                ProjectId = order.Quote.ProjectId,
                                QuoteId = order.QuoteId,
                                ProjectOwner = owner.FirstName + " " + owner.LastName,
                                ProjectOwnerId = owner.UserId,
                                BusinessId = owner.BusinessId.Value,
                                BusinessName = business.BusinessName,
                                OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                                CreatedByUserId = mod.UserId,
                                UpdatedByUserId = mod.UserId,
                                UpdatedByUser = mod.FirstName + " " + mod.LastName,
                                DiscountRequestId = order.DiscountRequestId,
                                CommissionRequestId = order.CommissionRequestId,
                                ShipToAddressId = order.ShipToAddressId,
                                ShipToName = project.ShipToName,

                                PricingTypeId = order.PricingTypeId,
                                PONumber = order.PONumber,
                                TotalDiscountPercent = order.TotalDiscountPercent,
                                Comments = order.Comments,
                                //EstimatedReleaseDate = order.EstimatedReleaseDate,
                                EstimatedDeliveryDate = (project.EstimatedDelivery != null) ? project.EstimatedDelivery : DateTime.Now,
                                DeliveryAppointmentRequired = order.DeliveryAppointmentRequired,
                                DeliveryContactName = order.DeliveryContactName,
                                DeliveryContactPhone = order.DeliveryContactPhone,
                                SubmittedByUserId = mod.UserId,
                                //SubmitDate = (order.SubmitDate != null) ? order.SubmitDate : DateTime.Now,
                                SubmitDate = (DateTime)order.SubmitDate,

                                ProjectDate = (project.ProjectDate != null) ? project.ProjectDate : DateTime.Now,
                                ERPOrderDate = (order.ERPOrderDate != null) ? order.ERPOrderDate.Value : (System.DateTime?)null,
                                ERPInvoiceNumber = (order.ERPInvoiceNumber != null) ? order.ERPInvoiceNumber : null,
                                ERPComments = (order.ERPComment != null) ? order.ERPComment : null,
                                ERPPOKey = (order.ERPPOKey != null) ? order.ERPPOKey.Value : (int?)null,
                                ERPStatus = (order.ERPStatus != null) ? order.ERPStatus : null,
                                ERPAccountId = business.ERPAccountId,

                                POAttachmentFileName = ort.FileName,
                                Timestamp = (order.Timestamp != null) ? order.Timestamp : DateTime.Now
                            };

                model = query.FirstOrDefault();
            }

            if (model == null)
            {
                this.Response.Messages.AddError("Order not found");
                return this.Response;
            }
            else
            {
                this.Response.Model = model;
            }

            return this.Response;
        }

        public ServiceResponse GetOrderInQuote(UserSessionModel user, long projectId, long quoteId)
        {
            OrderViewModel model = null;

            if (!string.IsNullOrEmpty(projectId.ToString()) &&
                !string.IsNullOrEmpty(quoteId.ToString()))
            {
                var query = from order in this.Db.QueryOrdersViewableByUser(user)

                            join mod in this.Db.Users on order.UpdatedByUserId equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()
                            join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                            where project.ProjectId == projectId
                            join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                            where quote.QuoteId == quoteId
                            join owner in this.Db.Users on project.OwnerId equals owner.UserId
                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                            join ort in this.Context.OrderAttachments on order.OrderId equals ort.OrderId
                            select new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                ProjectId = projectId,
                                QuoteId = quoteId,
                                ProjectOwner = owner.FirstName + " " + owner.LastName,
                                ProjectOwnerId = owner.UserId,
                                BusinessId = owner.BusinessId.Value,
                                BusinessName = business.BusinessName,
                                OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                                CreatedByUserId = mod.UserId,
                                UpdatedByUserId = mod.UserId,
                                UpdatedByUser = mod.FirstName + " " + mod.LastName,
                                DiscountRequestId = order.DiscountRequestId,
                                CommissionRequestId = order.CommissionRequestId,
                                ShipToAddressId = order.ShipToAddressId,
                                PricingTypeId = order.PricingTypeId,
                                PONumber = order.PONumber,
                                TotalDiscountPercent = order.TotalDiscountPercent,
                                Comments = order.Comments,
                                //EstimatedReleaseDate = order.EstimatedReleaseDate,
                                EstimatedDeliveryDate = (project.EstimatedDelivery != null) ? project.EstimatedDelivery : DateTime.Now,
                                DeliveryAppointmentRequired = order.DeliveryAppointmentRequired,
                                DeliveryContactName = order.DeliveryContactName,
                                DeliveryContactPhone = order.DeliveryContactPhone,
                                SubmittedByUserId = mod.UserId,
                                SubmitDate = (order.SubmitDate != null) ? order.SubmitDate : DateTime.Now,
                                ProjectDate = (project.ProjectDate != null) ? project.ProjectDate : DateTime.Now,
                                ERPOrderDate = (order.ERPOrderDate != null) ? order.ERPOrderDate.Value : (System.DateTime?)null,
                                ERPInvoiceNumber = (order.ERPInvoiceNumber != null) ? order.ERPInvoiceNumber : null,
                                ERPComments = (order.ERPComment != null) ? order.ERPComment : null,
                                ERPPOKey = (order.ERPPOKey != null) ? order.ERPPOKey.Value : (int?)null,
                                ERPStatus = (order.ERPStatus != null) ? order.ERPStatus : null,
                                POAttachmentFileName = ort.FileName,
                                TotalNetPrice = quote.TotalNet,
                                OrderReleaseDate = order.OrderReleaseDate,
                                Timestamp = (order.Timestamp != null) ? order.Timestamp : DateTime.Now,
                                ShipToName = project.ShipToName,
                                CustomerName = project.DealerContractorName,
                                SellerName = project.SellerName,
                                EngineerName = project.EngineerName
                            };

                model = query.FirstOrDefault();
            }

            if (model == null)
            {
                this.Response.Messages.AddError("Order not found");
                return this.Response;
            }
            else
            {
                finaliseModelSvc.FinaliseOrderModel(user, model);
                this.Response.Model = model;
            }

            return this.Response;
        }

        public ServiceResponse GetOrderListViewModel(UserSessionModel user, OrderViewModel orderVM)
        {
            List<OrderViewModel> orders = null;

            if (!string.IsNullOrEmpty(orderVM.QuoteId.ToString()))
            {
                var query = from order in this.Db.QueryOrdersViewableByUser(user)

                            join mod in this.Db.Users on order.UpdatedByUserId equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()
                            join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                            join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                            join owner in this.Db.Users on project.OwnerId equals owner.UserId
                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                            join ort in this.Context.OrderAttachments on order.OrderId equals ort.OrderId
                            //where order.OrderId == orderVM.OrderId
                            where order.QuoteId == orderVM.QuoteId
                            select new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                ProjectId = project.ProjectId,
                                QuoteId = order.QuoteId,
                                ProjectOwner = owner.FirstName + " " + owner.LastName,
                                ProjectOwnerId = owner.UserId,
                                BusinessId = owner.BusinessId.Value,
                                BusinessName = business.BusinessName,
                                OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                                CreatedByUserId = mod.UserId,
                                UpdatedByUserId = mod.UserId,
                                UpdatedByUser = mod.FirstName + " " + mod.LastName,
                                DiscountRequestId = order.DiscountRequestId,
                                CommissionRequestId = order.CommissionRequestId,
                                ShipToAddressId = order.ShipToAddressId,
                                PricingTypeId = order.PricingTypeId,
                                PONumber = order.PONumber,
                                TotalDiscountPercent = order.TotalDiscountPercent,
                                Comments = order.Comments,
                                EstimatedReleaseDate = order.EstimatedReleaseDate,
                                EstimatedDeliveryDate = (project.EstimatedDelivery != null) ? project.EstimatedDelivery : DateTime.Now,
                                DeliveryAppointmentRequired = order.DeliveryAppointmentRequired,
                                DeliveryContactName = order.DeliveryContactName,
                                DeliveryContactPhone = order.DeliveryContactPhone,
                                SubmittedByUserId = mod.UserId,
                                SubmitDate = (order.SubmitDate != null) ? order.SubmitDate : DateTime.Now,
                                ProjectDate = (project.ProjectDate != null) ? project.ProjectDate : DateTime.Now,
                                ERPOrderDate = (order.ERPOrderDate != null) ? order.ERPOrderDate.Value : (System.DateTime?)null,
                                ERPInvoiceNumber = (order.ERPInvoiceNumber != null) ? order.ERPInvoiceNumber : null,
                                ERPComments = (order.ERPComment != null) ? order.ERPComment : null,
                                ERPPOKey = (order.ERPPOKey != null) ? order.ERPPOKey.Value : (int?)null,
                                ERPStatus = (order.ERPStatus != null) ? order.ERPStatus : null,
                                POAttachmentFileName = ort.FileName,
                                TotalNetPrice = quote.TotalNet,
                                OrderReleaseDate = order.OrderReleaseDate,
                                Timestamp = (order.Timestamp != null) ? order.Timestamp : DateTime.Now
                            };

                orders = query.ToList();
            }

            if (orders == null)
            {
                this.Response.Messages.AddError("Order not found");
                return this.Response;
            }
            else
            {
                //FinaliseModel(user, model);
                this.Response.Model = orders;
            }

            return this.Response;
        }

        //TODO: Not being used?
        public ServiceResponse GetOrderListModel(UserSessionModel user, SearchOrders search)
        {
            Log.InfoFormat("Enter GetOrderListModel for user: {0}, searchValue: {1}",
                           user.Email, search.ToString());

            search.ReturnTotals = true;

            Log.Debug("Start getting list of OrderViewModel");

            var query = from order in this.Db.QueryOrderViewableBySearch(user, search)
                        join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                        join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                        join owner in this.Db.Users on project.OwnerId equals owner.UserId
                        join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                        select new OrderViewModel
                        {
                            OrderId = order.OrderId,
                            ProjectOwner = owner.FirstName + " " + owner.LastName,
                            ProjectOwnerId = owner.UserId,
                            BusinessId = owner.BusinessId ?? 0,
                            ProjectId = order.Quote.ProjectId,
                            QuoteId = order.QuoteId,

                            Project = new ProjectModel
                            {
                                Name = project.Name,
                                ActiveQuoteSummary = new QuoteListModel { Title = quote.Title }
                            },

                            SubmitDate = order.SubmitDate,
                            OrderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                            //OrderStatusTypeDescription = order.OrderStatusType.Name,
                            OrderReleaseDate = order.OrderReleaseDate,
                            Timestamp = order.Timestamp
                        };

            Log.DebugFormat("OrderViewModel items count: {0}", query.Count());
            this.Response.Model = query.ToList();
            Log.InfoFormat("Finished Exceute GetOrderViewModel");

            return this.Response;
        }

        public ServiceResponse GetOrdersForGrid(UserSessionModel user, SearchOrders search)
        {
            search.ReturnTotals = true;

            //var query = from order in this.Context.Orders
            var query = from order in this.Db.QueryOrderViewableBySearch(user, search)// TODO: this QueryOrderViewableBySearch is very slow. It can cause time out error with super user
                        join project in this.Db.Projects on order.Quote.ProjectId equals project.ProjectId
                        join quote in this.Db.Quotes on order.QuoteId equals quote.QuoteId
                        join discountRequest in this.Context.DiscountRequests
                          on new { project.ProjectId, quote.QuoteId } equals new { discountRequest.ProjectId, discountRequest.QuoteId } into Dr
                        from discountRequest in Dr.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()
                        join commissionRequest in this.Context.CommissionRequests
                           on new { project.ProjectId, quote.QuoteId } equals new { commissionRequest.ProjectId, commissionRequest.QuoteId } into Cr
                        from commissionRequest in Cr.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()
                        select new OrderGridViewModel
                        {
                            projectId = project.ProjectId,
                            projectName = project.Name,
                            orderId = order.OrderId,
                            poNumber = order.PONumber,
                            erpOrderNumber = order.ERPOrderNumber,
                            poAttachmentName = order.OrderAttachments.FirstOrDefault().FileName,
                            projectOwnerName = project.Owner.FirstName + (project.Owner.MiddleName != null ? " " + project.Owner.MiddleName : "") + " " + project.Owner.LastName,
                            dealerContractorName = project.DealerContractorName,
                            orderStatusTypeId = (OrderStatusTypeEnum)order.OrderStatusTypeId,
                            quoteId = quote.QuoteId,
                            activeQuoteTitle = quote.Title,
                            businessId = project.Owner.BusinessId,
                            businessName = project.Owner.Business.BusinessName,
                            projectDate = project.ProjectDate,
                            submitDate = order.SubmitDate,
                            estimatedDeliveryDate = project.EstimatedDelivery,
                            estimatedReleaseDate = order.EstimatedReleaseDate,
                            ERPShipDate = order.ERPShipDate,
                            pricingTypeId = quote.IsCommission ? (byte)2 : (byte)1,
                            totalNetPrice = quote.TotalNet,
                            totalListPrice = quote.TotalList,
                            totalSellPrice = quote.TotalSell,
                            //totalDiscountPercent = quote.DiscountPercentage,
                            totalDiscountPercent = quote.ApprovedDiscountPercentage,
                            darComStatus = quote.IsCommission ?
                                                (commissionRequest != null ? commissionRequest.CommissionRequestStatusType.Description : "") :
                                                (discountRequest != null ? discountRequest.DiscountRequestStatusType.Description : ""),
                            vrvODUcount = quote.VRVOutdoorCount,
                            splitODUcount = quote.TotalCountSplitOutdoor,
                            submittedByUserId = user.UserId,
                            submittedByUserName = user.LastName + ", " + user.FirstName,
                            createdByUserId = user.UserId,
                            updatedByUserId = user.UserId,
                            OrderReleaseDate = order.OrderReleaseDate
                        };

            this.Response.Model = query.ToList();

            return this.Response;
        }

        public Order GetEntity(UserSessionModel user, OrderViewModelLight model)
        {
            Order entity = null;

            if (!string.IsNullOrEmpty(model.OrderId.ToString()) && model.OrderId != 0)
            {
                entity = this.Db.QueryOrdersViewableByUser(user).FirstOrDefault(o => o.OrderId == model.OrderId);
            }
            else
            {
                entity = Db.OrderCreate(model.ProjectId, model.QuoteId);
            }

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        public long GetQuoteIdByOrder(long OrderId)
        {
            var quoteId = (from o in this.Context.Orders
                           where o.OrderId == OrderId
                           select o.QuoteId).First();
            return quoteId;
        }

        public long GetOrderId(long quoteId)
        {
            var orderId = this.Db.Context.Orders.Where(o => o.QuoteId == quoteId).Select(o => o.OrderId).FirstOrDefault();
            return orderId;
        }
        #endregion
    }
}
