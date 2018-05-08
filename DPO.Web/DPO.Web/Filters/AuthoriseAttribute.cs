using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO.Compression;
using System.Web.Routing;
using DPO.Domain;
using DPO.Common;
using DPO.Web.Controllers;


namespace DPO.Web
{
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
   public class AuthoriseAttribute : ActionFilterAttribute
   {
      public bool NoSecurityRequired { get; set; }

      public SystemAccessEnum Access { set { Accesses = new[] { value }; } get { return (Accesses == null || Accesses.Count() == 0) ? SystemAccessEnum.None : Accesses[0]; } }
      public SystemAccessEnum[] Accesses { get; set; }

      public UserTypeEnum UserTypeAllowed = UserTypeEnum.NotSet;

      public override void OnActionExecuting(ActionExecutingContext filterContext)
      {
          // is document server? then no login info needed
          //if (Utilities.IsDocumentServer())
          //{
          //    NoSecurityRequired = true;
          //}

         if (NoSecurityRequired)
         {
            return;
         }

         if (filterContext.Controller as ErrorController != null) return;

         var user = ((BaseController)filterContext.Controller).CurrentUser;

         // Is user logged in ?
         if (!filterContext.HttpContext.User.Identity.IsAuthenticated || user.Email == null)
         {
            filterContext.Result = BaseController.RedirectToLogin(null);

            return;
         }

         if (!user.Enabled)
         {
             filterContext.Result = BaseController.RedirectToNotAuthorised();
            return;
         }

         if ((Accesses != null && Accesses.Count() > 0))
         {
             if (user.HasAccess(this.Accesses) == false)
             {
                 filterContext.Result = BaseController.RedirectToNotAuthorised();
                 return;
             }
         }
         else
         if (this.Access != SystemAccessEnum.None && !user.HasAccess(this.Access))
         {
             filterContext.Result = BaseController.RedirectToNotAuthorised();
             return;
         }

         if (this.UserTypeAllowed != UserTypeEnum.NotSet && user.UserTypeId != this.UserTypeAllowed)
         {
             filterContext.Result = BaseController.RedirectToNotAuthorised();
             return;
         }

      }


   }



}
