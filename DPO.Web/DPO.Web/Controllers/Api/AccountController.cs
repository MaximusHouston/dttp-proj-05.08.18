using DPO.Common;
using DPO.Domain;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;
using System.Web.Security;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;

namespace DPO.Web.Controllers
{
    //[UserActionFilter]
    public class AccountApiController : BaseApiController
    {
        UserServiceLight userServiceLight = new UserServiceLight();
        UserServices userService = new UserServices();
        AccountServices accountService = new AccountServices();
        BusinessServices businessService = new BusinessServices();
        HtmlServices htmlService = new HtmlServices();

        BaseController MvcAccountController = new DPO.Web.Controllers.AccountController();

        [HttpGet]
        public ServiceResponse GetUserLoginModel()
        {
            ServiceResponse response = new ServiceResponse();

            UserLoginModel model = new UserLoginModel();

            model.Links = htmlService.DropDownModelLinks("LoginJump", model.SelectedLink);

            //model.Links.Items.Insert(0, new SelectListItemExt()
            //{
            //    Value = "/v2/#/home",
            //    Text = "Home",
            //    Selected = true
            //});

            response.Model = model;
            return response;
        }

        [HttpPost]
        public ServiceResponse LogIn(UserLoginModel model)
        {
            ServiceResponse response = new ServiceResponse();

            string mockEmail = null;
            if (model.Email.IndexOf(":") > 0)
            {
                mockEmail = model.Email.Split(':')[1];
                model.Email = model.Email.Split(':')[0];
            }

            response = accountService.Login(model);

            if (response.IsOK)
            {
                model = response.Model as UserLoginModel;

                ServiceResponse sessionModelResp = new ServiceResponse();

                if (model.Email == "daikincity@daikincomfort.com")
                {
                    FormsAuthentication.SetAuthCookie(mockEmail ?? model.Email, model.Persistent);

                    sessionModelResp = accountService.GetUserSessionModel(mockEmail ?? model.Email);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.Persistent);

                    sessionModelResp = accountService.GetUserSessionModel(model.Email);
                }


                if (sessionModelResp.IsOK)
                {
                    var user = sessionModelResp.Model as UserSessionModel;

                    //var requestUrl = HttpContext.Request.QueryString["ReturnUrl"]; // TODO: Check if we need to do this?
                    //if (requestUrl != null)
                    //{
                    //    if (requestUrl.Contains("TradeShow"))
                    //    {
                    //        model.SelectedLink = requestUrl;
                    //    }
                    //}

                    ServiceResponse defaultUrlResp = accountService.GetDefaultPageUrl(user, model.SelectedLink);

                    response.Model = defaultUrlResp.Model.ToString();

                }
            }

            return response;


        }

        [HttpGet]
        public ServiceResponse UserRegistration()
        {
            ServiceResponse response = userService.GetUserModelForRegistering();
            return response;
        }

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public ServiceResponse BusinessAddressLookup(string accountId)
        {
            ServiceResponse response = businessService.GetBusinessModelByAccountId(this.CurrentUser, accountId);
            return response;
        }


        //Not used - can not render MVC View
        [HttpPost]
        public ServiceResponse UserRegistration(UserModel model)
        {
            ServiceResponse response = new ServiceResponse();

            model.IsRegistering = true;
            // Lookup account by business name, state, postal code
            BusinessModel identifiedBusiness = SearchForBusiness(model);

            // In order to prevent duplicates we will lookup the business by name, state, postal code
            // and if it exists return back the business and send an email to the super user that a user
            // wants to register
            if (identifiedBusiness != null)
            {
                model.Business = identifiedBusiness;
                model.ExistingBusiness = ExistingBusinessEnum.Existing;
            }

            response = userService.PostModel(this.CurrentUser, model);

            model = response.Model as UserModel;

            if (response.IsOK)
            {
                if (model.IsRegistering)
                {
                    businessService.SetupDefaultPermission(this.CurrentUser, model.Business);
                    userService.SetupDefaultPermissions(this.CurrentUser, model);

                    try
                    {
                        var emailModel = userService.GetUserRegistrationEmailModel(model).Model as SendEmailModel;

                        emailModel.Subject = "New Project Office Registration";
                        emailModel.RenderTextVersion = true;

                        //emailModel.BodyTextVersion = RenderView(this, "SendEmailUserRegistration", emailModel);

                        //emailModel.BodyTextVersion = MvcAccountController.RenderView(MvcAccountController, "SendEmailUserRegistration", emailModel);

                        //GET
                        var serializedModel = JsonConvert.SerializeObject(emailModel);
                        var client = new HttpClient();
                        var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                        var result = client.GetAsync(baseUrl + "/ViewRender/Render?ViewName=UserRegistrationEmailTemplate&SerializedModel=" + serializedModel).Result;
                        //var result = client.GetAsync(baseUrl + "/Account/Render?ViewName=SendEmailUserRegistration&SerializedModel=" + serializedModel).Result;


                        //POST
                        //ViewRenderModel viewRenderModel = new ViewRenderModel
                        //{
                        //    ViewName = "SendEmailUserRegistration",
                        //    ViewModel = emailModel
                        //};
                        //var result = client.PostAsJsonAsync(baseUrl + "/ViewRender/Render", viewRenderModel).Result;

                        emailModel.RenderTextVersion = false;
                        //emailModel.BodyHtmlVersion = RenderView(this, "SendEmailUserRegistration", emailModel);
                        var emailservice = new EmailServices();
                        emailservice.SendEmail(emailModel);
                    }
                    catch (Exception e)
                    {
                        Utilities.ErrorLog(e);
                    }
                }
                //return PartialView("RegistrationAcknowledgement", "Account");
            }

            //var userModel = response.Model as UserModel;

            //return PartialView("UserRegistration", this.ServiceResponse.Model);
            return response;
        }


        private BusinessModel SearchForBusiness(UserModel model)
        {
            // If no business or business name for lookup, return null
            if (model.Business == null
                || string.IsNullOrEmpty(model.Business.BusinessName))
            {
                return null;
            }

            //If there is an ID, we will use that for lookup later
            if (!string.IsNullOrEmpty(model.Business.AccountId)
                && !string.IsNullOrEmpty(model.Business.DaikinCityId))
            {
                return null;
            }


            SearchBusiness busSearch = new SearchBusiness
            {
                ExactBusinessName = model.Business.BusinessName,
                ReturnTotals = true
            };

            if (model.Address != null)
            {
                var addr = model.Address;

                busSearch.CountryCode = addr.CountryCode;
                busSearch.StateId = addr.StateId;
                busSearch.PostalCode = addr.PostalCode;
            }

            // Returns all businesses that match without permissions applied
            var bus = businessService.GetBusinessModel(null, busSearch);

            if (bus != null
                && bus.IsOK
                && bus.Model != null)
            {
                // TODO:  Send request to super user
                return bus.Model as BusinessModel;
            }

            return null;
        }

        public ServiceResponse GetDistributorsAndReps(string businessName)
        {
            return businessService.GetDistributorsAndReps(this.CurrentUser, businessName);
        }

        [HttpGet]
        public ServiceResponse ResetBasketQuoteId()
        {
            var response = new ServiceResponse();
            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = null;
            CurrentUser.BasketQuoteId = null;

            return response;
        }



    }
}