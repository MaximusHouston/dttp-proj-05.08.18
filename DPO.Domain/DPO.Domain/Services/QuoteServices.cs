
using CsvHelper;
using DPO.Common;
using DPO.Data;
using DPO.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml;

namespace DPO.Domain
{
    public partial class QuoteServices : BaseServices
    {
        BasketServices basketServices;
        ProductServices productServices = new ProductServices();

        public QuoteServices()
            : base()
        {
            basketServices = new BasketServices();
        }

        public QuoteServices(DPOContext context)
            : base(context)
        {
            basketServices = new BasketServices(context);
        }

        public QuoteServices(BaseServices injectService, string propertyReference) : base()
        {
            this.Response = injectService.Response;
            this.Context = injectService.Context;
            this.Db = injectService.Db;
            this.Response.PropertyReference = propertyReference;
        }

        #region Get Requests

        public long GetProjectIdByQuoteId(UserSessionModel admin, long quoteId)
        {
            long projectId = 0;
            var query = from quote in this.Db.QueryQuoteViewableByQuoteId(admin, quoteId)
                        select quote.ProjectId;

            if (query.Count() > 0)
            {
                projectId = query.FirstOrDefault();
            }
            else
            {
                throw new ArgumentException("Cannot find project Id.");
            }

            return projectId;
        }

        public ServiceResponse GetQuoteListModel(UserSessionModel admin, SearchQuote search)
        {
            Log.InfoFormat("Enter GetQuoteListModel for user: {0}", admin.Email);
            Log.DebugFormat("SearcH filters: {0}", search.Filter);

            search.ReturnTotals = true;

            Log.Debug("Start getting Quotes by search");

            var query = from quote in this.Db.QueryQuotesViewableBySearch(admin, search)
                        select new QuoteListModel
                        {
                            QuoteId = quote.QuoteId,
                            Title = quote.Title,
                            Active = quote.Active,
                            Deleted = quote.Deleted,
                            Alert = quote.RecalculationRequired,
                            ItemCount = quote.QuoteItems.Where(qi => qi.Quantity > 0).Count(),
                            Revision = quote.Revision,
                            TotalSell = quote.TotalSell,
                            TotalNet = quote.TotalNet,
                            TotalList = quote.TotalList,
                            OrderStatusTypeId = (!string.IsNullOrEmpty(quote.Orders.FirstOrDefault().OrderStatusTypeId.ToString())) ?
                                                 (byte)quote.Orders.FirstOrDefault().OrderStatusTypeId : (byte)0,
                            Timestamp = quote.Timestamp,
                            CommissionConvertNo = (quote.CommissionConvertNo.HasValue) ? quote.CommissionConvertNo.Value : false,
                            CommissionConvertYes = (quote.CommissionConvertYes.HasValue) ? quote.CommissionConvertYes.Value : false,
                            IsCommission = quote.IsCommission
                        };

            try
            {
                this.Response.Model = query.ToList();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Exception Source: {0}", ex.Source);
                Log.FatalFormat("Exception: {0}", ex.Message);
                Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
            }

            Log.InfoFormat("GetQuoteListModel finished.");
            return this.Response;

        }
        public ServiceResponse GetQuoteModel(UserSessionModel admin, Quote quote)
        {
            Log.Info("enter GetQuoteModel");
            return GetQuoteModel(admin, (quote == null) ? null : (long?)quote.ProjectId, (quote == null) ? null : (long?)quote.QuoteId);
        }

        public ServiceResponse GetQuoteModel(UserSessionModel admin, long? projectId, long? quoteId)
        {
            Log.Info("enter GetQuoteModel");

            if (!projectId.HasValue) throw new ArgumentException("Project id cannot be empty");

            QuoteModel model = null;

            Log.DebugFormat("QuoteId has value: {0}", quoteId.HasValue);

            if (quoteId.HasValue)
            {
                var query = from q in this.Db.QueryQuoteViewableByQuoteId(admin, quoteId.Value)
                            join p in this.Db.Projects on q.ProjectId equals p.ProjectId

                            join active in this.Db.Quotes on quoteId.Value equals active.QuoteId into Laq
                            from active in Laq.DefaultIfEmpty()

                            join transfer in this.Context.ProjectTransfers on new { admin.UserId, p.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                            from transfer in Lt.DefaultIfEmpty()

                            join order in this.Context.Orders on q.QuoteId equals order.QuoteId into or
                            from order in or.DefaultIfEmpty()

                            join commission in this.Context.CommissionRequests on quoteId.Value equals commission.QuoteId into com
                            from commission in com.DefaultIfEmpty()

                            join discount in this.Context.DiscountRequests on quoteId.Value equals discount.QuoteId into dis
                            from discount in dis.DefaultIfEmpty()

                            select new QuoteModel
                            {
                                QuoteId = q.QuoteId,
                                Active = q.Active,
                                Multiplier = q.Multiplier,

                                IsCommissionSchemeAllowed = p.Owner.Business.CommissionSchemeAllowed,

                                AwaitingDiscountRequest = q.AwaitingDiscountRequest,

                                AwaitingCommissionRequest = q.AwaitingCommissionRequest,

                                IsCommissionScheme = q.IsCommissionScheme,

                                IsGrossMargin = q.IsGrossMargin,

                                IsTransferred = (transfer != null),

                                DiscountRequestId = dis.FirstOrDefault().DiscountRequestId,

                                //CommissionRequestId =  commission.CommissionRequestId,
                                CommissionRequestId = (q.CommissionRequestId == null) ? commission.CommissionRequestId : q.CommissionRequestId,

                                ApprovedCommissionPercentage = q.ApprovedCommissionPercentage,

                                ApprovedDiscountPercentage = q.ApprovedDiscountPercentage,

                                CommissionPercentage = q.CommissionPercentage,

                                IsCommission = q.IsCommission,

                                Description = q.Description,
                                Notes = q.Notes,

                                Revision = q.Revision,
                                Title = q.Title,

                                TotalList = q.TotalList,

                                DiscountPercentage = q.DiscountPercentage,

                                TotalNet = q.TotalNet,

                                TotalSell = q.TotalSell,

                                TotalMisc = q.TotalMisc,

                                TotalFreight = q.TotalFreight,

                                ItemCount = q.QuoteItems.Where(qi => qi.Quantity > 0).Count(),

                                Deleted = q.Deleted,

                                //ActiveQuoteSummary = new QuoteListModel {
                                //    Alert = q.RecalculationRequired
                                //},
                                RecalculationRequired = q.RecalculationRequired,

                                Timestamp = q.Timestamp,

                                CommissionRequestStatusTypeId = (commission.CommissionRequestStatusTypeId == null) ? (byte)0 : commission.CommissionRequestStatusTypeId.Value,
                                DiscountRequestStatusTypeId = (discount != null) ? discount.DiscountRequestStatusTypeId : (byte)0,

                                ProjectId = p.ProjectId,
                                ProjectStatusTypeId = (p == null) ? null : (byte?)p.ProjectStatusTypeId,
                                Project = new ProjectModel
                                {
                                    ProjectId = (p == null) ? (long?)null : p.ProjectId,
                                    Name = (p == null) ? null : p.Name,
                                },

                                CommissionAmount = commission.ApprovedCommissionTotal ?? commission.RequestedCommissionTotal ?? 0,
                                CommissionNetMultiplierValue = commission.RequestedNetMaterialValue ?? 0,
                                TotalNetCommission = (commission.TotalNet) ?? 0,
                                CommissionConvertNo = (q.CommissionConvertNo.HasValue) ? q.CommissionConvertNo.Value : false,
                                CommissionConvertYes = (q.CommissionConvertYes.HasValue) ? q.CommissionConvertYes.Value : false,
                                OrderId = (!string.IsNullOrEmpty(order.OrderId.ToString())) ? order.OrderId : 0,
                                OrderStatusTypeId = (!string.IsNullOrEmpty(order.OrderStatusTypeId.ToString())) ?
                                                     order.OrderStatusTypeId : (byte)0
                            };

                var countOfRows = query.Count();
                model = query.OrderBy(c => 1 == 1).Skip(countOfRows - 1).FirstOrDefault();

                if (model == null)
                {
                    this.Response.AddError(Resources.DataMessages.DM010);
                    Log.Error(this.Response.Messages.Items.Last().Text); // get latest error message
                }
            }

            // Create if not found or new
            if (model == null)
            {
                // Project needed too for both new and existing  quote models

                Log.Debug("start create new QuoteModel");

                model = (from project in this.Db.QueryProjectViewableByProjectId(admin, projectId)
                         select new QuoteModel
                         {
                             Project = new ProjectModel
                             {
                                 ProjectId = project.ProjectId,
                                 Name = project.Name
                             },
                             IsCommissionSchemeAllowed = project.Owner.Business.CommissionSchemeAllowed,
                         })
                                 .FirstOrDefault();

                model.ProjectId = projectId;

                Log.DebugFormat("projectId: {0} ProjectName: {1} IsCommissionSchemeAllowed: {2} ",
                                model.ProjectId, model.Project.Name, model.IsCommissionSchemeAllowed);

            }

            if (model.Project == null)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
                model.Project = new ProjectModel();
                Log.Error(this.Response.Messages.Items.Last().Text);
            }

            OrderServices orderServices = new OrderServices();

            model.OrderOptions = orderServices.GetOrderOptionsModel(admin, projectId, quoteId);

            model.QuoteOptions = GetQuoteOptionsModel(admin, model);

            FinaliseModel(admin, model);

            this.Response.Model = model;

            Log.Info("GetQuoteModel() finished.");

            return this.Response;
        }

        public ServiceResponse GetQuoteItemListModel(UserSessionModel admin, long quoteId)
        {
            return GetQuoteItemListModel(admin, new SearchQuoteItem { QuoteId = quoteId, PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL, ReturnTotals = false });
        }

        public ServiceResponse GetQuoteItemListModel(UserSessionModel admin, SearchQuoteItem search)
        {
            Log.Info("Enter GetQuoteItemListModel");
            Log.DebugFormat("CurrentFilter: {0} IsDesc: {1} QuoteId: {2} QuoteItemId: {3} " +
                            "Totals: {4} PreviousFilter: {5} ",
                           search.Filter, search.IsDesc, search.QuoteId, search.QuoteItemId,
                           search.ReturnTotals, search.PreviousFilter);

            var query = from q in this.Db.QuoteItemsQueryBySearch(admin, search)
                        join p in this.Db.Products on q.ProductId equals p.ProductId into Lp
                        from p in Lp.DefaultIfEmpty()
                        select new QuoteItemListModel
                        {
                            QuoteId = q.QuoteId,
                            ProductId = q.ProductId,
                            PriceNet = q.ListPrice * q.Multiplier,
                            PriceList = q.ListPrice,
                            IsCommissionable = p.AllowCommissionScheme,
                            Quantity = q.Quantity,
                            ProductNumber = q.ProductNumber,
                            Description = q.Description,
                            QuoteItemId = q.QuoteItemId,
                            ProductClassCode = p.ProductClassCode,
                            ProductStatusTypeId = p.ProductStatusId,
                            ProductStatusTypeDescription = p.ProductStatus.Description,
                            InventoryStatusId = p.InventoryStatusId,
                            InventoryStatusDescription = p.InventoryStatuses.Description,
                            InvAvailableDate = p.InvAvailableDate,
                            SubmittalSheetTypeId = (SubmittalSheetTypeEnum)p.SubmittalSheetTypeId,
                            Tags = q.Tags,
                            CodeString = q.CodeString,
                            //QuoteItemTypeId = q.QuoteItemTypeId
                            LineItemTypeId = q.LineItemTypeId
                        };
            try
            {
                this.Response.Model = query.ToList();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }

            Log.Info("GetQuoteItemListModel finished.");

            return this.Response;
        }

        public QuoteItemListModel GetSingleQuoteItemListModel(long QuoteItemId)
        {
            Log.Info("enter GetSingleQuoteItemListModel");
            Log.DebugFormat("Quoteid: {0}", QuoteItemId);

            var query = (from q in this.Db.QuoteItems
                         where q.QuoteItemId == QuoteItemId
                         select new QuoteItemListModel
                         {
                             QuoteId = q.QuoteId,
                             ProductId = q.ProductId,
                             Quantity = q.Quantity,
                             ProductNumber = q.ProductNumber,
                             QuoteItemId = q.QuoteItemId,
                             Tags = q.Tags
                         }).FirstOrDefault();

            Log.Info("GetSingleQuoteItemListModel finished");

            return query;
        }

