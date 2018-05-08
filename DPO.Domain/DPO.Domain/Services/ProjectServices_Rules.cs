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
using System.Linq;
using System.Linq.Expressions;
using DPO.Model.Light;

namespace DPO.Domain
{

    public partial class ProjectServices : BaseServices
    {

        /// <summary>
        /// Supports only partial update based on available edit functionality in the projects listing
        /// </summary>
        /// <param name="model"></param>
        public void RulesOnValidateModel(ProjectsModel model)
        {
            if (model == null || model.Items == null || model.Items.Count == 0)
            {
                return;
            }

            foreach (var project in model.Items)
            {
                Validation.IsDateSet(this.Response.Messages, project.BidDate, "BidDate", "Bid Date");
                Validation.IsDateSet(this.Response.Messages, project.EstimatedClose, "EstimatedClose", "Estimated Close");
                Validation.IsDateSet(this.Response.Messages, project.EstimatedDelivery, "EstimatedDelivery", "Estimated Delivery");

                // TODO:  Cleanup all these casts
                Validation.IsDropDownSet(this.Response.Messages, (int)project.ProjectStatusId, "ProjectStatusId", "Project Status");
                Validation.IsDropDownSet(this.Response.Messages, project.ProjectTypeId, "ProjectTypeId", "Project Type");
                Validation.IsDropDownSet(this.Response.Messages, project.ProjectOpenStatusId, "ProjectOpenStatusId", "Project Open Status");
            }
        }

        public void RulesOnValidateModel(ProjectsGridViewModel model)
        {
            if (model == null || model.Items == null || model.Items.Count == 0)
            {
                return;
            }

            foreach (var project in model.Items)
            {
                Validation.IsDateSet(this.Response.Messages, project.BidDate, "BidDate", "Bid Date");
                Validation.IsDateSet(this.Response.Messages, project.EstimatedClose, "EstimatedClose", "Estimated Close");
                Validation.IsDateSet(this.Response.Messages, project.EstimatedDelivery, "EstimatedDelivery", "Estimated Delivery");

                // TODO:  Cleanup all these casts
                Validation.IsDropDownSet(this.Response.Messages, (int)project.ProjectStatusId, "ProjectStatusId", "Project Status");
                Validation.IsDropDownSet(this.Response.Messages, project.ProjectTypeId, "ProjectTypeId", "Project Type");
                Validation.IsDropDownSet(this.Response.Messages, project.ProjectOpenStatusId, "ProjectOpenStatusId", "Project Open Status");
            }
        }

        public void RulesOnValidateModel(ProjectModel model)
        {
            Validation.IsText(this.Response.Messages, model.Name, "Name", "Project name", 255, true);

            Validation.IsText(this.Response.Messages, model.Description, "Description", "Description", 2, 1000, false);

            Validation.IsText(this.Response.Messages, model.CustomerName, "CustomerName", "Dealer/Contractor name", 255, false);

            Validation.IsText(this.Response.Messages, model.EngineerName, "EngineerName", "Engineer name", 255, false);

            Validation.IsText(this.Response.Messages, model.SellerName, "Seller Name", "Engineer name", 255, false);

            Validation.IsText(this.Response.Messages, model.ShipToName, "Ship To Name", "Ship to name", 255, false);

            // Validation.IsDateSet(this.Response.Messages, model.ProjectDate, "ProjectDate", "Project Date");
            Validation.IsDateSet(this.Response.Messages, model.ProjectDate, "ProjectDate", "Project Date");
            Validation.IsDateSet(this.Response.Messages, model.BidDate, "BidDate", "Bid Date");
            Validation.IsDateSet(this.Response.Messages, model.EstimatedClose, "EstimatedClose", "Estimated Close");
            Validation.IsDateSet(this.Response.Messages, model.EstimatedDelivery, "EstimatedDelivery", "Estimated Delivery");

            addressService.BeginPropertyReference(this, "EngineerAddress");
            addressService.RulesOnValidateModel(model.EngineerAddress, false);
            addressService.EndPropertyReference();

            addressService.BeginPropertyReference(this, "CustomerAddress");
            addressService.RulesOnValidateModel(model.CustomerAddress, false);
            addressService.EndPropertyReference();

            addressService.BeginPropertyReference(this, "SellerAddress");
            addressService.RulesOnValidateModel(model.SellerAddress, false);
            addressService.EndPropertyReference();

            addressService.BeginPropertyReference(this, "ShipToAddress");
            addressService.RulesOnValidateModel(model.ShipToAddress, false);
            addressService.EndPropertyReference();

            Validation.IsDropDownSet(this.Response.Messages, model.ConstructionTypeId, "ConstructionTypeId", "Construction Type");
            Validation.IsDropDownSet(this.Response.Messages, model.ProjectStatusTypeId, "ProjectStatusTypeId", "Project Status");
            Validation.IsDropDownSet(this.Response.Messages, model.ProjectTypeId, "ProjectTypeId", "Project Type");
            Validation.IsDropDownSet(this.Response.Messages, model.ProjectOpenStatusTypeId, "ProjectOpenStatusTypeId", "Project Open Status");
            Validation.IsDropDownSet(this.Response.Messages, model.VerticalMarketTypeId, "VerticalMarketTypeId", "Vertical Market");

            ProjectServices projectService = new ProjectServices();

        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel user, object entity)
        {
            var project = entity as Project;

            if (project == null)
            {
                this.Response.Messages.AddError("Project entity not loaded");
                return;
            }

            // Code error prevention
            if (user == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM008);
                return;
            }
            
