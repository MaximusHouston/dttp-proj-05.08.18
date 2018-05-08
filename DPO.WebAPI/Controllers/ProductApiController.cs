using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using DPO.Common;
using DPO.Domain;
using DPO.Data;
using System.Web.Http.OData;

namespace DPO.WebAPI.Controllers
{
    public class ProductApiController : ApiController
    {
        ProductServices productservices = new ProductServices();

        [AllowAnonymous]
        [HttpGet]
        [Route("webapi/gettime")]
        public IHttpActionResult GetTime() {
            return Ok("Server time is: " + DateTime.Now.ToString());
        }

        [Authorize]
        [HttpGet]
        [Route("webapi/getuser")]
        public IHttpActionResult GetAuthenticatedUser()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("Hello: " + identity.Name);
        }

        [Authorize]
        [HttpGet]
        [Route("odata/getProducts")]
        [EnableQuery]
        public IQueryable<Product> GetProducts() {
            var products = productservices.Db.Products;
            return products;
        }

        [Authorize]
        [HttpGet]
        [Route("odata/getProductModels")]
        [EnableQuery]
        public IQueryable<ProductListModel> GetProductListModels()
        {
            var query = productservices.GetProductListModels();
            return query;
        }

    }
}
