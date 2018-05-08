using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;

namespace DPO.Web.Controllers
{
    [Authorize]
    public class CommonController : BaseApiController
    {
        public CommonServiceLight commonservice = new CommonServiceLight();

        //public UploadFileService uploadfileservice = new UploadFileService() {
        //    FilePath = ConfigurationManager.AppSettings["dpo.setup.customerdata.location"]
        //};

        //[HttpPost]
        //public ServiceResponse SaveFileUpload(HttpPostedFileBase file)
        //{
        //    uploadfileservice.FilePath += "POAttachmentFiles\\" + "orderId" + "\\";
        //    var fileName = uploadfileservice.SaveFile(file);
        //    this.ServiceResponse.Model = fileName;
        //    return this.ServiceResponse;
           
        //}

        //[HttpPost]
        //public ServiceResponse RemoveFileUpload(string file)
        //{
        //    uploadfileservice.RemoveFile(file);
        //    return null;
        //}


        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetContructionTypes()
        {
            return commonservice.GetContructionTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjectStatusTypes()
        {
            return commonservice.GetProjectStatusTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjectTypes()
        {
            return commonservice.GetProjectTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjectOpenStatusTypes()
        {
            return commonservice.GetProjectOpenStatusTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetVerticalMarketTypes()
        {
            return commonservice.GetVerticalMarketTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetStates()
        {
            return commonservice.GetStates(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetStatesByCountry(string countryCode)
        {
            return commonservice.GetStatesByCountry(this.CurrentUser, countryCode);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetStateIdByStateCode(string stateCode)
        {
            return commonservice.GetStateIdByStateCode(this.CurrentUser, stateCode);
        }


    }
}