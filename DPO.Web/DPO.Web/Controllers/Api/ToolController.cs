using System;
using System.Collections.Generic;
using DPO.Common;
using DPO.Domain;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using DPO.Web.Controllers.Api.Filters;

namespace DPO.Web.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    [UserActionFilter]
    public class ToolController : BaseApiController
    {

        [HttpGet]
        public HttpResponseMessage SystemConfigurator(long? quoteId = null)
        {
            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = quoteId ?? 0;
            CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? 0;

            var response = Request.CreateResponse(HttpStatusCode.Found);
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(baseUrl + "/v2/#/tools/systemConfigurator");
            return response;
        }

        [HttpGet]
        public HttpResponseMessage SplitSystemConfigurator(long? quoteId = null)
        {
            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = quoteId ?? 0;
            CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? 0;

            var response = Request.CreateResponse(HttpStatusCode.Found);
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(baseUrl + "/v2/#/tools/splitSystemConfigurator");
            return response;
        }

        [HttpGet]
        public List<ToolModel> GetTools()
        {
            using (var permissionService = new PermissionServices())
            {
                var permissions = permissionService.GetToolLinksForUser(this.CurrentUser);

                return permissions;
            }
        }
    }
}