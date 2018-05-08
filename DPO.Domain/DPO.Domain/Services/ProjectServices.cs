
using CsvHelper;
using DPO.Common;
using DPO.Common.Models.General;
using DPO.Data;
using DPO.Domain.Services;
using DPO.Resources;
using EntityFramework;
using EntityFramework.Extensions;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DPO.Model.Light;
using System.Data;
using System.Text;
using StackExchange.Profiling;

namespace DPO.Domain
{
    public partial class ProjectServices : BaseServices
    {
        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
        private static HSSFWorkbook hssfworkbook;

        private HtmlServices htmlService;
        private AddressServices addressService;

        public ProjectServices()
            : base()
        {
            htmlService = new HtmlServices(this.Context);
            addressService = new AddressServices(this.Context);
        }

        public ProjectServices(DPOContext context)
            : base(context)
        {
            htmlService = new HtmlServices(context);
            addressService = new AddressServices(context);
        }

        #region Get Requests

        public string GenerateQuotePackageCoverPageFile(long quoteId, string content)
        {
            var file = string.Format("{0}\\DPO_QuotePackage_CoverPage_{1}.pdf", Utilities.GetQuotePackageDirectory(quoteId), quoteId);

            var pdf = new PdfConvertor();

            pdf.AppendHtml(content);

            pdf.WriteToFile(file);

            return file;
        }

        public string GenerateQuotePackageCoverPageFile(UserSessionModel user, long projectId, long quoteId)
        {
            var service = new ProductServices();

            var file = string.Format("{0}\\DPO_QuotePackage_CoverPage_{1}.pdf", Utilities.GetQuotePackageDirectory(quoteId), quoteId);

            // Regenerate every 24 hours

            var docUrl = Utilities.DocumentServerURL();

            var controller = string.Format("{0}/{1}", docUrl, "ProjectDashboard");

            var url = string.Format("{0}/{1}/{2}/{3}", controller, "QuotePackageCoverPage", projectId, quoteId);

            var pdf = new PdfConvertor();

            var web = new WebClientLocal(HttpContext.Current);

            pdf.AppendHtml(web.DownloadString(url));

            pdf.WriteToFile(file);

            return file;
        }

