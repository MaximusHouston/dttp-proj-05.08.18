using DPO.Common;
using DPO.Data;
using DPO.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace DPO.Domain
{
    public class AccountServices : BaseServices
    {
        public const string SESSION_KEY_USER = "UserSession";

        public AccountServices()
            : base()
        {
        }

        public AccountServices(DPOContext context)
            : base(context)
        {
        }

        //public ServiceResponse ResetBasketQuoteId() {
        //    var session = HttpContext.Current.Session;
        //    session["BasketQuoteId"] = null;
        //    CurrentUser.BasketQuoteId = null;
        //    return this.Response;
        //}

        public bool CanAccessQuote(string email, long quoteid)
        {
            var user = GetUserSessionModel(email).Model as UserSessionModel;
            return Db.QueryQuoteViewableByQuoteId(user, quoteid).Any();
        }

        public ServiceResponse GetContactForm()
        {
            var model = new SendEmailContactUsModel();
            var options = this.Db.Context.ContactUsFormOptions.FirstOrDefault();

            if (options != null)
            {
                model.ContactUsAddress = options.Address;
                model.ContactUsTel = options.Telephone;

                List<string> subjects = this.Db.Context.ContactUsFormSubjects.Select(x => x.SubjectName).ToList();

                if (subjects != null)
                {
                    model.ContactUsSubjects = subjects;
                }
            }

            this.Response.Model = model;
            return this.Response;
        }

        public ServiceResponse GetDefaultPageUrl(UserSessionModel user, string selectedLink = "")
        {
            this.Response = new ServiceResponse();

            //string url = "/Projectdashboard/Overview";
            string url = "/v2/#/home";

            if (selectedLink == null)
            {
                selectedLink = String.Empty;
            }

            if (user == null || !user.Enabled || user.UserId == 0)
            {
                url = "/Account/Login";
            }
            //else if (!user.HasAccess(SystemAccessEnum.ViewProject)
            //    && (selectedLink.ToLower().Contains("projectdashboard/projects")
            //        || selectedLink.ToLower().Contains("projectdashboard/overview")
            //        || String.IsNullOrEmpty(selectedLink)))
            //{
            //    url = "/ProjectDashboard/Tools";
            //}
            else if (!user.HasAccess(SystemAccessEnum.ViewProject)
                && (selectedLink.ToLower().Contains("/v2/#/projects")
                    || selectedLink.ToLower().Contains("projectdashboard/overview")
                    || String.IsNullOrEmpty(selectedLink)))
            {
                url = "/ProjectDashboard/Tools";
            }
            else if (!String.IsNullOrWhiteSpace(selectedLink))
            {
                url = selectedLink;
            }

            this.Response.Model = url;

            return this.Response;
        }

        public ServiceResponse GetSuperUserSessionModel()
        {
            this.Response = new ServiceResponse();

            var query = from user in Db.GetSuperUser() select user;

            return GetUserSessionModel(query);
        }

        public ServiceResponse GetUserSessionModel(long userid)
        {
            this.Response = new ServiceResponse();

            var query = from user in Db.UserQueryByUserId(userid).AsNoTracking() select user;

            return GetUserSessionModel(query);
        }

        public ServiceResponse GetUserSessionModel(string email)
        {
            this.Response = new ServiceResponse();

            var query = from user in Db.UserQueryByEmail(email).AsNoTracking() select user;
            var sess = GetUserSessionModel(query);

            if (sess.Model != null)
            {
                var sessModel = sess.Model as UserSessionModel;
                var defaultUrlResp = this.GetDefaultPageUrl(sessModel);
                sessModel.DefaultPageUrl = defaultUrlResp.Model.ToString();
            }

            return sess;
        }

        public UserSessionModel LoadUserSessionModel()
        {
            var context = HttpContext.Current;

            var curSess = context.Session[SESSION_KEY_USER] as UserSessionModel;

            if (curSess == null || String.IsNullOrEmpty(curSess.Email))
            {
                var email = context.User.Identity.Name;
                var user = GetUserSessionModel(email).Model as UserSessionModel;

                context.Session[SESSION_KEY_USER] = user ?? new UserSessionModel();
            }

            return context.Session[SESSION_KEY_USER] as UserSessionModel;
        }

        public ServiceResponse Login(UserLoginModel model)
        {
            this.Response = new ServiceResponse();

            this.Context.ReadOnly = false;

            var user = Db.UserQueryByEmail(model.Email).Include(u => u.Business).FirstOrDefault();

            if (user == null)
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU004);
                return this.Response;
            }

            // Check password
            var pwdHash = Crypto.Hash(model.Password, user.Salt);

            if (string.Compare(pwdHash, user.Password) != 0)
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU004);
                return this.Response;
            }

            if (!user.Approved)
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU006);
                return this.Response;
            }

            if (!user.Enabled)
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU005);
                return this.Response;
            }

            if (!user.Business.Enabled)
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU022);
                return this.Response;
            }

            user.LastLoginOn = DateTime.UtcNow;

            Db.SaveChanges();

            this.Response.Model = model;

            return this.Response;
        }

        //    if (currentUser != null && lastUpdate != currentUser.Timestamp)
        //    {
        //        currentUser = LoadUserSessionModel();
        //    }
        //}
        public ServiceResponse Logoff(UserSessionModel model, HttpContextBase context)
        {
            this.Response = new ServiceResponse();

            if (context != null)
            {
                context.Session[SESSION_KEY_USER] = null;
            }

            return this.Response;
        }

        //    var currentUser = context.Session["UserSessionModel"] as UserSessionModel;
        public ServiceResponse RequestNewPassword(UserResetPasswordModel model)
        {
            this.Response = new ServiceResponse();

            var emailUser = GetUserSessionModel(model.Email).Model as UserSessionModel;

            if (emailUser == null)
            {
                this.Response.AddError("Email", ResourceModelUser.MU007);
            }
            else
            {
                var emailModel = new SendEmailPasswordResetModel();

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "Daikin Office Project");

                emailModel.To.Add(new MailAddress(emailUser.Email, emailUser.DisplayName));

                model.GenerateSecurityKey();

                emailModel.SecurityKey = model.SecurityKey;

                emailModel.Subject = "Daikin Password Reset Request";

                this.Response.Model = emailModel;
            }
            return this.Response;
        }

        public ServiceResponse ResetPassword(UserResetPasswordModel model)
        {
            this.Response = new ServiceResponse();

            if (model.DecryptSecurityKey() == false)
            {
                this.Response.AddError(ResourceModelUser.MU009);
                return this.Response;
            }

            Validation.IsPasswordConfirmed(this.Response.Messages, model.NewPassword, model.ConfirmPassword, "ConfirmPassword");

            this.Db.ReadOnly = false;

            var user = this.Db.UserQueryByEmail(model.Email).FirstOrDefault();

            user.Password = Crypto.Hash(model.NewPassword, user.Salt);

            this.Db.SaveChanges();

            this.Response.Model = model;

            return this.Response;
        }

        public void SaveContactForm(SendEmailContactUsModel model)
        {
            var options = this.Db.Context.ContactUsFormOptions.FirstOrDefault();

            if (options == null) options = new ContactUsFormOption();

            options.Address = model.ContactUsAddress;
            options.Telephone = model.ContactUsTel;

            var subjects = this.Db.Context.ContactUsFormSubjects.DefaultIfEmpty();

            if (subjects != null) this.Db.Context.ContactUsFormSubjects.RemoveRange(subjects);

            foreach (var newsubject in model.ContactUsSubjects)
            {
                if (!String.IsNullOrEmpty(newsubject))
                {
                    ContactUsFormSubject entity = new ContactUsFormSubject
                    {
                        SubjectName = newsubject
                    };

                    this.Db.Context.ContactUsFormSubjects.Add(entity);
                }
            }

            this.Db.SaveChanges();
        }

        //    var lastUpdate = Db.UserQueryByEmail(context.User.Identity.Name).Select(u => u.Timestamp).FirstOrDefault();
        public ServiceResponse ValidateSecurityKey(UserResetPasswordModel model)
        {
            this.Response = new ServiceResponse();

            if (model.DecryptSecurityKey() == false)
            {
                this.Response.AddError(ResourceModelUser.MU009);
                return this.Response;
            }

            if (new TimeSpan(DateTime.UtcNow.Ticks - model.SecurityTicks).TotalMinutes > 30)
            {
                this.Response.AddError(ResourceModelUser.MU009);
                return this.Response;
            }

            this.Response.Model = model;

            return this.Response;
        }

        private ServiceResponse GetUserSessionModel(IQueryable<User> userQuery)
        {
            this.Response = new ServiceResponse();

            var query = from u in userQuery
                        join b in Db.Businesses on u.BusinessId equals b.BusinessId
                        select new UserSessionModel
                        {
                            UserId = u.UserId,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            MiddleName = u.MiddleName,
                            LastName = u.LastName,
                            UserTypeId = u.UserTypeId,
                            GroupId = u.GroupId,
                            IsGroupOwner = u.IsGroupOwner ?? false,
                            BusinessId = b.BusinessId,
                            BusinessName = b.BusinessName,
                            BusinessTypeId = b.BusinessTypeId,
                            ShowPrices = b.ShowPricing,
                            CommissionSchemeAllowed = b.CommissionSchemeAllowed,
                            Enabled = u.Enabled,
                            DisplaySettings = u.DisplaySettings,
                            Timestamp = u.Timestamp
                        };

            var currentuser = query.FirstOrDefault();

            if (currentuser != null)
            {
                currentuser.SystemAccesses = Db.GetSystemAccesses(userQuery).Cache().Select(p => (SystemAccessEnum)p.ReferenceId).ToList();

                currentuser.CityAccesses = Db.GetUserCityAccess(currentuser.UserId).Cache().Select(p => (int)p.ReferenceId).ToList();

                currentuser.ToolAccesses = Db.GetUserToolAccess(currentuser.UserId).Cache().ToList();

                currentuser.ProductFamilyAccesses = Db.GetProductFamilyAccess(currentuser.UserId).Cache().ToList();

                currentuser.BrandAccesses = Db.GetBrandAccess(currentuser.UserId).Cache().ToList();

                if (HttpContext.Current != null)
                {
                    var logodirectory = HttpContext.Current.Server.MapPath("/Images/BusinessLogos/");

                    if (logodirectory != null)
                    {
                        if (!Directory.Exists(logodirectory))
                        {
                            Directory.CreateDirectory(logodirectory);
                        }

                        var logo = Directory.GetFiles(logodirectory, currentuser.BusinessId.ToString() + ".*").FirstOrDefault();

                        if (logo != null)
                        {
                            currentuser.BusinessLogoUrl = "/Images/BusinessLogos/" + Path.GetFileName(logo);
                        }
                    }
                }

                currentuser.DisplaySettingsPageSize = 10;

                if (!string.IsNullOrWhiteSpace(currentuser.DisplaySettings))
                {
                    var pairs = currentuser.DisplaySettings.Split(',');
                    foreach (var pair in pairs)
                    {
                        var namevalue = pair.Split('=');
                        switch (namevalue[0])
                        {
                            case "pagesize": currentuser.DisplaySettingsPageSize = int.Parse(namevalue[1]);
                                break;
                        }
                    }
                }

                this.Response.Model = currentuser;
            }

            return this.Response;
        }

     
    }
}