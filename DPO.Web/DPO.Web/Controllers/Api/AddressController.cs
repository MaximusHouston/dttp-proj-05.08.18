using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Model.Light;
using DPO.Services.Light;
using System.Data;
using DPO.Domain.Services;
using DPO.Domain.DataQualityService;

namespace DPO.Web.Controllers
{
    [Authorize]
    public class AddressController : BaseApiController
    {
        public ServiceResponse response = new ServiceResponse();

        public ProjectServices projectService = new ProjectServices();

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse VerifyAddress(AddressModel model)
        {
            CleanAddressRequest addressReq = new CleanAddressRequest();
            addressReq.Address = new DQAddress();

            var addressline1 = model.AddressLine1.Replace(".", "");

            addressReq.Address.Line1 = addressline1;
            addressReq.Address.Line2 = (model.AddressLine2 != null) ? model.AddressLine2 : string.Empty;

            //var stateName = model.States.Items.Where(s => s.Value == model.StateId.ToString()).FirstOrDefault().Text;
            var stateCode = projectService.GetStateCodeByStateId((int)model.StateId);
            addressReq.Address.StateProvince = stateCode;
            addressReq.Address.ZipCode = model.PostalCode;
            addressReq.Address.City = model.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            CleanAddressResponse addressResp = (CleanAddressResponse)proxy.Execute(addressReq);
                        
            if (addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                this.response.AddError("Address is not verified");
                this.response.Model = addressResp;
            }
            else
            {
                if (addressResp.Addresses[0].Line1 == addressReq.Address.Line1 &&
                    addressResp.Addresses[0].City == addressReq.Address.City &&
                    addressResp.Addresses[0].StateProvince == addressReq.Address.StateProvince &&
                    addressResp.Addresses[0].ZipCode == addressReq.Address.ZipCode)
                {
                    this.response.Model = addressResp;
                }
                else {
                    this.response.AddError("Address does not match the suggested address.");
                    this.response.Model = addressResp;
                }
            }
            return this.response;
        }

        [HttpGet]
        public ServiceResponse GetStatesByCountry(string countryCode) {
            var states = new HtmlServices().DropDownModelStates(new AddressModel { CountryCode = countryCode });
            response.Model = states;
            return response;
        }
    }
}