
using DPO.Common;
using DPO.Data;
using DPO.Domain.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using log4net;

namespace DPO.Domain
{

    public partial class QuoteServices
    {
        // #################################################
        // Rules for model validation
        // #################################################

        public void RulesOnValidateModel(ModelStateDictionary modelState, QuoteModel model)
        {
            Log.InfoFormat("Enter RulesOnValidateModel for {0}", model.GetType());
            Log.DebugFormat("QuoteId: {0} ProjectId: {1} ", model.QuoteId, model.ProjectId);

            bool returnValue = false;

            Log.DebugFormat("Start validate QuoteModel's title. Title: {0}", model.Title);
            returnValue = Validation.IsText(this.Response.Messages, model.Title, "Title", "Quote title", 255, true);
            Log.DebugFormat("Title validation: {0}", returnValue);

            Log.DebugFormat("Start validate QuoteModel's Description. Description: {0}", model.Description);
            returnValue = Validation.IsText(this.Response.Messages, model.Description, "Description", "Description", 2, 1000, false);
            Log.DebugFormat("Description validation: {0}", returnValue);

            Log.DebugFormat("Start validate QuoteModel's Notes: {0}", model.Notes);
            returnValue = Validation.IsText(this.Response.Messages, model.Notes, "Notes", "Notes", 2000, false);
            Log.DebugFormat("Notes validation: {0}", returnValue);

            if (modelState != null) // Test cases dont supply modelstate
            {
                Log.DebugFormat("Start validate TotalFreight");
                if (modelState["TotalFreight"] != null)
                    returnValue = Validation.IsDecimal(this.Response.Messages, modelState["TotalFreight"].Value.AttemptedValue, "TotalFreight", "Freight cost", false);
                Log.DebugFormat("TotalFreight validation: {0}", returnValue);

                Log.DebugFormat("Start validate CommissionPercentage");
                if (modelState["CommissionPercentage"] != null)
                    returnValue = Validation.IsDecimal(this.Response.Messages, modelState["CommissionPercentage"].Value.AttemptedValue, "CommissionPercentage", "Pricing Percentage", false);
                Log.DebugFormat("CommissionPercentage validation: {0}", returnValue);
            }

            //TODO: This should be modified.
            //Combine CommissionConvertYes/No into one
            // if model.IsCommission == false && model.CommissionConvert == true, then model.IsCommission = true;
            if (model.CommissionConvertNo == false && model.IsCommission == true && model.CommissionConvertYes == true)
            {
                model.IsCommission = true;
                Log.DebugFormat("IsCommission: {0}", model.IsCommission);
            }

            Log.InfoFormat("RulesOnValidateModel finished.");
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel user, object entity)
        {
            Log.InfoFormat("Enter RulesOnAdd for entity: {0}", entity.GetType());

            var quote = entity as Quote;

            Log.DebugFormat("QuoteId: {0} ProjectId: {1}", quote.QuoteId, quote.ProjectId);

            Log.DebugFormat("Start calling RuleOnCommon");
            RulesCommon(user, Db, quote);

            // Get next available version
            Log.DebugFormat("Start get Quote Revision");
            quote.Revision = Db.QuoteGetNextVersionNumber(quote.ProjectId);

            // If the project is new and this is the first quote make it the active quote
            // reflect that on the project too
            if (quote.Project.ActiveVersion == 0)
            {
                Log.DebugFormat("this is the Only Quote in the project.set the quote to Active");
                quote.Project.ActiveVersion = quote.Revision;

                quote.Active = true;

                Log.DebugFormat("quote Active: {0}", quote.Active);
            }

            Log.InfoFormat("RulesOnAdd finished.");

        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel user, object entity)
        {
            Log.InfoFormat("Enter RulesOnEdit for enitity: {0}", entity.GetType());
            var quote = entity as Quote;

            //Commented out the method call below on 7/28/2015 as per requested by Depaak so that the user 
            //can always change the active flag regardless of if there has been a DAR requested or not.
            //DiscountRequestRules(user, Db, quote);

            Log.DebugFormat("Start calling RulesCommon");
            RulesCommon(user, Db, quote);

            Log.DebugFormat("Quote ActiveVerison: {0}", quote.Revision);
            Log.DebugFormat("Project ActiveVersion: {0}", quote.Project.ActiveVersion);

            if (quote.Active == true && base.Entry.HasChanged("Active"))
            {
                // Make sure the current active project version is different

                if (quote.Project.ActiveVersion != quote.Revision)
                {
                    // Get the current active quote and deactivate it
                    Log.DebugFormat("getting current active quote");
                    var currentActiveQuote = Db.QuoteGetCurrentActiveQuote(user, quote.ProjectId).FirstOrDefault();

                    Log.DebugFormat("deactive current Active quote");
                    currentActiveQuote.Active = false;

                    // Set this quote as active.
                    Log.DebugFormat("Set new Quote to Active");
                    quote.Project.ActiveVersion = quote.Revision;

                    quote.Active = true;

                    Log.DebugFormat("Quote Active: {0}", quote.Active);
                }
            }

            Log.InfoFormat("RulesOnEdit finished.");
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel user, object entity)
        {
            Log.InfoFormat("Enter RulesOnDelete for enitity: {0}", entity.GetType());

            var quote = entity as Quote;

            Log.DebugFormat("QuoteId: {0}, ProjectId: {1}", quote.QuoteId, quote.ProjectId);

            if (quote == null)
            {
                Log.ErrorFormat("Quote not loaded. Quote entity is Null");
                throw new ArgumentException("Quote entity not loaded");
            }

            // Can only soft delete if quote is not active ?
            if (quote.Active == true)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP007);
                Log.ErrorFormat(this.Response.Messages.Items.Last().Text);
                return;
            }

            quote.Deleted = true;
            Log.DebugFormat("quote set to deleted: {0}", quote.Deleted);

            Log.Debug("setting Entity State to 'Modified'");
            this.Entry.State = EntityState.Modified;

