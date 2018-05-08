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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DPO.Domain
{

    public partial class UserServices
    {

        // #################################################
        // Rules for model validation
        // #################################################
        public void RulesOnValidateModel(UserModel model)
        {
            Validation.IsEmail(this.Response.Messages, model.Email, "Email", "Email Address", 75, true);

            Validation.IsText(this.Response.Messages, model.FirstName, "FirstName", "First Name", 2, 50, true);

            Validation.IsText(this.Response.Messages, model.MiddleName, "MiddleName", "Middle Name", 2, 50, false);

            Validation.IsText(this.Response.Messages, model.LastName, "LastName", "Last Name", 2, 50, true);
            
            // Validate address
            if (model.ValidateAddress)
            {
                new AddressServices(this,"Address").RulesOnValidateModel(model.Address,true);
                this.Response.PropertyReference = "";
            }

            model.Contact.ValidateForCountryCode = model.Address.CountryCode;
            new ContactServices(this,"Contact").RulesOnValidateModel(model.Contact);
            this.Response.PropertyReference = "";

            if (model.IsRegistering && model.IsNewBusiness)
            {
                new BusinessServices(this,"Business").RulesOnValidateModel(null,model.Business, false);
                this.Response.PropertyReference = "";
            }

            if (model.IsRegistering && !model.IsNewBusiness)
            {
                Validation.IsText(this.Response.Messages, model.Business.AccountId, "Business.AccountId", "Account ID", 1, 50, true);
            }

        }


        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            var user = entity as User;

            if (user == null)
            {
                throw new ArgumentException("User entity not loaded");
            }

            if (user.IsRegistering)
            {
                user.RegisteredOn = DateTime.UtcNow;

                user.UserTypeId = (int)UserTypeEnum.NotSet;

                user.Approved = false;
            }

            user.Enabled = true;

            RulesCommon(admin, user);

            // If user is registering make sure he has a business
            if (user.Business == null && user.IsRegistering == false && admin.BusinessId.HasValue)
            {
                user.BusinessId = admin.BusinessId;
            }

        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var user = entity as User;

            if (user == null)
            {
                throw new ArgumentException("User entity not loaded");
            }

            RulesCommon(admin, user);

        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
                throw new ArgumentException("Cannot delete user");
        }

        // #################################################
        // Rules to calculate the actual project total
        // #################################################
        private void RulesCommon(UserSessionModel admin, User user)
        {

            if (!user.IsRegistering)
            {
                var passSecurity = (admin.HasAccess(SystemAccessEnum.AdminAccessRights) && admin.UserTypeId  >= user.UserTypeId);
                passSecurity = passSecurity || (admin.UserTypeId > user.UserTypeId && admin.UserId != user.UserId);

                if (!passSecurity)
                {
                    this.Response.Messages.AddError("", Resources.DataMessages.DM005);
                }

                // if you are managing your own account these values should not change
                if (!admin.HasAccess(SystemAccessEnum.AdminAccessRights) && user.UserId == admin.UserId && (Entry.HasChanged("UserTypeId") || Entry.HasChanged("BusinessId")))
                {
                    this.Response.Messages.AddError("", Resources.SystemMessages.SM007);
                }
            }

            if (user.Approved && Entry.HasChanged("Approved"))
            {
                user.ApprovedOn = DateTime.UtcNow;
                user.Enabled = true;
                user.Rejected = false;
            }

            if (user.Rejected && Entry.HasChanged("Rejected"))
            {
                user.Enabled = false;
                user.Approved = false;
            }



            // Make sure email is unique
            if (Entry.HasChanged("Email") && Db.IsUser(user.Email))
            {
                this.Response.Messages.AddError("Email", ResourceModelUser.MU001);
            }

            // Make sure access level is set for the user if not registering and not rejected
            if (!user.IsRegistering  && user.Rejected == false &&  user.UserTypeId == UserTypeEnum.NotSet)
            {
                if (user.UserTypeId == UserTypeEnum.NotSet)
                {
                    this.Response.Messages.AddError("UserTypeId", ResourceModelUser.MU015);
                }
            }

            if (user.Business != null)
            {
                new BusinessServices(this, "Business").ApplyBusinessRules(admin, user.Business);
                this.Response.PropertyReference = "";
            }

            if (user.Contact != null)
            {
                new ContactServices(this, "Contact").RulesOnEdit(admin, user.Contact);
                this.Response.PropertyReference = "";
            }

            if (user.Address != null)
            {
                new AddressServices(this, "Address").RulesOnEdit(admin, user.Address);
                this.Response.PropertyReference = "";
            }

            if (user.BusinessId != null && this.Response.IsOK && Entry.HasChanged("BusinessId"))
            {
                Db.ReplacePermissions((long)user.BusinessId, user.UserId, PermissionTypeEnum.Brand);
                Db.ReplacePermissions((long)user.BusinessId, user.UserId, PermissionTypeEnum.CityArea);
                Db.ReplacePermissions((long)user.BusinessId, user.UserId, PermissionTypeEnum.ProductFamily);
                Db.ReplacePermissions((long)user.BusinessId, user.UserId, PermissionTypeEnum.Tool);
                
                this.Response.AddSuccess("User product and tool permissions set to the default settings.");

                if (user.Business.BusinessTypeId == BusinessTypeEnum.Daikin ||
                    user.Business.BusinessTypeId == BusinessTypeEnum.ManufacturerRep ||
                    user.Business.BusinessTypeId == BusinessTypeEnum.Distributor)
                {
                    user.ShowPricing = true;
                }
                else
                {
                    user.ShowPricing = false;
                }
                this.Response.AddSuccess("Show prices has been set to the default setting.");
            }

            if (user.BusinessId != null && this.Response.IsOK && Entry.HasChanged("UserTypeId"))
            {
                Db.ReplacePermissions((long)user.UserTypeId, user.UserId, PermissionTypeEnum.SystemAccess);
                this.Response.AddSuccess("User system access permissions set to the default settings.");
            }

            //modify by aaraon
            ////if ( user.BusinessId != null && this.Response.IsOK )
            ////{
            ////    Db.ReplacePermissions((long)user.UserTypeId, user.UserId, PermissionTypeEnum.SystemAccess);
            ////    this.Response.AddSuccess("User system access permission set to the default settings");
            ////}

        }

    }

}