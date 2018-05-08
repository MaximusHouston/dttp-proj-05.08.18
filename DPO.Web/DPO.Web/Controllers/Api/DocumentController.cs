using System;
using System.Collections.Generic;
using DPO.Common;
using DPO.Domain;
using DPO.Model.Light;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using DPO.Web.Controllers.Api.Filters;


namespace DPO.Web.Controllers
{
    [Authorize]
    [UserActionFilter]
    public class DocumentController : BaseApiController
    {
        DocumentServices documentServices = new DocumentServices();
        //Daikin Equip App
        [HttpPost]
        public ServiceResponse GetAllDocuments(DocumentQueryModel queryModel)
        {
            if (queryModel == null)
            {
                queryModel = new DocumentQueryModel();
            }
            return documentServices.GetAllDocuments(queryModel);
            
        }
    }
}