        public ServiceResponse GetQuoteItemModel(UserSessionModel admin, long? quoteId, long? quoteItemId)
        {
            Log.Info("Enter GetQuoteItemModel(admin, quoteId, quoteItemId) ");
            Log.DebugFormat("userId: {0} quoteId: {1} quoteItemId: {2} ",
                           admin.UserId, quoteId, quoteItemId);

            if (!quoteId.HasValue)
            {
                Log.Error("Quote id is Null.");
                throw new ArgumentException("Quote id cannot be empty");
            }

            QuoteItemModel model = null;

            if (quoteItemId.HasValue)
            {
                Log.DebugFormat("quoteItemId has value");

                var query = from item in this.Db.QuoteItemQueryByQuoteItemId(admin, quoteItemId.Value)
                            select new QuoteItemModel
                            {
                                QuoteItemId = item.QuoteId,
                                AccountMultiplierId = item.AccountMultiplierId,
                                ProductNumber = item.ProductNumber,
                                ListPrice = item.ListPrice,
                                NetPrice = item.ListPrice * item.Multiplier,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                Timestamp = item.Timestamp
                            };

                Log.Debug("return latest quoteItemModel");

                model = query.FirstOrDefault();

                if (model == null)
                {
                    this.Response.AddError(Resources.DataMessages.DM023);
                    Log.Error(this.Response.Messages.Items.Last().Text);
                }
            }

            // Create if not found or new
            if (model == null)
            {
                Log.Debug("create new quoteItemModel");
                model = new QuoteItemModel();
            }

            model.QuoteId = quoteId;

            // quote needed too for both new and existing  quote item models
            model.Quote = (from quote in this.Db.QueryQuoteViewableByQuoteId(admin, quoteId)
                           select new QuoteModel
                           {
                               Title = quote.Title
                           })
                            .FirstOrDefault();

            if (model.Quote == null)
            {
                this.Response.AddError(Resources.DataMessages.DM010);
                Log.Error(this.Response.Messages.Items.Last().Text);
            }

            Log.Info("finalize model");
            FinaliseModel(admin, model);

            this.Response.Model = model;
            Log.Info("GetQuoteItemModel finished");

            return this.Response;
        }

        public QuoteItemsLoadOptions getQuoteItemsLoadOptions(long quoteId) {
            QuoteItemsLoadOptions loadOptions = new QuoteItemsLoadOptions();
            //var quoteItemsQuery = from quoteItem in this.Db.QuoteItems
            //                      where quoteItem.QuoteId == quoteId
            //                      select quoteItem;

            var quoteItems = this.Db.QuoteItems.Where(i => i.QuoteId == quoteId).ToList();
            if (quoteItems.Count > 0) {
                loadOptions.LoadQuoteItems = true;
            }

            var discountRequests = this.Db.DiscountRequests.Where(i => i.QuoteId == quoteId).ToList();
            if (discountRequests.Count > 0)
            {
                loadOptions.LoadDiscountRequests = true;
            }

            var commissionRequests = this.Db.CommissionRequests.Where(i => i.QuoteId == quoteId).ToList();
            if (commissionRequests.Count > 0)
            {
                loadOptions.LoadCommissionRequests = true;
            }

            var orders = this.Db.Orders.Where(i => i.QuoteId == quoteId).ToList();
            if (orders.Count > 0)
            {
                loadOptions.LoadQuoteOrders = true;
            }

            return loadOptions;
        }

