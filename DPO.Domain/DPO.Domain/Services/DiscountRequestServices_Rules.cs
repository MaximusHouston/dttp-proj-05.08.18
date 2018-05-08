//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

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

    public partial class DiscountRequestServices : BaseServices 
    {

        public void RulesOnValidateModel(DiscountRequestModel model)
        {

            this.Response.Messages.Clear();

            if (!model.IsValidEmails)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP117 );

                string errorMessage = "The following emails are not associated with DaikinCity account: ";
                for(int i=0; i < model.InvalidEmails.Count; i++ )
                {
                    if ( i == model.InvalidEmails.Count)
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
                return;
            }

            //TODO: need to investigate why Request Discount != RequestedDiscount VRV
            //when there are only VRV products
            //This is the hacking to make sure RequestedDiscount == RequestedDiscountVRV
            //when there is only VRV products in the request or 
            //RequestedDiscount == RequestedDiscountSplit when there is only Ductless products
            //Add on : Aaron Nguyen 09-23-2016

            //TODO: Do we need to do this? - Huy Nguyen
            //if (model.RequestedDiscountVRV > 0 && model.RequestedDiscountSplit == 0)
            //{
            //    if(model.RequestedDiscount != model.RequestedDiscountVRV)
            //    {
            //        model.RequestedDiscount = model.RequestedDiscountVRV;
            //    }
            //}
            //if(model.RequestedDiscountSplit > 0 && model.RequestedDiscountVRV == 0)
            //{
            //    if(model.RequestedDiscount != model.RequestedDiscountSplit)
            //    {
            //        model.RequestedDiscount = model.RequestedDiscountSplit;
            //    }
            //}
        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel user, object entity)
        {
            var discountRequest = entity as DiscountRequest;

            if (discountRequest == null)
            {
                throw new ArgumentException("Discount request entity not loaded");
            }

            discountRequest.RequestedOn = DateTime.Now;

            discountRequest.DiscountRequestStatusTypeId = (int)DiscountRequestStatusTypeEnum.Pending;

            if (discountRequest.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, discountRequest.QuoteId).Load();

                if (discountRequest.Quote == null)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
                    return;
                }

                if (discountRequest.Quote.DiscountRequestId != null)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP104);
                    return;
                }

                if (discountRequest.Quote.AwaitingDiscountRequest)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP103);
                    return;
                }

                
            }

            RulesCommon(user, discountRequest);

            discountRequest.Quote.AwaitingDiscountRequest = true;

        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel user, object entity)
        {
            var discountRequest = entity as DiscountRequest;

            if (discountRequest == null)
            {
                throw new ArgumentException("Discount request entity not loaded");
            }

            RulesCommon(user, discountRequest);

            RulesOnStatusChange(user, discountRequest);

        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel user, object entity)
        {
           throw new ArgumentException("Discount Request cannot be deleted");
        }

        public void RulesCommon(UserSessionModel user, DiscountRequest entity)
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

            if (string.IsNullOrEmpty(entity.Notes))
            {
                this.Response.Messages.AddError("Notes", Resources.ResourceModelProject.MP106);
            }

            if (string.IsNullOrEmpty(entity.ResponseNotes) && 
                base.Entry.HasChanged("DiscountRequestStatusTypeId") && 
                (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected ||
                 entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved) )
            {
                this.Response.Messages.AddError("ResponseNotes", Resources.ResourceModelProject.MP107);
            }

            if(entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending)
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

                if (entity.OrderPlannedFor == null)
                {
                    this.Response.Messages.AddError("OrderPlannedFor", Resources.ResourceModelProject.MP112);
                }

                if (entity.OrderDeliveryDate == null)
                {
                    this.Response.Messages.AddError("OrderDeliveryDate", Resources.ResourceModelProject.MP113);
                }

                if (entity.DaikinEquipmentAtAdvantageTypeId == null)
                {
                    this.Response.Messages.AddError("DaikinEquipmentAtAdvantageTypeId", Resources.ResourceModelProject.MP114);
                }

                if(entity.ProbabilityOfCloseTypeId == null)
                {
                    this.Response.Messages.AddError("ProbabilityOfCloseTypeId", Resources.ResourceModelProject.MP115);
                }

                // Copy over the requested amounts to approved amounts if the DAR is still pending// Why?
                if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.NewRecord
                    || entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending)
                {
                    entity.ApprovedDiscount = entity.RequestedDiscount;
                    entity.ApprovedDiscountSplit = entity.RequestedDiscountSplit;
                    entity.ApprovedDiscountVRV = entity.RequestedDiscountVRV;
                    entity.ApprovedDiscountUnitary = entity.RequestedDiscountUnitary;
                    entity.ApprovedDiscountLCPackage = entity.RequestedDiscountLCPackage;
                }
            }
        }

        public void RulesOnStatusChange(UserSessionModel user, DiscountRequest entity)
        {
            if (base.Entry.HasChanged("DiscountRequestStatusTypeId"))
            {
                entity.StatusTypeModifiedBy = user.UserId;
                entity.StatusTypeModifiedOn = DateTime.Now;

                if (entity.Quote.Project == null)
                {
                    // Needed for quote calculations
                    Db.QueryProjectViewableByProjectId(user, entity.ProjectId).Include("Owner").Include("Owner.Business").Load();
                }

                if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected ||
                    entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Deleted)
                {
                    entity.Quote.DiscountRequestId = null;
                    entity.Quote.AwaitingDiscountRequest = false;

                    entity.ApprovedDiscount = 0;
                    entity.ApprovedDiscountSplit = 0;
                    entity.ApprovedDiscountVRV = 0;
                    entity.ApprovedDiscountUnitary = 0;
                    entity.ApprovedDiscountLCPackage = 0;

                    RecalulateQuote(user, entity);

                    if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected)
                    {
                        if (this.Response.IsOK) this.Response.AddSuccess("Discount request rejected.");
                    }
                    else
                    {
                        if (this.Response.IsOK) this.Response.AddSuccess("Discount request deleted.");
                    }
                }

                if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved)
                {
                    entity.Quote.DiscountRequestId = entity.DiscountRequestId;
                    entity.Quote.ApprovedCommissionPercentage = entity.RequestedCommission;
                    if( entity.ApprovedDiscount != null && entity.ApprovedDiscount > 0 )
                    {
                        entity.Quote.ApprovedDiscountPercentage = entity.ApprovedDiscount.Value;
                    }
                    else
                    {
                        entity.Quote.ApprovedDiscountPercentage = entity.RequestedDiscount;

                    }
                    
                    entity.Quote.AwaitingDiscountRequest = false;

                    RecalulateQuote(user, entity);

                    if (this.Response.IsOK) this.Response.AddSuccess("Discount request approved.");
                }
            }

        }

        private void RecalulateQuote(UserSessionModel user, DiscountRequest entity)
        {
            Entry = Db.Entry(entity.Quote);

            entity.Quote.RecalculationRequired = true;

            Entry.State = EntityState.Modified;

            new QuoteServices(this,"Quote").ApplyBusinessRules(user, entity.Quote);
        }

    }

}