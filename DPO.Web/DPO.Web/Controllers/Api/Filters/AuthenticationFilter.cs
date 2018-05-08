using DPO.Domain;
using DPO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace DPO.Web.Controllers.Api.Filters
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public bool NoSecurityRequired { get; set; }

        public SystemAccessEnum Access { set { Accesses = new[] { value }; } get { return (Accesses == null || Accesses.Count() == 0) ? SystemAccessEnum.None : Accesses[0]; } }
        public SystemAccessEnum[] Accesses { get; set; }

        public UserTypeEnum UserTypeAllowed = UserTypeEnum.NotSet;
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var service = new AccountServices();
            var baseApiController = actionContext.ControllerContext.Controller as BaseApiController;

            if (NoSecurityRequired)
            {
                return;
            }

            if (actionContext.ControllerContext.Controller as ErrorController != null) return;

            var user = baseApiController.User;

            // Is user logged in ?
            if (!user.Identity.IsAuthenticated)
            {
                //filterContext.Result = BaseController.RedirectToLogin(null);

                BaseController.RedirectToLogin(null);

                //var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Request is Unauthorized"); ;

                return;
            }

            //TODO: To be completed

            //if (!user.Enabled)
            //{
            //    filterContext.Result = BaseController.RedirectToNotAuthorised();
            //    return;
            //}

            //if ((Accesses != null && Accesses.Count() > 0))
            //{
            //    if (user.HasAccess(this.Accesses) == false)
            //    {
            //        filterContext.Result = BaseController.RedirectToNotAuthorised();
            //        return;
            //    }
            //}
            //else
            //if (this.Access != SystemAccessEnum.None && !user.HasAccess(this.Access))
            //{
            //    filterContext.Result = BaseController.RedirectToNotAuthorised();
            //    return;
            //}

            //if (this.UserTypeAllowed != UserTypeEnum.NotSet && user.UserTypeId != this.UserTypeAllowed)
            //{
            //    filterContext.Result = BaseController.RedirectToNotAuthorised();
            //    return;
            //}

        }
    }
}