using DPO.Common;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Mvc;
using log4net;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace DPO.Web
{
   // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
   // visit http://go.microsoft.com/?LinkId=9394801

   public class MvcApplication : System.Web.HttpApplication
    { 
      protected void Application_BeginRequest()
      {
            /*start the MiniProfiler
            The code we’ve added only is executed if the application is running locally.It checks
            for the presence of a local HTTP request and if true, it starts MiniProfiler.This check is
            added for security reasons, in case you deployed your application to a production
            environment with the profiling code still enabled.You wouldn’t want anyone to see
            this sensitive information. */

            if (Request.IsLocal)
            {
                //MiniProfiler.Start();
            }
      }

      protected void Application_EndRequest()
      {
            //MiniProfiler.Stop(); 
      }
      
      protected void Application_Start()
      {
         ViewEngines.Engines.Clear();  // remove othe view engines.
         ViewEngines.Engines.Add(new RazorViewEngine()); // add only razor view that we used.
         AreaRegistration.RegisterAllAreas(); // register razor view engine.

         WebApiConfig.Register(GlobalConfiguration.Configuration);
         FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
         RouteConfig.RegisterRoutes(RouteTable.Routes);
         BundleConfig.RegisterBundles(BundleTable.Bundles);

         AuthConfig.RegisterAuth();
           
         /*-- Miniprofiler --*/
          MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
          MiniProfilerEF6.Initialize();

          //System.Data.Entity.DbConfiguration.Loaded += (sender, e) => e.ReplaceService<System.Data.Entity.Core.Common.DbProviderServices>(services, o) => EFProfiledSqlClientDbProviderServices.Instance);

            /*--register controllers;set the dependency resolver --*/
            var builder = new ContainerBuilder();
          
          //register MVC Controllers all at onces
         builder.RegisterControllers(typeof(MvcApplication).Assembly);
          
          //register Web abstraction (ex: httpContextBase ... )
         builder.RegisterModule<AutofacWebTypesModule>();
         
          // Register Property injection
         builder.RegisterSource(new ViewRegistrationSource());

          //Enable property inject to action filters
         builder.RegisterFilterProvider();

          // set Dependency Resolver
         var container = builder.Build();
         DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
         
         //setting log4net
         log4net.Config.XmlConfigurator.Configure();
      }

      protected void Application_Error(object sender, EventArgs e)
      {
          System.Diagnostics.Debug.WriteLine(e.ToString());
          
      }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }
   }
}