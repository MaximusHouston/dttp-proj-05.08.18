//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

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

namespace DPO.Domain
{

    public partial class DiscountRequestServices : BaseServices
    {
        HtmlServices htmlService;

        public DiscountRequestServices() : base() { htmlService = new HtmlServices(this.Context); }

        
        public DiscountRequestServices(DPOContext context)
            : base(context)
        {
            htmlService = new HtmlServices(context);
        }

        #region Get Requests

        public ServiceResponse GetDiscountRequestListModel(UserSessionModel user, SearchDiscountRequests search)
        {
            Log.InfoFormat("Enter GetDiscountRequestListModel for User: {0}", user.Email);
            Log.DebugFormat("SearchDiscountRequests Filter : {0}", search.Filter);

            search.ReturnTotals = true;

            var query = from dis in this.Db.QueryDiscountRequestsViewableBySearch(user, search)
                        join project in this.Db.Projects on dis.ProjectId equals project.ProjectId
                        join quote in this.Db.Quotes on dis.QuoteId equals quote.QuoteId
                        join owner in this.Db.Users on project.OwnerId equals owner.UserId
                        join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId
                        select new DiscountRequestModel
                        {
                            DiscountRequestId = dis.DiscountRequestId,
                            ProjectOwner = owner.FirstName + " " + owner.LastName,
                            ProjectOwnerId = owner.UserId,
                            BusinessId = owner.BusinessId,
                            BusinessName = business.BusinessName,
                            ProjectId = dis.ProjectId,
                            QuoteId = dis.QuoteId,
                            Project = new ProjectModel
                            {

                                Name = project.Name,
                                ActiveQuoteSummary = new QuoteListModel { Title = quote.Title }
                            },
                            RequestedOn = dis.RequestedOn,
                            DiscountRequestStatusTypeId = dis.DiscountRequestStatusTypeId,
                            DiscountRequestStatusTypeDescription = dis.DiscountRequestStatusType.Description,
                            Timestamp = dis.Timestamp
                        };

            Log.DebugFormat("return list of DiscountRequestModels ");

            try
            {
                this.Response.Model = query.ToList();
            }
            catch(Exception ex)
            {
                Log.FatalFormat("Exception Source: {0}", ex.Source);
                Log.FatalFormat("Exception: {0}", ex.Message);
                Log.FatalFormat("Inner Exception: {0}", ex.InnerException.Message);
            }

            Log.Info("GetDiscountRequestListModel finished.");

            return this.Response;
        }

        public bool ValidateEmails(List<string> emails)
        {
            Log.Info("Enter ValidateEmails");
            Log.Debug("Email list: ");

            foreach (var email in emails)
                Log.Debug(email);

            Log.DebugFormat("Start calling ValidateEmails from dbContext");
           
            bool result = this.Db.ValidateEmails(emails);

            Log.DebugFormat("ValidateEmails finished.");
            Log.DebugFormat("isValidate: {0}", result);

            return result;
        }

        public List<String> GetInvalidEmails(List<string> emails)
        {
            Log.InfoFormat("Enter GetInvalidEmails");
            Log.Debug("Email list: ");

            foreach (var email in emails)
                Log.Debug(email);

            Log.DebugFormat("Start calling GetInvalidEmails from dbContext: {0}", this.Db.GetType());

            List<string> InvalidEmails = this.Db.GetInvalidEmails(emails);

            Log.Debug("GetInvalidEmail finished");

            if (InvalidEmails.Count() > 0)
            {
                Log.Debug("List of Invalid Email: ");
                foreach (var invalidEmail in InvalidEmails)
                    Log.Debug(invalidEmail);
            }

            Log.Info("GetInvalidEmails finished.");
            return InvalidEmails;
        }

        public DiscountRequestSendEmailModel GetDiscountRequestSendEmailModel(DiscountRequestModel req)
        {

            Project proj = this.Db.GetProjectOwnerAndBusiness(req.ProjectId);
            Business business = this.Db.GetBusinessByProjectOwner(req.ProjectId);

            req.ProjectOwner = proj.Owner.FirstName + " " + proj.Owner.LastName;
            req.BusinessName = business.BusinessName;

            return new DiscountRequestSendEmailModel
            {
                discountRequest = req,
                AccountManagerEmail = business.AccountManagerEmail,
                AccountOwnerEmail = business.AccountOwnerEmail
            };
        }

