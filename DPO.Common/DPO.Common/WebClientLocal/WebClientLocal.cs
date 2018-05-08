using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DPO.Common
{

    //Cookie-Aware WebClient
    public class WebClientLocal : WebClient
    {

        public WebClientLocal(HttpContext context)
        {
            // Logon to document server using current creditential or super if from services

            var user = (context == null) ? Utilities.Config("dpo.setup.superuser.username") : context.User.Identity.Name;
         
            string url = Utilities.DocumentServerURL() + "/Account/InternalLogin"; //put this back before upload to Dev Server

            //string url = "http://localhost:62801" + "/Account/InternalLogin";

            string parms = "&securityKey=" + HttpUtility.UrlEncode(Crypto.Encrypt(user + "#" + DateTime.Now.ToString()));

            this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            string HtmlResult = this.UploadString(url, "POST", parms);

            return;
         
        }


        public CookieContainer cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = cookieContainer;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response;
            try
            {
                response = base.GetWebResponse(request);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                cookieContainer.Add(cookies);
            }
        }

    }

}