        public byte[] GetBytes(object obj)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                (obj as HSSFWorkbook).Write(buffer);
                return buffer.GetBuffer();
            }
        }

        private string ConvertToWorkbookName(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                text = text.Replace(c, '_');
            }

            return text;
        }

        public ServiceResponse GetProjectExportModel(UserSessionModel admin, ProjectsModel model)
        {
            model.PageSize = DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL;
            model.ReturnTotals = false;
            model.Page = 1;

            //default project export type to detailed, in case individual project is being exported
            model.ProjectExportType = model.ProjectExportType ?? ProjectExportTypeEnum.PipelineDetailed;

            var qry = (from project in this.Db.QueryProjectsViewableBySearch(admin, model).Include("ProjectPipelineNotes").Include("ProjectPipelineNoteTypes")

                       join os in this.Context.ProjectOpenStatusTypes on project.ProjectOpenStatusTypeId equals os.ProjectOpenStatusTypeId into Los
                       from os in Los.DefaultIfEmpty()

                       join ps in this.Context.ProjectStatusTypes on project.ProjectStatusTypeId equals ps.ProjectStatusTypeId into Lps
                       from ps in Lps.DefaultIfEmpty()

                       join pt in this.Context.ProjectTypes on project.ProjectTypeId equals pt.ProjectTypeId into Lpt
                       from pt in Lpt.DefaultIfEmpty()

                       join vt in this.Context.VerticalMarketTypes on project.VerticalMarketTypeId equals vt.VerticalMarketTypeId into Lvt
                       from vt in Lvt.DefaultIfEmpty()

                       join plst in this.Context.ProjectLeadStatusTypes on project.ProjectLeadStatusTypeId equals plst.ProjectLeadStatusTypeId into Lplst
                       from plst in Lplst.DefaultIfEmpty()

                       join quote in this.Context.Quotes on new { P = project.ProjectId, A = true } equals new { P = quote.ProjectId, A = quote.Active } into Lq
                       from quote in Lq.DefaultIfEmpty()

                           //This causes duplicated record
                       //join commission in this.Context.CommissionRequests on project.ProjectId equals commission.ProjectId into com
                       //from commission in com.DefaultIfEmpty()

                       join commission in this.Context.CommissionRequests
                       on new { C = quote.QuoteId, A = (byte?)6 } equals new { C = commission.QuoteId, A = commission.CommissionRequestStatusTypeId } into com
                       from commission in com.DefaultIfEmpty()


                       join transfer in this.Context.ProjectTransfers on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                       from transfer in Lt.DefaultIfEmpty()

                       join owner in this.Context.Users on project.OwnerId equals owner.UserId into Low
                       from owner in Low.DefaultIfEmpty()

                       join business in this.Context.Businesses on owner.BusinessId equals business.BusinessId into Lb
                       from business in Lb.DefaultIfEmpty()

                       select new
                       {
                           project,
                           os,
                           ps,
                           pt,
                           vt,
                           plst,
                           quote,
                           quote.QuoteItems,
                           transfer,
                           owner,
                           business,
                           commission,
                           latestProjectPipelineNote = project.ProjectPipelineNotes
                                .OrderByDescending(o => o.Timestamp)
                                .Select(ss => new
                                {
                                    noteTypeName = ss.ProjectPipelineNoteType.Name,
                                    note = ss.Note,
                                    noteTimestamp = ss.Timestamp
                                })
                                .Take(1).FirstOrDefault()
                       });

            var results = qry.ToList();

            List<ProjectExportModel> export = new List<ProjectExportModel>();

            long[] productIds = results.SelectMany(r => r.QuoteItems).Select(i => i.ProductId.Value).Distinct().ToArray();


            Db.ProductByProductIds(productIds).Load();

            List<ProductModelType> productModelTypes = Db.Context.ProductModelTypes.ToList();
            List<ProductCategory> productCategories = Db.Context.ProductCategories.ToList();

            foreach (var o in results)
            {
                //work out Daikin Account-related stuff regardless
                string RSMFullName = "";
                string CSMFullName = "";

                if ((o.business != null && o.business.AccountManagerFirstName != null))
                {
                    CSMFullName = o.business.AccountManagerFirstName;

                    if (o.business.AccountManagerLastName != null)
                    {
                        CSMFullName += " " + o.business.AccountManagerLastName;
                    }
                }

                if ((o.business != null && o.business.AccountOwnerFirstName != null))
                {
                    RSMFullName = o.business.AccountOwnerFirstName;

                    if (o.business.AccountOwnerLastName != null)
                    {
                        RSMFullName += " " + o.business.AccountOwnerLastName;
                    }
                }

                //pipeline report - just one row per project
                if (model.ProjectExportType == ProjectExportTypeEnum.Pipeline)
                {

                    #region Replaced with data saved in database (2016.01.05) - Delete after 2016.02.01
                    //work out quantities for a project - pipeline report only

                    //int vrvOutdoorUnitQty = 0;
                    //int rtuQty = 0;
                    //int splitOutdoorUnitQty = 0;
                    //int vrvIndoorUnitQty = 0;

                    //var calculator = new ProductComponentCalculator();

                    //if (o.QuoteItems != null && o.QuoteItems.Count > 0)
                    //{
                    //TODO
                    //i.      Count all units with VRV family and Model type as Outdoor on the quote [example: RXYQ120TTJU - unit count is 1]
                    //ii.      In case a units is large VRV outdoor unit (made of smaller outdoor units) on quote; count and add its individual components (unit module) to VRV-ODU# [Example: RXYQ360TTJU (composed of 3*RXYQ120TTJU) - unit count is 3]
                    //iii.      If above two units are listed on the quote then VRV-ODU# is 4
                    // vrvOutdoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //     && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV).Count();

                    //var vrvOutdoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //    && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                    //    .ToList();

                    ////vrvIndoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor
                    ////    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV).Count();

                    //var vrvIndoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //    && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                    //    .ToList();

                    ////TODO - In case a Model is System Type then count and add its individual Outdoor component to the mix.
                    ////splitOutdoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    ////    && (p.Product.ProductFamilyId == (int)ProductFamilyEnum.MiniSplit ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.Altherma ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.SkyAir ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit)).Count();

                    //var splitOutdoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //          && (p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //            || p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                    //          && (p.Product.ProductFamilyId == (int)ProductFamilyEnum.MiniSplit ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.Altherma ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.SkyAir ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit))
                    //          .ToList();

                    ////according to Deepak, RTU is actually the Packaged product family(??)
                    ////rtuQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    ////    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.Packaged).Count();

                    //var rtuList = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.Packaged)
                    //    .ToList();

                    //vrvOutdoorUnitQty = calculator.CalculateProductsComponent(vrvOutdoorUnitList);
                    //vrvIndoorUnitQty = calculator.CalculateProductsComponent(vrvIndoorUnitList);
                    //splitOutdoorUnitQty = calculator.CalculateProductsComponent(splitOutdoorUnitList, new int[] { (int)ProductModelTypeEnum.Outdoor });
                    //rtuQty = calculator.CalculateProductsComponent(rtuList);

                    //}

                    #endregion Replaced with data saved in database

                    var exportModel = new ProjectExportModel
                    {
                        // Project
                        ProjectReference = o.project.ProjectId.ToString(),
                        CRMAccountId = (o.business != null && o.business.AccountId != null) ? o.business.AccountId : "",
                        Region = (o.business != null && o.business.AccountOwningGroupName != null) ? o.business.AccountOwningGroupName : "",
                        RSM = RSMFullName,
                        CSM = CSMFullName,
                        BusinessName = (o.business != null && o.business.BusinessName != null) ? o.business.BusinessName : "",
                        SellerName = (o.project.SellerName == null) ? "" : o.project.SellerName,
                        ProjectOwnerName = o.project.Owner.FirstName + " " + o.project.Owner.LastName,
                        CustomerBusinessName = (o.project.CustomerName == null) ? "" : o.project.CustomerName,
                        EngineerFirm = (o.project.EngineerName == null) ? "" : o.project.EngineerName,
                        ProjectName = o.project.Name,
                        ProjectDate = o.project.ProjectDate,
                        ProjectType = (o.pt == null) ? "" : o.pt.Description,
                        ProjectOpenStatus = (o.os == null) ? "" : o.os.Description,
                        ProjectStatus = (o.ps == null) ? "" : o.ps.Description,
                        VerticalMarketDescription = (o.vt == null) ? "" : o.vt.Description,
                        BidDate = o.project.BidDate,
                        EstimatedCloseDate = o.project.EstimatedClose,
                        EstimatedDeliveryDate = o.project.EstimatedDelivery,
                        EstimatedDeliveryMonth = o.project.EstimatedDelivery.ToString("MMMM"),
                        ProjectExpirationDate = o.project.Expiration,
                        Transferred = (o.transfer != null) ? "Yes" : "",
                        ProjectNotes = (o.project.Description != null) ? o.project.Description.ToString() : "",

                        // Quote
                        TotalList = (o.quote == null) ? 0 : o.quote.TotalList,
                        TotalNet = (o.quote == null) ? 0 : o.quote.TotalNet,
                        TotalSell = (o.quote == null) ? 0 : o.quote.TotalSell,
                        VRVOutdoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountVRVOutdoor,

                        // TODO:  Calculate RTU
                        RTUQty = 0,
                        SplitOutdoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountSplitOutdoor,
                        VRVIndoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountVRVIndoor,
                        QuoteReference = (o.quote == null) ? "" : o.quote.QuoteId.ToString(),
                        QuoteName = (o.quote == null) ? "" : o.quote.Title.ToString(),
                        QuoteNotes = (o.quote != null && o.quote.Notes != null) ? o.quote.Notes.ToString() : "",
                        Revision = (o.quote == null) ? 0 : o.quote.Revision,
                        IsGrossMargin = (o.quote == null) ? "" : o.quote.IsGrossMargin.ToString(),
                        IsCommissionScheme = (o.quote == null) ? "" : o.quote.IsCommissionScheme.ToString(),
                        TotalFreight = (o.quote == null) ? 0 : o.quote.TotalFreight,
                        CommissionPercentage = (o.quote == null) ? 0 : o.quote.CommissionPercentage,
                        DiscountPercentage = (o.quote == null) ? 0 : o.quote.DiscountPercentage,
                    };

                    if (admin != null && admin.HasAccess(SystemAccessEnum.ViewPipelineData))
                    {
                        exportModel.ProjectLeadStatus = (o.plst == null) ? ResourceUI.Lead : o.plst.Name;

                        // Pipeline Note
                        if (o.latestProjectPipelineNote != null)
                        {
                            exportModel.ProjectPipelineNote = o.latestProjectPipelineNote.note;
                            exportModel.ProjectPipelineNoteType = o.latestProjectPipelineNote.noteTypeName;
                            exportModel.ProjectPipelineNoteDate = o.latestProjectPipelineNote.noteTimestamp;
                        }
                    }

                    export.Add(exportModel);
                }
                //detailed report - show all quote items for each project
                else if (model.ProjectExportType == ProjectExportTypeEnum.PipelineDetailed)
                {
                    long[] productsInActiveQuoteId = new long[] { };
                    Dictionary<long, ProductSpecificationsModel> allProductSpecsInQuote = new Dictionary<long, ProductSpecificationsModel>();

                    if (o.QuoteItems == null || o.QuoteItems.Count == 0)
                    {
                        o.QuoteItems.Add(new QuoteItem { }); // fake one record
                    }
                    else
                    {
                        //get all relevant product specs for all items in quote
                        productsInActiveQuoteId = o.QuoteItems.Select(qi => (long)qi.ProductId).ToArray();
                        allProductSpecsInQuote = Db.GetProductSpecifications(productsInActiveQuoteId, new string[] { "PowerVoltage", "UnitInstallationType" });
                    }

                    
                    foreach (var quoteItem in o.QuoteItems)
                    {
                        string powerVoltage = "";
                        string unitInstallationType = "";

                        if (allProductSpecsInQuote.Count > 0)
                        {
                            //get model from specs list that contains all the specs for this product
                            ProductSpecificationsModel productSpecs = allProductSpecsInQuote.Where(ps => ps.Key == quoteItem.ProductId).FirstOrDefault().Value;

                            if (productSpecs != null && productSpecs.All != null && productSpecs.All.Count > 0)
                            {
                                //find the actual specs hidden inside the list of containers, and assign
                                List<ProductSpecificationModel> actualSpecs = productSpecs.All.Select(lps => lps.Value).ToList();

                                //power voltage value is straight-forward
                                powerVoltage = actualSpecs.Where(pos => pos.Name == "PowerVoltage").Select(pos => pos.Value).FirstOrDefault();

                                //unit installation type needs to be converted to 'HP (Heat Pump')' or 'HR (Heat recovery)'
                                unitInstallationType = actualSpecs.Where(pos => pos.Name == "UnitInstallationType").Select(pos => pos.Value).FirstOrDefault();
                            }
                        }

                        //var oduQuantity = new ProductComponentCalculator().CalculateProductsComponent(quoteItem);

                        ProjectExportModel projectExportModel = new ProjectExportModel
                        {
                            // Project
                            ProjectReference = o.project.ProjectId.ToString(),
                            CRMAccountId = (o.business != null && o.business.AccountId != null) ? o.business.AccountId : "",
                            Region = (o.business != null && o.business.AccountOwningGroupName != null) ? o.business.AccountOwningGroupName : "",
                            RSM = RSMFullName,
                            CSM = CSMFullName,
                            BusinessName = (o.business != null && o.business.BusinessName != null) ? o.business.BusinessName : "",
                            SellerName = (o.project.SellerName == null) ? "" : o.project.SellerName,
                            ProjectOwnerName = o.project.Owner.FirstName + " " + o.project.Owner.LastName,
                            CustomerBusinessName = (o.project.CustomerName == null) ? "" : o.project.CustomerName,
                            EngineerFirm = (o.project.EngineerName == null) ? "" : o.project.EngineerName,
                            ProjectName = o.project.Name,
                            ProjectDate = o.project.ProjectDate,
                            ProjectType = (o.pt == null) ? "" : o.pt.Description,
                            ProjectOpenStatus = (o.os == null) ? "" : o.os.Description,
                            ProjectStatus = (o.ps == null) ? "" : o.ps.Description,
                            VerticalMarketDescription = (o.vt == null) ? "" : o.vt.Description,
                            BidDate = o.project.BidDate,
                            EstimatedCloseDate = o.project.EstimatedClose,
                            EstimatedDeliveryDate = o.project.EstimatedDelivery,
                            ProjectExpirationDate = o.project.Expiration,
                            Transferred = (o.transfer != null) ? "Yes" : "",
                            ProjectNotes = (o.project.Description != null) ? o.project.Description.ToString() : "",

                            // Quote
                            TotalList = (o.quote == null) ? 0 : o.quote.TotalList,
                            TotalNet = (o.quote == null) ? 0 : o.quote.TotalNet,
                            TotalSell = (o.quote == null) ? 0 : o.quote.TotalSell,
                            QuoteReference = (o.quote == null) ? "" : o.quote.QuoteId.ToString(),
                            QuoteName = (o.quote == null) ? "" : o.quote.Title.ToString(),
                            QuoteNotes = (o.quote != null && o.quote.Notes != null) ? o.quote.Notes.ToString() : "",
                            Revision = (o.quote == null) ? 0 : o.quote.Revision,
                            IsGrossMargin = (o.quote == null) ? "" : o.quote.IsGrossMargin.ToString(),
                            IsCommissionScheme = (o.quote == null) ? "" : o.quote.IsCommissionScheme.ToString(),
                            TotalFreight = (o.quote == null) ? 0 : o.quote.TotalFreight,
                            CommissionPercentage = (o.quote == null) ? 0 : o.quote.CommissionPercentage,
                            DiscountPercentage = (o.quote == null) ? 0 : o.quote.DiscountPercentage,

                            //Commission Request
                            RequestedCommissionPercent = (o.commission == null || o.commission.RequestedCommissionPercent == null) ? 0 : o.commission.RequestedCommissionPercent.Value,
                            ApprovedCommissionPercent = (o.commission == null || o.commission.ApprovedCommissionPercent == null) ? 0 : o.commission.ApprovedCommissionPercent.Value,
                            RequestedMultiplier = (o.commission == null || o.commission.RequestedMultiplier == null) ? 0 : o.commission.RequestedMultiplier.Value,
                            ApprovedMultiplier = (o.commission == null || o.commission.ApprovedMultiplier == null) ? 0 : o.commission.ApprovedMultiplier.Value,

                            RequestedCommissionSplitPercent = (o.commission == null || o.commission.RequestedCommissionPercentSplit == null) ? 0 : o.commission.RequestedCommissionPercentSplit.Value,
                            ApprovedCommissionSplitPercent = (o.commission == null || o.commission.ApprovedCommissionPercentSplit == null) ? 0 : o.commission.ApprovedCommissionPercentSplit.Value,
                            RequestedCommissionMultiplierSplit = (o.commission == null || o.commission.RequestedMultiplierSplit == null) ? 0 : o.commission.RequestedMultiplierSplit.Value,
                            ApprovedCommissionMultiplierSplit = (o.commission == null || o.commission.ApprovedMultiplierSplit == null) ? 0 : o.commission.ApprovedMultiplierSplit.Value,

                            RequestedCommissionVRVPercent = (o.commission == null || o.commission.RequestedCommissionPercentVRV == null) ? 0 : o.commission.RequestedCommissionPercentVRV.Value,
                            ApprovedCommissionVRVPercent = (o.commission == null || o.commission.ApprovedCommissionPercentVRV == null) ? 0 : o.commission.ApprovedCommissionPercentVRV.Value,
                            RequestedCommissionMultiplierVRV = (o.commission == null || o.commission.RequestedMultiplierVRV == null) ? 0 : o.commission.RequestedMultiplierVRV.Value,
                            ApprovedCommissionMultiplierVRV = (o.commission == null || o.commission.ApprovedMultiplierVRV == null) ? 0 : o.commission.ApprovedMultiplierVRV.Value,

                            IsCommissionRequest = (o.quote == null) ? false : o.quote.IsCommission,

                            // Product
                            ProductNumber = (quoteItem == null) ? "" : quoteItem.ProductNumber,
                            ProductDescription = (quoteItem == null) ? "" : quoteItem.Description,
                            ProductModelType = (quoteItem == null || quoteItem.Product == null || quoteItem.Product.ProductModel == null || string.IsNullOrEmpty(quoteItem.Product.ProductModel.Description)) ? "" : quoteItem.Product.ProductModel.Description.ToString(),
                            //product model type == product category
                            ProductType = (quoteItem == null || quoteItem.Product == null || quoteItem.Product.ProductCategory == null || string.IsNullOrEmpty(quoteItem.Product.ProductCategory.Name)) ? "" : quoteItem.Product.ProductCategory.Name.ToString(),
                            //HP/HR == unit installation type
                            HpHr = unitInstallationType,
                            Voltage = powerVoltage,
                            Quantity = (quoteItem == null) ? 0 : quoteItem.Quantity,
                            PriceList = (quoteItem == null) ? 0 : quoteItem.ListPrice,
                            PriceNet = (quoteItem == null) ? 0 : (quoteItem.ListPrice * quoteItem.Multiplier),
                            ExtendedNetPrice = (quoteItem == null) ? 0 : ((quoteItem.ListPrice * quoteItem.Multiplier) * quoteItem.Quantity),
                            ProductClassCode = (quoteItem == null || quoteItem.Product == null) ? "" : quoteItem.Product.ProductClassCode
                        };

                        if (admin != null && admin.HasAccess(SystemAccessEnum.ViewPipelineData))
                        {
                            projectExportModel.ProjectLeadStatus = (o.plst == null) ? ResourceUI.Lead : o.plst.Name;

                            // Pipeline Note
                            if (o.latestProjectPipelineNote != null)
                            {
                                projectExportModel.ProjectPipelineNote = o.latestProjectPipelineNote.note;
                                projectExportModel.ProjectPipelineNoteType = o.latestProjectPipelineNote.noteTypeName;
                                projectExportModel.ProjectPipelineNoteDate = o.latestProjectPipelineNote.noteTimestamp;
                            }
                        }

                        export.Add(projectExportModel);
                            
                    }
                }
            }

            if (this.Response.IsOK)
            {
                StringWriter sw = new StringWriter();
                int n = -1;

                var csv = new CsvWriter(sw);

                if (export.Count > 0)
                {
                    //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                    // Initialize NPOI
                    InitializeWorkbook();
                    ISheet sheet1 = hssfworkbook.CreateSheet("DPOProjectExport-" + model.ProjectExportType.ToString());
                    IDataFormat dataFormatCustom = hssfworkbook.CreateDataFormat();

                    var dateCellStyle = hssfworkbook.CreateCellStyle();
                    dateCellStyle.DataFormat = dataFormatCustom.GetFormat(ResourceUI.DateFormat);

                    var percentCellStyle = hssfworkbook.CreateCellStyle();
                    percentCellStyle.DataFormat = dataFormatCustom.GetFormat("0%");

                    var currencyCellStyle = hssfworkbook.CreateCellStyle();
                    currencyCellStyle.DataFormat = dataFormatCustom.GetFormat("$#,##0.00_);-$#,##0.00_)");

                    for (var k = 0; k < export.Count; k++)
                    {
                        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                        //Create the NPOI row object here and initialize it to the 1st row which is zero based.
                        n++;
                        IRow row = sheet1.CreateRow(n);

                        Dictionary<string, object> columns = this.ProjectExportModelToCsv(admin, export[k], model.ProjectExportType);

                        int cellCnt = 0;
                        if (k == 0)
                        {
                            foreach (var header in columns)
                            {
                                //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                                //NPOI Cell Headers get set here.
                                row.CreateCell(cellCnt).SetCellValue(header.Key);
                                cellCnt++;

                                //csv.WriteField(header.Key);
                            }

                            //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                            //Initialize the next row here because the 1st row (header row) is row 0 and then it has to set the values for the next row in the foreach section below.
                            n = k + 1;
                            row = sheet1.CreateRow(n);

                            //csv.NextRecord();
                        }

                        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                        cellCnt = 0; //Initialize back to 0.
                        foreach (var column in columns)
                        {
                            var value = column.Value;
                            var name = column.Key;

                            var cell = row.CreateCell(cellCnt);

                            if (!admin.HasAccess(SystemAccessEnum.ViewPipelineData))
                            {

                            }

                            //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                            // Fix to only convert the reference numbers to strings
                            if (String.Compare(name, ResourceUI.ProjectReference, true) == 0
                                || String.Compare(name, ResourceUI.QuoteReference, true) == 0
                                || String.Compare(name, ResourceUI.ProjectRef, true) == 0)
                            {
                                cell.SetCellValue(column.Value.ToString());
                            }
                            else if (
                                 value is short
                                 || value is ushort
                                 || value is int
                                 || value is uint
                                 || value is long
                                 || value is ulong
                                 || value is float
                                 || value is double
                                 || value is decimal)
                            {
                                double dVal;

                                if (double.TryParse(value.ToString(), out dVal))
                                {
                                    cell.SetCellValue(dVal);

                                    // HACK:  Crazy hack
                                    if (String.Compare(name, ResourceUI.TotalNet, true) == 0
                                        || String.Compare(name, ResourceUI.TotalList, true) == 0
                                        || String.Compare(name, ResourceUI.TotalSell, true) == 0
                                        || String.Compare(name, ResourceUI.TotalFreight, true) == 0
                                        || String.Compare(name, ResourceUI.TotalPrice, true) == 0
                                        || String.Compare(name, ResourceUI.PriceList, true) == 0
                                        || String.Compare(name, ResourceUI.PriceNetEach, true) == 0
                                        || String.Compare(name, ResourceUI.ExtendedPrice, true) == 0)
                                    {
                                        cell.CellStyle = currencyCellStyle;
                                    }
                                    else if (String.Compare(name, ResourceUI.CommissionPercentage, true) == 0
                                      || String.Compare(name, ResourceUI.DiscountPercentage, true) == 0)
                                    {
                                        cell.CellStyle = percentCellStyle;
                                    }
                                }
                            }
                            else if (value is DateTime)
                            {
                                cell.SetCellValue((DateTime)value);
                                cell.CellStyle = dateCellStyle;
                            }
                            else if (
                                  value is sbyte
                                 || value is byte)
                            {
                                cell.SetCellValue((byte)value);
                            }
                            else if (value == null)
                            {
                                cell.SetCellValue((string)null);
                            }
                            else
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cellCnt++;

                            //csv.WriteField(column.Value);
                        }

                        //csv.NextRecord();
                    }
                }

                // comment out smartcookie csv file
                //this.Response.Model = sw.ToString();
                this.Response.Model = hssfworkbook;

                return this.Response;
            }

            this.Response.Model = export;

            return this.Response;
        }

        //======= New Project Export ==============
        public ServiceResponse GetProjectExportExcelModel(UserSessionModel admin, ProjectExportParameter param)
        {
            //param.PageSize = (int)DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL;
            param.ReturnTotals = false;
            //model.Page = 1;

            //default project export type to detailed, in case individual project is being exported
            param.ProjectExportType = param.ProjectExportType ?? ProjectExportTypeEnum.PipelineDetailed;

            var qry = (from project in this.Db.QueryProjectsExportViewableByParam(admin, param).Include("ProjectPipelineNotes").Include("ProjectPipelineNoteTypes")

                       join os in this.Context.ProjectOpenStatusTypes on project.ProjectOpenStatusTypeId equals os.ProjectOpenStatusTypeId
                       into Los
                       from os in Los.DefaultIfEmpty()

                       join ps in this.Context.ProjectStatusTypes on project.ProjectStatusTypeId equals ps.ProjectStatusTypeId
                       into Lps
                       from ps in Lps.DefaultIfEmpty()

                       join pt in this.Context.ProjectTypes on project.ProjectTypeId equals pt.ProjectTypeId
                       into Lpt
                       from pt in Lpt.DefaultIfEmpty()

                       join vt in this.Context.VerticalMarketTypes on project.VerticalMarketTypeId equals vt.VerticalMarketTypeId
                       into Lvt
                       from vt in Lvt.DefaultIfEmpty()

                       join plst in this.Context.ProjectLeadStatusTypes on project.ProjectLeadStatusTypeId equals plst.ProjectLeadStatusTypeId
                       into Lplst
                       from plst in Lplst.DefaultIfEmpty()

                       join quote in this.Context.Quotes on new { P = project.ProjectId, A = true } equals new { P = quote.ProjectId, A = quote.Active } into Lq
                       from quote in Lq.DefaultIfEmpty()

                           //join commission in this.Context.CommissionRequests on new { C = quote.QuoteId, A = (byte?)6 } equals new { C = commission.QuoteId, A = commission.CommissionRequestStatusTypeId } into com
                           //from commission in com.DefaultIfEmpty()

                           //join dar in this.Context.DiscountRequests on new { D = quote.QuoteId, A = (byte)6 } equals new { D = dar.QuoteId, A = dar.DiscountRequestStatusTypeId }  into ld
                           //from dar in ld.DefaultIfEmpty()

                       join commission in this.Context.CommissionRequests on new { C = quote.QuoteId } equals new { C = commission.QuoteId } into com
                       from commission in com.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                       join dar in this.Context.DiscountRequests on new { D = quote.QuoteId } equals new { D = dar.QuoteId } into ld
                       from dar in ld.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                       join transfer in this.Context.ProjectTransfers on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                       from transfer in Lt.DefaultIfEmpty()

                       join owner in this.Context.Users on project.OwnerId equals owner.UserId into Low
                       from owner in Low.DefaultIfEmpty()

                       join business in this.Context.Businesses on owner.BusinessId equals business.BusinessId
                       into Lb
                       from business in Lb.DefaultIfEmpty()

                       select new
                       {
                           project,
                           os,
                           ps,
                           pt,
                           vt,
                           plst,
                           quote,
                           quote.QuoteItems,
                           transfer,
                           owner,
                           business,
                           commission,
                           dar,
                           latestProjectPipelineNote = project.ProjectPipelineNotes
                                .OrderByDescending(o => o.Timestamp)
                                .Select(ss => new
                                {
                                    noteTypeName = ss.ProjectPipelineNoteType.Name,
                                    note = ss.Note,
                                    noteTimestamp = ss.Timestamp
                                })
                                .Take(1).FirstOrDefault()
                       });

            var results = qry.ToList();

            List<ProjectExportModel> export = new List<ProjectExportModel>();

            long[] productIds = results.SelectMany(r => r.QuoteItems).Select(i => i.ProductId.Value).Distinct().ToArray();


            Db.ProductByProductIds(productIds).Load();

            List<ProductModelType> productModelTypes = Db.Context.ProductModelTypes.ToList();
            List<ProductCategory> productCategories = Db.Context.ProductCategories.ToList();

            foreach (var o in results)
            {
                //work out Daikin Account-related stuff regardless
                string RSMFullName = "";
                string CSMFullName = "";

                if ((o.business != null && o.business.AccountManagerFirstName != null))
                {
                    CSMFullName = o.business.AccountManagerFirstName;

                    if (o.business.AccountManagerLastName != null)
                    {
                        CSMFullName += " " + o.business.AccountManagerLastName;
                    }
                }

                if ((o.business != null && o.business.AccountOwnerFirstName != null))
                {
                    RSMFullName = o.business.AccountOwnerFirstName;

                    if (o.business.AccountOwnerLastName != null)
                    {
                        RSMFullName += " " + o.business.AccountOwnerLastName;
                    }
                }

                //pipeline report - just one row per project
                //if (param.ProjectExportType != "ExportDetailed")
                if (param.ProjectExportType == ProjectExportTypeEnum.Pipeline) 
                {

                    #region Replaced with data saved in database (2016.01.05) - Delete after 2016.02.01
                    //work out quantities for a project - pipeline report only

                    //int vrvOutdoorUnitQty = 0;
                    //int rtuQty = 0;
                    //int splitOutdoorUnitQty = 0;
                    //int vrvIndoorUnitQty = 0;

                    //var calculator = new ProductComponentCalculator();

                    //if (o.QuoteItems != null && o.QuoteItems.Count > 0)
                    //{
                    //TODO
                    //i.      Count all units with VRV family and Model type as Outdoor on the quote [example: RXYQ120TTJU - unit count is 1]
                    //ii.      In case a units is large VRV outdoor unit (made of smaller outdoor units) on quote; count and add its individual components (unit module) to VRV-ODU# [Example: RXYQ360TTJU (composed of 3*RXYQ120TTJU) - unit count is 3]
                    //iii.      If above two units are listed on the quote then VRV-ODU# is 4
                    // vrvOutdoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //     && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV).Count();

                    //var vrvOutdoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //    && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                    //    .ToList();

                    ////vrvIndoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor
                    ////    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV).Count();

                    //var vrvIndoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //    && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                    //    .ToList();

                    ////TODO - In case a Model is System Type then count and add its individual Outdoor component to the mix.
                    ////splitOutdoorUnitQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    ////    && (p.Product.ProductFamilyId == (int)ProductFamilyEnum.MiniSplit ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.Altherma ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.SkyAir ||
                    ////    p.Product.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit)).Count();

                    //var splitOutdoorUnitList = o.QuoteItems.Where(p => p.Product != null
                    //          && (p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //            || p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                    //          && (p.Product.ProductFamilyId == (int)ProductFamilyEnum.MiniSplit ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.Altherma ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.SkyAir ||
                    //          p.Product.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit))
                    //          .ToList();

                    ////according to Deepak, RTU is actually the Packaged product family(??)
                    ////rtuQty = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    ////    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.Packaged).Count();

                    //var rtuList = o.QuoteItems.Where(p => p.Product != null && p.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                    //    && p.Product.ProductFamilyId == (int)ProductFamilyEnum.Packaged)
                    //    .ToList();

                    //vrvOutdoorUnitQty = calculator.CalculateProductsComponent(vrvOutdoorUnitList);
                    //vrvIndoorUnitQty = calculator.CalculateProductsComponent(vrvIndoorUnitList);
                    //splitOutdoorUnitQty = calculator.CalculateProductsComponent(splitOutdoorUnitList, new int[] { (int)ProductModelTypeEnum.Outdoor });
                    //rtuQty = calculator.CalculateProductsComponent(rtuList);

                    //}

                    #endregion Replaced with data saved in database

                    var exportModel = new ProjectExportModel
                    {
                        // Project
                        ProjectReference = o.project.ProjectId.ToString(),
                        CRMAccountId = (o.business != null && o.business.AccountId != null) ? o.business.AccountId : "",
                        Region = (o.business != null && o.business.AccountOwningGroupName != null) ? o.business.AccountOwningGroupName : "",
                        RSM = RSMFullName,
                        CSM = CSMFullName,
                        BusinessName = (o.business != null && o.business.BusinessName != null) ? o.business.BusinessName : "",
                        SellerName = (o.project.SellerName == null) ? "" : o.project.SellerName,
                        ProjectOwnerName = o.project.Owner.FirstName + " " + o.project.Owner.LastName,
                        CustomerBusinessName = (o.project.CustomerName == null) ? "" : o.project.CustomerName,
                        EngineerFirm = (o.project.EngineerName == null) ? "" : o.project.EngineerName,
                        ProjectName = o.project.Name,
                        ProjectDate = o.project.ProjectDate,
                        ProjectType = (o.pt == null) ? "" : o.pt.Description,
                        ProjectOpenStatus = (o.os == null) ? "" : o.os.Description,
                        ProjectStatus = (o.ps == null) ? "" : o.ps.Description,
                        VerticalMarketDescription = (o.vt == null) ? "" : o.vt.Description,
                        BidDate = o.project.BidDate,
                        EstimatedCloseDate = o.project.EstimatedClose,
                        EstimatedDeliveryDate = o.project.EstimatedDelivery,
                        EstimatedDeliveryMonth = o.project.EstimatedDelivery.ToString("MMMM"),
                        ProjectExpirationDate = o.project.Expiration,
                        Transferred = (o.transfer != null) ? "Yes" : "",
                        ProjectNotes = (o.project.Description != null) ? o.project.Description.ToString() : "",

                        // Quote
                        TotalList = (o.quote == null) ? 0 : o.quote.TotalList,
                        TotalNet = (o.quote == null) ? 0 : o.quote.TotalNet,
                        TotalSell = (o.quote == null) ? 0 : o.quote.TotalSell,
                        VRVOutdoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountVRVOutdoor,

                        // TODO:  Calculate RTU
                        RTUQty = 0,
                        SplitOutdoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountSplitOutdoor,
                        VRVIndoorUnitQty = (o.quote == null) ? 0 : (int)o.quote.TotalCountVRVIndoor,
                        QuoteReference = (o.quote == null) ? "" : o.quote.QuoteId.ToString(),
                        QuoteName = (o.quote == null) ? "" : o.quote.Title.ToString(),
                        QuoteNotes = (o.quote != null && o.quote.Notes != null) ? o.quote.Notes.ToString() : "",
                        Revision = (o.quote == null) ? 0 : o.quote.Revision,
                        IsGrossMargin = (o.quote == null) ? "" : o.quote.IsGrossMargin.ToString(),
                        IsCommissionScheme = (o.quote == null) ? "" : o.quote.IsCommissionScheme.ToString(),
                        IsCommissionRequest = (o.quote == null) ? false : o.quote.IsCommission,
                        TotalFreight = (o.quote == null) ? 0 : o.quote.TotalFreight,

                        //CommissionPercentage = (o.quote == null) ? 0 : o.quote.CommissionPercentage,// TODO: Database is incorrect
                        //DiscountPercentage = (o.quote == null) ? 0 : o.quote.DiscountPercentage,// TODO: Database is incorrect

                        CommissionPercentage = (o.commission == null) ? 0 : (decimal)o.commission.ApprovedCommissionPercent,
                        DiscountPercentage = (o.dar == null)? 0: (decimal)o.dar.ApprovedDiscount,
                        

                    };

                    if (admin != null && admin.HasAccess(SystemAccessEnum.ViewPipelineData))
                    {
                        exportModel.ProjectLeadStatus = (o.plst == null) ? ResourceUI.Lead : o.plst.Name;

                        // Pipeline Note
                        if (o.latestProjectPipelineNote != null)
                        {
                            exportModel.ProjectPipelineNote = o.latestProjectPipelineNote.note;
                            exportModel.ProjectPipelineNoteType = o.latestProjectPipelineNote.noteTypeName;
                            exportModel.ProjectPipelineNoteDate = o.latestProjectPipelineNote.noteTimestamp;
                        }
                    }

                    export.Add(exportModel);
                }
                //detailed report - show all quote items for each project
                //else if (param.ProjectExportType == "ExportDetailed")
                else if(param.ProjectExportType == ProjectExportTypeEnum.PipelineDetailed)
                {
                    long[] productsInActiveQuoteId = new long[] { };
                    Dictionary<long, ProductSpecificationsModel> allProductSpecsInQuote = new Dictionary<long, ProductSpecificationsModel>();

                    if (o.QuoteItems == null || o.QuoteItems.Count == 0)
                    {
                        o.QuoteItems.Add(new QuoteItem { }); // fake one record
                    }
                    else
                    {
                        //get all relevant product specs for all items in quote
                        productsInActiveQuoteId = o.QuoteItems.Select(qi => (long)qi.ProductId).ToArray();
                        allProductSpecsInQuote = Db.GetProductSpecifications(productsInActiveQuoteId, new string[] { "PowerVoltage", "UnitInstallationType" });
                    }

                    foreach (var quoteItem in o.QuoteItems)
                    {
                        string powerVoltage = "";
                        string unitInstallationType = "";

                        if (allProductSpecsInQuote.Count > 0)
                        {
                            //get model from specs list that contains all the specs for this product
                            ProductSpecificationsModel productSpecs = allProductSpecsInQuote.Where(ps => ps.Key == quoteItem.ProductId).FirstOrDefault().Value;

                            if (productSpecs != null && productSpecs.All != null && productSpecs.All.Count > 0)
                            {
                                //find the actual specs hidden inside the list of containers, and assign
                                List<ProductSpecificationModel> actualSpecs = productSpecs.All.Select(lps => lps.Value).ToList();

                                //power voltage value is straight-forward
                                powerVoltage = actualSpecs.Where(pos => pos.Name == "PowerVoltage").Select(pos => pos.Value).FirstOrDefault();

                                //unit installation type needs to be converted to 'HP (Heat Pump')' or 'HR (Heat recovery)'
                                unitInstallationType = actualSpecs.Where(pos => pos.Name == "UnitInstallationType").Select(pos => pos.Value).FirstOrDefault();
                            }
                        }

                        //var oduQuantity = new ProductComponentCalculator().CalculateProductsComponent(quoteItem);

                        export.Add(
                            new ProjectExportModel
                            {
                                // Project
                                ProjectReference = o.project.ProjectId.ToString(),
                                CRMAccountId = (o.business != null && o.business.AccountId != null) ? o.business.AccountId : "",
                                Region = (o.business != null && o.business.AccountOwningGroupName != null) ? o.business.AccountOwningGroupName : "",
                                RSM = RSMFullName,
                                CSM = CSMFullName,
                                BusinessName = (o.business != null && o.business.BusinessName != null) ? o.business.BusinessName : "",
                                SellerName = (o.project.SellerName == null) ? "" : o.project.SellerName,
                                ProjectOwnerName = o.project.Owner.FirstName + " " + o.project.Owner.LastName,
                                CustomerBusinessName = (o.project.CustomerName == null) ? "" : o.project.CustomerName,
                                EngineerFirm = (o.project.EngineerName == null) ? "" : o.project.EngineerName,
                                ProjectName = o.project.Name,
                                ProjectDate = o.project.ProjectDate,
                                ProjectType = (o.pt == null) ? "" : o.pt.Description,
                                ProjectOpenStatus = (o.os == null) ? "" : o.os.Description,
                                ProjectStatus = (o.ps == null) ? "" : o.ps.Description,
                                VerticalMarketDescription = (o.vt == null) ? "" : o.vt.Description,
                                BidDate = o.project.BidDate,
                                EstimatedCloseDate = o.project.EstimatedClose,
                                EstimatedDeliveryDate = o.project.EstimatedDelivery,
                                ProjectExpirationDate = o.project.Expiration,
                                Transferred = (o.transfer != null) ? "Yes" : "",
                                ProjectNotes = (o.project.Description != null) ? o.project.Description.ToString() : "",

                                // Quote
                                TotalList = (o.quote == null) ? 0 : o.quote.TotalList,
                                TotalNet = (o.quote == null) ? 0 : o.quote.TotalNet,
                                TotalSell = (o.quote == null) ? 0 : o.quote.TotalSell,
                                QuoteReference = (o.quote == null) ? "" : o.quote.QuoteId.ToString(),
                                QuoteName = (o.quote == null) ? "" : o.quote.Title.ToString(),
                                QuoteNotes = (o.quote != null && o.quote.Notes != null) ? o.quote.Notes.ToString() : "",
                                Revision = (o.quote == null) ? 0 : o.quote.Revision,
                                IsGrossMargin = (o.quote == null) ? "" : o.quote.IsGrossMargin.ToString(),
                                IsCommissionScheme = (o.quote == null) ? "" : o.quote.IsCommissionScheme.ToString(),
                                TotalFreight = (o.quote == null) ? 0 : o.quote.TotalFreight,
                                //CommissionPercentage = (o.quote == null) ? 0 : o.quote.CommissionPercentage,
                                //DiscountPercentage = (o.quote == null) ? 0 : o.quote.DiscountPercentage,

                                CommissionPercentage = (o.commission == null) ? 0 : (decimal)o.commission.ApprovedCommissionPercent,
                                DiscountPercentage = (o.dar == null) ? 0 : (decimal)o.dar.ApprovedDiscount,

                                //Commission Request
                                RequestedCommissionPercent = (o.commission == null || o.commission.RequestedCommissionPercent == null) ? 0 : o.commission.RequestedCommissionPercent.Value,
                                ApprovedCommissionPercent = (o.commission == null || o.commission.ApprovedCommissionPercent == null) ? 0 : o.commission.ApprovedCommissionPercent.Value,
                                RequestedMultiplier = (o.commission == null || o.commission.RequestedMultiplier == null) ? 0 : o.commission.RequestedMultiplier.Value,
                                ApprovedMultiplier = (o.commission == null || o.commission.ApprovedMultiplier == null) ? 0 : o.commission.ApprovedMultiplier.Value,

                                RequestedCommissionSplitPercent = (o.commission == null || o.commission.RequestedCommissionPercentSplit == null) ? 0 : o.commission.RequestedCommissionPercentSplit.Value,
                                ApprovedCommissionSplitPercent = (o.commission == null || o.commission.ApprovedCommissionPercentSplit == null) ? 0 : o.commission.ApprovedCommissionPercentSplit.Value,
                                RequestedCommissionMultiplierSplit = (o.commission == null || o.commission.RequestedMultiplierSplit == null) ? 0 : o.commission.RequestedMultiplierSplit.Value,
                                ApprovedCommissionMultiplierSplit = (o.commission == null || o.commission.ApprovedMultiplierSplit == null) ? 0 : o.commission.ApprovedMultiplierSplit.Value,

                                RequestedCommissionVRVPercent = (o.commission == null || o.commission.RequestedCommissionPercentVRV == null) ? 0 : o.commission.RequestedCommissionPercentVRV.Value,
                                ApprovedCommissionVRVPercent = (o.commission == null || o.commission.ApprovedCommissionPercentVRV == null) ? 0 : o.commission.ApprovedCommissionPercentVRV.Value,
                                RequestedCommissionMultiplierVRV = (o.commission == null || o.commission.RequestedMultiplierVRV == null) ? 0 : o.commission.RequestedMultiplierVRV.Value,
                                ApprovedCommissionMultiplierVRV = (o.commission == null || o.commission.ApprovedMultiplierVRV == null) ? 0 : o.commission.ApprovedMultiplierVRV.Value,

                                IsCommissionRequest = (o.quote == null) ? false : o.quote.IsCommission,

                                // Product
                                ProductNumber = (quoteItem == null) ? "" : quoteItem.ProductNumber,
                                ProductDescription = (quoteItem == null) ? "" : quoteItem.Description,
                                ProductModelType = (quoteItem == null || quoteItem.Product == null || quoteItem.Product.ProductModel == null || string.IsNullOrEmpty(quoteItem.Product.ProductModel.Description)) ? "" : quoteItem.Product.ProductModel.Description.ToString(),
                                //product model type == product category
                                ProductType = (quoteItem == null || quoteItem.Product == null || quoteItem.Product.ProductCategory == null || string.IsNullOrEmpty(quoteItem.Product.ProductCategory.Name)) ? "" : quoteItem.Product.ProductCategory.Name.ToString(),
                                //HP/HR == unit installation type
                                HpHr = unitInstallationType,
                                Voltage = powerVoltage,
                                Quantity = (quoteItem == null) ? 0 : quoteItem.Quantity,
                                PriceList = (quoteItem == null) ? 0 : quoteItem.ListPrice,
                                PriceNet = (quoteItem == null) ? 0 : (quoteItem.ListPrice * quoteItem.Multiplier),
                                ExtendedNetPrice = (quoteItem == null) ? 0 : ((quoteItem.ListPrice * quoteItem.Multiplier) * quoteItem.Quantity),
                                ProductClassCode = (quoteItem == null || quoteItem.Product == null) ? "" : quoteItem.Product.ProductClassCode
                            });
                    }
                }
            }

            if (this.Response.IsOK)
            {
                StringWriter sw = new StringWriter();
                int n = -1;

                var csv = new CsvWriter(sw);

                if (export.Count > 0)
                {
                    //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                    // Initialize NPOI
                    InitializeWorkbook();
                    ISheet sheet1 = hssfworkbook.CreateSheet("DPOProjectExport-" + param.ProjectExportType.ToString());
                    IDataFormat dataFormatCustom = hssfworkbook.CreateDataFormat();

                    var dateCellStyle = hssfworkbook.CreateCellStyle();
                    dateCellStyle.DataFormat = dataFormatCustom.GetFormat(ResourceUI.DateFormat);

                    var percentCellStyle = hssfworkbook.CreateCellStyle();
                    percentCellStyle.DataFormat = dataFormatCustom.GetFormat("0.00%");

                    var currencyCellStyle = hssfworkbook.CreateCellStyle();
                    currencyCellStyle.DataFormat = dataFormatCustom.GetFormat("$#,##0.00_);-$#,##0.00_)");

                    for (var k = 0; k < export.Count; k++)
                    {
                        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                        //Create the NPOI row object here and initialize it to the 1st row which is zero based.
                        n++;
                        IRow row = sheet1.CreateRow(n);

                        Dictionary<string, object> columns = this.ProjectExportModelToCsv(admin, export[k], param.ProjectExportType);

                        int cellCnt = 0;
                        if (k == 0)
                        {
                            foreach (var header in columns)
                            {
                                //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                                //NPOI Cell Headers get set here.
                                row.CreateCell(cellCnt).SetCellValue(header.Key);
                                cellCnt++;

                                //csv.WriteField(header.Key);
                            }

                            //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                            //Initialize the next row here because the 1st row (header row) is row 0 and then it has to set the values for the next row in the foreach section below.
                            n = k + 1;
                            row = sheet1.CreateRow(n);

                            //csv.NextRecord();
                        }

                        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                        cellCnt = 0; //Initialize back to 0.
                        foreach (var column in columns)
                        {
                            var value = column.Value;
                            var name = column.Key;

                            var cell = row.CreateCell(cellCnt);

                            if (!admin.HasAccess(SystemAccessEnum.ViewPipelineData))
                            {

                            }

                            //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
                            // Fix to only convert the reference numbers to strings
                            if (String.Compare(name, ResourceUI.ProjectReference, true) == 0
                                || String.Compare(name, ResourceUI.QuoteReference, true) == 0
                                || String.Compare(name, ResourceUI.ProjectRef, true) == 0)
                            {
                                cell.SetCellValue(column.Value.ToString());
                            }
                            else if (
                                 value is short
                                 || value is ushort
                                 || value is int
                                 || value is uint
                                 || value is long
                                 || value is ulong
                                 || value is float
                                 || value is double
                                 || value is decimal)
                            {
                                double dVal;

                                if (double.TryParse(value.ToString(), out dVal))
                                {
                                    cell.SetCellValue(dVal);

                                    // HACK:  Crazy hack
                                    if (String.Compare(name, ResourceUI.TotalNet, true) == 0
                                        || String.Compare(name, ResourceUI.TotalList, true) == 0
                                        || String.Compare(name, ResourceUI.TotalSell, true) == 0
                                        || String.Compare(name, ResourceUI.TotalFreight, true) == 0
                                        || String.Compare(name, ResourceUI.TotalPrice, true) == 0
                                        || String.Compare(name, ResourceUI.PriceList, true) == 0
                                        || String.Compare(name, ResourceUI.PriceNetEach, true) == 0
                                        || String.Compare(name, ResourceUI.ExtendedPrice, true) == 0)
                                    {
                                        cell.CellStyle = currencyCellStyle;
                                    }
                                    else if (String.Compare(name, ResourceUI.CommissionPercentage, true) == 0
                                      || String.Compare(name, ResourceUI.DiscountPercentage, true) == 0)
                                    {
                                        cell.CellStyle = percentCellStyle;
                                    }
                                }
                            }
                            else if (value is DateTime)
                            {
                                cell.SetCellValue((DateTime)value);
                                cell.CellStyle = dateCellStyle;
                            }
                            else if (
                                  value is sbyte
                                 || value is byte)
                            {
                                cell.SetCellValue((byte)value);
                            }
                            else if (value == null)
                            {
                                cell.SetCellValue((string)null);
                            }
                            else
                            {
                                cell.SetCellValue(value.ToString());
                            }
                            cellCnt++;

                            //csv.WriteField(column.Value);
                        }

                        //csv.NextRecord();
                    }
                }

                // comment out smartcookie csv file
                //this.Response.Model = sw.ToString();
                this.Response.Model = hssfworkbook;

                return this.Response;
            }

            this.Response.Model = export;

            return this.Response;
        }

        //======= End of New Project Export ==============
        //TODO: need to discuss about Project Transfer logic. 
        //Currently, if A transfer project to B, then B transfer it back to A. A can't nor request DAR/COM
        //check IsProjectTransferred()
        

        public ServiceResponse GetProjectModel(UserSessionModel admin, long? projectId)
        {
            var addressService = new AddressServices(this.Context);

            ProjectModel model = null;

            if (projectId.HasValue)
            {
                var query = from project in this.Db.QueryProjectViewableByProjectId(admin, projectId)
                            join quote in this.Db.Quotes on new { id = project.ProjectId, active = true } 
                            equals new { id = quote.ProjectId, active = quote.Active } into Laq
                            from quote in Laq.DefaultIfEmpty()

                            join transfer in this.Context.ProjectTransfers on new { admin.UserId, project.ProjectId } 
                            equals new { transfer.UserId, transfer.ProjectId } into Lt
                            from transfer in Lt.DefaultIfEmpty()

                            join commission in this.Context.CommissionRequests on new { project.ProjectId } 
                            equals new { commission.ProjectId } into com
                            from commission in com.DefaultIfEmpty()

                            select new ProjectModel
                            {
                                ProjectId = project.ProjectId,
                                OwnerId = project.Owner.UserId,
                                OwnerName = project.Owner.FirstName + " " + project.Owner.LastName,
                                Name = project.Name,
                                Description = project.Description,
                                CustomerName = project.CustomerName,
                                EngineerName = project.EngineerName,
                                EngineerBusinessName = project.EngineerBusinessName,
                                DealerContractorName = project.DealerContractorName,
                                ShipToName = project.ShipToName,
                                SellerName = project.SellerName,
                                ProjectDate = project.ProjectDate,
                                BidDate = project.BidDate,
                                EstimatedClose = project.EstimatedClose,
                                EstimatedDelivery = project.EstimatedDelivery,
                                Expiration = project.Expiration,
                                ProjectLeadStatusTypeId = project.ProjectLeadStatusTypeId,
                                ProjectLeadStatusTypeDescription = project.ProjectLeadStatusType.Description,
                                ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                ProjectTypeId = project.ProjectTypeId,
                                ConstructionTypeId = project.ConstructionTypeId,
                                VerticalMarketTypeId = project.VerticalMarketTypeId,
                                ERPFirstOrderComment = project.ERPFirstOrderComment,
                                ERPFirstOrderDate = project.ERPFirstOrderDate,
                                ERPFirstOrderNumber = project.ERPFirstOrderNumber,
                                ERPFirstPONumber = project.ERPFirstPONumber,

                                ActualCloseDate = (project.ActualCloseDate != null) ? project.ActualCloseDate.Value : DateTime.Now,
                                EstimateReleaseDate = (project.EstimateReleaseDate != null) ? project.EstimateReleaseDate.Value : DateTime.Now,

                                SquareFootage = (project.SquareFootage != null) ? project.SquareFootage.Value : 0,
                                NumberOfFloors = (project.NumberOfFloors != null) ? project.NumberOfFloors.Value : 0,

                                OrderStatus = !string.IsNullOrEmpty(quote.Orders.FirstOrDefault().OrderStatusTypeId.ToString()) ? 
                                             (byte)quote.Orders.FirstOrDefault().OrderStatusTypeId : (byte)OrderStatusTypeEnum.NewRecord,

                                IsTransferred = (transfer != null),

                                CustomerAddress = new AddressModel
                                {
                                    AddressId = project.CustomerAddressId,
                                },
                                SellerAddress = new AddressModel
                                {
                                    AddressId = project.SellerAddressId,
                                },
                                EngineerAddress = new AddressModel
                                {
                                    AddressId = project.EngineerAddressId,
                                },
                                ShipToAddress = new AddressModel
                                {
                                    AddressId = project.ShipToAddressId,
                                },
                                ActiveQuoteSummary = new QuoteListModel
                                {
                                    ProjectId = project.ProjectId,
                                    QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                    Alert = (quote == null) ? false : quote.RecalculationRequired,
                                    Title = (quote == null) ? "" : quote.Title,
                                    Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                    TotalList = (quote == null) ? 0 : quote.TotalList,
                                    TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                    TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                    TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                    Revision = (quote == null) ? 0 : quote.Revision,
                                    CommissionAmount = commission.ApprovedCommissionTotal ?? commission.RequestedCommissionTotal ?? 0,
                                    NetMultiplierValue = commission.RequestedNetMaterialValue ?? 0,
                                    TotalNetCommission = (commission.TotalNet) ?? 0,
                
                                    HasDAR = (quote == null) ? false : (quote.DiscountRequestId != null || quote.AwaitingDiscountRequest), // common these two lines if it break the apps
                                    AwaitingDiscountRequest = (quote == null) ? false : quote.AwaitingDiscountRequest,

                                    HasCOM = (quote == null) ? false : (quote.AwaitingCommissionRequest),

                                    AwaitingCommissionRequest = (quote == null) ? false : quote.AwaitingCommissionRequest,

                                    IsCommission = (quote == null) ? false : string.IsNullOrEmpty(quote.IsCommission.ToString()) ?
                                                   false : quote.IsCommission,

                                    CommissionRequestStatusTypeId = (commission.CommissionRequestStatusTypeId == null) ? 
                                                                    (byte)0 : commission.CommissionRequestStatusTypeId.Value,
                                    OrderStatusTypeId = !string.IsNullOrEmpty(quote.Orders.FirstOrDefault().OrderStatusTypeId.ToString()) ? 
                                                        (byte)quote.Orders.FirstOrDefault().OrderStatusTypeId : (byte)0
                                },
                                ConstructionTypeDescription = project.ConstructionType.Description,
                                ProjectTypeDescription = project.ProjectType.Description,
                                ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                ProjectStatusDescription = project.ProjectStatusType.Description,
                                VerticalMarketDescription = project.VerticalMarketType.Description,
                                Deleted = project.Deleted,
                                Timestamp = project.Timestamp,
                                ProjectStatusNotes = project.ProjectStatusNotes,
                                IsCommission = (quote == null) ? null : (bool?)quote.IsCommission, 
                            };
                //if somehow the query return more than one records
                //then get the latest record ( the last record )
                if (query.Count() > 1)
                {
                    model = query.ToList().LastOrDefault();
                }
                else
                {
                    model = query.FirstOrDefault();
                }
            }
            // Create if not found or new
            if (model == null)
            {
                model = new ProjectModel();
                model.ProjectDate = DateTime.Now;

                model.ActiveQuoteSummary = new QuoteListModel();

                //#######################################################################
                //Copy user  business address to project seller address for new projects
                //#######################################################################
                model.SellerAddress = addressService.GetAddressModel(admin, model.SellerAddress);

                var businessData = Db.BusinessQueryByBusinessId(admin, admin.BusinessId).Select(u => new { u.AddressId, u.BusinessName }).FirstOrDefault();

                model.SellerAddress.Copy(new AddressServices(this.Context).GetAddressModel(admin, new AddressModel { AddressId = businessData.AddressId }));

                model.SellerName = businessData.BusinessName;

                this.DropDownMode = DropDownMode.NewRecord;
            }
            else
            {
                model.SellerAddress = addressService.GetAddressModel(admin, model.SellerAddress);

                this.DropDownMode = DropDownMode.EditRecord;
            }

            model.CustomerAddress = addressService.GetAddressModel(admin, model.CustomerAddress);

            model.EngineerAddress = addressService.GetAddressModel(admin, model.EngineerAddress);

            model.ShipToAddress = addressService.GetAddressModel(admin, model.ShipToAddress);

            model.ProjectId = projectId;

            if (projectId.HasValue && model.ProjectId != projectId)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
            }

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetProjectPipelineNoteListModel(UserSessionModel userSessionModel, long projectId)
        {
            var query = from pn in this.Context.ProjectPipelineNotes
                        join pnt in this.Context.ProjectPipelineNoteTypes on pn.ProjectPipelineNoteTypeId equals pnt.ProjectPipelineNoteTypeId
                        join u in this.Context.Users on pn.OwnerId equals u.UserId into pnu
                        from u in pnu.DefaultIfEmpty()
                        where pn.ProjectId == projectId
                        select new ProjectPipelineNoteModel
                        {
                            Note = pn.Note,
                            ProjectId = pn.ProjectId,
                            ProjectPipelineNoteId = pn.ProjectPipelineNoteId,
                            ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel
                            {
                                Description = pnt.Description,
                                Name = pnt.Name,
                                ProjectPipelineNoteTypeId = pnt.ProjectPipelineNoteTypeId
                            },
                            OwnerId = pn.OwnerId,
                            OwnerName = u != null ? u.FirstName + " " + u.LastName : string.Empty,
                            Timestamp = pn.Timestamp
                        };


            var model = new ProjectPipelineNoteListModel
            {
                Items = new PagedList<ProjectPipelineNoteModel>(query.ToList())
            };

            model.PageSize = DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL;

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetProjectQuotesModel(UserSessionModel user, ProjectQuotesModel model)
        {
            // Save for later use
            var search = new SearchQuote(model as Search);

            if (model.ProjectId.HasValue)
            {
                var query = from project in this.Db.QueryProjectViewableByProjectId(user, model.ProjectId)

                            join quote in this.Db.Quotes 
                            on new { id = project.ProjectId, active = true } 
                            equals new { id = quote.ProjectId, active = quote.Active } 
                            into Laq
                            from quote in Laq.DefaultIfEmpty()
                      
                            join transfer in this.Context.ProjectTransfers 
                            on new { user.UserId, project.ProjectId } 
                            equals new { transfer.UserId, transfer.ProjectId } into Lt
                            from transfer in Lt.DefaultIfEmpty()

                            select new ProjectQuotesModel
                            {
                                ProjectId = project.ProjectId,
                                ProjectName = project.Name,
                                ProjectStatusTypeId = (byte?)project.ProjectStatusTypeId,
                                Deleted = project.Deleted,

                                IsTransferred = (transfer != null),

                                AwaitingDiscountRequest = (quote != null) ? quote.AwaitingDiscountRequest : false,
                                DiscountRequestId = quote.DiscountRequestId,

                                AwaitingCommissionRequest = (quote != null) ? quote.AwaitingCommissionRequest : false,

                                ActiveQuoteSummary = new QuoteListModel
                                {
                                    ProjectId = project.ProjectId,
                                    QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                    Alert = (quote == null) ? false:  quote.RecalculationRequired,
                                    Title = (quote == null) ? "" : quote.Title,
                                    Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                    TotalList = (quote == null) ? 0 : quote.TotalList,
                                    TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                    TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                    TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                    Revision = (quote == null) ? 0 : quote.Revision,
                                    HasDAR = (quote == null) ? false : (quote.DiscountRequestId != null || quote.AwaitingDiscountRequest),
                                    HasCOM = (quote == null) ? false : (quote.AwaitingCommissionRequest),
                                    OrderStatusTypeId = (quote != null && quote.Orders.FirstOrDefault() != null) ? (byte)quote.Orders.FirstOrDefault().OrderStatusTypeId : (byte)0
                                },
                            };

                model = query.FirstOrDefault();

                if (model == null)
                {
                    this.Response.AddError(Resources.ResourceModelProject.MP008);
                }
            }

            // Create if not found or new
            if (model == null)
            {
                model = new ProjectQuotesModel();
                model.ProjectId = model.ProjectId;
            }

            // Create if not found or new
            if (model.ActiveQuoteSummary == null)
            {
                model.ActiveQuoteSummary = new QuoteListModel();
            }

            // get quotes items now

            search.ProjectId = model.ProjectId;

            search.ReturnTotals = true;

            if (!search.ProjectId.HasValue) search.ProjectId = 0; // prevent the full list returning

            var response = new QuoteServices().GetQuoteListModel(user, search);

            if (response.IsOK)
            {
                var items = response.Model as List<QuoteListModel>;

                model.Items = new PagedList<QuoteListModel>(items, model);

                // Page number might change to copy it
                model.Page = search.Page;

                // Page number might change to copy it
                model.PageSize = search.PageSize;

                // Page totals need to be copied
                model.TotalRecords = search.TotalRecords;
            }

            this.Response.Model = model;

            return this.Response;
        }

        [Obsolete("Now using GetAllProjects")]
        public ServiceResponse GetProjectsModel(UserSessionModel admin, ProjectsModel model)
        {
            model.ReturnTotals = true;

            var expireDate = DateTime.Today.AddDays(model.ExpirationDays ?? 0);
            var savePagesize = model.PageSize; // switch off paging

            model.PageSize = DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL;

            var query = from project in this.Db.QueryProjectsViewableBySearch(admin, model, true)
                        where project.ProjectId != null
                        join quote in this.Context.Quotes
                            on new { P = project.ProjectId, A = true }
                            equals new { P = quote.ProjectId, A = quote.Active }
                            into Lq
                        from quote in Lq.DefaultIfEmpty()

                        join leadStatus in this.Context.ProjectLeadStatusTypes
                            on project.ProjectLeadStatusTypeId equals leadStatus.ProjectLeadStatusTypeId
                        join status in this.Context.ProjectOpenStatusTypes
                            on project.ProjectOpenStatusTypeId equals status.ProjectOpenStatusTypeId
                        join bid in this.Context.ProjectStatusTypes
                            on project.ProjectStatusTypeId equals bid.ProjectStatusTypeId

                        join owner in this.Context.Users
                            on project.OwnerId equals owner.UserId

                        join transfer in this.Context.ProjectTransfers
                            on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                        from transfer in Lt.DefaultIfEmpty()

                       
                        join discountRequest in this.Context.DiscountRequests
                            on new { project.ProjectId, quote.QuoteId } equals new { discountRequest.ProjectId, discountRequest.QuoteId } into Dr
                        from discountRequest in Dr.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                        join commissionRequest in this.Context.CommissionRequests
                           on new { project.ProjectId, quote.QuoteId } equals new { commissionRequest.ProjectId, commissionRequest.QuoteId } into Cr
                        from commissionRequest in Cr.OrderByDescending(c => c.Timestamp).Take(1).DefaultIfEmpty()

                        join order in this.Context.Orders
                        on new { project.ProjectId, quote.QuoteId } equals new { order.Quote.ProjectId, order.QuoteId } into or
                        from order in or.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                        select new ProjectListModel
                        {
                            ProjectId = project.ProjectId,
                            Name = project.Name,
                            Alert = (quote == null) ? false : (quote.RecalculationRequired || project.Expiration < expireDate),
                            BidDate = project.BidDate,
                            EstimatedDelivery = project.EstimatedDelivery,
                            IsTransferred = (transfer != null),
                            EstimatedClose = project.EstimatedClose,
                            ProjectOwner = owner.FirstName + " " + owner.LastName,
                            BusinessName = owner.Business.BusinessName,
                            CustomerName = project.CustomerName,
                            EngineerName = project.EngineerName,
                            ShipToName = project.ShipToName,
                            ProjectLeadStatus = leadStatus.Description,
                            ProjectLeadStatusId = leadStatus.ProjectLeadStatusTypeId,
                            ProjectOpenStatus = status.Description,
                            ProjectOpenStatusId = status.ProjectOpenStatusTypeId,
                            ProjectStatus = bid.Description,
                            ProjectStatusId = (int)bid.ProjectStatusTypeId,
                            ProjectType = project.ProjectType.Description,
                            ProjectTypeId = project.ProjectTypeId,
                            ProjectDate = project.ProjectDate,
                            Expiration = project.Expiration,
                            ERPFirstOrderComment = project.ERPFirstOrderComment,
                            ERPFirstOrderNumber = project.ERPFirstOrderNumber,
                            ERPFirstPONumber = project.ERPFirstPONumber,
                            ERPFirstOrderDate = project.ERPFirstOrderDate,

                            ActiveQuoteSummary = new QuoteListModel
                            {
                                QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                RecalculationRequired = (quote == null) ? false : quote.RecalculationRequired,
                                Title = (quote == null) ? "No Active Quote" : quote.Title,
                                TotalList = (quote == null) ? 0 : quote.TotalList,
                                TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                Alert = (quote == null) ? false : quote.RecalculationRequired,
                                VRVOutdoorCount = (quote == null) ? 0 : quote.VRVOutdoorCount,
                                SplitCount = (quote == null) ? 0 : quote.SplitCount
                            },
                            ActiveDiscountRequestSummary = new DiscountRequestModel()
                            {
                                DiscountRequestId = (discountRequest != null) ? discountRequest.DiscountRequestId : (long?)null,
                                DiscountRequestStatusTypeDescription = (discountRequest != null) ? discountRequest.DiscountRequestStatusType.Description : "",
                                DiscountRequestStatusTypeId = (discountRequest != null) ? discountRequest.DiscountRequestStatusType.DiscountRequestStatusTypeId : (byte?)null
                            },

                            ActiveCommissionRequestSummary = new CommissionRequestModel()
                            {
                                CommissionRequestId = (commissionRequest != null) ? commissionRequest.CommissionRequestId : (long?)null,
                                CommissionRequestStatusTypeDescription = (commissionRequest != null) ? commissionRequest.CommissionRequestStatusType.Description : "",
                                CommissionRequestStatusTypeId = (commissionRequest != null) ? commissionRequest.CommissionRequestStatusType.CommissionRequestStatusTypeId : (byte?)null
                            },

                            Deleted = project.Deleted,
                            Timestamp = project.Timestamp
                        };

            string sortcolumn = (model.SortColumn + "").ToLower();

            bool desc = model.IsDesc;

            switch (sortcolumn)
            {
                case "biddate":
                    query = (desc) ? query.OrderByDescending(s => s.BidDate) : query.OrderBy(s => s.BidDate);
                    break;

                case "projectowner":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectOwner) : query.OrderBy(s => s.ProjectOwner);
                    break;

                case "businessname":
                    query = (desc) ? query.OrderByDescending(s => s.BusinessName) : query.OrderBy(s => s.BusinessName);
                    break;

                case "customername":
                    query = (desc) ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName);
                    break;

                case "estimatedclose":
                    query = (desc) ? query.OrderByDescending(s => s.EstimatedClose) : query.OrderBy(s => s.EstimatedClose);
                    break;

                case "estimateddelivery":
                    query = (desc) ? query.OrderByDescending(s => s.EstimatedDelivery) : query.OrderBy(s => s.EstimatedDelivery);
                    break;

                case "projectdate":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectDate) : query.OrderBy(s => s.ProjectDate);
                    break;

                case "projectid":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectId) : query.OrderBy(s => s.ProjectId);
                    break;

                case "totalnet":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteSummary.TotalNet) : query.OrderBy(s => s.ActiveQuoteSummary.TotalNet);
                    break;

                case "totallist":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteSummary.TotalList) : query.OrderBy(s => s.ActiveQuoteSummary.TotalList);
                    break;

                case "totalsell":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteSummary.TotalSell) : query.OrderBy(s => s.ActiveQuoteSummary.TotalSell);
                    break;

                case "projectopenstatus":
                    query = (desc) ? query.OrderByDescending(s => s.ProjectOpenStatus) : query.OrderBy(s => s.ProjectOpenStatus);
                    break;
                case "activedarlink":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveDiscountRequestSummary.DiscountRequestStatusTypeDescription) : query.OrderBy(s => s.ActiveDiscountRequestSummary.DiscountRequestStatusTypeDescription);
                    break;
                case "vrvoutdoorcount":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteSummary.VRVOutdoorCount) : query.OrderBy(s => s.ActiveQuoteSummary.VRVOutdoorCount);
                    break;
                case "splitcount":
                    query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteSummary.SplitCount) : query.OrderBy(s => s.ActiveQuoteSummary.SplitCount);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                    break;
            }

            if (model.ReturnTotals)
            {
                model.TotalRecords = query.Count();
            }

            model.PageSize = savePagesize;

            model.TotalList = query.Sum(s => s.ActiveQuoteSummary.TotalList) ?? 0;
            model.TotalNet = query.Sum(s => s.ActiveQuoteSummary.TotalNet) ?? 0;
            model.TotalSell = query.Sum(s => s.ActiveQuoteSummary.TotalSell) ?? 0;
            model.TotalVRVOutdoorCount = query.Sum(s => s.ActiveQuoteSummary.VRVOutdoorCount) ?? 0;
            model.TotalSplitCount = query.Sum(s => s.ActiveQuoteSummary.SplitCount) ?? 0;

            query = Db.Paging(admin, query, model);


            model.Items = new PagedList<ProjectListModel>(query.ToList(), model);


            FinaliseModel(Response.Messages, admin, model);

            model.Items.ForEach(p =>
            {
                if (p.Alert && !p.IsTransferred)
                {
                    if (p.ActiveQuoteSummary.RecalculationRequired)
                    {
                        p.AlertText = "Pricing update, recalculation required.";
                        return;
                    }

                    var days = new TimeSpan(p.Expiration.Ticks - DateTime.Today.Ticks).TotalDays;
                    if (days == 0)
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpiresToday, p.Name);
                    }
                    else if (days < 0)
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpired, p.Name);
                    }
                    else
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpire, p.Name, days);
                    }
                }

            });

            model.ProjectDateTypes = htmlService.DropDownDateTypes(GetProjectDateTypes(), model.DateTypeId);

            this.Response.Model = model;

            return this.Response;
        }

        //Get data for kendo grid 
        public ServiceResponse GetAllProjects(UserSessionModel admin, ProjectsGridViewModel model, ProjectsGridQueryInfo queryInfo)
        {
            model.ReturnTotals = true;

            var expireDate = DateTime.Today.AddDays(model.ExpirationDays ?? 0);
            
            var savePagesize = model.PageSize; // switch off paging

            model.PageSize = DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL;

            if (!string.IsNullOrEmpty(queryInfo.ShowDeletedProjects.ToString()))
            {
                model.ShowDeletedProjects = queryInfo.ShowDeletedProjects;
            }
            //quert has changed to return all projects
            var projects = (from project in this.Db.QueryProjectsViewableBySearch(admin, model, true)
                            select project
                           ).Distinct().AsQueryable();

           
            var query = from project in projects
                        join quote in this.Context.Quotes
                           on new { P = project.ProjectId, A = true }
                           equals new { P = quote.ProjectId, A = quote.Active }
                           into Lq
                        from quote in Lq.DefaultIfEmpty()

                        join leadStatus in this.Context.ProjectLeadStatusTypes
                            on project.ProjectLeadStatusTypeId equals leadStatus.ProjectLeadStatusTypeId
                        join status in this.Context.ProjectOpenStatusTypes
                            on project.ProjectOpenStatusTypeId equals status.ProjectOpenStatusTypeId
                        join bid in this.Context.ProjectStatusTypes
                            on project.ProjectStatusTypeId equals bid.ProjectStatusTypeId

                        join owner in this.Context.Users
                        on project.OwnerId equals owner.UserId

                        join transfer in this.Context.ProjectTransfers
                            on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                        from transfer in Lt.DefaultIfEmpty()

                        join discountRequest in this.Context.DiscountRequests
                            on new { project.ProjectId, quote.QuoteId } equals new { discountRequest.ProjectId, discountRequest.QuoteId } into Dr
                        from discountRequest in Dr.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                        join commissionRequest in this.Context.CommissionRequests
                           on new { project.ProjectId, quote.QuoteId } equals new { commissionRequest.ProjectId, commissionRequest.QuoteId } into Cr
                        from commissionRequest in Cr.OrderByDescending(c => c.Timestamp).Take(1).DefaultIfEmpty()

                        join order in this.Context.Orders
                        on new { project.ProjectId, quote.QuoteId } equals new { order.Quote.ProjectId, order.QuoteId } into or
                        from order in or.OrderByDescending(o => o.Timestamp).Take(1).DefaultIfEmpty()

                        select new ProjectViewModel
                        {
                            ProjectId = project.ProjectId,
                            Name = project.Name,
                            Alert = (quote == null) ? false : (quote.RecalculationRequired || project.Expiration < expireDate),
                            BidDate = project.BidDate,
                            EstimatedDelivery = project.EstimatedDelivery,
                            IsTransferred = (transfer != null),
                            EstimatedClose = project.EstimatedClose,
                            ProjectOwner = owner.FirstName + " " + owner.LastName,
                            BusinessName = owner.Business.BusinessName,
                            CustomerName = project.CustomerName,
                            EngineerName = project.EngineerName,
                            ShipToName = project.ShipToName,
                            ProjectLeadStatus = leadStatus.Description,
                            ProjectLeadStatusId = leadStatus.ProjectLeadStatusTypeId,
                            ProjectOpenStatus = status.Description,
                            ProjectOpenStatusId = status.ProjectOpenStatusTypeId,
                            ProjectStatus = bid.Description,
                            ProjectStatusId = (int)bid.ProjectStatusTypeId,
                            ProjectType = project.ProjectType.Description,
                            ProjectTypeId = project.ProjectTypeId,
                            ProjectDate = project.ProjectDate,
                            Expiration = project.Expiration,
                            ERPFirstOrderComment = project.ERPFirstOrderComment,
                            ERPFirstOrderNumber = project.ERPFirstOrderNumber,
                            ERPFirstPONumber = project.ERPFirstPONumber,
                            ERPFirstOrderDate = project.ERPFirstOrderDate,

                            //=== Active Quote section ====
                            ActiveQuoteId = (quote == null) ? 0 : quote.QuoteId,
                            ActiveQuoteTitle = (quote == null) ? "No Active Quote" : quote.Title,
                            IsCommission = (quote == null) ? null : (bool?)quote.IsCommission,
                            RecalculationRequired = (quote == null) ? false : quote.RecalculationRequired,
                            TotalList = (quote == null) ? 0 : quote.TotalList,
                            TotalNet = (quote == null) ? 0 : quote.TotalNet,
                            TotalSell = (quote == null) ? 0 : quote.TotalSell,
                            TotalCountVRVOutDoor = (quote == null) ? 0 : quote.VRVOutdoorCount,
                            TotalCountSplitOutDoor = (quote == null) ? 0 : quote.TotalCountSplitOutdoor,

                            DiscountRequestId = quote.DiscountRequestId,
                            CommissionRequestId = quote.CommissionRequestId,
                            DarComStatus = quote.IsCommission ?
                                                (commissionRequest != null ? commissionRequest.CommissionRequestStatusType.Description : "") :
                                                (discountRequest != null ? discountRequest.DiscountRequestStatusType.Description : ""),

                            Deleted = project.Deleted,
                            Timestamp = project.Timestamp
                        };


            query = Filter(admin, query, queryInfo);

            query = Sort(query, queryInfo);
            
            //string sortcolumn = (model.SortColumn + "").ToLower();
            //bool desc = model.IsDesc;


            if (model.ReturnTotals)
            {
                //Slow, 5 sec 
                model.TotalRecords = query.Count();
            }

            model.PageSize = savePagesize;

            //model.TotalList = query.Sum(s => s.TotalList) ?? 0;
            //model.TotalNet = query.Sum(s => s.TotalNet) ?? 0;
            //model.TotalSell = query.Sum(s => s.TotalSell) ?? 0;
            //model.TotalVRVOutdoorCount = query.Sum(s => s.TotalCountVRVOutDoor) ?? 0;
            //model.TotalSplitCount = query.Sum(s => s.TotalCountSplitOutDoor) ?? 0;

            var totals = query.GroupBy(g => 1)
               .Select(g => new
               {
                   TotalList = g.Sum(s => s.TotalList) ?? 0,
                   TotalNet = g.Sum(s => s.TotalNet) ?? 0,
                   TotalSell = g.Sum(s => s.TotalSell) ?? 0,
                   TotalCountVRVOutDoor = g.Sum(s => s.TotalCountVRVOutDoor) ?? 0,
                   TotalCountSplitOutDoor = g.Sum(s => s.TotalCountSplitOutDoor) ?? 0,

               }).FirstOrDefault();

            model.TotalList = totals != null ? totals.TotalList : 0;
            model.TotalNet = totals != null ? totals.TotalNet : 0;
            model.TotalSell = totals != null ? totals.TotalSell : 0;
            model.TotalVRVOutdoorCount = totals != null ? totals.TotalCountVRVOutDoor : 0;
            model.TotalSplitCount = totals != null ? totals.TotalCountSplitOutDoor : 0;

            //query = Db.Paging(admin, query, model);

            //***uncomment after testing******
            query = query.Skip(queryInfo.Skip).Take(queryInfo.Take);
            //********************************


            model.Items = new PagedList<ProjectViewModel>(query.ToList(), model);

            FinaliseModel(Response.Messages, admin, model);

            model.Items.ForEach(p =>
            {
                if (p.Alert && !p.IsTransferred)
                {
                    if (p.RecalculationRequired)
                    {
                        p.AlertText = "Pricing update, recalculation required.";
                        return;
                    }

                    var days = new TimeSpan(p.Expiration.Ticks - DateTime.Today.Ticks).TotalDays;
                    if (days == 0)
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpiresToday, p.Name);
                    }
                    else if (days < 0)
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpired, p.Name);
                    }
                    else
                    {
                        p.AlertText = string.Format(ResourceUI.OverviewProjectExpire, p.Name, days);
                    }
                }

            });


          

            model.ProjectDateTypes = htmlService.DropDownDateTypes(GetProjectDateTypes(), model.DateTypeId);

            this.Response.Model = model;

            return this.Response;
        }

        private IQueryable<ProjectViewModel> Filter(UserSessionModel currentUser, IQueryable<ProjectViewModel> query, QueryInfo queryInfo)
        {
            if (queryInfo.Filter != null)
            {
                var rootFilters = queryInfo.Filter.Filters;
                var rootLogic = queryInfo.Filter.Logic;
                var rootFilterName = queryInfo.Filter.Name;


                if (rootFilters != null && rootLogic == "and")
                {
                    query = ProcessFilterItemsAnd(query, rootFilters);
                }
                else if (rootFilters != null && rootLogic == "or" && rootFilterName != "search")// A or B --- (PairFilter)
                {
                    query = ProcessPairFilterOr(query, rootFilters);
                }
                else if (rootFilters != null && rootLogic == "or" && rootFilterName == "search")// A or B --- (PairFilter)
                {
                    query = ProcessSearchProjectFilter(query, rootFilters);// Project Search Box Filter
                }

            }

            if (!currentUser.HasAccess(SystemAccessEnum.RequestCommission) && !currentUser.HasAccess(SystemAccessEnum.ViewRequestedCommission) && !currentUser.HasAccess(SystemAccessEnum.ApprovedRequestCommission))
            {
                query = query.Where(p => p.IsCommission == false || p.IsCommission == null);
            }
                        

            return query;
        }

        private IQueryable<ProjectViewModel> ProcessFilterItemsAnd(IQueryable<ProjectViewModel> query, List<FilterItem> filters) {
            foreach (var filterItem in filters)
            {
                var field = filterItem.Field;
                var op = filterItem.Operator;
                var val = filterItem.Value;//string
                var childLogic = filterItem.Logic;
                var childFilters = filterItem.Filters;
                var childFilterName = filterItem.Name;

                if (field != null) // A and B and C and ... (Single)
                {
                    query = ProcessSingleFilter(query, field, op, val);

                }
                if (childFilters != null && childFilters.Count > 0 && childLogic == "or" && childFilterName == "search") // A and (B or C) --- PairFilter Or
                {
                    query = ProcessSearchProjectFilter(query, childFilters);

                }
                else if (childFilters != null && childFilters.Count > 0 && childLogic == "or") // A and (B or C) --- PairFilter Or
                {
                    query = ProcessPairFilterOr(query, childFilters);
                  
                }
                else if(childFilters != null && childFilters.Count > 0 && childLogic == "and") // A and (B and C) --- PairFilter And ==> same As 2 Single Filter Items And
                {
                    query = ProcessPairFilterAnd(query, childFilters);
                    //query = ProcessFilterItemsAnd(query,childFilters);
                }


            }
            return query;
        }

        private IQueryable<ProjectViewModel> ProcessPairFilterAnd(IQueryable<ProjectViewModel> query, List<FilterItem> filters) {
            query = ProcessFilterItemsAnd(query, filters);
            return query;
        }

        //ProcessSingleFilter
        private IQueryable<ProjectViewModel> ProcessSingleFilter(IQueryable<ProjectViewModel> query, string field, string op, string val)
        {
            switch (field)
            {
                case "projectIdStr":
                    if (op == "eq") {
                        var value = Int64.Parse(val);
                        query = query.Where(p => p.ProjectId == value);
                    }
                    break;
                case "projectTypeId":
                    if (op == "eq") {
                        var value = Int32.Parse(val);
                        query = query.Where(p => p.ProjectTypeId == value);
                    }
                    break;
                case "projectStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => p.ProjectStatusId == value);
                    }
                    break;
                case "projectOpenStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => p.ProjectOpenStatusId == value);
                    }
                    break;
                case "projectLeadStatusId":
                    if (op == "eq")
                    {
                        var value = Int32.Parse(val);
                        query = query.Where(p => (int)p.ProjectLeadStatusId == value);
                    }
                    break;
                case "businessName":
                    if (op == "eq")
                    {
                        var value = val;
                        query = query.Where(p => p.BusinessName == value);
                    }
                    break;
                case "projectOwner":
                    if (op == "eq")
                    {
                        var value = val;
                        query = query.Where(p => p.ProjectOwner == value);
                    }
                    break;
                case "isCommission":
                    if (op == "eq")
                    {
                        var value = Boolean.Parse(val);
                        query = query.Where(p => p.IsCommission == value);
                    }
                    break;
                case "pricingStrategy":
                    if (op == "eq")
                    {
                        if (val != null)
                        {
                            if (val == "Commission")
                            {
                                query = query.Where(p => p.IsCommission == true);
                            }
                            else if (val == "Buy/Sell")
                            {
                                query = query.Where(p => p.IsCommission == false);
                            }
                        }
                        else
                        {
                            query = query.Where(p => p.IsCommission == null);
                        }
                    }
                    break;
                //case "deleted":
                //    if (op == "eq")
                //    {
                //        var value = Boolean.Parse(val);
                //        query = query.Where(p => p.Deleted == value);
                //    }
                //    break;
                case "alert":
                    if (op == "eq")
                    {
                        var value = Boolean.Parse(val);
                        query = query.Where(p => p.Alert == value);
                    }
                    break;
                case "bidDate":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate == value);
                    } else if (op == "neq") {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.BidDate <= value);
                    }
                    break;
                case "estimatedClose":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedClose <= value);
                    }
                    break;
                case "estimatedDelivery":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.EstimatedDelivery <= value);
                    }
                    break;
                case "projectDate":
                    if (op == "eq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate == value);
                    }
                    else if (op == "neq")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate != value);
                    }
                    else if (op == "gt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate > value);
                    }
                    else if (op == "lt")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate < value);
                    }
                    else if (op == "gte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate >= value);
                    }
                    else if (op == "lte")
                    {
                        var value = DateTime.Parse(val);
                        query = query.Where(p => p.ProjectDate <= value);
                    }
                    break;

                                  
            }

            return query;

           
        }

        private IQueryable<ProjectViewModel> ProcessPairFilterOr(IQueryable<ProjectViewModel> query, List<FilterItem> filters)
        {
            
            var field1 = filters[0].Field;
            var field2 = filters[0].Field;
            var op1 = filters[0].Operator;
            var op2 = filters[1].Operator;


            if (field1 == "bidDate")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate >= value2);
                }
                else if(op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.BidDate == value1 || p.BidDate <= value2);
                }
                else if(op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate >= value1 || p.BidDate == value2);
                }
                else if(op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.BidDate <= value1 || p.BidDate == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.BidDate <= value1 || p.BidDate >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.BidDate >= value1 || p.BidDate <= value2);
                }
                // ....

            }
                       
            else if (field1 == "estimatedDelivery")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedDelivery == value1 || p.EstimatedDelivery <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery >= value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedDelivery <= value1 || p.EstimatedDelivery == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedDelivery <= value1 || p.EstimatedDelivery >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedDelivery >= value1 || p.EstimatedDelivery <= value2);
                }
                //....
            }

            else if (field1 == "estimatedClose")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedClose == value1 || p.EstimatedClose <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose >= value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.EstimatedClose <= value1 || p.EstimatedClose == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.EstimatedClose <= value1 || p.EstimatedClose >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.EstimatedClose >= value1 || p.EstimatedClose <= value2);
                }
                //....
            }

            else if (field1 == "projectDate")
            {
                var value1 = DateTime.Parse(filters[0].Value);
                var value2 = DateTime.Parse(filters[1].Value);
                if (op1 == "eq" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate == value2);
                }
                else if (op1 == "eq" && op2 == "gte")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate >= value2);
                }
                else if (op1 == "eq" && op2 == "lte")
                {
                    query = query.Where(p => p.ProjectDate == value1 || p.ProjectDate <= value2);
                }
                else if (op1 == "gte" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate >= value1 || p.ProjectDate == value2);
                }
                else if (op1 == "lte" && op2 == "eq")
                {
                    query = query.Where(p => p.ProjectDate <= value1 || p.ProjectDate == value2);
                }
                else if (op1 == "lte" && op2 == "gte")
                {
                    query = query.Where(p => p.ProjectDate <= value1 || p.ProjectDate >= value2);
                }
                else if (op1 == "gte" && op2 == "lte")
                {
                    query = query.Where(p => p.ProjectDate >= value1 || p.ProjectDate <= value2);
                }
            }
            //...
            return query;
        }

        //Process Project Search Box Filter
        private IQueryable<ProjectViewModel> ProcessSearchProjectFilter(IQueryable<ProjectViewModel> query, List<FilterItem> filters) {

            //var field1 = filters[0].Field;
            //var field2 = filters[1].Field;
            //var field3 = filters[2].Field;
            //var field4 = filters[3].Field;

            //var op1 = filters[0].Operator;
            //var op2 = filters[1].Operator;
            //var op3 = filters[2].Operator;
            //var op4 = filters[3].Operator;

            //var value1 = filters[0].Value;
            //var value2 = filters[1].Value;
            //var value3 = filters[2].Value;
            //var value4 = filters[3].Value;

            var value = filters[0].Value;

            query = query.Where(p => p.ProjectId.ToString().Contains(value) || p.Name.Contains(value) || p.ProjectOwner.Contains(value) || p.BusinessName.Contains(value));

            return query;
        }

        private IQueryable<ProjectViewModel> Sort(IQueryable<ProjectViewModel> query, QueryInfo queryInfo)
        {
            if (queryInfo.Sort.Count() > 0)
            {
                string sortcolumn = queryInfo.Sort[0].Field.ToLower();
                bool desc = (queryInfo.Sort[0].Dir == "desc");


                switch (sortcolumn)
                {
                    case "name":
                        query = (desc) ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                        break;
                    case "activequotetitle":
                        query = (desc) ? query.OrderByDescending(s => s.ActiveQuoteTitle) : query.OrderBy(s => s.ActiveQuoteTitle);
                        break;
                    case "biddate":
                        query = (desc) ? query.OrderByDescending(s => s.BidDate) : query.OrderBy(s => s.BidDate);
                        break;

                    case "projectowner":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectOwner) : query.OrderBy(s => s.ProjectOwner);
                        break;

                    case "businessname":
                        query = (desc) ? query.OrderByDescending(s => s.BusinessName) : query.OrderBy(s => s.BusinessName);
                        break;

                    case "customername":
                        query = (desc) ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName);
                        break;

                    case "estimatedclose":
                        query = (desc) ? query.OrderByDescending(s => s.EstimatedClose) : query.OrderBy(s => s.EstimatedClose);
                        break;

                    case "estimateddelivery":
                        query = (desc) ? query.OrderByDescending(s => s.EstimatedDelivery) : query.OrderBy(s => s.EstimatedDelivery);
                        break;

                    case "projectdate":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectDate) : query.OrderBy(s => s.ProjectDate);
                        break;

                    case "projectid":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectId) : query.OrderBy(s => s.ProjectId);
                        break;

                    case "projecttypeid":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectTypeId) : query.OrderBy(s => s.ProjectTypeId);
                        break;
                    case "projectstatusid":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectStatusId) : query.OrderBy(s => s.ProjectStatusId);
                        break;
                    case "projectopenstatusid":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectOpenStatusId) : query.OrderBy(s => s.ProjectOpenStatusId);
                        break;
                    case "projectleadstatusid":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectLeadStatusId) : query.OrderBy(s => s.ProjectLeadStatusId);
                        break;

                    case "totalnet":
                        query = (desc) ? query.OrderByDescending(s => s.TotalNet) : query.OrderBy(s => s.TotalNet);
                        break;

                    case "totallist":
                        query = (desc) ? query.OrderByDescending(s => s.TotalList) : query.OrderBy(s => s.TotalList);
                        break;

                    case "totalsell":
                        query = (desc) ? query.OrderByDescending(s => s.TotalSell) : query.OrderBy(s => s.TotalSell);
                        break;

                    case "projectopenstatus":
                        query = (desc) ? query.OrderByDescending(s => s.ProjectOpenStatus) : query.OrderBy(s => s.ProjectOpenStatus);
                        break;
                    case "darcomstatus":
                        query = (desc) ? query.OrderByDescending(s => s.DarComStatus) : query.OrderBy(s => s.DarComStatus);
                        break;
                    case "totalcountvrvoutdoor":
                        query = (desc) ? query.OrderByDescending(s => s.TotalCountVRVOutDoor) : query.OrderBy(s => s.TotalCountVRVOutDoor);
                        break;
                    case "totalcountsplitoutdoor":
                        query = (desc) ? query.OrderByDescending(s => s.TotalCountSplitOutDoor) : query.OrderBy(s => s.TotalCountSplitOutDoor);
                        break;

                    case "deleted":
                        query = (desc) ? query.OrderByDescending(s => s.Deleted) : query.OrderBy(s => s.Deleted);
                        break;

                    default:
                        query = (desc) ? query.OrderByDescending(s => s.ProjectDate) : query.OrderBy(s => s.ProjectDate);
                        break;
                }
            }
            else {
                query = query.OrderBy(s => s.ProjectDate);
            }
            

            return query;
        }

        public ServiceResponse GetProjectQuoteDARModel(UserSessionModel admin, long? projectId)
        {

            ProjectModel model = null;

            if (projectId.HasValue)
            {
                var query = from project in this.Db.QueryProjectViewableByProjectId(admin, projectId)
                            join quote in this.Db.Quotes on new { id = project.ProjectId, active = true } equals new { id = quote.ProjectId, active = quote.Active } into Laq
                            from quote in Laq.DefaultIfEmpty()

                            join discountRequest in this.Context.DiscountRequests
                                on quote.QuoteId equals discountRequest.QuoteId

                            join transfer in this.Context.ProjectTransfers on new { admin.UserId, project.ProjectId } equals new { transfer.UserId, transfer.ProjectId } into Lt
                            from transfer in Lt.DefaultIfEmpty()

                            join commission in this.Context.CommissionRequests on new { project.ProjectId } equals new { commission.ProjectId } into com
                            from commission in com.DefaultIfEmpty()

                            select new ProjectModel
                            {
                                ProjectId = project.ProjectId,
                                OwnerId = project.Owner.UserId,
                                OwnerName = project.Owner.FirstName + " " + project.Owner.LastName,
                                Name = project.Name,
                                Description = project.Description,
                                CustomerName = project.CustomerName,
                                EngineerName = project.EngineerName,
                                EngineerBusinessName = project.EngineerBusinessName,
                                DealerContractorName = project.DealerContractorName,
                                ShipToName = project.ShipToName,
                                SellerName = project.SellerName,
                                ProjectDate = project.ProjectDate,
                                BidDate = project.BidDate,
                                EstimatedClose = project.EstimatedClose,
                                EstimatedDelivery = project.EstimatedDelivery,
                                Expiration = project.Expiration,
                                ProjectLeadStatusTypeId = project.ProjectLeadStatusTypeId,
                                ProjectLeadStatusTypeDescription = project.ProjectLeadStatusType.Description,
                                ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                ProjectTypeId = project.ProjectTypeId,
                                ConstructionTypeId = project.ConstructionTypeId,
                                VerticalMarketTypeId = project.VerticalMarketTypeId,
                                ERPFirstOrderComment = project.ERPFirstOrderComment,
                                ERPFirstOrderDate = project.ERPFirstOrderDate,
                                ERPFirstOrderNumber = project.ERPFirstOrderNumber,
                                ERPFirstPONumber = project.ERPFirstPONumber,

                                IsTransferred = (transfer != null),

                                CustomerAddress = new AddressModel
                                {
                                    AddressId = project.CustomerAddressId,
                                },
                                SellerAddress = new AddressModel
                                {
                                    AddressId = project.SellerAddressId,
                                },
                                EngineerAddress = new AddressModel
                                {
                                    AddressId = project.EngineerAddressId,
                                },
                                ShipToAddress = new AddressModel
                                {
                                    AddressId = project.ShipToAddressId,
                                },
                                ActiveQuoteSummary = new QuoteListModel
                                {
                                    ProjectId = project.ProjectId,

                                    QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                    Alert = (quote == null) ? false : quote.RecalculationRequired,
                                    Title = (quote == null) ? "" : quote.Title,
                                    Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                    TotalList = (quote == null) ? 0 : quote.TotalList,
                                    TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                    TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                    TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                    Revision = (quote == null) ? 0 : quote.Revision,
                                    CommissionAmount = commission.ApprovedCommissionTotal ?? commission.RequestedCommissionTotal ?? 0,
                                    NetMultiplierValue = commission.RequestedNetMaterialValue ?? 0,
                                    TotalNetCommission = (commission.TotalNet) ?? 0,

                                    HasDAR = (quote == null) ? false : (quote.DiscountRequestId != null || quote.AwaitingDiscountRequest),
                                    AwaitingDiscountRequest = (quote == null) ? false : quote.AwaitingDiscountRequest,

                                    HasCOM = (quote == null) ? false : (quote.AwaitingCommissionRequest),

                                    AwaitingCommissionRequest = (quote == null) ? false : quote.AwaitingCommissionRequest,

                                    IsCommission = (quote == null) ? false : 
                                                   (string.IsNullOrEmpty(quote.IsCommission.ToString())) ? false : quote.IsCommission,
                                    DiscountRequestStatusTypeId = (discountRequest == null) ? (byte)0 : discountRequest.DiscountRequestStatusTypeId
                                },
                                ConstructionTypeDescription = project.ConstructionType.Description,
                                ProjectTypeDescription = project.ProjectType.Description,
                                ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                ProjectStatusDescription = project.ProjectStatusType.Description,
                                VerticalMarketDescription = project.VerticalMarketType.Description,
                                Deleted = project.Deleted,
                                Timestamp = project.Timestamp,
                                ProjectStatusNotes = project.ProjectStatusNotes
                            };

                model = query.FirstOrDefault();
            }

            if (projectId.HasValue && model.ProjectId != projectId)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
            }

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public MemoryStream QuotePrintExcelFile(UserSessionModel user, long projectId, long quoteId, bool showCostPrice)
        {
            QuotePrintModel model = new QuotePrintModel();

            var stream = new MemoryStream();

            var workbook = new HSSFWorkbook();

            var projectResponse = GetProjectModel(user, projectId);

            if (projectResponse.IsOK)
            {
                model.Project = projectResponse.Model as ProjectModel;

                var quoteService = new QuoteServices(this.Context);

                var quoteResponse = quoteService.GetQuoteModel(user, projectId, quoteId);

                var commissionService = new CommissionRequestServices();

                if (quoteResponse.IsOK)
                {
                    model.Quote = quoteResponse.Model as QuoteModel;

                    var itemsRepsonse = quoteService.GetQuoteItemListModel(user, quoteId);

                    if (itemsRepsonse.IsOK)
                    {
                        var items = itemsRepsonse.Model as List<QuoteItemListModel>;
                        var pagedItems = new PagedList<QuoteItemListModel>(items, new Search { Page = 1, PageSize = DPO.Common.Constants.DEFAULT_PAGESIZE_RETURN_ALL });
                        model.QuoteItems = new QuoteItemsModel { Items = pagedItems };

                        //create a entry of DocumentSummaryInformation
                        var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                        dsi.Company = user.BusinessName;
                        workbook.DocumentSummaryInformation = dsi;

                        //create a entry of SummaryInformation
                        var si = PropertySetFactory.CreateSummaryInformation();
                        si.Subject = string.Format("Project '{0}' , Quote '{1}' report ", model.Project.Name, model.Quote.Title);
                        workbook.SummaryInformation = si;


                        var quoteTitle = ConvertToWorkbookName(model.Quote.Title);

                        var worksheet = workbook.CreateSheet(quoteTitle);

                        var ch = new HSSFCreationHelper(workbook);
                        var fe = ch.CreateFormulaEvaluator();

                        int row = 0;

                        //stating create the rows

                        var r = worksheet.CreateRow(row++);
                        SetLabelValue(worksheet, r, 0, 1, ResourceUI.ProjectReference, model.Project.ProjectId.Value.ToString("D18"));
                        SetLabelValue(worksheet, r, 3, 4, "Quote ID", model.Quote.QuoteId.Value.ToString("D18"));

                        r = worksheet.CreateRow(row++);

                        SetLabelValue(worksheet, r, 0, 1, "Project Date:", model.Project.ProjectDate);
                        SetLabelValue(worksheet, r, 3, 4, "Estimated Close:", model.Project.EstimatedClose);
                        SetLabelValue(worksheet, r, 6, 7, "Project Type:", model.Project.ProjectTypeDescription);

                        r = worksheet.CreateRow(row++);
                        SetLabelValue(worksheet, r, 0, 1, "Project:", model.Project.Name);
                        SetLabelValue(worksheet, r, 3, 4, "Estimated Delivery:", model.Project.EstimatedDelivery);
                        SetLabelValue(worksheet, r, 6, 7, "Project Status:", model.Project.ProjectStatusDescription);

                        r = worksheet.CreateRow(row++);
                        SetLabelValue(worksheet, r, 0, 1, "Customer:", model.Project.CustomerName);
                        SetLabelValue(worksheet, r, 3, 4, "Vertical Market:", model.Project.VerticalMarketDescription);
                        SetLabelValue(worksheet, r, 6, 7, "Open Status:", model.Project.ProjectOpenStatusDescription);

                        if (user.ShowPrices)
                        {
                            //Prices
                            row += 2;

                            if (showCostPrice)
                            {
                                r = worksheet.CreateRow(row++);
                                SetLabelValue(worksheet, r, 0, 1, "Total List:", model.Quote.TotalList);
                                r = worksheet.CreateRow(row++);
                                SetLabelValue(worksheet, r, 0, 1, "Total Net:", model.Quote.TotalNet);
                                //SetLabelValue(worksheet, r, 6, 7, "Total Commissioning Cost:", model.Quote.TotalSell - model.Quote.TotalNet);
                                //r = worksheet.CreateRow(row++);
                            }

                            r = worksheet.CreateRow(row++);
                            SetLabelValue(worksheet, r, 0, 1, "Total Sell:", model.Quote.TotalSell);
                            r = worksheet.CreateRow(row++);
                            SetLabelValue(worksheet, r, 0, 1, "Freight Cost:", model.Quote.TotalFreight);
                            r = worksheet.CreateRow(row++);
                            SetLabelValue(worksheet, r, 0, 1, "Total:", model.Quote.TotalSell + model.Quote.TotalFreight);
                        }

                        //Address
                        row += 2;

                        var rowMax = Math.Max(SetAddress(worksheet, 12, 0, "Address", model.Project.CustomerAddress), row);

                        //Seller address
                        rowMax = Math.Max(SetAddress(worksheet, 12, 2, "Seller address", model.Project.SellerAddress), rowMax);

                        //Ship to address
                        rowMax = Math.Max(SetAddress(worksheet, 12, 4, "Ship to address", model.Project.ShipToAddress), rowMax);

                        // headers
                        row = rowMax + 1;

                        r = worksheet.CreateRow(row++);

                        var col = 0;
                        SetLabel(worksheet, r, col++, "Qty");
                        SetLabel(worksheet, r, col++, "Model No");
                        SetLabel(worksheet, r, col++, "Description");

                        if (user.ShowPrices)
                        {
                            if (showCostPrice && showCostPrice)
                            {
                                SetLabel(worksheet, r, col++, "List Price Each");
                                SetLabel(worksheet, r, col++, "Net Price Each");

                                SetLabel(worksheet, r, col++, "List Price Total");
                                SetLabel(worksheet, r, col++, "Net Price Total");
                            }
                        }

                        foreach (var item in model.QuoteItems.Items)
                        {
                            var req = new QuoteCalculationRequest
                            {
                                TotalList = item.PriceList.Value,
                                TotalNet = item.PriceNet.Value,
                                DiscountPercentage = model.Quote.AppliedDiscountPercentage / 100M,
                                CommissionPercentage = model.Quote.AppliedCommissionPercentage / 100M,
                                IsGrossMargin = model.Quote.IsGrossMargin

                            };
                            var cal = quoteService.CalculateTotals(req);

                            r = worksheet.CreateRow(row++);
                            col = 0;
                            SetValue(r, r.CreateCell(col++), item.Quantity);
                            SetValue(r, r.CreateCell(col++), item.ProductNumber);
                            SetValue(r, r.CreateCell(col++), item.Description);

                            if (user.ShowPrices && showCostPrice)
                            {
                                SetValue(r, r.CreateCell(col++), item.PriceList);
                                SetValue(r, r.CreateCell(col++), item.PriceNet);

                                SetValue(r, r.CreateCell(col++), (item.Quantity * item.PriceList));
                                SetValue(r, r.CreateCell(col++), (item.Quantity * item.PriceNet));
                            }

                        }

                        var commissionResponse = commissionService.GetCommissionRequestModel(user,
                                new CommissionRequestModel { CommissionRequestId = model.Quote.CommissionRequestId, ProjectId = model.Project.ProjectId, QuoteId = model.Quote.QuoteId });

                        var commissionRequest = commissionResponse.Model as CommissionRequestModel;

                        //stating Commission Details

                        if ((bool)model.Quote.IsCommission)
                        {
                            row++;
                            r = worksheet.CreateRow(row++);

                            col = 0;

                            SetLabel(worksheet, r, col++, ResourceUI.RequestedCommissionPercent);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedCommissionPercent);
                            SetLabel(worksheet, r, col++, ResourceUI.RequestedCommissionMultiplier);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedCommissionMultiplier);

                            SetLabel(worksheet, r, col++, ResourceUI.RequestedCommissionPercentSplit);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedCommissionPercentSplit);
                            SetLabel(worksheet, r, col++, ResourceUI.RequestedMultiplierSplit);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedMultiplierSplit);

                            SetLabel(worksheet, r, col++, ResourceUI.RequestCommissionPercentVRV);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedCommissionPercentVRV);
                            SetLabel(worksheet, r, col++, ResourceUI.RequestedMultiplierVRV);
                            SetLabel(worksheet, r, col++, ResourceUI.ApprovedMultiplierVRV);

                            r = worksheet.CreateRow(row++);
                            col = 0;
                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedCommissionTotal);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedCommissionTotal);
                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedMultiplier);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedMultiplier);

                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedCommissionPercentageSplit);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedCommissionSplit);
                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedMultiplierSplit);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedMultiplierSplit);

                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedCommissionVRV);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedCommissionVRV);
                            SetValue(r, r.CreateCell(col++), commissionRequest.RequestedMultiplierVRV);
                            SetValue(r, r.CreateCell(col++), commissionRequest.ApprovedMultiplierVRV);
                        }

                        //Notes
                        row++;
                        r = worksheet.CreateRow(row++);
                        SetLabel(worksheet, r, 0, "Notes");

                        // auto size all the columns
                        for (int j = 0; j <= 10; j++)
                        {
                            worksheet.AutoSizeColumn(j);
                        }
                    }
                }
            }

            workbook.Write(stream);

            return stream;
        }

        private static bool CheckIfFileExists(string strDir, string strFile)
        {
            if (File.Exists(strDir + strFile))
                return true;
            else
                return false;
        }

        //Don Carroll created this on 8/3/2015 to fix the formatting issue with the Project Reference column on the excel exorted file.
        private static void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;
        }

        private IRow GetOrCreateRow(ISheet sheet, int row)
        {
            return sheet.GetRow(row) ?? sheet.CreateRow(row);
        }

        private ProjectExportModel PopulateProjectExportModel(object o)
        {
            ProjectExportModel model = new ProjectExportModel();

            return model;
        }

        private Dictionary<string, object> ProjectExportModelToCsv(UserSessionModel admin, ProjectExportModel model, ProjectExportTypeEnum? exportType)
        {
            bool isDaikinReport = admin.isDaikinUser;
            bool showReportPrices = admin.ShowPrices || admin.isDaikinUser;
            bool isDetailedReport = exportType == ProjectExportTypeEnum.PipelineDetailed;
            bool showPipelineNotes = admin.HasAccess(SystemAccessEnum.ViewPipelineData);

            Dictionary<string, object> modelAsCSV = new Dictionary<string, object>
            {
                { ResourceUI.ProjectReference, model.ProjectReference}
            };

            if (isDaikinReport)
            {
                modelAsCSV.Add(ResourceUI.AccountID, model.CRMAccountId);
                modelAsCSV.Add("Region", model.Region);
                modelAsCSV.Add("RSM", model.RSM);
                modelAsCSV.Add("CSM", model.CSM);
            }

            modelAsCSV.Add(ResourceUI.BusinessName, model.BusinessName);
            modelAsCSV.Add(ResourceUI.SellerName, model.SellerName);
            modelAsCSV.Add(ResourceUI.ProjectOwner, model.ProjectOwnerName);

            if (isDetailedReport)
            {
                modelAsCSV.Add(ResourceUI.DealerContractorBusinessName, model.CustomerBusinessName);
                modelAsCSV.Add("Engineer Firm", model.EngineerFirm);
            }

            modelAsCSV.Add(ResourceUI.ProjectName, model.ProjectName);
            modelAsCSV.Add(ResourceUI.ProjectDate, model.ProjectDate);
            modelAsCSV.Add(ResourceUI.ProjectType, model.ProjectType);
            modelAsCSV.Add(ResourceUI.ProjectOpenStatus, model.ProjectOpenStatus);
            modelAsCSV.Add(ResourceUI.ProjectStatus, model.ProjectStatus);
            modelAsCSV.Add(ResourceUI.VerticalMarket, model.VerticalMarketDescription);
            modelAsCSV.Add(ResourceUI.BidDate, model.BidDate);
            modelAsCSV.Add(ResourceUI.EstimatedClose, model.EstimatedCloseDate);
            modelAsCSV.Add(ResourceUI.EstimatedDelivery, model.EstimatedDeliveryDate);

            if (isDetailedReport)
            {
                modelAsCSV.Add(ResourceUI.ProjectExpirationDate, model.ProjectExpirationDate);
                modelAsCSV.Add(ResourceUI.Transferred, model.Transferred);
            }

            modelAsCSV.Add(ResourceUI.ProjectNotes, model.ProjectNotes);

            if (showPipelineNotes)
            {
                modelAsCSV.Add(ResourceUI.PipelineStatus, model.ProjectLeadStatus);
                modelAsCSV.Add(ResourceUI.PipelineUpdate, model.ProjectPipelineNoteType);
                modelAsCSV.Add(ResourceUI.PipelineComment, model.ProjectPipelineNote);
                modelAsCSV.Add(ResourceUI.PipelineCommentDate, model.ProjectPipelineNoteDate);
            }

            if (showReportPrices)
            {
                modelAsCSV.Add(ResourceUI.TotalList, model.TotalList);
                modelAsCSV.Add(ResourceUI.TotalNet, model.TotalNet);
                modelAsCSV.Add(ResourceUI.TotalSell, model.TotalSell);
            }

            if (!isDetailedReport)
            {
                modelAsCSV.Add(ResourceUI.VRVOutdoorUnitQty, model.VRVOutdoorUnitQty);
                modelAsCSV.Add(ResourceUI.RTUQty, model.RTUQty);
                modelAsCSV.Add(ResourceUI.SplitOutdoorUnitQty, model.SplitOutdoorUnitQty);
                modelAsCSV.Add(ResourceUI.VRVIndoorUnitQty, model.VRVIndoorUnitQty);
            }

            modelAsCSV.Add(ResourceUI.QuoteReference, model.QuoteReference);
            modelAsCSV.Add(ResourceUI.QuoteName, model.QuoteName);
            modelAsCSV.Add(ResourceUI.QuoteNotes, model.QuoteNotes);
            modelAsCSV.Add(ResourceUI.Revision, model.Revision);

            if (showReportPrices)
            {
                modelAsCSV.Add(ResourceUI.IsGrossMargin, model.IsGrossMargin);
                //modelAsCSV.Add(ResourceUI.IsCommissionScheme, model.IsCommissionScheme);
                modelAsCSV.Add("Pricing Strategy", model.PricingStrategy);
                modelAsCSV.Add(ResourceUI.TotalFreight, model.TotalFreight);
                modelAsCSV.Add(ResourceUI.CommissionPercentage, model.CommissionPercentage);
                modelAsCSV.Add(ResourceUI.DiscountPercentage, model.DiscountPercentage);
            }

            if (isDetailedReport)
            {
                modelAsCSV.Add(ResourceUI.ProductNumber, model.ProductNumber);
                modelAsCSV.Add(ResourceUI.ProductDescription, model.ProductDescription);
                modelAsCSV.Add("Product Model Type", model.ProductModelType);
                modelAsCSV.Add("Product Type", model.ProductType);
                modelAsCSV.Add("HP/HR", model.HpHr);
                modelAsCSV.Add("Voltage", model.Voltage);
                modelAsCSV.Add(ResourceUI.Quantity, model.Quantity);

                if (showReportPrices)
                {
                    modelAsCSV.Add(ResourceUI.ListPriceEach, model.PriceList);
                    modelAsCSV.Add(ResourceUI.PriceNetEach, model.PriceNet);
                    modelAsCSV.Add("Extended Net Price", model.ExtendedNetPrice);
                    modelAsCSV.Add("Product Class Code", model.ProductClassCode);
                }
            }

            //if (model.IsCommissionRequest != null && model.IsCommissionRequest.Value)
            //{
            //    modelAsCSV.Add(ResourceUI.RequestedCommissionPercent, model.RequestedCommissionPercent);
            //    modelAsCSV.Add(ResourceUI.ApprovedCommissionPercent, model.ApprovedCommissionPercent);
            //    modelAsCSV.Add(ResourceUI.RequestedCommissionMultiplier, model.RequestedMultiplier);
            //    modelAsCSV.Add(ResourceUI.ApprovedCommissionMultiplier, model.ApprovedMultiplier);

            //    modelAsCSV.Add(ResourceUI.RequestedCommissionPercentSplit, model.RequestedCommissionSplitPercent);
            //    modelAsCSV.Add(ResourceUI.ApprovedCommissionPercentSplit, model.ApprovedCommissionSplitPercent);
            //    modelAsCSV.Add(ResourceUI.RequestedMultiplierSplit, model.RequestedCommissionMultiplierSplit);
            //    modelAsCSV.Add(ResourceUI.ApprovedMultiplierSplit, model.ApprovedCommissionMultiplierSplit);

            //    modelAsCSV.Add(ResourceUI.RequestCommissionPercentVRV, model.RequestedCommissionVRVPercent);
            //    modelAsCSV.Add(ResourceUI.ApprovedCommissionPercentVRV, model.ApprovedCommissionVRVPercent);
            //    modelAsCSV.Add(ResourceUI.RequestedMultiplierVRV, model.RequestedCommissionMultiplierVRV);
            //    modelAsCSV.Add(ResourceUI.ApprovedMultiplierVRV, model.ApprovedCommissionMultiplierVRV);
            //}

            return modelAsCSV;
        }

        private int SetAddress(ISheet sheet, int row, int col, string label, AddressModel address)
        {
            var r = GetOrCreateRow(sheet, row);

            SetLabel(sheet, r, col, label);

            r = GetOrCreateRow(sheet, ++row);

            if (address == null) return row;

            if (!string.IsNullOrWhiteSpace(address.AddressLine1))
            {
                r.CreateCell(col).SetCellValue(address.AddressLine1);
                r = GetOrCreateRow(sheet, ++row);
            }
            if (!string.IsNullOrWhiteSpace(address.AddressLine2))
            {
                r.CreateCell(col).SetCellValue(address.AddressLine2);
                r = GetOrCreateRow(sheet, ++row);
            }
            if (!string.IsNullOrWhiteSpace(address.AddressLine3))
            {
                r.CreateCell(col).SetCellValue(address.AddressLine3);
                r = GetOrCreateRow(sheet, ++row);
            }
            if (!string.IsNullOrWhiteSpace(address.Location))
            {
                r.CreateCell(col).SetCellValue(address.Location);
                r = GetOrCreateRow(sheet, ++row);
            }
            if (!string.IsNullOrWhiteSpace(address.PostalCode))
            {
                r.CreateCell(col).SetCellValue(address.PostalCode);
                r = GetOrCreateRow(sheet, ++row);
            }
            return row;
        }

        private void SetLabel(ISheet sheet, IRow row, int col, string value)
        {
            var cell = row.CreateCell(col);
            cell.SetCellValue(value);

            StyleLabel(cell);
        }

        private void SetLabelValue(ISheet sheet, IRow row, int labelCell, int valueCell, string label, string value)
        {
            SetLabel(sheet, row, labelCell, label);

            var cell = row.CreateCell(valueCell, CellType.String);

            HSSFRichTextString rts = new HSSFRichTextString(value);
            cell.SetCellValue(rts);
            //cell.SetCellValue(value);

            StyleValue(cell);
        }

        private void SetLabelValue(ISheet sheet, IRow row, int labelCell, int valueCell, string label, decimal? value)
        {
            SetLabel(sheet, row, labelCell, label);
            if (value.HasValue)
            {
                var cell = row.CreateCell(valueCell);
                cell.SetCellValue((double)value.Value);
                StyleValue(cell);
            }
        }

        private void SetLabelValue(ISheet sheet, IRow row, int labelCell, int valueCell, string label, DateTime? value)
        {
            SetLabel(sheet, row, labelCell, label);
            if (value.HasValue)
            {
                var cell = row.CreateCell(valueCell);
                cell.SetCellValue(value.Value);
                StyleValueDate(cell);
            }
        }

        private void SetValue(IRow row, ICell cell, decimal? value)
        {
            if (value.HasValue)
            {
                cell.SetCellValue((double)value.Value);
                StyleValue(cell);
            }
        }

        private void SetValue(IRow row, ICell cell, string value)
        {
            if (value != null)
            {
                cell.SetCellValue(value);
                StyleValue(cell);
            }
        }

        private void StyleLabel(ICell cell)
        {
            var font = cell.Sheet.Workbook.GetFontAt(0);
            if (font == null)
            {
                font = cell.Sheet.Workbook.CreateFont();
            }

            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            cell.CellStyle = cell.Sheet.Workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
        }

        private void StyleValue(ICell cell)
        {
            var font = cell.Sheet.Workbook.GetFontAt(1);
            if (font == null)
            {
                font = cell.Sheet.Workbook.CreateFont();
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
            }
            cell.CellStyle = cell.Sheet.Workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);
        }

        private void StyleValueDate(ICell cell)
        {
            var font = cell.Sheet.Workbook.GetFontAt(2);
            if (font == null)
            {
                font = cell.Sheet.Workbook.CreateFont();
            }

            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;

            cell.CellStyle = cell.Sheet.Workbook.CreateCellStyle();
            cell.CellStyle.DataFormat = cell.Sheet.Workbook.CreateDataFormat().GetFormat(Resources.ResourceUI.DateFormat);
            cell.CellStyle.SetFont(font);
        }

        #endregion Get Requests

        public ServiceResponse GetProjectPipelineNoteTypes(UserSessionModel user)
        {
            var profiler = MiniProfiler.Current;

            IQueryable<ProjectPipelineNoteTypeModel> query;

            using (profiler.Step("GetProjectPipelineNoteTypes"))
            {
                query = from nt in Db.Context.ProjectPipelineNoteTypes
                            select new ProjectPipelineNoteTypeModel
                            {
                                Description = nt.Description,
                                Name = nt.Name,
                                ProjectPipelineNoteTypeId = nt.ProjectPipelineNoteTypeId
                            };
            }

            using (profiler.Step("Build Model"))
            {
                this.Response.Model = new ProjectPipelineNoteTypeListModel()
                {
                    Items = new PagedList<ProjectPipelineNoteTypeModel>(query.ToArray())
                };
            }

            return this.Response;
        }

        public IEnumerable<DateTypeModel> GetProjectDateTypes()
        {
            var dateTypes = new List<DateTypeModel>(10);
            var profiler = MiniProfiler.Current;

            // TODO:  Switch this to ProjectDateTypeEnum
            var dateTypesString = new string[]
            {
                "Registration Date",
                "Bid Date",
                "Estimated Close",
                "Estimated Delivery"
            };

            using (profiler.Step("GetProjectDateTypes"))
            {
                for (int i = 0; i < dateTypesString.Length; i++)
                {
                    dateTypes.Add(new DateTypeModel
                    {
                        Id = i + 1,
                        Name = dateTypesString[i]
                    });
                }
            }

            return dateTypes;
        }

        #region Finalise Model

        


      

        //public void FinaliseModel(Messages messages, UserSessionModel admin, ProjectModel model)
        //{
        //    //mass upload change - had to turn these off
        //    var service = new HtmlServices(this.Context);

        //    new AddressServices(this.Context).FinaliseModel(model.CustomerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.EngineerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.SellerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.ShipToAddress);

        //    model.ConstructionTypes = service.DropDownModelConstructionTypes((model == null) ? null : model.ConstructionTypeId);

        //    model.ProjectLeadStatusTypes = service.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

        //    model.ProjectStatusTypes = service.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, this.DropDownMode);

        //    model.ProjectTypes = service.DropDownModelProjectTypes((model == null) ? null : model.ProjectTypeId);

        //    model.ProjectOpenStatusTypes = service.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

        //    model.VerticalMarketTypes = service.DropDownModelVerticalMarkets((model == null) ? null : model.VerticalMarketTypeId);
        //}

        #endregion Finalise Model

        #region Post Requests

        public ServiceResponse Delete(UserSessionModel user, ProjectModel model)
        {
            return DeleteAction(user, model, true);
        }

        public ServiceResponse DeleteProjects(UserSessionModel user, List<ProjectModel> projectsDeleteModel)
        {
            ServiceResponse serviceResponse = new ServiceResponse();

            foreach (ProjectModel model in projectsDeleteModel)
            {
                serviceResponse = DeleteAction(user, model, true);
            }

            return serviceResponse;
        }

        public ServiceResponse UpdateProjectOnCommissionRequest(UserSessionModel user, ProjectModel model)
        {
            this.Db.ReadOnly = false;

            if (model == null)
            {
                return this.Response;
            }
            try
            {
                var entity = ModelToEntity(user, model);

                this.SaveToDatabase(model, entity, "Project updated ");

                this.Response.Model = model;
            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            return this.Response;
        }

        public ServiceResponse AddProjectPipelineNote(UserSessionModel user, ProjectPipelineNoteModel model)
        {
            this.Db.ReadOnly = false;

            if (model == null)
            {
                return this.Response;
            }

            try
            {
                // TODO:  How to handle business rules
                Validation.IsText(this.Response.Messages, model.Note, "Note", "Note", 5000, false);
                Validation.IsDropDownSet(this.Response.Messages,
                    model.ProjectPipelineNoteType == null ? 0 : model.ProjectPipelineNoteType.ProjectPipelineNoteTypeId,
                    "ProjectPipelineNoteTypeId", "Project Pipeline Note Type");

                if (this.Response.IsOK)
                {

                    var entity = ModelToEntity(user, model);

                    this.SaveToDatabase(model, entity, string.Format("Project Pipeline Note '{0}'", entity.Note));

                    model.ProjectPipelineNoteId = entity.ProjectPipelineNoteId;
                    model.OwnerName = user != null ? user.FirstName + " " + user.LastName : string.Empty;

                    this.Response.Model = model;
                }
            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            return this.Response;
        }

        public ServiceResponse Duplicate(UserSessionModel user, ProjectModel model)
        {
            this.Db.ReadOnly = false;

            if (model.ProjectId.HasValue)
            {
                Project newProject = Db.ProjectDuplicate(user, model.ProjectId.Value);

                this.Response.Model = newProject;

                try
                {
                    ApplyBusinessRules(user, newProject);

                    if (this.Response.IsOK)
                    {
                        SaveToDatabase(model, newProject, string.Format("Project '{0}'", newProject.Name));
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

        public ServiceResponse PostModel(UserSessionModel admin, ProjectsModel model)
        {
            this.Db.ReadOnly = false;

            this.DropDownMode = DropDownMode.EditRecord; //set default. Changes in ApplyBusinessRules if editing

            RulesOnValidateModel(model);

            bool responseOK = true;
            var messages = new Messages();

            int updateCount = 0;

            foreach (var project in model.Items)
            {
                try
                {
                    Project entity = null;
                    project.ModelSaveState = (int)SaveStateEnum.Unchanged;

                    // Map to Entity
                    if (this.Response.IsOK)
                    {
                        entity = ModelToEntity(admin, project);

                        var entityState = Db.Entry(entity).State;

                        if (entityState == EntityState.Unchanged)
                        {
                            updateCount++;
                            continue;
                        }
                    }

                    // Validate Entity
                    if (this.Response.IsOK)
                    {
                        this.Response.PropertyReference = "";

                        ApplyBusinessRules(admin, entity);
                    }

                    if (this.Response.IsOK)
                    {
                        SaveToDatabase(project, entity, string.Format("Project '{0}'", entity.Name));

                        project.ModelSaveState = (int)SaveStateEnum.Saved;

                        project.ProjectStatus = Db.ProjectStatusTypes.Where(w => w.ProjectStatusTypeId == entity.ProjectStatusTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectOpenStatus = Db.ProjectOpenStatusTypes.Where(w => w.ProjectOpenStatusTypeId == entity.ProjectOpenStatusTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectType = Db.ProjectTypes.Where(w => w.ProjectTypeId == entity.ProjectTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectId = entity.ProjectId;
                        project.Timestamp = entity.Timestamp;
                    }
                    else
                    {
                        Db.Entry(entity).Reload();
                        project.Timestamp = (Db.Entry(entity).Entity as Project).Timestamp;
                    }

                    if (!this.Response.IsOK)
                    {
                        project.ModelSaveState = (int)SaveStateEnum.Error;
                    }

                    if (responseOK)
                    {
                        responseOK = this.Response.IsOK;
                    }

                    updateCount++;

                    if (model.PageSize.HasValue
                        && model.PageSize >= 0
                        && model.PageSize < updateCount)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    project.ModelSaveState = (int)SaveStateEnum.Error;
                    this.Response.AddError(e.Message);
                    this.Response.Messages.AddAudit(e);
                }
                finally
                {
                    // Reset messages for next iteration
                    foreach (var msg in this.Response.Messages.Items)
                    {
                        project.Messages = new Messages();
                        project.Messages.Add(msg);
                    }

                    this.Response.Messages = new Messages();
                }
            }

            this.Response.Messages = messages;

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse PostModel(UserSessionModel admin, ProjectsGridViewModel model)
        {
            this.Db.ReadOnly = false;

            this.DropDownMode = DropDownMode.EditRecord; //set default. Changes in ApplyBusinessRules if editing

            RulesOnValidateModel(model);

            bool responseOK = true;
            var messages = new Messages();

            int updateCount = 0;

            foreach (var project in model.Items)
            {
                try
                {
                    Project entity = null;
                    project.ModelSaveState = (int)SaveStateEnum.Unchanged;

                    // Map to Entity
                    if (this.Response.IsOK)
                    {
                        entity = ModelToEntity(admin, project);

                        Db.Entry(entity).State = EntityState.Modified;

                        //var entityState = Db.Entry(entity).State;

                        //if (entityState == EntityState.Unchanged)
                        //{
                        //    updateCount++;
                        //    continue;
                        //}
                    }

                    // Validate Entity
                    if (this.Response.IsOK)
                    {
                        this.Response.PropertyReference = "";

                        ApplyBusinessRules(admin, entity);
                    }

                    if (this.Response.IsOK)
                    {
                        SaveToDatabase(project, entity, string.Format("Project '{0}'", entity.Name));

                        project.ModelSaveState = (int)SaveStateEnum.Saved;

                        project.ProjectStatus = Db.ProjectStatusTypes.Where(w => w.ProjectStatusTypeId == entity.ProjectStatusTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectOpenStatus = Db.ProjectOpenStatusTypes.Where(w => w.ProjectOpenStatusTypeId == entity.ProjectOpenStatusTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectType = Db.ProjectTypes.Where(w => w.ProjectTypeId == entity.ProjectTypeId).Cache().Select(s => s.Description).FirstOrDefault();
                        project.ProjectId = entity.ProjectId;
                        project.Timestamp = entity.Timestamp;
                    }
                    else
                    {
                        Db.Entry(entity).Reload();
                        project.Timestamp = (Db.Entry(entity).Entity as Project).Timestamp;
                    }

                    if (!this.Response.IsOK)
                    {
                        project.ModelSaveState = (int)SaveStateEnum.Error;
                    }

                    if (responseOK)
                    {
                        responseOK = this.Response.IsOK;
                    }

                    updateCount++;

                    if (model.PageSize.HasValue
                        && model.PageSize >= 0
                        && model.PageSize < updateCount)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    project.ModelSaveState = (int)SaveStateEnum.Error;
                    this.Response.AddError(e.Message);
                    this.Response.Messages.AddAudit(e);
                }
                finally
                {
                    // Reset messages for next iteration
                    foreach (var msg in this.Response.Messages.Items)
                    {
                        project.Messages = new Messages();
                        project.Messages.Add(msg);
                    }

                    this.Response.Messages = new Messages();
                }
            }

            this.Response.Messages = messages;

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse PostModel(UserSessionModel admin, ProjectModel model)
        {
            this.Db.ReadOnly = false;

            this.DropDownMode = DropDownMode.NewRecord; //set default. Changes in ApplyBusinessRules if editing

            try
            {
                Project entity = null;

                // Validate Model
                RulesOnValidateModel(model);

                // Map to Entity
                if (this.Response.IsOK)
                {
                    entity = ModelToEntity(admin, model);
                }

                // Validate Entity
                if (this.Response.IsOK)
                {
                    this.Response.PropertyReference = "";

                    ApplyBusinessRules(admin, entity);
                }

                if (this.Response.IsOK)
                {
                    SaveToDatabase(model, entity, string.Format("Project '{0}'", entity.Name));

                    model.ProjectId = entity.ProjectId;
                }
            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse ProjectTransfer(UserSessionModel user, long? projectId, string email)
        {
            this.Db.ReadOnly = false;

            if (projectId.HasValue && email != null)
            {
                Project project = Db.ProjectTransfer(user, projectId.Value, email);

                this.Response.Model = project.Name;

                try
                {
                    ApplyBusinessRules(user, project);

                    if (this.Response.IsOK)
                    {
                        SaveToDatabase(new ProjectModel { ProjectId = projectId.Value }, project, string.Format("Transferred '{0}' to '{1}'. Project ", project.Name, email));
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

        public ServiceResponse Undelete(UserSessionModel admin, ProjectModel model)
        {
            return DeleteAction(admin, model, false);
        }

        public ServiceResponse VerifiyIsUser(string Email)
        {
            this.Response = new ServiceResponse();
            if (!Db.IsUser(Email))
            {
                this.Response.AddError(Resources.ResourceModelUser.MU007);
            }
            return this.Response;
        }

        private ServiceResponse DeleteAction(UserSessionModel user, ProjectModel model, bool delete)
        {
            this.Db.ReadOnly = false;
            model.Deleted = delete;

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                entity.Deleted = delete;

                Entry = Db.Entry(entity);

                if (delete) RulesOnDelete(user, entity);
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, string.Format("Project '{0}'", entity.Name));
            }

            this.Response.Model = model;

            return this.Response;
        }

        #endregion Post Requests

        #region Post Model To Entity

        public Project ModelToEntity(UserSessionModel admin, ProjectListModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.BidDate = model.BidDate;

            entity.EstimatedClose = model.EstimatedClose;

            entity.EstimatedDelivery = model.EstimatedDelivery;

            entity.Expiration = model.EstimatedClose;

            entity.ProjectOpenStatusTypeId = (byte)model.ProjectOpenStatusId;

            entity.ProjectTypeId = (byte)model.ProjectTypeId;

            if (model.ProjectLeadStatusId != null)
            {
                entity.ProjectLeadStatusTypeId = model.ProjectLeadStatusId.Value;
            }

            entity.ProjectStatusTypeId = (ProjectStatusTypeEnum)model.ProjectStatusId;

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        public Project ModelToEntity(UserSessionModel admin, ProjectViewModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.BidDate = model.BidDate;

            entity.EstimatedClose = model.EstimatedClose;

            entity.EstimatedDelivery = model.EstimatedDelivery;

            entity.Expiration = model.EstimatedClose;

            entity.ProjectOpenStatusTypeId = (byte)model.ProjectOpenStatusId;

            entity.ProjectTypeId = (byte)model.ProjectTypeId;

            if (model.ProjectLeadStatusId != null)
            {
                entity.ProjectLeadStatusTypeId = model.ProjectLeadStatusId.Value;
            }

            entity.ProjectStatusTypeId = (ProjectStatusTypeEnum)model.ProjectStatusId;

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        public Project ModelToEntity(UserSessionModel admin, ProjectModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.Name = Utilities.Trim(model.Name);

            entity.CustomerName = Utilities.Trim(model.CustomerName);

            entity.EngineerBusinessName = Utilities.Trim(model.EngineerBusinessName);

            entity.EngineerName = Utilities.Trim(model.EngineerName);

            entity.DealerContractorName = Utilities.Trim(model.DealerContractorName);

            entity.SellerName = Utilities.Trim(model.SellerName);

            entity.ShipToName = Utilities.Trim(model.ShipToName);

            entity.Description = Utilities.Trim(model.Description);

            entity.ProjectDate = model.ProjectDate ?? DateTime.UtcNow;

            entity.BidDate = model.BidDate ?? DateTime.UtcNow;

            entity.EstimatedClose = model.EstimatedClose ?? DateTime.UtcNow;

            entity.EstimatedDelivery = model.EstimatedDelivery ?? DateTime.UtcNow;

            entity.Expiration = (model.EstimatedClose != null) ? model.EstimatedClose.Value.AddDays(30) : DateTime.UtcNow.AddDays(30);

            entity.ProjectOpenStatusTypeId = model.ProjectOpenStatusTypeId.Value;

            entity.ProjectTypeId = model.ProjectTypeId.Value;

            entity.ConstructionTypeId = model.ConstructionTypeId.Value;

            entity.ProjectStatusTypeId = (ProjectStatusTypeEnum)model.ProjectStatusTypeId.Value;

            entity.VerticalMarketTypeId = model.VerticalMarketTypeId.Value;

            entity.EstimateReleaseDate = model.EstimateReleaseDate;

            entity.ActualCloseDate = model.ActualCloseDate;

            entity.SquareFootage = model.SquareFootage;

            entity.NumberOfFloors = model.NumberOfFloors;

            entity.OrderStatus = model.OrderStatus;

            if (!model.ProjectLeadStatusTypeId.HasValue)
            {
                entity.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Lead;
            }
            else
            {
                entity.ProjectLeadStatusTypeId = model.ProjectLeadStatusTypeId.Value;
            }

            addressService.BeginPropertyReference(this, "EngineerAddress");
            addressService.ModelToEntity(model.EngineerAddress);
            addressService.EndPropertyReference();

            if (model.EngineerAddress != null)
            {
                entity.EngineerAddress = addressService.ModelToEntity(model.EngineerAddress);
            }

            addressService.BeginPropertyReference(this, "CustomerAddress");
            addressService.ModelToEntity(model.CustomerAddress);
            addressService.EndPropertyReference();

            if (model.CustomerAddress != null)
            {
                entity.CustomerAddress = addressService.ModelToEntity(model.CustomerAddress);
            }

            addressService.BeginPropertyReference(this, "SellerAddress");
            addressService.ModelToEntity(model.SellerAddress);
            addressService.EndPropertyReference();

            if (model.SellerAddress != null)
            {
                entity.SellerAddress = addressService.ModelToEntity(model.SellerAddress);
            }

            addressService.BeginPropertyReference(this, "ShipToAddress");
            addressService.ModelToEntity(model.ShipToAddress);
            addressService.EndPropertyReference();

            if (model.ShipToAddress != null)
            {
                entity.ShipToAddress = addressService.ModelToEntity(model.ShipToAddress);
            }

            entity.ProjectStatusNotes = model.ProjectStatusNotes;

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        public ProjectPipelineNote ModelToEntity(UserSessionModel admin, ProjectPipelineNoteModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.Note = model.Note;
            entity.Title = model.Title;
            entity.ProjectId = model.ProjectId;
            entity.ProjectPipelineNoteTypeId = model.ProjectPipelineNoteType.ProjectPipelineNoteTypeId;
            entity.Timestamp = model.Timestamp;
            entity.OwnerId = admin.UserId;

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }


        #endregion Post Model To Entity


        public Project GetEntity(UserSessionModel user, ProjectListModel model)
        {
            var entity = model.ProjectId.HasValue ? this.Db.QueryProjectViewableByProjectId(user, model.ProjectId).FirstOrDefault() : Db.ProjectCreate(user);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        public Project GetEntity(UserSessionModel user, ProjectViewModel model)
        {
            var entity = model.ProjectId.HasValue ? this.Db.QueryProjectViewableByProjectId(user, model.ProjectId).FirstOrDefault() : Db.ProjectCreate(user);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        //private Project GetEntity(UserSessionModel user, ProjectPipelineNoteModel model)
        //{
        //    var entity = model.ProjectPipelineNoteId.HasValue ? this.Db.(user, model.ProjectId).FirstOrDefault() : Db.ProjectCreate(user);

        //    if (entity == null)
        //    {
        //        this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
        //    }

        //    return entity;
        //}

        public Project GetEntity(UserSessionModel user, ProjectModel model)
        {
            var entity = model.ProjectId.HasValue ? this.Db.QueryProjectViewableByProjectId(user, model.ProjectId).FirstOrDefault() : Db.ProjectCreate(user);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        private ProjectPipelineNote GetEntity(UserSessionModel admin, ProjectPipelineNoteModel model)
        {
            ProjectPipelineNote entity = null;

            if (model.ProjectPipelineNoteId.HasValue)
            {
                // TODO:  Add security?
                entity = (from pn in this.Context.ProjectPipelineNotes
                          where pn.ProjectPipelineNoteId == model.ProjectPipelineNoteId.Value
                          select pn).FirstOrDefault();
            }

            if (entity == null)
            {
                entity = Db.ProjectPipelineNoteCreate(admin);
            }

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        //public ServiceResponse GetProjectLocation(UserSessionModel userSessionModel, long? projectId)
        //{
        //    if (projectId.HasValue)
        //    {
        //        var query = from project in this.Context.Projects
        //                    join address in this.Context.Addresses on project.ShipToAddressId equals address.AddressId
        //                    where project.ProjectId == projectId
        //                    select new ShipToAddressViewModel
        //                    {
        //                        AddressId = address.AddressId,
        //                        ShipToName = project.ShipToName,
        //                        AddressLine1 = address.AddressLine1,
        //                        AddressLine2 = address.AddressLine2,
        //                        AddressLine3 = address.AddressLine3,
        //                        Location = address.Location,
        //                        PostalCode = address.PostalCode,
        //                        StateId = address.StateId,
        //                        CountryCode = address.State.CountryCode
        //                    };

        //        var model = query.FirstOrDefault();

        //        this.Response.Model = model;
        //    }

        //    return this.Response;


        //}

        public string GetStateByStateId(int stateId)
        {
            var state = this.Db.States.Where(s => s.StateId == stateId).Select(s => s.Name).First();

            return state;
        }

        public string GetStateCodeByStateId(int stateId)
        {
            var stateCode = this.Db.States.Where(s => s.StateId == stateId).Select(s => s.Code).First();
            return stateCode;
        }

        public int GetStateIdByState(string state)
        {
            var stateId = this.Db.States.Where(s => s.Code == state).Select(s => s.StateId).First();
            return stateId;
        }

        public ServiceResponse ValidateProjectLocationAddress(ProjectModel model)
        {
            DataQualityService.CleanAddressRequest addressReq = new DataQualityService.CleanAddressRequest();
            addressReq.Address = new DataQualityService.DQAddress();

            if (model.ShipToAddress.AddressLine1.Contains("."))
            {
                string newAddressLine1 = model.ShipToAddress.AddressLine1.Replace(".", "");
                model.ShipToAddress.AddressLine1 = newAddressLine1;
            }
            if (model.ShipToAddress.AddressLine1.Contains(","))
            {
                string newAddressLine1 = model.ShipToAddress.AddressLine1.Replace(",", "");
                model.ShipToAddress.AddressLine1 = newAddressLine1;
            }

            List<string> specificCharacters = new List<string> { "#", "Suite", "suite", "Ste", "ste", "STE" };

            addressReq.Address.Line1 = model.ShipToAddress.AddressLine1;
            addressReq.Address.Line2 = (model.ShipToAddress.AddressLine2 != null) ? model.ShipToAddress.AddressLine2 : string.Empty;

            if (model.ShipToAddress.StateId != null)
            {
                addressReq.Address.StateProvince = GetStateCodeByStateId(model.ShipToAddress.StateId.Value);
            }

            addressReq.Address.ZipCode = model.ShipToAddress.PostalCode;
            addressReq.Address.City = model.ShipToAddress.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            DataQualityService.CleanAddressResponse addressResp = (DataQualityService.CleanAddressResponse)proxy.Execute(addressReq);

            DataTable AddressDt = new DataTable("ShippingAddressSuggestion");
            DataColumn AddressLine1 = new DataColumn("AddressLine1", typeof(string));
            DataColumn AddressLine2 = new DataColumn("AddressLine2", typeof(string));
            DataColumn State = new DataColumn("State", typeof(string));
            DataColumn ZipCode = new DataColumn("ZipCode", typeof(string));
            DataColumn City = new DataColumn("City", typeof(string));

            AddressDt.Columns.Add(AddressLine1);
            AddressDt.Columns.Add(AddressLine2);
            AddressDt.Columns.Add(City);
            AddressDt.Columns.Add(State);
            AddressDt.Columns.Add(ZipCode);

            if (addressResp.Addresses != null && addressResp.Addresses.Count() > 0)
            {
                if (addressResp.Addresses[0].Line1 != addressReq.Address.Line1 ||
                addressResp.Addresses[0].City != addressReq.Address.City ||
                addressResp.Addresses[0].StateProvince != addressReq.Address.StateProvince ||
                addressResp.Addresses[0].ZipCode != addressReq.Address.ZipCode)
                {
                    StringBuilder output = new StringBuilder();

                    if (model.ShippingSuggestionAddress == null)
                    {
                        model.ShippingSuggestionAddress = new Dictionary<string, string>();
                    }

                    foreach (var item in addressResp.Addresses)
                    {
                        DataQualityService.DQAddress address = new DataQualityService.DQAddress();

                        if (addressReq.Address.Line2 != null)
                        {
                            foreach (string chars in specificCharacters)
                            {
                                if (addressReq.Address.Line2.Contains(chars))
                                {
                                    item.Line2 = addressReq.Address.Line2;
                                }
                            }

                        }

                        AddressDt.Rows.Add(item.Line1, item.Line2, item.City, item.StateProvince, item.ZipCode);
                    }

                    foreach (DataRow rows in AddressDt.Rows)
                    {
                        foreach (DataColumn col in AddressDt.Columns)
                        {
                            output.AppendFormat("{0} ", rows[col]);
                            model.ShippingSuggestionAddress.Add(col.ColumnName, rows[col].ToString());
                        }

                        output.AppendLine();
                    }

                    model.HasSuggestionAddress = true;

                    string messageTable = "<table class='ShippingAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                   "<td style='width:400px;'>Shipping Address you entered</td>" +
                                   "<td style='width:400px;'>Shipping Address we recommend</td>" +
                                   "</tr>" +
                                   "<tr>" +
                                   "<td style='width:400px;'>" +
                                   model.ShipToAddress.AddressLine1 + " " +
                                   model.ShipToAddress.AddressLine2 + " " +
                                   model.ShipToAddress.Location + " " +
                                   GetStateByStateId(model.ShipToAddress.StateId.Value) + " " +
                                   model.ShipToAddress.PostalCode +
                                   "</td>" +
                                   "<td style='width:400px;'>" +
                                   output.ToString() +
                                   "</td>" +
                                   "<td>" +
                                   "<input type='checkbox' id='useSuggestionShippingAddress' name='useSuggestionShippingAddress' value='True'>Use Suggested Address</input>" +
                                   "</td>" +
                                   "</tr>" +
                                   "</table>";

                    this.Response.Messages.AddError(messageTable);

                }
            }

            if (addressResp.Error.Message != null && addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                string errorMessage = "<table class='ShippingAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                   "<td style='width:400px;'>" +
                                   "Project Shipping Address is invalid.Please provide a valid address." +
                                   "</td>" +
                                   "<td style='width:400px;'>" +
                                   "<input type='checkbox' id='useShippingAddressAsIs' name='useShippingAddressAsIs' value='True'>Check this box if you still want to use this Address</input>" +
                                   "</td>" +
                                   "</tr></table>";
                this.Response.AddError(errorMessage);
            }

            FinaliseModel(this.Response.Messages, model.CurrentUser, model);
            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse ValidateDealorAddress(ProjectModel model)
        {
            DataQualityService.CleanAddressRequest addressReq = new DataQualityService.CleanAddressRequest();
            addressReq.Address = new DataQualityService.DQAddress();

            if (model.CustomerAddress.AddressLine1.Contains("."))
            {
                string newAddressLine1 = model.CustomerAddress.AddressLine1.Replace(".", "");
                model.CustomerAddress.AddressLine1 = newAddressLine1;
            }
            if (model.CustomerAddress.AddressLine1.Contains(","))
            {
                string newAddressLine1 = model.CustomerAddress.AddressLine1.Replace(",", "");
                model.CustomerAddress.AddressLine1 = newAddressLine1;
            }

            List<string> specificCharacters = new List<string> { "#", "Suite", "suite", "Ste", "ste", "STE" };

            //foreach (string chars in specificCharacters)
            //{
            //    if (model.CustomerAddress.AddressLine1.Contains(chars))
            //    {
            //        int index = model.CustomerAddress.AddressLine1.IndexOf(chars);
            //        string newAddressLine1 = model.CustomerAddress.AddressLine1.Substring(0, (index - 1));
            //        string newAddressLine2 = model.CustomerAddress.AddressLine1.Substring(index, (model.CustomerAddress.AddressLine1.Length - index));
            //        model.CustomerAddress.AddressLine1 = newAddressLine1;
            //        model.CustomerAddress.AddressLine2 = newAddressLine2;

            //    }
            //}

            addressReq.Address.Line1 = model.CustomerAddress.AddressLine1;
            addressReq.Address.Line2 = (model.CustomerAddress.AddressLine2 != null) ? model.CustomerAddress.AddressLine2 : string.Empty;

            if (model.CustomerAddress.StateId != null)
            {
                addressReq.Address.StateProvince = GetStateCodeByStateId(model.CustomerAddress.StateId.Value);
            }

            addressReq.Address.ZipCode = model.CustomerAddress.PostalCode;
            addressReq.Address.City = model.CustomerAddress.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            DataQualityService.CleanAddressResponse addressResp = (DataQualityService.CleanAddressResponse)proxy.Execute(addressReq);

            DataTable AddressDt = new DataTable("DealorAddressSuggestion");
            DataColumn AddressLine1 = new DataColumn("AddressLine1", typeof(string));
            DataColumn AddressLine2 = new DataColumn("AddressLine2", typeof(string));
            DataColumn State = new DataColumn("State", typeof(string));
            DataColumn ZipCode = new DataColumn("ZipCode", typeof(string));
            DataColumn City = new DataColumn("City", typeof(string));

            AddressDt.Columns.Add(AddressLine1);
            AddressDt.Columns.Add(AddressLine2);
            AddressDt.Columns.Add(City);
            AddressDt.Columns.Add(State);
            AddressDt.Columns.Add(ZipCode);

            if (model.SuggesstionAddresses == null)
            {
                model.SuggesstionAddresses = new List<string>();
            }



            if (addressResp.Addresses != null && addressResp.Addresses.Count() > 0)
            {
                if (addressResp.Addresses[0].Line1 != addressReq.Address.Line1 ||
               addressResp.Addresses[0].City != addressReq.Address.City ||
               addressResp.Addresses[0].StateProvince != addressReq.Address.StateProvince ||
               addressResp.Addresses[0].ZipCode != addressReq.Address.ZipCode)
                {

                    StringBuilder output = new StringBuilder();

                    if (model.DealorContractorSuggestionAddress == null)
                    {
                        model.DealorContractorSuggestionAddress = new Dictionary<string, string>();
                    }

                    foreach (var item in addressResp.Addresses)
                    {
                        DataQualityService.DQAddress address = new DataQualityService.DQAddress();

                        foreach (string chars in specificCharacters)
                        {
                            if (addressReq.Address.Line2.Contains(chars))
                            {
                                item.Line2 = addressReq.Address.Line2;
                            }
                        }

                        AddressDt.Rows.Add(item.Line1, item.Line2, item.City, item.StateProvince, item.ZipCode);
                    }

                    foreach (DataRow rows in AddressDt.Rows)
                    {
                        foreach (DataColumn col in AddressDt.Columns)
                        {
                            output.AppendFormat("{0} ", rows[col]);
                            model.DealorContractorSuggestionAddress.Add(col.ColumnName, rows[col].ToString());
                        }

                        output.AppendLine();
                    }

                    model.SuggesstionAddresses.Add(output.ToString());
                    model.HasSuggestionAddress = true;

                    string messageTable = "<table class='DealerAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                  "<td style='width:400px;'>Dealer/Contractor Address you entered</td>" +
                                  "<td style='width:400px;'>Dealer/Contractor Address we recommend</td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='width:400px'>" +
                                  model.CustomerAddress.AddressLine1 + " " +
                                  model.CustomerAddress.AddressLine2 + " " +
                                  model.CustomerAddress.Location + " " +
                                  GetStateByStateId(model.CustomerAddress.StateId.Value) + " " +
                                  model.CustomerAddress.PostalCode +
                                  "</td>" +
                                  "<td style='width:400px;'>" +
                                 output.ToString() +
                                  "</td>" +
                                    "<td>" +
                                   "<input type='checkbox' id='useSuggestionDealorAddress' name='useSuggestionDealorAddress' value='True'>Use Suggested Address</input>" +
                                   "</td>" +
                                  "</tr>" +
                                  "</table>";

                    this.Response.Messages.AddError(messageTable);
                }

            }

            if (addressResp.Error.Message != null && addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                string errorMessage = "<table class='DealerAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                      "<td style='width:400px;'>" +
                                      "Dealor/Contractor Address is invalid.Please provide a valid address." +
                                      "</td>" +
                                      "<td style='width:400px;'>" +
                                      "<input type='checkbox' id='useDealerAddressAsIs' name='useDealerAddressAsIs' value='True'>Check this box if you still want to use this Address</input>" +
                                      "</td>" +
                                      "</tr></table>";

                this.Response.AddError(errorMessage);
            }

            FinaliseModel(this.Response.Messages, model.CurrentUser, model);
            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse ValidateSellerAddress(ProjectModel model)
        {
            DataQualityService.CleanAddressRequest addressReq = new DataQualityService.CleanAddressRequest();
            addressReq.Address = new DataQualityService.DQAddress();

            if (model.SellerAddress.AddressLine1.Contains("."))
            {
                string newAddressLine1 = model.SellerAddress.AddressLine1.Replace(".", "");
                model.SellerAddress.AddressLine1 = newAddressLine1;
            }
            if (model.SellerAddress.AddressLine1.Contains(","))
            {
                string newAddressLine1 = model.SellerAddress.AddressLine1.Replace(",", "");
                model.SellerAddress.AddressLine1 = newAddressLine1;
            }

            List<string> specificCharacters = new List<string> { "#", "Suite", "suite", "Ste", "ste", "STE" };

            //foreach (string chars in specificCharacters)
            //{
            //    if (model.SellerAddress.AddressLine1.Contains(chars))
            //    {
            //        int index = model.SellerAddress.AddressLine1.IndexOf(chars);
            //        string newAddressLine1 = model.SellerAddress.AddressLine1.Substring(0, (index - 1));
            //        string newAddressLine2 = model.SellerAddress.AddressLine1.Substring(index, (model.SellerAddress.AddressLine1.Length - index));
            //        model.SellerAddress.AddressLine1 = newAddressLine1;
            //        model.SellerAddress.AddressLine2 = newAddressLine2;

            //    }
            //}

            addressReq.Address.Line1 = model.SellerAddress.AddressLine1;
            addressReq.Address.Line2 = (model.SellerAddress.AddressLine2 != null) ? model.SellerAddress.AddressLine2 : string.Empty;

            if (model.SellerAddress.StateId != null)
            {
                addressReq.Address.StateProvince = GetStateByStateId(model.SellerAddress.StateId.Value);
            }

            addressReq.Address.ZipCode = model.SellerAddress.PostalCode;
            addressReq.Address.City = model.SellerAddress.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            DataQualityService.CleanAddressResponse addressResp = (DataQualityService.CleanAddressResponse)proxy.Execute(addressReq);

            DataTable AddressDt = new DataTable("SellerAddressSuggestion");
            DataColumn AddressLine1 = new DataColumn("AddressLine1", typeof(string));
            DataColumn AddressLine2 = new DataColumn("AddressLine2", typeof(string));
            DataColumn State = new DataColumn("State", typeof(string));
            DataColumn ZipCode = new DataColumn("ZipCode", typeof(string));
            DataColumn City = new DataColumn("City", typeof(string));

            AddressDt.Columns.Add(AddressLine1);
            AddressDt.Columns.Add(AddressLine2);
            AddressDt.Columns.Add(City);
            AddressDt.Columns.Add(State);
            AddressDt.Columns.Add(ZipCode);


            if (model.SuggesstionAddresses == null)
            {
                model.SuggesstionAddresses = new List<string>();
            }


            if (addressResp.Addresses != null && addressResp.Addresses.Count() > 0)
            {
                if (addressResp.Addresses[0].Line1 != addressReq.Address.Line1 ||
              addressResp.Addresses[0].City != addressReq.Address.City ||
              addressResp.Addresses[0].StateProvince != addressReq.Address.StateProvince ||
              addressResp.Addresses[0].ZipCode != addressReq.Address.ZipCode)
                {

                    StringBuilder output = new StringBuilder();

                    if (model.SellerSuggestionAddress == null)
                    {
                        model.SellerSuggestionAddress = new Dictionary<string, string>();
                    }

                    foreach (var item in addressResp.Addresses)
                    {
                        DataQualityService.DQAddress address = new DataQualityService.DQAddress();

                        foreach (string chars in specificCharacters)
                        {
                            if (addressReq.Address.Line2.Contains(chars))
                            {
                                item.Line2 = addressReq.Address.Line2;
                            }
                        }

                        AddressDt.Rows.Add(item.Line1, item.Line2, item.City, item.StateProvince, item.ZipCode);
                    }

                    foreach (DataRow rows in AddressDt.Rows)
                    {
                        foreach (DataColumn col in AddressDt.Columns)
                        {
                            output.AppendFormat("{0} ", rows[col]);
                            model.SellerSuggestionAddress.Add(col.ColumnName, rows[col].ToString());
                        }

                        output.AppendLine();
                    }

                    model.SuggesstionAddresses.Add(output.ToString());
                    model.HasSuggestionAddress = true;

                    string messageTable = "<table><tr style='font-weight:bold;font-size:14px;'>" +
                                  "<td style='width:400px'>Seller Address you entered</td>" +
                                  "<td style='width:400px;'>Seller Address we recommend</td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='width:400px;'>" +
                                  model.SellerAddress.AddressLine1 + " " +
                                  model.SellerAddress.AddressLine2 + " " +
                                  model.SellerAddress.Location + " " +
                                  GetStateByStateId(model.SellerAddress.StateId.Value) + " " +
                                  model.SellerAddress.PostalCode +
                                  "</td>" +
                                  "<td style='width:400px;'>" +
                                 output.ToString() +
                                  "</td>" +
                                    "<td>" +
                                   "<input type='checkbox' id='useSuggestionSellerAddress' name='useSuggestionSellerAddress' value='True'>Use Suggested Address</input>" +
                                   "</td>" +
                                  "</tr>" +
                                  "</table>";

                    this.Response.Messages.AddError(messageTable);
                }
            }
            if (addressResp.Error.Message != null && addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                string errorMessage = "<table class='SellerAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                      "<td style='width:400px;'>" +
                                      "Seller Address is invalid.Please provide a valid address" +
                                      "</td>" +
                                      "<td style='width:400px;'>" +
                                      "<input type = 'checkbox' id = 'useSellerAddressAsIs' name = 'useSellerAddressAsIs' value = 'True' > Check this box if you still want to use this Address </ input > " +
                                      "</td>" +
                                      "</tr></table>";

                this.Response.AddError(errorMessage);
            }

            FinaliseModel(this.Response.Messages, model.CurrentUser, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse ValidateEngineerAddress(ProjectModel model)
        {
            DataQualityService.CleanAddressRequest addressReq = new DataQualityService.CleanAddressRequest();
            addressReq.Address = new DataQualityService.DQAddress();

            if (model.EngineerAddress.AddressLine1.Contains("."))
            {
                string newAddressLine1 = model.EngineerAddress.AddressLine1.Replace(".", "");
                model.EngineerAddress.AddressLine1 = newAddressLine1;
            }
            if (model.EngineerAddress.AddressLine1.Contains(","))
            {
                string newAddressLine1 = model.EngineerAddress.AddressLine1.Replace(",", "");
                model.EngineerAddress.AddressLine1 = newAddressLine1;
            }

            List<string> specificCharacters = new List<string> { "#", "Suite ", "Ste ", "ste ", "STE" };

            //foreach (string chars in specificCharacters)
            //{
            //    if (model.EngineerAddress.AddressLine1.Contains(chars))
            //    {
            //        int index = model.EngineerAddress.AddressLine1.IndexOf(chars);
            //        string newAddressLine1 = model.EngineerAddress.AddressLine1.Substring(0, (index - 1));
            //        string newAddressLine2 = model.EngineerAddress.AddressLine1.Substring(index, (model.EngineerAddress.AddressLine1.Length - index));
            //        model.EngineerAddress.AddressLine1 = newAddressLine1;
            //        model.EngineerAddress.AddressLine2 = newAddressLine2;

            //    }
            //}

            addressReq.Address.Line1 = model.EngineerAddress.AddressLine1;
            addressReq.Address.Line2 = (model.EngineerAddress.AddressLine2 != null) ? model.EngineerAddress.AddressLine2 : string.Empty;

            if (model.EngineerAddress.StateId != null)
            {
                addressReq.Address.StateProvince = GetStateCodeByStateId(model.EngineerAddress.StateId.Value);
            }

            addressReq.Address.ZipCode = model.EngineerAddress.PostalCode;
            addressReq.Address.City = model.EngineerAddress.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            DataQualityService.CleanAddressResponse addressResp = (DataQualityService.CleanAddressResponse)proxy.Execute(addressReq);

            DataTable AddressDt = new DataTable("EngineerAddressSuggestion");
            DataColumn AddressLine1 = new DataColumn("AddressLine1", typeof(string));
            DataColumn AddressLine2 = new DataColumn("AddressLine2", typeof(string));
            DataColumn State = new DataColumn("State", typeof(string));
            DataColumn ZipCode = new DataColumn("ZipCode", typeof(string));
            DataColumn City = new DataColumn("City", typeof(string));

            AddressDt.Columns.Add(AddressLine1);
            AddressDt.Columns.Add(AddressLine2);
            AddressDt.Columns.Add(City);
            AddressDt.Columns.Add(State);
            AddressDt.Columns.Add(ZipCode);


            if (model.SuggesstionAddresses == null)
            {
                model.SuggesstionAddresses = new List<string>();
            }

            if (addressResp.Addresses != null && addressResp.Addresses.Count() > 0)
            {
                if (addressResp.Addresses[0].Line1 != addressReq.Address.Line1 ||
              addressResp.Addresses[0].City != addressReq.Address.City ||
              addressResp.Addresses[0].StateProvince != addressReq.Address.StateProvince ||
              addressResp.Addresses[0].ZipCode != addressReq.Address.ZipCode)
                {

                    StringBuilder output = new StringBuilder();

                    if (model.EngineerSuggestionAddress == null)
                    {
                        model.EngineerSuggestionAddress = new Dictionary<string, string>();
                    }

                    foreach (var item in addressResp.Addresses)
                    {
                        DataQualityService.DQAddress address = new DataQualityService.DQAddress();

                        foreach (string chars in specificCharacters)
                        {
                            if (addressReq.Address.Line2.Contains(chars))
                            {
                                item.Line2 = addressReq.Address.Line2;
                            }
                        }

                        AddressDt.Rows.Add(item.Line1, item.Line2, item.City, item.StateProvince, item.ZipCode);
                    }

                    foreach (DataRow rows in AddressDt.Rows)
                    {
                        foreach (DataColumn col in AddressDt.Columns)
                        {
                            output.AppendFormat("{0} ", rows[col]);
                            model.EngineerSuggestionAddress.Add(col.ColumnName, rows[col].ToString());
                        }

                        output.AppendLine();
                    }

                    model.SuggesstionAddresses.Add(output.ToString());
                    model.HasSuggestionAddress = true;

                    string messageTable = "<table class='EngineerAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                  "<td style='width:400px;'>Engineer Address you entered</td>" +
                                  "<td style='width:400px;'>Engineer Address we recommend</td>" +
                                  "</tr>" +
                                  "<tr>" +
                                  "<td style='width:400px'>" +
                                  model.EngineerAddress.AddressLine1 + " " +
                                  model.EngineerAddress.AddressLine2 + " " +
                                  model.EngineerAddress.Location + " " +
                                  GetStateByStateId(model.EngineerAddress.StateId.Value) + " " +
                                  model.EngineerAddress.PostalCode +
                                  "</td>" +
                                  "<td style='width:400px;'>" +
                                 output.ToString() +
                                  "</td>" +
                                   "<td>" +
                                   "<input type='checkbox' id='useSuggestionEngineerAddress' name='useSuggestionEngineerAddress' value='True'>Use Suggested Address</input>" +
                                   "</td>" +
                                  "</tr>" +
                                  "</table>";

                    this.Response.Messages.AddError(messageTable);
                }
            }

            if (addressResp.Error.Message != null && addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                string errorMessage = "<table class='EngineerAddress'><tr style='font-weight:bold;font-size:14px;'>" +
                                      "<td style='width:400px;'>" +
                                      "Engineer Address is invalid.Please provide a valid address." +
                                      "</td>" +
                                      "<td style='width:400px;'>" +
                                      "<input type='checkbox' id='useEngineerAddressAsIs' name='useEngineerAddressAsIs' value='True'>Check this box if you still want to use this Address</input>" +
                                      "</td>" +
                                      "</tr></table>";

                this.Response.AddError(errorMessage);
            }

           FinaliseModel(this.Response.Messages, model.CurrentUser, model);
            this.Response.Model = model;

            return this.Response;
        }

        //This function is to save kendo grid configuration to database (not yet implemented)
        public ServiceResponse SaveGridState(UserSessionModel user, object data)
        {
            return this.Response;
        }

        public long GetProjectId(UserSessionModel user, long quoteId)
        {
            long projectId = this.Db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.Quotes.Any(q => q.QuoteId == quoteId)).Select(p => p.ProjectId).FirstOrDefault();
            return projectId;
        }

        public string GetProjectNameById(long projectId)
        {
            string projectName = this.Db.Context.Projects.Where(p => p.ProjectId == projectId)
                                 .Select(p => p.Name).FirstOrDefault();

            return projectName;
        }

        #region Finalise Model

        public void FinaliseModel(Messages messages, UserSessionModel user, ProjectsModel model)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Finalise ProjectsModel"))
            {
                var hasCommission = user.HasAccess(SystemAccessEnum.RequestCommission) ||
                       user.HasAccess(SystemAccessEnum.ApprovedRequestCommission) ||
                       user.HasAccess(SystemAccessEnum.ViewRequestedCommission);

                var hasDiscountRequest = user.HasAccess(SystemAccessEnum.RequestDiscounts) ||
                    user.HasAccess(SystemAccessEnum.ApproveDiscounts) ||
                    user.HasAccess(SystemAccessEnum.ViewDiscountRequest);

                model.ProjectOpenStatusTypes = htmlService.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

                model.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, DropDownMode.Filtering);

                model.ProjectLeadStatusTypes = htmlService.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

                if (hasDiscountRequest || hasCommission)
                {
                    model.ProjectDarComTypes = htmlService.DropDownModelProjectDarComStatusTypes(user, (model == null) ? null : model.ProjectDarComStatusTypeId);
                }

                model.ProjectTypes = htmlService.DropDownModelProjectTypes(null);

                model.UsersInGroup = htmlService.DropDownModelUsersInGroup(user, (model == null) ? null : model.UserId);

                model.BusinessesInGroup = htmlService.DropDownModelBusineesForProjects(user, (model == null) ? null : model.BusinessId);

                model.ProjectExportTypes = htmlService.DropDownModelProjectExportTypes(null);

                model.ProjectDateTypes = htmlService.DropDownDateTypes(GetProjectDateTypes(), model.DateTypeId);
            }
        }


        public void FinaliseModel(Messages messages, UserSessionModel user, ProjectsGridViewModel model)
        {
            var profiler = MiniProfiler.Current;
            using (profiler.Step("Finalise ProjectsGridViewModel"))
            {
                var hasCommission = user.HasAccess(SystemAccessEnum.RequestCommission) ||
                       user.HasAccess(SystemAccessEnum.ApprovedRequestCommission) ||
                       user.HasAccess(SystemAccessEnum.ViewRequestedCommission);

                var hasDiscountRequest = user.HasAccess(SystemAccessEnum.RequestDiscounts) ||
                    user.HasAccess(SystemAccessEnum.ApproveDiscounts) ||
                    user.HasAccess(SystemAccessEnum.ViewDiscountRequest);

                model.ProjectOpenStatusTypes = htmlService.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);
                model.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, DropDownMode.Filtering);
                model.ProjectLeadStatusTypes = htmlService.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

                if (hasDiscountRequest || hasCommission)
                {
                    model.ProjectDarComTypes = htmlService.DropDownModelProjectDarComStatusTypes(user, (model == null) ? null : model.ProjectDarComStatusTypeId);
                }

                model.ProjectTypes = htmlService.DropDownModelProjectTypes(null);
                model.UsersInGroup = htmlService.DropDownModelUsersInGroup(user, (model == null) ? null : model.UserId);
                model.BusinessesInGroup = htmlService.DropDownModelBusineesForProjects(user, (model == null) ? null : model.BusinessId);
                model.ProjectExportTypes = htmlService.DropDownModelProjectExportTypes(null);
                model.ProjectDateTypes = htmlService.DropDownDateTypes(GetProjectDateTypes(), model.DateTypeId);
            }
        }

        public void FinaliseModel(Messages messages, UserSessionModel admin, ProjectModel model)
        {
            //mass upload change - had to turn these off
            var service = new HtmlServices(this.Context);

            new AddressServices(this.Context).FinaliseModel(model.CustomerAddress);
            new AddressServices(this.Context).FinaliseModel(model.EngineerAddress);
            new AddressServices(this.Context).FinaliseModel(model.SellerAddress);
            new AddressServices(this.Context).FinaliseModel(model.ShipToAddress);

            model.ConstructionTypes = service.DropDownModelConstructionTypes((model == null) ? null : model.ConstructionTypeId);

            model.ProjectLeadStatusTypes = service.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

            model.ProjectStatusTypes = service.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, this.DropDownMode);

            model.ProjectTypes = service.DropDownModelProjectTypes((model == null) ? null : model.ProjectTypeId);

            model.ProjectOpenStatusTypes = service.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

            model.VerticalMarketTypes = service.DropDownModelVerticalMarkets((model == null) ? null : model.VerticalMarketTypeId);
        }

        #endregion Finalise Model
    }
}
