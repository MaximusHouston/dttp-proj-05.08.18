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
using DPO.Model.Light;

namespace DPO.Domain
{
    public partial class OrderServices : BaseServices
    {
        public void RulesOnValidateModel(OrderViewModelLight model)
        {
            this.Response.Messages.Clear();

            //if (!model.IsValidEmails)
            //{
            //    this.Response.Messages.AddError(Resources.ResourceModelProject.MP117);

            //    string errorMessage = "The following emails are not associated with DaikinCity account: ";
            //    for (int i = 0; i < model.InvalidEmails.Count; i++)
            //    {
            //        if (i == model.InvalidEmails.Count)
            //        {
            //            errorMessage += model.InvalidEmails[i];
            //        }
            //        else
            //        {
            //            errorMessage += model.InvalidEmails[i] + ",";
            //        }
            //    }

            //    this.Response.Messages.AddError(errorMessage);
            //}


            //if (model.Project.BidDate == null)
            //{
            //    this.Response.Messages.AddError("ProjectBidDate", Resources.ResourceModelProject.MP120);
            //}
            //else if (model.Project.BidDate < model.Project.EstimatedClose)
            //{
            //    this.Response.Messages.AddError("ProjectBidDateInvalid", Resources.ResourceModelProject.MP003);
            //}
            //if (model.Project.EstimatedClose == null)
            //{
            //    this.Response.Messages.AddError("EstimateCloseDate", Resources.ResourceModelProject.MP121);
            //}
            //else if (model.Project.EstimatedClose > model.Project.EstimatedDelivery)
            //{
            //    this.Response.Messages.AddError("EstimateCloseDate", Resources.ResourceModelProject.MP002);
            //}

            //if (model.Project.EstimateReleaseDate == null)
            //{
            //    this.Response.Messages.AddError("Estimate Delivery Date not set");
            //}
            //else if (model.Project.EstimateReleaseDate > model.Project.EstimatedDelivery.Value.AddDays(10))
            //{
            //    this.Response.Messages.AddError("EstimateDeliveryDateInvalid", Resources.ResourceModelProject.MP128);
            //}

            //if (model.Project.ActualCloseDate == null)
            //{
            //    if (model.SubmitDate != null)
            //    {
            //        model.Project.ActualCloseDate = model.SubmitDate.Value;
            //    }
            //    else
            //    {
            //        model.Project.ActualCloseDate = DateTime.Now;
            //    }
            //}

            if (model.ERPAccountId == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelBusiness.BM010);
            }
            
