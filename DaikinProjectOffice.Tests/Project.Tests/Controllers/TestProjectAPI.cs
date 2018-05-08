using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using DPO.Domain;
using DPO.Common;
using DPO.Model.Light;
using DPO.Services.Light;
using Resources = DPO.Resources;
using DPO.Data;
using DPO.Web.Controllers;
using System.Web;
using System.Web.Http;
using DPO.Web.Controllers.Api;
using Moq;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class TestProjectAPI : TestAdmin
    {
        private ProjectServices projectService;
        private ServiceResponse serviceResponse;
        private UserSessionModel user;
        private AccountServices accountService;
        private long projectId;
        private ProjectServiceLight projectServiceLight;
        private ProjectController projectApi;
        private Project project;
        public TestProjectAPI()
        {
            projectService = new ProjectServices();
            serviceResponse = new ServiceResponse();
            accountService = new AccountServices();
            projectServiceLight = new ProjectServiceLight();
            projectApi = new ProjectController();
           
            user = accountService.GetUserSessionModel("User15@test.com").Model as UserSessionModel;

            projectId = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.Deleted == false)
                .OrderByDescending(p => p.ProjectId)
                .Select(p => p.ProjectId)
                .FirstOrDefault();

            project = this.db.Projects.Where(p => p.ProjectId == projectId && p.OwnerId == user.UserId).FirstOrDefault();
        }

        [Test]
        [Category("ProjectAPI_POST")]
        public void TestProjectApi_GetProjects_ShouldReturnProjectsBasedOnQueryInfo()
        {
            SetupProjectAPIForTesting();

            ProjectsGridViewModel model = new ProjectsGridViewModel();
            ProjectsGridQueryInfo queryInfo = new ProjectsGridQueryInfo();

            Sort sort = new Sort();
            sort.Field = "biddate";
            queryInfo.Sort.Add(sort);

            serviceResponse = projectApi.GetProjects(queryInfo);

            Assert.That(serviceResponse, Is.Not.EqualTo(null));
            Assert.That(serviceResponse.Model, Is.Not.EqualTo(null));
            Assert.IsInstanceOf<ProjectsGridViewModel>(serviceResponse.Model);

            model = serviceResponse.Model as ProjectsGridViewModel;
            int projectCount = this.db.Projects.Where(p => p.OwnerId == user.UserId 
                               && p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
                               && p.Deleted == false && p.ActiveVersion == 1).Count();

            Assert.That(model.Items.Count, Is.GreaterThanOrEqualTo(projectCount));
        }

        [Test]
        [Category("ProjectAPI_POST")]
        public void TestProjectAPI_EditProjects_ShouldUpdatedMultipleProjectAtOnces()
        {
            SetupProjectAPIForTesting();

            ProjectsGridViewModel model = new ProjectsGridViewModel();
            ProjectModel projectModel = projectService.GetProjectModel(user, projectId).Model as ProjectModel;

            ProjectViewModel projectVM = MapToProjectViewModel(projectModel);
            string _originalName = projectVM.Name;

            projectVM.Name = "Test1234"; //change project Name for testing

            model.Items.Add(projectVM);

            serviceResponse = projectApi.EditProjects(model);

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            ProjectsGridViewModel projectsGridViewModel = serviceResponse.Model as ProjectsGridViewModel;
            Assert.That(projectsGridViewModel, Is.Not.EqualTo(null));
            Assert.That(projectsGridViewModel.Items.Count, Is.GreaterThan(0));
            Assert.That(projectsGridViewModel.Items.Any(p => p.Name.Contains("Test1234")), Is.EqualTo(true));

            model.Items.Clear();
            projectVM.Name = _originalName; //roll back original project Name so the project will be restore to original value.
            model.Items.Add(projectVM);

            serviceResponse = projectApi.EditProjects(model);
            Assert.That(projectsGridViewModel.Items.Any(p => p.Name.Contains(_originalName)), Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetProject_ShouldReturnProjectModelByProjectId()
        {
            SetupProjectAPIForTesting();

            serviceResponse = projectApi.GetProject(projectId);

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That(serviceResponse.Model, Is.Not.EqualTo(null));

            ProjectModel model = serviceResponse.Model as ProjectModel;
            Assert.That(model.ProjectId, Is.EqualTo(projectId));
            Assert.That(model.Name, Is.EqualTo(project.Name));
            Assert.That(model.ProjectStatusTypeId, Is.EqualTo(project.ProjectStatusTypeId));
            Assert.That(model.ProjectOpenStatusTypeId, Is.EqualTo(project.ProjectOpenStatusTypeId));
            Assert.That(model.ProjectTypeId, Is.EqualTo(project.ProjectTypeId));
            Assert.That(model.CustomerName, Is.EqualTo(project.CustomerName));
            Assert.That(model.DealerContractorName, Is.EqualTo(project.DealerContractorName));
            Assert.That(model.SellerAddress.AddressId, Is.EqualTo(project.SellerAddress.AddressId));
            Assert.That(model.CustomerAddress.AddressId, Is.EqualTo(project.CustomerAddress.AddressId));
            Assert.That(model.ShipToAddress.AddressId, Is.EqualTo(project.ShipToAddressId));
        }

        [Test]
        [Category("ProjectAPI_POST")]
        public void TestProjectAPI_PostProjectPipelineNote_ShouldAddProjectPipelineNote()
        {
            SetupProjectAPIForTesting();

            ProjectPipelineNoteModel model = new ProjectPipelineNoteModel();

            model.CurrentUser = user;
            model.Note = "Test Project Pipe Line Notes";
            model.OwnerId = user.UserId;
            model.OwnerName = user.FirstName + " " + user.LastName;
            model.ProjectId = projectId;
            model.ProjectPipelineNoteTypeName = "Test Project Pipe Line Note Type Name";
            model.Title = "Test Project Pipe Line Note Title";
            model.ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel();
            model.ProjectPipelineNoteType.CurrentUser = user;
            model.ProjectPipelineNoteType.Description = "Project Pipe Line Note Type Descritpion";
            model.ProjectPipelineNoteType.Name = "Project Pipe Line Note Type Name";
            model.ProjectPipelineNoteType.ProjectPipelineNoteTypeId = 1;

            serviceResponse = projectApi.PostProjectPipelineNote(model);

            Assert.That(serviceResponse.HasError, Is.Not.EqualTo(null));
            Assert.That(serviceResponse.Model, Is.Not.EqualTo(null));

            model = serviceResponse.Model as ProjectPipelineNoteModel;
            Assert.That(model.Note, Is.EqualTo("Test Project Pipe Line Notes"));
            Assert.That(model.ProjectId, Is.EqualTo(projectId));
            Assert.That(model.Title, Is.EqualTo("Test Project Pipe Line Note Title"));
            Assert.That(model.OwnerId, Is.EqualTo(user.UserId));
            Assert.That(model.ProjectPipelineNoteType.Name, Is.EqualTo("Project Pipe Line Note Type Name"));
            Assert.That(model.ProjectPipelineNoteType.Description, Is.EqualTo("Project Pipe Line Note Type Descritpion"));
        }

        [Test]
        [Category("ProjectAPI_POST")]
        public void TestProjectAPI_PostProject_ShouldSaveProjectChange()
        {
            SetupProjectAPIForTesting();

            ProjectModel model = projectService.GetProjectModel(user, projectId).Model as ProjectModel;
            string _originalName = model.Name;
            model.Name = "Test new Project Name";

                model.ProjectStatusTypeId = (byte)ProjectStatusTypeEnum.Open;
                serviceResponse = projectApi.PostProject(model);

                Assert.That(serviceResponse.HasError, Is.EqualTo(false));
                Assert.That(serviceResponse.Model, Is.Not.EqualTo(null));
                Assert.That((serviceResponse.Model as ProjectModel).Name, Is.EqualTo("Test new Project Name"));

                model.Name = _originalName;

                serviceResponse = projectApi.PostProject(model);

                Assert.That(serviceResponse.HasError, Is.EqualTo(false));
                Assert.That(serviceResponse.Model, Is.Not.EqualTo(null));
                Assert.That((serviceResponse.Model as ProjectModel).Name, Is.EqualTo(_originalName));
        }

        //[Test]
        //[Category("ProjectAPI_POST")]
        //[TestCase("Open")]
        //[TestCase("CloseWon")]
        //public void TestProjectAPI_TransferProject(string testCase)
        //{
        //    SetupProjectAPIForTesting();
        //    TransferProjectParameter parameter = new TransferProjectParameter();
        //    parameter.Email = "Aaron.Nguyen@daikincomfort.com";
          
        //    if(testCase == "Open")
        //    {
        //        long _userId = this.db.Users.Where(u => u.Email == parameter.Email).OrderByDescending(u => u.UserId).Select(u => u.UserId).FirstOrDefault();

        //        long _projectId = this.db.Projects
        //                              .Where(p => p.OwnerId == user.UserId && p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
        //                              && p.OwnerId != _userId)
        //                              .OrderByDescending(p => p.ProjectId)
        //                              .Select(p => p.ProjectId).FirstOrDefault();

        //        parameter.ProjectId = _projectId;
        //        serviceResponse = projectApi.TransferProject(parameter);

        //        Assert.That(serviceResponse.HasError, Is.EqualTo(false));
        //        ProjectModel model = serviceResponse.Model as ProjectModel;
        //        Assert.That(model, Is.Not.EqualTo(null));

        //        Project project = this.db.Projects.Where(p => p.ProjectId == _projectId).FirstOrDefault();

        //        Assert.That(project.OwnerId, Is.Not.EqualTo(user.UserId));
        //    }

        //    if(testCase == "CloseWon")
        //    {
        //        long _projectId = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon)
        //                              .OrderByDescending(p => p.ProjectId)
        //                              .Select(p => p.ProjectId).FirstOrDefault();

        //        parameter.ProjectId = _projectId;
        //        serviceResponse = projectApi.TransferProject(parameter);

        //        Assert.That(serviceResponse.HasError, Is.EqualTo(true));
        //        Assert.That(serviceResponse.Messages.Items.Any(m => m.Text.Contains("Project must be open before updates are allowed")), Is.EqualTo(true));
        //    }
           
        //}

        [Test]
        [Category("ProjectAPI")]
        [TestCase("CanDelete")]
        [TestCase("CanNotDelete")]
        public void TestProjectAPI_DeleteProject(string testCase)
        {
            SetupProjectAPIForTesting();

            if (testCase == "CanDelete")
            {
                long _projectId = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.Deleted == false)
                                      .OrderByDescending(p => p.ProjectId)
                                      .Select(p => p.ProjectId).FirstOrDefault();

                serviceResponse = projectApi.DeleteProject(_projectId);

                Assert.That(serviceResponse.HasError, Is.EqualTo(false));
                Assert.That((serviceResponse.Model as ProjectModel).Deleted, Is.EqualTo(true));
            }
            if(testCase == "CanNotDelete")
            {
                long _projectId = this.db.Context.ProjectTransfers.Where(pt => pt.UserId == user.UserId)
                                     .OrderByDescending(pt => pt.ProjectId)
                                     .Select(pt => pt.ProjectId).FirstOrDefault();

                serviceResponse = projectApi.DeleteProject(_projectId);

                Assert.That(serviceResponse.HasError, Is.EqualTo(true));
                Assert.That(serviceResponse.Messages.Items.Any(m => m.Text.Contains(Resources.ResourceModelProject.MP027)), Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetProjectPipeLineNotes()
        {
            SetupProjectAPIForTesting();
            ProjectPipelineNote projectPipeLineNote  = this.db.Context.ProjectPipelineNotes
                                                           .Where(ppn => ppn.OwnerId == user.UserId)
                                                           .OrderByDescending(ppn => ppn.ProjectId)
                                                           .FirstOrDefault();

            serviceResponse = projectApi.GetProjectPipelineNotes(projectPipeLineNote.ProjectId);

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            ProjectPipelineNoteListModel model = serviceResponse.Model as ProjectPipelineNoteListModel;
            Assert.That(model.Items.Count, Is.GreaterThan(0));
            ProjectPipelineNoteModel ppnModel = model.Items.First();
            Assert.That(ppnModel.Note, Is.EqualTo(projectPipeLineNote.Note));
        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetProjectPipeLineNoteTypes()
        {
            SetupProjectAPIForTesting();
            serviceResponse = projectApi.GetProjectPipelineNoteTypes();
            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That((serviceResponse.Model as ProjectPipelineNoteTypeListModel), Is.Not.EqualTo(null));

        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetProjectLocation_shouldReturnShipToLocation()
        {
            SetupProjectAPIForTesting();
            serviceResponse = projectApi.GetProjectLocation(projectId);
            ShipToAddressViewModel model = serviceResponse.Model as ShipToAddressViewModel;

            Address shipLocation = this.db.Context.Addresses.Where(add => add.AddressId == model.AddressId).FirstOrDefault();

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That(model.AddressId, Is.EqualTo(shipLocation.AddressId));
            Assert.That(model.AddressLine1, Is.EqualTo(shipLocation.AddressLine1));
            Assert.That(model.Location, Is.EqualTo(shipLocation.Location));
            Assert.That(model.StateId, Is.EqualTo(shipLocation.StateId));

        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetSellerInfo_ShouldReturnSellerInfo()
        {
            SetupProjectAPIForTesting();

            serviceResponse = projectApi.GetSellerInfo(projectId);

            SellerInfoViewModel model = serviceResponse.Model as SellerInfoViewModel;

            Address sellerAddress = this.db.Context.Addresses.Where(add => add.AddressId == model.AddressId).FirstOrDefault();

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That(model, Is.Not.EqualTo(null));
            Assert.That(model.AddressId, Is.EqualTo(sellerAddress.AddressId));
            Assert.That(model.AddressLine1, Is.EqualTo(sellerAddress.AddressLine1));
            Assert.That(model.Location, Is.EqualTo(sellerAddress.Location));
            Assert.That(model.PostalCode, Is.EqualTo(sellerAddress.PostalCode));
            Assert.That(model.StateId, Is.EqualTo(sellerAddress.StateId));
            Assert.That(model.SellerName, Is.EqualTo(project.SellerName));
        }

        [Test]
        [Category("ProjectAPI_GET")]
        public void TestProjectAPI_GetDealerContractorInfo_ShouldReturnCustomerInfo()
        {
            SetupProjectAPIForTesting();

            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId
                               && p.CustomerAddressId != null).OrderByDescending(p => p.ProjectId)
                                .Select(p => p.ProjectId)
                                .FirstOrDefault();

            serviceResponse = projectApi.GetDealerContractorInfo(_projectId);

            DealerContractorInfoViewModel model = serviceResponse.Model as DealerContractorInfoViewModel;

            Address customerAddress = this.db.Context.Addresses.Where(add => add.AddressId == model.AddressId).FirstOrDefault();

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That(model, Is.Not.EqualTo(null));
            Assert.That(model.AddressId, Is.EqualTo(customerAddress.AddressId));
            Assert.That(model.AddressLine1, Is.EqualTo(customerAddress.AddressLine1));
            Assert.That(model.Location, Is.EqualTo(customerAddress.Location));
            Assert.That(model.PostalCode, Is.EqualTo(customerAddress.PostalCode));
            Assert.That(model.StateId, Is.EqualTo(customerAddress.StateId));
            Assert.That(model.CustomerName, Is.EqualTo(project.CustomerName));
        }

        [Test]
        [Category("ProjectAPI_GET")]
        [TestCase("HasOrder")]
        [TestCase("NotHasOrder")]
        public void TestProjectAPI_HasOrder_ShouldCheckIfProjectHasOrder(string testCase)
        {
            SetupProjectAPIForTesting();

            if(testCase == "HasOrder")
            {
                var _projectId =(from p in this.db.Context.Projects
                                 join q in this.db.Context.Quotes
                                 on p.ProjectId equals q.ProjectId
                                 join o in this.db.Context.Orders
                                 on q.QuoteId equals o.QuoteId
                                 where p.OwnerId == user.UserId
                                 select p.ProjectId)
                                .FirstOrDefault();

                serviceResponse = projectApi.HasOrder(_projectId);
                Assert.That(serviceResponse.HasError, Is.EqualTo(false));
                Assert.That(serviceResponse.Model, Is.EqualTo(true));
            }

            if(testCase == "NotHasOrder")
            {
                var _projectId = (from p in this.db.Context.Projects
                                  join q in this.db.Context.Quotes
                                  on p.ProjectId equals q.ProjectId
                                  where p.OwnerId == user.UserId
                                  && !this.db.Context.Orders.Any(o => o.QuoteId == q.QuoteId)
                                  select p.ProjectId)
                               .FirstOrDefault();

                serviceResponse = projectApi.HasOrder(_projectId);
                Assert.That(serviceResponse.HasError, Is.EqualTo(false));
                Assert.That(serviceResponse.Model, Is.EqualTo(false));
            }
            
        }

        [Test]
        [Category("ProjectAPI")]
        public void TestProjectAPI_SaveGridState()
        {
            SetupProjectAPIForTesting();

            GridModel model = new GridModel();

            GridColumn column1 = new GridColumn();
            column1.Title = "Column1";
            column1.Field = "Column1";
            GridColumn column2 = new GridColumn();
            column2.Title = "Column2";
            column2.Field = "Column2";

            model.GridColumns = new List<GridColumn>();
            model.GridColumns.Add(column1);
            model.GridColumns.Add(column2);

            model.GridName = "Test Grid Name";

            model.FilterOptions = new List<GridFilter>();
            model.FilterOptions.Add(new GridFilter { Field = "Column1", Logic = "And", Operator = "&&" });

            model.SortOptions = new List<GridSort>();
            model.SortOptions.Add(new GridSort { Dir = "asc", Field = "Column1" });
           
            serviceResponse = projectApi.SaveGridState(model);

            Assert.That(serviceResponse.HasError, Is.EqualTo(false));
            Assert.That(serviceResponse.Status, Is.EqualTo(MessageTypeEnum.Success));
        }

        public ProjectViewModel MapToProjectViewModel(ProjectModel projectModel)
        {
            ProjectViewModel projectVM = new ProjectViewModel();
            projectVM.ActiveQuoteId = projectModel.ActiveQuoteSummary.QuoteId;
            projectVM.ActiveQuoteTitle = projectModel.ActiveQuoteSummary.Title;
            projectVM.Alert = projectModel.ActiveQuoteSummary.Alert;
            projectVM.BidDate = projectModel.BidDate.Value;
            projectVM.BusinessName = projectModel.BusinessName;
            projectVM.CurrentUser = projectModel.CurrentUser;
            projectVM.CustomerName = projectModel.CustomerName;
            projectVM.Deleted = projectModel.Deleted;
            projectVM.EngineerName = projectModel.EngineerName;
            projectVM.ERPFirstOrderComment = projectModel.ERPFirstOrderComment;
            projectVM.ERPFirstOrderDate = projectModel.ERPFirstOrderDate;
            projectVM.ERPFirstOrderNumber = projectModel.ERPFirstOrderNumber;
            projectVM.ERPFirstPONumber = projectModel.ERPFirstPONumber;
            projectVM.EstimatedClose = projectModel.EstimatedClose.Value;
            projectVM.EstimatedDelivery = projectModel.EstimatedDelivery.Value;
            projectVM.Expiration = projectModel.Expiration.Value;
            projectVM.ProjectDate = projectModel.ProjectDate.Value;
            projectVM.ProjectId = projectModel.ProjectId;
            projectVM.ProjectLeadStatusId = projectModel.ProjectLeadStatusTypeId;
            projectVM.ProjectOpenStatusId = (int)projectModel.ProjectOpenStatusTypeId;
            projectVM.ProjectStatusId = projectModel.ProjectStatusTypeId;
            projectVM.ProjectTypeId = (int)projectModel.ProjectTypeId;
            projectVM.ShipToName = projectModel.ShipToName;
            projectVM.Name = projectModel.Name;
      
            return projectVM;
        }
        private void SetupProjectAPIForTesting()
        {
            var httpRequest = new HttpRequest("", "http://localhost:62081/", "");
            var stringWriter = new System.IO.StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new System.Web.SessionState.HttpSessionStateContainer("id", 
                                   new System.Web.SessionState.SessionStateItemCollection(),
                                   new HttpStaticObjectsCollection(), 10, true,
                                       HttpCookieMode.AutoDetect,
                                       System.Web.SessionState.SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(System.Web.SessionState.HttpSessionState).GetConstructor(
                                                     System.Reflection.BindingFlags.NonPublic | 
                                                     System.Reflection.BindingFlags.Instance,
                                                     null, System.Reflection.CallingConventions.Standard,
                                                     new[] { typeof(System.Web.SessionState.HttpSessionStateContainer) },
                                                     null)
                                                .Invoke(new object[] { sessionContainer });

            //setup the User Pricinpal for HttpContext
            httpContext.User = new System.Security.Principal.GenericPrincipal(
                               new System.Security.Principal.GenericIdentity(user.Email.ToString()),
                               new string[0]);

            //set up the User information for ProjectAPI
            projectApi.CurrentUser = user;

            HttpContext.Current = httpContext;
        }
    }
}
