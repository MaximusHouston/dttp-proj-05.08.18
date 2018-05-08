using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DPO.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.IgnoreRoute("v2/");

            #region daikin city overrides

            // config
            routes.MapRoute(
                name: "DaikinCityConfigJson",
                url: "daikincityweb/json/config.json",
                defaults: new { controller = "CityCMS", action = "Config" }
                );

            // library documents
            routes.MapRoute(
                name: "DaikinCityDocumentsJson",
                url: "daikincityweb/json/documents.json",
                defaults: new { controller = "CityCMS", action = "LibraryDocuments" }
                );

            // hotel room
            routes.MapRoute(
                name: "DaikinCityHotelJson",
                url: "daikincityweb/json/hotel_room.json",
                defaults: new { controller = "CityCMS", action = "HotelRoom" }
                );

            // remaining buildings
            routes.MapRoute(
                name: "DaikinCityJson",
                url: "daikincityweb/json/{buildingname}.json",
                defaults: new { controller = "CityCMS", action = "BuildingToJson" }
                );

            #endregion

            routes.Add("image", new Route("image/{type}/{id}", new ImageRouteHandler()));
            routes.Add("document", new Route("document/{type}/{id}/{*pathInfo}", new DocumentRouteHandler()));
            routes.Add("daikincityweb", new Route("daikincity{resource}/{*all}", new DaikinCityWebRouteHandler()));
            routes.IgnoreRoute("Apps");

            routes.MapRoute("errors", "errors/{resource}", new { action = "view", controller = "error", resource = UrlParameter.Optional }, new string[] { "DPO.Web.Controllers" });

            routes.MapRoute(
                 name: "quote",
                 url: "projectdashboard/{action}/{projectid}/{quoteid}",
                 defaults: new { controller = "ProjectDashboard", action = "quote" }
              );

            routes.MapRoute(
               name: "product",
               url: "ProductDashboard/{action}/{productfamilyid}/{productcategoryid}/{quoteid}",
               defaults: new
               {
                   controller = "ProductDashboard",
                   action = "ProductFamilies",
                   productfamilyid = UrlParameter.Optional,
                   productcategoryid = UrlParameter.Optional,
                   productnumber = UrlParameter.Optional,
                   quoteid = UrlParameter.Optional
               }
            );

            routes.MapRoute(
               name: "project",
               url: "projectdashboard/{action}/{projectid}",
               defaults: new { controller = "ProjectDashboard", action = "project" }
           );

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                 name: "Default1",
                 url: "{controller}/{action}/{model}",
                 defaults: new { controller = "UserDashboard", action = "AssignUserToGroup", model = UrlParameter.Optional }
                 );

          //  routes.MapRoute(
          //     name: "Angular2",
          //     url: "Angular2/Index/{*path}",
          //     defaults: new { controller = "Angular2", action = "Index", path = UrlParameter.Optional }
          //);



            // Catch all
            routes.MapRoute(
             name: "Default2",
             url: "{controller}/{action}/{id}/{*pathInfo}",
             defaults: new { controller = "Home", action = "Index" }
            );

            //routes.MapRoute(
            //    name: "TradeShow",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "TradeShow", action = "Index", id = UrlParameter.Optional }
            //    );
        }
    }
}