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
                addressService.BeginPropertyReference(this, "Address");
                addressService.RulesOnValidateModel(model.Address, true);
                addressService.EndPropertyReference();
            }

            model.Contact.ValidateForCountryCode = model.Address.CountryCode;
            contactService.BeginPropertyReference(this, "Contact");
            contactService.RulesOnValidateModel(model.Contact);
            contactService.EndPropertyReference();

            if (model.IsRegistering)
            {
                switch (model.ExistingBusiness)
                {
                    case ExistingBusinessEnum.New:
                        businessService.BeginPropertyReference(this, "Business");
                        businessService.RulesOnValidateModel(null, model.Business, false);
                        businessService.EndPropertyReference();
                        break;
                    case ExistingBusinessEnum.Existing:
                        if (string.IsNullOrEmpty(model.Business.AccountId) && string.IsNullOrEmpty(model.Business.DaikinCityId))
                        {
                            Validation.IsText(this.Response.Messages, model.Business.AccountId, "Business.AccountId", "Account ID", 1, 50, true);
                        }
                        break;
                }
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
                var passSecurity = (admin.HasAccess(SystemAccessEnum.AdminAccessRights) && admin.UserTypeId >= user.UserTypeId);
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
            if (!user.IsRegistering && user.Rejected == false && user.UserTypeId == UserTypeEnum.NotSet)
            {
                if (user.UserTypeId == UserTypeEnum.NotSet)
                {
                    this.Response.Messages.AddError("UserTypeId", ResourceModelUser.MU015);
                }
            }

            // Validate that the business they are registering under is enabled if it was looked up
            if (user.IsRegistering
                && user.Business != null
                && !string.IsNullOrEmpty(user.Business.DaikinCityId)
                && !user.Business.Enabled)
            {
                this.Response.Messages.AddError(ResourceModelUser.MU023);
            }

            if (user.Business != null)
            {
                businessService.BeginPropertyReference(this, "Business");
                businessService.ApplyBusinessRules(admin, user.Business);
                businessService.EndPropertyReference();
            }

            if (user.Contact != null)
            {
                contactService.BeginPropertyReference(this, "Contact");
                contactService.RulesOnEdit(admin, user.Contact);
                contactService.EndPropertyReference();
            }

            if (user.Address != null)
            {
                addressService.BeginPropertyReference(this, "Address");
                addressService.RulesOnEdit(admin, user.Address);
                addressService.EndPropertyReference();
            }

            if (user.BusinessId != null && this.Response.IsOK && Entry.HasChanged("BusinessId"))
            {

                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.Business.BusinessTypeId, EntityEnum.User, user.UserId, PermissionTypeEnum.Brand);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.Business.BusinessTypeId, EntityEnum.User, user.UserId, PermissionTypeEnum.CityArea);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.Business.BusinessTypeId, EntityEnum.User, user.UserId, PermissionTypeEnum.ProductFamily);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.Business.BusinessTypeId, EntityEnum.User, user.UserId, PermissionTypeEnum.Tool);

                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.BusinessId, EntityEnum.User, user.UserId, PermissionTypeEnum.Brand);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.BusinessId, EntityEnum.User, user.UserId, PermissionTypeEnum.CityArea);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.BusinessId, EntityEnum.User, user.UserId, PermissionTypeEnum.ProductFamily);
                Db.ReplacePermissions(EntityEnum.BusinessType, (long)user.BusinessId, EntityEnum.User, user.UserId, PermissionTypeEnum.Tool);

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
                Db.ReplacePermissions(EntityEnum.UserType, (long)user.UserTypeId, EntityEnum.User, user.UserId, PermissionTypeEnum.SystemAccess);

                var permissionsToRemove = this.Db.GetPermissionList(user.UserId).Select(p => p)
                                     .Where(p => p.PermissionTypeId == PermissionTypeEnum.SystemAccess)
                                     .ToList();

                var permissionsToAdd = this.Db.GetPermissionList((long)user.UserTypeId)
                    .Where(p => p.PermissionTypeId == PermissionTypeEnum.SystemAccess)
                    .Select(p => new { permissionId = p.PermissionId, referenceId = p.ReferenceId, referenceEntityId = p.ReferenceEntityId }).ToList();

                var newPermissions = this.Db.GetPermissionList((long)user.UserTypeId)
                                         .Where(p => p.PermissionTypeId == PermissionTypeEnum.SystemAccess)
                                         .ToList();

                Db.UpdatePermissionAudit(EntityEnum.UserType, user, newPermissions, PermissionTypeEnum.SystemAccess, admin, permissionsToRemove);

                this.Response.AddSuccess("User system access permissions set to the default settings.");
            }

        }

    }

}