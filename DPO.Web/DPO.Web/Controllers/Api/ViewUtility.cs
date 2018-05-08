using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DPO.Web.Controllers
{
    //TODO: This class is not being used, it was created to render view in web api controller.
    public static class ViewUtility
    {
        public static string RenderView(string partialName, object model)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // point to an empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmptyController");

            //var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new ProjectDashboardController());

            var view = ViewEngines.Engines.FindView(controllerContext, partialName, null).View;

            view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }
        public static string RenderPartial(string partialName, object model)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // point to an empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmptyController");

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;

            view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }

        //ViewEngines.Engines.FindView
    }

    class EmptyController : Controller { }

    //class MockController : ProjectDashboardController { }
}