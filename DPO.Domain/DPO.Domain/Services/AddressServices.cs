//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Data;
using DPO.Resources;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using DPO.Domain.DataQualityService;

namespace DPO.Domain
{
    public partial class AddressServices : BaseServices 
    {
        public AddressServices() : base() { }

        public AddressServices(DPOContext context) : base(context) { }

        public Address ModelToEntity(AddressModel model)
        {
            var entity = GetEntity(model);

            if (this.Response.HasError) return null;

            if (model != null)
            {
                entity.AddressLine1 = Utilities.Trim(model.AddressLine1);

                entity.AddressLine2 = Utilities.Trim(model.AddressLine2);

                entity.AddressLine3 = Utilities.Trim(model.AddressLine3);

                entity.Location = Utilities.Trim(model.Location);

                entity.StateId = model.StateId;

                entity.PostalCode = model.PostalCode;
            }

            return entity;
        }

        private Address GetEntity(AddressModel model)
        {
            var entity = (model == null || !model.AddressId.HasValue) ? Db.AddressCreate() : Db.GetAddressByAddressId(model.AddressId.Value);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM024);
            }

            return entity;

        }

        public AddressModel GetAddressModel(UserSessionModel admin, AddressModel model)
        {
            //mass upload change - turned this off
            var htmlService = new HtmlServices(this.Context);

            if (model != null && model.AddressId.HasValue)
            {
                var query =
                from address in this.Db.GetAddressQueryByAddressId(model.AddressId)
                select new AddressModel
                    {
                        AddressId = address.AddressId,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressLine3 = address.AddressLine3,
                        Location = address.Location,
                        PostalCode = address.PostalCode,
                        StateId = address.StateId,
                        StateName = address.State.Name,
                        CountryCode = address.State.CountryCode
                    };

                model = query.FirstOrDefault();
            }

            return model ?? new AddressModel();
        }

        public void FinaliseModel(AddressModel model)
        {
            //mass upload change - had to turn these off
            var htmlService = new HtmlServices(this.Context);

            model.Countries = htmlService.DropDownModelCountries(model);
            model.States = htmlService.DropDownModelStates(model);
        }

        public object ValidateAddressesForDARSubmission(UserSessionModel admin, long ProjectId)
        {
            ProjectModel model = (from project  in this.Db.QueryProjectViewableByProjectId(admin, ProjectId)
                                  select new ProjectModel
                                  {
                                      CustomerName = project.CustomerName,
                                      CustomerAddress = new AddressModel
                                      {
                                          AddressId = project.CustomerAddressId,
                                      },

                                      EngineerName = project.EngineerName,
                                      EngineerAddress = new AddressModel
                                      {
                                          AddressId = project.EngineerAddressId,
                                      },

                                      ShipToName = project.ShipToName,
                                      ShipToAddress = new AddressModel
                                      {
                                          AddressId = project.ShipToAddressId,
                                      },

                                  }).FirstOrDefault();

            model.CustomerAddress = this.GetAddressModel(admin, model.CustomerAddress);

            model.EngineerAddress = this.GetAddressModel(admin, model.EngineerAddress);

            model.ShipToAddress = this.GetAddressModel(admin, model.ShipToAddress);

            //public string CustomerName { get; set; }
            // public string EngineerName { get; set; }
            //public string ShipToName    { get; set; }

            //engineer name?
            List<AddressModel> Addresses = new List<AddressModel>
            {
                model.EngineerAddress,
                model.CustomerAddress,
                model.ShipToAddress
            };

            List<string> AddressNames = new List<string>
            {
                model.EngineerName,
                model.CustomerName,
                model.ShipToName
            };

            List<string> AddressTypes = new List<string>{
                ResourceUI.EngineerDetails,
                ResourceUI.CustomerAddress,
                ResourceUI.ShipToAddress
            };

            List<object> errors = new List<object>();

            for (int i = 0; i < Addresses.Count; i++ )
            {
                var address = new
                {
                    name = AddressTypes[i],
                    errors = new List<string>()
                };

                if(AddressNames[i] == null)
                {
                    address.errors.Add(ResourceUI.BusinessName);
                }

                if (Addresses[i].AddressLine1 == null)
                {
                    address.errors.Add(ResourceUI.AddressLine1);
                }
                //does this need to be validated?
                //if (Addresses[i].AddressLine2 == null)
                //{
                //    address.errors.Add(ResourceUI.AddressLine2);
                //}
                if (Addresses[i].Location == null)
                {
                    address.errors.Add(ResourceUI.Location);
                }
                if (Addresses[i].PostalCode == null)
                {
                    address.errors.Add(ResourceUI.ZipCode);
                }
                if(Addresses[i].CountryCode == null)
                {
                    address.errors.Add(ResourceUI.Country);
                }
                if (Addresses[i].StateId == null)
                {
                    address.errors.Add(ResourceUI.State);
                }

                if (address.errors.Count > 0)
                {
                    errors.Add(address);
                }

            }

            return new { errors = errors, isvalid = (errors.Count == 0) };
        }

        public ServiceResponse VerifyAddress(AddressModel model) {
            ProjectServices projectSvc = new ProjectServices();
            CleanAddressRequest addressReq = new CleanAddressRequest();
            addressReq.Address = new DQAddress();

            var addressline1 = model.AddressLine1.Replace(".", "");

            addressReq.Address.Line1 = addressline1;
            addressReq.Address.Line2 = (model.AddressLine2 != null) ? model.AddressLine2 : string.Empty;

            //var stateName = model.States.Items.Where(s => s.Value == model.StateId.ToString()).FirstOrDefault().Text;
            var stateCode = projectSvc.GetStateCodeByStateId((int)model.StateId);
            addressReq.Address.StateProvince = stateCode;
            addressReq.Address.ZipCode = model.PostalCode;
            addressReq.Address.City = model.Location;

            DataQualityService.DataQualityServiceClient proxy = new DataQualityService.DataQualityServiceClient("BasicHttpBinding_IDataQualityService");
            CleanAddressResponse addressResp = (CleanAddressResponse)proxy.Execute(addressReq);

            if (addressResp.Addresses == null || addressResp.Addresses.Count() == 0)
            {
                this.Response.AddError("Address is not verified");
                this.Response.Model = addressResp;
            }
            else
            {
                if (addressResp.Addresses[0].Line1 == addressReq.Address.Line1 &&
                    addressResp.Addresses[0].City == addressReq.Address.City &&
                    addressResp.Addresses[0].StateProvince == addressReq.Address.StateProvince &&
                    addressResp.Addresses[0].ZipCode == addressReq.Address.ZipCode)
                {
                    this.Response.Model = addressResp;
                }
                else
                {
                    this.Response.AddError("Please verify address");
                    this.Response.Model = addressResp;
                }
            }
            return this.Response;

        }
  
    }

}