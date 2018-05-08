//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DPO.Domain
{

    public partial class BusinessServices
    {
        // #################################################
        // Rules for model validation
        // #################################################
        public void RulesOnValidateModel(UserSessionModel admin, BusinessModel model, bool businessEdit)
        {
            Validation.IsText(this.Response.Messages, model.BusinessName, "BusinessName", "Business Name", 50, true);

            Validation.IsText(this.Response.Messages, model.AccountId, "AccountId", "Account ID", 1, 50, false);

            if (model.IsRegistering && model.BusinessTypeId == null)
            {
                this.Response.Messages.AddError("BusinessTypeId", Resources.ResourceModelBusiness.BM005);
            }

            if (businessEdit && model.BusinessTypeId == null && admin != null && admin.UserTypeId >= UserTypeEnum.DaikinEmployee)
            {
                this.Response.Messages.AddError("BusinessTypeId", Resources.ResourceModelBusiness.BM005);
            }

            if (model.BusinessId == model.ParentBusinessId) {
                this.Response.Messages.AddError("BusinessId cannot be the same as Parent BusinessId!");
            }

            if (businessEdit)
            {
                addressService.BeginPropertyReference(this, "Address");
                addressService.RulesOnValidateModel(model.Address, false);
                addressService.EndPropertyReference();

                model.Contact.ValidateForCountryCode = model.Address.CountryCode;
                contactService.BeginPropertyReference(this, "Contact");
                contactService.RulesOnValidateModel(model.Contact);
                contactService.EndPropertyReference();
            }
        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            var business = entity as Business;

            RulesCommon(admin, business);

            if (business.Address != null)
            {
                addressService.BeginPropertyReference(this, "Address");
                addressService.RulesOnAdd(admin, business.Address);
                addressService.EndPropertyReference();
            }

            if (business.Contact != null)
            {
                contactService.BeginPropertyReference(this, "Contact");
                contactService.RulesOnAdd(admin, business.Contact);
                contactService.EndPropertyReference();
            }

            if (business.IsWebServiceImport)
            {
                business.Enabled = true;
            }
        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var business = entity as Business;

            RulesCommon(admin, business);

            if (business.Address != null)
            {
                addressService.BeginPropertyReference(this, "Address");
                addressService.RulesOnEdit(admin, business.Address);
                addressService.EndPropertyReference();
            }

            if (business.Contact != null)
            {
                contactService.BeginPropertyReference(this, "Contact");
                contactService.RulesOnEdit(admin, business.Contact);
                contactService.EndPropertyReference();
            }

            // Make only Daikin personnel can change businesstype
            if (Entry.HasChanged("BusinessTypeId") && admin.UserTypeId < UserTypeEnum.DaikinEmployee)
            {
                this.Response.Messages.AddError("BusinessTypeId", Resources.ResourceModelBusiness.BM006);
            }

            if (Entry.HasChanged("BusinessTypeId"))
            {
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)business.BusinessTypeId, EntityEnum.Business, business.BusinessId, PermissionTypeEnum.Brand);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)business.BusinessTypeId, EntityEnum.Business, business.BusinessId, PermissionTypeEnum.CityArea);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)business.BusinessTypeId, EntityEnum.Business, business.BusinessId, PermissionTypeEnum.ProductFamily);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)business.BusinessTypeId, EntityEnum.Business, business.BusinessId, PermissionTypeEnum.Tool);

                this.Response.AddSuccess("Business product and tool permissions set to default settings.");
            }
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
            throw new NotImplementedException("Business cannot be deleted");
        }

        protected void RulesCommon(UserSessionModel admin, Business business)
        {
            if (business == null)
            {
                throw new ArgumentException("Business entity not loaded");
            }

            if (!business.IsWebServiceImport && !string.IsNullOrEmpty(business.AccountId) && Entry.HasChanged("AccountId"))
            {
                if (Db.IsAccountIdInUse(business.AccountId))
                {
                    this.Response.AddError("AccountId", Resources.ResourceModelBusiness.BM003);
                }
                else
                {
                    //Call Daikin web services to see if account exists
                    if (new DaikinServices().IsDaikinAccount(business.AccountId) == false)
                    {
                        this.Response.AddError("AccountId", Resources.ResourceModelBusiness.BM001);
                    }
                }
            }

            if (business.IsWebServiceImport)
            {
                if (Entry.HasChanged("BusinessName") && Db.IsBusinessNameInUse(admin, business.BusinessName))
                {
                    this.Response.AddError("BusinessName", Resources.ResourceModelBusiness.BM004);
                    return;
                }
            }

            // Make sure Daikin / Distributors and ManufacturerRep have account ids 
            if ((business.BusinessTypeId == BusinessTypeEnum.Distributor ||
                business.BusinessTypeId == BusinessTypeEnum.ManufacturerRep ||
                business.BusinessTypeId == BusinessTypeEnum.Daikin))
            {
                if (string.IsNullOrEmpty(business.AccountId) && string.IsNullOrEmpty(business.DaikinCityId))
                {
                    this.Response.Messages.AddError("BusinessTypeId", Resources.ResourceModelBusiness.BM007);
                }
            }
        }
    }
}