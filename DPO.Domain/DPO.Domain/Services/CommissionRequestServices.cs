using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework;
using EntityFramework.Extensions;
using System.Web.Mvc;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using DPO.Resources;
using DPO.Common.Models;

namespace DPO.Domain
{
    public partial class CommissionRequestServices : BaseServices
    {
        private HtmlServices htmlService;
        private QuoteServices quoteService;

        public CommissionRequestServices()
            : base()
        {
            htmlService = new HtmlServices(this.Context);
            quoteService = new QuoteServices(this.Context);
        }

        public CommissionRequestServices(DPOContext context)
            : base(context)
        {
            htmlService = new HtmlServices(context);
            quoteService = new QuoteServices(context);
        }

        #region Get Requests

        public ServiceResponse GetCommissionRequestListModel(UserSessionModel user, SearchCommissionRequests search)
        {

            search.ReturnTotals = true;

            var query = from commission in this.Db.QueryCommissionRequestsViewableBySearch(user, search)
                        join quote in this.Db.Quotes on commission.QuoteId equals quote.QuoteId
                        join owner in this.Db.Users on quote.Project.OwnerId equals owner.UserId
                        join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                        select new CommissionRequestModel
                        {
                            CommissionRequestId = commission.CommissionRequestId,
                            ProjectOwner = owner.FirstName + " " + owner.LastName,
                            ProjectOwnerId = owner.UserId,
                            BusinessId = owner.BusinessId,
                            BusinessName = business.BusinessName,
                            ProjectId = commission.ProjectId,
                            QuoteId = commission.QuoteId,
                            Project = new ProjectModel
                            {
                                Name = quote.Project.Name,
                                ActiveQuoteSummary = new QuoteListModel { Title = quote.Title }
                            },

                            RequestedOn = commission.RequestedOn,
                            CommissionRequestStatusTypeId = commission.CommissionRequestStatusTypeId,
                            CommissionRequestStatusTypeDescription = commission.CommissionRequestStatusType.Description,
                            Timestamp = commission.Timestamp
                        };

            this.Response.Model = query.ToList();
            return this.Response;

        }

        public bool ValidateEmails(List<string> emails)
        {
            bool result = this.Db.ValidateEmails(emails);
            return result;
        }

        public List<String> GetInvalidEmails(List<string> emails)
        {
            List<string> InvalidEmails = this.Db.GetInvalidEmails(emails);
            return InvalidEmails;
        }

        public CommissionRequestSendEmailModel GetCommissionRequestSendEmailModel(CommissionRequestModel req)
        {
            Project proj = this.Db.GetProjectOwnerAndBusiness(req.ProjectId);
            Business business = this.Db.GetBusinessByProjectOwner(req.ProjectId);

            req.ProjectOwner = proj.Owner.FirstName + " " + proj.Owner.LastName;
            req.BusinessName = business.BusinessName;

            return new CommissionRequestSendEmailModel
            {
                commissionRequest = req,
                AccountManagerEmail = business.AccountManagerEmail,
                AccountOwnerEmail = business.AccountOwnerEmail
            };
        }

        public MemoryStream GetCommissionRequestExportExcelFile(CommissionRequestModel model, bool showCostPricing)
        {
            var stream = new MemoryStream();

            var workbook = new HSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = model.BusinessName;
            workbook.DocumentSummaryInformation = dsi;

            //create a entry of SummaryInformation
            var si = PropertySetFactory.CreateSummaryInformation();

            si.Subject = string.Format("Project '{0}' , Quote '{1}' report ", (model.Project.Name != null) ? model.Project.Name : ResourceUI.NotGiven,
                (model.Quote.Title != null) ? model.Quote.Title : ResourceUI.NotGiven);

            workbook.SummaryInformation = si;

            var worksheet = workbook.CreateSheet(model.Quote.Title);

            var ch = new HSSFCreationHelper(workbook);
            var fe = ch.CreateFormulaEvaluator();

            int row = 0;

            //header information
            var r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Status:", (model.CommissionRequestStatusTypeDescription != null) ? model.CommissionRequestStatusTypeDescription : ResourceUI.NotGiven);

            if (model.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved)
            {
                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Approved by:", (model.CommissionRequestStatusModifiedBy != null) ? model.CommissionRequestStatusModifiedBy : ResourceUI.NotGiven);
                SetLabelValue(worksheet, r, 2, 3, "Approved on:", (model.CommissionRequestStatusModifiedOn != null) ? model.CommissionRequestStatusModifiedOn : Convert.ToDateTime("01/01/1000"));
            }

            //project details

            row++;
            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Project Name:", (model.Project.Name != null) ? model.Project.Name : ResourceUI.NotGiven);
            SetLabelValue(worksheet, r, 3, 4, "Construction Type:", (model.Project.ConstructionTypeDescription != null) ? model.Project.ConstructionTypeDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Project Reference:", (model.Project.ProjectId != null) ? model.Project.ProjectId : 0);
            SetLabelValue(worksheet, r, 3, 4, "Project Status:", (model.Project.ProjectStatusDescription != null) ? model.Project.ProjectStatusDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Registration Date:", (model.Project.ProjectDate != null) ? model.Project.ProjectDate : Convert.ToDateTime("01/01/1000"));
            SetLabelValue(worksheet, r, 3, 4, "Project Type:", (model.Project.ProjectTypeDescription != null) ? model.Project.ProjectTypeDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Bid Date:", (model.Project.BidDate != null) ? model.Project.BidDate : Convert.ToDateTime("01/01/1000"));
            SetLabelValue(worksheet, r, 3, 4, "Project Open Status:", (model.Project.ProjectOpenStatusDescription != null) ? model.Project.ProjectOpenStatusDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Estimated Close:", (model.Project.EstimatedClose != null) ? model.Project.EstimatedClose : Convert.ToDateTime("01/01/1000"));
            SetLabelValue(worksheet, r, 3, 4, "Vertical Market:", (model.Project.VerticalMarketDescription != null) ? model.Project.VerticalMarketDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Estimated Delivery:", (model.Project.EstimatedDelivery != null) ? model.Project.EstimatedDelivery : Convert.ToDateTime("01/01/1000"));
            SetLabelValue(worksheet, r, 3, 4, "Project Notes:", (model.Project.Description != null) ? model.Project.Description : ResourceUI.NotGiven);


            //addresses
            row++;
            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, "Engineer Details:", true);
            SetCellValue(worksheet, r, 1, "Dealer/Contractor Address:", true);
            SetCellValue(worksheet, r, 2, "Seller Address:", true);
            SetCellValue(worksheet, r, 3, "Ship To Address:", true);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, (model.Project.EngineerAddress.AddressLine1 != null) ? model.Project.EngineerAddress.AddressLine1 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 1, (model.Project.CustomerAddress.AddressLine1 != null) ? model.Project.CustomerAddress.AddressLine1 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 2, (model.Project.SellerAddress.AddressLine1 != null) ? model.Project.SellerAddress.AddressLine1 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 3, (model.Project.ShipToAddress.AddressLine1 != null) ? model.Project.ShipToAddress.AddressLine1 : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, (model.Project.EngineerAddress.AddressLine2 != null) ? model.Project.EngineerAddress.AddressLine2 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 1, (model.Project.CustomerAddress.AddressLine2 != null) ? model.Project.CustomerAddress.AddressLine2 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 2, (model.Project.SellerAddress.AddressLine2 != null) ? model.Project.SellerAddress.AddressLine2 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 3, (model.Project.ShipToAddress.AddressLine2 != null) ? model.Project.ShipToAddress.AddressLine2 : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, (model.Project.EngineerAddress.AddressLine3 != null) ? model.Project.EngineerAddress.AddressLine3 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 1, (model.Project.CustomerAddress.AddressLine3 != null) ? model.Project.CustomerAddress.AddressLine3 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 2, (model.Project.SellerAddress.AddressLine3 != null) ? model.Project.SellerAddress.AddressLine3 : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 3, (model.Project.ShipToAddress.AddressLine3 != null) ? model.Project.ShipToAddress.AddressLine3 : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, (model.Project.EngineerAddress.Location != null) ? model.Project.EngineerAddress.Location : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 1, (model.Project.CustomerAddress.Location != null) ? model.Project.CustomerAddress.Location : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 2, (model.Project.SellerAddress.Location != null) ? model.Project.SellerAddress.Location : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 3, (model.Project.ShipToAddress.Location != null) ? model.Project.ShipToAddress.Location : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, (model.Project.EngineerAddress.PostalCode != null) ? model.Project.EngineerAddress.PostalCode : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 1, (model.Project.CustomerAddress.PostalCode != null) ? model.Project.CustomerAddress.PostalCode : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 2, (model.Project.SellerAddress.PostalCode != null) ? model.Project.SellerAddress.PostalCode : ResourceUI.NotGiven);
            SetCellValue(worksheet, r, 3, (model.Project.ShipToAddress.PostalCode != null) ? model.Project.ShipToAddress.PostalCode : ResourceUI.NotGiven);

            //quote details
            row++;

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Quote Name:", (model.Quote.Title != null) ? model.Quote.Title : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Products in Quote:", (!string.IsNullOrEmpty(model.QuoteItems.Count.ToString())) ? 
                          model.QuoteItems.Count : 0);

            if (model.QuoteItems.Count > 0)
            {
                SetLabelValue(worksheet, r, 0, 1, "Product Number", "Quantity");

                for (var i = 0; i < model.QuoteItems.Count; i++)
                {
                    r = worksheet.CreateRow(row++);

                    SetCellValue(worksheet, r, 0, model.QuoteItems[i].ProductNumber.ToString());
                    SetCellValue(worksheet, r, 1, model.QuoteItems[i].Quantity.ToString());
                }
            }

            //actual COM stuff
            row++;
            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, "Project systems and competitive position of opportunity".ToUpper(), true);

            row++;
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Project System Basis Of Design", (model.SystemBasisDesignTypeDescription != null) ? model.SystemBasisDesignTypeDescription : ResourceUI.NotGiven);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Brand Specified:", (model.BrandApprovedTypeDescription != null) ? model.BrandApprovedTypeDescription : ResourceUI.NotGiven);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Zone Strategy:", (model.ZoneStrategyTypeDescription != null) ? model.ZoneStrategyTypeDescription : ResourceUI.NotGiven);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Approved Equals:", (model.BrandSpecifiedTypeDescription != null) ? model.BrandSpecifiedTypeDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Copy of competitors price to customer available", 
                  ( !string.IsNullOrEmpty(model.HasCompetitorPrice.ToString())) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Copy of competitors quote to customer available", 
                (!string.IsNullOrEmpty(model.HasCompetitorQuote.ToString())) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Line by line comparison of competitor to Daikin completed", 
                (!string.IsNullOrEmpty(model.HasCompetitorLineComparsion.ToString())) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Is Daikin equipment at an advantage:",
                (model.DaikinEquipmentAtAdvantageDescription != null) ? model.DaikinEquipmentAtAdvantageDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Rep/distributor confident that competition offer is equal to this offer", 
                (!string.IsNullOrEmpty(model.IsConfidentCompetitorQuote.ToString())) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitor price:", (model.CompetitorPrice != null) ? string.Format("{0:C}", 
                         model.CompetitorPrice) : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitor quote attached:", (model.CompetitorQuoteFileName != null) ? 
                          ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitors line By line comparison file attached:", 
                         (model.CompetitorLineComparsionFileName != null) ? ResourceUI.Yes : ResourceUI.No);

