
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using NUnit.Framework;
using NUnit.Common;
using Resources = DPO.Resources;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using DPO.Domain.DataQualityService;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public partial class TestProjectServices_Rule : TestAdmin
    {
        UserSessionModel user = new UserSessionModel();

        ProjectServices projectService;
        SystemTestDataServices systemService;
        BusinessServices businessService;

        ProjectType projecttypes;
        ProjectOpenStatusType ProjectOpenStatus;
        ProjectStatusType projectstatus;
        VerticalMarketType verticaltype;

        ServiceResponse Response = new ServiceResponse();
        ProjectModel projectModel = new ProjectModel();
        ProjectsModel projectsModel = new ProjectsModel();

        long _projectId;
        long _quoteId;

        string returnMessage = null;
        string expectMessage = null;

        public TestProjectServices_Rule()
        {
            systemService = new SystemTestDataServices(this.TContext);
            projectService = new ProjectServices(this.TContext);
            businessService = new BusinessServices(this.TContext);

            user = GetUserSessionModel("User15@test.com");

            _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.Deleted == false).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            projectModel = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;

            projectsModel.ProjectId = _projectId;
            Response = projectService.GetProjectsModel(user, projectsModel);
     
            projectsModel = projectService.GetProjectsModel(user, projectsModel).Model as ProjectsModel;
            

            projecttypes = db.ProjectTypes.FirstOrDefault();
            ProjectOpenStatus = db.ProjectOpenStatusTypes.FirstOrDefault();
            projectstatus = db.ProjectStatusTypes.FirstOrDefault();
            verticaltype = db.VerticalMarketTypes.FirstOrDefault();
        }

        [Test]
        [Category("ProjectServicesRules")]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        public void TestProjectServicesRules_RulesOnValidateModel_ShouldPeformModelValidationForProjectsModel(int testValue)
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            switch (testValue)
            {
                case -1:
                    projectsModel.Items[0].ProjectStatusId = testValue;
                    projectService.RulesOnValidateModel(projectsModel);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("")), Is.EqualTo(true));
                    break;
                case -2:
                    projectsModel.Items[0].ProjectTypeId = testValue;
                    projectService.RulesOnValidateModel(projectsModel);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("")), Is.EqualTo(true));
                    break;
                case -3:
                    projectsModel.Items[0].ProjectOpenStatusId = testValue;
                    projectService.RulesOnValidateModel(projectsModel);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("")), Is.EqualTo(true));
                    break;
                case 0:
                    projectService.RulesOnValidateModel(projectsModel);
                    Assert.That(this.Response.HasError, Is.EqualTo(false));
                    break;
            }
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesOnValidateModel_ShouldPerformModelValidationForProjectModel()
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            ProjectModel _projectModel = new ProjectModel();
            _projectModel.Description = "for testing";
            _projectModel.CustomerName = "1234";
            _projectModel.EngineerName = "1234";
            _projectModel.SellerName = "1234";
            _projectModel.ShipToName = "1234";

            projectService.RulesOnValidateModel(_projectModel);

            Assert.That(this.Response.HasError, Is.EqualTo(true));

            if(_projectModel.Name == null)
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project name is required.")), Is.EqualTo(true));

            if (_projectModel.ProjectDate == null)
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project Date is required")), Is.EqualTo(true));
            
            if(_projectModel.BidDate == null)
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Bid Date is required")), Is.EqualTo(true));

            if(_projectModel.EstimatedClose == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Estimated Close is required")), Is.EqualTo(true));

            if(_projectModel.EstimatedDelivery == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Estimated Delivery is required")), Is.EqualTo(true));

            if(_projectModel.ConstructionTypeId == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Construction Type is required")), Is.EqualTo(true));

            if(_projectModel.ProjectStatusTypeId == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project Status is required")), Is.EqualTo(true));

            if(_projectModel.ProjectTypeId == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project Type is required")), Is.EqualTo(true));

            if(_projectModel.ProjectOpenStatusTypeId == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project Open Status is required")), Is.EqualTo(true));

            if(_projectModel.VerticalMarketTypeId == null)
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Vertical Market is required")), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        [TestCase("projectIsNull")]
        [TestCase("userIsNull")]
        [TestCase("rulesForDropDowns")]
        [TestCase("rulesForProjectName")]
        [TestCase("rulesForProjectDates")]
        public void TestProjectServicesRules_RuleOnAdd(string testCase)
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            Project _project = new Project(); 
            var originalUser = user;
     
            switch(testCase)
            {
                case "projectIsNull":
                    _project = null;
                    projectService.RulesOnAdd(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project entity not loaded")), Is.EqualTo(true));
                    break;
                case "userIsNull":
                    user = null;
                    projectService.RulesOnAdd(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.DataMessages.DM008)), Is.EqualTo(true));
                    break;
                case "rulesForDropDowns":
                    projectService.RulesOnAdd(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP013)), Is.EqualTo(true));
                    break;
                case "rulesForProjectName":
                    string projectName = projectModel.Name;
                    _project.Name = projectName;
                    projectService.RulesOnAdd(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP001)), Is.EqualTo(true));
                    break;
                case "rulesForProjectDates":
                    _project.ProjectDate = DateTime.Now;
                    _project.BidDate = DateTime.Now.AddDays(-1);
                    _project.EstimatedClose = _project.BidDate.AddDays(-1);
                    _project.EstimatedDelivery = _project.EstimatedClose.AddDays(-1);
                    projectService.RulesOnAdd(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP003)), Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP002)), Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP006)), Is.EqualTo(true));
                    break;
            }

            user = originalUser;
        }

        [Test]
        [Category("ProjectServicesRules")]
        [TestCase("projectIsNull")]
        [TestCase("userIsNull")]
        [TestCase("rulesForDropDowns")]
        [TestCase("rulesForProjectName")]
        [TestCase("rulesForProjectDates")]
        [TestCase("rulesForProjectTransfer")]
        [TestCase("rulesForDiscountRequests")]
        [TestCase("rulesForCommissionRequests")]
        public void TestProjectServicesRules_RuleOnEdit(string testCase)
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            Project _project = new Project();

            switch (testCase)
            {
                case "projectIsNull":
                    _project = null;
                    projectService.RulesOnEdit(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains("Project entity not loaded")), Is.EqualTo(true));
                    break;
                case "rulesForDropDowns":
                    projectService.RulesOnEdit(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP013)), Is.EqualTo(true));
                    break;
                case "rulesForProjectName":
                    string projectName = projectModel.Name;
                    _project.Name = projectName;
                    projectService.RulesOnEdit(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP001)), Is.EqualTo(true));
                    break;
                case "rulesForProjectDates":
                    _project.ProjectDate = DateTime.Now;
                    _project.BidDate = DateTime.Now.AddDays(-1);
                    _project.EstimatedClose = _project.BidDate.AddDays(-1);
                    _project.EstimatedDelivery = _project.EstimatedClose.AddDays(-1);
                    projectService.RulesOnEdit(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP003)), Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP002)), Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP006)), Is.EqualTo(true));
                    break;
                case "rulesForProjectTransfer":
                    long _projectId = this.db.Context.ProjectTransfers.Where(pt => pt.UserId == user.UserId).OrderByDescending(pt => pt.ProjectId).Select(pt => pt.ProjectId).FirstOrDefault();
                    _project.ProjectId = _projectId;
                    projectService.RulesOnEdit(user, _project);
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP027)), Is.EqualTo(true));
                    break;
                case "rulesForDiscountRequests":
                    var query = from p in this.db.Context.Projects
                                join q in this.db.Context.Quotes
                                on p.ProjectId equals q.ProjectId
                                where p.OwnerId == user.UserId && q.AwaitingDiscountRequest == true
                                select new
                                {
                                    p
                                };
                    var result = query.FirstOrDefault();
                    Project projectWithDar = result.p as Project;
                    projectWithDar.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedWon;

                    projectService.RulesOnEdit(user, projectWithDar);
       
                    Assert.That(this.Response.HasError, Is.EqualTo(true));
                    Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP116)), Is.EqualTo(true));
                    break;
            }
        }

        [Test]
        [Category("ProjectServicesRules")]
        [TestCase("NonTransferProject")]
        [TestCase("TransferProject")]
        public void TestProjectServicesRules_RuleOnDelete(string testValue)
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            if (testValue == "NonTransferProject")
            {
                var query = from p in this.db.Context.Projects
                             where !this.db.Context.ProjectTransfers.Any(pt => pt.ProjectId == p.ProjectId)
                             select new { Project = p } ;

                var _project = query.FirstOrDefault().Project;

                projectService.RulesOnDelete(user, _project);

                Assert.That(this.Response.HasError, Is.EqualTo(false));
            }

            if( testValue == "TransferProject")
            {
                var query = from p in this.db.Context.Projects
                            join pt in this.db.Context.ProjectTransfers
                            on p.ProjectId equals pt.ProjectId
                            where pt.UserId == user.UserId
                            select new { Project = p };

                var _project = query.FirstOrDefault().Project;

                projectService.RulesOnDelete(user, _project);

                Assert.That(this.Response.HasError, Is.EqualTo(true));
                Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP027)), Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesForDropDowns()
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            Project _project = new Project();
            projectService.RulesForDropDowns(user, _project);

            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP013)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesForProjectName()
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            Project _project = new Project();
            _project.Name = projectModel.Name;

            projectService.RulesForProjectName(user, _project);
            Assert.That(this.Response.Messages.HasErrors, Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP001)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesForProjectDates()
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            Project _project = new Project();
            _project.ProjectDate = DateTime.Now;
            _project.BidDate = _project.ProjectDate.AddDays(-1);
            _project.EstimatedClose = _project.BidDate.AddDays(-1);
            _project.EstimatedDelivery = _project.EstimatedClose.AddDays(-1);

            projectService.RulesForProjectDates(user, _project);

            Assert.That(this.Response.HasError, Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP003)), Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP002)), Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP006)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesForProjectTransfer()
        {
            var query = from p in this.db.Context.Projects
                        join pt in this.db.Context.ProjectTransfers
                        on p.ProjectId equals pt.ProjectId
                        where pt.UserId == user.UserId
                        select new { Project = p };

            var _project = query.FirstOrDefault().Project;

            projectService.RulesForProjectTransfer(user, _project);

            Assert.That(this.Response.HasError, Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP027)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        public void TestProjectServicesRules_RulesForDiscountRequests()
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            var query = from p in this.db.Context.Projects
                        join q in this.db.Context.Quotes
                        on p.ProjectId equals q.ProjectId
                        where p.OwnerId == user.UserId && q.AwaitingDiscountRequest == true
                        select new
                        {
                            p
                        };
            var result = query.FirstOrDefault();
            Project projectWithDar = result.p as Project;
            projectWithDar.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedWon;

            projectService.RulesForDiscountRequests(user, projectWithDar);
            Assert.That(this.Response.HasError, Is.EqualTo(true));
            Assert.That(this.Response.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP116)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServicesRules")]
        [TestCase("ChangeFromActiveToOpen")]
        [TestCase("ChangeToOpportunityWhenProjectOpentatusIsSubmittal")]
        [TestCase("ChangeToOpportinityWhenDARIsApproved")]
        [TestCase("ChangeFromInActiveToDisqualified")]
        [TestCase("AddNoteWhenChangeFromLeadToOpportunity")]
        public void TestProjectServicesRules_RulesForPipelineStatus(string testValue)
        {
            this.Response.Messages.Clear();
            this.Response.Messages.HasErrors = false;

            var query1 = from p in this.db.Context.Projects
                         join q in this.db.Context.Quotes
                         on p.ProjectId equals q.ProjectId
                         where p.OwnerId == user.UserId && q.AwaitingDiscountRequest == true
                         && p.ProjectStatusTypeId == ProjectStatusTypeEnum.Inactive
                         select new
                         {
                             p
                         };
            var result1 = query1.FirstOrDefault();
            Project _project = result1.p as Project;
        
            switch (testValue)
            {
                case "ChangeFromInActiveToOpen":
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;
                    projectService.RulesForPipelineStatus(user, _project);
                    Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(ProjectLeadStatusTypeEnum.Opportunity));
                    break;
                case "ChangeToOpportunityWhenProjectOpentatusIsSubmittal":
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;
                    _project.ProjectOpenStatusTypeId = (byte)ProjectOpenStatusTypeEnum.Submittal;
                    projectService.RulesForPipelineStatus(user, _project);
                    Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(ProjectLeadStatusTypeEnum.Opportunity));
                    break;
                case "ChangeToOpportinityWhenDARIsApproved":
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;
                    projectService.RulesForPipelineStatus(user, _project);
                    Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(ProjectLeadStatusTypeEnum.Opportunity));
                    break;
                case "ChangeFromInActiveToDisqualified":
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Inactive;
                    projectService.RulesForPipelineStatus(user, _project);
                    Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(ProjectLeadStatusTypeEnum.Disqualified));
                    break;
                case "AddNoteWhenChangeFromLeadToOpportunity":
                    _project.ProjectStatusTypeId = ProjectStatusTypeEnum.Open;
                    _project.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Opportunity;
                    projectService.RulesForPipelineStatus(user, _project);
                    ProjectPipelineNote _model = this.db.Context.ProjectPipelineNotes.Where(ppn => ppn.ProjectId == _project.ProjectId).FirstOrDefault();
                    Assert.That(_model, Is.Not.EqualTo(null));
                    Assert.That(_model.ProjectId, Is.EqualTo(_project.ProjectId));
                    Assert.That(_model.Note, Is.EqualTo(Resources.ResourceUI.ConvertToOpportunity));
                    break;
            }

        }
    }
}