        public ServiceResponse GetQuoteItemsModel(UserSessionModel admin, QuoteItemsModel model)
        {
            Log.Info("enter GetQuoteItemsModel");
            Log.DebugFormat("QuoteItems count: {0} ", model.Items.Count);
            Log.DebugFormat("DiscountRequestId: {0} CommissionRequestId: {1}",
                             model.DiscountRequestId.HasValue ? model.DiscountRequestId : 0,
                             model.CommissionRequestId.HasValue ? model.CommissionRequestId : 0);
            Log.DebugFormat("Awaiting Commision: {0} Awaiting Discount: {1} ",
                            model.AwaitingCommissionRequest,
                            model.AwaitingDiscountRequest);

            Log.DebugFormat("loadDiscountRequest: {0} LoadCommissionRequest: {1} ",
                             model.LoadDiscountRequests,
                             model.LoadCommissionRequests
                             );

            var loadQuoteItems = model.LoadQuoteItems;
            var loadDiscountRequests = model.LoadDiscountRequests;
            var loadCommissionRequests = model.LoadCommissionRequests;
            var loadQuoteOrders = model.LoadQuoteOrders;

            var searchQuoteItems = new SearchQuoteItem(model as Search);

            if (model.QuoteId.HasValue)
            {
                Log.Debug("QuoteId is Not Null");

                var query = from quote in this.Db.QueryQuoteViewableByQuoteId(admin, model.QuoteId)
                            join project in this.Db.Projects on quote.ProjectId equals project.ProjectId

                            join active in this.Db.Quotes on model.QuoteId.Value equals active.QuoteId into Laq
                            from active in Laq.DefaultIfEmpty()

                            join transfer in this.Context.ProjectTransfers on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                            from transfer in Lt.DefaultIfEmpty()

                            join commission in this.Context.CommissionRequests on model.QuoteId equals commission.QuoteId into com
                            from commission in com.DefaultIfEmpty()

                            join discount in this.Context.DiscountRequests on model.QuoteId equals discount.QuoteId into dis
                            from discount in dis.DefaultIfEmpty()

                            select new QuoteItemsModel
                            {
                                ProjectId = quote.ProjectId,
                                QuoteId = quote.QuoteId,
                                Title = quote.Title,
                                ProjectName = project.Name,
                                ProjectStatusTypeId = (byte?)project.ProjectStatusTypeId,
                                IsTransferred = (transfer != null),
                                AwaitingDiscountRequest = quote.AwaitingDiscountRequest,
                                AwaitingCommissionRequest = quote.AwaitingCommissionRequest,
                                DiscountRequestId = discount.DiscountRequestId,
                                DiscountRequestAvailable =
                                    !quote.IsCommission
                                    && !(quote.DiscountRequests.Any(w =>
                                        w.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved
                                            || w.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending)),
                                CommissionRequestAvailable =
                                    quote.IsCommission
                                    && !(quote.CommissionRequests.Any(w =>
                                        w.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved
                                        || w.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending)),
                                CommissionRequestId = (quote.CommissionRequestId != null) ? quote.CommissionRequestId : commission.CommissionRequestId,

                                ProjectOwnerCommissionSchemeAllowed = project.Owner.Business.CommissionSchemeAllowed,
                                Deleted = quote.Deleted,
                                Active = quote.Active,
                                OrderStatusTypeId = (!string.IsNullOrEmpty(quote.Orders.FirstOrDefault().OrderId.ToString()))
                                                    ? quote.Orders.FirstOrDefault().OrderStatusTypeId : (byte)0,
                                IsCommissionRequest = (commission.IsCommissionCalculation == null) ? false : !commission.IsCommissionCalculation.Value,
                                IsCommission = quote.IsCommission,
                                CommissionConvertNo = (quote.CommissionConvertNo == null) ? true : quote.CommissionConvertNo.Value,
                                CommissionConvertYes = (quote.CommissionConvertYes == null) ? false : quote.CommissionConvertYes.Value,
                                CommissionRequestStatusTypeId = (commission.CommissionRequestStatusTypeId == null) ? (byte)0 : commission.CommissionRequestStatusTypeId.Value,

                                ActiveQuoteSummary = new QuoteListModel
                                {
                                    ProjectId = project.ProjectId,
                                    QuoteId = (quote == null) ? 0 : active.QuoteId,
                                    Alert = (quote == null) ? false : quote.RecalculationRequired,
                                    Title = (quote == null) ? "" : active.Title,
                                    Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                    TotalList = (quote == null) ? 0 : active.TotalList,
                                    TotalMisc = (quote == null) ? 0 : active.TotalMisc,
                                    TotalNet = (quote == null) ? 0 : active.TotalNet,
                                    TotalSell = (quote == null) ? 0 : active.TotalSell,
                                    Revision = (quote == null) ? 0 : active.Revision,
                                    CommissionAmount = commission.ApprovedCommissionTotal ?? commission.RequestedCommissionTotal ?? 0,
                                    NetMultiplierValue = commission.RequestedNetMaterialValue ?? 0,
                                    TotalNetCommission = commission.TotalNet ?? 0,
                                    IsCommission = (string.IsNullOrEmpty(quote.IsCommission.ToString())) ? false : quote.IsCommission,
                                    CommissionConvertNo = (quote.CommissionConvertNo == null) ? true : quote.CommissionConvertNo.Value,
                                    CommissionConvertYes = (quote.CommissionConvertYes == null) ? false : quote.CommissionConvertYes.Value,
                                    CommissionRequestStatusTypeId = (commission.CommissionRequestStatusTypeId == null) ?
                                                                   (byte)0 : commission.CommissionRequestStatusTypeId.Value,

                                    OrderId = quote.Orders.FirstOrDefault().OrderId != null ?
                                              quote.Orders.FirstOrDefault().OrderId : 0,

                                    OrderStatusTypeId = (quote.Orders.FirstOrDefault().OrderStatusTypeId != null) ?
                                                       (byte)(quote.Orders).FirstOrDefault().OrderStatusTypeId : (byte)0,
                                },
                            };

                var countOfRows = query.Count();

                Log.Debug("Get QuoteItemsModel");

                try
                {
                    model = query.OrderBy(c => 1 == 1).Skip(countOfRows - 1).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Fatal("Exception: " + ex.Message);
                    Log.ErrorFormat("Inner Exception: {0}", (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
                    Log.Error("Exception Source: " + ex.Source);
                }

                if (model == null)
                {
                    this.Response.AddError(Resources.DataMessages.DM010);
                    Log.Error(this.Response.Messages.Items.Last().Text);
                }
            }

            // Create if not found or new
            if (model == null)
            {
                Log.Debug("Create new QuoteItemsModel");

                model = new QuoteItemsModel
                {
                    ProjectId = model.ProjectId,
                    QuoteId = model.QuoteId
                };
            }

            if (model.ActiveQuoteSummary == null)
            {
                model.ActiveQuoteSummary = new QuoteListModel();
                Log.Debug("model.ActiveQuoteSummary is null");
            }

            if (loadQuoteItems)
            {
                searchQuoteItems.QuoteId = model.QuoteId;

                searchQuoteItems.ReturnTotals = true;

                if (!searchQuoteItems.QuoteId.HasValue)
                    searchQuoteItems.QuoteId = 0; // prevent the full list returning ie if quoteid is null

                var response = GetQuoteItemListModel(admin, searchQuoteItems);

                var items = response.Model as List<QuoteItemListModel>;

                model.Items = new PagedList<QuoteItemListModel>(items, model);

                foreach (var item in model.Items) {
                    //exclude LC-configured products from Inventory check
                    if (item.LineItemTypeId != (byte)LineItemTypeEnum.Configured) {
                        if (item.ProductStatusTypeId == (int)ProductStatusTypeEnum.Abolished && item.InventoryStatusId == (int)ProductInventoryStatusTypeEnum.NotAvailable)
                        {
                            model.HasObsoleteAndUnavailableProduct = true;
                        }
                        if (item.ProductStatusTypeId == (int)ProductStatusTypeEnum.Abolished)
                        {
                            model.HasObsoleteProduct = true;
                        }
                        if (item.InventoryStatusId == (int)ProductInventoryStatusTypeEnum.NotAvailable)
                        {
                            //model.InventoryStatusId = (int)ProductInventoryStatusTypeEnum.NotAvailable;
                            model.HasUnavailableProduct = true;
                        }
                    }
                    
                }

                
                //model.HasObsoleteProduct = ...
            }

            if (loadDiscountRequests)
            {
                Log.Debug("loadDiscountRequests equal true");

                var searchDiscountRequests = new SearchDiscountRequests(model as Search);

                searchDiscountRequests.QuoteId = model.QuoteId;

                searchDiscountRequests.ReturnTotals = false;

                searchDiscountRequests.PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL;

                if (!searchDiscountRequests.QuoteId.HasValue)
                    searchDiscountRequests.QuoteId = 0; // prevent the full list returning ie if quoteid is null

                var response = new DiscountRequestServices(this.Context).GetDiscountRequestListModel(admin, searchDiscountRequests);

                var items = response.Model as List<DiscountRequestModel>;

                model.DiscountRequests = new PagedList<DiscountRequestModel>(items, model);

                Log.Debug("DiscountRequests count: " + model.DiscountRequests.Count());
            }

            if (loadCommissionRequests)
            {
                Log.Debug("loadCommissionRequests equal true");

                var searchCommissionRequests = new SearchCommissionRequests(model as Search);
                searchCommissionRequests.QuoteId = model.QuoteId;
                searchCommissionRequests.ReturnTotals = false;
                searchCommissionRequests.PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL;
                if (!searchCommissionRequests.QuoteId.HasValue)
                {
                    searchCommissionRequests.QuoteId = 0;
                }
                var response = new CommissionRequestServices(this.Context).GetCommissionRequestListModel(admin, searchCommissionRequests);
                var items = response.Model as List<CommissionRequestModel>;
                model.CommissionRequests = new PagedList<CommissionRequestModel>(items, model);

                Log.Debug("CommissionRequests count: " + model.CommissionRequests.Count());
            }

            if (loadQuoteOrders)
            {
                Log.Debug("loadQuoteOrders is true");

                var OrderVM = new OrderViewModel();
                OrderVM.QuoteId = (long)model.QuoteId;

                var response = new OrderServices(this.Context).GetOrderListViewModel(admin, OrderVM);
                List<OrderViewModel> items = response.Model as List<OrderViewModel>;
                if (items != null && items.Count > 0)
                {
                    model.HasOrder = true;
                    model.QuoteOrders = new PagedList<OrderViewModel>(items, model);
                }

            }

            // Page number might change to copy it
            model.Page = searchQuoteItems.Page;

            // Page size might change to copy it
            model.PageSize = searchQuoteItems.PageSize;

            // Page totals need to be copied
            model.TotalRecords = searchQuoteItems.TotalRecords;

            this.Response.Model = model;

            Log.DebugFormat("Page: {0} PageSize: {1} TotalRecords: {2}",
                            model.Page, model.PageSize, model.TotalRecords);

            Log.Info("GetQuoteItemsModel finished");

            return this.Response;
        }

        public ServiceResponse GetQuoteItems(UserSessionModel admin, long? quoteId)
        {
            Log.Info("Enter GetQuoteItems");
            Log.Debug("quoteId: " + quoteId);

            if (quoteId.HasValue)
            {
                Log.Debug("quoteId has value.execute query.");

                var query = from quoteItem in this.Context.QuoteItems
                            where quoteItem.Quantity > 0
                            join quote in this.Context.Quotes on quoteItem.QuoteId equals quote.QuoteId
                            join project in this.Context.Projects on quote.ProjectId equals project.ProjectId
                            where quoteItem.QuoteId == quoteId
                            select new QuoteItemModel
                            {
                                ProjectId = quote.ProjectId,
                                QuoteId = quote.QuoteId,
                                ProductId = quoteItem.ProductId,
                                ProductNumber = quoteItem.ProductNumber,
                                Description = quoteItem.Description,
                                Quantity = quoteItem.Quantity,
                                ListPrice = quoteItem.ListPrice,
                                NetPrice = quoteItem.ListPrice * quoteItem.Multiplier,
                                AccountMultiplierId = quoteItem.AccountMultiplierId,
                                CodeString = quoteItem.CodeString,
                                LineItemTypeId = quoteItem.LineItemTypeId
                            };

                List<QuoteItemModel> model = new List<QuoteItemModel>();

                try
                {
                    model = query.ToList();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Exception: {0}", ex.Message);
                    Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                    Log.FatalFormat("Exception Source: {0}", ex.Source);
                }

                this.Response.Model = model;
            }

            Log.Debug("GetQuoteItems finished");
            Log.Debug(this.Response.Model);

            return this.Response;
        }

        public ServiceResponse GetOptionItemsModel(UserSessionModel user, long quoteItemId)
        {

            var optionItems = Db.QuoteItemOptionsByQuoteItemId(user, quoteItemId).ToList();
            var optionItemsModel = new List<QuoteItemOptionModel>();
            foreach (var optionItem in optionItems)
            {
                var optionItemModel = new QuoteItemOptionModel()
                {
                    QuoteItemOptionId = optionItem.QuoteItemOptionId,
                    QuoteItemId = optionItem.QuoteItemId,
                    QuoteId = optionItem.QuoteId,
                    BaseProductId = optionItem.BaseProductId,
                    OptionProductId = optionItem.OptionProductId,
                    OptionProductNumber = optionItem.OptionProductNumber,
                    OptionProductName = optionItem.OptionProductDescription,
                    RequiredQuantity = optionItem.RequiredQuantity,
                    Quantity = optionItem.Quantity,
                    ListPrice = optionItem.ListPrice,
                    LineItemOptionTypeId = (LineItemOptionTypeEnum)optionItem.LineItemOptionTypeId
                };

                optionItemsModel.Add(optionItemModel);


            }

            this.Response.Model = optionItemsModel;

            return this.Response;
        }

        //public List<QuoteItemListModel> GetOptionItemsAsQuoteItemList(UserSessionModel user, long quoteItemId)
        //{

        //    var query = from q in this.Db.QuoteItemOptionsByQuoteItemId(user, quoteItemId)
        //                join p in this.Db.Products on q.OptionProductId equals p.ProductId into Lp
        //                from p in Lp.DefaultIfEmpty()
        //                select new QuoteItemListModel
        //                {
        //                    QuoteId = q.QuoteId,
        //                    ProductId = q.OptionProductId,
        //                    //PriceNet = q.ListPrice * q.Multiplier,
        //                    PriceList = q.ListPrice,
        //                    IsCommissionable = p.AllowCommissionScheme,
        //                    Quantity = q.Quantity,
        //                    ProductNumber = q.OptionProductNumber,
        //                    Description = q.OptionProductDescription,
        //                    QuoteItemId = q.QuoteItemId,
        //                    ProductClassCode = p.ProductClassCode,
        //                    SubmittalSheetTypeId = (SubmittalSheetTypeEnum)p.SubmittalSheetTypeId,
        //                    //Tags = q.Tags,
        //                    CodeString = q.CodeString,
        //                    //QuoteItemTypeId = q.QuoteItemTypeId
        //                    //LineItemTypeId = q.LineItemTypeId
        //                };

        //    var optionItems = query.ToList();
        //    return optionItems;

        //}

        //public ServiceResponse GetQuoteQuotePackage(UserSessionModel admin, QuoteItemsModel model)
        //{
        //    Log.InfoFormat("Enter GetQuoteQuotePackge for {0}", model.GetType());
        //    Log.Debug("PageSize: " + model.PageSize);

        //    model.PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL;

        //    // get QuoteItems items now
        //    var search = new SearchQuoteItem(model as Search);

        //    Log.Debug("Searching Filter: " + search.Filter);

        //    if (!model.QuoteId.HasValue)
        //    {
        //        this.Response.AddError(Resources.DataMessages.DM010);
        //        Log.ErrorFormat(this.Response.Messages.Items.Last().Text);
        //    }

        //    var query = from quote in this.Db.QueryQuoteViewableByQuoteId(admin, model.QuoteId)
        //                join project in this.Db.Projects on quote.ProjectId equals project.ProjectId

        //                join active in this.Db.Quotes on new { id = project.ProjectId, active = true } equals new { id = active.ProjectId, active = active.Active } into Laq
        //                from active in Laq.DefaultIfEmpty()

        //                select new QuoteItemsModel
        //                {
        //                    ProjectId = quote.ProjectId,
        //                    QuoteId = quote.QuoteId,
        //                    Title = quote.Title,
        //                    ProjectName = project.Name,
        //                };

        //    try
        //    {
        //        Log.DebugFormat("Start retrieve QuoteItemsModel for QuoteId: {0}", model.QuoteId);
        //        model = query.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.FatalFormat("Exception Source: {0}", ex.Source);
        //        Log.FatalFormat("Exception: {0}", ex.Message);
        //        Log.FatalFormat("Exception Detail: {0}", ex.InnerException.Message);
        //    }

        //    if (model == null)
        //    {
        //        this.Response.AddError(Resources.DataMessages.DM010);
        //        Log.ErrorFormat("the return {0} model is null", model.GetType());
        //    }

        //    search.QuoteId = model.QuoteId;

        //    search.ReturnTotals = false;

        //    var response = GetQuoteItemListModel(admin, search);

        //    var items = response.Model as List<QuoteItemListModel>; // original items list

        //    //Get QuoteItemOptions
        //    var finalItemsList = new List<QuoteItemListModel>();

        //    foreach (var item in items)
        //    {
        //        if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
        //        {
        //            model.HasConfiguredItem = true;
        //            var optionItems = GetOptionItemsAsQuoteItemList(admin, (long)item.QuoteItemId);
        //            finalItemsList.AddRange(optionItems);
        //        }
        //    }

        //    //Combine with original items list
        //    finalItemsList.AddRange(items);

        //    //Remove duplicated Items
        //    //finalItemsList = finalItemsList.GroupBy(i => i.ProductId).Select(i => i.First()).ToList();

        //    Log.DebugFormat("Total QuoteItemListModel return from search: {0}", finalItemsList.Count);

        //    model.Items = new PagedList<QuoteItemListModel>(finalItemsList, model);

        //    var products = finalItemsList.Cast<ProductModel>().ToList();

        //    Log.DebugFormat("Total Products return from search: {0}", products.Count);

        //    new ProductServices(this.Context).GetDocuments(products);

        //    var baseDirectory = Utilities.GetQuotePackageDirectory(model.QuoteId.Value);
        //    Log.DebugFormat("QuotePackageDirectory return: {0}", baseDirectory);

        //    var packagefiles = Directory.GetFiles(baseDirectory);

        //    Log.DebugFormat("Total files return from  QuotePackageDirectory (included lock files): {0}", packagefiles.Count());
        //    foreach (var file in packagefiles)
        //    {
        //        Log.Debug(file);
        //    }

        //    model.QuotePackage = new List<DocumentModel>();

        //    model.QuotePackage.AddRange(packagefiles.Select(f => new DocumentModel { Description = Path.GetFileName(f) }).ToList());

        //    var quotePackageFileName = Utilities.QuotePackageFileName(model.QuoteId.Value);

        //    var attachedfiles = Directory.GetFiles(baseDirectory).Where(f =>
        //    {
        //        var file = System.IO.Path.GetFileName(f);
        //        var isSystemPackageFile = file.StartsWith("DPO_QuotePackage_");
        //        var isLock = file.EndsWith(".lck");
        //        return !isSystemPackageFile && !isLock;
        //    }).ToList();

        //    Log.DebugFormat("Total files return from  QuotePackageDirectory : {0}", attachedfiles.Count());
        //    foreach (var attFile in attachedfiles)
        //    {
        //        Log.Debug(attachedfiles);
        //    }

        //    model.QuotePackageAttachedFiles = new List<DocumentModel>();

        //    model.QuotePackageAttachedFiles.AddRange(attachedfiles.Select(f =>
        //        new DocumentModel
        //        {
        //            FileName = Path.GetFileName(f),
        //            DocumentTypeId = (int)DocumentTypeEnum.QuotePackageAttachedFile,
        //            Type = "QuotePackageAttachedFile",
        //            Description = Path.GetFileName(f)
        //        }).ToList());

        //    Log.DebugFormat("Total QuotePackageAttachmentfiles have added to QuoteItemsModel: {0}", model.QuotePackageAttachedFiles.Count());
        //    foreach (var qpaf in model.QuotePackageAttachedFiles)
        //    {
        //        Log.Debug(qpaf.FileName);
        //    }

        //    this.Response.Model = model;
        //    Log.InfoFormat("GetQuoteQuotePackage finished");

        //    return this.Response;
        //}


        public ServiceResponse Duplicate(UserSessionModel user, QuoteModel model)
        {
            Log.InfoFormat("Enter Uplicate for {0}", model.GetType());
            Log.DebugFormat("QuoteId {0}: ", model.QuoteId);
            Log.DebugFormat("Is Active: {0}", model.Active);

            this.Db.ReadOnly = false;

            //start Logging the duplicate process
            Log.Info("Starting the Duplicate Quote Serivce");
            Log.Info("QuoteModel value before duplicate:");
            Log.InfoFormat("ApprovedCommissionPercentage:{0}" +
                            "ApproveDiscountPercentage: {1} " +
                            "DiscountPercentage: {2} " +
                            "CommissionPercentage: {3}",
                             model.ApprovedCommissionPercentage,
                             model.ApprovedDiscountPercentage,
                             model.DiscountPercentage,
                             model.CommissionPercentage);

            if (model.ProjectId.HasValue && model.QuoteId.HasValue)
            {
                Quote newQuote = Db.QuoteDuplicate(user, model.ProjectId.Value, model.QuoteId.Value);

                newQuote.Active = false;

                this.Response.Model = newQuote;

                try
                {
                    ApplyBusinessRules(user, newQuote);

                    if (this.Response.IsOK)
                    {
                        SaveToDatabase(model, newQuote, string.Format("Quote revision '{0}' added", newQuote.Revision));
                    }

                }
                catch (Exception e)
                {
                    this.Response.AddError(e.Message);
                    this.Response.Messages.AddAudit(e);
                }
            }

            return this.Response;
        }

        #endregion

        #region Finalise Model

        public void FinaliseModel(UserSessionModel admin, QuoteModel model)
        {
            Log.InfoFormat("Enter finaliseModel for {0}", model.GetType());

            var service = new HtmlServices(this.Context);
            model.CommissionMultipliersTypes = new HtmlServices(this.Context).DropDownModelCommissionMultiplerTypes(model.Multiplier);

            var projectActiveQuoteSummary = model.Project.ActiveQuoteSummary;

            Log.Info("FinaliseModel finished");
            //model.DiscountPercentage *= 100;
            //model.CommissionPercentage *= 100;
            //model.ApprovedCommissionPercentage *= 100;
            //model.ApprovedDiscountPercentage *= 100;
        }

        public void FinaliseModel(UserSessionModel admin, QuoteItemModel model)
        {
            Log.InfoFormat("Enter FinaliseModel for {0}", model.GetType());
            var service = new HtmlServices(this.Context);
            Log.InfoFormat("FinaliseModel finished");
        }

        #endregion

        #region Post Requests

        public ServiceResponse PostModel(UserSessionModel admin, QuoteModel model)
        {
            Log.Info("Enter PostModel");

            this.Db.ReadOnly = false;

            Quote entity = null;

            try
            {
                // Validate Model 
                Log.Debug("Start validate QuoteModel");
                RulesOnValidateModel(this.ModelState, model);

                // Map to Entity
                if (this.Response.IsOK)
                {
                    Log.Debug("Validate model success.");
                    Log.DebugFormat("start Map {0} to Entity ", model.GetType());
                    entity = ModelToEntity(admin, model);
                }

                if (this.Response.IsOK)
                {
                    Log.DebugFormat("Map {0} to {1} success.", model.GetType(), entity.GetType());
                    Log.Debug("Start applyBusinessRules");
                    ApplyBusinessRules(admin, entity);
                }

                if (this.Response.IsOK)
                {
                    Log.DebugFormat("ApplyBusinessRule for {0} success", model.GetType());
                    Log.Debug("Start write to Database");

                    base.SaveToDatabase(model, entity, string.Format("Quote '{0}'", entity.Title));

                    Log.DebugFormat("Write to Database success. Enity: {0} - QuoteId: {1} ",
                                    entity.Title, entity.QuoteId);

                    model.QuoteId = entity.QuoteId;
                }

            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);

                Log.FatalFormat("Exception Source {0}", e.Source);
                Log.FatalFormat("Exception: {1} ", model.GetType(), e.Message);
                Log.FatalFormat("Inner Exception {1}", model.GetType(), (e.InnerException != null) ? e.InnerException.Message : e.Message);
            }

            //mass upload change - had to turn this off
            Log.DebugFormat("Start finalize {0} model", model.GetType());
            FinaliseModel(admin, model);

            //mass upload change - this should be model, not entity
            this.Response.Model = model;

            Log.InfoFormat("PostModel for {0} finished", model.GetType());

            return this.Response;
        }

        public ServiceResponse SetActive(UserSessionModel admin, QuoteModel model)
        {
            Log.InfoFormat("Enter SetActive for {0}", model.GetType());

            this.Db.ReadOnly = false;

            Log.DebugFormat("Calling GetEntity for {0}", model.GetType());

            var entity = GetEntity(admin, model);

            if (this.Response.IsOK)
            {
                Log.Debug("GetEntity success. Set Entity.Active = true");
                entity.Active = true;

                Log.DebugFormat("Start applyApplyBusinessRule for {0}", entity.GetType());

                ApplyBusinessRules(admin, entity);
            }

            if (this.Response.IsOK)
            {
                Log.DebugFormat("ApplyBusinessRules for {0} success", entity.GetType());
                Log.DebugFormat("Start write to database. Entity: {0}", entity.Title);

                base.SaveToDatabase(model, entity, string.Format("Quote '{0}'", entity.Title));

                Log.DebugFormat("Write to database success");
                model.Active = true;

                Log.Debug("Set model.Active = true");
            }

            Log.DebugFormat("Start finaliseModel for {0}", model.GetType());

            FinaliseModel(admin, model);

            this.Response.Model = model;

            Log.DebugFormat("SetActive for {0} finished.", model.GetType());

            return this.Response;
        }

        public ServiceResponse UnDelete(UserSessionModel admin, QuoteModel model, bool delete)
        {
            Log.InfoFormat("Enter Undelete for {0}", model.GetType());
            return DeleteAction(admin, model, false);
        }

        public ServiceResponse Delete(UserSessionModel admin, QuoteModel model, bool delete)
        {
            Log.InfoFormat("Enter Delete for {0}", model.GetType());
            return DeleteAction(admin, model, true);
        }

        private ServiceResponse DeleteAction(UserSessionModel user, QuoteModel model, bool delete)
        {
            Log.InfoFormat("Enter DeleteAction for {0}", model.GetType());

            this.Db.ReadOnly = false;

            Log.Debug("get Entity for model");

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                Log.Debug("Get Entity success.");

                entity.Deleted = delete;

                Log.DebugFormat("Entity State: {0}", entity.Deleted ? "Delete" : "Unknown");

                Entry = Db.Entry(entity);

                Log.Debug("Calling RuleOnDelete for entity");

                if (delete) RulesOnDelete(user, entity);
            }

            if (this.Response.IsOK)
            {
                Log.Debug("RuleOnDelete success.Start writing to database");

                base.SaveToDatabase(model, entity, string.Format("Quote '{0}'", entity.Title));
            }

            this.Response.Model = model;

            Log.InfoFormat("DeleteAction for {0} finished.", model.GetType());

            return this.Response;
        }

