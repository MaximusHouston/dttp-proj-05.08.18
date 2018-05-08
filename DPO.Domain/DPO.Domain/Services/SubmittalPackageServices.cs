using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DPO.Domain 
{
    public partial class SubmittalPackageServices : BaseServices
    {
        public ProjectServices projectService;
        public HtmlServices htmlService;
        public QuoteServices quoteService;

        public SubmittalPackageServices() : base()
        {
            htmlService = new HtmlServices();
            projectService = new ProjectServices();
            quoteService = new QuoteServices();
        }

        public SubmittalPackageServices(DPOContext context)
            : base(context)
        {
             
        }

        public ServiceResponse GetQuoteQuotePackage(UserSessionModel admin, SubmittalRequestModel model)
        {
            Log.InfoFormat("Enter GetQuoteQuotePackge for {0}", model.GetType());
            Log.Debug("PageSize: " + model.PageSize);

            model.PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL;

            // get QuoteItems items now
            var search = new SearchQuoteItem(model as Search);

            Log.Debug("Searching Filter: " + search.Filter);

            if (!model.QuoteId.HasValue)
            {
                this.Response.AddError(Resources.DataMessages.DM010);
                Log.ErrorFormat(this.Response.Messages.Items.Last().Text);
            }

            var query = from quote in this.Db.QueryQuoteViewableByQuoteId(admin, model.QuoteId)
                        join project in this.Db.Projects on quote.ProjectId equals project.ProjectId

                        join active in this.Db.Quotes on new { id = project.ProjectId, active = true } equals new { id = active.ProjectId, active = active.Active } into Laq
                        from active in Laq.DefaultIfEmpty()

                        select new SubmittalRequestModel
                        {
                            ProjectId = quote.ProjectId,
                            QuoteId = quote.QuoteId,
                            Title = quote.Title,
                            ProjectName = project.Name,
                        };

            try
            {
                Log.DebugFormat("Start retrieve QuoteItemsModel for QuoteId: {0}", model.QuoteId);
                model = query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Exception Source: {0}", ex.Source);
                Log.FatalFormat("Exception: {0}", ex.Message);
                Log.FatalFormat("Exception Detail: {0}", ex.InnerException.Message);
            }

            if (model == null)
            {
                this.Response.AddError(Resources.DataMessages.DM010);
                Log.ErrorFormat("the return {0} model is null", model.GetType());
            }

            search.QuoteId = model.QuoteId;

            search.ReturnTotals = false;

            var response = GetSubmittalPackageQuoteItemListModel(admin, search);

            var items = response.Model as List<QuoteItemListModel>; // original items list

            //Get QuoteItemOptions
            var finalItemsList = new List<QuoteItemListModel>();

            foreach (var item in items)
            {
                if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
                {
                    model.HasConfiguredItem = true;
                    var optionItems = GetOptionItemsAsQuoteItemList(admin, (long)item.QuoteItemId);
                    finalItemsList.AddRange(optionItems);
                }
            }

            //Combine with original items list
            finalItemsList.AddRange(items);

            //Remove duplicated Items
            //finalItemsList = finalItemsList.GroupBy(i => i.ProductId).Select(i => i.First()).ToList();

            Log.DebugFormat("Total QuoteItemListModel return from search: {0}", finalItemsList.Count);

            model.Items = new PagedList<QuoteItemListModel>(finalItemsList, model);

            var products = finalItemsList.Cast<ProductModel>().ToList();

            Log.DebugFormat("Total Products return from search: {0}", products.Count);

            new ProductServices(this.Context).GetDocuments(products);

            var baseDirectory = Utilities.GetQuotePackageDirectory(model.QuoteId.Value);
            Log.DebugFormat("QuotePackageDirectory return: {0}", baseDirectory);

            var packagefiles = Directory.GetFiles(baseDirectory);

            Log.DebugFormat("Total files return from  QuotePackageDirectory (included lock files): {0}", packagefiles.Count());
            foreach (var file in packagefiles)
            {
                Log.Debug(file);
            }

            model.QuotePackage = new List<DocumentModel>();

            model.QuotePackage.AddRange(packagefiles.Select(f => new DocumentModel { Description = Path.GetFileName(f) }).ToList());

            var quotePackageFileName = Utilities.QuotePackageFileName(model.QuoteId.Value);

            var attachedfiles = Directory.GetFiles(baseDirectory).Where(f =>
            {
                var file = System.IO.Path.GetFileName(f);
                var isSystemPackageFile = file.StartsWith("DPO_QuotePackage_");
                var isLock = file.EndsWith(".lck");
                return !isSystemPackageFile && !isLock;
            }).ToList();

            Log.DebugFormat("Total files return from  QuotePackageDirectory : {0}", attachedfiles.Count());
            foreach (var attFile in attachedfiles)
            {
                Log.Debug(attachedfiles);
            }

            model.QuotePackageAttachedFiles = new List<DocumentModel>();

            model.QuotePackageAttachedFiles.AddRange(attachedfiles.Select(f =>
                new DocumentModel
                {
                    FileName = Path.GetFileName(f),
                    DocumentTypeId = (int)DocumentTypeEnum.QuotePackageAttachedFile,
                    Type = "QuotePackageAttachedFile",
                    Description = Path.GetFileName(f)
                }).ToList());

            Log.DebugFormat("Total QuotePackageAttachmentfiles have added to QuoteItemsModel: {0}", model.QuotePackageAttachedFiles.Count());
            foreach (var qpaf in model.QuotePackageAttachedFiles)
            {
                Log.Debug(qpaf.FileName);
            }

            this.Response.Model = model;
            Log.InfoFormat("GetQuoteQuotePackage finished");

            return this.Response;
        }

        public ServiceResponse GetSubmittalPackageQuoteItemListModel(UserSessionModel admin, SearchQuoteItem search)
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

        public List<QuoteItemListModel> GetOptionItemsAsQuoteItemList(UserSessionModel user, long quoteItemId)
        {

            var query = from q in this.Db.QuoteItemOptionsByQuoteItemId(user, quoteItemId)
                        join p in this.Db.Products on q.OptionProductId equals p.ProductId into Lp
                        from p in Lp.DefaultIfEmpty()
                        select new QuoteItemListModel
                        {
                            QuoteId = q.QuoteId,
                            ProductId = q.OptionProductId,
                            //PriceNet = q.ListPrice * q.Multiplier,
                            PriceList = q.ListPrice,
                            IsCommissionable = p.AllowCommissionScheme,
                            Quantity = q.Quantity,
                            ProductNumber = q.OptionProductNumber,
                            Description = q.OptionProductDescription,
                            QuoteItemId = q.QuoteItemId,
                            ProductClassCode = p.ProductClassCode,
                            SubmittalSheetTypeId = (SubmittalSheetTypeEnum)p.SubmittalSheetTypeId,
                            //Tags = q.Tags,
                            CodeString = q.CodeString,
                            //QuoteItemTypeId = q.QuoteItemTypeId
                            //LineItemTypeId = q.LineItemTypeId
                        };

            var optionItems = query.ToList();
            return optionItems;

        }                 
    }
}
