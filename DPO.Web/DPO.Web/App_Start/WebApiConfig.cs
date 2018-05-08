using DPO.Common;
using DPO.Web;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DPO.Web
{
    public static class WebApiConfig
    {
        public static string UrlPrefix { get { return "api"; } }

        public static string UrlPrefixRelative { get { return "~/api"; } }

        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: WebApiConfig.UrlPrefix + "/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var json = config.Formatters.JsonFormatter;
            // Required for camel case.
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Required to handle issue with longs not properly being sent.  They must be strings.
            json.SerializerSettings.Converters.Add(new JsonLongToStringConverter());

            // Required for proper serialization.
            ((DefaultContractResolver)json.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

      
        }
    }
}
