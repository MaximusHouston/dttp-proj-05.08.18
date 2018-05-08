using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using DPO.Common;
using System.IO;
using System.Web.Routing;
using System.Diagnostics;
using log4net;

namespace DPO.Web.Controllers
{
    [Authorise]
    public class BaseController : Controller
    {
       long start = DateTime.Now.Ticks;
       string controllerName;
       string actionName;

       public ServiceResponse ServiceResponse = null;
       public ILog Log = log4net.LogManager.GetLogger(typeof(BaseController));

       public BaseController() : base()
       {
       }

       public UserSessionModel CurrentUser { get; set; }

       public bool IsPostRequest { get { return (Request.HttpMethod.ToLower() == "post"); } }

       public ActionResult AjaxRedirectToReferrer()
       {
           return Content("REDIRECT_TO_REFERRER:");
       }

       public ActionResult AjaxReloadPage()
       {
           return Content("RELOAD:");
       }

       public ActionResult AJAXRedirectTo(string actionName, string controllerName,object routeValues)
       {
            if (routeValues != null)
            {
                return Content("REDIRECT_TO:" + Url.Action(actionName, controllerName, routeValues));
            }
            return View(actionName);
            
       }

       protected override void OnActionExecuting(ActionExecutingContext filterContext)
       {
           var service = new AccountServices();

           base.OnActionExecuting(filterContext);

           controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

           actionName = filterContext.ActionDescriptor.ActionName;

           var httpContext = this.HttpContext;

           this.CurrentUser = service.LoadUserSessionModel();

           this.CurrentUser.BasketQuoteId = (long?)this.Session["BasketQuoteId"] ?? 0;

           //var pagemessages = this.TempData["PageMessages"] as Messages;

           if (actionName != "Basket")
           {
               this.ViewData["PageMessages"] = new Messages();

               var keymessages = this.TempData["KeyMessages"] as Messages;

                if(keymessages != null && keymessages.Items.Count > 0)
                {
                    this.ViewData["KeyMessages"] = keymessages;
                }
                else
                {
                    this.ViewData["KeyMessages"] = new Message();
                }
               
           }
       }

       protected override void OnResultExecuting(ResultExecutingContext filterContext)
       {
           ViewData["CurrentUser"] = this.CurrentUser ?? new UserSessionModel();

          // Apply current user information
          var pageModel = filterContext.Controller.ViewData.Model as PageModel;

          if (pageModel != null)
          {
              pageModel.CurrentUser = ViewData["CurrentUser"] as UserSessionModel;
          }

          if (actionName != "Basket")
          {
              var pagemessages = ViewData["PageMessages"] as Messages;

              var keymessages = ViewData["KeyMessages"] as Messages;

              var messagesToTempSave = new Messages();

              // save current saved messages for next request
              messagesToTempSave.Add(pagemessages);

              // add previous saved messages to current messages
              AddMessagesToResponse(this.TempData["SavedMessages"] as Messages);

              this.TempData["SavedMessages"] = messagesToTempSave;

            }

          base.OnResultExecuting(filterContext);

          Debug.WriteLine(string.Format("Time taken ms: {0}. Controller/Action={1}/{2}", new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds, controllerName, actionName));

       }

        protected bool ProcessServiceResponse()
        { 
            return ProcessServiceResponse(this.ServiceResponse);
        }

        public bool ShowKeyMessagesOnPage { get; set; }

        protected bool ProcessServiceResponse(ServiceResponse response)
        {
            // Clear default model errors
            foreach (var modelValue in ModelState.Values)
            {
                modelValue.Errors.Clear();
            }

            AddMessagesToResponse(response.Messages);

           return response.IsOK;
        }

        protected void AddMessagesToResponse( Messages messages)
        {
            if (messages == null) return;

            var pagemessages = ViewData["PageMessages"] as Messages;
            var keymessages  = ViewData["KeyMessages"] as Messages;

            messages.Items
            .Where(m => m.Type != MessageTypeEnum.Audit).ToList()
            .ForEach(i =>
            {
                if (!pagemessages.Items.Any(m => m.Key == i.Key && m.Text == i.Text)) // Prevent duplicates
                {
                    // Show only errors messages if an error and only information if no errors
                    if ((messages.HasErrors && (i.Type == MessageTypeEnum.Error ||
                                                      i.Type == MessageTypeEnum.Warning ||
                                                      i.Type == MessageTypeEnum.Critial))
                         ||
                         (!messages.HasErrors && (i.Type == MessageTypeEnum.Information ||
                                                      i.Type == MessageTypeEnum.Success)
                        )) 
                    {
                        if (string.IsNullOrEmpty(i.Key) || ShowKeyMessagesOnPage)
                        {
                            
                            pagemessages.Add(i);
                        }
                        else
                        {
                            if(keymessages == null)
                            {
                                keymessages = new Messages();
                            }
                            
                            keymessages.Add(i);
                            ModelState.AddModelError(i.Key, i.Text);
                        }
                    }
                }
                messages.Items.Remove(i);
            });
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
           if (Url.IsLocalUrl(returnUrl))
           {
              return Redirect(returnUrl);
           }
           else
           {
              return RedirectToAction("Index", "Home");
           }
        }


        protected string RenderView(Controller controller, string viewName, object model)
         {
            var result = ViewEngines.Engines.FindView(controller.ControllerContext, viewName,null);
            return Render(result, controller, viewName, model);
         }


        protected string RenderPartialView(Controller controller, string viewName, object model)
        {
           var result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
           return Render(result, controller, viewName,  model);
        }

        private string Render(ViewEngineResult result, Controller controller, string viewName, object model)
         {
            if (result.View == null)
            {
               throw new Exception("Cannot find view , searched here : \n" + string.Join("\n",result.SearchedLocations));
            }
            if (string.IsNullOrEmpty(viewName))
            {
               viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            using (var sw = new StringWriter())
            {
               controller.ViewData.Model = model;
               var viewContext = new ViewContext(controller.ControllerContext, result.View, controller.ViewData, controller.TempData, sw);
               result.View.Render(viewContext, sw);
               result.ViewEngine.ReleaseView(controller.ControllerContext, result.View);
               return sw.GetStringBuilder().ToString();
            }
         }

        public static ActionResult RedirectToLogin(Uri returnUrl)
        {
            // redirect to logon page
            RouteValueDictionary rv = new RouteValueDictionary(new
            {
                controller = "Account",
                action = "Login",
                ReturnUrl = returnUrl
            });

            return new RedirectToRouteResult(rv);
        }

        public static ActionResult RedirectToNotAuthorised()
        {
            // redirect to logon page
            RouteValueDictionary rv = new RouteValueDictionary(new
            {
                controller = "Account",
                action = "NotAuthorised"
            });

            return new RedirectToRouteResult(rv);
        }

        protected virtual string ToHtml(string viewToRender, ViewDataDictionary viewData)
        {
            //var controllerContext = Request.RequestContext;
            var result = ViewEngines.Engines.FindView(ControllerContext, viewToRender, null);

            StringWriter output;
            using (output = new StringWriter())
            {
                var viewContext = new ViewContext(ControllerContext, result.View, viewData, ControllerContext.Controller.TempData, output);
                result.View.Render(viewContext, output);
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }

            return output.ToString();
        }

    }
}