        public ServiceResponse QuoteRecalculate(UserSessionModel user, QuoteModel model)
        {
            Log.InfoFormat("Enter QuoteRecalculate for {0} ", model.GetType());

            this.Db.ReadOnly = false;

            Log.Debug("Get Entity for model");

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                Log.Debug("Get Entity success.");

                Entry = Db.Entry(entity);

                entity.RecalculationRequired = true;
                Log.DebugFormat("RecalculationRequired : {0}", entity.RecalculationRequired);

                Entry.State = EntityState.Modified;
                Log.DebugFormat("Entry State: {0}", Entry.State);

                Log.Debug("ApplyBusinessRules starting");
                ApplyBusinessRules(user, entity);
            }

            if (this.Response.IsOK)
            {
                Log.Debug("ApplyBusinessRules success.Start writing to database.");

                try
                {
                    base.SaveToDatabase(model, entity, string.Format("Quote '{0}'", entity.Title));
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Exception Source: {0} ", ex.Source);
                    Log.FatalFormat("Exception: {0}", ex.Message);
                    Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                }
            }

            model.RecalculationRequired = entity.RecalculationRequired;

            this.Response.Model = model;

            Log.Info("QuoteRecalculate finished.");

            return this.Response;
        }

        public ServiceResponse ImportProductsFromCSV(UserSessionModel user, CsvReader data, QuoteModel model)
        {
            Log.InfoFormat("Enter ImportProductsFromCSV for {0}", model.GetType());
            Log.DebugFormat("QuoteId: {0}", model.QuoteId);

            this.Context.ReadOnly = false;

            if (model == null || model.QuoteId == null || model.ProjectId == null)
            {
                this.Response.AddError(Resources.ResourceModelProject.MP024);
                if (model == null)
                {
                    Log.ErrorFormat("{0} is null", model.GetType());
                }
                if (model.QuoteId == null)
                {
                    Log.Error("Quoteid is null");
                }
                if (model.ProjectId == null)
                {
                    Log.Error("ProjectId is null");
                }
                return this.Response;
            }

            if (!Db.QueryQuoteViewableByQuoteId(user, model.QuoteId).Any())
            {
                this.Response.AddError(Resources.ResourceModelProject.MP024);
                Log.Error(this.Response.Messages.Items.Last());
                return this.Response;
            }

            bool isValid = false;

            Dictionary<string, QuoteItem> itemsDict = new Dictionary<string, QuoteItem>();
            List<QuoteItem> items = null;

            Log.Debug("Start reading csv file");

            while (data.Read())
            {
                var columns = data.CurrentRecord;

                if (columns.Length >= 3)
                {
                    if (isValid)
                    {
                        decimal qty;
                        var productNumber = columns[0].Trim();

                        if (String.Compare(productNumber, "R410A", true) != 0)
                        {
                            if (decimal.TryParse(columns[1].Trim(), out qty))
                            {

                                if (string.IsNullOrWhiteSpace(productNumber) == false || productNumber.Contains("piping") == false)
                                {
                                    if (itemsDict.ContainsKey(productNumber))
                                    {
                                        itemsDict[productNumber].Quantity += qty;
                                    }
                                    else
                                    {
                                        itemsDict[productNumber] = new QuoteItem { ProductNumber = productNumber, Quantity = qty, Tags = "" };
                                    }

                                    //items.Add(new QuoteItem { ProductNumber = productNumber, Quantity = qty });
                                }
                            }
                            else
                            {
                                isValid = false;
                                Log.Debug("ProductNumber equal 'R410A'. Product is Invalid");
                            }
                        }
                    }
                }

                /* to add - validation for other columns */
                if (columns.Length >= 2)
                {
                    if (itemsDict.ContainsKey(columns[1]) && columns[0].Length > 0)
                    {
                        if (!itemsDict[columns[1]].Tags.Contains(columns[0]))
                        {
                            if (itemsDict[columns[1]].Tags.Length > 0)
                            {
                                itemsDict[columns[1]].Tags += ", ";
                            }

                            itemsDict[columns[1]].Tags += columns[0];
                        }
                    }
                }

                if (!isValid && columns[0].ToLower().Trim() == "model" && columns[1].ToLower().Trim() == "qty")
                {
                    isValid = true;
                }
            }

            if (itemsDict.Count > 0)
            {
                items = itemsDict.Values.ToList();

                var productNumberList = items.Select(i => i.ProductNumber).Distinct().ToArray();

                var productLookup = Db.ProductByProductNumbers(user, productNumberList).Distinct().ToArray().ToDictionary(p => p.ProductNumber, p => p.ProductId);

                foreach (var import in items)
                {
                    if (productLookup.ContainsKey(import.ProductNumber))
                    {
                        import.ProductId = productLookup[import.ProductNumber];
                    }
                    else
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP023, import.ProductNumber));
                    }
                }
            }

            Log.Debug("Reading csv file finished.");

            // select only valid items
            if (items != null)
            {
                var itemsToImport = items.Where(i => i.ProductId != null).ToList();

                if (itemsToImport.Count > 0)
                {
                    Log.Debug("Start calling AdjustQuoteItems");

                    AdjustQuoteItems(user, new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId }, items, true);

                    if (this.Response.IsOK)
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP025, itemsToImport.Count));
                        Log.Debug(this.Response.Messages.Items.Last());
                    }
                }
                else
                {
                    this.Response.AddError(Resources.ResourceModelProject.MP141);
                }

                Log.Info("ImportProductsFromCSV finished.");
            }
            else
            {
                this.Response.AddError(Resources.ResourceModelProject.MP141);
            }

            return this.Response;

        }

        //This service takes HttpPostedFileBase (MVC Controller)
        public ServiceResponse ImportProductsFromXml(UserSessionModel user, HttpPostedFileBase file, QuoteModel model)
        {
            Log.InfoFormat("Enter ImportProductsFromXml for {0}", model.GetType());
            Log.Debug("Start loading file");

            StreamReader reader = new StreamReader(file.InputStream);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNodeList root = doc.GetElementsByTagName("Workbook");

            List<QuoteItem> quoteItems = new List<QuoteItem>();
            Dictionary<string, QuoteItem> itemsDict = new Dictionary<string, QuoteItem>();

            bool isValidFormat = false;

            //bool isValid = false;
            Log.Debug("Start reading.");

            foreach (XmlNode node in root)
            {
                for (int x = 0; x < node.ChildNodes.Count; x++)
                {
                    if (node.ChildNodes[x].Name == "Worksheet")
                    {
                        isValidFormat = true;
                        bool isValid = false;
                        XmlNode childNode = node.ChildNodes[x].FirstChild;

                        for (int i = 0; i < childNode.ChildNodes.Count; i++)
                        {
                            XmlNode desNode = childNode.ChildNodes[i];
                            if (desNode.HasChildNodes)
                            {
                                if (desNode.FirstChild.InnerText.Contains("Model") || desNode.FirstChild.InnerText.Contains("_.SECTION"))
                                {
                                    isValid = true;
                                    continue;
                                }
                            }
                            if (isValid)
                            {
                                var productNumber = desNode.ChildNodes[0].InnerText.ToString();
                                var qty = (desNode.ChildNodes[1].InnerText != string.Empty) ? Convert.ToDecimal(desNode.ChildNodes[1].InnerText) : 0;
                                var tags = (desNode.ChildNodes[3] != null && desNode.ChildNodes[3].InnerText != string.Empty) ? desNode.ChildNodes[3].InnerText : null;

                                if (!string.IsNullOrEmpty(productNumber) && !productNumber.ToLower().Contains("piping"))
                                {
                                    if (itemsDict.ContainsKey(productNumber))
                                    {
                                        itemsDict[productNumber].Quantity += qty;
                                    }
                                    else
                                    {
                                        itemsDict[productNumber] = new QuoteItem
                                        {
                                            ProductNumber = productNumber,
                                            Quantity = qty,
                                            Tags = (tags != null) ? tags : null
                                        };
                                    }
                                }
                                else
                                {
                                    //isValid = false;
                                    Log.Debug("productNumber is Null or productNumber contain 'piping'.Invalid record");
                                }
                            }
                        }
                    }

                }
            }

            if (!isValidFormat)
            {
                this.Response.AddError(Resources.ResourceModelProject.MP141);
            }

            if (itemsDict.Count > 0)
            {

                quoteItems = itemsDict.Values.ToList();

                var productNumberList = quoteItems.Select(i => i.ProductNumber).Distinct().ToArray();

                var results = Db.ProductByProductNumbers(user, productNumberList)
                              .GroupBy(r => r.ProductNumber)
                              .Select(g => g.FirstOrDefault()).ToArray();

                var productLookup = new Dictionary<string, long>();

                for (int i = 0; i < results.Count(); i++)
                {
                    productLookup.Add(results[i].ProductNumber, results[i].ProductId);
                }

                foreach (var import in quoteItems)
                {
                    if (productLookup.ContainsKey(import.ProductNumber))
                    {
                        import.ProductId = productLookup[import.ProductNumber];
                    }
                    else
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP023, import.ProductNumber));
                        Log.DebugFormat("{0} {1}", this.Response.Messages.Items.Last().Text, import.ProductNumber);
                    }
                }
            }

            Log.Debug("finish reading file");

            // select valid items only
            if (quoteItems.Count > 0)
            {
                var itemsToImport = quoteItems.Where(i => i.ProductId != null).ToList();

                if (itemsToImport.Count > 0)
                {
                    Log.Debug("Calling AdjustQuoteItems");
                    AdjustQuoteItems(user, new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId }, quoteItems, true);

                    if (this.Response.IsOK)
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP025, itemsToImport.Count));
                        Log.DebugFormat("{0} {1} ", this.Response.Messages.Items.Last().Text, itemsToImport.Count);
                    }
                }

                Log.InfoFormat("ImportProductsFromXml finished");
            }
            else
            {
                this.Response.AddError(Resources.ResourceModelProject.MP141);
            }

            return this.Response;
        }

        //This service takes HttpPostedFile (Web Api Controller)
        public ServiceResponse ImportProductsFromXML(UserSessionModel user, HttpPostedFile file, QuoteModel model)
        {
            Log.InfoFormat("Enter ImportProductsFromXml for {0}", model.GetType());
            Log.Debug("Start loading file");

            StreamReader reader = new StreamReader(file.InputStream);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            XmlNodeList root = doc.GetElementsByTagName("Workbook");

            List<QuoteItem> quoteItems = new List<QuoteItem>();
            Dictionary<string, QuoteItem> itemsDict = new Dictionary<string, QuoteItem>();

            bool isValidFormat = false;

            //bool isValid = false;
            Log.Debug("Start reading.");

            foreach (XmlNode node in root)
            {
                for (int x = 0; x < node.ChildNodes.Count; x++)
                {
                    if (node.ChildNodes[x].Name == "Worksheet")
                    {
                        isValidFormat = true;
                        bool isValid = false;
                        XmlNode childNode = node.ChildNodes[x].FirstChild;

                        for (int i = 0; i < childNode.ChildNodes.Count; i++)
                        {
                            XmlNode desNode = childNode.ChildNodes[i];
                            if (desNode.HasChildNodes)
                            {
                                if (desNode.FirstChild.InnerText.Contains("Model") || desNode.FirstChild.InnerText.Contains("_.SECTION"))
                                {
                                    isValid = true;
                                    continue;
                                }
                            }
                            if (isValid)
                            {
                                var productNumber = desNode.ChildNodes[0].InnerText.ToString();
                                var qty = (desNode.ChildNodes[1].InnerText != string.Empty) ? Convert.ToDecimal(desNode.ChildNodes[1].InnerText) : 0;
                                var tags = (desNode.ChildNodes[3] != null && desNode.ChildNodes[3].InnerText != string.Empty) ? desNode.ChildNodes[3].InnerText : null;

                                if (!string.IsNullOrEmpty(productNumber) && !productNumber.ToLower().Contains("piping"))
                                {
                                    if (itemsDict.ContainsKey(productNumber))
                                    {
                                        itemsDict[productNumber].Quantity += qty;
                                    }
                                    else
                                    {
                                        itemsDict[productNumber] = new QuoteItem
                                        {
                                            ProductNumber = productNumber,
                                            Quantity = qty,
                                            Tags = (tags != null) ? tags : null
                                        };
                                    }
                                }
                                else
                                {
                                    //isValid = false;
                                    Log.Debug("productNumber is Null or productNumber contain 'piping'.Invalid record");
                                }
                            }
                        }
                    }

                }
            }

            if (!isValidFormat)
            {
                this.Response.AddError(Resources.ResourceModelProject.MP141);
            }

            if (itemsDict.Count > 0)
            {

                quoteItems = itemsDict.Values.ToList();

                var productNumberList = quoteItems.Select(i => i.ProductNumber).Distinct().ToArray();

                var results = Db.ProductByProductNumbers(user, productNumberList)
                              .GroupBy(r => r.ProductNumber)
                              .Select(g => g.FirstOrDefault()).ToArray();

                var productLookup = new Dictionary<string, long>();

                for (int i = 0; i < results.Count(); i++)
                {
                    productLookup.Add(results[i].ProductNumber, results[i].ProductId);
                }

                foreach (var import in quoteItems)
                {
                    if (productLookup.ContainsKey(import.ProductNumber))
                    {
                        import.ProductId = productLookup[import.ProductNumber];
                    }
                    else
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP023, import.ProductNumber));
                        Log.DebugFormat("{0} {1}", this.Response.Messages.Items.Last().Text, import.ProductNumber);
                    }
                }
            }

            Log.Debug("finish reading file");

            // select valid items only
            if (quoteItems.Count > 0)
            {
                var itemsToImport = quoteItems.Where(i => i.ProductId != null).ToList();

                if (itemsToImport.Count > 0)
                {
                    Log.Debug("Calling AdjustQuoteItems");
                    AdjustQuoteItems(user, new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId }, quoteItems, true);

                    if (this.Response.IsOK)
                    {
                        this.Response.AddSuccess(string.Format(Resources.ResourceModelProject.MP025, itemsToImport.Count));
                        Log.DebugFormat("{0} {1} ", this.Response.Messages.Items.Last().Text, itemsToImport.Count);
                    }
                }

                Log.InfoFormat("ImportProductsFromXml finished");
            }
            else
            {
                this.Response.AddError(Resources.ResourceModelProject.MP141);
            }

            return this.Response;
        }

        public ServiceResponse QuoteItemRemove(UserSessionModel user, QuoteItemModel model)
        {
            Log.InfoFormat("Enter QuoteItemRemove for {0}", model.GetType());

            if (model.QuoteItemId.HasValue)
            {
                Log.DebugFormat("QuoteId: {0}", model.QuoteId);

                this.Context.ReadOnly = false;

                var quoteModel = new QuoteModel { QuoteId = model.QuoteId };

                Log.DebugFormat("Get Entity for {0}", quoteModel.GetType());

                var quote = GetEntity(user, quoteModel);

                if (quote != null && model.QuoteItemId.HasValue)
                {
                    Log.DebugFormat("{0} is not null", quote.GetType());

                    Log.DebugFormat("Get QuoteItems by QuoteId from DbContext: {0}", Db.GetType());

                    Db.QuoteItemsByQuoteId(user, model.QuoteId).Load();

                    Log.DebugFormat("Get QuoteItem to Delete");

                    var quoteItemToDelete = Db.Context.QuoteItems.Local.Where(q => q.QuoteItemId == model.QuoteItemId.Value).FirstOrDefault();

                    Log.DebugFormat("QuoteItem to Delete: {0}", quoteItemToDelete.QuoteItemId);

                    if (quoteItemToDelete != null)
                    {
                        this.Context.Entry(quoteItemToDelete).State = EntityState.Deleted;

                        this.Context.Entry(quote).State = EntityState.Modified;

                        quote.QuoteItemBeingDeleted = true;

                        Log.DebugFormat("Start ApplyBusinessRules for {0}", quote.GetType());

                        ApplyBusinessRules(user, quote);

                        if (this.Response.IsOK)
                        {
                            Log.Debug("ApplyBusinessRules success.Start writing to database.");

                            try
                            {
                                base.SaveToDatabase(model, quote, string.Format("Quote '{0}'", quote.Title));
                            }
                            catch (Exception ex)
                            {
                                Log.FatalFormat("Exception Source: {0}", ex.Source);
                                Log.FatalFormat("Exception: {0}", ex.Message);
                                Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                            }
                        }
                    }
                }

                Log.DebugFormat("Calling CalculateUnitCounts for {0}", quoteModel.GetType());
                CalculateUnitCounts(user, quoteModel);
            }

            this.Response.Model = model;

            Log.Info("QuoteItemRemove finished.");
            return this.Response;
        }

        public ServiceResponse QuoteItemAdjustments(UserSessionModel user, QuoteModel model, string quoteItemUpdates)
        {
            Log.InfoFormat("Start QuoteItemAdjustments for {0} with quoteItemUpdates: {1}",
                     model.GetType(), quoteItemUpdates);

            if (model != null && !string.IsNullOrWhiteSpace(quoteItemUpdates))
            {
                Log.Debug("{0} is not null; quoteItemUpdates is not empty.Starting process");

                try
                {
                    var quoteAdjustments = Json.Decode<List<QuoteItem>>(quoteItemUpdates);

                    foreach (QuoteItem item in quoteAdjustments)
                    {
                        if (item.Quantity == 0)
                        {
                            item.QuoteId = model.QuoteId.Value;
                            RemoveProductsFromQuote(user, item);
                        }
                    }

                    Log.Debug("Calling AdjustQuoteItems");
                    AdjustQuoteItems(user, model, quoteAdjustments, false);

                    Log.DebugFormat("Calling CalculateUnitCounts for {0}", model.GetType());
                    CalculateUnitCounts(user, model);
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Exception Source: {0}", ex.Source);
                    Log.FatalFormat("Exception: {0}", ex.Message);
                }
            }

            this.Response.Model = model;
            Log.InfoFormat("QuoteItemAdjustments finished for {0}", model.GetType());

            return this.Response;
        }

        public void RemoveProductsFromQuote(UserSessionModel user, QuoteItem item)
        {
            Log.InfoFormat("Enter RemoveProductsFromQuote for {0}", item.GetType());
            Log.DebugFormat("QuoteId: {0}", item.QuoteId);
            Log.DebugFormat("QuoteItemId: {0}", item.QuoteItemId);

            if (!string.IsNullOrEmpty(item.QuoteItemId.ToString()))
            {
                this.Context.ReadOnly = false;

                var quoteModel = new QuoteModel { QuoteId = item.QuoteId };

                Log.Debug("Start getting Entity for model");

                var quote = GetEntity(user, quoteModel);

                if (quote != null && !string.IsNullOrEmpty(item.QuoteItemId.ToString()))
                {
                    Log.Debug("Get Entity for model return success.");
                    Log.DebugFormat("Entity type: {0}", quote.GetType());
                    Log.DebugFormat("Start loading QuoteItem for quoteId {0}", item.QuoteId);

                    Db.QuoteItemsByQuoteId(user, item.QuoteId).Load();

                    Log.DebugFormat("Start getting QuoteItem to Delete");

                    var quoteItemToDelete = Db.Context.QuoteItems.Local.Where(q => q.QuoteItemId == item.QuoteItemId).FirstOrDefault();

                    if (quoteItemToDelete != null)
                    {
                        Log.DebugFormat("QuoteItem need to delete: {0}", quoteItemToDelete.QuoteId);

                        //Remove QuoteItemOptions 
                        if (quoteItemToDelete.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                        {
                            var QuoteItemOptionsToDelete = Db.QuoteItemOptionsByQuoteItemId(user, quoteItemToDelete.QuoteItemId).ToList();
                            foreach (var optionItem in QuoteItemOptionsToDelete)
                            {
                                this.Context.Entry(optionItem).State = EntityState.Deleted;
                            }
                        }

                        this.Context.Entry(quoteItemToDelete).State = EntityState.Deleted;

                        this.Context.Entry(quote).State = EntityState.Modified;

                        Log.DebugFormat("Update state of quote to {0}", this.Context.Entry(quote).State);

                        quote.QuoteItemBeingDeleted = true;

                        Log.Debug("Starting ApplyBusinessRules on quote");
                        ApplyBusinessRules(user, quote);

                        if (this.Response.IsOK)
                        {
                            Log.Debug("ApplyBusinessRules success.Start writing to database");
                            try
                            {
                                base.SaveToDatabase(item, quote, string.Format("Quote '{0}'", quote.Title));
                            }
                            catch (Exception ex)
                            {
                                Log.FatalFormat("Exception Source: {0}", ex.Source);
                                Log.FatalFormat("Exception: {0}", ex.Message);
                                Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                            }
                        }
                    }
                }

                Log.DebugFormat("Calling CalculateUnitCounts for model");
                CalculateUnitCounts(user, quoteModel);
            }

            Log.InfoFormat("RemoveProductsFromQuote finished");

        }

        public void CalculateUnitCounts(UserSessionModel user, QuoteModel model)
        {
            Log.InfoFormat("Enter CalculateUnitCounts for model: {0}", model.GetType());
            Log.DebugFormat("QuoteId: {0}", model.QuoteId);

            var quoteModel = new QuoteModel { QuoteId = model.QuoteId };

            Log.Debug("Get Entity for quoteModel");

            var quote = GetEntity(user, quoteModel);

            Log.DebugFormat("Calling CalculateUnitCounts for Entity: {0}", quote.GetType());

            CalculateUnitCounts(user, quote);
        }

        public void CalculateUnitCounts(UserSessionModel user, Quote quote)
        {
            // TODO:  I think this is working kinda outside the originally intended framework
            //  it appears that partial saves are not really used in the framework.
            // TODO:  This code is duplicated in a few places and should be fixed

            Log.InfoFormat("Enter CalculateUnitCounts for Entity: {0}", quote.GetType());

            if (quote == null)
            {
                Log.Debug("enity is null. Exit CalculateUnitCounts");
                return;
            }

            //QuoteModel model = new QuoteModel() { QuoteId = quote.QuoteId };

            // TODO:  Cleanup Context
            // Load proper 

            Log.Debug("Start getting QuoteItems");

            var quoteQuery = from qi in quote.QuoteItems
                             join p in Db.Products
                                on qi.ProductId equals p.ProductId
                             join mt in Db.Context.MultiplierTypes
                                on p.MultiplierTypeId equals mt.MultiplierTypeId
                             join mtmct in Db.Context.MultiplierTypesMultiplierCategoryTypes
                                 on mt.MultiplierTypeId equals mtmct.MultiplierTypeId
                             select new
                             {
                                 Item = qi,
                                 MultiplierCategoryTypeId = mtmct.MultiplierCategoryTypeId
                             };

            // Calculate all items that are classified VRV, Outdoor
            Log.Debug("Start getting all QuoteItems that are VRV-Outdoor");

            var vrvOutdoorUnitList = quoteQuery
                .Where(w => w.Item.Product != null
                    && w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV
                    && w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor)
                .Select(s => s.Item)
                .ToList();
            Log.DebugFormat("Total VRV-Outdoor QuoteItems: {0}", vrvOutdoorUnitList.Count());
            foreach (var item in vrvOutdoorUnitList)
            {
                Log.Debug(item.ProductNumber);
            }

            // Get all items that are classified VRV, Indoor
            Log.Debug("Start geeting all QuoteItems that are VRV-Indoor");

            var vrvIndoorUnitList = quoteQuery
                .Where(w => w.Item.Product != null
                    && w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV
                    && w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor)
                .Select(s => s.Item)
                .ToList();
            Log.DebugFormat("Total VRV-Indoor QuoteItems: {0}", vrvIndoorUnitList.Count());
            foreach (var item in vrvIndoorUnitList)
            {
                Log.Debug(item.ProductNumber);
            }

            // Get all items that are classified Split, Outdoor
            Log.Debug("Start geeting all QuoteItems that are Split-Outdoor");

            var splitOutdoorUnitList = quoteQuery
                .Where(w => w.Item.Product != null
                    && (w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                        || w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                    && w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Split)
                .Select(s => s.Item)
                .ToList();

            Log.DebugFormat("Total Split-Outdoor QuoteItems: {0}", splitOutdoorUnitList.Count());
            foreach (var item in splitOutdoorUnitList)
            {
                Log.Debug(item);
            }

            // Get all items that are classified Split, Indoor
            Log.Debug("Start getting all QuoteItems that are Split-Indoor");

            var splitIndoorUnitList = quoteQuery
                .Where(w => w.Item.Product != null
                    && (w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                        || w.Item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                    && w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Split)
                .Select(s => s.Item)
                .ToList();

            Log.DebugFormat("Total Split-Indoor QuoteItems: {0}", splitIndoorUnitList.Count());
            foreach (var item in splitIndoorUnitList)
            {
                Log.Debug(item.ProductNumber);
            }

            Log.Debug("Start getting all QuoteItems that are Outdoor and productFamily is packaged");

            var rtuList = quote.QuoteItems
                .Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    && (p.Product.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialSplitSystem || p.Product.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialPackagedSystem))
                .ToList();

            Log.DebugFormat("Total Outdoor-PackagedType QuoteItems: {0}", rtuList.Count());

            // Calculate all items that are classified VRV, Outdoor
            Log.Debug("Start getting all QuoteItems that are VRV-Outdoor");

            var lcPackageUnitList = quoteQuery
                .Where(w => w.Item.Product != null
                    && w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.LCPackage
                    && w.Item.Product.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialPackagedSystem)
                .Select(s => s.Item)
                .ToList();
            Log.DebugFormat("Total LC-Split Unit Count: {0}", lcPackageUnitList.Count());
            foreach (var item in lcPackageUnitList)
            {
                Log.Debug(item.ProductNumber);
            }

            var calculator = new ProductComponentCalculator();
            int totalQtyVRVIndoor = calculator.CalculateProductsComponent(vrvIndoorUnitList);
            int totalQtyVRVOutdoor = calculator.CalculateProductsComponent(vrvOutdoorUnitList);
            int totalQtySplitOutdoor = calculator.CalculateProductsComponent(splitOutdoorUnitList, new int[] { (int)ProductModelTypeEnum.Outdoor });
            int totalQtySplitIndoor = calculator.CalculateProductsComponent(splitIndoorUnitList);
            int rtuQty = calculator.CalculateProductsComponent(rtuList);
            int lcPkgQty = calculator.CalculateProductsComponent(lcPackageUnitList);// not used yet

            Log.Debug("Start doing calculation for Quote Totals (VRV, SPLIT) ");

            quote.TotalCountVRV = totalQtyVRVIndoor + totalQtyVRVOutdoor;
            quote.TotalCountVRVIndoor = totalQtyVRVIndoor;
            quote.TotalCountVRVOutdoor = totalQtyVRVOutdoor;

            quote.TotalCountSplit = totalQtySplitIndoor + totalQtySplitOutdoor;
            quote.TotalCountSplitOutdoor = totalQtySplitOutdoor;

            Log.DebugFormat("quote.TotalCountVRV: {0}, quote.TotalCountVRVIndoor: {1}, quote.TotalCountVRVOutdoor: {2}",
                             quote.TotalCountVRV, quote.TotalCountVRVIndoor, quote.TotalCountVRVOutdoor);

            Log.DebugFormat("quote.TotalCountSplit: {0}, quote.TotalCountSplitOutdoor: {1}",
                             quote.TotalCountSplit, quote.TotalCountSplitOutdoor);

            // TODO: HACK: Remove all references to the below VRVOutdoorCount & SplitCount
            quote.VRVOutdoorCount = totalQtyVRVOutdoor;
            quote.SplitCount = totalQtySplitOutdoor;

            Log.DebugFormat("quote.VRVOutdoorCount: {0}", quote.VRVOutdoorCount);
            Log.DebugFormat("quote.SplitCount: {0}", quote.SplitCount);

            //PartialSaveToDatabase(null, updatedQuote, "Quote count information");

            Log.DebugFormat("CalculateUnitCounts for Entity {0} finished.", quote.GetType());
        }

        public ServiceResponse AddProductToQuote(UserSessionModel user, QuoteModel model, long productId, int quantity)
        {
            Log.InfoFormat("Enter AddproductToQuote for {0}, productId: {1}, quantity: {2}",
                           model.GetType(), productId, quantity);

            this.Response = new ServiceResponse();

            this.Context.ReadOnly = false;

            if (model != null)
            {
                Log.Debug("model is not null. Start AdjustQuoteItems");
                AdjustQuoteItems(user, model, new List<QuoteItem> { new QuoteItem { ProductId = productId, Quantity = quantity } }, true);
            }

            this.Response.Model = model;
            Log.InfoFormat("AddProductToQuote for {0} finished.", model.GetType());

            return this.Response;

        }

        public ServiceResponse AddProductToQuoteWithTag(UserSessionModel user, QuoteModel model, long productId, int quantity, string tag)
        {
            Log.InfoFormat("Enter AddproductToQuote for {0}, productId: {1}, quantity: {2} , tag: {3}",
                           model.GetType(), productId, quantity, tag);

            this.Response = new ServiceResponse();

            this.Context.ReadOnly = false;

            if (model != null)
            {
                Log.Debug("model is not null. Start AdjustQuoteItems");
                AdjustQuoteItems(user, model, new List<QuoteItem> { new QuoteItem { ProductId = productId, Quantity = quantity, Tags = tag } }, true);
            }

            this.Response.Model = model;
            Log.InfoFormat("AddProductToQuote for {0} finished.", model.GetType());

            return this.Response;

        }

        public ServiceResponse AddProductsToQuote(UserSessionModel user, QuoteModel model, ProductsModel productsModel)
        {
            Log.InfoFormat("Enter AddProductsToQuote for {0}, {1} ", model.GetType(), productsModel.GetType());

            this.Response = new ServiceResponse();

            this.Context.ReadOnly = false;

            if (model != null)
            {
                Log.Debug("QuoteModel is not null.Start Adding process");
                Log.DebugFormat("Total Products: {0}", productsModel.Products.Count());

                //TODO: Delete after Oct, 01, 2017
                //for (int i = 0; i < productsModel.Products.Count; i++)
                //{
                //    if (productsModel.Products[i].Product.Quantity > 0)
                //    {
                //        Log.DebugFormat("Start AdjustQuoteItems for product: {0}", productsModel.Products[i].Product.ProductNumber);

                //        AdjustQuoteItems(user, model, new List<QuoteItem> { new QuoteItem { ProductId = productsModel.Products[i].Product.ProductId, Quantity = productsModel.Products[i].Product.Quantity } }, true);
                //    }
                //}

                List<QuoteItem> QuoteItemsList = new List<QuoteItem>();
                for (int i = 0; i < productsModel.Products.Count; i++)
                {
                    if (productsModel.Products[i].Product.Quantity > 0)
                    {
                        Log.DebugFormat("Start AdjustQuoteItems for product: {0}", productsModel.Products[i].Product.ProductNumber);

                        //AdjustQuoteItems(user, model, new List<QuoteItem> { new QuoteItem { ProductId = productsModel.Products[i].Product.ProductId, Quantity = productsModel.Products[i].Product.Quantity } }, true);

                        QuoteItem quoteItem = new QuoteItem()
                        {
                            ProductId = productsModel.Products[i].Product.ProductId,
                            Quantity = productsModel.Products[i].Product.Quantity
                        };

                        QuoteItemsList.Add(quoteItem);
                    }
                }

                if (QuoteItemsList.Count > 0)
                {
                    AdjustQuoteItems(user, model, QuoteItemsList, true);
                }


            }

            this.Response.Model = model;

            Log.InfoFormat("AddProductsToQuote for {0}, {1} finished", model.GetType(), productsModel.GetType());

            return this.Response;
        }

        public ServiceResponse AddConfiguredProductsToQuote(UserSessionModel user, QuoteModel quoteModel, LCSTPackagesModel packagesModel)
        {
            this.Response = new ServiceResponse();

            //this.Context.ReadOnly = false;

            var quote = GetEntity(user, quoteModel);

            if (quoteModel != null)
            {
                List<QuoteItem> QuoteItemsList = BuildQuoteItemList(user, quote.QuoteId, packagesModel);

                if (QuoteItemsList.Count > 0)
                {
                    AdjustQuoteItems(user, quoteModel, QuoteItemsList, true);

                    SaveLCSubmittalData(packagesModel);
                }


            }

            this.Response.Model = quoteModel;

            return this.Response;
        }


        public List<QuoteItem> BuildQuoteItemList(UserSessionModel user, long quoteId, LCSTPackagesModel packagesModel)
        {

            List<QuoteItem> QuoteItemsList = new List<QuoteItem>();

            for (int i = 0; i < packagesModel.Packages.Count; i++)
            {
                var package = packagesModel.Packages[i];
                if (package.BaseModel != null)
                {

                    var baseModel = productServices.GetProductbyProductNumber(user, package.BaseModel);

                    if (baseModel != null && baseModel.ProductId > 0)
                    {
                        // Add Configured Model as QuoteItem
                        var configuredQuoteItem = new QuoteItem();
                        configuredQuoteItem.ProductId = baseModel.ProductId;
                        configuredQuoteItem.Quantity = 1;
                        configuredQuoteItem.ListPrice = CalculateConfiguredItemPrice(user, package);
                        configuredQuoteItem.CodeString = package.Model; //Configured ModelNumber
                        configuredQuoteItem.LineItemTypeId = (byte?)LineItemTypeEnum.Configured;
                        configuredQuoteItem.ConfigType = package.ConfigType;
                        configuredQuoteItem.Tags = package.SystemId;

                        // Add BaseModel to QuoteItemOptions 
                        var baseOptionItem = new QuoteItemOption()
                        {
                            QuoteItemId = configuredQuoteItem.QuoteItemId,
                            QuoteId = quoteId,
                            CodeString = configuredQuoteItem.CodeString,
                            BaseProductId = (long)configuredQuoteItem.ProductId,
                            RequiredQuantity = 1,
                            Quantity = 1,
                            OptionProductId = baseModel.ProductId,
                            OptionProductNumber = baseModel.ProductNumber,
                            OptionProductDescription = baseModel.Name,
                            ListPrice = baseModel.ListPrice,
                            LineItemOptionTypeId = (byte)LineItemOptionTypeEnum.BaseModel
                        };

                        configuredQuoteItem.QuoteItemOptions.Add(baseOptionItem);

                        // Add Accessories QuoteItemOptions 
                        foreach (var accessory in package.Accessories)
                        {
                            var product = productServices.GetProductbyProductNumber(user, accessory.AccessoryModel);
                            //if (product != null && accessory.AccessoryType == (byte)LineItemOptionTypeEnum.FactoryInstalled)
                            if (product != null && accessory.AccessoryType == "Y")
                            {
                                //var QuoteItemOption = Db.QuoteItemOptionCreate(configuredQuoteItem);
                                var QuoteItemOption = new QuoteItemOption()
                                {
                                    QuoteItemId = configuredQuoteItem.QuoteItemId,
                                    QuoteId = quoteId,
                                    CodeString = configuredQuoteItem.CodeString,
                                    BaseProductId = (long)configuredQuoteItem.ProductId,
                                    RequiredQuantity = 1,
                                    Quantity = 1,
                                    OptionProductId = product.ProductId,
                                    OptionProductNumber = product.ProductNumber,
                                    OptionProductDescription = product.Name,
                                    ListPrice = product.ListPrice,
                                    //QuoteItemOptionTypeId = (byte)LineItemOptionTypeEnum.FactoryInstalled
                                    LineItemOptionTypeId = (byte)LineItemOptionTypeEnum.FactoryInstalled
                                };

                                configuredQuoteItem.QuoteItemOptions.Add(QuoteItemOption);
                            }
                            else if (product != null && accessory.AccessoryType == "N")
                            {
                                //Add Field Installed Item as normal Quote Item
                                var fieldInstalledItem = new QuoteItem();
                                fieldInstalledItem.ProductId = product.ProductId;
                                fieldInstalledItem.Quantity = 1;
                                fieldInstalledItem.ListPrice = product.ListPrice;
                                fieldInstalledItem.CodeString = package.Model; //Configured ModelNumber
                                fieldInstalledItem.LineItemTypeId = (byte?)LineItemTypeEnum.Standard;
                                fieldInstalledItem.Tags = package.SystemId;

                                QuoteItemsList.Add(fieldInstalledItem);
                            }
                        }

                        QuoteItemsList.Add(configuredQuoteItem);

                    }
                }
            }

            return QuoteItemsList;
        }

        public ServiceResponse ImportProductsFromBasket(UserSessionModel user, QuoteModel model)
        {
            Log.InfoFormat("Enter ImportProductsFromBasket for {0}", model.GetType());

            Log.DebugFormat("Start getting UserBasketModel for user: {0}", user.Email);

            this.Response = basketServices.GetUserBasketModel(user);

            this.Context.ReadOnly = false;

            if (this.Response.IsOK)
            {
                Log.Debug("GetUserBasketModel successed. Start Import Product process");

                var basket = this.Response.Model as UserBasketModel;

                var items = basket.Items.Select(basketItem => new QuoteItem { ProductId = basketItem.ItemId, Quantity = basketItem.Quantity }).ToList();

                Log.DebugFormat("Start AdjustQuoteItems for user: {0}, quote: {1}, total items: {2}",
                           user.Email, model.Title, items.Count());

                AdjustQuoteItems(user, model, items, true);

                if (this.Response.IsOK)
                {
                    Log.Debug("AdjustQuoteitems successed. Start clearing Basket");
                    basketServices.Clear(user);
                    Log.Debug("Clearing Basket finisged.");
                }

            }

            this.Response.Model = model;

            Log.InfoFormat("ImportProductsFromBasket for Quote: {O} finished", model.Title);

            return this.Response;

        }

        public ServiceResponse AdjustQuoteItems(UserSessionModel user, QuoteItemsModel quoteItemsModel)
        {
            this.Response = new ServiceResponse();
            try
            {
                QuoteModel quote = new QuoteModel()
                {
                    QuoteId = quoteItemsModel.QuoteId
                };

                List<QuoteItem> quoteItems = new List<QuoteItem>();
                foreach (var item in quoteItemsModel.Items)
                {
                    QuoteItem _item = new QuoteItem();
                    _item.QuoteId = (long)item.QuoteId;
                    _item.QuoteItemId = (long)item.QuoteItemId;
                    _item.Quantity = item.Quantity;
                    _item.Tags = item.Tags;
                    _item.CodeString = item.CodeString;
                    _item.LineItemTypeId = item.LineItemTypeId;
                    quoteItems.Add(_item);
                }

                foreach (QuoteItem item in quoteItems)
                {
                    if (item.Quantity == 0)
                    {
                        item.QuoteId = quoteItemsModel.QuoteId.Value;
                        RemoveProductsFromQuote(user, item);
                        this.Response.Messages.AddInformation("Item(s)-Removed");
                        // This message is used on client side (in quoteSvc.adjustQuoteItems() callback function)
                        // to check if quoteItems grid need to be refreshed.
                    }
                }

                AdjustQuoteItems(user, quote, quoteItems, false);
                this.Response.Model = quoteItemsModel.Items;
                return this.Response;

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Exception Source: {0}", ex.Source);
                Log.FatalFormat("Exception: {0}", ex.Message);
                this.Response.AddError(ex.Message);
                return this.Response;
            }


            //try
            //{
            //    var quoteAdjustments = Json.Decode<List<QuoteItem>>(quoteItemUpdates);

            //    foreach (QuoteItem item in quoteAdjustments)
            //    {
            //        if (item.Quantity == 0)
            //        {
            //            item.QuoteId = model.QuoteId.Value;
            //            RemoveProductsFromQuote(user, item);
            //        }
            //    }

            //    Log.Debug("Calling AdjustQuoteItems");
            //    AdjustQuoteItems(user, model, quoteAdjustments, false);

            //    Log.DebugFormat("Calling CalculateUnitCounts for {0}", model.GetType());
            //    CalculateUnitCounts(user, model);
            //}
            //catch (Exception ex)
            //{
            //    Log.FatalFormat("Exception Source: {0}", ex.Source);
            //    Log.FatalFormat("Exception: {0}", ex.Message);
            //}
        }
        private void AdjustQuoteItems(UserSessionModel user, QuoteModel model, List<QuoteItem> QuoteItemUpdates, bool isAdd, Quote entity = null)
        {
            Log.InfoFormat("Enter AdjustQuoteItems for quote: {0}, Total Items: {1}, isAdd: {2}",
                          model.Title, QuoteItemUpdates.Count(), isAdd);

            this.Context.ReadOnly = false;

            var quote = entity;

            if (entity == null)
            {
                Log.DebugFormat("Entity is not null.Start getting Entity for QuoteModel");

                quote = GetEntity(user, model);
            }

            if (quote.CommissionRequestId == null)
            {
                Log.Debug("CommissionRequestId is not null.Start setting commissionRequestid for Entity");

                if (model.CommissionRequestId.HasValue)
                {
                    quote.CommissionRequestId = model.CommissionRequestId;
                }

            }

            if (quote != null && QuoteItemUpdates != null && QuoteItemUpdates.Count > 0)
            {

                Log.DebugFormat("QuoteItemupdate is not null and ItemUpdate is greater than 0." +
                                "Start getting QuoteItem for user: {0} with quoteId: {1}",
                                user.Email, model.QuoteId);

                var quoteItems = Db.QuoteItemsByQuoteId(user, model.QuoteId).ToList();

                QuoteItemUpdates.ForEach(import =>
                {
                    var quoteItem = new QuoteItem();
                    if (import.LineItemTypeId == (byte?)LineItemTypeEnum.Configured) // Configured QuoteItem
                    {

                        //*******If congfigured item already exists then update quantity**********
                        quoteItem = (import.QuoteItemId == 0) ?
                                     quoteItems.Where(i => (i.ProductId == import.ProductId && i.CodeString == import.CodeString)).FirstOrDefault() : //Add to existing one
                                     quoteItems.Where(i => i.QuoteItemId == import.QuoteItemId).FirstOrDefault();//Adjust quantity

                        if (quoteItem == null && import.ProductId > 0)// new quoteItem
                        {
                            quoteItem = Db.QuoteItemCreate(quote);
                            quoteItem.ProductId = import.ProductId;
                            quoteItem.Quantity = 0;
                            quoteItem.ListPrice = import.ListPrice;
                            quoteItem.CodeString = import.CodeString;
                            quoteItem.LineItemTypeId = import.LineItemTypeId;
                            quoteItem.ConfigType = import.ConfigType;

                            //Add QuoteItemOptions
                            if (import.QuoteItemOptions.Count > 0)
                            {
                                foreach (var item in import.QuoteItemOptions)
                                {
                                    var QuoteItemOption = Db.QuoteItemOptionCreate(quoteItem);
                                    QuoteItemOption.CodeString = quoteItem.CodeString;
                                    QuoteItemOption.OptionProductId = item.OptionProductId;
                                    QuoteItemOption.OptionProductNumber = item.OptionProductNumber;
                                    QuoteItemOption.OptionProductDescription = item.OptionProductDescription;
                                    QuoteItemOption.RequiredQuantity = 1;
                                    QuoteItemOption.Quantity = 1;
                                    QuoteItemOption.ListPrice = item.ListPrice;

                                    QuoteItemOption.LineItemOptionTypeId = item.LineItemOptionTypeId;

                                    quoteItem.QuoteItemOptions.Add(QuoteItemOption);
                                }

                            }
                        }


                    }
                    else// Standard QuoteItem
                    {

                        quoteItem = (import.QuoteItemId == 0) ?
                                 quoteItems.Where(i => i.ProductId == import.ProductId && i.LineItemTypeId != (byte?)LineItemTypeEnum.Configured).FirstOrDefault() :
                                 quoteItems.Where(i => i.QuoteItemId == import.QuoteItemId).FirstOrDefault();

                        //Has quote got the item
                        if (quoteItem == null && import.ProductId > 0)// new quoteItem
                        {
                            //Rest of data added during rule processing
                            Log.Debug("quoteItem is null.Start create quoteItem");

                            quoteItem = Db.QuoteItemCreate(quote);
                            quoteItem.ProductId = import.ProductId;
                            quoteItem.Quantity = 0;
                            quoteItem.LineItemTypeId = (byte?)LineItemTypeEnum.Standard;
                            quoteItem.CodeString = import.CodeString; // This is for Field Installed Item
                        }
                    }


                    if (quoteItem != null)
                    {

                        Log.Debug("quoteItem is not null.Start adding tags process");

                        if (import.Tags != null)
                        {
                            if (quoteItem.Tags != import.Tags)
                            {
                                if (quoteItem.Tags != null)
                                {
                                    Log.Debug("quoteItem.Tags is not null.Start checking for existed tags");

                                    if (isAdd)
                                    {
                                        if (!quoteItem.Tags.Contains(import.Tags))
                                        {
                                            Log.Debug("tags not exised.Start adding tags to quoteItem");

                                            quoteItem.Tags = quoteItem.Tags + ", " + import.Tags;
                                        }
                                    }
                                    else
                                    {
                                        // isEdit
                                        Log.Debug("tags existed. Start updating tags for quoteItem");
                                        quoteItem.Tags = import.Tags;
                                    }
                                }
                                else
                                {
                                    quoteItem.Tags = import.Tags;
                                }
                            }


                        }

                        quoteItem.Quantity = import.Quantity + ((isAdd) ? quoteItem.Quantity : 0M);

                        //Update QuoteItemOption Quantity
                        if (quoteItem.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                        {
                            var QuoteItemOptions = Db.QuoteItemOptionsByQuoteItemId(user, quoteItem.QuoteItemId).ToList();
                            foreach (var optionItem in QuoteItemOptions)
                            {
                                optionItem.Quantity = optionItem.RequiredQuantity * (int)quoteItem.Quantity;
                            }
                        }
                        Log.DebugFormat("quoteItem quantity: {0}", quoteItem.Quantity);
                    }


                });


                this.Context.Entry(quote).State = EntityState.Modified;

                Log.DebugFormat("Start ApplyBusinessRules for quote: {0}", quote.QuoteId);

                ApplyBusinessRules(user, quote);


                if (this.Response.IsOK)
                {
                    Log.DebugFormat("ApplybusinessRule for quote: {0} successed.", quote.Title);
                    Log.Debug("Start writing to database");

                    try
                    {
                        base.SaveToDatabase(model, quote, string.Format("Quote '{0}'", quote.Title));
                    }
                    catch (Exception ex)
                    {
                        Log.FatalFormat("Exception source: {0}", ex.Source);
                        Log.FatalFormat("Exception: {0}", ex.Message);
                        Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                    }
                }

                // In order to catch the newly added product we must recalculate the commission after save of the quote
                if (quote.CommissionRequestId != null)
                {
                    Log.Debug("quote has CommissionRquest.Start Recalculate Commission process");

                    CommissionRecalculate(user, quote);
                }
            }

            //return;
            //modify here to see if the quote update
            //FinaliseModel(user, model);
            //this.Response.Model = model;

            return;

        }

        #endregion

        #region AdjustQuoteItemsConfigured
        private void AdjustConfiguredQuoteItems(UserSessionModel user, QuoteModel model, LCSTPackageModel configuredItem, bool isAdd, Quote entity = null)
        {

            this.Context.ReadOnly = false;

            var quote = entity;

            if (entity == null)
            {
                Log.DebugFormat("Entity is not null.Start getting Entity for QuoteModel");

                quote = GetEntity(user, model);
            }

            if (quote.CommissionRequestId == null)
            {
                Log.Debug("CommissionRequestId is not null.Start setting commissionRequestid for Entity");

                if (model.CommissionRequestId.HasValue)
                {
                    quote.CommissionRequestId = model.CommissionRequestId;
                }

            }

            if (quote != null && configuredItem != null)
            {

                Log.DebugFormat("QuoteItemupdate is not null and ItemUpdate is greater than 0." +
                                "Start getting QuoteItem for user: {0} with quoteId: {1}",
                                user.Email, model.QuoteId);

                var quoteItems = Db.QuoteItemsByQuoteId(user, model.QuoteId).ToList();

                //QuoteItemUpdates.ForEach(import =>
                //{
                //    var quoteItem = (import.QuoteItemId == 0) ?
                //        quoteItems.Where(i => i.ProductId == import.ProductId).FirstOrDefault() :
                //        quoteItems.Where(i => i.QuoteItemId == import.QuoteItemId).FirstOrDefault();

                //    //Has quote got the item
                //    if (quoteItem == null && import.ProductId > 0)
                //    {
                //        //Rest of data added during rule processing
                //        Log.Debug("quoteItem is null.Start create quoteItem");

                //        quoteItem = Db.QuoteItemCreate(quote);
                //        quoteItem.ProductId = import.ProductId;
                //        quoteItem.Quantity = 0;
                //    }

                //    if (quoteItem != null)
                //    {

                //        Log.Debug("quoteItem is not null.Start adding tags process");

                //        //if (import.Tags != null)
                //        //{
                //        //    if (quoteItem.Tags != import.Tags)
                //        //    {
                //        //        if (quoteItem.Tags != null)
                //        //        {
                //        //            Log.Debug("quoteItem.Tags is not null.Start checking for existed tags");

                //        //            if (isAdd)
                //        //            {
                //        //                if (!quoteItem.Tags.Contains(import.Tags))
                //        //                {
                //        //                    Log.Debug("tags not exised.Start adding tags to quoteItem");

                //        //                    quoteItem.Tags = quoteItem.Tags + ", " + import.Tags;
                //        //                }
                //        //            }
                //        //            else
                //        //            {
                //        //                // isEdit
                //        //                Log.Debug("tags existed. Start updating tags for quoteItem");
                //        //                quoteItem.Tags = import.Tags;
                //        //            }
                //        //        }
                //        //        else
                //        //        {
                //        //            quoteItem.Tags = import.Tags;
                //        //        }
                //        //    }


                //        //}

                //        quoteItem.Quantity = import.Quantity + ((isAdd) ? quoteItem.Quantity : 0M);
                //        Log.DebugFormat("quoteItem quantity: {0}", quoteItem.Quantity);
                //    }
                //});

                var baseProductId = productServices.GetProductId(user, configuredItem.BaseModel);
                if (baseProductId > 0)
                {
                    var quoteItem = Db.QuoteItemCreate(quote);
                    quoteItem.ProductId = baseProductId;
                    quoteItem.Quantity = 1;
                    quoteItem.ListPrice = CalculateConfiguredItemPrice(user, configuredItem);
                    quoteItem.CodeString = configuredItem.Model;
                    quoteItem.LineItemTypeId = (byte?)LineItemTypeEnum.Configured;
                }


                //AddConfiguredItemComponents () // QuoteItemOptions


                this.Context.Entry(quote).State = EntityState.Modified;

                Log.DebugFormat("Start ApplyBusinessRules for quote: {0}", quote.QuoteId);

                ApplyBusinessRules(user, quote);// This reset prices


                if (this.Response.IsOK)
                {
                    Log.DebugFormat("ApplybusinessRule for quote: {0} successed.", quote.Title);
                    Log.Debug("Start writing to database");

                    try
                    {
                        base.SaveToDatabase(model, quote, string.Format("Quote '{0}'", quote.Title));
                    }
                    catch (Exception ex)
                    {
                        Log.FatalFormat("Exception source: {0}", ex.Source);
                        Log.FatalFormat("Exception: {0}", ex.Message);
                        Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
                    }
                }

                // In order to catch the newly added product we must recalculate the commission after save of the quote
                if (quote.CommissionRequestId != null)
                {
                    Log.Debug("quote has CommissionRquest.Start Recalculate Commission process");

                    CommissionRecalculate(user, quote);
                }
            }


            return;

        }
        #endregion

        public decimal CalculateConfiguredItemPrice(UserSessionModel user, LCSTPackageModel configuredItem)
        {
            decimal configuredItemPrice = 0;

            var baseModel = productServices.GetProductbyProductNumber(user, configuredItem.BaseModel);
            if (baseModel != null)
            {
                configuredItemPrice += baseModel.ListPrice;
            }

            foreach (var accessory in configuredItem.Accessories)
            {
                //if (accessory.AccessoryType != (byte)LineItemOptionTypeEnum.FieldInstalled)
                if (accessory.AccessoryType != "N")
                {
                    var item = productServices.GetProductbyProductNumber(user, accessory.AccessoryModel);
                    if (item != null)
                    {
                        configuredItemPrice += item.ListPrice;
                    }
                }

            }

            return configuredItemPrice;
        }

        #region Post Model To Entity

        private Quote ModelToEntity(UserSessionModel admin, QuoteModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.Title = Utilities.Trim(model.Title);

            entity.Active = model.Active;

            if (admin.ShowPrices)
            {
                entity.IsCommissionScheme = model.IsCommissionScheme;

                if (entity.IsCommissionScheme)
                {
                    entity.Multiplier = model.Multiplier;

                    entity.DiscountPercentage = Db.GetCommissionMultipliers().Where(m => model.Multiplier == m.Multiplier).Select(m => m.CommissionPercentage).FirstOrDefault();
                }

                entity.TotalFreight = model.TotalFreight;

                //No needed anymore. Front end input has been changed. This is for Buy/Sell-Commission Percentage. Not for Commission Quote. ... Naming is confusing...
                //model.CommissionPercentage /= 100M;

                model.DiscountPercentage /= 100M;

                entity.CommissionPercentage = model.CommissionPercentage;

                entity.DiscountPercentage = model.DiscountPercentage;

                entity.IsGrossMargin = model.IsGrossMargin;

                entity.CommissionConvertYes = model.CommissionConvertYes;

                entity.CommissionConvertNo = model.CommissionConvertNo;

                entity.IsCommission = model.IsCommission;
            }

            entity.Description = Utilities.Trim(model.Description);

            entity.Notes = Utilities.Trim(model.Notes);


            entity.Deleted = model.Deleted;

            ModelToEntityConcurrenyProcessing(entity, model);


            entity.IsCommission = model.IsCommission;

            entity.CommissionConvertNo = model.CommissionConvertNo;

            entity.CommissionConvertYes = model.CommissionConvertYes;

            return entity;
        }

        #endregion

        private Quote GetEntity(UserSessionModel user, QuoteModel model)
        {
            Log.InfoFormat("Enter GetEntity for quote: {0}", model.Title);

            Quote entity;

            if (!model.QuoteId.HasValue)
            {
                Log.DebugFormat("quoteId is null.Start create new Quote for projectId: {0}", model.ProjectId);

                entity = Db.QuoteCreate(model.ProjectId);
            }
            else
            {
                Log.DebugFormat("quoteId is not null. Start getting Quote Entity " +
                                "for quoteId: {0}, that viewable by user: {1}",
                                model.QuoteId, user.Email);

                entity = this.Db.QueryQuoteViewableByQuoteId(user, model.QuoteId).FirstOrDefault();
            }

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM010);
                Log.Error(this.Response.Messages.Items.Last());
            }
            else
            {
                //mass upload change - turned this off
                Log.DebugFormat("Start getting Project Owner and Businees for projectId: {0}", entity.ProjectId);
                Db.GetProjectOwnerAndBusiness(entity.ProjectId);
            }

            Log.DebugFormat("GetEntity for quote: {0} finished.", model.QuoteId);

            return entity;
        }

        public ServiceResponse CheckProductWithNoClassCode(UserSessionModel user, long quoteId)
        {
            this.Response = GetQuoteItems(user, quoteId);

            if (this.Response.IsOK)
            {
                var quoteItemsModel = this.Response.Model as List<QuoteItemModel>;

                foreach (QuoteItemModel quoteItem in quoteItemsModel)
                {
                    var productClassCode = this.Db.Context.Products
                                               .Where(p => p.ProductNumber == quoteItem.ProductNumber)
                                               .Select(p => p.ProductClassCode).FirstOrDefault();

                    if (productClassCode == string.Empty)
                    {
                        this.Response.AddError(Resources.ResourceModelProject.MP142);
                    }
                }

            }

            return this.Response;
        }


        public QuoteOptionsModel GetQuoteOptionsModel(UserSessionModel user, QuoteModel quoteModel)
        {
            QuoteOptionsModel quoteOptions = new QuoteOptionsModel();

            quoteOptions.CanEditQuote = CanEditQuote(user, quoteModel);
            quoteOptions.CanDeleteQuote = CanDeleteQuote(user, quoteModel);
            quoteOptions.CanUnDeleteQuote = CanUnDeleteQuote(user, quoteModel);
            quoteOptions.CanSetActive = CanSetActive(user, quoteModel);
            quoteOptions.CanRequestDiscount = CanRequestDiscount(user, quoteModel);
            quoteOptions.CanRequestCommission = CanRequestCommission(user, quoteModel);
            quoteOptions.CanCalculateCommission = CanCalculateCommission(user, quoteModel);
            quoteOptions.CanDuplicate = CanDuplicate(user, quoteModel);
            quoteOptions.CanAddProducts = CanAddProducts(user, quoteModel);
            quoteOptions.CanEditTags = CanEditTags(user, quoteModel);

            return quoteOptions;
        }

        public bool CanEditQuote(UserSessionModel user, QuoteModel quoteModel)
        {
            //bool canEditquote = false;
            if (!user.HasAccess(SystemAccessEnum.EditProject))
            {
                return false;
            }
            else if (quoteModel.IsTransferred || quoteModel.HasDAR)
            {
                return false;
            }
            else if (quoteModel.HasCOM && (quoteModel.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending || quoteModel.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved))
            {
                return false;
            }
            else if (quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Inactive
                      || quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.ClosedWon)
            {
                return false;
            }
            else if (quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.AwaitingCSR
                     || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.InProcess
                     || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
            {
                return false;
            }
            else
            {
                return true;
            }
            //TODO: QuotePrint?
            //return canEditquote;
        }
        public bool CanDeleteQuote(UserSessionModel user, QuoteModel quoteModel)
        {
            if (CanEditQuote(user, quoteModel) && quoteModel.Deleted == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanUnDeleteQuote(UserSessionModel user, QuoteModel quoteModel)
        {
            //bool canUnDeletequote = false;
            if (!quoteModel.Deleted)
            {
                return false;
            }
            else if (!user.HasAccess(SystemAccessEnum.EditProject) || !user.HasAccess(SystemAccessEnum.UndeleteProject))
            {
                return false;
            }
            else if (quoteModel.IsTransferred || quoteModel.HasDAR || quoteModel.HasCOM)
            {
                return false;
            }
            else if (quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Inactive)
            {
                return false;
            }
            else if (quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.AwaitingCSR
                     || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
            {
                return false;
            }
            else
            {
                return true;
            }
            //TODO: QuotePrint?
            //return canUnDeletequote;
        }

        public bool CanSetActive(UserSessionModel user, QuoteModel quoteModel)
        {

            if (quoteModel.Active)
            {
                return false;
            }
            else if (!user.HasAccess(SystemAccessEnum.EditProject))
            {
                return false;
            }
            else if (quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Inactive
                     || quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.ClosedWon)
            {
                return false;
            }
            else if (quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.AwaitingCSR
                     || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool CanRequestDiscount(UserSessionModel user, QuoteModel quoteModel)
        {
            bool canRequestDiscount = false;
            if (!quoteModel.IsCommission
                && CanEditQuote(user, quoteModel)
                && user.HasAccess(SystemAccessEnum.RequestDiscounts)
                && user.ShowPrices && quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Open)
            {
                canRequestDiscount = true;
            }
            return canRequestDiscount;
        }

        public bool CanRequestCommission(UserSessionModel user, QuoteModel quoteModel)
        {
            bool canRequestCommission = false;
            if (quoteModel.IsCommission
                && CanEditQuote(user, quoteModel)
                && user.HasAccess(SystemAccessEnum.RequestCommission)
                && user.ShowPrices && quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Open)
            {
                canRequestCommission = true;
            }
            return canRequestCommission;
        }

        public bool CanCalculateCommission(UserSessionModel user, QuoteModel quoteModel)
        {

            //bool canCalculateCommission = false;
            //if (quoteModel.IsCommission
            //    && CanEditQuote(user, quoteModel)
            //    && user.HasAccess(SystemAccessEnum.RequestCommission)
            //    && user.ShowPrices && quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Open)
            //{
            //    canCalculateCommission = true;
            //}

            if (!quoteModel.IsCommission || !user.ShowPrices)
            {
                return false;
            }
            else if (!user.HasAccess(SystemAccessEnum.RequestCommission) && !user.HasAccess(SystemAccessEnum.ViewRequestedCommission))
            {
                return false;
            }
            else if (quoteModel.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending || quoteModel.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved)
            {
                return false;
            }
            else if (quoteModel.HasOrder)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool CanDuplicate(UserSessionModel user, QuoteModel quoteModel)
        {

            if (!user.HasAccess(SystemAccessEnum.EditProject))
            {
                return false;
            }
            //else if (quoteModel.HasDAR || quoteModel.HasCOM)// Has DAR/COM Rejected?
            //{
            //    return false;
            //}
            else if (quoteModel.HasDAR && (quoteModel.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending))
            {
                return false;
            }
            else if (quoteModel.HasCOM && (quoteModel.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending))
            {
                return false;
            }

            else if (quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.Inactive
                      || quoteModel.ProjectStatusTypeId == (byte)ProjectStatusTypeEnum.ClosedWon)
            {
                return false;
            }
            else if (quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.AwaitingCSR
                    || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
            {
                return false;
            }
            else if (quoteModel.AwaitingDiscountRequest == true || quoteModel.AwaitingCommissionRequest == true)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool CanAddProducts(UserSessionModel user, QuoteModel quoteModel)
        {
            if (CanEditQuote(user, quoteModel))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CanEditTags(UserSessionModel user, QuoteModel quoteModel)
        {

            if (!user.HasAccess(SystemAccessEnum.EditProject))
            {
                return false;
            }
            else if (quoteModel.IsTransferred)
            {
                return false;
            }
            else if (quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted
                   || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.AwaitingCSR
                   || quoteModel.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Accepted)
            {
                return false;
            }
            else
            {
                return true;
            }

        }



        //Test
        public void ConvertPdfFromBase64String(string base64Str)
        {

            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);

            string nameFile = "Test Submittal" + ".pdf";

            string subPath = grandParent + "/CustomerDataFiles/Submittals";

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            string filePath = grandParent + "/CustomerDataFiles/Submittals/" + nameFile;

            using (FileStream stream = System.IO.File.Create(filePath))
            {
                byte[] byteArray = Convert.FromBase64String(base64Str);
                stream.Write(byteArray, 0, byteArray.Length);
            }

        }

        public void SaveLCSubmittalData(LCSTPackagesModel packagesModel)
        {


            string SubmittalDirectory = Utilities.GetSubmittalDirectory();


            foreach (var package in packagesModel.Packages)
            {
                string filePath = SubmittalDirectory + package.Model + ".pdf";
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    byte[] byteArray = Convert.FromBase64String(package.SubmittalPdf);
                    stream.Write(byteArray, 0, byteArray.Length);
                }
            }

        }

        public bool HasConfiguredModel(long quoteId)
        {
            var query = from quoteItem in Db.QuoteItems
                        where quoteItem.QuoteId == quoteId
                        select quoteItem;
            var hasConfiguredModel = query.Any(i => i.LineItemTypeId == (byte)LineItemTypeEnum.Configured);
            return hasConfiguredModel;
        }
    }
}