            if (showCostPricing == true)
            {
                row++;

                r = worksheet.CreateRow(row++);
                SetCellValue(worksheet, r, 0, "Rep/distributor and Daikin information and costing for opportunity".ToUpper(), true);
                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total net price based on standard multipliers", "");

                row++;

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total Listed Value Of This Project Offering",
                    (!string.IsNullOrEmpty(model.StandardTotals.TotalList.ToString())) ? 
                      string.Format("{0:C}", model.StandardTotals.TotalList) : ResourceUI.NotGiven);

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total Net", 
                    (!string.IsNullOrEmpty(model.StandardTotals.TotalNet.ToString())) ? 
                      string.Format("{0:C}", model.StandardTotals.TotalNet) : ResourceUI.NotGiven);

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Freight Costs", 
                    (!string.IsNullOrEmpty(model.Quote.TotalFreight.ToString())) ? 
                      string.Format("{0:C}", model.Quote.TotalFreight) : ResourceUI.NotGiven);

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Start up / Commissioning costs", 
                    (!string.IsNullOrEmpty(model.StartUpCosts.ToString())) ? 
                      model.StartUpCosts.ToString("0.00") : ResourceUI.NotGiven);

                if ( !string.IsNullOrEmpty(model.StartUpCosts.ToString()) && 
                     !string.IsNullOrEmpty(model.StandardTotals.TotalCommissionAmount.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Rep/Distributor Gross Profit on Opportunity", 
                                  model.StartUpCosts.ToString("0.00") + "/" + string.Format("{0:C}", 
                                  model.StandardTotals.TotalCommissionAmount));
                }

                if (!string.IsNullOrEmpty(model.StandardTotals.TotalSell.ToString()) && 
                    !string.IsNullOrEmpty(model.StartUpCosts.ToString()) && 
                    !string.IsNullOrEmpty(model.Quote.TotalFreight.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Total standard sales value of this opportunity from Rep/Dist to customer", 
                                  string.Format("{0:C}", model.StandardTotals.TotalSell + model.StartUpCosts + model.Quote.TotalFreight));
                }

                row++;

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Opportunity commisison amount requested", "");

                if (!string.IsNullOrEmpty(model.TotalNet.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Total Net", string.Format("{0:C}", model.TotalNet));
                }

                if (!string.IsNullOrEmpty(model.RequestedCommissionPercentage.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Requested Commission Percentage", string.Format("{0}%", 
                                  Math.Round(model.RequestedCommissionPercentage, 2)));
                }

                if (!string.IsNullOrEmpty(model.ApprovedCommissionPercentage.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Approved Commission Percentage", 
                                  string.Format("{0}%", Math.Round(model.ApprovedCommissionPercentage, 2)));
                }

                if (!string.IsNullOrEmpty(model.RequestedCommissionTotal.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Commission being requested for this opportunity",
                   (!string.IsNullOrEmpty(model.RequestedCommissionTotal.ToString())) ? 
                     string.Format("{0:C}", Math.Round(model.RequestedCommissionTotal, 2)) : ResourceUI.NotGiven);
                }

                if (!string.IsNullOrEmpty(model.ApprovedCommissionTotal.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Commission being approved for this opportunity",
                   (!string.IsNullOrEmpty(model.ApprovedCommissionTotal.ToString())) ? 
                     string.Format("{0:C}", Math.Round(model.ApprovedCommissionTotal, 2)) : ResourceUI.NotGiven);
                }

                if (!string.IsNullOrEmpty(model.ApprovedMultiplier.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Net Multiplier",
                    (!string.IsNullOrEmpty(model.ApprovedMultiplier.ToString())) ? 
                      model.ApprovedMultiplier.ToString("0.0000") : model.RequestedMultiplier.ToString("0.0000"));
                }

                if (!string.IsNullOrEmpty(model.RequestedNetMaterialValueMultiplier.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Net Material Value Multiplier", 
                                  string.Format("{0:C}", model.RequestedNetMaterialValueMultiplier));
                }

                if (!string.IsNullOrEmpty(model.RequestedNetMaterialValue.ToString()))
                {
                    r = worksheet.CreateRow(row++);
                    SetLabelValue(worksheet, r, 0, 1, "Net Material Value", model.RequestedNetMaterialValue);
                }

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Revised Gross", 
                    (!string.IsNullOrEmpty(model.ApprovedCommissionTotal.ToString())) ? 
                      string.Format("{0:C}", model.ApprovedCommissionTotal) : string.Format("{0:C}", 0.00));
            }

            row++;

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Probability of close:", 
                (model.ProbabilityOfCloseTypeDescription != null) ? model.ProbabilityOfCloseTypeDescription : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "If approved, order will be issued to Daikin on:", 
                (model.OrderPlannedFor != null) ? model.OrderPlannedFor : Convert.ToDateTime("01/01/1000"));

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Approximate delivery date for required equipment:", 
                (model.OrderDeliveryDate != null) ? model.OrderDeliveryDate : Convert.ToDateTime("01/01/1000"));

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Any further information or reason for discount request:", 
                (model.Notes != null) ? model.Notes : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Response from approval team", 
                (model.ResponseNotes != null) ? model.ResponseNotes : ResourceUI.NotGiven);

            workbook.Write(stream);

            return stream;
        }

        public ServiceResponse GetCommissionRequestModel(UserSessionModel admin, CommissionRequestModel req, CommissionCalculationModel calculationModel = null)
        {
            CommissionRequestModel model = null;

            if (req.CommissionRequestId.HasValue)
            {
                var query = from commission in this.Db.QueryCommissionRequestsViewable(admin)
                            join mod in this.Db.Users on commission.StatusTypeModifiedBy equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()

                            join userTypes in this.Db.UserTypes on mod.UserTypeId equals userTypes.UserTypeId

                            join project in this.Db.Projects on commission.ProjectId equals project.ProjectId

                            join quote in this.Db.Quotes on commission.QuoteId equals quote.QuoteId

                            join owner in this.Db.Users on project.OwnerId equals owner.UserId

                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId

                            where commission.CommissionRequestId == req.CommissionRequestId.Value

                            select new CommissionRequestModel
                            {
                                CommissionRequestId = commission.CommissionRequestId,

                                ProjectId = commission.ProjectId,

                                QuoteId = commission.QuoteId,

                                RequestedOn = commission.RequestedOn,

                                ProjectOwner = owner.FirstName + " " + owner.LastName,

                                ProjectOwnerId = owner.UserId,

                                BusinessId = owner.BusinessId,

                                BusinessName = business.BusinessName,

                                CommissionRequestStatusTypeId = (byte?)commission.CommissionRequestStatusTypeId,
                                CommissionRequestStatusTypeDescription = commission.CommissionRequestStatusType.Description,

                                CommissionRequestStatusModifiedOn = commission.StatusTypeModifiedOn,
                                CommissionRequestStatusModifiedBy = mod.FirstName + " " + mod.LastName,

                                SystemBasisDesignTypeId = (byte?)commission.SystemBasisDesignTypeId,
                                SystemBasisDesignTypeDescription = commission.SystemBasisDesignType.Description,

                                ZoneStrategyTypeId = (byte?)commission.ZoneStrategyTypeId,
                                ZoneStrategyTypeDescription = commission.ZoneStrategyType.Description,

                                BrandSpecifiedTypeId = (byte?)commission.BrandSpecifiedTypeId,
                                BrandSpecifiedTypeDescription = commission.BrandSpecifiedType.Description,

                                BrandApprovedTypeId = (byte?)commission.BrandApprovedTypeId,
                                BrandApprovedTypeDescription = commission.BrandApprovedType.Description,

                                DaikinEquipmentAtAdvantageTypeId = (byte?)commission.DaikinEquipmentAtAdvantageTypeId,

                                ProbabilityOfCloseTypeId = commission.ProbabilityOfCloseTypeId,

                                HasCompetitorPrice = commission.HasCompetitorPrice,

                                CompetitorPrice = commission.CompetitorPrice,

                                HasCompetitorQuote = commission.HasCompetitorQuote,

                                CompetitorQuoteFileName = commission.CompetitorQuoteFileName,

                                StartUpCosts = commission.StartUpCosts,
                                ThirdPartyEquipmentCosts = (commission.ThirdPartyEquipmentCosts.HasValue) ? commission.ThirdPartyEquipmentCosts.Value : 0,

                                RequestedMultiplier = (commission.RequestedMultiplier == null) ? 0 : commission.RequestedMultiplier.Value,
                                ApprovedMultiplier = (commission.ApprovedMultiplier == null) ? 0 : commission.ApprovedMultiplier.Value,

                                RequestedCommissionPercentageVRV = commission.RequestedCommissionPercentVRV.HasValue ?
                                                                   commission.RequestedCommissionPercentVRV.Value : 0,

                                ApprovedCommissionPercentageVRV = (commission.ApprovedCommissionPercentVRV == null) ? 0 :
                                                                   commission.ApprovedCommissionPercentVRV.Value,

                                RequestedMultiplierVRV = (commission.RequestedMultiplierVRV == null) ? 0 :
                                                          commission.RequestedMultiplierVRV.Value,

                                ApprovedMultiplierVRV = (commission.ApprovedMultiplierVRV.HasValue) ?
                                                          commission.ApprovedMultiplierVRV.Value : 0,

                                RequestedCommissionVRV = (commission.RequestedCommissionVRV.HasValue) ?
                                                          commission.RequestedCommissionVRV.Value : 0,

                                ApprovedCommissionVRV = (commission.RequestedCommissionPercentVRV.HasValue ) ?
                                                        commission.RequestedCommissionPercentVRV.Value : 0,

                                RequestedNetMaterialMultiplierVRV = (commission.RequestedNetMaterialMultiplierVRV.HasValue) ?
                                                                     commission.RequestedNetMaterialMultiplierVRV.Value : 0,

                                RequestedNetMaterialValueVRV = (commission.RequestedNetMultiplierValueVRV.HasValue) ?
                                                                 commission.RequestedNetMultiplierValueVRV.Value : 0,

                                RequestedCommissionSplit = (commission.RequestedCommissionSplit.HasValue) ?
                                                            commission.RequestedCommissionSplit.Value : 0,

                                RequestedCommissionPercentageSplit = commission.RequestedCommissionPercentSplit.HasValue ?
                                                                     commission.RequestedCommissionPercentSplit.Value : 0,

                                ApprovedCommissionPercentageSplit = (commission.ApprovedCommissionPercentSplit.HasValue) ?
                                                                     commission.ApprovedCommissionPercentSplit.Value : 0,

                                ApprovedCommissionSplit = (commission.RequestedCommissionSplit.HasValue) ? 
                                                           commission.RequestedCommissionSplit.Value : 0,

                                RequestedMultiplierSplit = (commission.RequestedMultiplierSplit == null) ? 0 :
                                                            commission.RequestedMultiplierSplit.Value,

                                ApprovedMultiplierSplit = (commission.ApprovedMultiplierSplit.HasValue) ?
                                                           commission.ApprovedMultiplierSplit.Value : 0,

                                RequestedNetMaterialMultiplierSplit = (commission.RequestedNetMaterialMultiplierSplit.HasValue) ?
                                                                       commission.RequestedNetMaterialMultiplierSplit.Value : 0,

                                RequestedNetMaterialValueSplit = (commission.RequestedNetMultiplierValueSplit.HasValue) ?
                                                                  commission.RequestedNetMultiplierValueSplit.Value : 0,

                                RequestedCommissionUnitary = (commission.RequestedCommissionUnitary.HasValue) ?
                                                              commission.RequestedCommissionUnitary.Value : 0,

                                RequestedCommissionPercentageUnitary = commission.RequestedCommissionPercentUnitary.HasValue ?
                                                                       commission.RequestedCommissionPercentUnitary.Value : 0,

                                ApprovedCommissionPercentageUnitary = commission.ApprovedCommissionPercentUnitary.HasValue ?
                                                                      commission.ApprovedCommissionPercentUnitary.Value : 0,

                                ApprovedCommissionUnitary = (commission.RequestedCommissionUnitary.HasValue ) ?
                                                             commission.RequestedCommissionUnitary.Value :0,
                                                             
                                RequestedMultiplierUnitary = (commission.RequestedMultiplierUnitary == null) ? 0 :
                                                            commission.RequestedMultiplierUnitary.Value,

                                ApprovedMultiplierUnitary = (commission.ApprovedMultiplierUnitary.HasValue) ?
                                                           commission.ApprovedMultiplierUnitary.Value : 0,

                                RequestedNetMaterialMultiplierUnitary = (commission.RequestedNetMaterialMultiplierUnitary.HasValue) ?
                                                                       commission.RequestedNetMaterialMultiplierUnitary.Value : 0,

                                RequestedNetMaterialValueUnitary = (commission.RequestedNetMultiplierValueUnitary.HasValue) ?
                                                                  commission.RequestedNetMultiplierValueUnitary.Value : 0,

                                // LC Package
                                RequestedCommissionLCPackage = (commission.RequestedCommissionLCPackage.HasValue) ?
                                                              commission.RequestedCommissionLCPackage.Value : 0,

                                RequestedCommissionPercentageLCPackage = commission.RequestedCommissionPercentLCPackage.HasValue ?
                                                                       commission.RequestedCommissionPercentLCPackage.Value : 0,

                                ApprovedCommissionPercentageLCPackage = commission.ApprovedCommissionPercentLCPackage.HasValue ?
                                                                      commission.ApprovedCommissionPercentLCPackage.Value : 0,

                                ApprovedCommissionLCPackage = (commission.RequestedCommissionLCPackage.HasValue) ?
                                                             commission.RequestedCommissionLCPackage.Value : 0,

                                RequestedMultiplierLCPackage = (commission.RequestedMultiplierLCPackage == null) ? 0 :
                                                            commission.RequestedMultiplierLCPackage.Value,

                                ApprovedMultiplierLCPackage = (commission.ApprovedMultiplierLCPackage.HasValue) ?
                                                           commission.ApprovedMultiplierLCPackage.Value : 0,

                                RequestedNetMaterialMultiplierLCPackage = (commission.RequestedNetMaterialMultiplierLCPackage.HasValue) ?
                                                                       commission.RequestedNetMaterialMultiplierLCPackage.Value : 0,

                                RequestedNetMaterialValueLCPackage = (commission.RequestedNetMultiplierValueLCPackage.HasValue) ?
                                                                  commission.RequestedNetMultiplierValueLCPackage.Value : 0,
                                //=====

                                RequestedCommissionPercentage = (commission.RequestedCommissionPercent.HasValue) ?
                                                                 commission.RequestedCommissionPercent.Value : 0,

                                ApprovedCommissionPercentage = (commission.ApprovedCommissionPercent.HasValue) ?
                                                                commission.ApprovedCommissionPercent.Value : 0,

                                RequestedNetMaterialValueMultiplier = (commission.RequestedNetMaterialValueMultiplier.HasValue) ?
                                                                       commission.RequestedNetMaterialValueMultiplier.Value : 0,

                                RequestedNetMaterialValue = (commission.RequestedNetMaterialValue.HasValue) ?
                                                             commission.RequestedNetMaterialValue.Value : 0,

                                TotalNetSplit = (!string.IsNullOrEmpty(quote.TotalNetSplit.ToString()) && quote.TotalNetSplit > 0) ?
                                                 quote.TotalNetSplit :
                                                (commission.TotalNetSplit.HasValue) ? commission.TotalNetSplit.Value : 0,

                                TotalNetVRV = (!string.IsNullOrEmpty(quote.TotalNetVRV.ToString()) && quote.TotalNetVRV > 0) ?
                                                 quote.TotalNetVRV :
                                                (commission.TotalNetVRV.HasValue) ? commission.TotalNetVRV.Value : 0,

                                TotalNetUnitary = (quote.TotalNetUnitary.HasValue && quote.TotalNetUnitary.Value > 0) ?
                                                   quote.TotalNetUnitary.Value :
                                                 (commission.TotalNetUnitary.HasValue) ? commission.TotalNetUnitary.Value : 0,
                                TotalNetLCPackage = (quote.TotalNetLCPackage.HasValue && quote.TotalNetLCPackage.Value > 0) ?
                                                   quote.TotalNetLCPackage.Value :
                                                 (commission.TotalNetLCPackage.HasValue) ? commission.TotalNetLCPackage.Value : 0,

                                TotalNet = (!string.IsNullOrEmpty(quote.TotalNet.ToString()) && quote.TotalNet > 0) ?
                                                quote.TotalNet :
                                                (commission.TotalNet.HasValue) ? commission.TotalNet.Value : 0,

                                TotalRevised = (commission.TotalRevised.HasValue) ? commission.TotalRevised.Value : 0,

                                RequestedCommissionTotal = (commission.RequestedCommissionTotal.HasValue) ?
                                                            commission.RequestedCommissionTotal.Value : 0,
                                ApprovedCommissionTotal = (commission.ApprovedCommissionTotal.HasValue) ?
                                                           commission.ApprovedCommissionTotal.Value : 0,

                                CompetitorLineComparsionFileName = commission.CompetitorLineComparsionFileName,

                                HasCompetitorLineComparsion = commission.HasCompetitorLineComparsion,

                                IsConfidentCompetitorQuote = commission.IsConfidentCompetitorQuote,

                                OrderPlannedFor = commission.OrderPlannedFor,

                                OrderDeliveryDate = commission.OrderDeliveryDate,

                                Notes = commission.Notes,

                                ResponseNotes = commission.ResponseNotes,

                                Timestamp = commission.Timestamp,

                                ShouldSendEmail = req.ShouldSendEmail,

                                EmailsList = commission.EmailsList,

                                WinLossConditionTypeId = (byte?)commission.WinLossTypeId,

                                FundingTypeId = (byte?)commission.FundingTypeId,
                                                 
                                CustomerTypeId = (byte?)commission.CustomerTypeId,
                                            
                                RepPhoneNumber = commission.RepPhoneNumber,

                                SellerPhoneNumber = commission.SellerPhoneNumber,

                                RepEmail = commission.RepEmail,

                                SellerEmail = commission.SellerEmail,

                                RepFaxNumber = commission.RepFaxNumber,

                                SellerFaxNumber = commission.SellerFaxNumber,

                                IsCommissionCalculation =  (commission.IsCommissionCalculation.HasValue ) ? 
                                                           commission.IsCommissionCalculation.Value: false

                            };

                model = query.OrderByDescending(m => m.CommissionRequestId).First();
            }

            if (model == null)
            {
                model = new CommissionRequestModel
                {
                    CommissionRequestStatusTypeId = (byte)CommissionRequestStatusTypeEnum.NewRecord,
                    ProjectId = req.ProjectId,
                    QuoteId = req.QuoteId
                };
            }

            FinaliseModel(this.Response.Messages, admin, model, calculationModel);

            this.Response.Model = model;

            return this.Response;
        }

        public List<decimal> GetCommissionRequestPercentage(decimal? vrvCommissionRequestMultiplier, decimal? splitCommissionRequestMultiplier)
        {
            var vrvComPercentage = this.Db.GetCommissionMultipliers()
                                  .Where(cm => cm.Multiplier == vrvCommissionRequestMultiplier)
                                  .Select(cm => cm.CommissionPercentage).FirstOrDefault();

            var splitComPercentage = this.Db.GetCommissionMultipliers()
                                     .Where(cm => cm.Multiplier == splitCommissionRequestMultiplier)
                                     .Select(cm => cm.CommissionPercentage).FirstOrDefault();

            List<decimal> CommissionPercentages = new List<decimal>();
            CommissionPercentages.Add(vrvComPercentage);
            CommissionPercentages.Add(splitComPercentage);

            return CommissionPercentages;
        }


        public ServiceResponse GetCommissionMultipliers(UserSessionModel user, CommissionMultipliersModel model)
        {
            var qry = from cm in this.Db.QueryCommissionMultipliersViewableBySearch(user, model)
                      select new CommissionMultiplierListModel
                      {
                          CommissionMultiplierId = cm.CommissionMultiplierId,
                          CommissionPercentage = cm.CommissionPercentage,
                          Multiplier = cm.Multiplier,
                          MultiplierCategoryType = cm.MultiplierCategoryTypeId
                      };

            model.Items = new PagedList<CommissionMultiplierListModel>(qry.ToList());
            this.Response.Model = model;
            return this.Response;
        }

        public ServiceResponse GetUnitaryMultiplier(UserSessionModel user, CommissionRequestModel model)
        {
            if (model == null)
            {
                this.Response.AddError("Please specify search criteria");
                return this.Response;
            }

            var averageUnitaryCommissionPercentage = UpdateCommissionPercentageForUnitaryGroups(user, model);

            var qry = from cm in this.Context.UnitaryCommissionCurves
                      select new CommissionMultiplierModel
                      {
                          CommissionMultiplierId = cm.CommissionCurveId,
                          CommissionPercentage = averageUnitaryCommissionPercentage,
                          Multiplier = cm.Multiplier,
                          MultiplierCategoryType = (MultiplierCategoryTypeEnum)cm.MultiplierCategoryTypeId
                      };

            var foundModel = qry.FirstOrDefault();
            if (foundModel == null)
            {
                foundModel = new CommissionMultiplierModel
                {
                    CommissionPercentage = 0,
                    Multiplier = model.RequestedMultiplier
                };
            }

            this.Response.Model = foundModel;
            return this.Response;
        }

        public decimal UpdateCommissionPercentageForUnitaryGroups(UserSessionModel user, CommissionRequestModel model)
        {
            ServiceResponse resp;
            if (model.QuoteItems == null)
            {
                resp = quoteService.GetQuoteItemListModel(user, model.QuoteId.Value);

                model.QuoteItems = resp.Model as List<QuoteItemListModel>;
            }

            //List<string> CN_Group = new List<string> { "CN", "HP", "FN", "PC", "PG", "PH", "LC", "LG", "LH" };
            List<string> CN_Group = new List<string> { "CN", "HP", "FN", "PC", "PG", "PH"};
            List<string> AH_Group = new List<string> { "AH", "CL" };

            decimal totalListPrice_AH_Group  = 0;
            decimal totalListPrice_CN_Group  = 0;
            decimal totalCommission_AH_Group = 0;
            decimal totalCommission_CN_Group = 0;
            decimal totalNetPrice_AH_Group   = 0;
            decimal totalNetPrice_CN_Group   = 0;

            decimal averagUnitaryCommissionPercentage = 0;

            
            foreach (QuoteItemListModel quoteItem in model.QuoteItems)
            {
                if(CN_Group.Contains(quoteItem.ProductClassCode))
                {
                    var comissionPercentage = this.Db.Context.UnitaryCommissionCurves
                                                  .Where(ucc => ucc.Multiplier == model.RequestedMultiplierUnitary)
                                                  .Select(ucc => ucc.CommissionPercentage_CN).FirstOrDefault();
                    totalListPrice_CN_Group = totalListPrice_CN_Group + quoteItem.PriceList.Value * quoteItem.Quantity;
                    totalNetPrice_CN_Group = totalListPrice_CN_Group * model.RequestedMultiplierUnitary;

                    totalCommission_CN_Group = totalNetPrice_CN_Group * (comissionPercentage/100);            
                }

                if(AH_Group.Contains(quoteItem.ProductClassCode))
                {
                    var comissionPercentage = this.Db.Context.UnitaryCommissionCurves
                                                  .Where(ucc => ucc.Multiplier == model.RequestedMultiplierUnitary)
                                                  .Select(ucc => ucc.CommissionPercentage_AH).FirstOrDefault();
                    totalListPrice_AH_Group = totalListPrice_AH_Group + quoteItem.PriceList.Value * quoteItem.Quantity;

                    totalNetPrice_AH_Group = totalListPrice_AH_Group * model.RequestedMultiplierUnitary;

                    totalCommission_AH_Group = totalNetPrice_AH_Group * (comissionPercentage/100);
                }
            }
            if (totalNetPrice_AH_Group != 0 || totalNetPrice_CN_Group != 0)
            {
                averagUnitaryCommissionPercentage = (totalCommission_AH_Group + totalCommission_CN_Group) / (totalNetPrice_AH_Group + totalNetPrice_CN_Group); 
            }

            return (averagUnitaryCommissionPercentage*100);
        }
        public ServiceResponse GetCommissionMultiplier(UserSessionModel user, CommissionMultipliersModel model)
        {
            if (model == null)
            {
                this.Response.AddError("Please specify search criteria");

                return this.Response;
            }

            var qry = from cm in this.Db.QueryCommissionMultipliersViewableBySearch(user, model)
                      select new CommissionMultiplierModel
                      {
                          CommissionMultiplierId = cm.CommissionMultiplierId,
                          CommissionPercentage = cm.CommissionPercentage,
                          Multiplier = cm.Multiplier,
                          MultiplierCategoryType = cm.MultiplierCategoryTypeId
                      };

            var foundModel = qry.FirstOrDefault();
            if (foundModel == null)
            {
                foundModel = new CommissionMultiplierModel
                {
                    CommissionPercentage = 0,
                    Multiplier = model.Multiplier
                };
            }

            this.Response.Model = foundModel;

            return this.Response;
        }

        public ServiceResponse GetCommissionPercentage(UserSessionModel user, CommissionMultiplierModelV2 model)
        {
            var query = from commissionMultiplier in this.Db.CommissionMultipliers
                        where commissionMultiplier.MultiplierCategoryTypeId == model.MultiplierCategoryTypeId
                            && commissionMultiplier.Multiplier == model.Multiplier
                        select new CommissionMultiplierModelV2
                        {
                            MultiplierCategoryTypeId = commissionMultiplier.MultiplierCategoryTypeId,
                            Multiplier = commissionMultiplier.Multiplier,
                            CommissionPercentage = commissionMultiplier.CommissionPercentage,
                        };
            CommissionMultiplierModelV2 result = query.FirstOrDefault();

            //return CommissionPercentage = 0 if not found
            if (result == null) {
                result = new CommissionMultiplierModelV2
                {
                    MultiplierCategoryTypeId = model.MultiplierCategoryTypeId,
                    Multiplier = model.Multiplier,
                    CommissionPercentage = 0
                };
            }

            this.Response.Model = result;

            return this.Response;
        }

        /// <summary>
        /// TODO:  This should return ServiceResponse
        /// </summary>
        /// <param name="vrvCommissionRequestPercentage"></param>
        /// <param name="splitCommissionRequestPercentage"></param>
        /// <returns></returns>
        public List<decimal> GetCommissionRequestMultiplier(decimal? vrvCommissionRequestPercentage, decimal? splitCommissionRequestPercentage)
        {
            var vrvComMultiplier = this.Db.GetCommissionMultipliers()
                                  .Where(cm => cm.CommissionPercentage == vrvCommissionRequestPercentage)
                                  .Select(cm => cm.Multiplier).FirstOrDefault();

            var splitComMultiplier = this.Db.GetCommissionMultipliers()
                                     .Where(cm => cm.CommissionPercentage == splitCommissionRequestPercentage)
                                     .Select(cm => cm.Multiplier).FirstOrDefault();

            List<decimal> CommissionMultipliers = new List<decimal>();
            CommissionMultipliers.Add(vrvComMultiplier);
            CommissionMultipliers.Add(splitComMultiplier);

            return CommissionMultipliers;
        }

        #endregion
        //comment
        #region Finalise Model

        public void FinaliseCommissionCalculationModel(UserSessionModel admin, CommissionCalculation model)
        {
            var businessService = new BusinessServices(this.Context);
            var userService = new UserServices(this.Context);


            if (!string.IsNullOrEmpty(model.ProjectId.ToString()) && !string.IsNullOrEmpty(model.QuoteId.ToString()))
            {
                var projectQuery = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)
                                   join quote in this.Db.Quotes on new { id = project.ProjectId, qId = model.QuoteId } equals new { id = quote.ProjectId, qId = quote.QuoteId } into Laq
                                   from quote in Laq.DefaultIfEmpty()
                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       OwnerId = project.Owner.UserId,
                                       Name = project.Name,
                                       Description = project.Description,
                                       ProjectDate = project.ProjectDate,
                                       BidDate = project.BidDate,
                                       EstimatedClose = project.EstimatedClose,
                                       EstimatedDelivery = project.EstimatedDelivery,
                                       Expiration = project.Expiration,
                                       ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                       ProjectTypeId = project.ProjectTypeId,
                                       ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                       ConstructionTypeId = project.ConstructionTypeId,
                                       VerticalMarketTypeId = project.VerticalMarketTypeId,
                                       SellerName = project.SellerName,
                                       CustomerName = project.CustomerName,
                                       EngineerName = project.EngineerName,

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
                                           TotalListSplit = (quote == null) ? 0 : quote.TotalListSplit,
                                           TotalListVRV = (quote == null) ? 0 : quote.TotalListVRV,
                                           TotalListUnitary = (quote == null) ? 0: 
                                                              (quote.TotalListUnitary.HasValue ) ? quote.TotalListUnitary.Value : 0,
                                           TotalListLCPackage = (quote == null) ? 0 :
                                                              (quote.TotalListLCPackage.HasValue) ? quote.TotalListLCPackage.Value : 0,
                                           TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                           TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                           TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                           TotalSellSplit = (quote == null) ? 0 : quote.TotalSellSplit,
                                           TotalSellVRV = (quote == null) ? 0 : quote.TotalSellVRV,
                                           TotalSellUnitary = (quote == null) ? 0 : 
                                                              (quote.TotalSellUnitary.HasValue) ? quote.TotalSellUnitary.Value : 0,
                                           TotalSellLCPackage = (quote == null) ? 0 :
                                                              (quote.TotalSellLCPackage.HasValue) ? quote.TotalSellLCPackage.Value : 0,
                                           TotalCountSplit = (quote == null) ? 0 : quote.TotalCountSplit,
                                           TotalCountVRV = (quote == null) ? 0 : quote.TotalCountVRV,
                                           TotalCountVRVIndoor = (quote == null) ? 0 : quote.TotalCountVRVIndoor,
                                           TotalCountVRVOutdoor = (quote == null) ? 0 : quote.TotalCountVRVOutdoor,
                                           ApprovedCommissionPercentage = (quote == null) ? 0 : quote.ApprovedCommissionPercentage,
                                           ApprovedDiscountPercentage = (quote == null) ? 0 : quote.ApprovedDiscountPercentage,
                                           ApprovedDiscountPercentageSplit = (quote == null) ? 0 : quote.ApprovedDiscountPercentageSplit,
                                           ApprovedDiscountPercentageVRV = (quote == null) ? 0 : quote.ApprovedDiscountPercentageVRV,
                                           ApprovedDiscountPercentageUnitary = (quote == null) ? 0: 
                                                                               (quote.ApprovedDiscountPercentageUnitary.HasValue) ? 
                                                                                quote.ApprovedDiscountPercentageUnitary.Value : 0,
                                           ApprovedDiscountPercentageLCPackage = (quote == null) ? 0 :
                                                                               (quote.ApprovedDiscountPercentageLCPackage.HasValue) ?
                                                                                quote.ApprovedDiscountPercentageLCPackage.Value : 0,
                                           TotalNetCommission = (quote == null) ? 0 : quote.TotalNetCommission,
                                           TotalNetNonCommission = (quote == null) ? 0 : quote.TotalNetNonCommission,
                                           TotalNetSplit = (quote == null) ? 0 : quote.TotalNetSplit,
                                           TotalNetVRV = (quote == null) ? 0 : quote.TotalNetVRV,
                                           TotalNetUnitary = (quote == null) ? 0 : 
                                                             (quote.TotalNetUnitary.HasValue) ? quote.TotalNetUnitary.Value : 0,
                                           TotalNetLCPackage = (quote == null) ? 0 :
                                                             (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0,
                                           IsGrossMargin = (quote == null) ? false : quote.IsGrossMargin,
                                           TotalFreight = (quote == null) ? 0 : quote.TotalFreight,
                                           DiscountPercentage = (quote == null) ? 0 : quote.DiscountPercentage,
                                           CommissionPercentage = (quote == null) ? 0 : quote.CommissionPercentage,
                                           Revision = (quote == null) ? 0 : quote.Revision,
                                           CommissionAmount = (model.TotalCommissionAmount == null) ? 0 : model.TotalCommissionAmount,
                                           NetMultiplierValue = (model.TotalNetMaterialValue == null) ? 0 : model.TotalNetMaterialValue

                                       },
                                       ConstructionTypeDescription = project.ConstructionType.Description,
                                       ProjectTypeDescription = project.ProjectType.Description,
                                       ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                       ProjectStatusDescription = project.ProjectStatusType.Description,
                                       VerticalMarketDescription = project.VerticalMarketType.Description,
                                       Deleted = project.Deleted,
                                       Timestamp = project.Timestamp
                                   };

                // model.Project = projectQuery.FirstOrDefault();
            }

            if (model == null || model.Project == null)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
                return;
            }
        }

        public void FinaliseModel(Messages messages, UserSessionModel admin, CommissionRequestModel model, CommissionCalculationModel calculationModel = null)
        {
            var businessService = new BusinessServices(this.Context);
            var userService = new UserServices(this.Context);

            decimal quoteListModelCommissionAmount = 0;

            if (calculationModel != null)
            {
                quoteListModelCommissionAmount = (string.IsNullOrEmpty(calculationModel.RequestedCommissionTotal.ToString())) ? 0 : 
                                                  calculationModel.RequestedCommissionTotal;
            }

            if (model.ProjectId.HasValue && model.QuoteId.HasValue)
            {
                var myQry = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)
                            select project;

                var res = myQry.FirstOrDefault();

                var projectQuery = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)
                                   join quote in this.Db.Quotes on new { id = project.ProjectId, qId = model.QuoteId.Value } 
                                    equals new { id = quote.ProjectId, qId = quote.QuoteId } into Laq
                                   from quote in Laq.DefaultIfEmpty()
                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       OwnerId = project.Owner.UserId,
                                       Name = project.Name,
                                       Description = project.Description,
                                       ProjectDate = project.ProjectDate,
                                       BidDate = project.BidDate,
                                       EstimatedClose = project.EstimatedClose,
                                       EstimatedDelivery = project.EstimatedDelivery,
                                       Expiration = project.Expiration,
                                       ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                       ProjectTypeId = project.ProjectTypeId,
                                       ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                       ConstructionTypeId = project.ConstructionTypeId,
                                       VerticalMarketTypeId = project.VerticalMarketTypeId,
                                       SellerName = project.SellerName,
                                       CustomerName = project.CustomerName,
                                       EngineerName = project.EngineerName,

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

                                       ShipToName = project.ShipToName,

                                       DealerContractorName = project.DealerContractorName,

                                       ActiveQuoteSummary = new QuoteListModel
                                       {
                                           ProjectId = project.ProjectId,
                                           QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                           Alert = (quote == null) ? false : quote.RecalculationRequired,
                                           Title = (quote == null) ? "" : quote.Title,
                                           Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                           TotalList = (quote == null) ? 0 : quote.TotalList,
                                           TotalListSplit = (quote == null) ? 0 : quote.TotalListSplit,
                                           TotalListVRV = (quote == null) ? 0 : quote.TotalListVRV,
                                           TotalListUnitary = (quote == null) ? 0 : 
                                                              ( quote.TotalListUnitary.HasValue ) ? 
                                                              quote.TotalListUnitary.Value : 0,
                                           TotalListLCPackage = (quote == null) ? 0 :
                                                              (quote.TotalListLCPackage.HasValue) ?
                                                              quote.TotalListLCPackage.Value : 0,
                                           TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                           TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                           TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                           TotalSellSplit = (quote == null) ? 0 : quote.TotalSellSplit,
                                           TotalSellVRV = (quote == null) ? 0 : quote.TotalSellVRV,
                                           TotalSellUnitary = (quote == null) ? 0 : (quote.TotalSellUnitary.HasValue) ? quote.TotalSellUnitary.Value : 0,
                                           TotalSellLCPackage = (quote == null) ? 0 : (quote.TotalSellLCPackage.HasValue) ? quote.TotalSellLCPackage.Value : 0,
                                           TotalCountSplit = (quote == null) ? 0 : quote.TotalCountSplit,
                                           TotalCountVRV = (quote == null) ? 0 : quote.TotalCountVRV,
                                           TotalCountVRVIndoor = (quote == null) ? 0 : quote.TotalCountVRVIndoor,
                                           TotalCountVRVOutdoor = (quote == null) ? 0 : quote.TotalCountVRVOutdoor,
                                           ApprovedCommissionPercentage = (quote == null) ? 0 : quote.ApprovedCommissionPercentage,
                                           ApprovedDiscountPercentage = (quote == null) ? 0 : quote.ApprovedDiscountPercentage,
                                           ApprovedDiscountPercentageSplit = (quote == null) ? 0 : quote.ApprovedDiscountPercentageSplit,
                                           ApprovedDiscountPercentageVRV = (quote == null) ? 0 : quote.ApprovedDiscountPercentageVRV,
                                           ApprovedDiscountPercentageUnitary = (quote == null) ? 0: 
                                                                               (quote.ApprovedDiscountPercentageUnitary.HasValue ) ? 
                                                                                quote.ApprovedDiscountPercentageUnitary.Value : 0,
                                           //TODO: verify if this is needed
                                           ApprovedDiscountPercentageLCPackage = (quote == null) ? 0 :
                                                                               (quote.ApprovedDiscountPercentageLCPackage.HasValue) ?
                                                                                quote.ApprovedDiscountPercentageLCPackage.Value : 0,
                                           TotalNetCommission = (quote == null) ? 0 : quote.TotalNetCommission,
                                           TotalNetNonCommission = (quote == null) ? 0 : quote.TotalNetNonCommission,
                                           TotalNetSplit = (quote == null) ? 0 : quote.TotalNetSplit,
                                           TotalNetVRV = (quote == null) ? 0 : quote.TotalNetVRV,
                                           TotalNetUnitary = (quote == null) ? 0 : ( quote.TotalNetUnitary.HasValue ) ? quote.TotalNetUnitary.Value : 0,
                                           TotalNetLCPackage = (quote == null) ? 0 : (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0,
                                           IsGrossMargin = (quote == null) ? false : quote.IsGrossMargin,
                                           TotalFreight = (quote == null) ? 0 : quote.TotalFreight,
                                           DiscountPercentage = (quote == null) ? 0 : quote.DiscountPercentage,
                                           CommissionPercentage = (quote == null) ? 0 : quote.CommissionPercentage,
                                           Revision = (quote == null) ? 0 : quote.Revision,
                                           CommissionAmount = (string.IsNullOrEmpty(quoteListModelCommissionAmount.ToString())) ? 0 : 
                                                               quoteListModelCommissionAmount
                                        
                                       },
                                       ConstructionTypeDescription = project.ConstructionType.Description,
                                       ProjectTypeDescription = project.ProjectType.Description,
                                       ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                       ProjectStatusDescription = project.ProjectStatusType.Description,
                                       VerticalMarketDescription = project.VerticalMarketType.Description,
                                       Deleted = project.Deleted,
                                       Timestamp = project.Timestamp
                                   };

                model.Project = projectQuery.FirstOrDefault();

                var addressService = new AddressServices(this.Context);
                model.Project.SellerAddress = addressService.GetAddressModel(admin, model.Project.SellerAddress);
                model.Project.CustomerAddress = addressService.GetAddressModel(admin, model.Project.CustomerAddress);
                model.Project.EngineerAddress = addressService.GetAddressModel(admin, model.Project.EngineerAddress);
                model.Project.ShipToAddress = addressService.GetAddressModel(admin, model.Project.ShipToAddress);

                model.QuoteItems = new QuoteServices(this.Context).GetQuoteItemListModel(admin, (long)model.QuoteId).Model as List<QuoteItemListModel>;

            }

            if (model == null || model.Project == null)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
                return;
            }

            var service = new QuoteServices();


            if (calculationModel != null)
            {
                //model.RequestedCommissionPercentageVRV = calculationModel.RequestedCommissionVRVPercentage;
                //model.RequestedCommissionPercentageSplit = calculationModel.RequestedCommissionSplitPercentage;
                //model.RequestedCommissionVRV = calculationModel.CommissionAmountVRV;
                //model.RequestedCommissionSplit = calculationModel.CommissionAmountSplit;
                //model.RequestedCommissionTotal = calculationModel.TotalCommissionAmount;
                //model.RequestedMultiplierVRV = calculationModel.CommissionRequestMultiplierVRV;
                //model.RequestedMultiplierSplit = calculationModel.CommissionRequestMultiplierSplit;
            }
            else
            {
                model.StandardTotals = service.CalculateTotalStandard(model.Quote);
                //model.RequestedCommissionTotal *= 100M;

                //model.RequestedCommissionSplitPercentage *= 100M;
                //model.RequestedCommissionVRV *= 100M;

                //model.ApprovedCommissionTotal *= 100M;
                //model.ApprovedCommissionSplit *= 100M;
                //model.ApprovedCommissionVRV *= 100M;
            }

            if (model.ApprovedCommissionTotal > 0)
            {
                model.ApprovedTotalsCommission = service.CalculateTotalDiscountsApproved(
                    model.Quote, 
                    model.ApprovedCommissionTotal,
                    model.ApprovedCommissionSplit, 
                    model.ApprovedCommissionVRV, 
                    model.ApprovedCommissionUnitary,
                    model.ApprovedCommissionLCPackage,  
                    model.RequestedCommissionPercentage);
            }
            else
            {
                model.ApprovedTotalsCommission = service.CalculateTotalDiscountsApproved(
                    model.Quote, 
                    model.RequestedCommissionTotal,
                    //model.RequestedCommissionPercentageSplit, 
                    model.RequestedCommissionSplit,
                    model.RequestedCommissionVRV, 
                    model.RequestedCommissionUnitary, 
                    model.RequestedCommissionLCPackage,
                    model.RequestedCommissionPercentage);
            }

            // Dropdowns 

            model.CommissionRequestStatusTypes = htmlService.DropDownModelDiscountRequestStatusTypes((model == null) ? null : model.CommissionRequestStatusTypeId);

            model.SystemBasisDesignTypes = htmlService.DropDownModelSystemBasisDesignTypes((model == null) ? null : model.SystemBasisDesignTypeId);

            model.ZoneStrategyTypes = htmlService.DropDownModelZoneStrategyTypes((model == null) ? null : model.ZoneStrategyTypeId);

            model.BrandSpecifiedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandSpecifiedTypeId);

            model.BrandApprovedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandApprovedTypeId);

            model.DaikinEquipmentAtAdvantageTypes = htmlService.DropDownModelDaikinEquipmentAtAdvantageTypes((model == null) ? null : model.DaikinEquipmentAtAdvantageTypeId);

            model.ProbabilityOfCloseTypes = htmlService.DropDownModelProbabilityOfCloseTypes((model == null) ? null : model.ProbabilityOfCloseTypeId);

            model.Project.ProjectTypes = htmlService.DropDownModelProjectTypes((model == null) ? null : model.Project.ProjectTypeId);

            model.Project.VerticalMarketTypes = htmlService.DropDownModelVerticalMarkets((model == null) ? null : model.Project.VerticalMarketTypeId);

            model.Project.ConstructionTypes = htmlService.DropDownModelConstructionTypes((model == null) ? null : model.Project.ConstructionTypeId);

            model.Project.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.Project.ProjectStatusTypeId, DropDownMode.Filtering);

            model.Project.ShipToAddress.States = htmlService.DropDownModelStates((model == null) ? null : model.Project.ShipToAddress);

            model.WinLossConditionTypes = htmlService.DropDownModelWinLossConditionTypes((model == null) ? null : model.WinLossConditionTypeId, DropDownMode.Filtering);

            model.Project.CustomerAddress.States = htmlService.DropDownModelStates((model == null) ? null : model.Project.CustomerAddress);

            model.FundingTypes = htmlService.DropDownModelFundingTypes((model == null) ? null : model.FundingTypeId, DropDownMode.Filtering);

            model.CustomerTypes = htmlService.DropDownModelCustomerTypes((model == null) ? null : model.CustomerTypeId, DropDownMode.Filtering);

        }

        #endregion

        #region Post Requests

        public ServiceResponse Delete(UserSessionModel user, CommissionRequestModel model)
        {
            return ChangeStatus(user, model, CommissionRequestStatusTypeEnum.Deleted);
        }

        public ServiceResponse Approve(UserSessionModel user, CommissionRequestModel model)
        {
            return ChangeStatus(user, model, CommissionRequestStatusTypeEnum.Approved);
        }

        public ServiceResponse Reject(UserSessionModel user, CommissionRequestModel model)
        {
            return ChangeStatus(user, model, CommissionRequestStatusTypeEnum.Rejected);
        }

        private ServiceResponse ChangeStatus(UserSessionModel user, CommissionRequestModel model, CommissionRequestStatusTypeEnum status)
        {
            this.Db.ReadOnly = false;

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                entity.CommissionRequestStatusTypeId = (byte)status;

                if (status == CommissionRequestStatusTypeEnum.Approved ||
                    status == CommissionRequestStatusTypeEnum.Rejected)
                {
                    entity.ResponseNotes = model.ResponseNotes;
                    if (entity.ResponseNotes == null)
                    {
                        entity.ResponseNotes = "None";
                    }

                    entity.ApprovedCommissionPercentVRV = model.RequestedCommissionPercentageVRV;
                    entity.ApprovedCommissionPercentSplit = model.RequestedCommissionPercentageSplit;
                    entity.ApprovedCommissionPercentUnitary = model.RequestedCommissionPercentageUnitary;
                    entity.ApprovedCommissionPercentLCPackage = model.RequestedCommissionPercentageLCPackage;

                    entity.ApprovedCommissionPercent = model.RequestedCommissionPercentage;

                    entity.ApprovedMultiplier = model.RequestedMultiplier;
                    entity.ApprovedMultiplierSplit = model.RequestedMultiplierSplit;
                    entity.ApprovedMultiplierVRV = model.RequestedMultiplierVRV;
                    entity.ApprovedMultiplierUnitary = model.RequestedMultiplierUnitary;
                    entity.ApprovedMultiplierLCPackage = model.RequestedMultiplierLCPackage;

                    entity.ApprovedCommissionTotal = model.RequestedCommissionTotal;

                    entity.RequestedCommissionVRV = model.RequestedCommissionVRV;
                    entity.RequestedCommissionSplit = model.RequestedCommissionSplit;
                    entity.RequestedCommissionUnitary = model.RequestedCommissionUnitary;
                    entity.RequestedCommissionLCPackage = model.RequestedCommissionLCPackage;

                    entity.RequestedNetMaterialMultiplierVRV = model.RequestedNetMaterialMultiplierVRV;
                    entity.RequestedNetMaterialMultiplierSplit = model.RequestedNetMaterialMultiplierSplit;
                    entity.RequestedNetMaterialMultiplierUnitary = model.RequestedNetMaterialMultiplierUnitary;
                    entity.RequestedNetMaterialMultiplierLCPackage = model.RequestedNetMaterialMultiplierLCPackage;

                    entity.RequestedNetMultiplierValueVRV = model.RequestedNetMaterialValueVRV;
                    entity.RequestedNetMultiplierValueSplit = model.RequestedNetMaterialValueSplit;
                    entity.RequestedNetMultiplierValueUnitary = model.RequestedNetMaterialValueUnitary;
                    entity.RequestedNetMultiplierValueLCPackage = model.RequestedNetMaterialValueLCPackage;

                    entity.RequestedNetMaterialValue = model.RequestedNetMaterialValue;
                    entity.RequestedNetMaterialValueMultiplier = model.RequestedNetMaterialValueMultiplier;

                    entity.TotalNet = model.TotalNet;
                    entity.TotalRevised = model.TotalRevised;

                    entity.TotalNetSplit = model.TotalNetSplit;
                    entity.TotalNetVRV = model.TotalNetVRV;
                    entity.TotalNetUnitary = model.TotalNetUnitary;
                    entity.TotalNetLCPackage = model.TotalNetLCPackage;

                    entity.EmailsList = model.EmailsList;
                }

                Entry = Db.Entry(entity);

                RulesOnEdit(user, entity);
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, "Commission Request");
            }

            var newModel = GetCommissionRequestModel(user, model).Model as CommissionRequestModel;

            this.Response.Model = newModel;

            return this.Response;
        }

        //TODO: is this being used?
        public ServiceResponse PostCommissionCalculationModel(UserSessionModel admin, CommissionCalculation model)
        {
            this.Db.ReadOnly = false;

            try
            {
                CommissionCalculation entity = null;
                entity = CommissionCalculationModelToEntity(admin, model);
                SaveToDatabase(model, entity, string.Format("Commission Calculation '0'", ""));
            }
            catch (Exception ex)
            {
                this.Response.AddError(ex.Message);
                this.Response.Messages.AddAudit(ex);
            }

            FinaliseCommissionCalculationModel(admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse PostModel(UserSessionModel admin, CommissionRequestModel model, CommissionCalculationModel calculationModel = null)
        {
            this.Db.ReadOnly = false;

            try
            {
                CommissionRequest entity = null;

                model.IsCommissionCalculation = (calculationModel != null || model.IsCommissionCalculation);

                if (model.IsCommissionCalculation)
                {
                    //model.RequestedCommissionTotal /= 100M;
                    //model.RequestedCommissionSplitPercentage /= 100M;
                    //model.RequestedCommissionVRVPercentage /= 100M;
                    //model.RequestedCommissionVRV /= 100M;
                    //model.ApprovedCommissionTotal /= 100M;
                    //model.ApprovedCommissionSplit /= 100M;
                    //model.ApprovedCommissionVRV /= 100M;
                    //model.RequestedCommissionTotal /= 100M;

                    if (calculationModel != null)
                    {//TODO: missing code for Unitary? is it handled differently?
                        model.RequestedCommissionPercentageVRV = calculationModel.RequestedCommissionPercentageVRV;
                        model.RequestedCommissionPercentageSplit = calculationModel.RequestedCommissionPercentageSplit;
                        model.RequestedCommissionPercentageLCPackage = calculationModel.RequestedCommissionPercentageLCPackage;

                        model.RequestedCommissionVRV = calculationModel.RequestedCommissionVRV;
                        model.RequestedCommissionSplit = calculationModel.RequestedCommissionSplit;
                        model.RequestedCommissionLCPackage = calculationModel.RequestedCommissionLCPackage;

                        model.RequestedCommissionTotal = calculationModel.RequestedCommissionTotal;

                        model.RequestedMultiplierVRV = calculationModel.RequestedMultiplierVRV;
                        model.RequestedMultiplierSplit = calculationModel.RequestedMultiplierSplit;
                        model.RequestedMultiplierLCPackage = calculationModel.RequestedMultiplierLCPackage;

                        model.RequestedNetMaterialMultiplierSplit = calculationModel.RequestedNetMaterialMultiplierSplit;
                        model.RequestedNetMaterialMultiplierVRV = calculationModel.RequestedNetMaterialMultiplierVRV;
                        model.RequestedNetMaterialMultiplierLCPackage = calculationModel.RequestedNetMaterialMultiplierLCPackage;

                        model.RequestedNetMaterialValueSplit = calculationModel.RequestedNetMaterialValueSplit;
                        model.RequestedNetMaterialValueVRV = calculationModel.RequestedNetMaterialValueVRV;
                        model.RequestedNetMaterialValueLCPackage = calculationModel.RequestedNetMaterialValueLCPackage;

                        model.RequestedCommissionTotal = calculationModel.RequestedCommissionTotal;
                        model.RequestedCommissionPercentage = calculationModel.RequestedCommissionPercentage;

                        model.RequestedMultiplier = calculationModel.RequestedMultiplier;

                        model.TotalNetVRV = calculationModel.TotalNetVRV;
                        model.TotalNetSplit = calculationModel.TotalNetSplit;
                        model.TotalNetLCPackage = calculationModel.TotalNetLCPackage;

                        model.RequestedNetMaterialValue = calculationModel.RequestedNetMaterialValue;
                        model.RequestedNetMaterialValueMultiplier = calculationModel.RequestedNetMaterialValueMultiplier;

                        model.TotalNet = calculationModel.TotalNet;

                        //assign default value to the required fields so the RequestCommission can pass the ApplyBusinessRules so it can saved to Database

                        model.Notes = "None";
                        //model.SystemBasisDesignTypeId = 1;
                        // model.ZoneStrategyTypeId = 1;
                        //model.BrandApprovedTypeId = 1;
                        // model.BrandSpecifiedTypeId = 1;
                        model.OrderPlannedFor = System.DateTime.Now;
                        model.OrderDeliveryDate = System.DateTime.Now;
                        model.DaikinEquipmentAtAdvantageTypeId = 1;
                        model.ProbabilityOfCloseTypeId = 1;
                        model.IsCommissionCalculation = true;
                    }
                }
                else
                {
                    model.IsCommissionCalculation = false;
                }

                // Validate Model 
                RulesOnValidateModel(admin, model);

                // Map to Entity
                if (this.Response.IsOK)
                {
                    entity = ModelToEntity(admin, model);

                    var message = Utilities.SavePostedFile(model.CompetitorQuoteFile, Utilities.GetDARDirectory(model.QuoteId.Value), 512000);

                    if (message != null)
                    {
                        this.Response.AddError(message);
                    }

                    message = Utilities.SavePostedFile(model.CompetitorLineComparsionFile, Utilities.GetDARDirectory(model.QuoteId.Value), 512000);

                    if (message != null)
                    {
                        this.Response.AddError(message);
                    }
                }

                // Validate Entity
                if (this.Response.IsOK)
                {
                    this.Response.PropertyReference = "";

                    //ProjectServices projectServices = new ProjectServices();

                    //var projectEntity = projectServices.GetEntity(admin, model.Project);

                    //Entry = Db.Entry(entity);

                    //projectEntity = projectServices.ModelToEntity(admin, model.Project);

                    //if (this.Response.IsOK)
                    //{


                    //    //this.Context.Entry(projectEntity).State = EntityState.Modified;
                    //    this.Context.Entry(projectEntity).State = EntityState.Detached;

                    //    projectServices.RulesOnEdit(admin, projectEntity);
                    //}

                    //if (this.Response.IsOK)
                    //{
                    //    base.SaveToDatabase(model.Project, projectEntity, string.Format("Project '{0}'", projectEntity.Name));
                    //}


                    ApplyBusinessRules(admin, entity);
                }

                if (this.Response.IsOK)
                {
                    SaveToDatabase(model, entity, string.Format("Commission Request '0'", ""));

                    if (entity.CommissionRequestStatusTypeId == 2 && model.CommissionRequestStatusTypeId == 0)
                    {
                        model.CommissionRequestStatusTypeId = 2;
                    }
                }

            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            //if(model.CommissionRequestStatusTypeId == 0)
            //{
            //    this.Response.Messages.Clear();
            //}

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        #endregion

        #region Post Model To Entity

        private CommissionCalculation CommissionCalculationModelToEntity(UserSessionModel admin, CommissionCalculation model)
        {
            var entity = GetCommissionCalculationEntity(admin, model);

            if (this.Response.HasError) return null;

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        public CommissionRequest ModelToEntity(UserSessionModel admin, CommissionRequestModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.SystemBasisDesignTypeId          = model.SystemBasisDesignTypeId;
            entity.ZoneStrategyTypeId               = model.ZoneStrategyTypeId;
            entity.BrandSpecifiedTypeId             = model.BrandSpecifiedTypeId;
            entity.BrandApprovedTypeId              = model.BrandApprovedTypeId;
            entity.FundingTypeId                    = model.FundingTypeId;
            entity.WinLossTypeId                    = model.WinLossConditionTypeId;
            entity.HasCompetitorPrice               = model.HasCompetitorPrice;
            entity.CompetitorPrice                  = model.CompetitorPrice;
            entity.HasCompetitorQuote               = model.HasCompetitorQuote;
            entity.CompetitorQuoteFileName          = model.CompetitorQuoteFileName;
            entity.HasCompetitorLineComparsion      = model.HasCompetitorLineComparsion;
            entity.CompetitorLineComparsionFileName = model.CompetitorLineComparsionFileName;
            entity.IsConfidentCompetitorQuote       = model.IsConfidentCompetitorQuote;

            entity.StartUpCosts             = model.StartUpCosts;
            entity.ThirdPartyEquipmentCosts = model.ThirdPartyEquipmentCosts;

            entity.OrderPlannedFor   = model.OrderPlannedFor;
            entity.OrderDeliveryDate = model.OrderDeliveryDate;

            entity.RequestedMultiplier        = model.RequestedMultiplier;
            entity.RequestedMultiplierSplit   = model.RequestedMultiplierSplit;
            entity.RequestedMultiplierVRV     = model.RequestedMultiplierVRV;
            entity.RequestedMultiplierUnitary = model.RequestedMultiplierUnitary;
            entity.RequestedMultiplierLCPackage = model.RequestedMultiplierLCPackage;

            entity.RequestedCommissionPercent        = model.RequestedCommissionPercentage;
            entity.RequestedCommissionPercentSplit   = model.RequestedCommissionPercentageSplit;
            entity.RequestedCommissionPercentVRV     = model.RequestedCommissionPercentageVRV;
            entity.RequestedCommissionPercentUnitary = model.RequestedCommissionPercentageUnitary;
            entity.RequestedCommissionPercentLCPackage = model.RequestedCommissionPercentageLCPackage;

            entity.RequestedCommissionTotal   = model.RequestedCommissionTotal;
            entity.RequestedCommissionSplit   = model.RequestedCommissionSplit;
            entity.RequestedCommissionVRV     = model.RequestedCommissionVRV;
            entity.RequestedCommissionUnitary = model.RequestedCommissionUnitary;
            entity.RequestedCommissionLCPackage = model.RequestedCommissionLCPackage;

            entity.RequestedNetMaterialMultiplierSplit   = model.RequestedNetMaterialMultiplierSplit;
            entity.RequestedNetMaterialMultiplierVRV     = model.RequestedNetMaterialMultiplierVRV;
            entity.RequestedNetMaterialMultiplierUnitary = model.RequestedNetMaterialMultiplierUnitary;
            entity.RequestedNetMaterialMultiplierLCPackage = model.RequestedNetMaterialMultiplierLCPackage;

            entity.RequestedNetMultiplierValueSplit   = model.RequestedNetMaterialValueSplit;
            entity.RequestedNetMultiplierValueVRV     = model.RequestedNetMaterialValueVRV;
            entity.RequestedNetMultiplierValueUnitary = model.RequestedNetMaterialValueUnitary;
            entity.RequestedNetMultiplierValueLCPackage = model.RequestedNetMaterialValueLCPackage;

            entity.TotalNetVRV     = model.TotalNetVRV;
            entity.TotalNetSplit   = model.TotalNetSplit;
            entity.TotalNetUnitary = model.TotalNetUnitary;
            entity.TotalNetLCPackage = model.TotalNetLCPackage;

            entity.TotalListVRV     = model.Quote.TotalListVRV;
            entity.TotalListSplit   = model.Quote.TotalListSplit;
            entity.TotalListUnitary = model.Quote.TotalListUnitary;
            entity.TotalListLCPackage = model.Quote.TotalListLCPackage;

            entity.TotalNet     = model.TotalNet;
            entity.TotalRevised = model.TotalRevised;

            entity.RequestedNetMaterialValue           = model.RequestedNetMaterialValue;
            entity.RequestedNetMaterialValueMultiplier = model.RequestedNetMaterialValueMultiplier;

            entity.ApprovedCommissionPercentSplit   = model.ApprovedCommissionPercentageSplit;
            entity.ApprovedCommissionPercentVRV     = model.ApprovedCommissionPercentageVRV;
            entity.ApprovedCommissionPercentUnitary = model.ApprovedCommissionPercentageUnitary;
            entity.ApprovedCommissionPercentLCPackage = model.ApprovedCommissionPercentageLCPackage;

            entity.ApprovedMultiplier        = model.ApprovedMultiplier;
            entity.ApprovedMultiplierSplit   = model.ApprovedMultiplierSplit;
            entity.ApprovedMultiplierVRV     = model.ApprovedMultiplierVRV;
            entity.ApprovedMultiplierUnitary = model.ApprovedMultiplierUnitary;
            entity.ApprovedMultiplierLCPackage = model.ApprovedMultiplierLCPackage;

            entity.ApprovedCommissionPercent = model.ApprovedCommissionPercentage;

            entity.WinLossTypeId = model.WinLossConditionTypeId;
            entity.FundingTypeId = model.FundingTypeId;

            entity.Notes         = model.Notes;
            entity.ResponseNotes = (model.ResponseNotes == null) ? "None" : model.ResponseNotes;

            entity.CustomerTypeId    = model.CustomerTypeId;
            entity.RepPhoneNumber    = model.RepPhoneNumber;
            entity.SellerPhoneNumber = model.SellerPhoneNumber;
            entity.RepEmail          = model.RepEmail;
            entity.SellerEmail       = model.SellerEmail;
            entity.RepFaxNumber      = model.RepFaxNumber;
            entity.SellerPhoneNumber = model.SellerFaxNumber;

            entity.IsCommissionCalculation = model.IsCommissionCalculation;
            entity.RequestedOn             = (model.RequestedOn == null) ? System.DateTime.Now : model.RequestedOn.Value;

            if (model.CurrentUser != null)
            {
                entity.StatusTypeModifiedBy = model.CurrentUser.UserId;
            }
            else
            {
                entity.StatusTypeModifiedBy = admin.UserId;
            }

            if (admin.HasAccess(SystemAccessEnum.ApprovedRequestCommission))
            {
                entity.ResponseNotes = model.ResponseNotes;
            }

            if (model.EmailsList != null)
            {
                entity.EmailsList = model.EmailsList;
            }

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        #endregion


        public CommissionRequest GetEntity(UserSessionModel user, CommissionRequestModel model)
        {
            var entity = model.CommissionRequestId.HasValue ?
                this.Db.QueryCommissionRequestsViewable(user)
                .Where(d => d.CommissionRequestId == model.CommissionRequestId.Value)
                .FirstOrDefault() : Db.CommissionRequestCreate(model.ProjectId.Value, model.QuoteId.Value);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;

        }

        #region excel workbook methods

        private void SetCellValue(ISheet sheet, IRow row, int cellId, string value, bool styleCellAsLabel = false)
        {
            var cell = row.CreateCell(cellId);
            cell.SetCellValue(value);
            if (styleCellAsLabel == true)
            {
                StyleLabel(cell);
            }
            else
            {
                StyleValue(cell);
            }
        }

        private void SetLabel(ISheet sheet, IRow row, int col, string value)
        {

            var cell = row.CreateCell(col);
            cell.SetCellValue(value);

            StyleLabel(cell);
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

        private void SetLabelValue(ISheet sheet, IRow row, int labelCell, int valueCell, string label, string value)
        {
            SetLabel(sheet, row, labelCell, label);

            var cell = row.CreateCell(valueCell);
            cell.SetCellValue(value);

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

        private IRow GetOrCreateRow(ISheet sheet, int row)
        {
            return sheet.GetRow(row) ?? sheet.CreateRow(row);
        }


        #endregion

        #region Calculation Methods

        private CommissionCalculation GetCommissionCalculationEntity(UserSessionModel user, CommissionCalculation model)
        {
            var entity = (string.IsNullOrEmpty(model.CommissionCalculationId.ToString())) ?
                          this.Db.QueryCommissionCalculationViewable(user)
                          .Where(d => d.CommissionCalculationId == model.CommissionCalculationId)
                          .FirstOrDefault() : Db.CommissionCalculationCreate(model.ProjectId, model.QuoteId);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
            }

            return entity;
        }

        public ServiceResponse CalculateCommission(UserSessionModel user, CommissionRequestModel model)
        {
            // TODO:  We really need to completely refactor this whole commission calculation thing.  It's pretty message up.
            // TODO:  This is just a patch for release.
            // TODO:  We don't need to save all of this craziness.
            var entity = ModelToEntity(user, model);

            RecalculateCommission(user, entity);
            RecalculateQuote(user, entity);

            ServiceResponse resp = this.quoteService.GetQuoteModel(user, model.ProjectId, model.QuoteId);
            var quote = resp.Model as QuoteModel;

            resp = this.quoteService.PostModel(user, quote);

            var entry = Db.Entry(entity);

            if (entry.State == EntityState.Added)
            {
                Db.SaveChanges();
            }

            entry.OriginalValues.SetValues(entry.GetDatabaseValues());

            Update(entity,
                     o => o.RequestedCommission,
                     o => o.RequestedCommissionPercent,
                     o => o.RequestedCommissionPercentSplit,
                     o => o.RequestedCommissionPercentVRV,
                     o => o.RequestedCommissionPercentUnitary,
                     o => o.RequestedCommissionSplit,
                     o => o.RequestedCommissionUnitary,
                     o => o.RequestedCommissionTotal,
                     o => o.RequestedCommissionVRV,
                     o => o.RequestedMultiplier,
                     o => o.RequestedMultiplierSplit,
                     o => o.RequestedMultiplierVRV,
                     o => o.RequestedMultiplierUnitary,
                     o => o.RequestedNetMaterialMultiplierSplit,
                     o => o.RequestedNetMaterialMultiplierVRV,
                     o => o.RequestedNetMaterialMultiplierUnitary,
                     o => o.RequestedNetMaterialValue,
                     o => o.RequestedNetMaterialValueMultiplier,
                     o => o.RequestedNetMultiplierValueSplit,
                     o => o.RequestedNetMultiplierValueVRV,
                     o => o.RequestedNetMultiplierValueUnitary,
                     o => o.TotalNetSplit,
                     o => o.TotalNetVRV,
                     o => o.TotalNetUnitary,
                     o => o.TotalNet);

            return this.GetCommissionRequestModel(user, model);
        }

        #endregion Calculation Methods

    }
}