            // Setup default project lead status
            if (project.ProjectLeadStatusType == null)
            {
                project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Lead;
            }

            project.OwnerId = user.UserId;

            RulesForDropDowns(user, project);

            RulesForProjectName(user, project);

            RulesForProjectDates(user, project);

            // Check quote (i.e if project has been duplicated)
            foreach (var quote in project.Quotes)
            {
                new QuoteServices().ApplyBusinessRules(user, quote);
            }

        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var project = entity as Project;

            if (project == null)
            {
                //throw new ArgumentException("Project entity not loaded");
                this.Response.Messages.AddError("Project entity not loaded");
                return;
            }

            //###############################################################
            // Make sure nothing can change unless the project status is open
            //###############################################################

            if (project.ProjectStatusTypeId != ProjectStatusTypeEnum.Open)
            {
                if (Entry != null)
                {
                    //get project status prior to editing
                    object projectStatusBeforeEdit = Entry.OriginalValues.GetValue<object>("ProjectStatusTypeId");

                    //allow for changes if changing project status to closed from open
                    if ((ProjectStatusTypeEnum)projectStatusBeforeEdit != ProjectStatusTypeEnum.Open)
                    {
                        //disallow changes to anything other than project status or notes, when project is not open

                        if (Entry.CurrentValues.PropertyNames.Any(p => p != "ProjectStatusTypeId" && p != "Description" && Entry.Property(p).IsModified))
                        {
                            this.Response.Messages.AddError("ProjectStatusTypeId", "[" + project.Name + "] "+ Resources.ResourceModelProject.MP012);
                        }
                    }
                }
                else  // this is for Unit Test-Aaron Nguyen 07/15/2016
                {
                    if (typeof(Project).GetProperties().Any(p => p.Name != "ProjectStatusTypeId" && p.Name != "Description") && project.ProjectStatusTypeId == (ProjectStatusTypeEnum.Open))
                    {
                        this.Response.Messages.AddError("ProjectStatusTypeId", Resources.ResourceModelProject.MP012);

                    }
                }
            }

            //Change/Update the Est. Close date to Actual close date when project is closed won, closed lost, etc
            if(project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon ||
               project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost)
            {
                project.EstimatedClose = (project.ActualCloseDate!= null && project.ActualCloseDate != DateTime.MinValue) ? project.ActualCloseDate.Value : project.EstimatedClose;
                project.EstimatedDelivery = (project.ActualDeliveryDate != null && project.ActualDeliveryDate != DateTime.MinValue) ? project.ActualDeliveryDate.Value : project.EstimatedDelivery;
            }

