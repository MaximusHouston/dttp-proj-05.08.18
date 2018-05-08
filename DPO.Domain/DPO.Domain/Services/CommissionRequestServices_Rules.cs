
using DPO.Common;
using DPO.Data;
using DPO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace DPO.Domain
{

    public partial class CommissionRequestServices : BaseServices
    {
        public void RulesOnValidateModel(UserSessionModel admin, CommissionRequestModel model)
        {

            this.Response.Messages.Clear();

            if (!model.IsValidEmails)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP117);

                string errorMessage = "The following emails are not associated with DaikinCity account: ";
                for (int i = 0; i < model.InvalidEmails.Count; i++)
                {
                    if (i == model.InvalidEmails.Count)
                    {
                        errorMessage += model.InvalidEmails[i];
                    }
                    else
                    {
                        errorMessage += model.InvalidEmails[i] + ",";
                    }
                }

                this.Response.Messages.AddError(errorMessage);

                errorMessage = "";
            }

            return;
        }

        public override void RulesOnAdd(UserSessionModel user, object entity)
        {
            var commissionRequest = entity as CommissionRequest;

            if (commissionRequest == null)
            {
                throw new ArgumentException("Commission request entity not loaded");
            }

            if (!commissionRequest.IsCommissionCalculation.Value)
            {
                commissionRequest.RequestedOn = DateTime.Now;
                commissionRequest.CommissionRequestStatusTypeId = (int)CommissionRequestStatusTypeEnum.Pending;

            }

            if (commissionRequest.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, commissionRequest.QuoteId).Load();

                if (commissionRequest.Quote == null)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
                    return;
                }

                if (commissionRequest.Quote.DiscountRequestId != null)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP104);
                    return;
                }

                //if (commissionRequest.Quote.CommissionRequestId != null)
                //{
                //    this.Response.Messages.AddError(Resources.ResourceModelProject.MP118);
                //    return;
                //}

                if (commissionRequest.Quote.AwaitingCommissionRequest)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP119);
                    return;
                }
            }

            RulesCommon(user, commissionRequest);

            if (!commissionRequest.IsCommissionCalculation.Value)
            {
                commissionRequest.Quote.AwaitingCommissionRequest = true;
            }

            RuleOnAddOrUpdate(user, commissionRequest);
        }


        public override void RulesOnEdit(UserSessionModel user, object entity)
        {
            var commissionRequest = entity as CommissionRequest;

            if (commissionRequest == null)
            {
                throw new ArgumentException("Commission request entity not loaded");
            }

            if (commissionRequest.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.NewRecord)
            {
                if (commissionRequest.IsCommissionCalculation.HasValue && !commissionRequest.IsCommissionCalculation.Value)
                {
                    commissionRequest.RequestedOn = DateTime.Now;
                    commissionRequest.CommissionRequestStatusTypeId = (int)CommissionRequestStatusTypeEnum.Pending;
                }
            }

            RulesCommon(user, commissionRequest);

            if (!commissionRequest.IsCommissionCalculation.Value)
            {
                if (commissionRequest.CommissionRequestStatusTypeId == (int)CommissionRequestStatusTypeEnum.Pending)
                {
                    commissionRequest.Quote.AwaitingCommissionRequest = true;
                }
            }

            RulesOnStatusChange(user, commissionRequest);
        }

        public override void RulesOnDelete(UserSessionModel user, object entity)
        {
            throw new ArgumentException("Commission Request cannot be deleted");
        }

        public void RulesCommon(UserSessionModel user, CommissionRequest entity)
        {

            if (entity.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, entity.QuoteId).Load();
            }

            if (entity.HasCompetitorPrice && (entity.CompetitorPrice == null || entity.CompetitorPrice.Value < 0))
            {
                this.Response.Messages.AddError("CompetitorPrice", Resources.ResourceModelProject.MP100);
            }


            if (entity.HasCompetitorQuote)
            {
                string fileLocation = Utilities.GetDARDirectory(entity.QuoteId) + entity.CompetitorQuoteFileName;

                if (string.IsNullOrWhiteSpace(entity.CompetitorQuoteFileName) || !File.Exists(fileLocation))
                {
                    this.Response.Messages.AddError("CompetitorQuoteFileName", Resources.ResourceModelProject.MP101);
                }
            }

            if (entity.HasCompetitorLineComparsion)
            {
                string fileLocation = Utilities.GetDARDirectory(entity.QuoteId) + entity.CompetitorLineComparsionFileName;

                if (string.IsNullOrWhiteSpace(entity.CompetitorLineComparsionFileName) || !File.Exists(fileLocation))
                {
                    this.Response.Messages.AddError("CompetitorLineComparsionFileName", Resources.ResourceModelProject.MP102);
                }
            }

            if (string.IsNullOrEmpty(entity.Notes) && entity.IsCommissionCalculation != true)  // uncommment this code if the CommissionRequest View need to have the Request Notes TextArea
            {
                this.Response.Messages.AddError("Notes", Resources.ResourceModelProject.MP106);
            }

            if (string.IsNullOrEmpty(entity.ResponseNotes) &&
                base.Entry.HasChanged("CommissionRequestStatusTypeId") &&
                (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Rejected ||
                 entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved))
            {
                this.Response.Messages.AddError("ResponseNotes", Resources.ResourceModelProject.MP107);
            }

            if (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending)
            {
                if (entity.SystemBasisDesignTypeId == null)
                {
                    this.Response.Messages.AddError("SystemBasisDesignTypeId", Resources.ResourceModelProject.MP108);
                }

                if (entity.ZoneStrategyTypeId == null)
                {
                    this.Response.Messages.AddError("ZoneStrategyTypeId", Resources.ResourceModelProject.MP109);
                }

                if (entity.BrandApprovedTypeId == null)
                {
                    this.Response.Messages.AddError("BrandApprovedTypeId", Resources.ResourceModelProject.MP110);
                }

                if (entity.BrandSpecifiedTypeId == null)
                {
                    this.Response.Messages.AddError("BrandSpecifiedTypeId", Resources.ResourceModelProject.MP111);
                }

                if (entity.FundingTypeId == null)
                {
                    this.Response.Messages.AddError("FundingTypeId", Resources.ResourceModelProject.MP122);
                }
            }

            var quoteItems = Db.QuoteItemsByQuoteId(user, entity.QuoteId).Include("Product").ToList();
            this.quoteService.SetQuoteItemMultipliers(entity.Quote, quoteItems, entity);
        }

        public void RulesOnStatusChange(UserSessionModel user, CommissionRequest entity)
        {
            if (base.Entry.HasChanged("CommissionRequestStatusTypeId"))
            {
                entity.StatusTypeModifiedBy = user.UserId;
                entity.StatusTypeModifiedOn = DateTime.Now;

                if (entity.Quote.Project == null)
                {
                    // Needed for quote calculations
                    Db.QueryProjectViewableByProjectId(user, entity.ProjectId).Include("Owner").Include("Owner.Business").Load();
                }

                if (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending)
                {
                    RuleOnAddOrUpdate(user, entity);
                }

                if (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Rejected ||
                    entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Deleted)
                {
                    RuleOnRejectOrDelete(user, entity);
                }

                if (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved)
                {
                    RuleOnApproved(user, entity);
                }

                //RecalculateQuote(user, entity);
            }
        }

        private void RuleOnRejectOrDelete(UserSessionModel user, CommissionRequest entity)
        {
            entity.Quote.CommissionRequestId = null;
            entity.Quote.AwaitingCommissionRequest = false;

            entity.ApprovedMultiplier = 0;
            entity.ApprovedMultiplierSplit = 0;
            entity.ApprovedMultiplierVRV = 0;
            entity.ApprovedMultiplierUnitary = 0;
            entity.ApprovedMultiplierLCPackage = 0;

            RecalculateQuote(user, entity);

            if (entity.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Rejected)
            {
                if (this.Response.IsOK) this.Response.AddSuccess("Commission request rejected.");
            }
            else
            {
                if (this.Response.IsOK) this.Response.AddSuccess("Commission request deleted.");
            }
        }

        private void RuleOnApproved(UserSessionModel user, CommissionRequest entity)
        {
            // Apply approved commission
            entity.Quote.CommissionRequestId = entity.CommissionRequestId;
            entity.Quote.ApprovedCommissionPercentage = entity.ApprovedCommissionPercent.Value;
            entity.Quote.CommissionPercentage = entity.ApprovedCommissionPercent.Value;

            // Apply approved discount multipliers
            entity.Quote.ApprovedDiscountPercentageVRV = entity.ApprovedCommissionPercentVRV.Value;
            entity.Quote.ApprovedDiscountPercentageSplit = entity.ApprovedMultiplierSplit.Value;
            entity.Quote.ApprovedDiscountPercentageUnitary = entity.ApprovedCommissionPercentUnitary.Value;
            entity.Quote.ApprovedDiscountPercentageLCPackage = entity.ApprovedCommissionPercentLCPackage.Value;

            entity.Quote.TotalNet = entity.TotalNet.Value;

            // Remove awaiting commission request
            entity.Quote.AwaitingCommissionRequest = false;

            //RecalculateQuote(user, entity);
            RecalculateCommission(user, entity);

            if (this.Response.IsOK) this.Response.AddSuccess("Commission request approved.");
        }

        private void UpdateProjectInfo(UserSessionModel user, Project entity)
        {

            ProjectServices projectServices = new ProjectServices();
            ServiceResponse resposne = projectServices.GetProjectModel(user, entity.ProjectId);

            ProjectModel projectModel = resposne.Model as ProjectModel;

            Entry = Db.Entry(entity);

            Entry.State = EntityState.Modified;

            projectServices.RulesOnEdit(user, entity);

            if (this.Response.IsOK)
            {
                projectServices.SaveToDatabase(projectModel, entity, "Project updated");
            }
        }

        private void RecalculateQuote(UserSessionModel user, CommissionRequest entity)
        {
            Entry = Db.Entry(entity.Quote);

            entity.Quote.RecalculationRequired = true;

            Entry.State = EntityState.Modified;

            new QuoteServices(this, "Quote").ApplyBusinessRules(user, entity.Quote);
        }

        private void RecalculateCommission(UserSessionModel user, CommissionRequest entity)
        {
            if (entity.CommissionRequestStatusTypeId != (byte)CommissionRequestStatusTypeEnum.Approved)
            {

                if (entity.Quote == null)
                {
                    entity.Quote = Db.QueryQuoteViewableByQuoteId(user, entity.QuoteId).FirstOrDefault();
                }

                if (entity.Quote == null)
                {
                    this.Response.AddCritical(ResourceModelProject.MP140);
                    return;
                }

                #region VRV Commission

                // VRV Commission Calculation
                entity.TotalNetVRV = entity.Quote.TotalListVRV * entity.RequestedMultiplierVRV;
                entity.RequestedCommissionVRV = entity.TotalNetVRV * (entity.RequestedCommissionPercentVRV / 100);

                // VRV Net Material Cost
                entity.RequestedNetMultiplierValueVRV = entity.TotalNetVRV - entity.RequestedCommissionVRV;
                if(entity.Quote.TotalListVRV != 0)
                {
                    entity.RequestedNetMaterialMultiplierVRV = entity.RequestedNetMultiplierValueVRV / entity.Quote.TotalListVRV;
                }
               

                #endregion VRV Commission

                #region Split Commission

                // Split Commission Requested
                entity.TotalNetSplit = entity.Quote.TotalListSplit * entity.RequestedMultiplierSplit;
                entity.RequestedCommissionSplit = entity.TotalNetSplit * (entity.RequestedCommissionPercentSplit / 100);

                // Split Net Material Cost
                entity.RequestedNetMultiplierValueSplit = entity.TotalNetSplit - entity.RequestedCommissionSplit;
                if (entity.Quote.TotalListSplit != 0)
                {
                    entity.RequestedNetMaterialMultiplierSplit = entity.RequestedNetMultiplierValueSplit / entity.Quote.TotalListSplit;
                }
                else
                {
                    entity.RequestedNetMaterialMultiplierSplit = 0;
                }

                #endregion Split Commission

                #region UnitaryCommission
                entity.TotalNetUnitary = entity.Quote.TotalListUnitary * entity.RequestedMultiplierUnitary;
                entity.RequestedCommissionUnitary = entity.TotalNetUnitary * (entity.RequestedCommissionPercentUnitary / 100);

                entity.RequestedNetMultiplierValueUnitary = entity.TotalNetUnitary - entity.RequestedCommissionUnitary;
                if (entity.Quote.TotalListUnitary != 0)
                {
                    entity.RequestedNetMaterialMultiplierUnitary = entity.RequestedNetMultiplierValueUnitary / entity.Quote.TotalListUnitary;
                }
                else
                {
                    entity.RequestedNetMaterialMultiplierUnitary = 0;
                }
                #endregion

                #region LCPackageCommission
                entity.TotalNetLCPackage = entity.Quote.TotalListLCPackage * entity.RequestedMultiplierLCPackage;
                entity.RequestedCommissionLCPackage = entity.TotalNetLCPackage * (entity.RequestedCommissionPercentLCPackage / 100);

                entity.RequestedNetMultiplierValueLCPackage = entity.TotalNetLCPackage - entity.RequestedCommissionLCPackage;
                if (entity.Quote.TotalListLCPackage != 0)
                {
                    entity.RequestedNetMaterialMultiplierLCPackage = entity.RequestedNetMultiplierValueLCPackage / entity.Quote.TotalListLCPackage;
                }
                else
                {
                    entity.RequestedNetMaterialMultiplierLCPackage = 0;
                }
                #endregion

                // Requested Commission Total
                entity.RequestedCommissionTotal = entity.RequestedCommissionVRV +
                                                  entity.RequestedCommissionSplit + 
                                                  entity.RequestedCommissionUnitary +
                                                  entity.RequestedCommissionLCPackage;

                // Total Net
                entity.TotalNet = entity.TotalNetSplit + entity.TotalNetVRV + entity.TotalNetUnitary + entity.TotalNetLCPackage; 
                entity.RequestedCommissionPercent = (entity.RequestedCommissionTotal / entity.TotalNet) * 100;

                // Net Material Calculations
                entity.RequestedNetMaterialValue = entity.TotalNet - entity.RequestedCommissionTotal;
                entity.RequestedNetMaterialValueMultiplier = entity.RequestedNetMaterialValue / 
                                                            (entity.Quote.TotalListSplit + 
                                                             entity.Quote.TotalListVRV + 
                                                             entity.Quote.TotalListUnitary +
                                                             entity.Quote.TotalListLCPackage);

                // Total Net Multiplier
                entity.RequestedMultiplier = entity.TotalNet / 
                                             (entity.Quote.TotalListSplit + 
                                              entity.Quote.TotalListVRV +
                                              entity.Quote.TotalListUnitary +
                                              entity.Quote.TotalListLCPackage);
            }
        }

        private void RuleOnAddOrUpdate(UserSessionModel user, CommissionRequest entity)
        {
            // Apply approved commission
            entity.Quote.CommissionRequestId = entity.CommissionRequestId;
            entity.Quote.CommissionPercentage = entity.RequestedCommissionPercent.Value;
            entity.Quote.ApprovedCommissionPercentage = entity.ApprovedCommissionPercent.Value;

            entity.Quote.TotalNet = entity.TotalNet.Value;
        }
    }

}