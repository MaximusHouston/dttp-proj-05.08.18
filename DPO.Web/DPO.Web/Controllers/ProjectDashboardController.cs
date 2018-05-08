using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPO.Common;
using DPO.Domain;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Net.Mail;
using Newtonsoft.Json;
using DPO.Common.Models.Project;
using System.Text.RegularExpressions;
using DPO.Model.Light;
using DPO.Services.Light;
using log4net;
using StackExchange.Profiling;

namespace DPO.Web.Controllers
{

    public partial class ProjectDashboardController : BaseController
    {
        public BasketServices basketService = new BasketServices();
        public DiscountRequestServices discountRequestService = new DiscountRequestServices();
        public HtmlServices htmlService = new HtmlServices();
        public OverviewServices overviewService = new OverviewServices();
        public ProjectServices projectService = new ProjectServices();
        public QuoteServices quoteService = new QuoteServices();
        public OrderServices orderService = new OrderServices();
        public OrderServiceLight orderServiceLight = new OrderServiceLight();
        public CommissionRequestServices commissionRequestService = new CommissionRequestServices();
        public FinaliseModelService finaliseModelSvc = new FinaliseModelService();

        public static List<long> items = new List<long>();
        public static List<long> projectsDeleteId = new List<long>();

        public ILog log;

        public ProjectDashboardController()
        {
            projectService.ModelState = this.ModelState;
            quoteService.ModelState = this.ModelState;
            basketService.ModelState = this.ModelState;
            htmlService.ModelState = this.ModelState;
            discountRequestService.ModelState = this.ModelState;
            this.log = Log;
        }

        #region projects

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult Project(long? projectId)
        {
            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);

            ProcessServiceResponse(this.ServiceResponse);