            RulesForDropDowns(admin, project);

            RulesForProjectName(admin, project);

            RulesForProjectDates(admin, project);

            RulesForProjectTransfer(admin, project);

            RulesForDiscountRequests(admin, project);

            RulesForPipelineStatus(admin, project);
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
            var project = entity as Project;

            if (project == null)
            {
                throw new ArgumentException("Project entity not loaded");
            }

            RulesForProjectTransfer(admin, project);
        }

        public void RulesForDropDowns(UserSessionModel admin, Project project)
        {
            if (project.ProjectStatusTypeId <= 0)
            {
                this.Response.Messages.AddError("ProjectStatusTypeId", Resources.ResourceModelProject.MP013);
            }

            if (project.ProjectTypeId <= 0)
            {
                this.Response.Messages.AddError("ProjectTypeId", Resources.ResourceModelProject.MP013);
            }

            if (project.ProjectOpenStatusTypeId <= 0)
            {
                this.Response.Messages.AddError("ProjectOpenStatusTypeId", Resources.ResourceModelProject.MP013);
            }

            if (project.VerticalMarketTypeId <= 0)
            {
                this.Response.Messages.AddError("VerticalMarketTypeId", Resources.ResourceModelProject.MP013);
            }
        }

        public void RulesForProjectName(UserSessionModel admin, Project project)
        {
            //mass upload change - turned this off
            if (base.Entry != null)
            {
                if (base.Entry.HasChanged("Name") && Db.IsProjectNameUnique(admin, project.Name))
                {
                    this.Response.Messages.AddError("Name", Resources.ResourceModelProject.MP001);
                }
            }
            else
            {
                var result = this.Db.Projects.Where(p => p.OwnerId == admin.UserId && p.Name == project.Name).Count();
                if(result > 0)
                {
                    this.Response.Messages.AddError("Name", Resources.ResourceModelProject.MP001);
                }
            }
            
        }

        public void RulesForProjectDates(UserSessionModel admin, Project project)
        {
            if (Entry != null)
            {
                if (Entry.State == EntityState.Modified && Entry.HasChanged("ProjectDate"))
                {
                    this.Response.Messages.AddError("ProjectDate", "[" + project.Name + "] " + Resources.ResourceModelProject.MP011);
                }
            }
            
            if (project.BidDate.Date < project.ProjectDate.Date)
            {
                this.Response.Messages.AddError("BidDate", "[" + project.Name + "] " + Resources.ResourceModelProject.MP003);
            }

            if (project.EstimatedClose.Date < project.BidDate.Date)
            {
                this.Response.Messages.AddError("EstimatedClose", "[" + project.Name + "] " + Resources.ResourceModelProject.MP002);
            }

            if (project.EstimatedDelivery.Date < project.EstimatedClose.Date)
            {
                this.Response.Messages.AddError("EstimatedDelivery", "[" + project.Name + "] " + Resources.ResourceModelProject.MP006);
            }
        }

        public void RulesForProjectTransfer(UserSessionModel admin, Project project)
        {
            if (Entry != null)
            {
                if (Entry.State != EntityState.Unchanged && Db.IsProjectTransferred(admin, project.ProjectId))
                {
                    this.Response.Messages.AddError("[" + project.Name + "] " + Resources.ResourceModelProject.MP027);
                }
            }
            else 
            {
                if(Db.IsProjectTransferred(admin, project.ProjectId))
                {
                    this.Response.Messages.AddError(Resources.ResourceModelProject.MP027);

                }
            }
        }

        public void RulesForDiscountRequests(UserSessionModel admin, Project project)
        {
            // block customer/customer admin and super user from editing project status to 
            // closed-won/closed-lost if a discount request is pending

            if (admin.UserTypeId == UserTypeEnum.CustomerUser || admin.UserTypeId == UserTypeEnum.CustomerAdmin || admin.UserTypeId == UserTypeEnum.CustomerSuperUser)
            {
                if (project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon || project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost)
                {
                    // project.Quotes is always length 0, have to pull out the full project model
                    // to access the ActiveQuoteSummary
                    ServiceResponse response = GetProjectModel(admin, project.ProjectId);
                    if (response != null)
                    {
                        ProjectModel projectModel = (ProjectModel)response.Model;

                        if (projectModel.ActiveQuoteSummary.AwaitingDiscountRequest)
                        {
                            this.Response.Messages.AddError(Resources.ResourceModelProject.MP116);
                        }
                    }
                }
            }
        }

