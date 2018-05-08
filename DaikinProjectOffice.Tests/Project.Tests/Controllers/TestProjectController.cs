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
using System.Web.Mvc;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using System.IO;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;


namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class TestProjectController : TestAdmin
    {

        public BasketServices basketService = new BasketServices();
        public DiscountRequestServices discountRequestService = new DiscountRequestServices();
        public HtmlServices htmlService = new HtmlServices();
        public OverviewServices overviewService = new OverviewServices();
        public ProjectServices projectService = new ProjectServices();
        public QuoteServices quoteService = new QuoteServices();
        public OrderServices orderService = new OrderServices();
        public UserServices userService = new UserServices();
        public UserSessionModel user = null;
        
        SystemTestDataServices systemService;
        BusinessServices businessService;

        public OrderServiceLight orderServiceLight = new OrderServiceLight();

        public CommissionRequestServices commissionRequestService = new CommissionRequestServices();

        public static List<long> items = new List<long>();
        public static List<long> projectsDeleteId = new List<long>();

        ProjectDashboardController projectController;
        ServiceResponse response;

        RouteData routeData;

        public long projectId;
        public TestProjectController()
        {
            response = new ServiceResponse();
            projectController = new ProjectDashboardController();

            routeData = new RouteData();

            systemService = new SystemTestDataServices(this.TContext);
            projectService = new ProjectServices(this.TContext);
            businessService = new BusinessServices(this.TContext);


            user = GetUserSessionModel("User15@test.com");
            projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
        }

        public HttpContextBase FakeHttpContext(string httpMethod)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new MockHttpSession();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Request.HttpMethod).Returns(httpMethod);
            return context.Object;
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectDashboardController_Project_ShouldReturnProjectModelByProjectId(string httpMethod)
        {

            //Arrange
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            //Act
            ViewResult result = projectController.Project(projectId) as ViewResult;

            //Result
            Assert.That(this.response.HasError, Is.EqualTo(false));
            ProjectModel _model = result.Model as ProjectModel;
            Assert.That(_model, Is.Not.Null);
            Assert.That(_model.ProjectId, Is.EqualTo(projectId));
            Assert.That(projectController.RouteData.Values["action"], Is.EqualTo("Project"));
            Assert.That(result, Is.Not.Null);   
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_GetProject_ShouldReturnProjectModelAsJsonResult(string httpMethod)
        {
            Project _project = this.db.Context.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
            
            //Arrange
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            JsonResult result = projectController.GetProject(Convert.ToString(projectId)) as JsonResult;
         
            Assert.That(this.response.HasError, Is.EqualTo(false));

            int index = result.Data.ToString().IndexOf("Data");
            index += 6;
            var tempValue = result.Data.ToString().Substring(index, result.Data.ToString().Length - index);
            tempValue = tempValue.Remove(tempValue.Length - 1);
            string json = tempValue;

            var jo = Newtonsoft.Json.Linq.JObject.Parse(json);
            var data = jo["Model"];

            ProjectModel _model = new ProjectModel
            {
                ProjectId = Convert.ToInt64(data["ProjectId"]),
                Name = data["Name"].ToString(),
                ProjectStatusTypeId = (byte)(data["ProjectStatusTypeId"])
            };

            Assert.That(_model, Is.Not.Null);
            Assert.That( _model.ProjectId, Is.EqualTo(projectId));
            Assert.That(_model.Name, Is.EqualTo(_project.Name));
            Assert.That(_model.ProjectStatusTypeId, Is.EqualTo((byte)_project.ProjectStatusTypeId));
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectAddressValidation(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;
            
            string result = projectController.ProjectAddressValidation(projectId);

            var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
            var errors = jo["errors"];

            Assert.That(result, Is.Not.EqualTo(string.Empty));

            List<string> errorList = new List<string>
            {
                "Address Line 1",
                "City",
                "Zip Code",
                "Country",
                "State/Province"
            };

            if(errors.ToArray().Count() > 0)
            {
                for (int i = 0; i < errors.ToArray().Count(); i++)
                {
                    var innerErrors = Newtonsoft.Json.Linq.JObject.Parse(errors[i].ToString());

                    for ( int a = 0; a < innerErrors["errors"].ToArray().Count(); a++)
                    {
                        switch(a)
                        {
                            case 0:
                                Assert.That(innerErrors["errors"].ToArray()[a].ToString(), Is.Not.EqualTo(string.Empty));
                                break;
                            case 1:
                                Assert.That(errorList.Contains(innerErrors["errors"].ToArray()[a].ToString()), Is.EqualTo(true));
                                break;
                            case 2:
                                Assert.That(errorList.Contains(innerErrors["errors"].ToArray()[a].ToString()), Is.EqualTo(true));
                                break;
                            case 3:
                                Assert.That(errorList.Contains(innerErrors["errors"].ToArray()[a].ToString()), Is.EqualTo(true));
                                break;
                            case 4:
                                Assert.That(errorList.Contains(innerErrors["errors"].ToArray()[a].ToString()), Is.EqualTo(true));
                                break;
                            case 5:
                                Assert.That(errorList.Contains(innerErrors["errors"].ToArray()[a].ToString()), Is.EqualTo(true));
                                break;
                        }
                        
                    }
                }
            }
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST")]
        public void TestProjectController_ProjectDelete(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId).OrderBy(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            ProjectModel _model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;

            ViewResult result = projectController.ProjectDelete(_model) as ViewResult;
            Assert.That(this.response.HasError, Is.EqualTo(false));
            Assert.That(result, Is.Not.Null);

            bool deleted = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectId == _projectId).Select(p => p.Deleted).FirstOrDefault();
            Assert.That(deleted, Is.EqualTo(true));
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST")]
        public void TestProjectController_ProjectsDelete(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            List<long> _projectIds = this.db.Context.Projects.Where( p => p.OwnerId == user.UserId && p.Name.Contains("AA")).Select(p => p.ProjectId).Take(3).ToList();
            
            ProjectsModel model = new ProjectsModel();

            PagedList<ProjectListModel> items = new PagedList<ProjectListModel>();

            foreach(long projectId in _projectIds)
            {
                ProjectListModel item = new ProjectListModel();
                item.ProjectId = projectId;
                items.Add(item);
            }

            model.Items = items;

            projectController.ViewData["PageMessages"] = new Messages();
            projectController.ViewData["KeyMessages"] = new Messages();

            FormCollection collection = new FormCollection();

            for (int i =0; i < model.Items.Count; i++)
            {
                collection[model.Items[i].ProjectId.ToString()] = model.Items[i].ProjectId.ToString();
            }

            ViewResult result = projectController.ProjectsDelete(model, collection) as ViewResult;

            Assert.That(this.response.HasError, Is.EqualTo(false));
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("Projects"));

        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST")]
        public void TestProjectController_ProjectDuplicate(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ProjectModel model = projectService.GetProjectModel(user, projectId).Model as ProjectModel;
            projectController.ViewData["PageMessages"] = new Messages();
            projectController.ViewData["KeyMessages"] = new Messages();

            projectController.Session["RequestAction"] = null;
            projectController.Session["RequestController"] = null;
            projectController.Session["ProjectId"] = null;
            projectController.Session["QuoteId"] = null;
            projectController.Session["CommissionRequestid"] = null;

            ViewResult result = projectController.ProjectDuplicate(model) as ViewResult;

            Assert.That(this.response.HasError, Is.EqualTo(false));
            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.Model, Is.Not.EqualTo(null));
            Assert.That(result.Model.GetType(), Is.EqualTo(typeof(ProjectsModel)));
            Assert.That(result.ViewName, Is.EqualTo("Projects"));

        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectEdit(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ViewResult result = projectController.ProjectEdit(projectId, null, null) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("ProjectEdit"));

            Project project = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectId == projectId).FirstOrDefault();
            ProjectModel model = result.Model as ProjectModel;

            Assert.That(model.ProjectId, Is.EqualTo(projectId));
            Assert.That(model.ProjectLeadStatusTypeId, Is.EqualTo(project.ProjectLeadStatusTypeId));
            Assert.That(model.ProjectOpenStatusTypeId, Is.EqualTo(project.ProjectOpenStatusTypeId));
            Assert.That(model.ProjectStatusTypeId, Is.EqualTo((byte)project.ProjectStatusTypeId));
            Assert.That(model.ProjectTypeId, Is.EqualTo(project.ProjectTypeId));
            Assert.That(model.SellerAddress.AddressId, Is.EqualTo(project.SellerAddressId));
            Assert.That(model.ShipToAddress.AddressId, Is.EqualTo(project.ShipToAddressId));
            Assert.That(model.CustomerAddress.AddressId, Is.EqualTo(project.CustomerAddressId));
            Assert.That(model.EngineerAddress.AddressId, Is.EqualTo(project.EngineerAddressId));
            Assert.That(model.ConstructionTypeId, Is.EqualTo(project.ConstructionTypeId));
            Assert.That(model.Active, Is.EqualTo(Convert.ToBoolean(project.ActiveVersion)));
            Assert.That(model.ActualCloseDate, Is.EqualTo(project.ActualCloseDate));
            Assert.That(model.BidDate, Is.EqualTo(project.BidDate));
            Assert.That(model.Name, Is.EqualTo(project.Name));
            Assert.That(model.NumberOfFloors, Is.EqualTo(project.NumberOfFloors));
            Assert.That(model.OwnerId, Is.EqualTo(user.UserId));
            Assert.That(model.ProjectDate, Is.EqualTo(project.ProjectDate));
            Assert.That(model.VerticalMarketTypeId, Is.EqualTo(project.VerticalMarketTypeId));

        }

        [Test]
        [Category("ProjectController")]
        [TestCase("GET", "ProjectId,QuoteId,CommisionRequestId")]
        [TestCase("GET", "ProjectId,QuoteId")]
        [TestCase("GET", "ProjectId")]
        public void TestProjectController_GetRequestUrl(string httpMethod, string testValue)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;


            long _quoteId = 1234;
            long _commissionRequestId = 2345;
            string _previousRequestUrl = "/Projectdashboard/Project";
            string result;

            switch (testValue)
            {
                case "ProjectId,QuoteId,CommisionRequestId":
                 result = projectController.GetRequestUrl(projectId, _quoteId, _commissionRequestId, _previousRequestUrl);
                Assert.That(result, Is.Not.Empty);
                Assert.That(result, Is.EqualTo("ProjectDashboard/CommissionRequest/" + projectId + "/" + _quoteId + "?CommissionRequestId=" + _commissionRequestId));
                    break;
                case "ProjectId,QuoteId":
                     result = projectController.GetRequestUrl(projectId, _quoteId,null, _previousRequestUrl);
                    Assert.That(result, Is.Not.Empty);
                    Assert.That(result, Is.EqualTo("ProjectDashBoard/Quote/" + projectId + "/" + _quoteId));
                    break;
                case "ProjectId":
                     result = projectController.GetRequestUrl(projectId,null,null,_previousRequestUrl);
                    Assert.That(result, Is.Not.Empty);
                    Assert.That(result, Is.EqualTo("ProjectDashboard/Project" + projectId));
                    break;
            }
           
        }

        [Test]
        [Category("ProjectController")]
        [TestCase("GET","ProjectId,QuoteId,CommissionRequestId")]
        [TestCase("GET","ProjectId,QuoteId")]
        [TestCase("GET","ProjectId")]
        public void TestProjectController_SetRequestSession(string httpMethod, string testValues)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;
            long _quoteId = 1234;
            long _commissionRequestId = 2345;
            string _previousRequestUrl = "/Projectdashboard/Project";

            switch(testValues)
            {
                case "ProjectId,QuoteId,CommissionRequestId":
                    projectController.SetRequestSession(projectId, _quoteId, _commissionRequestId, _previousRequestUrl);
                    Assert.That(projectController.Session["RequestAction"], Is.EqualTo("CommissionRequest"));
                    Assert.That(projectController.Session["RequestController"], Is.EqualTo("ProjectDashboard"));
                    Assert.That(projectController.Session["ProjectId"], Is.EqualTo(projectId));
                    Assert.That(projectController.Session["QuoteId"], Is.EqualTo(_quoteId));
                    Assert.That(projectController.Session["CommissionRequestid"], Is.EqualTo(_commissionRequestId));
                    break;
                case "ProjectId,QuoteId":
                    projectController.SetRequestSession(projectId, _quoteId, null, _previousRequestUrl);
                    Assert.That(projectController.Session["RequestAction"], Is.EqualTo("Quote"));
                    Assert.That(projectController.Session["RequestController"], Is.EqualTo("ProjectDashboard"));
                    Assert.That(projectController.Session["ProjectId"], Is.EqualTo(projectId));
                    Assert.That(projectController.Session["QuoteId"], Is.EqualTo(_quoteId));
                    break;
                case "ProjectId":
                    projectController.SetRequestSession(projectId,null,null, _previousRequestUrl);
                    Assert.That(projectController.Session["RequestAction"], Is.EqualTo("Project"));
                    Assert.That(projectController.Session["ProjectId"], Is.EqualTo(projectId));
                    break;
            }
           
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectEditDetails(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ViewResult result = projectController.ProjectEditDetails(projectId) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("ProjectEditDetails"));

            Project project = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectId == projectId).FirstOrDefault();

            ProjectModel model = result.Model as ProjectModel;
            Assert.That(model.ProjectId, Is.EqualTo(projectId));
            Assert.That(model.ProjectLeadStatusTypeId, Is.EqualTo(project.ProjectLeadStatusTypeId));
            Assert.That(model.ProjectOpenStatusTypeId, Is.EqualTo(project.ProjectOpenStatusTypeId));
            Assert.That(model.ProjectStatusTypeId, Is.EqualTo((byte)project.ProjectStatusTypeId));
            Assert.That(model.ProjectTypeId, Is.EqualTo(project.ProjectTypeId));
            Assert.That(model.SellerAddress.AddressId, Is.EqualTo(project.SellerAddressId));
            Assert.That(model.ShipToAddress.AddressId, Is.EqualTo(project.ShipToAddressId));
            Assert.That(model.CustomerAddress.AddressId, Is.EqualTo(project.CustomerAddressId));
            Assert.That(model.EngineerAddress.AddressId, Is.EqualTo(project.EngineerAddressId));
            Assert.That(model.ConstructionTypeId, Is.EqualTo(project.ConstructionTypeId));
            Assert.That(model.Active, Is.EqualTo(Convert.ToBoolean(project.ActiveVersion)));
            Assert.That(model.ActualCloseDate, Is.EqualTo(project.ActualCloseDate));
            Assert.That(model.BidDate, Is.EqualTo(project.BidDate));
            Assert.That(model.Name, Is.EqualTo(project.Name));
            Assert.That(model.NumberOfFloors, Is.EqualTo(project.NumberOfFloors));
            Assert.That(model.OwnerId, Is.EqualTo(user.UserId));
            Assert.That(model.ProjectDate, Is.EqualTo(project.ProjectDate));
            Assert.That(model.VerticalMarketTypeId, Is.EqualTo(project.VerticalMarketTypeId));
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectEditLocation(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ViewResult result = projectController.ProjectEditLocation(projectId) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("ProjectEditLocation"));
            Assert.That(result.Model, Is.Not.EqualTo(null));

            Assert.That(result.Model, Is.EqualTo(projectId.ToString()));

        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectEditDealerContractor(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ViewResult result = projectController.ProjectEditDealerContractor(projectId) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.Model, Is.EqualTo(projectId.ToString()));
            Assert.That(result.ViewName, Is.EqualTo("ProjectEditDealerContractor"));
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectEditSeller(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ViewResult result = projectController.ProjectEditSeller(projectId) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.Model, Is.EqualTo(projectId.ToString()));
            Assert.That(result.ViewName, Is.EqualTo("ProjectEditSeller"));
        }

        [Test]
        [Category("ProjectController")]
        [TestCase("GET", 497640822926229504)]
        public void TestProjectController_AddressCompare(string httpMethod, long projectId)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ProjectModel oldModel = projectService.GetProjectModel(user, projectId).Model as ProjectModel;
            ProjectModel newModel = new ProjectModel();
            newModel.CustomerAddress.AddressLine1 = "5151 San Felipe St";
            newModel.CustomerAddress.Location = "Houston";
            newModel.CustomerAddress.StateId = 57;
            newModel.CustomerAddress.PostalCode = "77056";
            newModel.CustomerAddress.CountryCode = "US";
            Dictionary<string, bool> result = projectController.AddressCompare(oldModel, newModel);

            Assert.That(result["IsSameOnDealerAddress"], Is.EqualTo(true));

            newModel.CustomerAddress.AddressLine1 = "5151";

            result = projectController.AddressCompare(oldModel, newModel);

            Assert.That(result["IsSameOnDealerAddress"], Is.EqualTo(false));
        }

        [Test]
        [Category("ProjectController")]
        [TestCase("POST")]
        public void TestProjectController_ProjectExport(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;
            ProjectsModel model = new ProjectsModel();
            model = projectService.GetProjectsModel(user, model).Model as ProjectsModel;
            ViewResult result = projectController.ProjectExport(model) as ViewResult;
            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        [Category("ProjectController_GET")]
        [TestCase("GET")]
        public void TestProjectController_ProjectQuotes(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ProjectQuotesModel model = new ProjectQuotesModel();
            model.ProjectId = projectId;
            model = projectService.GetProjectQuotesModel(user, model).Model as ProjectQuotesModel;
            ViewResult result = projectController.ProjectQuotes(model) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("ProjectQuotes"));
            Assert.That(result.Model, Is.Not.EqualTo(null));
            Assert.That((result.Model as ProjectQuotesModel).Items.Count, Is.GreaterThan(0));

            if ((result.Model as ProjectQuotesModel).Items.Count > 0)
            {
                QuoteListModel item = (result.Model as ProjectQuotesModel).Items.First();
                Assert.That(item.QuoteId, Is.Not.EqualTo(null));
                Assert.That(item.Title, Is.Not.EqualTo(null));
            }
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST")]
        public void TestProjectController_UpdateProjectsDeleteItem(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ProjectsModel model = new ProjectsModel();
            model.ProjectId = projectId;
            FormCollection form = new FormCollection();
            ProjectListModel projectListModel = new ProjectListModel();

            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectStatusTypeId == ProjectStatusTypeEnum.Inactive).OrderByDescending(p => p.ProjectId).Select(p => p.ProjectId).FirstOrDefault();
            projectListModel.ProjectId = _projectId;
            model.Items.Add(projectListModel);

            System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
            nameValueCollection.Add(_projectId.ToString(), _projectId.ToString());
            form.Add(nameValueCollection);

            ViewResult result = projectController.UpdateProjectsDeleteItem(model, form) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("Projects"));
            Assert.That(result.Model, Is.Not.EqualTo(null));
            Assert.That((result.Model as ProjectsModel).DeleteProjects.First().ProjectId, Is.EqualTo(_projectId));
        }

        [Test]
        [Category("ProjectController")]
        [TestCase("GET")]
        public void TestProjectController_GetFormFilter(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            FormCollection form = new FormCollection();
            form.Add("UserId", "1234");
            form.Add("BusinessId", "123456");
            form.Add("ProjectStatusTypeId","1");
            form.Add("ProjectOpenStatusTypeId", "2");
            form.Add("ProjectLeadStatusTypeId", "3");
            form.Add("DateTypeId", "4");
            form.Add("OnlyAlertProjects", "false");
            form.Add("ShowDeleteProjects", "false");
            form.Add("TotalRecords", "127");

            Dictionary<string, object> result = projectController.GetFormFilters(form) as Dictionary<string,object>;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.Count, Is.GreaterThan(0));
            Assert.That(result["UserId"], Is.EqualTo(1234));
            Assert.That(result["BusinessId"], Is.EqualTo(123456));
            Assert.That(result["ProjectStatusTypeId"], Is.EqualTo(1));
            Assert.That(result["ProjectOpenStatusTypeId"], Is.EqualTo(2));
            Assert.That(result["ProjectLeadStatusTypeId"], Is.EqualTo(ProjectLeadStatusTypeEnum.OpenOrder));
            Assert.That(result["DateTypeId"], Is.EqualTo(4));
            Assert.That(result["OnlyAlertProjects"], Is.EqualTo(false));
            Assert.That(result["ShowDeleteProjects"], Is.EqualTo(false));
            Assert.That(result["TotalRecords"], Is.EqualTo(127));
        }

        [Test]
        [Category("ProjectController")]
        [TestCase("GET")]
        public void TestProjectController_Projects_ShouldReturnProjectsViewWithProjectsModel(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            ProjectsModel projectsModel = new ProjectsModel();
            projectsModel.ProjectId = projectId;

            FormCollection form = new FormCollection();

            projectController.Session["RequestAction"] = null;
            projectController.Session["RequestController"] = null;
            projectController.Session["ProjectId"] = null;
            projectController.Session["QuoteId"] = null;
            projectController.Session["CommissionRequestid"] = null;

            ViewResult result = projectController.Projects(projectsModel, form, null) as ViewResult;

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result.Model, Is.Not.EqualTo(null));
            Assert.That(result.ViewName, Is.EqualTo("Projects"));
            Assert.IsInstanceOf<ProjectsModel>(result.Model);
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST", "ProjectStatusIsOpen")]
        [TestCase("POST", "ProjectStatusIsNotOpen")]
        public void TestProjectController_ProjectTransfer_ShouldSendOutEmailAndRedirectToActionProjects(string httpMethod, string projectStatusCase)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            projectController.ViewData["PageMessages"] = new Messages();
            projectController.ViewData["KeyMessages"] = new Messages();

            long _projectId;

            if (projectStatusCase == "ProjectStatusIsOpen")
            {
                _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open)
                                   .OrderByDescending(p => p.ProjectId)
                                   .Select(p => p.ProjectId)
                                   .FirstOrDefault();
              
                RedirectResult result = projectController.ProjectTransfer(_projectId, "aaron.nguyen@daikincomfort.com") as RedirectResult;

                Assert.That(result, Is.Not.EqualTo(null));
                Assert.That(result.Url, Is.EqualTo("/Projectdashboard/Projects"));
            }
            if(projectStatusCase == "ProjectStatusIsNotOpen")
            {
                _projectId = this.db.Projects.Where(p => p.OwnerId == user.UserId && p.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost)
                                        .OrderByDescending(p => p.ProjectId)
                                        .Select(p => p.ProjectId)
                                        .FirstOrDefault();

                RedirectResult result = projectController.ProjectTransfer(_projectId, "aaron.nguyen@daikincomfort.com") as RedirectResult;
              
                Assert.That(result, Is.EqualTo(null));
               
            }
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST", "aaron.nguyen@daikincomfort.com")]
        public void TestProjectController_ProjectTransferVerifyEmail_ShouldReturnEmptyResult(string httpMethod, string email)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            EmptyResult result = projectController.ProjectTransferVerfiyEmail(email);

            Assert.IsInstanceOf<EmptyResult>(result);
            Assert.That(result, Is.Not.EqualTo(null)); 
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST")]
        public void TestProjectController_ProjectUndelete_ShouldReturnRedirectToActionProject(string httpMethod)
        {
            SetUpProjectControllerForTesting(httpMethod);
            projectController.CurrentUser = user;

            projectController.ViewData["PageMessages"] = new Messages();
            projectController.ViewData["KeyMessages"] = new Messages();

            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId && p.Deleted == true)
                                  .OrderByDescending(p => p.ProjectId)
                                  .Select(p => p.ProjectId)
                                  .FirstOrDefault();

            ProjectModel model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
            RedirectToRouteResult result = projectController.ProjectUndelete(model) as RedirectToRouteResult;
            
            Assert.That(result, Is.Not.EqualTo(null));

            Assert.That(result.RouteValues["action"], Is.EqualTo("Project"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("Projectdashboard"));
            Assert.That(result.RouteValues["ProjectId"], Is.EqualTo(_projectId));
            Assert.That(result.RouteValues["Active"], Is.EqualTo(true));
            Assert.That(result.RouteValues["Deleted"], Is.EqualTo(false));
            Assert.That(result.RouteValues["OwnerId"], Is.EqualTo(user.UserId));
            
        }

        [Test]
        [Category("ProjectController_POST")]
        [TestCase("POST","UserHasPermission")]
        [TestCase("POST","UserDoesNotHavePermission")]
        public void TestProjectController_Tools_ShouldReturnTheToolsView(string httpMethod, string userPermisison)
        {
            SetUpProjectControllerForTesting(httpMethod);

            if (userPermisison == "UserHasPermission")
            {
                projectController.CurrentUser = user;

                ViewResult result = projectController.Tools() as ViewResult;

                Assert.That(result, Is.Not.EqualTo(null));

                List<ToolModel> model = result.Model as List<ToolModel>;

                Assert.That(model, Is.Not.EqualTo(null));
                Assert.That(model.Count, Is.GreaterThan(0));
                Assert.That(model.Count, Is.GreaterThan(8));

                if (model.Count > 8)
                {
                    Assert.That(model[0].Name, Is.EqualTo("VRV Xpress"));
                    Assert.That(model[1].Name, Is.EqualTo("WEBXpress (VRV)"));
                    Assert.That(model[2].Name, Is.EqualTo("Ventilation Xpress"));
                    Assert.That(model[3].Name, Is.EqualTo("8-Zone Multi-Split Selection Tool"));
                    Assert.That(model[4].Name, Is.EqualTo("Psychrometric Tool"));
                    Assert.That(model[5].Name, Is.EqualTo("Unit Converter"));
                    Assert.That(model[6].Name, Is.EqualTo("Ventilation Rate Calculator"));
                    Assert.That(model[7].Name, Is.EqualTo("VRV Refrigerant Charge Calculator"));
                    Assert.That(model[8].Name, Is.EqualTo("VRV Mechanical Schedule Generator"));
                }
            }
            if(userPermisison == "UserDoesNotHavePermission")
            {
                projectController.CurrentUser = new UserSessionModel();
                projectController.CurrentUser.UserId = 1234;

                ViewResult result = projectController.Tools() as ViewResult;

                Assert.That(result, Is.Not.EqualTo(null));

                List<ToolModel> model = result.Model as List<ToolModel>;

                Assert.That(model, Is.Not.EqualTo(null));
                Assert.That(model.Count, Is.EqualTo(0));
            }
        }

        private void SetUpProjectControllerForTesting(string httpMethod)
        {
            //Arrange
            var httpContextMock = FakeHttpContext(httpMethod);
            var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

            var routeData = new RouteData();
            routeData.Values.Add("key1", "/ProjectDashboard/Projects");

            var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
            projectController.ControllerContext = controllerContext;
        }

    }

    
}
