
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
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
using System.Net.Http;
using System.Net.Http.Headers;

namespace DaikinProjectOffice.Tests
{

    [TestFixture]
    public partial class TestProjectServices : TestAdmin
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

        long _projectId;
        long _quoteId;        
        string returnMessage = null;
        string expectMessage = null;

        public TestProjectServices()
        {
            systemService = new SystemTestDataServices(this.TContext);
            projectService = new ProjectServices(this.TContext);
            businessService = new BusinessServices(this.TContext);

            user = GetUserSessionModel("User15@test.com");

            _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            projectModel = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;

            projecttypes = db.ProjectTypes.FirstOrDefault();
            ProjectOpenStatus = db.ProjectOpenStatusTypes.FirstOrDefault();
            projectstatus = db.ProjectStatusTypes.FirstOrDefault();
            verticaltype = db.VerticalMarketTypes.FirstOrDefault();
        }

        [Test]
        [Category("ProjectServices")]
        [TestCase(1, "Testing...")]
        public void TestProjectServices_GenerateQuotePackageCoverPageFile_ShouldRenderPdfFile(long quoteId, string content)
        {
            string file = projectService.GenerateQuotePackageCoverPageFile(quoteId, content);
            Assert.That(file, Is.Not.EqualTo(null));

            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(file))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }

            Assert.That(text.ToString().Contains(content), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServices_GET")]
        public void TestProjectServices_GetProjectExportModel_ShouldReturnHSSFWorkbook()
        {
            ProjectsModel _projectsModel = new ProjectsModel();
            _projectsModel = projectService.GetProjectsModel(user, _projectsModel).Model as ProjectsModel;

            Response = projectService.GetProjectExportModel(user, _projectsModel);

            NPOI.HSSF.UserModel.HSSFWorkbook _model = Response.Model as NPOI.HSSF.UserModel.HSSFWorkbook;

            Assert.That(Response.IsOK, Is.EqualTo(true));

            if (_model != null)
            {
                Assert.That(_model.Count, Is.GreaterThan(0));
                Assert.That(_model.Workbook.NumRecords, Is.GreaterThan(1));
                Assert.That(_model.Workbook.NumSheets, Is.GreaterThan(0));
                Assert.That(_model.Workbook.NumRecords, Is.GreaterThan(_projectsModel.Items.Count));
            }
        }

        [Test]
        [Category("ProjectServices_GET")]
        public void TestProjectServices_GetProjectModel_ShouldReturnProjectModel()
        {
            Response = projectService.GetProjectModel(user, _projectId);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            ProjectModel _model = Response.Model as ProjectModel;
            Assert.That(_model, Is.Not.EqualTo(null));
            Assert.That(_model.Quotes, Is.Not.EqualTo(null));
            Assert.That(_model.Name, Is.Not.EqualTo(null));
            Assert.That(_model.ProjectDate, Is.Not.EqualTo(null));
            Assert.That(_model.ProjectOpenStatusTypes, Is.Not.EqualTo(null));
            Assert.That(_model.ProjectStatusTypes, Is.Not.EqualTo(null));
            Assert.That(_model.VerticalMarketTypes, Is.Not.EqualTo(null));
            Assert.That(_model.ProjectTypeId, Is.Not.EqualTo(null));
            Assert.That(_model.BidDate, Is.Not.EqualTo(null));
            Assert.That(_model.BidDate, Is.GreaterThan(DateTime.MinValue));
            Assert.That(_model.EstimatedClose, Is.Not.EqualTo(null));
            Assert.That(_model.EstimatedClose, Is.GreaterThan(DateTime.MinValue));
            Assert.That(_model.ActiveQuoteSummary, Is.Not.EqualTo(null));

            Assert.That(_model.EngineerAddress.AddressId, Is.Not.EqualTo(null));
            Assert.That(this.db.Context.Addresses.Where(ad => ad.AddressId == _model.EngineerAddress.AddressId).Count(), Is.EqualTo(1));
            Assert.That(_model.EngineerAddress, Is.Not.EqualTo(null));

            Assert.That(_model.SellerAddress.AddressId, Is.Not.EqualTo(null));
            Assert.That(this.db.Context.Addresses.Where(ad => ad.AddressId == _model.SellerAddress.AddressId).Count(), Is.EqualTo(1));
            Assert.That(_model.SellerAddress, Is.Not.EqualTo(null));

            Assert.That(_model.CustomerAddress.AddressId, Is.Not.EqualTo(null));
            Assert.That(this.db.Context.Addresses.Where(ad => ad.AddressId == _model.CustomerAddress.AddressId).Count(), Is.EqualTo(1));
            Assert.That(_model.CustomerAddress, Is.Not.EqualTo(null));

            Assert.That(_model.ShipToAddress.AddressId, Is.Not.EqualTo(null));
            Assert.That(this.db.Context.Addresses.Where(ad => ad.AddressId == _model.ShipToAddress.AddressId).Count(), Is.EqualTo(1));
            Assert.That(_model.ShipToAddress, Is.Not.EqualTo(null));
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(206262985251880960)]
        public void TestProjectServices_GetProjectPipelineNoteListModel_ShouldReturnListOfProjectPipelineNoteListModel(long projectId)
        {
            Response = projectService.GetProjectPipelineNoteListModel(user, projectId);
            Assert.That(Response.IsOK, Is.EqualTo(true));

            ProjectPipelineNoteListModel _model = Response.Model as ProjectPipelineNoteListModel;
            Assert.That(_model, Is.Not.EqualTo(null));
            Assert.That(_model.Items.Count, Is.GreaterThan(1));
            Assert.That(_model.Page, Is.GreaterThan(0));
            Assert.That(_model.PageSizes.Count(), Is.GreaterThan(1));

            ProjectPipelineNoteModel item = _model.Items.FirstOrDefault();
            Assert.That(this.db.Context.ProjectPipelineNotes.Where(ppn => ppn.ProjectPipelineNoteId == item.ProjectPipelineNoteId).Any(), Is.EqualTo(true));
            Assert.That(this.db.Context.ProjectPipelineNoteTypes.Where(ppt => ppt.ProjectPipelineNoteTypeId == item.ProjectPipelineNoteType.ProjectPipelineNoteTypeId).Any(), Is.EqualTo(true));

            Assert.That(_model.Items.Any(i => i.ProjectPipelineNoteType.Name.Contains("Pull Estimated Delivery Forward")), Is.EqualTo(true));
            Assert.That(_model.Items.Any(i => i.ProjectPipelineNoteType.Name.Contains("Project On Hold")), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(484641374210097152)]
        public void TestProjectServices_GetProjectQuotesModel_ShouldReturnProjectQuotesModel(long projectId)
        {
            ProjectQuotesModel _model = new ProjectQuotesModel();
            _model.ProjectId = projectId;
            Response = projectService.GetProjectQuotesModel(user, _model);

            Quote quote = this.db.Context.Quotes.Where(q => q.ProjectId == projectId).FirstOrDefault();
            Assert.That(Response.IsOK, Is.EqualTo(true));

            _model = Response.Model as ProjectQuotesModel;

            Assert.That(_model.Items.Count(), Is.GreaterThan(0));

            QuoteListModel _quoteListModel = _model.Items.First() as QuoteListModel;

            Assert.That(_quoteListModel, Is.Not.EqualTo(null));
            Assert.That(_quoteListModel.Active, Is.EqualTo(quote.Active));
            Assert.That(_quoteListModel.Title, Is.EqualTo(quote.Title));
            Assert.That(_quoteListModel.TotalList, Is.EqualTo(quote.TotalList));
            Assert.That(_quoteListModel.TotalNet, Is.EqualTo(quote.TotalNet));
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(484641374210097152)]
        public void TestProjectServices_GetProjectsModel_ShouldReturnListOfProjectListModel(long projectId)
        {
            ProjectsModel _projectModel = new ProjectsModel();
            _projectModel.ProjectId = projectId;
            Response = projectService.GetProjectsModel(user, _projectModel);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            _projectModel = Response.Model as ProjectsModel;
            ProjectListModel _model = _projectModel.Items[0];

            Assert.That(_model, Is.Not.EqualTo(null));
            Assert.That(_model.ActiveQuoteSummary, Is.Not.EqualTo(null));
            Assert.That(_model.Name, Is.Not.Empty);
            Assert.That(_model.ProjectDate, Is.Not.EqualTo(null));
            Assert.That(_model.CustomerName, Is.Not.Empty);
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(476969808328392704)]
        public void TestProjectServices_GetProjectQuoteDARModel_ShouldReturnDARModelBelongToQuoteInProject(long projectId)
        {
            Response = projectService.GetProjectQuoteDARModel(user, projectId);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.Model, Is.Not.Null);
            ProjectModel _model = Response.Model as ProjectModel;
            Assert.That(_model.ActiveQuoteSummary.HasDAR, Is.EqualTo(true));
            
        }

        [Test]
        [Category("ProjectServices")]
        [TestCase(476969808328392704, 476970850038317056, false)]
        [TestCase(476969808328392704, 476970850038317056, true)]
        public void TestProjectServices_QuotePrintExcelFile_ShouldReturnStreamMemoryForExcel(long projectId, long quoteId, bool showPrice)
        {
            MemoryStream stream = projectService.QuotePrintExcelFile(user, projectId, quoteId, showPrice);
            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.Length, Is.GreaterThan(0));
        }

        [Test]
        public void TestProjectServices_Rules_Project_Date_Before_Delivery_Date()
        {
            var model = GetProjectModel(user, "Project 1");

            model.EstimatedDelivery = model.ProjectDate.Value.AddDays(-1);

            var response = projectService.PostModel(user, model);

            Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP006), Is.True);
        }

        [Test]
        [Category("ProjectServices_GET")]     
        public void TestProjectServices_GetProjectPipelineNoteTypes_ShouldReturnListOfProjectPipeLineNoteType()
        {
            Response = projectService.GetProjectPipelineNoteTypes(user);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            ProjectPipelineNoteTypeListModel _models = Response.Model as ProjectPipelineNoteTypeListModel;
            Assert.That(_models, Is.Not.Null);
            Assert.That(_models.Items.Count, Is.GreaterThan(0));
            Assert.That(_models.Items[0].ProjectPipelineNoteTypeId, Is.Not.Null);
            Assert.That(_models.Items[0].Name, Is.EqualTo("Convert to Opportunity"));
        }

        [Test]
        [Category("ProjectServices_GET")]
        public void TestProjectServices_GetProjectDateTypes_ShouldReturnListOfDataTypeModel()
        {
            List<DPO.Common.Models.General.DateTypeModel> _models = projectService.GetProjectDateTypes().ToList();
            Assert.That(_models, Is.Not.Null);
            Assert.That(_models.Count(), Is.GreaterThan(0));
            Assert.That(_models.First().Name, Is.EqualTo("Registration Date"));
        }

        [Test]
        [Category("ProjectServices_Delete")]
        public void TestProjectServices_Delete()
        {
            long projectId = this.db.Context.Projects.Where(p => p.Name.Contains("AA")).Select(p => p.ProjectId).FirstOrDefault();
            ProjectModel _model = projectService.GetProjectModel(user, projectId).Model as ProjectModel;
            Response = projectService.Delete(user, _model);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));
        }

        [Test]
        [Category("ProjectServices_Delete")]
        public void TestProjectServices_DeleteProjects()
        {
            List<long> _projectIds = this.db.Context.Projects.Where(p => p.Name.Contains("AA")).Select(p => p.ProjectId).Take(3).ToList();
            List<ProjectModel> _models = new List<ProjectModel>();
            for (int i = 0; i < _projectIds.Count; i++)
            {
                ProjectModel model = projectService.GetProjectModel(user, _projectIds[i]).Model as ProjectModel;
                if (model != null)
                    _models.Add(model);
            }

            Response = projectService.DeleteProjects(user, _models);

            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));

            _models.Clear();

            for (int i = 0; i < _projectIds.Count; i++)
            {
                ProjectModel model = projectService.GetProjectModel(user, _projectIds[i]).Model as ProjectModel;
                if (model != null)
                    _models.Add(model);
            }

            foreach (ProjectModel model in _models)
            {
                Assert.That(model.Deleted, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServices_POST")]
        public void TestProjectServices_ModelToEntity_ShouldReturnEntityMatchProjectModelValues()
        {
            long projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).Select(p => p.ProjectId).FirstOrDefault();

            ProjectsModel _projectModel = new ProjectsModel();

            _projectModel.ProjectId = projectId;
            Response = projectService.GetProjectsModel(user, _projectModel);

            Assert.That(Response.IsOK, Is.EqualTo(true));

            _projectModel = Response.Model as ProjectsModel;

            ProjectListModel _model = new ProjectListModel();

            if (_projectModel.Items.Count() > 0)
            {
               _model = _projectModel.Items[0];
            }

           Project _project = projectService.ModelToEntity(user, _model);

            Assert.That(_project, Is.Not.Null);
            Assert.That(_project.BidDate, Is.EqualTo(_model.BidDate));
            Assert.That(_project.ERPFirstOrderComment, Is.EqualTo(_model.ERPFirstOrderComment));
            Assert.That(_project.ERPFirstOrderDate, Is.EqualTo(_model.ERPFirstOrderDate));
            Assert.That(_project.ERPFirstOrderNumber, Is.EqualTo(_model.ERPFirstOrderNumber));
            Assert.That(_project.ERPFirstPONumber, Is.EqualTo(_model.ERPFirstPONumber));
            Assert.That(_project.EstimatedClose, Is.EqualTo(_model.EstimatedClose));
            Assert.That(_project.EstimatedDelivery, Is.EqualTo(_model.EstimatedDelivery));
            Assert.That(_project.Name, Is.EqualTo(_model.Name));
            Assert.That(_project.ProjectDate, Is.EqualTo(_model.ProjectDate));
            Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(_model.ProjectLeadStatusId));
            Assert.That(_project.ProjectOpenStatusTypeId, Is.EqualTo(_model.ProjectOpenStatusId));

            if(_project.Owner == null)
            {
               var _projectOwner = this.db.Context.Users.Where(u => u.UserId == _project.OwnerId).Select(u => new { u.FirstName, u.LastName }).FirstOrDefault();
                _project.Owner = new User();
                _project.Owner.FirstName = _projectOwner.FirstName;
                _project.Owner.LastName = _projectOwner.LastName;
            }

            Assert.That(_project.Owner.FirstName + " " + _project.Owner.LastName, Is.EqualTo(_model.ProjectOwner));
            Assert.That(_project.ProjectStatusType.Description, Is.EqualTo(_model.ProjectStatus));
            Assert.That(_project.ProjectTypeId, Is.EqualTo(_model.ProjectTypeId));
            Assert.That(_project.ShipToName, Is.EqualTo(_model.ShipToName));
        }

        [Test]
        [Category("ProjectServices_POST")]
        public void TestProjectServices_UpdateProjectOnCommissionRequest()
        {
            Response = projectService.UpdateProjectOnCommissionRequest(user, projectModel);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));
            Assert.That(Response.Messages.Items.Any(m => m.Text.Contains("Project updated ")), Is.EqualTo(true));
            Assert.That(Response.Model, Is.Not.Null);

            var model = Response.Model as ProjectModel;
            Assert.That(model.ProjectId, Is.EqualTo(projectModel.ProjectId));
            Assert.That(model.ProjectDate, Is.EqualTo(projectModel.ProjectDate));
            Assert.That(model.ProjectStatusTypeId, Is.EqualTo(projectModel.ProjectStatusTypeId));
            Assert.That(model.ProjectOpenStatusTypeId, Is.EqualTo(projectModel.ProjectOpenStatusTypeId));
        }

        [Test]
        [Category("ProjectServices_POST")]
        public void TestProjectServices_AddProjectPipelineNote_ShouldupdateProjectPipeLineNotes()
        {
            ProjectPipelineNoteModel _model = new ProjectPipelineNoteModel();
            _model.CurrentUser = user;
            _model.Note = "Testing";
            _model.OwnerId = user.UserId;
            _model.OwnerName = user.FirstName + " " + user.LastName;
            _model.ProjectId = projectModel.ProjectId.Value;
            _model.ProjectPipelineNoteId = 2;
            _model.ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel();
            _model.ProjectPipelineNoteType.ProjectPipelineNoteTypeId = 1;
            _model.Title = "For Testing Only";

            Response = projectService.AddProjectPipelineNote(user, _model);

            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));
            Assert.That(Response.Model, Is.EqualTo(_model));

            var _projectPipeLineNoteModel = Response.Model as ProjectPipelineNoteModel;

            Assert.That(_projectPipeLineNoteModel.CurrentUser, Is.EqualTo(_model.CurrentUser));
            Assert.That(_projectPipeLineNoteModel.Note, Is.EqualTo(_model.Note));
            Assert.That(_projectPipeLineNoteModel.OwnerId, Is.EqualTo(_model.OwnerId));
            Assert.That(_projectPipeLineNoteModel.OwnerName, Is.EqualTo(_model.OwnerName));
            Assert.That(_projectPipeLineNoteModel.ProjectId, Is.EqualTo(projectModel.ProjectId.Value));
            Assert.That(_projectPipeLineNoteModel.ProjectPipelineNoteId, Is.EqualTo(_model.ProjectPipelineNoteId));
            Assert.That(_projectPipeLineNoteModel.Title, Is.EqualTo(_model.Title));
        }

        [Test]
        [Category("ProjectServices_POST")]
        public void TestProjectServices_Duplicate_ShouldReturnTheDuplicateProject()
        {
            Response = projectService.Duplicate(user, projectModel);

            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));

            var _newProject = Response.Model as Project;

            Assert.That(_newProject, Is.Not.Null);
            Assert.That(_newProject.ActualCloseDate, Is.EqualTo(projectModel.ActualCloseDate));

            if (_newProject.ActualDeliveryDate == null)
                _newProject.ActualDeliveryDate = DateTime.MinValue;
            Assert.That(_newProject.ActualDeliveryDate, Is.EqualTo(projectModel.ActualDeliveryDate));

            if (_newProject.BidDate == null)
                _newProject.BidDate = DateTime.MinValue;
            Assert.That(_newProject.BidDate, Is.EqualTo(projectModel.BidDate));

            if (_newProject.CustomerAddress != null)
            {
                Assert.That(_newProject.CustomerAddress.AddressLine1, Is.EqualTo(projectModel.CustomerAddress.AddressLine1));
                Assert.That(_newProject.CustomerAddress.AddressLine2, Is.EqualTo(projectModel.CustomerAddress.AddressLine2));
                Assert.That(_newProject.CustomerAddress.Location, Is.EqualTo(projectModel.CustomerAddress.Location));
                Assert.That(_newProject.CustomerAddress.PostalCode, Is.EqualTo(projectModel.CustomerAddress.PostalCode));
                Assert.That(_newProject.CustomerAddress.StateId, Is.EqualTo(projectModel.CustomerAddress.StateId));
                if (_newProject.CustomerName != null)
                    Assert.That(_newProject.CustomerName, Is.EqualTo(projectModel.CustomerName));
            }
            if (_newProject.SellerAddress != null)
            {
                Assert.That(_newProject.SellerAddress.AddressLine1, Is.EqualTo(projectModel.SellerAddress.AddressLine1));
                Assert.That(_newProject.SellerAddress.AddressLine2, Is.EqualTo(projectModel.SellerAddress.AddressLine2));
                Assert.That(_newProject.SellerAddress.Location, Is.EqualTo(projectModel.SellerAddress.Location));
                Assert.That(_newProject.SellerAddress.PostalCode, Is.EqualTo(projectModel.SellerAddress.PostalCode));
                Assert.That(_newProject.SellerAddress.StateId, Is.EqualTo(projectModel.SellerAddress.StateId));
                if (_newProject.SellerName != null)
                    Assert.That(_newProject.SellerName, Is.EqualTo(projectModel.SellerName));
            }
            if (_newProject.EngineerAddress != null)
            {
                Assert.That(_newProject.EngineerAddress.AddressLine1, Is.EqualTo(projectModel.EngineerAddress.AddressLine1));
                Assert.That(_newProject.EngineerAddress.AddressLine2, Is.EqualTo(projectModel.EngineerAddress.AddressLine2));
                Assert.That(_newProject.EngineerAddress.Location, Is.EqualTo(projectModel.EngineerAddress.Location));
                Assert.That(_newProject.EngineerAddress.PostalCode, Is.EqualTo(projectModel.EngineerAddress.PostalCode));
                Assert.That(_newProject.EngineerAddress.StateId, Is.EqualTo(projectModel.EngineerAddress.StateId));
                if(_newProject.EngineerName != null)
                    Assert.That(_newProject.EngineerName, Is.EqualTo(projectModel.EngineerName));
                if(_newProject.EngineerBusinessName != null)
                    Assert.That(_newProject.EngineerBusinessName, Is.EqualTo(projectModel.EngineerBusinessName));
            }
            if(_newProject.ShipToAddress != null)
            {
                Assert.That(_newProject.ShipToAddress.AddressLine1, Is.EqualTo(projectModel.ShipToAddress.AddressLine1));
                Assert.That(_newProject.ShipToAddress.AddressLine2, Is.EqualTo(projectModel.ShipToAddress.AddressLine2));
                Assert.That(_newProject.ShipToAddress.Location, Is.EqualTo(projectModel.ShipToAddress.Location));
                Assert.That(_newProject.ShipToAddress.PostalCode, Is.EqualTo(projectModel.ShipToAddress.PostalCode));
                Assert.That(_newProject.ShipToAddress.StateId, Is.EqualTo(projectModel.ShipToAddress.StateId));
                if (_newProject.ShipToName != null)
                    Assert.That(_newProject.ShipToName, Is.EqualTo(projectModel.ShipToName)); 
            }

            if(_newProject.DealerContractorName != null)
                Assert.That(_newProject.DealerContractorName, Is.EqualTo(projectModel.DealerContractorName));

            Assert.That(_newProject.Description, Is.EqualTo(projectModel.Description));
            Assert.That(_newProject.ERPFirstOrderComment, Is.EqualTo(projectModel.ERPFirstOrderComment));
            Assert.That(_newProject.ERPFirstOrderDate, Is.EqualTo(projectModel.ERPFirstOrderDate));
            Assert.That(_newProject.ERPFirstOrderNumber, Is.EqualTo(projectModel.ERPFirstOrderNumber));
            Assert.That(_newProject.ERPFirstPONumber, Is.EqualTo(projectModel.ERPFirstPONumber));
            Assert.That(_newProject.EstimatedClose, Is.EqualTo(projectModel.EstimatedClose));
            Assert.That(_newProject.EstimatedDelivery, Is.EqualTo(projectModel.EstimatedDelivery));
            Assert.That(_newProject.EstimateReleaseDate, Is.EqualTo(projectModel.EstimateReleaseDate));
            Assert.That(_newProject.Expiration, Is.EqualTo(projectModel.Expiration));
            Assert.That(_newProject.Name.Contains(projectModel.Name), Is.EqualTo(true));
            Assert.That(_newProject.NumberOfFloors, Is.EqualTo(projectModel.NumberOfFloors));
            Assert.That(_newProject.OwnerId, Is.EqualTo(projectModel.OwnerId));

            if (_newProject.Owner == null)
                _newProject.Owner = new User();
            _newProject.Owner.FirstName = user.FirstName;
            _newProject.Owner.LastName = user.LastName;
            Assert.That(_newProject.Owner.FirstName + " " + _newProject.Owner.LastName, Is.EqualTo(projectModel.OwnerName));

            Assert.That(_newProject.ProjectDate, Is.EqualTo(projectModel.ProjectDate));
            Assert.That(_newProject.ConstructionTypeId, Is.EqualTo(projectModel.ConstructionTypeId));
            Assert.That(_newProject.ProjectLeadStatusTypeId, Is.EqualTo(projectModel.ProjectLeadStatusTypeId));
            Assert.That(_newProject.ProjectOpenStatusTypeId, Is.EqualTo(projectModel.ProjectOpenStatusTypeId));
            Assert.That(_newProject.ProjectStatusNotes, Is.EqualTo(projectModel.ProjectStatusNotes));
            Assert.That(_newProject.ProjectStatusTypeId, Is.EqualTo((ProjectStatusTypeEnum)projectModel.ProjectStatusTypeId));
            Assert.That(_newProject.ProjectTypeId, Is.EqualTo(projectModel.ProjectTypeId));
        }

        [Test]
        [Category("ProjectServices_POST")]
        [TestCase("Project Name")]
        [TestCase("Project Date")]
        [TestCase("Square Footages: 20000")]
        [TestCase("Number Of Floors: 10")]
        public void TestProjectServices_PostModel_ShouldUpdateTheProjectOnDatabaseAndReturnProjectsModel(string testValue)
        {
            if(testValue.Contains("Project Name"))
            {
                testValue = "AA PRO" + DateTime.Now.ToShortDateString();
                projectModel.Name = testValue;
                Response = projectService.PostModel(user, projectModel);
                Assert.That(Response.IsOK, Is.EqualTo(true));
                Assert.That(Response.HasError, Is.EqualTo(false));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains(" has been updated")), Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).Name, Is.EqualTo(testValue));
            }
            if (testValue.Contains("Project Date"))
            {
                testValue = DateTime.Now.ToShortDateString();
                projectModel.ProjectDate = Convert.ToDateTime( testValue );
                Response = projectService.PostModel(user, projectModel);
                Assert.That(Response.IsOK, Is.EqualTo(true));
                Assert.That(Response.HasError, Is.EqualTo(false));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains(" has been updated")), Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).ProjectDate, Is.EqualTo(Convert.ToDateTime(testValue)));
            }
            if(testValue.Contains("Square Footages: "))
            {
                testValue = testValue.TrimStart(("Square Footages: ").ToArray());
                projectModel.SquareFootage = Convert.ToInt16(testValue);
                Response = projectService.PostModel(user, projectModel);
                Assert.That(Response.IsOK, Is.EqualTo(true));
                Assert.That(Response.HasError, Is.EqualTo(false));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains(" has been updated")), Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).SquareFootage, Is.EqualTo(Convert.ToInt16(testValue))); 
            }
            if(testValue.Contains("Number Of Floors: "))
            {
                testValue = testValue.TrimStart(("Number Of Floors: ").ToArray());
                projectModel.NumberOfFloors = Convert.ToInt16(testValue);
                Response = projectService.PostModel(user, projectModel);
                Assert.That(Response.IsOK, Is.EqualTo(true));
                Assert.That(Response.HasError, Is.EqualTo(false));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains(" has been updated")), Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).NumberOfFloors, Is.EqualTo(Convert.ToInt16(testValue)));
            } 
        }

        [Test]
        [Category("ProjectServices_POST")]
        [TestCase("Aaron.Nguyen@daikincomfort.com")]
        public void TestProjectServices_ProjectTransfer(string email)
        {
            Response = projectService.ProjectTransfer(user, _projectId, email);
            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));
            returnMessage = Response.Messages.Items.FirstOrDefault().Text.ToString();
            expectMessage = "Transferred '" + (Response.Model as ProjectModel).Name + "' to '" + email + "'. Project  has been updated.";
            Assert.That(returnMessage, Is.EqualTo(expectMessage)); 
        }

        [Test]
        [Category("ProjectServices_POST")]
        public void TestProjectServices_Undelete_ShouldReturnDeleteValueAsFalse()
        {
            long _projectId = this.db.Context.Projects.Where(p => p.Deleted == true).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            Response = projectService.Undelete(user, _model);

            Assert.That(Response.IsOK, Is.EqualTo(true));
            Assert.That(Response.HasError, Is.EqualTo(false));
            returnMessage = Response.Messages.Items.First().Text;
            expectMessage = "Project '" + _model.Name + "' has been updated.";
            Assert.That(returnMessage, Is.EqualTo(expectMessage));

            _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            Assert.That(_model.Deleted, Is.EqualTo(false));
        }

        [Test]
        [Category("ProjectServices_POST")]
        [TestCase("Aaron.Nguyen@daikincomfort.com")]
        [TestCase("Nguyen.Aaron.google.com")]
        public void TestProjectServices_VerifiyIsUser_ShouldReturnErrorForInvalidUser(string email)
        {
            Response = projectService.VerifiyIsUser(email);
            if (email == "Aaron.Nguyen@daikincomfort.com")
            {
                Assert.That(Response.IsOK, Is.EqualTo(true));
                Assert.That(Response.HasError, Is.EqualTo(false));
            }
            if (email == "Nguyen.Aaron.google.com")
            Assert.That(Response.Messages.Items.Any( m => m.Text == Resources.ResourceModelUser.MU007));
        }

        [Test]
        [Category("ProjectServices")]
        public void TestProjectServices_ModelToEntity_ShouldReturnEntityMatchProjectPipeLineNoteModel()
        {
            ProjectPipelineNoteModel _model = new ProjectPipelineNoteModel();
            _model.CurrentUser = user;
            _model.Note = "testing...";
            _model.OwnerId = user.UserId;
            _model.OwnerName = user.FirstName + " " + user.LastName;
            _model.ProjectId = 1234;
            _model.ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel();
            _model.ProjectPipelineNoteType.ProjectPipelineNoteTypeId = 1;
            _model.Title = "Test Pipe Line";
            ProjectPipelineNote entity = projectService.ModelToEntity(user, _model);

           Assert.That(entity.Note, Is.EqualTo(_model.Note));
            Assert.That(entity.OwnerId, Is.EqualTo(_model.OwnerId));
            Assert.That(entity.ProjectId, Is.EqualTo(1234));
            Assert.That(entity.ProjectPipelineNoteTypeId, Is.EqualTo(1));
            Assert.That(entity.Title, Is.EqualTo(_model.Title));
        }

        //[Test]
        //[Category("ProjectServices_GET")]
        //public void TestProjectServices_GetAllProjects_ShouldReturnProjectsBasedOnQuery()
        //{
        //    long projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).Select(p => p.ProjectId).FirstOrDefault();
        //    ProjectsModel _projectModel = new ProjectsModel();
        //    Response = projectService.GetProjectsModel(user, _projectModel);
            
        //    _projectModel = Response.Model as ProjectsModel;

        //    DPO.Model.Light.QueryInfo _queryInfo = new DPO.Model.Light.QueryInfo();
        //    Response = projectService.GetAllProjects(user, _projectModel, _queryInfo);

        //    Assert.That(Response.IsOK, Is.EqualTo(true));
        //    Assert.That(Response.HasError, Is.EqualTo(false));

        //    var expectProjects = from p in this.db.Context.Projects
        //                         join u in this.db.Context.Users
        //                         on p.OwnerId equals u.UserId
        //                         join g in this.db.Groups
        //                         on u.GroupId equals g.GroupId
        //                         where g.GroupId == user.GroupId
        //                         && p.ProjectDate <= DateTime.Now
        //                         && p.Deleted == false 
        //                         orderby p.Name ascending
        //                         select new { Project = p };

        //    Assert.That((Response.Model as ProjectsModel).TotalRecords, Is.EqualTo(expectProjects.Count()));
        //}

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(484641374210097152)]
        [TestCase(1234)]
        public void TestProjectServices_GetEntity_WithProjectListModel(long projectId)
        {
            ProjectsModel _projectModel = new ProjectsModel();
            _projectModel.ProjectId = projectId;
            Response = projectService.GetProjectsModel(user, _projectModel);
            
            if (projectId == 484641374210097152)
            {
                Assert.That(Response.IsOK, Is.EqualTo(true));
                _projectModel = Response.Model as ProjectsModel;

                ProjectListModel _model = new ProjectListModel();
                _model = _projectModel.Items[0];

                Project _project = projectService.GetEntity(user, _model);

                Assert.That(_project, Is.Not.Null);
                Assert.That(_project.ProjectId, Is.EqualTo(_model.ProjectId));
            }
            if(projectId == 1234)
            {
                ProjectListModel _model = new ProjectListModel();
                _model.ProjectId = projectId;
          
                Project _project = projectService.GetEntity(user, _model);

                Assert.That(this.Response.Messages.Items.Any(t => t.Text.Contains(Resources.ResourceModelProject.MP004)), Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServices_GET")]
        public void TestProjectServices_GetEntity_WithProjectModel()
        {
            long _projectId = this.db.Projects.Where(p => p.OwnerId == user.UserId).Select(p => p.ProjectId).FirstOrDefault();

            Response = projectService.GetProjectModel(user, _projectId);

            ProjectModel _model = Response.Model as ProjectModel;

            Project _project = projectService.GetEntity(user, _model);

            Assert.That(_project, Is.Not.Null);
            Assert.That(_project.ProjectId, Is.EqualTo(_projectId));
            Assert.That(_project.Name, Is.EqualTo(_model.Name));
            Assert.That(_project.ProjectStatusTypeId, Is.EqualTo((ProjectStatusTypeEnum)_model.ProjectStatusTypeId));
            Assert.That(_project.ProjectTypeId, Is.EqualTo(_model.ProjectTypeId));
            Assert.That(_project.ProjectLeadStatusTypeId, Is.EqualTo(_model.ProjectLeadStatusTypeId));
            Assert.That(_project.ProjectOpenStatusTypeId, Is.EqualTo(_model.ProjectOpenStatusTypeId));
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(1)]
        [TestCase(57)]
        [TestCase(58)]
        public void TestProjectServices_GetStateByStateId_ShouldReturnStateByStateId(int stateId)
        {
            string state = projectService.GetStateByStateId(stateId);
            if (stateId == 1)
                Assert.That(state, Is.EqualTo("Alberta"));
            if (stateId == 57)
                Assert.That(state, Is.EqualTo("Texas"));
            if (stateId == 58)
                Assert.That(state, Is.EqualTo("Utah"));
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase(1)]
        [TestCase(57)]
        [TestCase(58)] 
        public void TestProjectServices_GetStateCodeByStateId_ShouldReturnStateCodeByStateId(int stateId)
        {
            string stateCode = projectService.GetStateCodeByStateId(stateId);
            switch (stateId)
            {
                case 1:
                    Assert.That(stateCode, Is.EqualTo("AB"));
                break;
                case 2:
                    Assert.That(stateCode, Is.EqualTo("TX"));
                break;
                case 3:
                    Assert.That(stateCode, Is.EqualTo("UT"));
                break;
            }
        }

        [Test]
        [Category("ProjectServices_GET")]
        [TestCase("AB")]
        [TestCase("TX")]
        [TestCase("UT")]
        public void TestProjectServices_GetStateIdByState_ShouldReturnStateIdByState(string state)
        {
            int _stateId = projectService.GetStateIdByState(state);
            switch(state)
            {
                case "Alberta":
                    Assert.That(_stateId, Is.EqualTo(1));
                    break;
                case "Texas":
                    Assert.That(_stateId, Is.EqualTo(57));
                    break;
                case "Utah":
                    Assert.That(_stateId, Is.EqualTo(58));
                    break;
            }
        }

        [Test]
        [Category("ProjectServices_AddressValidation")]
        [TestCase("5151 San Felipe St")]
        [TestCase("1234 GoToHell")]
        [TestCase("77055")]
        public void TestProjectServices_ValidateProjectLocationAddress_ShouldPerformValidationAndProvideSuggestionForShippingAddress(string testValue)
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            long _projectId =(from p in this.db.Projects
                            join a in this.db.Addresses
                            on p.ShipToAddressId equals a.AddressId
                            where a.AddressLine1 != null && p.OwnerId == user.UserId
                            select p.ProjectId).FirstOrDefault() ;

            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            if (_model.ShipToAddress.AddressLine1 == string.Empty)
                _model.ShipToAddress.AddressLine1 = testValue;

            Response = projectService.ValidateProjectLocationAddress(_model);

            if (testValue == "5151 San Felipe St")
            {
                Assert.That(Response.HasError, Is.EqualTo(false));
            }

            if (testValue == "1234 GoToHell")
            {
                _model.ShipToAddress.AddressLine1 = testValue;
                Response = projectService.ValidateProjectLocationAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains("Project Shipping Address is invalid.Please provide a valid address")), Is.EqualTo(true));
            }
            if (testValue == "77055")
            {
                _model.ShipToAddress.PostalCode = testValue;
                Response = projectService.ValidateProjectLocationAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).HasSuggestionAddress, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServices_AddressValidation")]
        [TestCase("5151 San Felipe St")]
        [TestCase("1234 GoToHell")]
        [TestCase("77055")]
        public void TestProjectServices_ValidateDealorAddress_ShouldPerformValidationAndProvideSuggestionForDealorAddress(string testValue)
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            long _projectId = (from p in this.db.Projects
                               join a in this.db.Addresses
                               on p.ShipToAddressId equals a.AddressId
                               where a.AddressLine1 != null && p.OwnerId == user.UserId
                               select p.ProjectId).FirstOrDefault();

            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            if (_model.CustomerAddress.AddressLine1 == string.Empty)
                _model.CustomerAddress.AddressLine1 = testValue;

            Response = projectService.ValidateDealorAddress(_model);

            if (testValue == "5151 San Felipe St")
            {
                Assert.That(Response.HasError, Is.EqualTo(false));
            }

            if (testValue == "1234 GoToHell")
            {
                _model.CustomerAddress.AddressLine1 = testValue;
                Response = projectService.ValidateDealorAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains("Dealor/Contractor Address is invalid.Please provide a valid address")), Is.EqualTo(true));
            }
            if (testValue == "77055")
            {
                _model.CustomerAddress.PostalCode = testValue;
                Response = projectService.ValidateDealorAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).HasSuggestionAddress, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServices_AddressValidation")]
        [TestCase("")]
        [TestCase("1234 GoToHell")]
        [TestCase("77055")]
        public void TestProjectServices_ValidateSellerAddress_ShouldPerformValidationAndProvideSuggestionForSellerAddress(string testValue)
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            long _projectId = (from p in this.db.Projects
                               join a in this.db.Addresses
                               on p.ShipToAddressId equals a.AddressId
                               where a.AddressLine1 != null && p.OwnerId == user.UserId
                               select p.ProjectId).FirstOrDefault();

            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            if (_model.SellerAddress.AddressLine1 == string.Empty)
                _model.SellerAddress.AddressLine1 = testValue;

            Response = projectService.ValidateSellerAddress(_model);

            if (testValue == "") // seller address always has value, so we do not need to provide data
            {
                Assert.That(Response.HasError, Is.EqualTo(true));
            }

            if (testValue == "1234 GoToHell")
            {
                _model.SellerAddress.AddressLine1 = testValue;
                Response = projectService.ValidateSellerAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains("Seller Address is invalid.Please provide a valid address")), Is.EqualTo(true));
            }
            if (testValue == "77055")
            {
                _model.SellerAddress.PostalCode = testValue;
                Response = projectService.ValidateSellerAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).HasSuggestionAddress, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectServices_AddressValidation")]
        [TestCase("5151 San Felipe St")]
        [TestCase("1234 GoToHell")]
        [TestCase("77055")]
        public void TestProjectServices_ValidateEngineerAddress_ShouldPerformValidationAndProvideSuggestionForEngineerAddress(string testValue)
        {
            this.Response.Messages.Items.Clear();
            this.Response.Messages.HasErrors = false;

            long _projectId = (from p in this.db.Projects
                               join a in this.db.Addresses
                               on p.ShipToAddressId equals a.AddressId
                               where a.AddressLine1 != null && p.OwnerId == user.UserId
                               select p.ProjectId).FirstOrDefault();

            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            if (_model.EngineerAddress.AddressLine1 == string.Empty)
                _model.EngineerAddress.AddressLine1 = testValue;

            Response = projectService.ValidateEngineerAddress(_model);

            if (testValue == "5151 San Felipe St")
            {
                Assert.That(Response.HasError, Is.EqualTo(false));
            }

            if (testValue == "1234 GoToHell")
            {
                _model.EngineerAddress.AddressLine1 = testValue;
                Response = projectService.ValidateEngineerAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That(Response.Messages.Items.Any(m => m.Text.Contains("Engineer Address is invalid.Please provide a valid address")), Is.EqualTo(true));
            }
            if (testValue == "77055")
            {
                _model.EngineerAddress.PostalCode = testValue;
                Response = projectService.ValidateEngineerAddress(_model);
                Assert.That(Response.HasError, Is.EqualTo(true));
                Assert.That((Response.Model as ProjectModel).HasSuggestionAddress, Is.EqualTo(true));
            }
        }

        [Test]
        [TestCase("5151 San Felipe St")]
        public void TestProjectServices_Should_return_projects_within_last_seven_days(string testValue)
        {
            var dateTimePeriod = DateTime.Now.AddDays(-7);
            var a = db.Projects.Where(x => x.Timestamp > dateTimePeriod).ToList();

            Assert.IsTrue(a.Count > 0);
        }

        [Test]
        [TestCase("http://localhost:63596/iSeries/api/orders/")]
        [TestCase("http://localhost:63596/iSeries/api/invoices/")]
        public void TestProjectServices_Should_return_orders_from_Mapics(string testValue)
        {
            ERPClient erpClient = new ERPClient();
            var client = new HttpClient();
            var userPwd = "erpapi" + ":" + "api4erp";
            var auth = new AuthenticationHelper(userPwd);         //Encoding.UTF8.GetBytes(userPwd).ToArray();
            var token = auth.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var orderResponseList = new List<OrderResponse>();
            var datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
             
            using (client)
            {
                try
                {
                    var url = testValue;
                    var httpResponseMsg = client.GetAsync(url + datetime).Result;

                    var responseHelper = new HttpResponseHelper(httpResponseMsg);
                    orderResponseList = httpResponseMsg.IsSuccessStatusCode
                            ? responseHelper.GetOrdersToUpdateApiResponse().ToList()
                            : null;
                }
                catch (Exception ex)
                {
                    orderResponseList.Count.Equals(0);
                }
            }
             
            Assert.IsNotNull(orderResponseList);
        }

        //[Test]
        //public void TestProjectServices_Rules_Project_Date_Before_Estimated_Close_Date()
        //{
        //    var model = GetProjectModel(sa, "Project 1");

        //    model.BidDate = model.ProjectDate;

        //    model.EstimatedClose = model.BidDate.Value.AddDays(-1);

        //    var response = service.PostModel(sa, model);

        //    Assert.That(response.Messages.Items.Any(m => m.Text == Resources.ResourceModelProject.MP002), Is.True);
        //}

        //[Test]
        //public void TestProjectServices_Rules_Check_Project_Mandatory_Fields()
        //{
        //    var model = GetProjectModel(sa, "Project 1");
        //    var response = service.PostModel(sa, model);

        //    Assert.IsTrue(response.IsOK);

        //    model.EstimatedClose = null;
        //    model.EstimatedDelivery = null;

        //    service.Response.Messages.Clear();
        //    response = service.PostModel(sa, model);

        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Estimated Close")), Is.True);
        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Estimated Delivery")), Is.True);

        //    model.ProjectDate = DateTime.Now;
        //    model.BidDate = DateTime.Now;
        //    model.EstimatedClose = DateTime.Now;
        //    model.EstimatedDelivery = DateTime.Now;

        //    model.ConstructionTypeId = null;
        //    model.ProjectOpenStatusTypeId = null;
        //    model.ProjectStatusTypeId = null;
        //    model.ProjectTypeId = null;
        //    model.VerticalMarketTypeId = 0;

        //    service.Response.Messages.Clear();
        //    response = service.PostModel(sa, model);

        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Project Open Status")), Is.True);
        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Project Status")), Is.True);
        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Project Type")), Is.True);
        //    Assert.That(response.Messages.Items.Any(m=>m.Text.Contains("Vertical Market")), Is.True);
        //    Assert.That(response.Messages.Items.Any(m => m.Text.Contains("Construction Type")), Is.True);
        //}

        //[Test]
        //public void TestProjectServices_Rules_New_Project_Copies_Business_Address_To_Seller_Address()
        //{
        //   var user = GetUserSessionModel("USAM1@Somewhere.com");

        //   var business = GetBusinessModel(sa, "USB1");

        //   var newProject = service.GetProjectModel(user, null).Model as ProjectModel;

        // newProject.VerticalMarketTypeId = 1;
        // newProject.ProjectDate = DateTime.Now;
        // newProject.ConstructionTypeId = 1;
        // newProject.ProjectStatusTypeId = 1;
        // newProject.ProjectOpenStatusTypeId = 1;
        // newProject.CustomerName = "alan";
        // newProject.Name = "test";
        // newProject.ProjectTypeId = 1;
        // newProject.EstimatedDelivery = DateTime.Now.AddDays(1);
        // newProject.EstimatedClose = DateTime.Now.AddDays(1);
        // newProject.Expiration = DateTime.Now.AddDays(1);
        // newProject.Description = "test";
        // newProject.EngineerName = "engine" ;                    

        //   var model = service.PostModel(user,newProject).Model as ProjectModel;

        //   Assert.That(business.Address.AddressId,Is.Not.EqualTo(model.SellerAddress.AddressId));
        //   Assert.That(business.Address.AddressLine1, Is.EqualTo(model.SellerAddress.AddressLine1));
        //   Assert.That(business.Address.AddressLine2, Is.EqualTo(model.SellerAddress.AddressLine2));
        //   Assert.That(business.Address.AddressLine3, Is.EqualTo(model.SellerAddress.AddressLine3));
        //   Assert.That(business.Address.Location, Is.EqualTo(model.SellerAddress.Location));
        //   Assert.That(business.Address.PostalCode, Is.EqualTo(model.SellerAddress.PostalCode));
        //   Assert.That(business.Address.CountryCode, Is.EqualTo(model.SellerAddress.CountryCode));
        //}

        //[Test]
        //public void TestProjectServices_Create_New_Project_Check()
        //{
        //    var newProject = service.GetProjectModel(sa, null).Model as ProjectModel;
        //    var saveDatetime = DateTime.Today;

        //    newProject.VerticalMarketTypeId = 2;
        //    newProject.ProjectDate = saveDatetime;
        //    newProject.ConstructionTypeId = 1;
        //    newProject.ProjectStatusTypeId = 2;
        //    newProject.ProjectOpenStatusTypeId = 2;
        //    newProject.CustomerName = "alan";
        //    newProject.Name = "test44354543";
        //    newProject.ProjectTypeId = 2;
        //    newProject.BidDate = saveDatetime.AddDays(1);
        //    newProject.EstimatedClose = saveDatetime.AddDays(2);
        //    newProject.EstimatedDelivery = saveDatetime.AddDays(3);
        //    newProject.Expiration = saveDatetime.AddDays(4);
        //    newProject.Description = "test";
        //    newProject.EngineerName = "engine";

        //    var response = service.PostModel(sa, newProject);

        //    Assert.That(response.IsOK, Is.True);

        //    var model = response.Model as ProjectModel;
        //    var savedProject = GetProjectModel(sa, newProject.Name);

        //    AssertPropertiesThatMatchAreEqual(newProject, savedProject, false, new string[] { "Timestamp","Concurrency","ProjectId","ProjectDate","OwnerId", "ProjectTypeDescription", "ProjectOpenStatusDescription", "ProjectStatusDescription", "ConstructionTypeDescription", "VerticalMarketDescription" });
        //}

        //[Test]
        //public void TestProjectServices_Rules_When_Project_Is_Closed_No_Edits_Allowed_Only_When_Open()
        //{
        //    var model = GetProjectModel(sa, "Project 1");

        //    model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.ClosedWon;
        //    model.Name = "dsdf";

        //    var response = service.PostModel(sa, model);

        //    Assert.IsTrue(response.Messages.Items[0].Text == Resources.ResourceModelProject.MP012);

        //    model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.ClosedLost;

        //    service.Response.Messages.Clear();
        //    response = service.PostModel(sa, model);

        //    Assert.IsTrue(response.Messages.Items[0].Text == Resources.ResourceModelProject.MP012);

        //    service.Response.Messages.Clear();

        //    model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.Open;

        //    response = service.PostModel(sa, model);

        //    Assert.That(response.IsOK, Is.True);

        //    service.Response.Messages.Clear();

        //    model.Name = "dsdf";

        //    response = service.PostModel(sa, model);

        //    Assert.That(response.IsOK, Is.True);
        //}

    }
}