        public void RulesForCommissionRequests(UserSessionModel admin, Project project)
        {
            if (admin.UserTypeId == UserTypeEnum.CustomerUser || admin.UserTypeId == UserTypeEnum.CustomerAdmin || admin.UserTypeId == UserTypeEnum.CustomerSuperUser)
            {
                if (project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon || project.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost)
                {
                    // project.Quotes is always length 0, have to pull out the full project model
                    // to access the ActiveQuoteSummary
                    ServiceResponse response = GetProjectModel(admin, project.ProjectId);
                    if (response != null)
                    {
                        ProjectModel projectModel = (ProjectModel)response.Model;

                        if (projectModel.ActiveQuoteSummary.AwaitingCommissionRequest)
                        {
                            this.Response.Messages.AddError(Resources.ResourceModelProject.MP120);
                        }
                    }
                }
            }
        }

        public void RulesForPipelineStatus(UserSessionModel admin, Project project)
        {
            //If Project Status changed from 'Inactive' to 'Open', change pipeline status to 'Opportunity'. 
            if(project.ProjectStatusTypeId == ProjectStatusTypeEnum.Open)
            {
                var response = GetProjectModel(admin, project.ProjectId);
                if(response != null){
                    var oldmodel = (ProjectModel)response.Model;
                    if(oldmodel.ProjectStatusTypeId == (byte?)ProjectStatusTypeEnum.Inactive){
                        project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Opportunity;
                    }
                }
            }

            //If project Open Status changed to 'Submittals' or 'Rep has PO', change pipeline status to 'Opportunity'.
            if (project.ProjectOpenStatusTypeId == (byte)ProjectOpenStatusTypeEnum.RepHasPO || project.ProjectOpenStatusTypeId == (byte)ProjectOpenStatusTypeEnum.Submittal)
            {
                project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Opportunity;
            }

            //If project has approved DAR , change pipeline status to 'Opportunity'.
            //ServiceResponse serviceResponse = GetProjectQuoteDARModel(admin, project.ProjectId);
            ServiceResponse serviceResponse = GetProjectModel(admin, project.ProjectId);
            if (serviceResponse != null)
            {
                ProjectModel projectModel = (ProjectModel)serviceResponse.Model;

                if (projectModel.ActiveQuoteSummary.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved)
                {
                    project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Opportunity;
                }
            }

            //If project status changed to 'Inactive', change pipeline status to 'Disqualified'.
            if (project.ProjectStatusTypeId == ProjectStatusTypeEnum.Inactive)
            {
                project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Disqualified;
            }

            // Add project note when pipeline status is changed from 'Lead' to 'Opportunity'     
            if (project.ProjectLeadStatusTypeId == ProjectLeadStatusTypeEnum.Opportunity && 
                !string.IsNullOrEmpty(project.ProjectId.ToString()))
            {
                // get old ProjectLeadStatusTypeId
                ServiceResponse response = GetProjectModel(admin, project.ProjectId);
                if (response != null)
                {
                    ProjectModel oldmodel = (ProjectModel)response.Model;
                    if (oldmodel.ProjectLeadStatusTypeId == ProjectLeadStatusTypeEnum.Lead)
                    {
                       //Add note
                        var newNoteModel = new ProjectPipelineNoteModel();
                        newNoteModel.ProjectId = project.ProjectId;
                        newNoteModel.Note = Resources.ResourceUI.ConvertToOpportunity;

                        newNoteModel.ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel()
                        {
                            ProjectPipelineNoteTypeId = 1,
                            Name = @Resources.ResourceUI.ProjectPipelineNoteTypeName1
                        };

                        AddProjectPipelineNote(admin, newNoteModel);
                    }
                }
                              
            }

        }
    }
}