            this.RouteData.Values["action"] = "Project";

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("Project", this.ServiceResponse.Model) : View("Project", this.ServiceResponse.Model));
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public JsonResult GetProject(string projectIdStr)
        {
            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, Convert.ToInt64(projectIdStr));
            ProcessServiceResponse(this.ServiceResponse);
            //return Json(new { Data = this.ServiceResponse.Model }, JsonRequestBehavior.AllowGet);
            var result = JsonConvert.SerializeObject(this.ServiceResponse);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public string ProjectAddressValidation(long ProjectId)
        {
            var response = new AddressServices().ValidateAddressesForDARSubmission(this.CurrentUser, ProjectId);
            var str = JsonConvert.SerializeObject(response, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return str;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectDelete(ProjectModel model)
        {
            this.ServiceResponse = projectService.Delete(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            //return AJAXRedirectTo("Projects", "Projectdashboard", null);
            return RedirectToAction("Project", "Projectdashboard", this.ServiceResponse.Model);
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectsDelete(ProjectsModel model, FormCollection collection)
        {
            if (model.DeleteProjects == null)
            {
                model.DeleteProjects = new List<ProjectModel>();
            }

            for (int i = 0; i < model.Items.Count; i++)
            {
                var item = collection[model.Items[i].ProjectId.ToString()];

                if (item != null)
                {
                    projectsDeleteId.Add((long)model.Items[i].ProjectId);
                }
            }

            if (model.DeleteProjects.Count == 0)
            {
                foreach (long id in projectsDeleteId)
                {
                    this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, (long)id);
                    model.DeleteProjects.Add((ProjectModel)this.ServiceResponse.Model);
                }
            }

            this.ServiceResponse = projectService.DeleteProjects(this.CurrentUser, model.DeleteProjects);

            if (this.ServiceResponse.IsOK)
            {
                this.ServiceResponse.Messages.Clear();
                Message message = new Message();
                message.Type = MessageTypeEnum.Success;
                message.Key = Resources.ResourceUI.ProjectsDelete;
                message.Text = Resources.ResourceUI.DeleteProjectMessage;
                this.ServiceResponse.Messages.Add(message);
            }

            ProcessServiceResponse(this.ServiceResponse);

            model.DeleteProjects.Clear();
            projectsDeleteId.Clear();
            items.Clear();

            ProjectsModel projectsModel = TempData["ProjectsModel"] as ProjectsModel;

            if (projectsModel != null)
            {
                model.BusinessId = projectsModel.BusinessId;
                model.UserId = projectsModel.UserId;
                model.ProjectStatusTypeId = projectsModel.ProjectStatusTypeId;
                model.ProjectOpenStatusTypeId = projectsModel.ProjectOpenStatusTypeId;
                model.ProjectLeadStatusTypeId = projectsModel.ProjectLeadStatusTypeId;
                model.DateTypeId = projectsModel.DateTypeId;
                model.ProjectStartDate = projectsModel.ProjectStartDate;
                model.ProjectStartEnd = projectsModel.ProjectStartEnd;
                model.OnlyAlertedProjects = projectsModel.OnlyAlertedProjects;
                model.ShowDeletedProjects = projectsModel.ShowDeletedProjects;
                model.ModelMode = ModelModeEnum.Edit;
            }

#pragma warning disable CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'
            this.ServiceResponse = projectService.GetProjectsModel(this.CurrentUser, model);
#pragma warning restore CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'

            //return AJAXRedirectTo("projects", "Projectdashboard", null);
            return View("Projects", this.ServiceResponse.Model);

        }

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectDuplicate(ProjectModel model)
        {
            this.ServiceResponse = projectService.Duplicate(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return Projects(new ProjectsModel(), new FormCollection(), null);

        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEdit(long? projectId, long? quoteId, long? commissionRequestId)
        {
            string previousRequestUrl = string.Empty;

            if (Request.UrlReferrer != null)
                previousRequestUrl = Request.UrlReferrer.PathAndQuery;

            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);

            ProcessServiceResponse(this.ServiceResponse);

            ProjectModel model = this.ServiceResponse.Model as ProjectModel;

            //Session["requestUrl"] = GetRequestUrl(projectId, quoteId, commissionRequestId);
            // Session.Abandon();
            //Session["RequestAction"] = "";
            //Session["RequestController"] = "";
            //Session["ProjectId"] = "";
            //Session["QuoteId"] = "";
            //Session["CommissionRequestid"] = "";

            SetRequestSession(projectId, quoteId, commissionRequestId, previousRequestUrl);

            return View("ProjectEdit", model);

        }

        public string GetRequestUrl(long? projectId, long? quoteId, long? commissionRequestId, string previousRequestUrl)
        {
            string requestUrl = "";

            if (projectId != null && quoteId != null && commissionRequestId != null)
            {
                requestUrl = "ProjectDashboard/CommissionRequest/" + projectId + "/" + quoteId + "?CommissionRequestId=" + commissionRequestId;
            }
            else if (projectId != null && quoteId != null && previousRequestUrl.Contains("CommissionRequest"))
            {
                requestUrl = "ProjectDashboard/CommissionRequest/" + projectId + "/" + quoteId;
            }
            else if (projectId != null && quoteId != null)
            {
                requestUrl = "ProjectDashBoard/Quote/" + projectId + "/" + quoteId;
            }
            else if (projectId != null)
            {
                requestUrl = "ProjectDashboard/Project" + projectId;
            }

            return requestUrl;
        }

        public void SetRequestSession(long? projectId, long? quoteId, long? commissionRequestId, string previousRequestUrl)
        {
            if (projectId != null && quoteId != null && commissionRequestId != null)
            {
                //requestUrl = "ProjectDashboard/CommissionRequest/" + projectId + "/" + quoteId + "?CommissionRequestId=" + commissionRequestId;

                Session["RequestAction"] = "CommissionRequest";
                Session["RequestController"] = "ProjectDashboard";
                Session["ProjectId"] = projectId;
                Session["QuoteId"] = quoteId;
                Session["CommissionRequestid"] = commissionRequestId;

            }
            else if (projectId != null && quoteId != null && previousRequestUrl.Contains("CommissionRequest"))
            {
                Session["RequestAction"] = "CommissionRequest";
                Session["RequestController"] = "ProjectDashboard";
                Session["ProjectId"] = projectId;
                Session["QuoteId"] = quoteId;
            }
            else if (projectId != null && quoteId != null)
            {
                //requestUrl = "ProjectDashBoard/Quote/" + projectId + "/" + quoteId;

                Session["RequestAction"] = "Quote";
                Session["RequestController"] = "ProjectDashboard";
                Session["ProjectId"] = projectId;
                Session["QuoteId"] = quoteId;
            }
            else if (projectId != null)
            {
                //requestUrl = "ProjectDashboard/Project" + projectId;
                Session["RequestAction"] = "Project";
                Session["ProjectId"] = projectId;

            }
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEditDetails(long? projectId)
        {

            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);

            ProcessServiceResponse(this.ServiceResponse);

            ProjectModel model = this.ServiceResponse.Model as ProjectModel;

            return View("ProjectEditDetails", model);

        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEditLocation(long? projectId)
        {
            String projectIdStr = projectId.ToString();
            return View("ProjectEditLocation", model: projectIdStr);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEditDealerContractor(long? projectId)
        {
            String projectIdStr = projectId.ToString();
            return View("ProjectEditDealerContractor", model: projectIdStr);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEditSeller(long? projectId)
        {
            String projectIdStr = projectId.ToString();
            return View("ProjectEditSeller", model: projectIdStr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectEdit(ProjectModel model, FormCollection collection, string requestUrl)
        {
            #region 05/24/2016 - Delete Code after 07/01/2016

            //int projectDetailStateSelectCount = 0;
            //if (collection["ProjectDetailStateSelectCount"] != null)
            //{
            //    int.TryParse(collection["ProjectDetailStateSelectCount"].ToString(), out projectDetailStateSelectCount);
            //}
            //int projectDetailCountrySelectCount = 0;
            //if (collection["ProjectDetailsCountrySelectCount"] != null)
            //{
            //    int.TryParse(collection["ProjectDetailsCountrySelectCount"].ToString(), out projectDetailCountrySelectCount);
            //}
            //int projectLocationStateSelectCount = 0;
            //if (collection["ProjectLocationStateSelectCount"] != null)
            //{
            //    int.TryParse(collection["ProjectLocationStateSelectCount"].ToString(), out projectLocationStateSelectCount);
            //}

            //int projectLocationCountrySelectCount = 0;
            //if (collection["ProjectLocationCountrySelectCount"] != null)
            //{
            //    int.TryParse(collection["ProjectLocationCountrySelectCount"].ToString(), out projectLocationCountrySelectCount);
            //}
            //if (collection["ShipToAddress.StateId"] != null)
            //{
            //    if (projectDetailStateSelectCount > projectLocationStateSelectCount)
            //    {
            //        model.ShipToAddress.StateId = Int32.Parse(collection["ShipToAddress.StateId"].ToString());
            //    }
            //    else if (projectDetailStateSelectCount < projectLocationStateSelectCount)
            //    {
            //        if (collection["StateIdForProjectLocation"] != null)
            //        {
            //            model.ShipToAddress.StateId = Int32.Parse(collection["StateIdForProjectLocation"].ToString());
            //        }
            //    }
            //}

            #endregion
            if (this.ServiceResponse != null)
                this.ServiceResponse.Messages.Items.Clear();


            // Set project date if new project
            if (model.ProjectDate == null)
            {
                model.ProjectDate = DateTime.Now;
            }

            ProjectModel oldProjectModel = projectService.GetProjectModel(this.CurrentUser, model.ProjectId).Model as ProjectModel;

            //Compare oldModle with currentModel, If they are differnce, the ndo the AddressVlaidation. If they are the same then ByPass the AddressValidation

            Dictionary<string, bool> checkAddresses = new Dictionary<string, bool>();

            if (oldProjectModel.CustomerAddress != null || oldProjectModel.EngineerAddress != null || oldProjectModel.ShipToAddress != null)
                checkAddresses = AddressCompare(oldProjectModel, model);

            if (model.ShipToAddress.AddressLine1 != null)
            {
                this.ServiceResponse = projectService.ValidateProjectLocationAddress(model);
            }

            if (model.EngineerAddress.AddressLine1 != null)
            {
                this.ServiceResponse = projectService.ValidateEngineerAddress(model);
            }
            if (model.CustomerAddress.AddressLine1 != null)
            {
                this.ServiceResponse = projectService.ValidateDealorAddress(model);
            }

            bool useShippingAddressAsIs = false;

            if (collection["useShippingAddressAsIs"] != null)
            {
                bool.TryParse(collection["useShippingAddressAsIs"].ToString(), out useShippingAddressAsIs);
            }

            bool useEngineerAddressAsIs = false;
            if (collection["useEngineerAddressAsIs"] != null)
            {
                bool.TryParse(collection["useEngineerAddressAsIs"].ToString(), out useEngineerAddressAsIs);
            }

            bool useDealerAddressAsIs = false;
            if (collection["useDealerAddressAsIs"] != null)
            {
                bool.TryParse(collection["useDealerAddressAsIs"].ToString(), out useDealerAddressAsIs);
            }

            bool useSuggestionShippingAddress = false;

            if (collection["useSuggestionShippingAddress"] != null)
            {
                bool.TryParse(collection["useSuggestionShippingAddress"].ToString(), out useSuggestionShippingAddress);
            }

            if (useSuggestionShippingAddress)
            {
                model.ShipToAddress.AddressLine1 = model.ShippingSuggestionAddress["AddressLine1"];
                model.ShipToAddress.AddressLine2 = model.ShippingSuggestionAddress["AddressLine2"];
                model.ShipToAddress.Location = model.ShippingSuggestionAddress["City"];
                model.ShipToAddress.StateId = projectService.GetStateIdByState(model.ShippingSuggestionAddress["State"]);
                model.ShipToAddress.PostalCode = model.ShippingSuggestionAddress["ZipCode"];
            }

            bool useSuggestionDealorAddress = false;

            if (collection["useSuggestionDealorAddress"] != null)
            {
                bool.TryParse(collection["useSuggestionDealorAddress"].ToString(), out useSuggestionDealorAddress);
            }

            if (useSuggestionDealorAddress)
            {
                model.CustomerAddress.AddressLine1 = model.DealorContractorSuggestionAddress["AddressLine1"];
                model.CustomerAddress.AddressLine2 = model.DealorContractorSuggestionAddress["AddressLine2"];
                model.CustomerAddress.Location = model.DealorContractorSuggestionAddress["City"];
                model.CustomerAddress.StateId = projectService.GetStateIdByState(model.DealorContractorSuggestionAddress["State"]);
                model.CustomerAddress.PostalCode = model.DealorContractorSuggestionAddress["ZipCode"];
            }

            bool useSuggestionSellerAddress = false;

            if (collection["useSuggestionSellerAddress"] != null)
            {
                bool.TryParse(collection["useSuggestionSellerAddress"].ToString(), out useSuggestionSellerAddress);
            }

            if (useSuggestionSellerAddress)
            {
                model.SellerAddress.AddressLine1 = model.SellerSuggestionAddress["AddressLine1"];
                model.SellerAddress.AddressLine2 = model.SellerSuggestionAddress["AddressLine2"];
                model.SellerAddress.Location = model.SellerSuggestionAddress["City"];
                model.SellerAddress.StateId = projectService.GetStateIdByState(model.SellerSuggestionAddress["State"]);
                model.SellerAddress.PostalCode = model.SellerSuggestionAddress["ZipCode"];
            }

            bool useSuggestionEngineerAddress = false;

            if (collection["useSuggestionEngineerAddress"] != null)
            {
                bool.TryParse(collection["useSuggestionEngineerAddress"].ToString(), out useSuggestionEngineerAddress);
            }

            if (useSuggestionEngineerAddress)
            {
                model.EngineerAddress.AddressLine1 = model.EngineerSuggestionAddress["AddressLine1"];
                model.EngineerAddress.AddressLine2 = model.EngineerSuggestionAddress["AddressLine2"];
                model.EngineerAddress.Location = model.EngineerSuggestionAddress["City"];
                model.EngineerAddress.StateId = projectService.GetStateIdByState(model.EngineerSuggestionAddress["State"]);
                model.EngineerAddress.PostalCode = model.EngineerSuggestionAddress["ZipCode"];
            }

            //if ( useSuggestionDealorAddress  ||
            //     useSuggestionEngineerAddress  ||
            //     useSuggestionShippingAddress  ||
            //     useShippingAddressAsIs  ||
            //     useEngineerAddressAsIs  ||
            //     useDealerAddressAsIs  ||
            //     (checkAddresses.Count > 0 && checkAddresses.All(chk => chk.Value == true)))
            //{

            //    if (this.ServiceResponse != null)
            //        this.ServiceResponse.Messages.Clear();

            //    if (this.ServiceResponse == null)
            //    {
            //        this.ServiceResponse = new ServiceResponse();
            //    }
            //}
            //else
            //{
            //    if (checkAddresses.Any(chk => chk.Key == "IsSameOnShippingAddress" && chk.Value == true))
            //    {
            //        int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Project Shipping Address is invalid.Please provide a valid address."));
            //        if (index > -1)
            //        {
            //            this.ServiceResponse.Messages.Items.RemoveAt(index);
            //        }
            //    }
            //    if (checkAddresses.Any(chk => chk.Key == "IsSameOnDealerAddress" && chk.Value == true))
            //    {
            //        int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Dealor/Contractor Address is invalid.Please provide a valid address."));
            //        if (index > 0)
            //        {
            //            this.ServiceResponse.Messages.Items.RemoveAt(index);
            //        }
            //    }
            //    if (checkAddresses.Any(chk => chk.Key == "IsSameOnEngineerAddress" && chk.Value == true))
            //    {
            //        int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Engineer Address is invalid.Please provide a valid address."));
            //        if (index > 0)
            //        {
            //            this.ServiceResponse.Messages.Items.RemoveAt(index);
            //        }
            //    }

            //}

            if (useSuggestionDealorAddress)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Dealer/Contractor Address we recommend"));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            if (useSuggestionEngineerAddress)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Engineer Address we recommend"));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            if (useSuggestionSellerAddress)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Seller Address we recommend"));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            if (useSuggestionShippingAddress)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Shipping Address we recommend"));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }

            if (checkAddresses.Any(chk => chk.Key == "IsSameOnShippingAddress" && chk.Value == true))
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Project Shipping Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            else if (useShippingAddressAsIs)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Project Shipping Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }

            if (checkAddresses.Any(chk => chk.Key == "IsSameOnDealerAddress" && chk.Value == true))
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Dealor/Contractor Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            else if (useDealerAddressAsIs)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains(@"Dealor/Contractor Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }

            if (checkAddresses.Any(chk => chk.Key == "IsSameOnEngineerAddress" && chk.Value == true))
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Engineer Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }
            else if (useEngineerAddressAsIs)
            {
                int index = this.ServiceResponse.Messages.Items.FindIndex(m => m.Text.Contains("Engineer Address is invalid.Please provide a valid address."));
                if (index > -1)
                {
                    this.ServiceResponse.Messages.Items.RemoveAt(index);
                }
            }

            if (this.ServiceResponse != null && this.ServiceResponse.Messages.Items.Count == 0)
            {
                this.ServiceResponse.Messages.HasErrors = false;
            }

            this.ServiceResponse = projectService.PostModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as ProjectModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                if (!projectService.NewRecordAdded)
                {
                    if (Session["RequestAction"] != null && Session["RequestController"] != null)
                    {

                        if (Session["ProjectId"] != null && Session["QuoteId"] != null && Session["CommissionRequestId"] != null)
                        {
                            return AJAXRedirectTo(Session["RequestAction"].ToString(), Session["RequestController"].ToString(),
                                new { ProjectId = Session["ProjectId"].ToString(), QuoteId = Session["QuoteId"].ToString(), CommissionRequestId = Session["CommissionRequestId"].ToString() });
                        }


#pragma warning disable CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
                        if (Session["ProjectId"] != null && Session["QuoteId"] != null && Session["RequestController"] != "CommissionRequest")
#pragma warning restore CS0252 // Possible unintended reference comparison; to get a value comparison, cast the left hand side to type 'string'
                        {
                            return AJAXRedirectTo(Session["RequestAction"].ToString(), Session["RequestController"].ToString(),
                                new { ProjectId = Session["ProjectId"].ToString(), QuoteId = Session["QuoteId"].ToString() });
                        }

                        if (Session["ProjectId"] != null)
                        {
                            return AJAXRedirectTo("Project", "Projectdashboard", new { ProjectId = Session["ProjectId"].ToString() });
                        }
                    }

                    return AJAXRedirectTo("Project", "Projectdashboard", new { ProjectId = model.ProjectId });
                }

                model.NewRecordAdded = true;
            }

            if (model.ProjectLeadStatusTypeId == null)
            {

            }

            return PartialView("ProjectEdit", model);
        }

        public Dictionary<string, bool> AddressCompare(ProjectModel oldModel, ProjectModel newModel)
        {
            bool IsSameOnEngineerAddress = false;
            bool IsSameOnDealerAddress = false;
            bool IsSameOnShippingAddress = false;

            Dictionary<string, bool> checkAddresses = new Dictionary<string, bool>();

            if (oldModel.ProjectId == null && newModel.ShipToAddress.AddressLine1 == null && newModel.EngineerAddress.AddressLine1 == null && newModel.CustomerAddress.AddressLine1 == null)
            {
                checkAddresses.Add("", true);
                return checkAddresses;
            }
            else
            {
                if (oldModel.EngineerAddress.AddressLine1 != null && newModel.EngineerAddress.AddressLine1 != null &&
                    oldModel.EngineerAddress.PostalCode != null && newModel.EngineerAddress.PostalCode != null &&
                    oldModel.EngineerAddress.CountryCode != null && newModel.EngineerAddress.CountryCode != null &&
                    oldModel.EngineerAddress.StateId != null && newModel.EngineerAddress.StateId != null)
                {
                    if (Regex.Replace(oldModel.EngineerAddress.AddressLine1, @"\s+", "") == Regex.Replace(newModel.EngineerAddress.AddressLine1, @"\s+", "") &&
                        Regex.Replace(oldModel.EngineerAddress.CountryCode, @"\s+", "") == Regex.Replace(newModel.EngineerAddress.CountryCode, @"\s+", "") &&
                        Regex.Replace(oldModel.EngineerAddress.Location, @"\s+", "") == Regex.Replace(newModel.EngineerAddress.Location, @"\s+", "") &&
                        Regex.Replace(oldModel.EngineerAddress.PostalCode, @"\s+", "") == Regex.Replace(newModel.EngineerAddress.PostalCode, @"\s+", "") &&
                        oldModel.EngineerAddress.StateId == newModel.EngineerAddress.StateId)
                    {
                        IsSameOnEngineerAddress = true;
                    }

                    if (oldModel.EngineerAddress.AddressLine2 != null && newModel.EngineerAddress.AddressLine2 != null)
                    {
                        if (Regex.Replace(oldModel.EngineerAddress.AddressLine2, @"\s+", "") == Regex.Replace(newModel.EngineerAddress.AddressLine2, @"\s+", ""))
                        {
                            IsSameOnEngineerAddress = true;
                        }
                        else
                        {
                            IsSameOnEngineerAddress = false;
                        }
                    }

                    checkAddresses.Add("IsSameOnEngineerAddress", IsSameOnEngineerAddress);
                }

                if (oldModel.ShipToAddress.AddressLine1 != null && newModel.ShipToAddress.AddressLine1 != null &&
                    oldModel.ShipToAddress.PostalCode != null && newModel.ShipToAddress.PostalCode != null &&
                    oldModel.ShipToAddress.CountryCode != null && newModel.ShipToAddress.CountryCode != null &&
                    oldModel.ShipToAddress.StateId != null && newModel.ShipToAddress.StateId != null)
                {
                    if (Regex.Replace(oldModel.ShipToAddress.AddressLine1, @"\s+", "") == Regex.Replace(newModel.ShipToAddress.AddressLine1, @"\s+", "") &&
                       Regex.Replace(oldModel.ShipToAddress.Location, @"\s+", "") == Regex.Replace(newModel.ShipToAddress.Location, @"\s+", "") &&
                       Regex.Replace(oldModel.ShipToAddress.CountryCode, @"\s+", "") == Regex.Replace(newModel.ShipToAddress.CountryCode, @"\s+", "") &&
                       Regex.Replace(oldModel.ShipToAddress.PostalCode, @"\s+", "") == Regex.Replace(newModel.ShipToAddress.PostalCode, @"\s+", "") &&
                       oldModel.ShipToAddress.StateId == newModel.ShipToAddress.StateId)
                    {
                        IsSameOnShippingAddress = true;
                    }

                    if (oldModel.ShipToAddress.AddressLine2 != null && newModel.ShipToAddress.AddressLine2 != null)
                    {
                        if (Regex.Replace(oldModel.ShipToAddress.AddressLine2, @"\s+", "") == Regex.Replace(newModel.ShipToAddress.AddressLine2, @"\s+", ""))
                        {
                            IsSameOnShippingAddress = true;
                        }
                        else
                        {
                            IsSameOnShippingAddress = false;
                        }
                    }

                    checkAddresses.Add("IsSameOnShippingAddress", IsSameOnShippingAddress);
                }

                if (oldModel.CustomerAddress.AddressLine1 != null && newModel.CustomerAddress.AddressLine1 != null &&
                    oldModel.CustomerAddress.PostalCode != null && newModel.CustomerAddress.PostalCode != null &&
                    oldModel.CustomerAddress.CountryCode != null && newModel.CustomerAddress.CountryCode != null &&
                    oldModel.CustomerAddress.StateId != null && newModel.CustomerAddress.StateId != null)
                {
                    if (Regex.Replace(oldModel.CustomerAddress.AddressLine1, @"\s+", "") == Regex.Replace(newModel.CustomerAddress.AddressLine1, @"\s+", "") &&
                       Regex.Replace(oldModel.CustomerAddress.Location, @"\s+", "") == Regex.Replace(newModel.CustomerAddress.Location, @"\s+", "") &&
                       Regex.Replace(oldModel.CustomerAddress.CountryCode, @"\s+", "") == Regex.Replace(newModel.CustomerAddress.CountryCode, @"\s+", "") &&
                       Regex.Replace(oldModel.CustomerAddress.PostalCode, @"\s+", "") == Regex.Replace(newModel.CustomerAddress.PostalCode, @"\s+", "") &&
                       oldModel.CustomerAddress.StateId == newModel.CustomerAddress.StateId)
                    {
                        IsSameOnDealerAddress = true;
                    }

                    if (oldModel.CustomerAddress.AddressLine2 != null && newModel.CustomerAddress.AddressLine2 != null)
                    {
                        if (Regex.Replace(oldModel.CustomerAddress.AddressLine2, @"\s+", "") == Regex.Replace(newModel.CustomerAddress.AddressLine2, @"\s+", ""))
                        {
                            IsSameOnDealerAddress = true;
                        }
                        else
                        {
                            IsSameOnDealerAddress = false;
                        }
                    }

                    checkAddresses.Add("IsSameOnDealerAddress", IsSameOnDealerAddress);
                }

                return checkAddresses;
            }
        }

        //TODO : this function nolonger used-- comment by Aaron 08/03/2016
        //[HttpPost]
        //[Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        //public JsonResult ProjectUpdate(string model)
        //{
        //    ProjectModel projectModel = JsonConvert.DeserializeObject<ProjectModel>(model);

        //    ServiceResponse response = projectService.GetProjectModel(this.CurrentUser, projectModel.ProjectId);

        //    //ProjectModel proj = response.Model as ProjectModel;

        //    this.ServiceResponse = projectService.PostModel(this.CurrentUser, projectModel);

        //    return Json(new { this.ServiceResponse }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ProjectExport(ProjectsModel model)
        {
            this.ServiceResponse = projectService.GetProjectExportModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                string saveAsFileName = "DPOProjectExport-" + model.ProjectExportType.ToString() + ".xls";

                Response.ClearContent();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                Response.Clear();
                Response.BinaryWrite(projectService.GetBytes(this.ServiceResponse.Model));
                Response.End();

                //this is use for unit test the Export button on ProjectButtonBar Partial View
                ViewData["exportProjectStatus"] = "export project success";

            }
            else {
                ViewData["exportProjectStatus"] = "export project failed";
            }

            return new EmptyResult();
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult ExportProject(ProjectExportParameter exportParam)
        {

            this.ServiceResponse = projectService.GetProjectExportExcelModel(this.CurrentUser, exportParam);

            if (this.ServiceResponse.IsOK)
            {
                string saveAsFileName = "DPOProjectExport-" + exportParam.ProjectExportType.ToString() + ".xls";

                Response.ClearContent();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                Response.Clear();
                Response.BinaryWrite(projectService.GetBytes(this.ServiceResponse.Model));
                Response.End();
            }

            return new EmptyResult();
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult ProjectQuotes(ProjectQuotesModel model)
        {

            this.ServiceResponse = projectService.GetProjectQuotesModel(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            this.RouteData.Values["action"] = "ProjectQuotes";

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("ProjectQuotes", this.ServiceResponse.Model) : View("ProjectQuotes", this.ServiceResponse.Model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult UpdateProjectsDeleteItem(ProjectsModel model, FormCollection collection)
        {
            if (model.DeleteProjects == null)
            {
                model.DeleteProjects = new List<ProjectModel>();
            }
            for (int i = 0; i < model.Items.Count; i++)
            {
                var item = collection[model.Items[i].ProjectId.ToString()];

                if (item != null)
                {
                    //items.Add((long)model.Items[i].ProjectId);

                    projectsDeleteId.Add((long)model.Items[i].ProjectId);
                }
            }

            if (model.DeleteProjects.Count == 0)
            {
                foreach (long id in projectsDeleteId)
                {
                    this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, (long)id);
                    model.DeleteProjects.Add((ProjectModel)this.ServiceResponse.Model);
                }

            }

            items = projectsDeleteId;

            //return AJAXRedirectTo("projects", "Projectdashboard", null);

            TempData["ProjectsModel"] = model;

            projectService.FinaliseModel(this.ServiceResponse.Messages, this.CurrentUser, model);

            return View("Projects", model);
            //return PartialView("ConfirmModalProjectsDelete", model);
        }

        public Dictionary<string, object> GetFormFilters(FormCollection collection)
        {
            Dictionary<string, object> FormFilters = new Dictionary<string, object>();

            if (collection["UserId"] != "")
            {
                FormFilters.Add("UserId", Convert.ToInt64(collection["UserId"]));
            }
            if (collection["BusinessId"] != "")
            {
                FormFilters.Add("BusinessId", Convert.ToInt64(collection["BusinessId"]));

            }
            if (collection["ProjectStatusTypeId"] != "")
            {
                FormFilters.Add("ProjectStatusTypeId", Convert.ToInt16(collection["ProjectStatusTypeId"]));
            }
            if (collection["ProjectOpenStatusTypeId"] != "")
            {
                FormFilters.Add("ProjectOpenStatusTypeId", Convert.ToInt16(collection["ProjectOpenStatusTypeId"]));
            }
            if (collection["ProjectLeadStatusTypeId"] != null && collection["ProjectLeadStatusTypeId"] != "")
            {
                FormFilters.Add("ProjectLeadStatusTypeId", (ProjectLeadStatusTypeEnum)Enum.Parse(typeof(ProjectLeadStatusTypeEnum), collection["ProjectLeadStatusTypeId"], true));
            }

            if (collection["DateTypeId"] != "")
            {
                FormFilters.Add("DateTypeId", Convert.ToInt16(collection["DateTypeId"]));
            }

            if (collection["ProjectStartDate"] != "")
            {
                FormFilters.Add("ProjectStartDate", Convert.ToDateTime(collection["ProjectStartDate"]));
            }

            if (collection["ProjectStartEnd"] != "")
            {
                FormFilters.Add("ProjectStartEnd", Convert.ToDateTime(collection["ProjectStartEnd"]));
            }

            if (collection["OnlyAlertProjects"] != "")
            {
                FormFilters.Add("OnlyAlertProjects", Convert.ToBoolean(collection["OnlyAlertProjects"]));
            }
            if (collection["ShowDeleteProjects"] != "")
            {
                FormFilters.Add("ShowDeleteProjects", Convert.ToBoolean(collection["ShowDeleteProjects"]));
            }

            if (collection["TotalRecords"] != null)
            {
                FormFilters.Add("TotalRecords", Convert.ToInt16(collection["TotalRecords"]));
            }

            return FormFilters;
        }

        //this controller action start using profiler for performance monitor.
        //add on by Aaron 11/14/2016
        [ValidateInput(false)]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult Projects(ProjectsModel model, FormCollection collection, ModelModeEnum? ModelMode)
        {
            Session.Abandon();

            var profiler = MiniProfiler.Current;

            if (Session["RequestAction"] != null)
            {
                Session["RequestAction"] = "";
            }
            if (Session["RequestController"] != null)
            {
                Session["RequestController"] = "";
            }
            if (Session["ProjectId"] != null)
            {
                Session["ProjectId"] = "";
            }
            if (Session["QuoteId"] != null)
            {
                Session["QuoteId"] = "";
            }
            if (Session["CommissionRequestid"] != null)
            {
                Session["CommissionRequestid"] = "";
            }


            Dictionary<string, object> FormFilters = GetFormFilters(collection);

            if (model.DeleteProjects != null)
            {
                model.DeleteProjects.Clear();
            }
            if (projectsDeleteId.Count > 0)
            {
                projectsDeleteId.Clear();
            }
            if (items.Count > 0)
            {
                items.Clear();
            }

            ModelState.DumpErrors();
            model.ReturnTotals = true;

            if (model.DeleteProjects == null)
            {
                model.DeleteProjects = new List<ProjectModel>();
            }

            for (int i = 0; i < model.Items.Count; i++)
            {
                var item = collection[model.Items[i].ProjectId.ToString()];

                if (item != null)
                {
                    projectsDeleteId.Add((long)model.Items[i].ProjectId);
                }
            }

            if (model.SortColumn == null) // Default opening parameters
            {
                model.ProjectStatusTypeId = (int)ProjectStatusTypeEnum.Open;
                model.SortColumn = "projectid";
            }

            if (model.ModelMode == ModelModeEnum.Save)
            {
                if (this.CurrentUser.HasAccess(SystemAccessEnum.EditProject))
                {
                    this.ServiceResponse = projectService.PostModel(this.CurrentUser, model);

                    ProcessServiceResponse(this.ServiceResponse);

                    using (profiler.Step("retrieve Projects"))
                    {

#pragma warning disable CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'
                        this.ServiceResponse = projectService.GetProjectsModel(this.CurrentUser, model);
#pragma warning restore CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'

                    }

                    if (FormFilters.ContainsKey("UserId"))
                    {
                        model.UserId = (long)FormFilters["UserId"];
                    }
                    if (FormFilters.ContainsKey("BusinessId"))
                    {
                        model.BusinessId = (long)FormFilters["BusinessId"];
                    }
                    if (FormFilters.ContainsKey("ProjectStatusTypeId"))
                    {
                        model.ProjectStatusTypeId = (Int16)FormFilters["ProjectStatusTypeId"];
                    }
                    if (FormFilters.ContainsKey("ProjectOpenStatusTypeId"))
                    {
                        model.ProjectOpenStatusTypeId = (Int16)FormFilters["ProjectOpenStatusTypeId"];
                    }
                    if (FormFilters.ContainsKey("ProjectLeadStatusTypeId"))
                    {
                        model.ProjectLeadStatusTypeId = (ProjectLeadStatusTypeEnum)FormFilters["ProjectLeadStatusTypeId"];
                    }
                    if (FormFilters.ContainsKey("ProjectDarComStatusTypeId"))
                    {
                        model.ProjectDarComStatusTypeId = (ProjectDarComStatusTypeEnum)FormFilters["ProjectDarComStatusTypeId"];
                    }
                    if (FormFilters.ContainsKey("DateTypeId"))
                    {
                        model.DateTypeId = (Int16)FormFilters["DateTypeId"];
                    }
                    if (FormFilters.ContainsKey("ProjectStartDate"))
                    {
                        model.ProjectStartDate = (DateTime)FormFilters["ProjectStartDate"];
                    }
                    if (FormFilters.ContainsKey("ProjectStartEnd"))
                    {
                        model.ProjectStartEnd = (DateTime)FormFilters["ProjectStartEnd"];
                    }
                    if (FormFilters.ContainsKey("OnlyAlertProject"))
                    {
                        model.OnlyAlertedProjects = (bool)FormFilters["OnlyAlertProject"];
                    }
                    if (FormFilters.ContainsKey("ShowDeleteProject"))
                    {
                        model.ShowDeletedProjects = (bool)FormFilters["ShowDeleteProject"];
                    }


                    model.ModelMode = ModelModeEnum.Edit;

                    return View("Projects", model);
                }

                model.ModelMode = ModelModeEnum.View;
            }

#pragma warning disable CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'
            this.ServiceResponse = projectService.GetProjectsModel(this.CurrentUser, model);
#pragma warning restore CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'

            if (ModelMode == ModelModeEnum.Edit)
            {
                model.ModelMode = ModelModeEnum.Edit;
                //return (IsPostRequest) ? (ViewResultBase)PartialView("Projects", model) : (ViewResultBase)View("Projects", model);
                ProjectsModel projectsModel = TempData["ProjectsModel"] as ProjectsModel;

                if (projectsModel != null)
                {
                    model.BusinessId = projectsModel.BusinessId;
                    model.UserId = projectsModel.UserId;
                    model.ProjectStatusTypeId = projectsModel.ProjectStatusTypeId;
                    model.ProjectOpenStatusTypeId = projectsModel.ProjectOpenStatusTypeId;
                    model.ProjectLeadStatusTypeId = projectsModel.ProjectLeadStatusTypeId;
                    model.DateTypeId = projectsModel.DateTypeId;
                    model.ProjectStartDate = projectsModel.ProjectStartDate;
                    model.ProjectStartEnd = projectsModel.ProjectStartEnd;
                    model.OnlyAlertedProjects = projectsModel.OnlyAlertedProjects;
                    model.ShowDeletedProjects = projectsModel.ShowDeletedProjects;
                    model.ProjectDarComTypes = projectsModel.ProjectDarComTypes;
                }

#pragma warning disable CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'
                this.ServiceResponse = projectService.GetProjectsModel(this.CurrentUser, model);
#pragma warning restore CS0618 // 'ProjectServices.GetProjectsModel(UserSessionModel, ProjectsModel)' is obsolete: 'Now using GetAllProjects'
                return View("Projects", this.ServiceResponse.Model);
            }

            TempData["ProjectsModel"] = model;

            return View("Projects", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.TransferProject })]
        public ActionResult ProjectTransfer(long projectId, string email)
        {
            this.ServiceResponse = this.projectService.ProjectTransfer(this.CurrentUser, projectId, email);

            if (ProcessServiceResponse())
            {
                var projectName = this.ServiceResponse.Model as string;

                var user = new AccountServices().GetUserSessionModel(email).Model as UserSessionModel;

                var emailModel = new SendEmailProjectTransfer { Email = email, ProjectName = projectName };

                emailModel.Subject = string.Format("A DPO project has been transferred to you.");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "DPO Registration");

                emailModel.To.Add(new MailAddress(email, user.DisplayName));

                emailModel.RenderTextVersion = true;
                emailModel.BodyTextVersion = RenderView(this, "SendEmailProjectTransfer", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailProjectTransfer", emailModel);

                new EmailServices().SendEmail(emailModel);
            }

            return RedirectToAction("Projects");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public EmptyResult ProjectTransferVerfiyEmail(string email)
        {
            this.ServiceResponse = projectService.VerifiyIsUser(email);

            if (this.ServiceResponse.HasError)
            {
                foreach (var message in this.ServiceResponse.Messages.Items)
                {
                    Response.Write(message.Text + "\n");
                }
            }
            else
            {
                Response.Write("OK");
            }

            return new EmptyResult();
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.TransferProject })]
        public JsonResult TransferProject(TransferProjectParameter transferProjectParameter)
        {
            var email = transferProjectParameter.Email;
            var projectId = transferProjectParameter.ProjectId;

            this.ServiceResponse = projectService.VerifiyIsUser(email);

            //return if email is invalid
            if (this.ServiceResponse.HasError)
            {
                return Json(this.ServiceResponse);
            }

            this.ServiceResponse = projectService.ProjectTransfer(this.CurrentUser, projectId, email);

            if (this.ServiceResponse.IsOK)
            {
                var projectName = this.ServiceResponse.Model as string;

                var user = new AccountServices().GetUserSessionModel(email).Model as UserSessionModel;

                var emailModel = new SendEmailProjectTransfer { Email = email, ProjectName = projectName };

                emailModel.Subject = string.Format("A DPO project has been transferred to you.");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "DPO Registration");
                emailModel.TransferFrom = this.CurrentUser.Email;

                if (string.IsNullOrEmpty(emailModel.ProjectName))
                {
                    if (!string.IsNullOrEmpty(projectId.ToString()) && projectId > 0)
                    {
                        emailModel.ProjectName = projectService.GetProjectNameById(projectId);
                    }
                }

                emailModel.To.Add(new MailAddress(email, user.DisplayName));
                //emailModel.To.Add(new MailAddress("huy.nguyen@daikincomfort.com", user.DisplayName));

                emailModel.RenderTextVersion = true;
                emailModel.BodyTextVersion = RenderView(this, "SendEmailProjectTransfer", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailProjectTransfer", emailModel);

                new EmailServices().SendEmail(emailModel);
            }

            return Json(this.ServiceResponse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectUndelete(ProjectModel model)
        {
            this.ServiceResponse = projectService.Undelete(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            //return AJAXRedirectTo("Projects", "Projectdashboard", null);
            return RedirectToAction("Project", "Projectdashboard", this.ServiceResponse.Model);
        }
        #endregion

        #region tools

        public ActionResult Tools()
        {
            var permissions = new PermissionServices().GetToolLinksForUser(this.CurrentUser);
            return View(permissions);
        }
        #endregion        
    }
}
