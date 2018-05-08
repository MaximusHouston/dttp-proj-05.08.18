using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DPO.Web.Controllers
{
    public class UserActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var service = new AccountServices();


            var baseApiController = actionContext.ControllerContext.Controller as BaseApiController;

            if (baseApiController != null)
            {
                baseApiController.CurrentUser = service.LoadUserSessionModel();

                var session = HttpContext.Current.Session;

                baseApiController.CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? (long?)0;
            }


        }
    }
}