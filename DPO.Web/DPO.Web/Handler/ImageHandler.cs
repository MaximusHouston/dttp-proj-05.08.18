using DPO.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace DPO.Web
{
    public class ImageHandler : IHttpHandler
    {
        private string id;
        private string type;

        public ImageHandler(string type, string id)
        {
            this.id = id;
            this.type = type;
        }


        public bool IsReusable
        {
            get { return true; }
        }


        public void ProcessRequest(HttpContext request)
        {
            var context = request;

            var imageLocation = request.Server.MapPath("~/Images/");

            byte imageType;
            if (!byte.TryParse(this.type, out imageType))
            {
                imageType = (byte)ImageTypeEnum.DocumentImages;
            }

            string imageFile = null;

            switch ((ImageTypeEnum)imageType)
            {
                case ImageTypeEnum.None:
                    break;
                case ImageTypeEnum.ProductFamily:
                    imageLocation += "Products\\Family\\";
                    imageFile = imageLocation + this.id + ".png";
                    break;
                case ImageTypeEnum.ProductCategory:
                    imageLocation += "Products\\Category\\";
                    imageFile = imageLocation + this.id + ".png";
                    break;
                case ImageTypeEnum.DocumentImages:
                    var baseDirectory = Utilities.GetDocumentDirectory();
                    try
                    {
                        imageFile = Directory.GetFiles(baseDirectory + type, this.id + ".*").FirstOrDefault();
                    }
                    catch(Exception ex)
                    {
                        // Image not found
                        int x = 1;
                    }
                    break;
                default:
                    break;
            }

            context.Response.Clear();

            //TODO Get image from webservice then save as file
            if (!File.Exists(imageFile))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var updatedDate = File.GetLastWriteTime(imageFile);

            if (!String.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var lastMod = DateTime.ParseExact(context.Request.Headers["If-Modified-Since"], "r", provider).ToLocalTime();
                // A way of stripping milli secs
                if (Math.Abs(new TimeSpan(updatedDate.Ticks - lastMod.Ticks).TotalSeconds) <= 1)
                {
                    context.Response.StatusCode = 304;
                    return;
                }
            }

            context.Response.ContentType = MimeMapping.GetMimeMapping(imageFile);
            context.Response.AddFileDependency(imageFile);
            context.Response.Cache.SetETagFromFileDependencies();
            context.Response.Cache.SetLastModifiedFromFileDependencies();
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddTicks(600));
            context.Response.Cache.SetMaxAge(new TimeSpan(999));
            context.Response.Cache.SetSlidingExpiration(true);
            context.Response.Cache.SetValidUntilExpires(true);
            context.Response.TransmitFile(imageFile);
            return;
        }
    }

    //public class SessionableImageHandler : HttpControllerHandler, IRequiresSessionState
    //{
    //    public SessionableImageHandler(RouteData routeData) : base(routeData)
    //    {
    //        var f = 1;
    //    }
    //}
    public class ImageRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string type = requestContext.RouteData.Values["type"] as string;
            string id = requestContext.RouteData.Values["id"] as string;

            return new ImageHandler(type, id);

        }
    }
}
