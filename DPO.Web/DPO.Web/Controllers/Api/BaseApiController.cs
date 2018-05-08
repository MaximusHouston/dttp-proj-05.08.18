using DPO.Common;
using DPO.Domain;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace DPO.Web.Controllers
{
    public class BaseApiController : ApiController
    {
        public ServiceResponse ServiceResponse = null;

        private AccountServices accountService = null;

        private UserSessionModel currentUser = null; 

        public UserSessionModel CurrentUser
        {
            get
            {
                return accountService.LoadUserSessionModel();
            }
            set
            {
                this.currentUser = value;
            }
        }

        public bool IsPostRequest { get { return (Request.Method == HttpMethod.Post); } }

        public BaseApiController()
            : base()
        {
            accountService = new AccountServices();
        }
        
        public HttpResponseMessage RedirectToLogin()
        {
            var response = Request.CreateResponse(HttpStatusCode.Found);
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(baseUrl + "/v2/#/account/login");
            return response;
        }
    }
}