            Log.InfoFormat("RulesOnDelete finished.");
        }


        // #################################################
        // Discount Request Rules
        // #################################################
        private void DiscountRequestRules(UserSessionModel admin, Repository db, Quote quote)
        {
            Log.InfoFormat("Enter DiscountRequestRules for entity: {0}", quote.GetType());
            Log.DebugFormat("QuoteId: {0}, ProjectId: {1}", quote.QuoteId, quote.ProjectId);
            Log.DebugFormat("User: {0}", admin.Email);

            if (Entry.HasChanged("DiscountRequestId") == false &&
                Entry.HasChanged("AwaitingDiscountRequest") == false &&
                (quote.AwaitingDiscountRequest || quote.DiscountRequestId != null))
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP028);
                Log.Error(this.Response.Messages.Items.Last().Text);
            }

            if (Entry.HasChanged("DiscountRequestId"))
            {
                if (quote.DiscountRequestId == null)
                {
                    Log.Debug("Entry has change tracking on DiscountRequestId.");
                    Log.Debug("Quote has DiscountRequestId equals Null");
                    Log.Debug("Setting ApprovedCommissionPercentage and ApprovedDiscountPercentage to zero");

                    quote.ApprovedCommissionPercentage = 0;
                    quote.ApprovedDiscountPercentage = 0;
                }

                Log.Debug("Setting AwaitingDiscountRequest to False");
                quote.AwaitingDiscountRequest = false;
            }

            Log.Info("DiscountRequestRules finished");
        }

        // #################################################
        // Commission Request Rules
        // #################################################
        private void CommissionRequestRules(UserSessionModel admin, Repository db, Quote quote)
        {
            Log.InfoFormat("Enter CommissionRequestRules for entity: {0}", quote.GetType());
            Log.DebugFormat("QuoteId: {0}, ProjectId: {1}", quote.QuoteId, quote.ProjectId);
            Log.DebugFormat("User: {0}", admin.Email);

            if (Entry.HasChanged("CommissionRequestId") == false &&
                Entry.HasChanged("AwaitingCommissionRequest") == false &&
                (quote.AwaitingCommissionRequest || quote.CommissionRequestId != null))
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP121);
                Log.Error(this.Response.Messages.Items.Last().Text);
            }

            if (Entry.HasChanged("CommissionRequestId"))
            {
                Log.Debug("Entry has change tracking on CommissionRequestId");

                if (quote.CommissionRequestId == null)
                {
                    Log.Debug("quote has CommissionRequestId equals Null");
                    Log.Debug("Setting ApprovedCommissionPercentage t and ApprovedDiscountPercentage to Zero");

                    quote.ApprovedCommissionPercentage = 0;
                    quote.ApprovedDiscountPercentage = 0;
                }

                Log.Debug("Setting AwaitingCommissionRequest to False");
                quote.AwaitingCommissionRequest = false;
            }

            Log.Info("CommissionRequestRules finished");
        }

        // #################################################
        // Rules Common
        // #################################################
        private void RulesCommon(UserSessionModel admin, Repository db, Quote quote)
        {
            Log.InfoFormat("Enter RulesCommon for entity: {0}", quote.GetType());
            Log.DebugFormat("QuoteId: {0}, ProjectId: {1}", quote.QuoteId, quote.ProjectId);
            Log.DebugFormat("User: {0}", admin.Email);

            //mass upload change - turned this off
            if (quote == null)
            {
                Log.Error(this.Response.Messages.Items.Last().Text);
                throw new ArgumentException("Quote entity not loaded");
            }

            if (quote.Project == null)
            {
                Log.DebugFormat("Project is null.Start loading Project by Id: {0}", quote.ProjectId);

                quote.Project = Db.GetProjectByProjectId(quote.ProjectId);

                if (quote.Project == null)
                {
                    Log.Error("Project equals to Null");
                    Log.ErrorFormat("Can not load Project entity for ProjectId: {0}", quote.ProjectId);

                    throw new ArgumentException("Quote project entity not loaded");
                }
            }

            if (quote.Project.Owner == null)
            {
                Log.DebugFormat("ProjectOwner is Null.Start loading ProjectOwner for ProjectId: {0}", quote.ProjectId);

                quote.Project = Db.GetProjectOwnerAndBusiness(quote.Project.ProjectId);

                if (quote.Project.Owner == null)
                {
                    Log.Error("ProjectOwner is Null.");
                    Log.ErrorFormat("Can not load ProjectOwner for ProjectId: {0}", quote.ProjectId);

                    throw new ArgumentException("Quote owner entity not loaded");
                }
            }

            if (quote.Project.Owner.Business == null)
            {
                if (quote.Project.Owner.BusinessId != null)
                {
                    Log.Debug("ProjectOwner's Business is Null");
                    Log.Debug("ProjectOwner's ProjectId is Not Null");
                    Log.DebugFormat("Start loading ProjectOwner's Business for BusinessId: {0}", quote.Project.Owner.BusinessId);

                    var business = this.Db.Context.Businesses
                                   .Where(b => b.BusinessId == quote.Project.Owner.BusinessId)
                                   .FirstOrDefault();

                    quote.Project.Owner.Business = business;

                }
                else
                {
                    throw new ArgumentException("Quote owner business entity not loaded");
                }
            }

            //###############################################################
            // Make sure nothing can change unless the project status is open
            //###############################################################
            if (quote.Project.ProjectStatusTypeId != ProjectStatusTypeEnum.Open)
            {
                if (Entry.CurrentValues.PropertyNames.Any(p => Entry.Property(p).IsModified))
                {
                    this.Response.Messages.AddError("ProjectStatusTypeId", Resources.ResourceModelProject.MP020);
                }
            }

            var ids = quote.QuoteItems.Select(i => i.QuoteItemId).ToArray();

            Db.QuoteItemsByQuoteId(admin, quote.QuoteId).Where(i => !ids.Contains(i.QuoteItemId)).Load();

            var items = quote.QuoteItems.ToList();

            RulesToRefreshQuoteItemProductsAndPrices(quote, items);

            RulesToCalculateQuoteTotals(admin, quote, items);

            quote.RecalculationRequired = false;
        }

        public void CommissionRecalculate(UserSessionModel user, Quote quote)
        {
            CommissionRequestServices commissionRequestService = new CommissionRequestServices();

            ServiceResponse respone = commissionRequestService.GetCommissionRequestModel(user, new CommissionRequestModel { ProjectId = quote.ProjectId, QuoteId = quote.QuoteId, CommissionRequestId = quote.CommissionRequestId }, null);

            CommissionRequestModel commissionRequest = respone.Model as CommissionRequestModel;

            #region VRV Commission

            // VRV Commission Calculation
            commissionRequest.TotalNetVRV = quote.TotalListVRV * commissionRequest.RequestedMultiplierVRV;
            commissionRequest.RequestedCommissionVRV = commissionRequest.TotalNetVRV * (commissionRequest.RequestedCommissionPercentageVRV / 100);

            // VRV Net Material Cost
            commissionRequest.RequestedNetMaterialValueVRV = commissionRequest.TotalNetVRV - commissionRequest.RequestedCommissionVRV;

            if (quote.TotalListVRV == Decimal.Zero)
            {
                commissionRequest.RequestedNetMaterialMultiplierVRV = commissionRequest.RequestedNetMaterialValueVRV;
            }
            else
            {
                commissionRequest.RequestedNetMaterialMultiplierVRV = commissionRequest.RequestedNetMaterialValueVRV / quote.TotalListVRV;
            }

            #endregion VRV Commission

            #region Split Commission

            // Split Commission Requested
            commissionRequest.TotalNetSplit = quote.TotalListSplit * commissionRequest.RequestedMultiplierSplit;
            commissionRequest.RequestedCommissionSplit = commissionRequest.TotalNetSplit * (commissionRequest.RequestedCommissionPercentageSplit / 100);

            // Split Net Material Cost
            commissionRequest.RequestedNetMaterialValueSplit = commissionRequest.TotalNetSplit - commissionRequest.RequestedCommissionSplit;

            if (quote.TotalListSplit == Decimal.Zero)
            {
                commissionRequest.RequestedNetMaterialMultiplierSplit = commissionRequest.RequestedNetMaterialValueSplit;
            }
            else
            {
                commissionRequest.RequestedNetMaterialMultiplierSplit = commissionRequest.RequestedNetMaterialValueSplit / quote.TotalListSplit;
            }

            #endregion Split Commission

            #region Unitary Commission

            // Unitary Commission Requested
            commissionRequest.TotalNetUnitary = quote.TotalListUnitary.Value * commissionRequest.RequestedMultiplierUnitary;
            commissionRequest.RequestedCommissionUnitary = commissionRequest.TotalNetUnitary * (commissionRequest.RequestedCommissionPercentageUnitary / 100);

            // Unitary Net Material Cost
            commissionRequest.RequestedNetMaterialValueUnitary = commissionRequest.TotalNetUnitary - commissionRequest.RequestedCommissionUnitary;

            if (quote.TotalListUnitary == Decimal.Zero)
            {
                commissionRequest.RequestedNetMaterialMultiplierUnitary = commissionRequest.RequestedNetMaterialValueUnitary;
            }
            else
            {
                commissionRequest.RequestedNetMaterialMultiplierUnitary = commissionRequest.RequestedNetMaterialValueUnitary / quote.TotalListUnitary.Value;
            }

            #endregion Unitary Commission


            #region LCPackage Commission

            // LCPackage Commission Requested
            commissionRequest.TotalNetLCPackage = quote.TotalListLCPackage.Value * commissionRequest.RequestedMultiplierLCPackage;
            commissionRequest.RequestedCommissionLCPackage = commissionRequest.TotalNetLCPackage * (commissionRequest.RequestedCommissionPercentageLCPackage / 100);

            // LCPackage Net Material Cost
            commissionRequest.RequestedNetMaterialValueLCPackage = commissionRequest.TotalNetLCPackage - commissionRequest.RequestedCommissionLCPackage;

            if (quote.TotalListUnitary == Decimal.Zero)
            {
                commissionRequest.RequestedNetMaterialMultiplierLCPackage = commissionRequest.RequestedNetMaterialValueLCPackage;
            }
            else
            {
                commissionRequest.RequestedNetMaterialMultiplierLCPackage = commissionRequest.RequestedNetMaterialValueLCPackage / quote.TotalListLCPackage.Value;
            }

            #endregion LCPackage Commission

            // Requested Commission Total
            commissionRequest.RequestedCommissionTotal = commissionRequest.RequestedCommissionVRV + commissionRequest.RequestedCommissionSplit + commissionRequest.RequestedCommissionUnitary + commissionRequest.RequestedCommissionLCPackage;

            // Total Net
            commissionRequest.TotalNet = commissionRequest.TotalNetSplit + commissionRequest.TotalNetVRV + commissionRequest.TotalNetUnitary + commissionRequest.TotalNetLCPackage;// commissionRequest.RequestedCommissionTotal + NetMaterialCostTotal;
            commissionRequest.RequestedCommissionPercentage = (commissionRequest.RequestedCommissionTotal / commissionRequest.TotalNet) * 100;

            // Net Material Calculations
            commissionRequest.RequestedNetMaterialValue = commissionRequest.TotalNet - commissionRequest.RequestedCommissionTotal;
            commissionRequest.RequestedNetMaterialValueMultiplier = commissionRequest.RequestedNetMaterialValue / (quote.TotalListSplit + quote.TotalListVRV + quote.TotalListUnitary.Value + quote.TotalListLCPackage.Value);

            // Total Net Multiplier
            commissionRequest.RequestedMultiplier = commissionRequest.TotalNet / (quote.TotalListSplit + quote.TotalListVRV + quote.TotalListUnitary.Value + quote.TotalListLCPackage.Value);

            CommissionRequest entity = commissionRequestService.GetEntity(user, commissionRequest);

            commissionRequest.IsCommissionCalculation = true;

            commissionRequestService.PostModel(user, commissionRequest);
        }


        // #################################################
        // Process new quote items
        // #################################################
        public void RulesToRefreshQuoteItemProductsAndPrices(Quote quote, List<QuoteItem> items)
        {
            // Load an missing products (i.e from newly added items) //TODO: looks like this is not used anywhere
            var productIds = items.Where(p => p.ProductId.HasValue && p.Product == null).Select(i => i.ProductId.Value).Distinct().ToArray();

            // Load all the products and fill and changed the missing data //TODO: looks like this is not used anywhere
            var products = Db.ProductByProductIds(productIds).ToList();

            foreach (var item in items)
            {
                if (item.ProductId.HasValue && item.Product == null)
                {
                    this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP021, item.Description));
                }
                else
                {
                    if (string.Compare(item.Description, item.Product.Name) != 0)
                    {
                        item.Description = item.Product.Name;
                    }
                    if (string.Compare(item.ProductNumber, item.Product.ProductNumber) != 0)
                    {
                        item.ProductNumber = item.Product.ProductNumber;
                    }
                    if (item.ListPrice != item.Product.ListPrice && item.LineItemTypeId != (byte?)LineItemTypeEnum.Configured)
                    {
                        item.ListPrice = item.Product.ListPrice;
                    }

                }
            }

            // Lookup commission request
            CommissionRequest commissionRequest = null;

            // Load the commission request for the quote
            if (quote.CommissionRequestId != null)
            {
                commissionRequest = this.Context.CommissionRequests
                    .Where(c => c.CommissionRequestId == quote.CommissionRequestId).FirstOrDefault();
            }
            else // Load the temporary commission request for calculation
            {
                commissionRequest = this.Context.CommissionRequests
                    .Where(c => c.QuoteId == quote.QuoteId
                        && c.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.NewRecord
                    ).FirstOrDefault();
            }

            SetQuoteItemMultipliers(quote, items, commissionRequest);
        }


        /// <summary>
        /// Identify and set quote item multipliers
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="items"></param>
        /// <param name="commissionRequest"></param>
        public void SetQuoteItemMultipliers(Quote quote, List<QuoteItem> items, CommissionRequest commissionRequest)
        {
            // TODO:  Where should this go.  Kinda iffy - Charles 06/08/2016
            // TODO:  I think this should go in commission request actually, have an idea for later.

            //If there are any systems then we need to load the system sub products
            var subProductIds = items.Where(i => i.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                                     .Select(p => p.ProductId.Value).ToArray();

            // TODO:  Shouldn't we do this another way?
            var multiplierTypes = this.Context.MultiplierTypes.Select(
                s => new
                {
                    s.MultiplierTypeId,
                    s.MultiplierTypesMultiplierCategoryTypes
                }
            ).ToDictionary(d => d.MultiplierTypeId);

            Dictionary<long, List<ProductAccessoryModel>> systemSubProductLookup = null;

            if (subProductIds.Length > 0)
            {
                Db.QueryProductSystemSubProductsByProductIds(subProductIds).Load();

                systemSubProductLookup = Db.GetProductSubProductModel(subProductIds);
            }

            // Multiplier processing for non commissionable products
            //var productClassCodes = Db.Context.Products.Local.Select(p => p.ProductClassCode).Distinct().ToArray();

            var productMultiplierTypeId = Db.Context.Products.Local.Select(p => p.MultiplierTypeId).Distinct().ToArray();

            var project = quote.Project;
            if (project == null)
            {
                project = this.Context.Projects.Where(s => s.ProjectId == quote.ProjectId).Include("Owner").FirstOrDefault();
            }

            if (project.Owner == null)
            {
                project.Owner = this.Context.Users.Where(u => u.UserId == project.OwnerId).FirstOrDefault();
            }
            //get multiplier value by BusinessId and MultiplierTypeId
            var multipliers = Db.AccountMultiplierByBusinessAndMultiplierTypeId(project.Owner.BusinessId.Value, productMultiplierTypeId).ToList();

            foreach (var item in items)
            {
                // TODO:  Should we change the way this works?

                // Set multiplier for non-commission items
                if (!quote.IsCommission)
                {
                    AccountMultiplier accountMultipler = null;

                    // If it is a system get a multiplier from the first sub products
                    if (item.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System)
                    {
                        List<ProductAccessoryModel> subProducts;
                        systemSubProductLookup.TryGetValue(item.ProductId.Value, out subProducts);

                        if (subProducts != null)
                        {
                            //accountMultipler = multiplers.Where(m => m.ProductClassCode == subProducts[0].Accessory.ProductClassCode).FirstOrDefault();
                            accountMultipler = multipliers.Where(m => m.MultiplierTypeId == subProducts[0].Accessory.MultiplierTypeId).FirstOrDefault();
                        }
                    }
                    else
                    {
                        //accountMultipler = multiplers.Where(m => m.ProductClassCode == item.Product.ProductClassCode).FirstOrDefault();
                        accountMultipler = multipliers.Where(m => m.MultiplierTypeId == item.Product.MultiplierTypeId).FirstOrDefault();
                    }

                    // If no multiplier found set to 1 and continue
                    if (accountMultipler == null)
                    {
                        if (item.AccountMultiplierId != null) item.AccountMultiplierId = null;
                        if (item.Multiplier != 1) item.Multiplier = 1;

                        continue;
                    }

                    if (item.Multiplier != accountMultipler.Multiplier)
                    {
                        item.Multiplier = accountMultipler.Multiplier;
                    }

                    if (item.AccountMultiplierId != accountMultipler.AccountMultiplierId)
                    {
                        item.AccountMultiplierId = accountMultipler.AccountMultiplierId;
                    }

                    continue;
                }

                if (commissionRequest == null && quote.CommissionRequests.Count == 0)
                {

                    item.Multiplier = 1;
                }
                else
                {
                    //updated product item on commission calculation
                    bool isSplitType = false;
                    bool isVRVType = false;
                    bool isUnitaryType = false;
                    bool isLCPackageType = false;

                    if (item.Product.MultiplierTypeId != null
                        && multiplierTypes.ContainsKey(item.Product.MultiplierTypeId.Value))
                    {
                        var multiTypeCategories = multiplierTypes[item.Product.MultiplierTypeId.Value].MultiplierTypesMultiplierCategoryTypes;

                        isSplitType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Split);
                        isVRVType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV);
                        isUnitaryType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Unitary);
                        isLCPackageType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.LCPackage);
                    }

                    var commission = quote.CommissionRequests.First();
                    decimal? vrvMultiplier = 0;
                    decimal? splitMultiplier = 0;
                    decimal? unitaryMultiplier = 0;
                    decimal? lcPackageMultiplier = 0;

                    if (commission != null)
                    {
                        vrvMultiplier = commission.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.Approved ?
                                        commission.ApprovedMultiplierVRV : commission.RequestedMultiplierVRV;
                        splitMultiplier = commission.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.Approved ?
                                          commission.ApprovedMultiplierSplit : commission.RequestedMultiplierSplit;
                        unitaryMultiplier = commission.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.Approved ?
                                            commission.ApprovedMultiplierUnitary : commission.RequestedMultiplierUnitary;

                        lcPackageMultiplier = commission.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.Approved ?
                                            commission.ApprovedMultiplierLCPackage : commission.RequestedMultiplierLCPackage;
                    }

                    if (isVRVType && vrvMultiplier.HasValue && vrvMultiplier.Value > 0)
                    {
                        item.Multiplier = vrvMultiplier.Value;
                    }
                    else if (isSplitType && splitMultiplier.HasValue && splitMultiplier.Value > 0)
                    {
                        item.Multiplier = splitMultiplier.Value;
                    }
                    else if (isUnitaryType && unitaryMultiplier.HasValue && unitaryMultiplier.Value > 0)
                    {
                        item.Multiplier = unitaryMultiplier.Value;
                    }
                    else if (isLCPackageType && lcPackageMultiplier.HasValue && lcPackageMultiplier.Value > 0)
                    {
                        item.Multiplier = lcPackageMultiplier.Value;
                    }
                    else
                    {
                        item.Multiplier = 1;
                    }
                }
            }
        }

        // #################################################
        // Calculate quote totals
        // #################################################
        public void RulesToCalculateQuoteTotals(UserSessionModel user, Quote quote, List<QuoteItem> items)
        {
            quote.TotalMisc = 0;

            quote.TotalListCommission = 0;
            quote.TotalNetCommission = 0;

            quote.TotalListNonCommission = 0;
            quote.TotalNetNonCommission = 0;

            quote.TotalNetVRV = 0;
            quote.TotalListVRV = 0;

            quote.TotalNetSplit = 0;
            quote.TotalListSplit = 0;

            quote.TotalNetUnitary = 0;
            quote.TotalListUnitary = 0;

            quote.TotalListLCPackage = 0;
            quote.TotalNetLCPackage = 0;

            quote.TotalListService = 0;
            quote.TotalNetService = 0;

            quote.TotalCountCommission = 0;
            quote.TotalCountNonCommission = 0;
            quote.TotalCountService = 0;
            quote.TotalCountSplit = 0;
            quote.TotalCountSplitOutdoor = 0;
            quote.TotalCountVRV = 0;
            quote.TotalCountVRVIndoor = 0;
            quote.TotalCountVRVOutdoor = 0;
            quote.TotalCountMisc = 0;

            foreach (var item in items)
            {
                if (!item.ProductId.HasValue)
                {
                    quote.TotalCountMisc++;
                    quote.TotalMisc += item.ListPrice * item.Quantity;
                }
                else
                {
                    // Load the multiplier type if it isn't loaded yet
                    if (item.Product.MultiplierTypeId == null
                        || item.Product.MultiplierType == null)
                    {
                        item.Product.MultiplierType = (Db.Context.MultiplierTypes
                            .Include(i => i.MultiplierTypesMultiplierCategoryTypes)
                            .Where(w => w.Products.Any(a => a.ProductId == item.ProductId))
                            .Select(s => s)).FirstOrDefault();
                    }

                    bool isSplitType = false;
                    bool isVRVType = false;
                    bool isUnitaryType = false;
                    bool isLCPackageType = false;

                    if (item.Product.MultiplierType != null
                        && item.Product.MultiplierType.MultiplierTypesMultiplierCategoryTypes != null)
                    {

                        var multiTypeCategories = item.Product.MultiplierType.MultiplierTypesMultiplierCategoryTypes;

                        isSplitType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Split);
                        isVRVType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.VRV);
                        isUnitaryType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.Unitary);
                        isLCPackageType = multiTypeCategories.Any(w => w.MultiplierCategoryTypeId == MultiplierCategoryTypeEnum.LCPackage);
                    }

                    var prodCalc = new ProductComponentCalculator(this.Context);

                    #region VRVType
                    if (isVRVType)
                    {
                        quote.TotalListVRV += item.Product.ListPrice * item.Quantity;

                        // TODO:  Remove this code after next release.  01/04/2016
                        //var indoorCount = prodCalc.CalculateVRVIndoor(item) * item.Quantity;
                        //var outdoorCount = prodCalc.CalculateVRVOutdoor(item) * item.Quantity;

                        //// TODO:  Move this out to another function.  This is duplicate data.
                        //quote.TotalCountVRVIndoor += indoorCount;
                        //quote.TotalCountVRVOutdoor += outdoorCount;

                        //quote.TotalCountVRV += indoorCount + outdoorCount;

                        if (item.Product.AllowCommissionScheme)
                        {
                            quote.TotalNetVRV += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? quote.Multiplier : item.Multiplier);
                        }
                        else
                        {
                            quote.TotalNetVRV += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                        }
                    }
                    #endregion

                    //TODO: put these into a function GetProductNetPrice(item,quote)
                    #region SplitType

                    if (isSplitType)
                    {
                        quote.TotalListSplit += item.Product.ListPrice * item.Quantity;
                        //quote.TotalCountSplitOutdoor += prodCalc.CalculateSplitOutdoor(item) * item.Quantity;

                        if (item.Product.AllowCommissionScheme)
                        {
                            quote.TotalNetSplit += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? quote.Multiplier : item.Multiplier);
                        }
                        else
                        {
                            quote.TotalNetSplit += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                        }
                    }
                    #endregion

                    #region UnitaryType

                    if (isUnitaryType)
                    {
                        quote.TotalListUnitary += item.Product.ListPrice * item.Quantity;

                        if (item.Product.AllowCommissionScheme)
                        {
                            quote.TotalNetUnitary += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 
                                                     quote.Multiplier : item.Multiplier);
                        }
                        else
                        {
                            quote.TotalNetUnitary += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                        }
                    }

                    #endregion

                    #region LCPackageType

                    if (isLCPackageType)
                    {
                        if (item.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                        {
                            quote.TotalListLCPackage += item.ListPrice * item.Quantity;
                        }
                        else
                        {
                            quote.TotalListLCPackage += item.Product.ListPrice * item.Quantity;
                        }
                        //quote.TotalListLCPackage += item.Product.ListPrice * item.Quantity;

                        if (item.Product.AllowCommissionScheme)
                        {
                            if (item.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                            {
                                quote.TotalNetLCPackage += item.ListPrice * item.Quantity * ((quote.IsCommission) ?
                                                     quote.Multiplier : item.Multiplier);
                            }
                            else
                            {
                                quote.TotalNetLCPackage += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ?
                                                     quote.Multiplier : item.Multiplier);
                            }

                            //quote.TotalNetLCPackage += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ?
                            //                         quote.Multiplier : item.Multiplier);
                        }
                        else
                        {
                            if (item.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                            {
                                quote.TotalNetLCPackage += item.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                            }
                            else
                            {
                                quote.TotalNetLCPackage += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                            }

                            //quote.TotalNetLCPackage += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier); 
                        }
                    }

                    #endregion

                    if (item.Product.AllowCommissionScheme)
                    {
                        quote.TotalCountCommission += item.Quantity;

                        if (item.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                        {
                            quote.TotalListCommission += item.ListPrice * item.Quantity;

                            quote.TotalNetCommission += item.ListPrice * item.Quantity * ((quote.IsCommission) ? quote.Multiplier : item.Multiplier);
                        }
                        else
                        {
                            quote.TotalListCommission += item.Product.ListPrice * item.Quantity;

                            quote.TotalNetCommission += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? quote.Multiplier : item.Multiplier);
                        }

                        //quote.TotalListCommission += item.Product.ListPrice * item.Quantity;

                        //quote.TotalNetCommission += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 
                        //                           quote.Multiplier : item.Multiplier);
                    }
                    else
                    {
                        quote.TotalCountNonCommission += item.Quantity;

                        if (item.LineItemTypeId == (byte?)LineItemTypeEnum.Configured)
                        {
                            quote.TotalListNonCommission += item.ListPrice * item.Quantity;

                            quote.TotalNetNonCommission += item.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                        }
                        else
                        {
                            quote.TotalListNonCommission += item.Product.ListPrice * item.Quantity;

                            quote.TotalNetNonCommission += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);
                        }

                        //quote.TotalListNonCommission += item.Product.ListPrice * item.Quantity;

                        //quote.TotalNetNonCommission += item.Product.ListPrice * item.Quantity * ((quote.IsCommission) ? 1 : item.Multiplier);

                    }
                }
            }

            CalculateUnitCounts(user, quote);

            quote.TotalService = quote.TotalNetService;

            quote.TotalList = quote.TotalListCommission + quote.TotalListNonCommission;

            if (!quote.IsCommission)
            {
                quote.TotalNet = quote.TotalNetCommission + quote.TotalNetNonCommission;
            }

            QuoteCalculationModel calc = null;

            if (quote.DiscountRequestId != null)
            {
                calc = CalculateTotalDiscountsApproved(quote);
            }
            else if (quote.CommissionRequestId != null)
            {
                calc = CalculateTotalCommissionApproved(quote);
            }
            else
            {
                calc = CalculateTotalStandard(quote);
            }

            quote.TotalNet = calc.TotalNet;
            quote.TotalNetSplit = calc.TotalNetSplit;
            quote.TotalNetVRV = calc.TotalNetVRV;
            quote.TotalNetUnitary = calc.TotalNetUnitary;
            quote.TotalNetLCPackage = calc.TotalNetLCPackage;

            quote.TotalSell = calc.TotalSell;
            quote.TotalSellSplit = calc.TotalSellSplit;
            quote.TotalSellVRV = calc.TotalSellVRV;
            quote.TotalSellUnitary = calc.TotalSellUnitary;
            quote.TotalSellLCPackage = calc.TotalSellLCPackage;

            quote.DiscountPercentageSplit = calc.TotalDiscountPercentageSplit;
            quote.DiscountPercentageVRV = calc.TotalDiscountPercentageVRV;
            quote.DiscountPercentageUnitary = calc.TotalDiscountPercentageUnitary;
            quote.DiscountPercentageLCPackage = calc.TotalDiscountPercentageLCPackage;

            quote.DiscountPercentage = calc.TotalDiscountPercentage / 100;

        }

        public QuoteCalculationModel CalculateTotalCommissionApproved(Quote quote)
        {
            var net = (!quote.IsCommission) ? (quote.TotalNetCommission + quote.TotalNetNonCommission) : quote.TotalNet;

            var req = new QuoteCalculationRequest
            {
                CommissionPercentage = quote.ApprovedCommissionPercentage,
                DiscountPercentage = quote.ApprovedDiscountPercentage,
                IsGrossMargin = quote.IsGrossMargin,
                TotalList = quote.TotalList,
                TotalListSplit = quote.TotalListSplit,
                TotalListVRV = quote.TotalListVRV,
                TotalListUnitary = (quote.TotalListUnitary.HasValue) ? quote.TotalListUnitary.Value : 0,
                TotalListLCPackage = (quote.TotalListLCPackage.HasValue) ? quote.TotalListLCPackage.Value : 0,
                TotalNet = net,
                TotalNetSplit = quote.TotalNetSplit,
                TotalNetVRV = quote.TotalNetVRV,
                TotalNetUnitary = (quote.TotalNetUnitary.HasValue) ? quote.TotalNetUnitary.Value : 0,
                TotalNetLCPackage = (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0
            };

            return CalculateTotals(req);
        }

        public QuoteCalculationModel CalculateTotalDiscountsApproved(Quote quote)
        {
            var net = (quote.TotalNetCommission + quote.TotalNetNonCommission);
            var req = new QuoteCalculationRequest
            {
                CommissionPercentage = quote.ApprovedCommissionPercentage,
                DiscountPercentage = quote.ApprovedDiscountPercentage,
                IsGrossMargin = quote.IsGrossMargin,
                TotalList = quote.TotalList,
                TotalListSplit = quote.TotalListSplit,
                TotalListVRV = quote.TotalListVRV,
                TotalListUnitary = (quote.TotalListUnitary.HasValue) ? quote.TotalListUnitary.Value : 0,
                TotalListLCPackage = (quote.TotalListLCPackage.HasValue) ? quote.TotalListLCPackage.Value : 0,
                TotalNet = net,
                TotalNetSplit = quote.TotalNetSplit,
                TotalNetVRV = quote.TotalNetVRV,
                TotalNetUnitary = (quote.TotalNetUnitary.HasValue) ? quote.TotalNetUnitary.Value : 0,
                TotalNetLCPackage = (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0
            };

            return CalculateTotals(req);
        }

        public QuoteCalculationModel CalculateTotalDiscountsApproved(QuoteListModel quote,
            decimal requestedDiscount,
            decimal requestedDiscountSplit,
            decimal requestedDiscountVRV,
            decimal requestedDiscountUnitary,
            decimal requestedDiscountLCPackage,
            decimal requestedCommissionPercentage)
        {
            var net = ((quote.TotalNetCommission ?? 0) + (quote.TotalNetNonCommission ?? 0));
            var req = new QuoteCalculationRequest
            {
                CommissionPercentage = requestedCommissionPercentage,//TODO: This seems not correct. Updated: requestedCommission -> requestedCommissionPercentage
                DiscountPercentage = requestedDiscount,
                DiscountPercentageSplit = requestedDiscountSplit,
                DiscountPercentageVRV = requestedDiscountVRV,
                DiscountPercentageUnitary = requestedDiscountUnitary,
                DiscountPercentageLCPackage = requestedDiscountLCPackage,
                IsGrossMargin = quote.IsGrossMargin,
                TotalList = quote.TotalList ?? 0,
                TotalListSplit = quote.TotalListSplit,
                TotalListVRV = quote.TotalListVRV,
                TotalListUnitary = quote.TotalListUnitary,
                TotalListLCPackage = quote.TotalListLCPackage,
                TotalNet = net,
                TotalNetSplit = quote.TotalNetSplit,
                TotalNetVRV = quote.TotalNetVRV,
                TotalNetUnitary = quote.TotalNetUnitary,
                TotalNetLCPackage = quote.TotalNetLCPackage
            };

            return CalculateTotals(req);
        }

        //Broken: Not being used. To be removed.

        //public QuoteCalculationModel CalculateTotalDiscountsApproved(QuoteListModel quote)
        //{
        //    var net = ((quote.TotalNetCommission ?? 0) + (quote.TotalNetNonCommission ?? 0));
        //    var req = new QuoteCalculationRequest
        //    {
        //        CommissionPercentage = quote.ApprovedCommissionPercentage,
        //        DiscountPercentage = quote.ApprovedDiscountPercentage,
        //        DiscountPercentageSplit = quote.ApprovedDiscountPercentageSplit,
        //        DiscountPercentageVRV = quote.ApprovedDiscountPercentageVRV,
        //        DiscountPercentageUnitary = quote.ApprovedDiscountPercentageUnitary,
        //        IsGrossMargin = quote.IsGrossMargin,
        //        TotalList = quote.TotalList ?? 0,
        //        TotalListSplit = quote.TotalListSplit,
        //        TotalListVRV = quote.TotalListVRV,
        //        TotalListUnitary = quote.TotalListUnitary,
        //        TotalNet = net,
        //        TotalNetSplit = quote.TotalNetSplit,
        //        TotalNetVRV = quote.TotalNetVRV,
        //        TotalNetUnitary = quote.TotalNetUnitary
        //    };

        //    return CalculateTotals(req);
        //}

        public QuoteCalculationModel CalculateTotalStandard(Quote quote)
        {
            var net = (quote.TotalNetCommission + quote.TotalNetNonCommission);
            var req = new QuoteCalculationRequest
            {
                CommissionPercentage = quote.CommissionPercentage,
                DiscountPercentage = 0,
                DiscountPercentageSplit = 0,
                DiscountPercentageVRV = 0,
                DiscountPercentageUnitary = 0,
                IsGrossMargin = quote.IsGrossMargin,
                TotalList = quote.TotalList,
                TotalListSplit = quote.TotalListSplit,
                TotalListVRV = quote.TotalListVRV,
                TotalListUnitary = (quote.TotalListUnitary.HasValue) ? quote.TotalNetUnitary.Value : 0,
                TotalListLCPackage = (quote.TotalListLCPackage.HasValue) ? quote.TotalListLCPackage.Value : 0,
                TotalNet = net,
                TotalNetSplit = quote.TotalNetSplit,
                TotalNetVRV = quote.TotalNetVRV,
                TotalNetUnitary = (quote.TotalNetUnitary.HasValue) ? quote.TotalNetUnitary.Value : 0,
                TotalNetLCPackage = (quote.TotalNetLCPackage.HasValue) ? quote.TotalNetLCPackage.Value : 0,
            };

            return CalculateTotals(req);
        }

        public QuoteCalculationModel CalculateTotalStandard(QuoteListModel quote)
        {
            var net = ((quote.TotalNetCommission ?? 0) + (quote.TotalNetNonCommission ?? 0));
            var req = new QuoteCalculationRequest
            {
                CommissionPercentage = quote.CommissionPercentage,
                DiscountPercentage = 0,
                DiscountPercentageSplit = 0,
                DiscountPercentageVRV = 0,
                DiscountPercentageUnitary = 0,
                IsGrossMargin = quote.IsGrossMargin,
                TotalList = quote.TotalList ?? 0,
                TotalListSplit = quote.TotalListSplit,
                TotalListVRV = quote.TotalListVRV,
                TotalListUnitary = quote.TotalListUnitary,
                TotalListLCPackage = quote.TotalListLCPackage,
                TotalNet = net,
                TotalNetSplit = quote.TotalNetSplit,
                TotalNetVRV = quote.TotalNetVRV,
                TotalNetUnitary = quote.TotalNetUnitary,
                TotalNetLCPackage = quote.TotalNetLCPackage
            };

            return CalculateTotals(req);
        }

        public QuoteCalculationModel CalculateTotals(QuoteCalculationRequest request)
        {
            // TODO:  This needs to be cleaned up, to much duplication of logic
            var newCalc = new QuoteCalculationModel();

            newCalc.TotalList = request.TotalList;
            newCalc.TotalListSplit = request.TotalListSplit;
            newCalc.TotalListVRV = request.TotalListVRV;
            newCalc.TotalListUnitary = request.TotalListUnitary;
            newCalc.TotalListLCPackage = request.TotalListLCPackage;

            //newCalc.TotalCommissionPercentage = request.CommissionPercentage * 100; 
            newCalc.TotalCommissionPercentage = request.CommissionPercentage; //  This is Buy/Sell-Commission Percentage (Gross profit percentage)

            newCalc.TotalNet = request.TotalNet - (request.TotalNet * request.DiscountPercentage);
            newCalc.TotalNetSplit = request.TotalNetSplit - (request.TotalNetSplit * request.DiscountPercentageSplit);
            newCalc.TotalNetVRV = request.TotalNetVRV - (request.TotalNetVRV * request.DiscountPercentageVRV);
            newCalc.TotalNetUnitary = request.TotalNetUnitary - (request.TotalNetUnitary * request.DiscountPercentageUnitary);
            newCalc.TotalNetLCPackage = request.TotalNetLCPackage - (request.TotalNetLCPackage * request.DiscountPercentageLCPackage);

            if (request.IsGrossMargin)
            {
                newCalc.TotalSell = newCalc.TotalNet;
                newCalc.TotalSellSplit = newCalc.TotalNetSplit;
                newCalc.TotalSellVRV = newCalc.TotalNetVRV;
                newCalc.TotalSellUnitary = newCalc.TotalNetUnitary;
                newCalc.TotalSellLCPackage = newCalc.TotalNetLCPackage;

                if (request.CommissionPercentage != 1)
                {
                    newCalc.TotalSell = newCalc.TotalNet / (1 - request.CommissionPercentage);
                    newCalc.TotalSellSplit = newCalc.TotalNetSplit / (1 - request.CommissionPercentage);
                    newCalc.TotalSellVRV = newCalc.TotalNetVRV / (1 - request.CommissionPercentage);
                    newCalc.TotalSellUnitary = newCalc.TotalUnitary / (1 - request.CommissionPercentage);
                    newCalc.TotalSellLCPackage = newCalc.TotalLCPackage / (1 - request.CommissionPercentage);
                }
            }
            else // Mark up
            {
                newCalc.TotalSell = newCalc.TotalNet * (1 + request.CommissionPercentage);
                newCalc.TotalSellSplit = newCalc.TotalNetSplit * (1 + request.CommissionPercentage);
                newCalc.TotalSellVRV = newCalc.TotalNetVRV * (1 + request.CommissionPercentage);
                newCalc.TotalSellUnitary = newCalc.TotalNetUnitary * (1 + request.CommissionPercentage);
                newCalc.TotalSellLCPackage = newCalc.TotalNetLCPackage * (1 + request.CommissionPercentage);
            }

            // Total Discount Amount

            newCalc.TotalCommissionAmount = newCalc.TotalSell - newCalc.TotalNet;

            newCalc.TotalDiscountAmount = request.TotalNet * request.DiscountPercentage;

            newCalc.TotalDiscountAmountSplit = request.TotalNetSplit * request.DiscountPercentageSplit;
            newCalc.TotalDiscountAmountVRV = request.TotalNetVRV * request.DiscountPercentageVRV;
            newCalc.TotalDiscountAmountUnitary = request.TotalNetUnitary * request.DiscountPercentageUnitary;
            newCalc.TotalDiscountAmountLCPackage = request.TotalNetLCPackage * request.DiscountPercentageLCPackage;

            newCalc.TotalDiscountPercentage = request.DiscountPercentage * 100;

            newCalc.TotalPriceAfterDiscount = newCalc.TotalSellVRV + newCalc.TotalSellSplit + newCalc.TotalSellUnitary + newCalc.TotalSellLCPackage;
           
            if (newCalc.TotalDiscountPercentage == 0 && newCalc.TotalSell != 0)
            {
                newCalc.TotalDiscountPercentage = (1 - (newCalc.TotalPriceAfterDiscount / newCalc.TotalSell)) * 100;
            }

            if (newCalc.TotalDiscountAmount == 0)
            {
                //TODO: Is this already calculated above?
                newCalc.TotalDiscountAmount = newCalc.TotalDiscountAmountVRV + newCalc.TotalDiscountAmountSplit + newCalc.TotalDiscountAmountUnitary + newCalc.TotalDiscountAmountLCPackage;
            }

            newCalc.TotalDiscountPercentageSplit = request.DiscountPercentageSplit * 100;
            newCalc.TotalDiscountPercentageVRV = request.DiscountPercentageVRV * 100;
            newCalc.TotalDiscountPercentageUnitary = request.DiscountPercentageUnitary * 100;
            newCalc.TotalDiscountPercentageLCPackage = request.DiscountPercentageLCPackage * 100;

            //Net Material Values

            newCalc.NetMaterialValue = request.TotalNet - newCalc.TotalDiscountAmount;

            newCalc.NetMaterialValueSplit = request.TotalNetSplit - newCalc.TotalDiscountAmountSplit;
            newCalc.NetMaterialValueVRV = request.TotalNetVRV - newCalc.TotalDiscountAmountVRV;
            newCalc.NetMaterialValueUnitary = request.TotalNetUnitary - newCalc.TotalDiscountAmountUnitary;
            newCalc.NetMaterialValueLCPackage = request.TotalNetLCPackage - newCalc.TotalDiscountAmountLCPackage;

            //Net Multipliers

            newCalc.NetMultiplier = 0;
            newCalc.NetMultiplierSplit = 0;
            newCalc.NetMultiplierVRV = 0;
            newCalc.NetMultiplierUnitary = 0;
            newCalc.NetMultiplierLCPackage = 0;

            if (newCalc.TotalList > 0)
            {
                newCalc.NetMultiplier = (request.TotalNet - newCalc.TotalDiscountAmount) / newCalc.TotalList;
            }

            if (newCalc.TotalListSplit > 0)
            {
                newCalc.NetMultiplierSplit = (request.TotalNetSplit - newCalc.TotalDiscountAmountSplit) / newCalc.TotalListSplit;
            }

            if (newCalc.TotalListVRV > 0)
            {
                newCalc.NetMultiplierVRV = (request.TotalNetVRV - newCalc.TotalDiscountAmountVRV) / newCalc.TotalListVRV;
            }

            if (newCalc.TotalListUnitary > 0)
            {
                newCalc.NetMultiplierUnitary = (request.TotalNetUnitary - newCalc.TotalDiscountAmountUnitary) / newCalc.TotalListUnitary;
            }

            if (newCalc.TotalListLCPackage > 0)
            {
                newCalc.NetMultiplierLCPackage = (request.TotalNetLCPackage - newCalc.TotalDiscountAmountLCPackage) / newCalc.TotalListLCPackage;
            }

            return newCalc;
        }

        // #################################################
        // Rules for commission based quotes
        // #################################################
        private void RulesToApplyCommissionablePolicy(Repository db, Quote quote)
        {

            // Only allow commission if business allows it
            if (quote.IsCommissionScheme && !quote.Project.Owner.Business.CommissionSchemeAllowed && quote.IsCommission)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP014);
            }

            var buysellTotalThreshold = decimal.Parse(Utilities.Config("dpo.sales.commission.buysell.total.threshold"));

            // Quote must be commission based if business is a commission based account and the threshold of commissionable products is reached

            if (!quote.IsCommissionScheme && quote.Project.Owner.Business.CommissionSchemeAllowed && quote.TotalNetCommission > buysellTotalThreshold)
            {
                this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP018, buysellTotalThreshold));
            }

            if (quote.IsCommissionScheme)
            {

                if (quote.TotalCountNonCommission > 0)
                {
                    if (quote.QuoteItemBeingDeleted)
                    {
                        this.Response.Messages.AddWarning(Resources.ResourceModelProject.MP019);
                    }
                    else
                    {
                        this.Response.Messages.AddError(Resources.ResourceModelProject.MP019);
                    }
                }

                var competitiveMultiplierLimit = decimal.Parse(Utilities.Config("dpo.sales.commission.competitive.multiplier"));
                var negotiationMultiplierLimit = decimal.Parse(Utilities.Config("dpo.sales.commission.negotiation.multiplier"));

                var competitiveMinListPrice = decimal.Parse(Utilities.Config("dpo.sales.commission.competitive.totallist.threshold"));
                var negotiationMinListPrice = decimal.Parse(Utilities.Config("dpo.sales.commission.negotiation.totallist.threshold"));


                // Make sure multiplier is a valid one.
                if (!Db.GetCommissionMultipliers().Any(m => quote.Multiplier == m.Multiplier))
                {
                    this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP017, competitiveMinListPrice));
                }

                //###################################################
                //Multiplier range check
                //###################################################

                // competitive policy check
                if (quote.Multiplier > negotiationMultiplierLimit && quote.Multiplier <= competitiveMultiplierLimit)
                {
                    if (quote.TotalListCommission < competitiveMinListPrice)
                    {
                        this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP016, competitiveMinListPrice));
                    }
                }

                // negotiation policy check
                if (quote.Multiplier > 0 && quote.Multiplier <= negotiationMultiplierLimit)
                {
                    if (quote.TotalListCommission < negotiationMinListPrice)
                    {
                        this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP016, negotiationMinListPrice));
                    }
                }


            }
        }
        //// #################################################
        //// Update quote prices to latest
        //// #################################################
        //public void RulesToCalculateQuoteItemPrices(Quote quote, List<QuoteItem> items)
        //{

        //    // ########################################################
        //    // Make sure products still exists
        //    // ########################################################
        //    foreach (var item in items)
        //    {
        //        if (item.ProductId.HasValue && item.Product == null)
        //        {
        //            this.Response.Messages.AddError(string.Format(Resources.ResourceModelProject.MP021, item.Description));
        //        }
        //    }

        //    if (!this.Response.IsOK) return;

        //    Dictionary<string, decimal> productMultipliers = null;

        //    // ########################################################
        //    // Buy/Sell Scheme product multipliers.
        //    // Commission Scheme does not use product level multipliers 
        //    // ########################################################
        //    if (!quote.IsCommissionScheme)
        //    {
        //        var multipliers = items.Where(i => i.AccountMultiplierId.HasValue && i.AccountMultiplier == null).Select(i => i.AccountMultiplierId).Distinct().ToArray();

        //        Db.AccountMultipliersByBusinessAndProductClassCode.AccountMultipliers.Where(a => multipliers.Contains(a.AccountMultiplierId))
        //        .Select(a => new
        //        {
        //            ProductClassCode = a.ProductClassCode,
        //            Multiplier = a.Multiplier
        //        })
        //        .ToList()
        //        .ForEach(r => productMultipliers.Add(r.ProductClassCode, r.Multiplier));
        //    }

        //    // ########################################################
        //    // Update quote items
        //    // ########################################################
        //    foreach (var item in items)
        //    {
        //        decimal multiplier = 1;
        //        if (!quote.IsCommissionScheme)
        //        {
        //            productMultipliers.TryGetValue(item.Product.ProductClassCode, out multiplier);
        //        }

        //        if (item.ListPrice != item.Product.ListPrice) { item.ListPrice = item.Product.ListPrice; };
        //        if (item.Description != item.Product.Name) { item.Description = item.Product.Name; };
        //        if (item.Multiplier != multiplier) { item.Multiplier = multiplier; };

        //    }
        //}

    }

}