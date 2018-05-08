using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;

namespace DPO.Web.Controllers
{
    public class OverviewController : BaseController
    {
        public OverviewServices services = new OverviewServices();

        public ActionResult OverviewTemplateData(WidgetModel widget, WidgetContainerModel container)
        {
            container.PageSize = null;

            string defaultFilter = services.getDefaultFilter();
            string currentFilter = services.getCurrentFilter(container);
            bool loadCachedData = (currentFilter == defaultFilter);
            string cacheKey = "";

            switch (widget.TemplateId)
            {
                case "ActiveProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "ActiveProjectsTemplate", container);

                    if (loadCachedData) {
                        widget.Data = HttpContext.Cache.Get(cacheKey) as object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetActiveProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else {
                        widget.Data = services.GetActiveProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }
                    //widget.Data = HttpContext.Cache.Get(widget.TemplateId) as object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetActiveProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "OpenProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "OpenProjectsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetOpenProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetOpenProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetOpenProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "NewRegistrationsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "NewRegistrationsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetNewRegistrations(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetNewRegistrations(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetNewRegistrations(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "ProjectAlertsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "ProjectAlertsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetProjectAlerts(this.CurrentUser, 15, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetProjectAlerts(this.CurrentUser, 15, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetProjectAlerts(this.CurrentUser, 15, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "OpenProjectTypesTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "OpenProjectTypesTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetProjectTypes(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetProjectTypes(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetProjectTypes(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "VerticalMarketsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "VerticalMarketsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetVerticalMarket(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetVerticalMarket(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetVerticalMarket(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "WonProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "WonProjectsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetWonProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else {
                        widget.Data = services.GetWonProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetWonProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}

                    break;
                case "LostProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "LostProjectsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetLostProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetLostProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetLostProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "NewProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "NewProjectsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetNewProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetNewProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetNewProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;
                case "PendingProjectsTemplate":

                    cacheKey = services.getCacheKey(this.CurrentUser.UserId.ToString(), "PendingProjectsTemplate", container);

                    if (loadCachedData)
                    {
                        widget.Data = HttpContext.Cache[cacheKey] as Object;
                        if (widget.Data == null)
                        {
                            widget.Data = services.GetPendingProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                            DateTime expiryTime = DateTime.Now.AddMinutes(5);
                            HttpContext.Cache.Insert(cacheKey, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                        }
                    }
                    else
                    {
                        widget.Data = services.GetPendingProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    }

                    //widget.Data = HttpContext.Cache[widget.TemplateId] as Object;
                    //if (widget.Data == null)
                    //{
                    //    widget.Data = services.GetPendingProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                    //    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    //    HttpContext.Cache.Insert(widget.TemplateId, widget.Data, null, expiryTime, Cache.NoSlidingExpiration);
                    //}
                    break;


                //case "ActiveProjectsTemplate":
                //    widget.Data = services.GetActiveProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "OpenProjectsTemplate":
                //    widget.Data = services.GetOpenProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "NewRegistrationsTemplate":
                //    widget.Data = services.GetNewRegistrations(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "ProjectAlertsTemplate":
                //    widget.Data = services.GetProjectAlerts(this.CurrentUser, 15, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "OpenProjectTypesTemplate":
                //    widget.Data = services.GetProjectTypes(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "VerticalMarketsTemplate":
                //    widget.Data = services.GetVerticalMarket(this.CurrentUser, container.ToSearchProjectModel()).Model as List<WidgetData>;
                //    break;
                //case "WonProjectsTemplate":
                //    widget.Data = services.GetWonProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                //    break;
                //case "LostProjectsTemplate":
                //    widget.Data = services.GetLostProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                //    break;
                //case "NewProjectsTemplate":
                //    widget.Data = services.GetNewProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                //    break;
                //case "PendingProjectsTemplate":
                //    widget.Data = services.GetPendingProjects(this.CurrentUser, container.ToSearchProjectModel()).Model as Dictionary<string, List<WidgetData>>;
                //    break;

                default:
                    break;
            }

            return Json(widget);
        }

        
       
        public ActionResult OverviewTemplate(WidgetModel widget)
        {
            List<WidgetModel> availableWidgetTypes = services.GetAvailableWidgetTypes();
            
            var model = new WidgetContainerModel()
            {
                AvailableWidgetTypes = availableWidgetTypes,
                Widget = widget
            };

            widget.Data = HttpContext.Cache.Get(widget.TemplateId) as object;

            return PartialView(widget.TemplateId, model);
        }


    }
}