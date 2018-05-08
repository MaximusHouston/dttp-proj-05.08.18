using DPO.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace DPO.Web
{
    //public class SessionableImageHandler : HttpControllerHandler, IRequiresSessionState
    //{
    //    public SessionableImageHandler(RouteData routeData) : base(routeData)
    //    {
    //        var f = 1;
    //    }
    //}
    public class DaikinCityWebRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new DaikinCityWebHandler();
        }
    }

    public class DaikinCityWebHandler : IHttpHandler
    {
        public DaikinCityWebHandler()
        {
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public static void StreamIndexBodySection()
        {
            string html = Html("index.html");

            // Populate the html string here

            int startpos = html.IndexOf(">", html.IndexOf("<!--DAIKINCITY-START-->")) + 1;
            int endpos = html.IndexOf("<!--DAIKINCITY-END-->", startpos);

            string body = html.Substring(startpos, endpos - startpos);

            HttpContext.Current.Response.Write(body);
        }

        public static string MapPath(string url)
        {
            var baseDirectory = "";
            
            if (url.Contains("daikincityweb/documents/"))
            {
                url = url.Replace("daikincityweb/documents/", "daikincitydocuments/");

                baseDirectory =  Utilities.GetDocumentDirectory();
            }
            else
            {
                url = url.Replace("daikincityweb/", "");

                baseDirectory = Utilities.GetDaikinCityDirectory();
            }

            var file = baseDirectory + url;

            file = file.Replace("/", "\\");

            file = HttpUtility.UrlDecode(file.Replace("\\\\", "\\"));

            file = HttpUtility.UrlDecode(file.Replace("#",""));

            file = HttpUtility.UrlDecode(file.Replace("&","-"));

            file = HttpUtility.UrlDecode(file.Replace("@",""));

            return file;

        }

        public static string Html(string url)
        {
            var file = MapPath(url);

            var html = File.ReadAllText(file);

            return html;

        }
        public void ProcessRequest(HttpContext context)
        {

            var request = context.Request;

            var path = request.Url.AbsolutePath.ToLower();
 
            var file = MapPath(path);

            context.Response.Clear();
             
            //TODO Get image from webservice then save as file
            if (!File.Exists(file))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var updatedDate = File.GetLastWriteTime(file);

            if (!String.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var lastMod = DateTime.ParseExact(context.Request.Headers["If-Modified-Since"], "r", provider).ToLocalTime();
          
                if (new TimeSpan(updatedDate.Ticks - lastMod.Ticks).TotalSeconds <= 1)
                {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.ContentType = MimeMapping.GetMimeMapping(file);
            context.Response.AddFileDependency(file);
            context.Response.Headers.Add("Accept-Ranges", "bytes");
            context.Response.Cache.SetETagFromFileDependencies();
            context.Response.Cache.SetLastModifiedFromFileDependencies();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddTicks(600));
            context.Response.Cache.SetMaxAge(new TimeSpan(999));
            context.Response.Cache.SetSlidingExpiration(true);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.TransmitFile(file);
            return;
        }
    }
}
