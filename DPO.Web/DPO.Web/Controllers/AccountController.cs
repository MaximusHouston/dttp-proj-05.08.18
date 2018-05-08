using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPO.Domain;
using System.Net.Mail;
using DPO.Common;
using DPO.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DPO.Web.Controllers
{
    [Authorise(NoSecurityRequired = true)]
    public class AccountController : BaseController
    {
        public AccountServices accountService = new AccountServices();

        public UserServices userService = new UserServices();

        private BusinessServices businessService = new BusinessServices();

        public HtmlServices htmlService = new HtmlServices();

        [HttpGet]
        public ActionResult Login()
        {
            UserLoginModel m = new UserLoginModel();

            m.Links = htmlService.DropDownModelLinks("LoginJump", m.SelectedLink);
            m.Links.Items.Insert(0, new SelectListItemExt()
            {
                Value = "/",
                Text = "Home",
                Selected = true
            });

            //var baseUrl = Request.UrlReferrer.AbsoluteUri;


            //if (baseUrl.Contains("/v2/"))
            //{
            //    return Redirect(baseUrl + "#/account/login");
            //}
            //else
            //{
            //    return Redirect(baseUrl + "v2/#/account/login");
            //}


            return View(m);
        }

        [Authorise(NoSecurityRequired = true)]
        public ActionResult Contact()
        {
            return View("Contact", "_ContactFormLayout", new SendEmailContactUsModel());
        }

        [Authorise(NoSecurityRequired = true)]
        public ActionResult ContactRequest()
        {
            this.ServiceResponse = accountService.GetContactForm();
            return View("Contact", "_ContactFormLayout", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactRequest(string name, string email, string subject, string message)
        {
            return Contact(name, email, subject, message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string name, string email, string subject, string message)
        {

            this.ServiceResponse = new ServiceResponse();

            var emailModel = new SendEmailContactUsModel
            {
                UserName = name,
                UserEmail = email,
                Subject = subject,
                Message = message
            };

            if (string.IsNullOrWhiteSpace(name))
            {
                this.ServiceResponse.AddError("UserName", "Please enter your name.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                this.ServiceResponse.AddError("UserEmail", "Please enter your email address.");
            }

            var emailTest = Validation.IsEmail(email, "Email address", 255, true);
            if (emailTest != null)
            {
                this.ServiceResponse.AddError("UserEmail", emailTest);
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                this.ServiceResponse.AddError("Subject", "Please select a subject.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                this.ServiceResponse.AddError("Message", "Please enter a message");
            }


            if (message.Length > 300000)
            {
                this.ServiceResponse.AddError("Message", "Message too long");
            }

            if (ProcessServiceResponse())
            {

                emailModel.Subject = string.Format("A contact request received");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "DPO System");

                emailModel.To.Add(new MailAddress(Utilities.Config("dpo.sys.email.from"), "Help Desk"));

                emailModel.RenderTextVersion = true;
                emailModel.BodyTextVersion = RenderView(this, "SendEmailContactRequest", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailContactRequest", emailModel);

                new EmailServices().SendEmail(emailModel);

                return PartialView("RegistrationAcknowledgement", "Account");
            }

            return View("Contact", emailModel);
        }

        [HttpPost]
        public ActionResult Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                string mockEmail = null;
                if (model.Email.IndexOf(":") > 0)
                {
                    mockEmail = model.Email.Split(':')[1];
                    model.Email = model.Email.Split(':')[0];
                }

                var response = accountService.Login(model);

                if (ProcessServiceResponse(response))
                {
                    model = response.Model as UserLoginModel;

                    FormsAuthentication.SetAuthCookie(mockEmail ?? model.Email, model.Persistent);

                    var sessionModelResp = accountService.GetUserSessionModel(mockEmail ?? model.Email);
                    if (sessionModelResp.IsOK)
                    {
                        var user = sessionModelResp.Model as UserSessionModel;

                        var requestUrl = HttpContext.Request.QueryString["ReturnUrl"];

                        if (requestUrl != null)
                        {
                            if (requestUrl.Contains("TradeShow"))
                            {
                                model.SelectedLink = requestUrl;
                            }

                        }

                        var defaultUrlResp = accountService.GetDefaultPageUrl(user, model.SelectedLink);

                        return base.Redirect(defaultUrlResp.Model.ToString());
                    }
                }
            }

            model.Links = htmlService.DropDownModelLinks("LoginJump", model.SelectedLink);
            model.Links.Items.Insert(0, new SelectListItemExt()
            {
                Value = String.Empty,
                Text = "Home",
                Selected = true
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult InternalLogin(string securityKey)
        {
            var parts = Crypto.Decrypt(securityKey.Replace(" ", "+")).Split('#');

            if (DateTime.Parse(parts[1]) > DateTime.Now.AddMinutes(-1))
            {
                FormsAuthentication.SetAuthCookie(parts[0], true);

                return new ContentResult { Content = "OK" };
            }

            return new ContentResult { Content = "Failed" };
        }


        public RedirectToRouteResult Logoff()
        {
            accountService.Logoff(this.CurrentUser, this.HttpContext);

            if (FormsAuthentication.IsEnabled)
            {
                FormsAuthentication.SignOut();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult NotAuthorised()
        {
            return View();
        }

        #region New Password

        [HttpGet]
        public ActionResult RequestNewPassword(String UserEmail)
        {
            var model = new UserResetPasswordModel
            {
                Email = UserEmail
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestNewPassword(UserResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var response = accountService.RequestNewPassword(model);

                if (response.IsOK)
                {
                    ViewData["CurrentUser"] = this.CurrentUser ?? new UserSessionModel();

                    var emailModel = response.Model as SendEmailPasswordResetModel;

                    emailModel.RenderTextVersion = true;
                    emailModel.BodyTextVersion = RenderView(this, "SendEmailPasswordReset", emailModel);

                    emailModel.RenderTextVersion = false;
                    emailModel.BodyHtmlVersion = RenderView(this, "SendEmailPasswordReset", emailModel);

                    new EmailServices().SendEmail(emailModel);

                }

                if (ProcessServiceResponse(response))
                {
                    return View("RequestNewPasswordAcknowledgement");
                }

            }

            return View(model);
        }
        [HttpGet]
        public ActionResult RequestNewPasswordAcknowledgement()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ResettingPasswordFromSecurityLink(string securityKey)
        {
            if (ModelState.IsValid)
            {

                var response = accountService.ValidateSecurityKey(new UserResetPasswordModel { SecurityKey = securityKey });

                if (!ProcessServiceResponse(response))
                {
                    response.AddError("", ResourceModelUser.MU009);
                }

            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResettingPasswordFromSecurityLink(UserResetPasswordModel model)
        {

            if (ModelState.IsValid)
            {
                var response = accountService.ValidateSecurityKey(model);

                if (ProcessServiceResponse(response))
                {
                    response = accountService.ResetPassword(model);

                    if (ProcessServiceResponse(response))
                    {
                        FormsAuthentication.SignOut();

                        return View("PasswordResetAcknowledgement");
                    }
                }

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult PasswordResetAcknowledgement()
        {
            return View();
        }

        #endregion

        #region User Registration

        [HttpGet]
        public ActionResult UserRegistration()
        {
            var response = new UserServices().GetUserModelForRegistering();

            ProcessServiceResponse(response);

            return View("UserRegistration", response.Model);
        }

        /// <summary>
        /// Searches for a business by exact name, state, postal code,
        /// and country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private BusinessModel SearchForBusiness(UserModel model)
        {
            // If no business or business name for lookup, return null
            if (model.Business == null
                || string.IsNullOrEmpty(model.Business.BusinessName))
            {
                return null;
            }

            // If there is an ID, we will use that for lookup later
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegistration(UserModel model)
        {
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

            this.ServiceResponse = userService.PostModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as UserModel;

            //this.ServiceResponse = userService.GetUserModel(this.CurrentUser, model);
            //model = this.ServiceResponse.Model as UserModel;

            if (ProcessServiceResponse(this.ServiceResponse))
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

                        emailModel.BodyTextVersion = RenderView(this, "SendEmailUserRegistration", emailModel);

                        emailModel.RenderTextVersion = false;
                        emailModel.BodyHtmlVersion = RenderView(this, "SendEmailUserRegistration", emailModel);

                        var emailservice = new EmailServices();
                        emailservice.SendEmail(emailModel);
                    }
                    catch (Exception e)
                    {
                        Utilities.ErrorLog(e);
                    }
                }
                return PartialView("RegistrationAcknowledgement", "Account");
            }

            var userModel = this.ServiceResponse.Model as UserModel;

            return PartialView("UserRegistration", this.ServiceResponse.Model);
        }



        [HttpPost]
        public JsonResult RegisterUser(UserModel model)
        {
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

            //return null;

            this.ServiceResponse = userService.PostModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as UserModel;

            if (this.ServiceResponse.IsOK)
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

                        emailModel.BodyTextVersion = RenderView(this, "SendEmailUserRegistration", emailModel);

                        emailModel.RenderTextVersion = false;
                        emailModel.BodyHtmlVersion = RenderView(this, "SendEmailUserRegistration", emailModel);

                        var emailservice = new EmailServices();
                        emailservice.SendEmail(emailModel);
                    }
                    catch (Exception e)
                    {
                        Utilities.ErrorLog(e);
                    }
                }

                return Json(this.ServiceResponse);
            }

            return Json(this.ServiceResponse);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UserRegistrationGetBusinessDetails(string accountId)
        //{
        //    var superUserModel = new AccountServices().GetSuperUserSessionModel().Model as UserSessionModel;

        //    this.ServiceResponse = new BusinessServices().GetBusinessModelByAccountId(superUserModel, accountId);

        //    ((BusinessModel)this.ServiceResponse.Model).IsRegistering = true;

        //    return PartialView("_UserBusinessDetailsPartial", this.ServiceResponse.Model);
        //}

        #endregion

        #region user account details
        [HttpGet]
        public ActionResult AccountDetailsEdit()
        {

            this.ServiceResponse = userService.GetUserModel(this.CurrentUser, this.CurrentUser.UserId, false);
            ProcessServiceResponse(this.ServiceResponse);

            var userModel = this.ServiceResponse.Model as UserModel;

            return View("AccountDetailsEdit", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountDetailsEdit(UserModel model)
        {
            if (model.UserId != CurrentUser.UserId)
            {
                return RedirectToNotAuthorised();
            }

            this.ServiceResponse = userService.PostModel(this.CurrentUser, model);

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                return AjaxRedirectToReferrer();
            }

            return PartialView("AccountDetailsEdit", this.ServiceResponse.Model);
        }

        public ActionResult IsUserLoggedIn()
        {
            Response.Write((this.Request.IsAuthenticated && this.CurrentUser != null) ? "1" : "0");
            return new EmptyResult();
        }


        public ActionResult DaikinCityAccess()
        {
            if (this.Request.IsAjaxRequest() && this.IsPostRequest)
            {
                if (this.Request.IsAuthenticated && this.CurrentUser != null)
                {
                    var permissions = this.CurrentUser.CityAccesses.Select(i => new { id = i }).OrderBy(i => i.id).ToList();

                    return Json(permissions);
                }
                else
                {
                    var noLoginList = (userService.GetCityAreasForNonLoggedOnUsers().Model as List<int>).Select(i => new { id = i }).OrderBy(i => i.id).ToList();

                    return Json(noLoginList);
                }
            }

            return new EmptyResult();
        }
        #endregion



    }
}