            if (model.CurrentUser == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelUser.MU024);
            }
            else if (model.CurrentUser.Email == null)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP124);
            }

            if (model.CurrentUser != null && !model.CurrentUser.ShowPrices)
            {
                this.Response.Messages.AddError(Resources.ResourceModelBusiness.BM008);
            }

            if (model.POAttachmentFileName == null || model.POAttachmentFileName==string.Empty)
            {
                this.Response.Messages.AddError("POAttachment", ResourceUI.POAttachmentRequired);
            }

            if(model.OrderReleaseDate == null || model.OrderReleaseDate == DateTime.MinValue)
            {
                this.Response.Messages.AddError("OrderReleaseDate", Resources.ResourceModelProject.MP133);
            }

            if(model.OrderReleaseDate < model.SubmitDate)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP134);
            }

            if (model.DeliveryAppointmentRequired)
            {
                if(model.DeliveryContactName == null || model.DeliveryContactName.Length == 0)
                {
                    this.Response.Messages.AddError("DeliveryContactName", ResourceModelProject.MP139);
                }
                if(model.DeliveryContactPhone == null || model.DeliveryContactPhone.Length == 0)
                {
                    this.Response.Messages.AddError("DeliveryContactPhone", ResourceModelProject.MP138);
                }
                
            }

            //validate the POfile name for invalid character
            if((model.POAttachment != null &&  model.POAttachment.FileName != null) || model.POAttachmentFileName != null)
            {
                List<String> replaceCharacters = new List<String> { "@", "%", "*", "#", "&" };
                List<string> removeCharacters = new List<string> { "'", "~" };
                  foreach (string chac in replaceCharacters)
                  {
                    model.POAttachmentFileName = model.POAttachmentFileName.Replace(chac, "_");

                    if (model.OrderAttachmentFileName !=null)
                    {
                        model.OrderAttachmentFileName.Replace(chac, "_");
                    }
                  }
                  foreach(string chac in removeCharacters)
                  {
                    if (model.OrderAttachmentFileName != null)
                    {
                        model.OrderAttachmentFileName.Replace(chac, "");
                    }
                }
            }
            
        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel user, object entity)
        {
            var order = entity as Order;

            if (order == null)
            {
                throw new ArgumentException("Order entity not loaded");
            }

            order.SubmitDate = DateTime.Now;

            order.OrderStatusTypeId = (byte)OrderStatusTypeEnum.Submitted;

            if (order.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, order.QuoteId).Load();

                if (order.Quote == null)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP004);
                    return;
                }

                //if (order.Quote.DiscountRequestId != null)
                //{
                //    this.Response.Messages.AddError(Resources.ResourceModelProject.MP104);
                //    return;
                //}

                if (order.Quote.AwaitingDiscountRequest)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP103);
                    return;
                }

                if(order.OrderItems.Count() == 0)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP135);
                }
                
                bool HasQuantity = true;
                
                foreach(var item in order.OrderItems)
                {
                    if(item.Quantity == 0)
                    {
                        HasQuantity = false;
                    }
                }

                if(HasQuantity == false)
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP136);
                }
            }

            if(order.Quote.AwaitingDiscountRequest)
            {
                this.Response.Messages.AddError(Resources.ResourceModelProject.MP103);
                return;
            }

            //check to see if quote already have Order submitted
            Order lastOrder = this.Db.Context.Orders.Where(o => o.QuoteId == order.QuoteId).FirstOrDefault();

            if(lastOrder != null)
            {
                order.OrderStatusTypeId = lastOrder.OrderStatusTypeId; // copy the staus of first Order into Second Order
                order.DiscountRequestId = (lastOrder.DiscountRequestId != null ) ? lastOrder.DiscountRequestId : 0;
                order.CommissionRequestId = (lastOrder.CommissionRequestId != null) ? lastOrder.CommissionRequestId : 0;
            }

            RulesCommon(user, order);

            order.Quote.AwaitingOrder = true;

            RulesOnStatusChange(user, order);

        }

        // this funtion has been create to test the RuleOnAdd of the Order
        //I have to replicate the RulesOnAdd funtion by this one to perform unit test on
        // the RulesOnAdd(UserSessionModel user, object entity)
        //TODO: discuss with team how to overcome this issue--Aaron 06/14/2016

        public void RulesOnAdd(UserSessionModel user, object entity, out ServiceResponse Response)
        {
            var order = entity as Order;

            Response = this.Response;

            if (order == null)
            {
                throw new ArgumentException("Order entity not loaded");
            }

            order.SubmitDate = DateTime.Now;

            order.OrderStatusTypeId = (byte)OrderStatusTypeEnum.Submitted;

            bool HasQuantity = true;

            if (order.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, order.QuoteId).Load();

                if (order.Quote == null)
                {
                    Response.Messages.AddError(Resources.ResourceModelProject.MP004);
                    return;
                }

                //if (order.Quote.DiscountRequestId != null)
                //{
                //    this.Response.Messages.AddError(Resources.ResourceModelProject.MP104);
                //    return;
                //}

                if (order.Quote.AwaitingDiscountRequest)
                {
                    Response.Messages.AddError(Resources.ResourceModelProject.MP103);
                    return;
                }

                if (order.OrderItems.Count() == 0)
                {
                    Response.Messages.AddError(Resources.ResourceModelProject.MP135);
                    return;
                }

                //bool HasQuantity = true;

                foreach (var item in order.OrderItems)
                {
                    if (item.Quantity == 0)
                    {
                        HasQuantity = false;
                    }
                }

                if (HasQuantity == false)
                {
                    Response.Messages.AddError(Resources.ResourceModelProject.MP136);
                    return;
                }
            }

            if (order.Quote.AwaitingDiscountRequest)
            {
                Response.Messages.AddError(Resources.ResourceModelProject.MP103);
                return;
            }


            if(order.OrderItems.Count == 0)
            {
                Response.Messages.AddError(Resources.ResourceModelProject.MP135);
                return;
            }

            if(order.OrderItems.Count > 0)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Quantity == 0)
                    {
                        HasQuantity = false;
                    }
                }

                if (HasQuantity == false)
                {
                    Response.Messages.AddError(Resources.ResourceModelProject.MP136);
                    return;
                }
            }

            

            //check to see if quote already have Order submitted
            Order lastOrder = this.Db.Context.Orders.Where(o => o.QuoteId == order.QuoteId).FirstOrDefault();

            if (lastOrder != null)
            {
                order.OrderStatusTypeId = lastOrder.OrderStatusTypeId; // copy the staus of first Order into Second Order
                order.DiscountRequestId = (lastOrder.DiscountRequestId != null) ? lastOrder.DiscountRequestId : 0;
                order.CommissionRequestId = (lastOrder.CommissionRequestId != null) ? lastOrder.CommissionRequestId : 0;
            }

            RulesCommon(user, order);

            order.Quote.AwaitingOrder = true;

            RulesOnStatusChange(user, order);

           
        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel user, object entity)
        {
            var order = entity as Order;

            if (order == null)
            {
                throw new ArgumentException("Order entity not loaded");
            }

            RulesCommon(user, order);

            RulesOnStatusChange(user, order);

        }

      

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel user, object entity)
        {
            throw new ArgumentException("Order cannot be deleted");
        }

        public void RulesCommon(UserSessionModel user, Order entity)
        {
            if (entity.Quote == null)
            {
                Db.QueryQuoteViewableByQuoteId(user, entity.QuoteId).Load();
            }
          
            if(entity.PONumber == null)
            {
                this.Response.Messages.AddError("PONumber", Resources.ResourceModelProject.MP125);
            }

            if (entity.DeliveryAppointmentRequired == true)
            {
                if (entity.DeliveryContactName == null)
                {
                    this.Response.Messages.AddError("DeliveryContactName", Resources.ResourceModelProject.MP122);
                }

                if (entity.DeliveryContactPhone == null)
                {
                    this.Response.Messages.AddError("DeliveryContactNumber", Resources.ResourceModelProject.MP123);
                }
            }
           
            if(entity.Quote.DiscountRequests != null)
            {
                bool hasPendingDiscount = false;
               

                foreach(DiscountRequest dis in entity.Quote.DiscountRequests)
                {
                    if(dis.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending)
                    {
                        hasPendingDiscount = true;        
                    }
                }
                if(hasPendingDiscount)
                {
                    this.Response.Messages.AddError("Can not Submit Order becuase the Quote has pending Discount Request");
                }
            }

            if(entity.Quote.CommissionRequests != null)
            {
                bool hasPendingCommission = false;
                foreach(CommissionRequest com in entity.Quote.CommissionRequests)
                {
                    if(com.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Pending)
                    {
                        hasPendingCommission = true;
                    }
                }
                if(hasPendingCommission)
                {
                    this.Response.Messages.AddError("Can not Submit Order becuase the Quote has pending Commission Request");
                }
            }
               
            //load the Project if project is null
            if(entity.Quote.Project == null)
            {
                entity.Quote.Project = this.Db.Context.Projects.Where(p => p.ProjectId == entity.Quote.ProjectId).FirstOrDefault();
            }

            if(entity.Quote.Project.ProjectStatusTypeId == ProjectStatusTypeEnum.Inactive ||
               entity.Quote.Project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost)
                {
                    this.Response.Messages.AddError( Resources.ResourceModelProject.MP129);
                }

                //if(entity.Quote.Project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon)
                //{
                //    this.Response.Messages.AddError( Resources.ResourceModelProject.MP130);
                //}
               
               if (entity.Quote.IsCommission)
               {
                  this.Response.Messages.AddError( Resources.ResourceModelProject.MP131);
               }
               
               if(string.IsNullOrEmpty(entity.ShipToAddressId.ToString()))
               {
                   this.Response.Messages.AddError("ShipToAddress is not created");
               }

              //if(!entity.Quote.Active)
              //{
              //    this.Response.Messages.AddError("QuoteActiveStatus", Resources.ResourceModelProject.MP132);
              //}
               
            //if (entity.HasCompetitorPrice && (entity.CompetitorPrice == null || entity.CompetitorPrice.Value < 0))
            //{
            //    this.Response.Messages.AddError("CompetitorPrice", Resources.ResourceModelProject.MP100);
            //}


            //if (entity.HasCompetitorQuote)
            //{
            //    string fileLocation = Utilities.GetDARDirectory(entity.QuoteId) + entity.CompetitorQuoteFileName;

            //    if (string.IsNullOrWhiteSpace(entity.CompetitorQuoteFileName) || !File.Exists(fileLocation))
            //    {
            //        this.Response.Messages.AddError("CompetitorQuoteFileName", Resources.ResourceModelProject.MP101);
            //    }
            //}

            //if (entity.HasCompetitorLineComparsion)
            //{
            //    string fileLocation = Utilities.GetDARDirectory(entity.QuoteId) + entity.CompetitorLineComparsionFileName;

            //    if (string.IsNullOrWhiteSpace(entity.CompetitorLineComparsionFileName) || !File.Exists(fileLocation))
            //    {
            //        this.Response.Messages.AddError("CompetitorLineComparsionFileName", Resources.ResourceModelProject.MP102);
            //    }
            //}

            //if (entity.RequestedDiscount == 0 && entity.RequestedCommission == 0)
            //{
            //    this.Response.Messages.AddError("RequestedCommission", Resources.ResourceModelProject.MP105);
            //    this.Response.Messages.AddError("RequestedDiscount", Resources.ResourceModelProject.MP105);
            //}

            //if (string.IsNullOrEmpty(entity.Notes))
            //{
            //    this.Response.Messages.AddError("Notes", Resources.ResourceModelProject.MP106);
            //}

            //if (string.IsNullOrEmpty(entity.ResponseNotes) &&
            //    base.Entry.HasChanged("DiscountRequestStatusTypeId") &&
            //    (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected ||
            //     entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved))
            //{
            //    this.Response.Messages.AddError("ResponseNotes", Resources.ResourceModelProject.MP107);
            //}

            if (entity.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
            {
                if(string.IsNullOrEmpty(entity.PricingTypeId.ToString()))
                {
                    this.Response.Messages.AddError("PricingTypeId", Resources.ResourceModelProject.MP118);
                }

                if(entity.PONumber == null)
                {
                    this.Response.Messages.AddError("PONumber", Resources.ResourceModelProject.MP119);
                }

                //if (entity.SystemBasisDesignTypeId == null)
                //{
                //    this.Response.Messages.AddError("SystemBasisDesignTypeId", Resources.ResourceModelProject.MP108);
                //}

                //if (entity.ZoneStrategyTypeId == null)
                //{
                //    this.Response.Messages.AddError("ZoneStrategyTypeId", Resources.ResourceModelProject.MP109);
                //}

                //if (entity.BrandApprovedTypeId == null)
                //{
                //    this.Response.Messages.AddError("BrandApprovedTypeId", Resources.ResourceModelProject.MP110);
                //}

                //if (entity.BrandSpecifiedTypeId == null)
                //{
                //    this.Response.Messages.AddError("BrandSpecifiedTypeId", Resources.ResourceModelProject.MP111);
                //}

                //if (entity.OrderPlannedFor == null)
                //{
                //    this.Response.Messages.AddError("OrderPlannedFor", Resources.ResourceModelProject.MP112);
                //}

                //if (entity.OrderDeliveryDate == null)
                //{
                //    this.Response.Messages.AddError("OrderDeliveryDate", Resources.ResourceModelProject.MP113);
                //}

                //if (entity.DaikinEquipmentAtAdvantageTypeId == null)
                //{
                //    this.Response.Messages.AddError("DaikinEquipmentAtAdvantageTypeId", Resources.ResourceModelProject.MP114);
                //}

                //if (entity.ProbabilityOfCloseTypeId == null)
                //{
                //    this.Response.Messages.AddError("ProbabilityOfCloseTypeId", Resources.ResourceModelProject.MP115);
                //}

                // Copy over the requested amounts to approved amounts if the DAR is still pending
                //if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.NewRecord
                //    || entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Pending)
                //{
                //    entity.ApprovedDiscount = entity.RequestedDiscount;
                //    entity.ApprovedDiscountSplit = entity.RequestedDiscountSplit;
                //    entity.ApprovedDiscountVRV = entity.RequestedDiscountVRV;
                //}
              
            }
        }

        public void RulesOnStatusChange(UserSessionModel user, Order entity)
        {

            if (base.Entry.HasChanged("OrderStatusTypeId"))
            {
                entity.UpdatedByUserId = user.UserId;
        
                if (entity.Quote.Project == null)
                {
                    // Needed for quote calculations
                    Db.QueryProjectViewableByProjectId(user, entity.Quote.ProjectId).Include("Owner").Include("Owner.Business").Load();
                }

                if(entity.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Submitted)
                {
                    
                    entity.Quote.Active = true;

                    QuoteServices quoteService = new QuoteServices();
                    QuoteModel quoteVM = new QuoteModel();
                    quoteVM = quoteService.GetQuoteModel(user, entity.Quote.ProjectId, entity.QuoteId).Model as QuoteModel;

                    if (quoteVM != null)
                    {
                        this.Response = quoteService.SetActive(user, quoteVM);
                    }

                    if (this.Response.IsOK)
                    {
                        entity.Quote.Project.ProjectOpenStatusTypeId = (byte)ProjectOpenStatusTypeEnum.DaikinHasPO;
                        entity.Quote.Project.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedWon;
                    }
                    else
                    {
                        this.Response.Messages.AddError("Set Quote to Active failed");
                    }
           
                    
                }
                //if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected ||
                //    entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Deleted)
                //{
                //    entity.Quote.DiscountRequestId = null;
                //    entity.Quote.AwaitingDiscountRequest = false;

                //    entity.ApprovedDiscount = 0;
                //    entity.ApprovedDiscountSplit = 0;
                //    entity.ApprovedDiscountVRV = 0;

                //    RecalulateQuote(user, entity);

                //    if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Rejected)
                //    {
                //        if (this.Response.IsOK) this.Response.AddSuccess("Discount request rejected.");
                //    }
                //    else
                //    {
                //        if (this.Response.IsOK) this.Response.AddSuccess("Discount request deleted.");
                //    }
                //}

                //if (entity.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved)
                //{
                //    entity.Quote.DiscountRequestId = entity.DiscountRequestId;
                //    entity.Quote.ApprovedCommissionPercentage = entity.RequestedCommission;
                //    if (entity.ApprovedDiscount != null && entity.ApprovedDiscount > 0)
                //    {
                //        entity.Quote.ApprovedDiscountPercentage = entity.ApprovedDiscount.Value;
                //    }
                //    else
                //    {
                //        entity.Quote.ApprovedDiscountPercentage = entity.RequestedDiscount;

                //    }

                //    entity.Quote.AwaitingDiscountRequest = false;

                //    RecalulateQuote(user, entity);

                //    if (this.Response.IsOK) this.Response.AddSuccess("Discount request approved.");
                //}

            }

        }

        private void RecalulateQuote(UserSessionModel user, DiscountRequest entity)
        {
            Entry = Db.Entry(entity.Quote);

            entity.Quote.RecalculationRequired = true;

            Entry.State = EntityState.Modified;

            new QuoteServices(this, "Quote").ApplyBusinessRules(user, entity.Quote);
        }

        

        
    }
}