        public MemoryStream GetDiscountRequestExportExcelFile(DiscountRequestModel model, bool showCostPricing)
        {
            var stream = new MemoryStream();

            var workbook = new HSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = model.BusinessName;
            workbook.DocumentSummaryInformation = dsi;

            //create a entry of SummaryInformation
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = string.Format("Project '{0}' , Quote '{1}' report ", model.Project.Name, model.Quote.Title);
            workbook.SummaryInformation = si;

            var worksheet = workbook.CreateSheet(model.Quote.Title);

            var ch = new HSSFCreationHelper(workbook);
            var fe = ch.CreateFormulaEvaluator();

            int row = 0;

            //header information

            var r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Status:", model.DiscountRequestStatusTypeDescription);

            if (model.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved)
            {
                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Approved by:", model.DiscountRequestStatusModifiedBy);
                SetLabelValue(worksheet, r, 2, 3, "Approved on:", model.DiscountRequestStatusModifiedOn);
            }

            //project details

            row++;
            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Project Name:", model.Project.Name);
            SetLabelValue(worksheet, r, 3, 4, "Construction Type:", model.Project.ConstructionTypeDescription);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Project Reference:", model.Project.ProjectId);
            SetLabelValue(worksheet, r, 3, 4, "Project Status:", model.Project.ProjectStatusDescription);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Registration Date:", model.Project.ProjectDate);
            SetLabelValue(worksheet, r, 3, 4, "Project Type:", model.Project.ProjectTypeDescription);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Bid Date:", model.Project.BidDate);
            SetLabelValue(worksheet, r, 3, 4, "Project Open Status:", model.Project.ProjectOpenStatusDescription);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Estimated Close:", model.Project.EstimatedClose);
            SetLabelValue(worksheet, r, 3, 4, "Vertical Market:", model.Project.VerticalMarketDescription);

            r = worksheet.CreateRow(row++);

            SetLabelValue(worksheet, r, 0, 1, "Estimated Delivery:", model.Project.EstimatedDelivery);
            SetLabelValue(worksheet, r, 3, 4, "Project Notes:", model.Project.Description);

            //addresses
            row++;
            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, "Engineer Details:", true);
            SetCellValue(worksheet, r, 1, "Dealer/Contractor Address:", true);
            SetCellValue(worksheet, r, 2, "Seller Address:", true);
            SetCellValue(worksheet, r, 3, "Ship To Address:", true);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, model.Project.EngineerAddress.AddressLine1);
            SetCellValue(worksheet, r, 1, model.Project.CustomerAddress.AddressLine1);
            SetCellValue(worksheet, r, 2, model.Project.SellerAddress.AddressLine1);
            SetCellValue(worksheet, r, 3, model.Project.ShipToAddress.AddressLine1);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, model.Project.EngineerAddress.AddressLine2);
            SetCellValue(worksheet, r, 1, model.Project.CustomerAddress.AddressLine2);
            SetCellValue(worksheet, r, 2, model.Project.SellerAddress.AddressLine2);
            SetCellValue(worksheet, r, 3, model.Project.ShipToAddress.AddressLine2);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, model.Project.EngineerAddress.AddressLine3);
            SetCellValue(worksheet, r, 1, model.Project.CustomerAddress.AddressLine3);
            SetCellValue(worksheet, r, 2, model.Project.SellerAddress.AddressLine3);
            SetCellValue(worksheet, r, 3, model.Project.ShipToAddress.AddressLine3);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, model.Project.EngineerAddress.Location);
            SetCellValue(worksheet, r, 1, model.Project.CustomerAddress.Location);
            SetCellValue(worksheet, r, 2, model.Project.SellerAddress.Location);
            SetCellValue(worksheet, r, 3, model.Project.ShipToAddress.Location);

            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, model.Project.EngineerAddress.PostalCode);
            SetCellValue(worksheet, r, 1, model.Project.CustomerAddress.PostalCode);
            SetCellValue(worksheet, r, 2, model.Project.SellerAddress.PostalCode);
            SetCellValue(worksheet, r, 3, model.Project.ShipToAddress.PostalCode);

            //quote details
            row++;

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Quote Name:", model.Quote.Title);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Products in Quote:", model.QuoteItems.Count);

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

            //actual DAR stuff
            row++;
            r = worksheet.CreateRow(row++);
            SetCellValue(worksheet, r, 0, "Project systems and competitive position of opportunity".ToUpper(), true);

            row++;
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Project System Basis Of Design", model.SystemBasisDesignTypeDescription);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Brand Specified:", model.BrandApprovedTypeDescription);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Zone Strategy:", model.ZoneStrategyTypeDescription);
            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Approved Equals:", model.BrandSpecifiedTypeDescription);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Copy of competitors price to customer available", (model.HasCompetitorPrice) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Copy of competitors quote to customer available", (model.HasCompetitorQuote) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Line by line comparison of competitor to Daikin completed", (model.HasCompetitorLineComparsion) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Is Daikin equipment at an advantage:", (model.DaikinEquipmentAtAdvantageDescription != null) ? model.DaikinEquipmentAtAdvantageDescription : "No answer given");

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Rep/distributor confident that competition offer is equal to this offer", (model.IsConfidentCompetitorQuote) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitor price:", (model.CompetitorPrice != null) ? string.Format("{0:C}", model.CompetitorPrice) : ResourceUI.NotGiven);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitor quote attached:", (model.CompetitorQuoteFileName != null) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Competitors line By line comparison file attached:", (model.CompetitorLineComparsionFileName != null) ? ResourceUI.Yes : ResourceUI.No);

            if (showCostPricing == true)
            {
                row++;

                r = worksheet.CreateRow(row++);
                SetCellValue(worksheet, r, 0, "Rep/distributor and Daikin information and costing for opportunity".ToUpper(), true);
                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total net price based on standard multipliers", "");

                row++;

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total Listed Value Of This Project Offering", string.Format("{0:C}", model.StandardTotals.TotalList));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total Net", string.Format("{0:C}", model.StandardTotals.TotalNet));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Freight Costs", string.Format("{0:C}", model.Quote.TotalFreight));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Start up / Commissioning costs", model.StartUpCosts.ToString("0.00"));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Rep/Distributor Gross Profit on Opportunity", model.StartUpCosts.ToString("0.00") + "/" + string.Format("{0:C}", model.StandardTotals.TotalCommissionAmount));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total standard sales value of this opportunity from Rep/Dist to customer", string.Format("{0:C}", model.StandardTotals.TotalSell + model.StartUpCosts + model.Quote.TotalFreight));

                row++;

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Opportunity discount amount requested", "");

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total net price based on requested discounts", "");

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Discount being requested for this opportunity", string.Format("{0}%", Math.Round(model.RequestedDiscount, 2)));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Net Multiplier", string.Format("{0:N3}", model.ApprovedTotals.NetMultiplier));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Net Material Value", string.Format("{0:C}", model.ApprovedTotals.NetMaterialValue));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Rep/Distributor revised gross profit on opportunity", (long)model.RequestedCommission);

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Total revised Rep/Dist selling price as a result of Discount", string.Format("{0:C}", model.ApprovedTotals.TotalSell));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Discount Amount", string.Format("{0:C}", model.ApprovedTotals.TotalDiscountAmount));

                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Revised Gross", string.Format("{0:C}", model.ApprovedTotals.TotalCommissionAmount));
            }

            row++;

            //deprecated
            //r = worksheet.CreateRow(row++);
            //SetLabelValue(worksheet, r, 0, 1, "Will approval of this request assure Rep/Dist of order", (model.ApprovalAssuresOrder) ? ResourceUI.Yes : ResourceUI.No);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Probability of close:", (model.ProbabilityOfCloseTypeDescription != null) ? model.ProbabilityOfCloseTypeDescription : "Not given");

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "If approved, order will be issued to Daikin on:", model.OrderPlannedFor);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Approximate delivery date for required equipment:", model.OrderDeliveryDate);

            r = worksheet.CreateRow(row++);
            SetLabelValue(worksheet, r, 0, 1, "Any further information or reason for discount request:", model.Notes);

            if (model.ResponseNotes != null)
            {
                r = worksheet.CreateRow(row++);
                SetLabelValue(worksheet, r, 0, 1, "Response from approval team", model.ResponseNotes);
            }

            workbook.Write(stream);

            return stream;
        }

        public ServiceResponse GetDiscountRequestModel(UserSessionModel admin, DiscountRequestModel req)
        {
            DiscountRequestModel model = null;

            if (req.DiscountRequestId.HasValue)
            {
                var query = from dis in this.Db.QueryDiscountRequestsViewable(admin)

                            join mod in this.Db.Users on dis.StatusTypeModifiedBy equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()

                            join project in this.Db.Projects on dis.ProjectId equals project.ProjectId

                            join quote in this.Db.Quotes on dis.QuoteId equals quote.QuoteId

                            join owner in this.Db.Users on project.OwnerId equals owner.UserId

                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId

                            where dis.DiscountRequestId == req.DiscountRequestId.Value

                            select new DiscountRequestModel
                            {
                                DiscountRequestId = dis.DiscountRequestId,

                                ProjectId = dis.ProjectId,

                                QuoteId = dis.QuoteId,

                                RequestedOn = dis.RequestedOn,

                                ProjectOwner = owner.FirstName + " " + owner.LastName,

                                ProjectOwnerId = owner.UserId,

                                BusinessId = owner.BusinessId,

                                BusinessName = business.BusinessName,

                                DiscountRequestStatusTypeId = dis.DiscountRequestStatusTypeId,
                                DiscountRequestStatusTypeDescription = dis.DiscountRequestStatusType.Description,

                                DiscountRequestStatusModifiedOn = dis.StatusTypeModifiedOn,

                                DiscountRequestStatusModifiedBy = mod.FirstName + " " + mod.LastName,

                                SystemBasisDesignTypeId = dis.SystemBasisDesignTypeId,
                                SystemBasisDesignTypeDescription = dis.SystemBasisDesignType.Description,

                                ZoneStrategyTypeId = dis.ZoneStrategyTypeId,
                                ZoneStrategyTypeDescription = dis.ZoneStrategyType.Description,

                                BrandSpecifiedTypeId = dis.BrandSpecifiedTypeId,
                                BrandSpecifiedTypeDescription = dis.BrandSpecifiedType.Description,

                                BrandApprovedTypeId = dis.BrandApprovedTypeId,
                                BrandApprovedTypeDescription = dis.BrandApprovedType.Description,

                                DaikinEquipmentAtAdvantageTypeId = dis.DaikinEquipmentAtAdvantageTypeId,
                                DaikinEquipmentAtAdvantageDescription = dis.DaikinEquipmentAtAdvantageType.Description,

                                ProbabilityOfCloseTypeId = dis.ProbabilityOfCloseTypeId,
                                ProbabilityOfCloseTypeDescription = dis.ProbablilityOfCloseType.Description,

                                HasCompetitorPrice = dis.HasCompetitorPrice,

                                CompetitorPrice = dis.CompetitorPrice,

                                HasCompetitorQuote = dis.HasCompetitorQuote,

                                CompetitorQuoteFileName = dis.CompetitorQuoteFileName,

                                StartUpCosts = dis.StartUpCosts,

                                RequestedDiscount = dis.RequestedDiscount,
                                ApprovedDiscount = dis.ApprovedDiscount ?? 0,

                                RequestedDiscountVRV = dis.RequestedDiscountVRV,
                                ApprovedDiscountVRV = dis.ApprovedDiscountVRV,

                                RequestedDiscountSplit = dis.RequestedDiscountSplit,
                                ApprovedDiscountSplit = dis.ApprovedDiscountSplit,

                                RequestedDiscountUnitary = ( dis.RequestedDiscountUnitary.HasValue ) ? dis.RequestedDiscountUnitary.Value : 0,
                                ApprovedDiscountUnitary = ( dis.ApprovedDiscountUnitary.HasValue ) ? dis.ApprovedDiscountUnitary.Value : 0,

                                RequestedDiscountLCPackage = (dis.RequestedDiscountLCPackage.HasValue) ? dis.RequestedDiscountLCPackage.Value : 0,
                                ApprovedDiscountLCPackage = (dis.ApprovedDiscountLCPackage.HasValue) ? dis.ApprovedDiscountLCPackage.Value : 0,

                                RequestedCommission = dis.RequestedCommission,

                                CompetitorLineComparsionFileName = dis.CompetitorLineComparsionFileName,

                                HasCompetitorLineComparsion = dis.HasCompetitorLineComparsion,

                                IsConfidentCompetitorQuote = dis.IsConfidentCompetitorQuote,

                                //deprecated
                                //ApprovalAssuresOrder = dis.ApprovalAssuresOrder,

                                OrderPlannedFor = dis.OrderPlannedFor,

                                OrderDeliveryDate = dis.OrderDeliveryDate,

                                Notes = dis.Notes,

                                ResponseNotes = dis.ResponseNotes,

                                Timestamp = dis.Timestamp,

                                ShouldSendEmail = req.ShouldSendEmail,

                                EmailsList = dis.EmailsList
                            };

                model = query.FirstOrDefault();
            }

            if (model == null)
            {
                model = new DiscountRequestModel
                {
                    DiscountRequestStatusTypeId = (byte)DiscountRequestStatusTypeEnum.NewRecord,
                    ProjectId = req.ProjectId,
                    QuoteId = req.QuoteId
                };
            }

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetDiscountRequestModel(UserSessionModel admin, long projectId, long quoteId)
        {
            DiscountRequestModel model = null;

            if (!string.IsNullOrEmpty(projectId.ToString()) && !string.IsNullOrEmpty(quoteId.ToString()))
            {
                var query = from dis in this.Db.QueryDiscountRequestsViewable(admin)

                            join mod in this.Db.Users on dis.StatusTypeModifiedBy equals mod.UserId into Lmod
                            from mod in Lmod.DefaultIfEmpty()

                            join project in this.Db.Projects on dis.ProjectId equals project.ProjectId
                            where project.ProjectId == projectId

                            join quote in this.Db.Quotes on dis.QuoteId equals quote.QuoteId
                            where quote.QuoteId == quoteId

                            join owner in this.Db.Users on project.OwnerId equals owner.UserId

                            join business in this.Db.Businesses on owner.BusinessId equals business.BusinessId

                            select new DiscountRequestModel
                            {
                                DiscountRequestId = dis.DiscountRequestId,

                                ProjectId = projectId,

                                QuoteId = quoteId,

                                RequestedOn = dis.RequestedOn,

                                ProjectOwner = owner.FirstName + " " + owner.LastName,

                                ProjectOwnerId = owner.UserId,

                                BusinessId = owner.BusinessId,

                                BusinessName = business.BusinessName,

                                DiscountRequestStatusTypeId = dis.DiscountRequestStatusTypeId,
                                DiscountRequestStatusTypeDescription = dis.DiscountRequestStatusType.Description,

                                DiscountRequestStatusModifiedOn = dis.StatusTypeModifiedOn,

                                DiscountRequestStatusModifiedBy = mod.FirstName + " " + mod.LastName,

                                SystemBasisDesignTypeId = dis.SystemBasisDesignTypeId,
                                SystemBasisDesignTypeDescription = dis.SystemBasisDesignType.Description,

                                ZoneStrategyTypeId = dis.ZoneStrategyTypeId,
                                ZoneStrategyTypeDescription = dis.ZoneStrategyType.Description,

                                BrandSpecifiedTypeId = dis.BrandSpecifiedTypeId,
                                BrandSpecifiedTypeDescription = dis.BrandSpecifiedType.Description,

                                BrandApprovedTypeId = dis.BrandApprovedTypeId,
                                BrandApprovedTypeDescription = dis.BrandApprovedType.Description,

                                DaikinEquipmentAtAdvantageTypeId = dis.DaikinEquipmentAtAdvantageTypeId,
                                DaikinEquipmentAtAdvantageDescription = dis.DaikinEquipmentAtAdvantageType.Description,

                                ProbabilityOfCloseTypeId = dis.ProbabilityOfCloseTypeId,
                                ProbabilityOfCloseTypeDescription = dis.ProbablilityOfCloseType.Description,

                                HasCompetitorPrice = dis.HasCompetitorPrice,

                                CompetitorPrice = dis.CompetitorPrice,

                                HasCompetitorQuote = dis.HasCompetitorQuote,

                                CompetitorQuoteFileName = dis.CompetitorQuoteFileName,

                                StartUpCosts = dis.StartUpCosts,

                                RequestedDiscount = dis.RequestedDiscount,
                                ApprovedDiscount = dis.ApprovedDiscount ?? 0,

                                RequestedDiscountVRV = dis.RequestedDiscountVRV,
                                ApprovedDiscountVRV = dis.ApprovedDiscountVRV,

                                RequestedDiscountSplit = dis.RequestedDiscountSplit,
                                ApprovedDiscountSplit = dis.ApprovedDiscountSplit,

                                RequestedDiscountUnitary = ( dis.RequestedDiscountUnitary.HasValue ) ? dis.RequestedDiscountUnitary.Value : 0,
                                ApprovedDiscountUnitary = ( dis.ApprovedDiscountUnitary.HasValue ) ? dis.ApprovedDiscountUnitary.Value : 0,

                                RequestedDiscountLCPackage = (dis.RequestedDiscountLCPackage.HasValue) ? dis.RequestedDiscountLCPackage.Value : 0,
                                ApprovedDiscountLCPackage = (dis.ApprovedDiscountLCPackage.HasValue) ? dis.ApprovedDiscountLCPackage.Value : 0,

                                RequestedCommission = dis.RequestedCommission,

                                CompetitorLineComparsionFileName = dis.CompetitorLineComparsionFileName,

                                HasCompetitorLineComparsion = dis.HasCompetitorLineComparsion,

                                IsConfidentCompetitorQuote = dis.IsConfidentCompetitorQuote,

                                OrderPlannedFor = dis.OrderPlannedFor,

                                OrderDeliveryDate = dis.OrderDeliveryDate,

                                Notes = dis.Notes,

                                ResponseNotes = dis.ResponseNotes,

                                Timestamp = dis.Timestamp,

                                EmailsList = dis.EmailsList
                            };

                model = query.FirstOrDefault();
            }

            if (model == null)
            {
                model = new DiscountRequestModel
                {
                    DiscountRequestStatusTypeId = (byte)DiscountRequestStatusTypeEnum.NewRecord,
                    ProjectId = projectId,
                    QuoteId = quoteId
                };
            }

            FinaliseModel(this.Response.Messages, admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        #endregion

        #region Finalise Model

        public void FinaliseModel(Messages messages, UserSessionModel admin, DiscountRequestModel model)
        {
            if (model.ProjectId.HasValue && model.QuoteId.HasValue)
            {
                var projectQuery = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)

                                   join quote in this.Db.Quotes on new { id = project.ProjectId, qId = model.QuoteId.Value } equals new { id = quote.ProjectId, qId = quote.QuoteId } into Laq
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
                                       ProjectStatusNotes = project.ProjectStatusNotes,

                                       EngineerName = project.EngineerName,
                                       EngineerBusinessName = project.EngineerBusinessName,

                                       DealerContractorName = project.DealerContractorName,
                                       CustomerName = project.CustomerName,// this is Dealer/Contractor BusinessName

                                       SellerName = project.SellerName,
                                       ShipToName = project.ShipToName,
                                       

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
                                           TotalListUnitary = (quote == null) ? 0 : 
                                                              ( quote.TotalListUnitary.HasValue ) ? quote.TotalListUnitary.Value : 0,
                                           TotalListLCPackage = (quote == null) ? 0 :
                                                              (quote.TotalListLCPackage.HasValue) ? quote.TotalListLCPackage.Value : 0,
                                           TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                           TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                           TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                           TotalSellSplit = (quote == null) ? 0 : quote.TotalSellSplit,
                                           TotalSellVRV = (quote == null) ? 0 : quote.TotalSellVRV,
                                           TotalSellUnitary = (quote == null) ? 0 : 
                                                              (quote.TotalSellUnitary.HasValue) ? quote.TotalSellUnitary : 0,
                                           TotalSellLCPackage = (quote == null) ? 0 :
                                                              (quote.TotalSellLCPackage.HasValue) ? quote.TotalSellLCPackage : 0,
                                           TotalCountSplit = (quote == null) ? 0 : quote.TotalCountSplit,
                                           TotalCountVRV = (quote == null) ? 0 : quote.TotalCountVRV,
                                           TotalCountVRVIndoor = (quote == null) ? 0 : quote.TotalCountVRVIndoor,
                                           TotalCountVRVOutdoor = (quote == null) ? 0 : quote.TotalCountVRVOutdoor,
                                           ApprovedCommissionPercentage = (quote == null) ? 0 : quote.ApprovedCommissionPercentage,
                                           ApprovedDiscountPercentage = (quote == null) ? 0 : quote.ApprovedDiscountPercentage,
                                           ApprovedDiscountPercentageSplit = (quote == null) ? 0 : quote.ApprovedDiscountPercentageSplit,
                                           ApprovedDiscountPercentageVRV = (quote == null) ? 0 : quote.ApprovedDiscountPercentageVRV,
                                           ApprovedDiscountPercentageUnitary = (quote == null) ? 0 : 
                                                                               ( quote.ApprovedDiscountPercentageUnitary.HasValue ) ? 
                                                                               quote.ApprovedDiscountPercentageUnitary.Value : 0,
                                           ApprovedDiscountPercentageLCPackage = (quote == null) ? 0 :
                                                                               (quote.ApprovedDiscountPercentageLCPackage.HasValue) ?
                                                                               quote.ApprovedDiscountPercentageLCPackage.Value : 0,
                                           TotalNetCommission = (quote == null) ? 0 : quote.TotalNetCommission,
                                           TotalNetNonCommission = (quote == null) ? 0 : quote.TotalNetNonCommission,
                                           TotalNetSplit = (quote == null) ? 0 : quote.TotalNetSplit,
                                           TotalNetVRV = (quote == null) ? 0 : quote.TotalNetVRV,
                                           TotalNetUnitary = (quote == null ) ? 0: 
                                                             ( quote.TotalNetUnitary.HasValue ) ? quote.TotalNetUnitary.Value : 0,
                                           TotalNetLCPackage = (quote == null) ? 0 :
                                                             (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0,
                                           IsGrossMargin = (quote == null) ? false : quote.IsGrossMargin,
                                           TotalFreight = (quote == null) ? 0 : quote.TotalFreight,
                                           DiscountPercentage = (quote == null) ? 0 : quote.DiscountPercentage,
                                           CommissionPercentage = (quote == null) ? 0 : quote.CommissionPercentage,
                                           Revision = (quote == null) ? 0 : quote.Revision
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

                if (model.Project == null)
                {
                    this.Response.AddError(Resources.DataMessages.DM007);
                    return;
                }

                var addressService = new AddressServices(this.Context);
                model.Project.SellerAddress = addressService.GetAddressModel(admin, model.Project.SellerAddress);
                model.Project.CustomerAddress = addressService.GetAddressModel(admin, model.Project.CustomerAddress);
                model.Project.EngineerAddress = addressService.GetAddressModel(admin, model.Project.EngineerAddress);
                model.Project.ShipToAddress = addressService.GetAddressModel(admin, model.Project.ShipToAddress);

                if (model.Project.CustomerName == null )
                {
                    model.Project.CustomerName = this.Db.Context.Projects.Where(p => p.ProjectId == model.ProjectId).Select(p => p.CustomerName).FirstOrDefault();
                }

                model.QuoteItems = new QuoteServices(this.Context).GetQuoteItemListModel(admin, (long)model.QuoteId).Model as List<QuoteItemListModel>;

            }

            var service = new QuoteServices();

            if (model.ApprovedDiscount > 0)
            {
                model.ApprovedTotals = service.CalculateTotalDiscountsApproved(model.Quote, model.ApprovedDiscount,
                    model.ApprovedDiscountSplit, model.ApprovedDiscountVRV, model.ApprovedDiscountUnitary, model.ApprovedDiscountLCPackage, model.RequestedCommission);
            }
            else
            {
                model.ApprovedTotals = service.CalculateTotalDiscountsApproved(model.Quote, model.RequestedDiscount,
                    model.RequestedDiscountSplit, model.RequestedDiscountVRV, model.RequestedDiscountUnitary, model.RequestedDiscountLCPackage, model.RequestedCommission);
            }

            model.StandardTotals = service.CalculateTotalStandard(model.Quote);

            model.RequestedDiscount *= 100M; 
            
            model.RequestedDiscountSplit *= 100M;
            model.RequestedDiscountVRV *= 100M;
            model.RequestedDiscountUnitary *= 100M;
            model.RequestedDiscountLCPackage *= 100M;

            model.ApprovedDiscount *= 100M;
            
            model.ApprovedDiscountSplit *= 100M;
            model.ApprovedDiscountVRV *= 100M;
            model.ApprovedDiscountUnitary *= 100M;
            model.ApprovedDiscountLCPackage *= 100M;

            model.RequestedCommission *= 100;

            // Dropdowns 

            model.DiscountRequestStatusTypes = htmlService.DropDownModelDiscountRequestStatusTypes((model == null) ? null : model.DiscountRequestStatusTypeId);

            model.SystemBasisDesignTypes = htmlService.DropDownModelSystemBasisDesignTypes((model == null) ? null : model.SystemBasisDesignTypeId);

            model.ZoneStrategyTypes = htmlService.DropDownModelZoneStrategyTypes((model == null) ? null : model.ZoneStrategyTypeId);

            model.BrandSpecifiedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandSpecifiedTypeId);

            model.BrandApprovedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandApprovedTypeId);

            model.DaikinEquipmentAtAdvantageTypes = htmlService.DropDownModelDaikinEquipmentAtAdvantageTypes((model == null) ? null : model.DaikinEquipmentAtAdvantageTypeId);

            model.ProbabilityOfCloseTypes = htmlService.DropDownModelProbabilityOfCloseTypes((model == null) ? null : model.ProbabilityOfCloseTypeId);

        }

        #endregion

        #region Post Requests

        public ServiceResponse Delete(UserSessionModel user, DiscountRequestModel model)
        {
            return ChangeStatus(user, model, DiscountRequestStatusTypeEnum.Deleted);
        }

        public ServiceResponse Approve(UserSessionModel user, DiscountRequestModel model)
        {
            return ChangeStatus(user, model, DiscountRequestStatusTypeEnum.Approved);
        }

        public ServiceResponse Reject(UserSessionModel user, DiscountRequestModel model)
        {
            return ChangeStatus(user, model, DiscountRequestStatusTypeEnum.Rejected);
        }

        private ServiceResponse ChangeStatus(UserSessionModel user, DiscountRequestModel model, DiscountRequestStatusTypeEnum status)
        {
            this.Db.ReadOnly = false;

            var entity = GetEntity(user, model);

            if (this.Response.IsOK)
            {
                entity.DiscountRequestStatusTypeId = (byte)status;

                if (status == DiscountRequestStatusTypeEnum.Approved ||
                    status == DiscountRequestStatusTypeEnum.Rejected)
                {
                    entity.ResponseNotes = model.ResponseNotes;
                    entity.ApprovedDiscountVRV = model.ApprovedDiscountVRV / 100M;
                    entity.ApprovedDiscountSplit = model.ApprovedDiscountSplit/ 100M;
                    entity.ApprovedDiscountUnitary = model.ApprovedDiscountUnitary / 100M;
                    entity.ApprovedDiscountLCPackage = model.ApprovedDiscountLCPackage / 100M;
                    entity.ApprovedDiscount = model.ApprovedDiscount / 100M;
                    entity.EmailsList = model.EmailsList;
                }

                Entry = Db.Entry(entity);

                RulesOnEdit(user, entity);
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, "Discount Request");
            }

            var newModel = GetDiscountRequestModel(user, model).Model as DiscountRequestModel;

            this.Response.Model = newModel;

            return this.Response;
        }

        public ServiceResponse PostModel(UserSessionModel admin, DiscountRequestModel model)
        {
            this.Db.ReadOnly = false;

            try
            {
                DiscountRequest entity = null;

                //======TODO: will be removed============

                model.RequestedDiscount /= 100M;
                model.RequestedDiscountSplit /= 100M;
                model.RequestedDiscountVRV /= 100M;
                model.RequestedDiscountUnitary /= 100M;
                model.RequestedDiscountLCPackage /= 100M;

                model.ApprovedDiscount /= 100M;
                model.ApprovedDiscountSplit /= 100M;
                model.ApprovedDiscountVRV /= 100M;
                model.ApprovedDiscountUnitary /= 100M;
                model.ApprovedDiscountLCPackage /= 100M;

                model.RequestedCommission /= 100M;
                //=========================================


                // Validate Model 
                RulesOnValidateModel(model);

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

                    ApplyBusinessRules(admin, entity);
                }

                if (this.Response.IsOK)
                {
                    SaveToDatabase(model, entity, string.Format("Discount Request '0'", ""));
                }

            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            FinaliseModel(this.Response.Messages, admin, model);// Why do we need to do this after data has been saved?

            this.Response.Model = model;

            return this.Response;
        }

        #endregion

        #region Post Model To Entity

        private DiscountRequest ModelToEntity(UserSessionModel admin, DiscountRequestModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.SystemBasisDesignTypeId = model.SystemBasisDesignTypeId;
            entity.ZoneStrategyTypeId = model.ZoneStrategyTypeId;
            entity.BrandSpecifiedTypeId = model.BrandSpecifiedTypeId;
            entity.BrandApprovedTypeId = model.BrandApprovedTypeId;
            entity.DaikinEquipmentAtAdvantageTypeId = model.DaikinEquipmentAtAdvantageTypeId;
            entity.ProbabilityOfCloseTypeId = model.ProbabilityOfCloseTypeId;

            entity.HasCompetitorPrice = model.HasCompetitorPrice;

            entity.CompetitorPrice = model.CompetitorPrice;

            entity.HasCompetitorQuote = model.HasCompetitorQuote;

            entity.CompetitorQuoteFileName = model.CompetitorQuoteFileName;

            entity.HasCompetitorLineComparsion = model.HasCompetitorLineComparsion;

            entity.CompetitorLineComparsionFileName = model.CompetitorLineComparsionFileName;

            entity.IsConfidentCompetitorQuote = model.IsConfidentCompetitorQuote;

            //deprecated
            //entity.ApprovalAssuresOrder = model.ApprovalAssuresOrder;

            entity.StartUpCosts = model.StartUpCosts;

            entity.OrderPlannedFor = model.OrderPlannedFor;

            entity.OrderDeliveryDate = model.OrderDeliveryDate;

            entity.RequestedDiscount = model.RequestedDiscount;
            entity.RequestedDiscountSplit = model.RequestedDiscountSplit;
            entity.RequestedDiscountVRV = model.RequestedDiscountVRV;
            entity.RequestedDiscountUnitary = model.RequestedDiscountUnitary;
            entity.RequestedDiscountLCPackage = model.RequestedDiscountLCPackage;

            entity.RequestedCommission = model.RequestedCommission;

            entity.ApprovedDiscount = model.ApprovedDiscount;
            entity.ApprovedDiscountSplit = model.ApprovedDiscountSplit;
            entity.ApprovedDiscountVRV = model.ApprovedDiscountVRV;
            entity.ApprovedDiscountUnitary = model.ApprovedDiscountUnitary;
            entity.ApprovedDiscountLCPackage = model.ApprovedDiscountLCPackage;

            entity.Notes = model.Notes;

            if (admin.HasAccess(SystemAccessEnum.ApproveDiscounts))
            {
                entity.ResponseNotes = model.ResponseNotes;
            }

            if (model.EmailsList != null)
            {
                entity.EmailsList = model.EmailsList;
            }

            //entity.Name = Utilities.Trim(model.Name);

            //entity.CustomerName = Utilities.Trim(model.CustomerName);

            //entity.EngineerName = Utilities.Trim(model.EngineerName);

            //entity.SellerName = Utilities.Trim(model.SellerName);

            //entity.ShipToName = Utilities.Trim(model.ShipToName);

            //entity.Description = Utilities.Trim(model.Description);

            //entity.ProjectDate = model.ProjectDate ?? DateTime.UtcNow;

            //entity.BidDate = model.BidDate ?? DateTime.UtcNow; 

            //entity.EstimatedClose = model.EstimatedClose ?? DateTime.UtcNow; 

            //entity.EstimatedDelivery = model.EstimatedDelivery ?? DateTime.UtcNow;

            //entity.Expiration = model.Expiration ?? DateTime.UtcNow;

            //entity.ProjectOpenStatusTypeId = model.ProjectOpenStatusTypeId.Value;

            //entity.ProjectTypeId = model.ProjectTypeId.Value;

            //entity.ConstructionTypeId = model.ConstructionTypeId.Value;

            //entity.ProjectStatusTypeId = (ProjectStatusTypeEnum)model.ProjectStatusTypeId.Value;

            //entity.VerticalMarketTypeId = model.VerticalMarketTypeId.Value;

            //entity.EngineerAddress = new AddressServices(this, "EngineerAddress").ModelToEntity(model.EngineerAddress);
            //this.Response.PropertyReference = "";

            //entity.CustomerAddress = new AddressServices(this,"CustomerAddress").ModelToEntity(model.CustomerAddress);
            //this.Response.PropertyReference = "";

            //entity.SellerAddress = new AddressServices(this, "SellerAddress").ModelToEntity(model.SellerAddress);
            //this.Response.PropertyReference = "";

            //entity.ShipToAddress = new AddressServices(this, "ShipToAddress").ModelToEntity(model.ShipToAddress);
            //this.Response.PropertyReference = "";

            ModelToEntityConcurrenyProcessing(entity as IConcurrency, model as IConcurrency);

            return entity;
        }

        #endregion

        private DiscountRequest GetEntity(UserSessionModel user, DiscountRequestModel model)
        {
            var entity = model.DiscountRequestId.HasValue ?
                this.Db.QueryDiscountRequestsViewable(user).Where(d => d.DiscountRequestId == model.DiscountRequestId.Value).FirstOrDefault()
                :
                Db.DiscountRequestCreate(model.ProjectId.Value, model.QuoteId.Value);

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

    }

}