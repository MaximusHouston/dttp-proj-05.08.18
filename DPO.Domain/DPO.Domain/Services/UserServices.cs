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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DPO.Resources;
using System.Net.Mail;
using System.Configuration;
using DPO.Model.Light;
using DPO.Common.Models.General;

namespace DPO.Domain
{

    public partial class UserServices : BaseServices
    {
        private PermissionServices permissionService;
        private AddressServices addressService;
        private ContactServices contactService;
        private BusinessServices businessService;
        private BusinessLinkServices businessLinkService;
        private HtmlServices htmlService;

        public UserServices()
            : base()
        {
            permissionService = new PermissionServices(this.Context);
            addressService = new AddressServices(this.Context);
            contactService = new ContactServices(this.Context);
            businessService = new BusinessServices(this.Context);
            businessLinkService = new BusinessLinkServices(this.Context);
            htmlService = new HtmlServices(this.Context);
        }

        public UserServices(DPOContext context)
            : base(context)
        {
            permissionService = new PermissionServices(this.Context);
            addressService = new AddressServices(this.Context);
            contactService = new ContactServices(this.Context);
            businessService = new BusinessServices(this.Context);
            htmlService = new HtmlServices(this.Context);
        }

        #region Get Requests

        public ServiceResponse GetCityAreaPermissions(UserSessionModel user)
        {
            this.Response.Model = Db.GetPermissionList(user.UserId, PermissionTypeEnum.CityArea).ToList();

            return this.Response;
        }

        public ServiceResponse GetCityAreasForNonLoggedOnUsers()
        {
            this.Response.Model = Db.CityAreas.Where(c => c.CityAreaId == 1).Select(i => i.CityAreaId).ToList(); // Library Only

            return this.Response;
        }

        public ServiceResponse GetApprovalRequestListModel(UserSessionModel user, SearchUser search)
        {
            search.ReturnTotals = true;

            var query = from u in this.Db.QueryUsersViewableBySearch(user, search, true)
                        select new UserListModel
                        {
                            UserId = u.UserId,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            MiddleName = u.MiddleName,
                            LastName = u.LastName,
                            Enabled = u.Enabled,
                            AccountId = u.Business.AccountId,
                            DaikinCityId = u.Business.DaikinCityId,
                            BusinessId = u.BusinessId,
                            BusinessName = u.Business.BusinessName,
                            BusinessTypeDescription = u.Business.BusinessType.Description,
                            UserTypeId = (UserTypeEnum?)u.UserTypeId,
                            RegisteredOn = u.RegisteredOn,
                            Approved = u.Approved,
                            Rejected = u.Rejected,
                            Timestamp = u.Timestamp
                        };

            var result = query.ToList();

            var usertypes = htmlService.DropDownListUserTypes(user);

            result.ForEach(i => i.UserTypes = htmlService.DropDownModel(usertypes, ((int)i.UserTypeId).ToString()));

            this.Response.Model = result;

            return this.Response;
        }

        public ServiceResponse GetUserListModel(UserSessionModel admin, SearchUser search)
        {
            search.ReturnTotals = true;

            var query = from user in this.Db.QueryUsersViewableBySearch(admin, search, true)
                        join business in this.Db.Context.Businesses on user.BusinessId equals business.BusinessId
                        select new UserListModel
                        {
                            UserId = user.UserId,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            MiddleName = user.MiddleName,
                            LastName = user.LastName,
                            Enabled = user.Enabled,
                            AccountId = business.AccountId,
                            BusinessId = user.BusinessId,
                            DaikinCityId = business.DaikinCityId,
                            BusinessName = business.BusinessName,
                            BusinessTypeDescription = business.BusinessType.Description,
                            UserTypeId = (UserTypeEnum?)user.UserTypeId,
                            UserTypeDescription = user.UserType.Description,
                            Approved = user.Approved,
                            Rejected = user.Rejected,
                            LastLoginOn = user.LastLoginOn,
                            ApprovedOn = user.ApprovedOn,
                            RegisteredOn = user.RegisteredOn,
                            Timestamp = user.Timestamp
                        };

            this.Response.Model = query.ToList();

            return this.Response;
        }

        public ServiceResponse GetUsersViewable(UserSessionModel admin)
        {
            var search = new SearchUser();
            search.ReturnTotals = true;

            var query = from user in this.Db.QueryUsersViewableBySearch(admin, search, true)
                            //where (from project in this.Context.Projects
                            //       select project.OwnerId).Contains(user.UserId)
                        select new UserAutoComplete
                        {
                            userFullName = user.FirstName + (user.MiddleName != null ? " " + user.MiddleName : "") + " " + user.LastName
                        };
            this.Response.Model = query.ToList();

            return this.Response;
        }

        public ServiceResponse GetUserModelForRegistering()
        {
            return GetUserModel(null, new UserModel() { IsRegistering = true }, true);
        }

        public ServiceResponse GetUserModel(UserSessionModel user, UserModel templateModel, bool isEditing = false)
        {
            UserModel model = null;
            if (templateModel != null)
            {
                var userId = templateModel.UserId;

                if (userId.HasValue)
                {
                    var query = from u in this.Db.QueryUserViewableByUserId(user, userId.Value)
                                select
                                new UserModel
                                {
                                    UserId = u.UserId,
                                    Email = u.Email,
                                    FirstName = u.FirstName,
                                    MiddleName = u.MiddleName,
                                    LastName = u.LastName,
                                    Enabled = u.Enabled,
                                    Business = new BusinessModel { BusinessId = u.BusinessId },
                                    // AddressId = user.AddressId,
                                    //ContactId = user.ContactId,
                                    ShowPricing = u.ShowPricing,
                                    UseBusinessAddress = u.UseBusinessAddress,
                                    UserTypeId = (UserTypeEnum?)u.UserTypeId,
                                    Approved = u.Approved,
                                    Timestamp = u.Timestamp,
                                    Address = new AddressModel
                                    {
                                        AddressId = u.AddressId,
                                    },
                                    Contact = new ContactModel
                                    {
                                        ContactId = u.ContactId,
                                    }
                                };



                    model = query.FirstOrDefault();

                    //Get Parent Business if available // Moved to GetBusinessModel()
                    //var businessLink = this.Db.BusinessLinkQueryByBusinessId(model.Business.BusinessId).FirstOrDefault();
                    //if (businessLink != null)
                    //{
                    //    SearchBusiness businessSearch = new SearchBusiness
                    //    {
                    //        BusinessId = businessLink.ParentBusinessId,
                    //        ReturnTotals = true
                    //    };
                    //    var resp = businessService.GetBusinessModel(user, businessSearch);
                    //    if (resp != null && resp.IsOK && resp.Model != null)
                    //    {
                    //        var parentBusiness = resp.Model as BusinessModel;
                    //        model.Business.ParentBusinessId = (long)parentBusiness.BusinessId;
                    //        model.Business.ParentBusinessName = parentBusiness.BusinessName;
                    //    }

                    //}

                    if (model == null)
                    {
                        this.Response.AddError(Resources.DataMessages.DM004);
                    }
                }
            }

            model = model ?? new UserModel();

            var userTypeChanged = false;

            // Setup from templateModel
            if (templateModel != null)
            {
                //if (model.UserTypeId != templateModel.UserTypeId) userTypeChanged = true;   this line causing the problem, comment by aaron
                if (templateModel.UserTypeId.HasValue) model.UserTypeId = templateModel.UserTypeId;
                model.IsRegistering = templateModel.IsRegistering;
            }

            // Load Model Children
            addressService.BeginPropertyReference(this, "Address");
            model.Address = addressService.GetAddressModel(user, model.Address);
            addressService.EndPropertyReference();

            contactService.BeginPropertyReference(this, "Contact");
            model.Contact = contactService.GetContactModel(user, model.Contact);
            contactService.EndPropertyReference();

            businessService.BeginPropertyReference(this, "Business");
            model.Business = businessService.GetBusinessModel(user, model.Business.BusinessId, true).Model as BusinessModel;
            businessService.EndPropertyReference();

            FinaliseModel(user, model, isEditing, userTypeChanged);

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetUserModel(UserSessionModel user, long? userId, bool isRegistering, bool isEditing = false)
        {
            return GetUserModel(user, new UserModel() { UserId = userId, IsRegistering = isRegistering }, isEditing);
        }

        #endregion

        #region Finalise Model

        public void FinaliseModel(UserSessionModel admin, UserModel model, bool isEditing = false, bool userTypeChanged = false)
        {
            businessService.FinaliseModel(admin, model.Business, isEditing);

            addressService.FinaliseModel(model.Address);

            if (!model.IsRegistering)
            {
                // Dropdowns 
                model.Businesses = htmlService.DropDownModelBusinesses(admin, model.Business.BusinessId);

                model.UserTypes = htmlService.DropDownModelUserTypes(admin, model.UserTypeId);

                // Permissions

                var id = model.UserId ?? 0;
                var businessId = (model.Business != null && model.Business.BusinessId.HasValue) ? model.Business.BusinessId.Value : 0;

                if (!model.Approved.HasValue || !model.Approved.Value)
                {

                    // Pull from business defaults
                    model.CityAreas = htmlService.CheckBoxListModelBusinessPermissions(admin, businessId, model.CityAreas, PermissionTypeEnum.CityArea);
                    model.Brands = htmlService.CheckBoxListModelBusinessPermissions(admin, businessId, model.Brands, PermissionTypeEnum.Brand);
                    model.ProductFamilies = htmlService.CheckBoxListModelBusinessPermissions(admin, businessId, model.ProductFamilies, PermissionTypeEnum.ProductFamily);
                    model.Tools = htmlService.CheckBoxListModelBusinessPermissions(admin, businessId, model.Tools, PermissionTypeEnum.Tool);
                }
                else
                {
                    model.CityAreas = htmlService.CheckBoxListModelUserPermissions(admin, id, model.CityAreas, PermissionTypeEnum.CityArea, isEditing);
                    model.Brands = htmlService.CheckBoxListModelUserPermissions(admin, id, model.Brands, PermissionTypeEnum.Brand, isEditing);
                    model.ProductFamilies = htmlService.CheckBoxListModelUserPermissions(admin, id, model.ProductFamilies, PermissionTypeEnum.ProductFamily, isEditing);
                    model.Tools = htmlService.CheckBoxListModelUserPermissions(admin, id, model.Tools, PermissionTypeEnum.Tool, isEditing);
                }

                if (userTypeChanged || !model.Approved.HasValue || !model.Approved.Value)
                {
                    model.SystemAccesses = htmlService.CheckBoxListModelUserPermissions(admin, businessId, model.UserTypeId, model.SystemAccesses, PermissionTypeEnum.SystemAccess, isEditing);
                }
                else
                {
                    model.SystemAccesses = htmlService.CheckBoxListModelUserPermissions(admin, id, model.SystemAccesses, PermissionTypeEnum.SystemAccess, isEditing);
                }
            }
        }

        #endregion

        #region Post Requests

        public ServiceResponse ChangeUserStatus(UserSessionModel admin, UserModel model)
        {
            this.Db.ReadOnly = false;

            var entity = GetEntity(admin, model);

            if (this.Response.IsOK)
            {
                if (model.Approved.HasValue) entity.Approved = model.Approved.Value;

                if (model.Enabled.HasValue) entity.Enabled = model.Enabled.Value;

                if (model.Rejected.HasValue) entity.Rejected = model.Rejected.Value;

                entity.UserTypeId = (model.UserTypeId.HasValue) ? model.UserTypeId.Value : entity.UserTypeId;

                ApplyBusinessRules(admin, entity);

                // Make the property level messages page level
                this.Response.Messages.Items.Where(m => !string.IsNullOrEmpty(m.Key)).ToList()
                    .ForEach(m => { m.Key = ""; m.Text = "'" + Helpers.DisplayName(entity) + "'. " + m.Text; });
            }

            if (this.Response.IsOK)
            {
                base.SaveToDatabase(model, entity, string.Format("User '{0}'", Helpers.DisplayName(entity)));
            }

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetUserRegistrationEmailModel(UserModel model)
        {
            this.Response = new ServiceResponse();

            var emailModel = new SendEmailModel();

            if (model.IsRegistering)
            {
                try
                {
                    List<UserListModel> groupowners = null;
                    IQueryable<User> query = null;
                    IQueryable<User> suQuery = null;

                    // Send to group owners if account registered. 
                    // TODO: Move this logic to one spot
                    if (string.IsNullOrEmpty(model.Business.AccountId) == false)
                    {
                        query = this.Db.QueryDaikinGroupOwnersByAccountId(model.Business.AccountId);
                        suQuery = this.Db.QueryUsersApprovedBusinessSuperUser(model.Business.AccountId);

                    }
                    else if (!string.IsNullOrEmpty(model.Business.DaikinCityId))
                    {
                        query = this.Db.QueryDaikinGroupOwnersByAccountId(model.Business.DaikinCityId);
                        suQuery = this.Db.QueryUsersApprovedBusinessSuperUser(model.Business.DaikinCityId);
                    }

                    //else if (string.IsNullOrEmpty(model.Business.BusinessName) == false)
                    //{
                    //    query = this.Db.QueryDaikinGroupOwnersByBusinessName(model.Business.BusinessName);
                    //}

                    if (query != null)
                    {
                        groupowners = (from user in query
                                       select new UserListModel
                                       {
                                           UserId = user.UserId,
                                           Email = user.Email,
                                           FirstName = user.FirstName,
                                           MiddleName = user.MiddleName,
                                           LastName = user.LastName
                                       }).ToList();

                        groupowners.ForEach(e => emailModel.To.Add(new MailAddress(e.Email, e.DisplayName)));
                    }

                    if (suQuery != null)
                    {
                        var superUsers = (from u in suQuery
                                          select new UserListModel
                                          {
                                              UserId = u.UserId,
                                              Email = u.Email,
                                              FirstName = u.FirstName,
                                              MiddleName = u.MiddleName,
                                              LastName = u.LastName
                                          }).ToList();

                        superUsers.ForEach(e => emailModel.To.Add(new MailAddress(e.Email, e.DisplayName)));
                    }
                }
                catch
                {
                }

                emailModel.To.Add(new MailAddress(Utilities.Config("dpo.sales.team.email"), "Daikin Sales Team"));

                if (Utilities.IsLive())
                {
                    emailModel.To.Add(new MailAddress("Deepak.Mandloi@daikincomfort.com", "Mandloi, Deepak"));
                }
                else {
                    emailModel.To.Add(new MailAddress("huy.nguyen@daikincomfort.com", "Nguyen, Huy"));
                }

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "DPO Registration");
            }

            emailModel.UserFirstName = model.FirstName;
            emailModel.UserLastName = model.LastName;
            emailModel.BusinessName = model.Business.BusinessName;
            emailModel.BusinessTypeDescription = model.Business.BusinessTypeDescription;

            this.Response.Model = emailModel;

            return this.Response;

        }

        public ServiceResponse GetUserEmailModel(UserSessionModel user, UserModel model)
        {
            this.Response = new ServiceResponse();

            var emailModel = new SendEmailModel();

            // Send to group owners if account registered.
            if (model.UserId.HasValue)
            {
                var users = (from u in this.Db.QueryUserViewableByUserId(user, model.UserId.Value)
                             select new UserListModel
                             {
                                 UserId = u.UserId,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName
                             }).ToList();

                users.ForEach(e => emailModel.To.Add(new MailAddress(e.Email, e.DisplayName)));
            }

            emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.from"), "Daikin Office Project");


            this.Response.Model = emailModel;

            return this.Response;

        }

        public User GetUserByEmail(string email) {
            var user = this.Db.UserQueryByEmail(email).FirstOrDefault();
            return user;
        }

        public ServiceResponse SetupDefaultPermissions(UserSessionModel admin, UserModel model)
        {
            this.Db.ReadOnly = false;
            this.Response = new ServiceResponse();

            if (model == null)
            {
                return this.Response;
            }

            var businessId = model.Business != null && model.Business.BusinessId.HasValue ? model.Business.BusinessId : (long?)null;
            var userId = model.UserId;

            if (businessId != null && userId != null)
            {
                var busId = businessId.Value;
                var uId = userId.Value;

                Db.ReplacePermissions(EntityEnum.Business, busId, EntityEnum.User, uId, PermissionTypeEnum.Brand);
                Db.ReplacePermissions(EntityEnum.Business, busId, EntityEnum.User, uId, PermissionTypeEnum.CityArea);
                Db.ReplacePermissions(EntityEnum.Business, busId, EntityEnum.User, uId, PermissionTypeEnum.ProductFamily);
                Db.ReplacePermissions(EntityEnum.Business, busId, EntityEnum.User, uId, PermissionTypeEnum.Tool);

                Db.SaveChanges();
            }

            return this.Response;
        }

        public ServiceResponse PostModel(UserSessionModel admin, UserModel model)
        {
            this.Db.ReadOnly = false;

            try
            {
                User entity = null;

                // Validate Model 
                RulesOnValidateModel(model);

                // Map to Entity
                if (this.Response.IsOK)
                {
                    entity = ModelToEntity(admin, model);
                }

                // Validate Entity
                if (this.Response.IsOK)
                {
                    ApplyBusinessRules(admin, entity);
                }

                // Do this last, painful for users if not last
                if (this.Response.IsOK)
                {
                    if (model.IsRegistering)
                    {
                        this.Response.PropertyReference = "";

                        Validation.IsText(this.Response.Messages, model.Password, "Password", "Password", 5, 50, true);

                        Validation.IsText(this.Response.Messages, model.ConfirmPassword, "ConfirmPassword", "Confirm Password", 50, true);

                        Validation.IsPasswordConfirmed(this.Response.Messages, model.Password, model.ConfirmPassword, "ConfirmPassword");
                    }
                }

                if (this.Response.IsOK)
                {
                    var dbSelection = this.Context.Permissions.Where(p => p.ObjectId == entity.UserId
                             && p.PermissionTypeId == PermissionTypeEnum.SystemAccess).Select(r => r).ToList();

                    base.SaveToDatabase(model, entity, string.Format("User '{0}'", Helpers.DisplayName(entity)));
                    model.UserId = entity.UserId;

                    var permissions = CheckBoxListModel.ToPermissionListModel(model.SystemAccesses);

                    this.Db.UpdatePermissionAudit(EntityEnum.UserType, entity, permissions,
                                     PermissionTypeEnum.SystemAccess, admin, dbSelection);

                    // If registered successfully prevent message from being sent
                    // Reasom is that they remain in temp data and show up on sign in. 0000067: Sign In window acts funky
                    if (model.IsRegistering)
                    {
                        this.Response.Messages.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                this.Response.AddError(e.Message);
                this.Response.Messages.AddAudit(e);
            }

            FinaliseModel(admin, model);

            this.Response.Model = model;

            return this.Response;
        }

        #endregion

        #region Post Model To Entity

        public User ModelToEntity(UserSessionModel admin, UserModel model)
        {
            var entity = GetEntity(admin, model);

            if (this.Response.HasError) return null;

            entity.Email = Utilities.Trim(model.Email);

            entity.FirstName = Utilities.Trim(model.FirstName);

            entity.MiddleName = Utilities.Trim(model.MiddleName);

            entity.LastName = Utilities.Trim(model.LastName);

            entity.MiddleName = Utilities.Trim(model.MiddleName);

            entity.LastName = Utilities.Trim(model.LastName);

            entity.UseBusinessAddress = model.UseBusinessAddress;

            entity.IsRegistering = model.IsRegistering; // not saved in DB but only for reference

            entity.IsNewBusiness = model.ExistingBusiness == ExistingBusinessEnum.New; // not saved in DB but only for reference

            contactService.BeginPropertyReference(this, "Contact");
            entity.Contact = contactService.ModelToEntity(model.Contact);
            contactService.EndPropertyReference();

            //Check address if you are not using business address
            if (model.UseBusinessAddress == false || entity.IsNewBusiness)
            {
                addressService.BeginPropertyReference(this, "Address");
                entity.Address = addressService.ModelToEntity(model.Address);
                addressService.EndPropertyReference();
            }

            if (entity.IsRegistering)
            {
                entity.Salt = Crypto.GenerateSalt();

                entity.Password = Crypto.Hash(model.Password, entity.Salt);

                if (!entity.IsNewBusiness)
                {
                    var dweb = new DaikinServices();

                    // Lookup by CRM Account Id and Daikin City Id
                    entity.Business = this.Db.GetBusinessByAccountId(model.Business.AccountId);
                    if (entity.Business == null
                        && !string.IsNullOrEmpty(model.Business.DaikinCityId))
                    {
                        entity.Business = this.Db.GetBusinessByAccountId(model.Business.DaikinCityId);
                    }

                    if (entity.Business == null)
                    {
                        // Create record so that the business rules can be applied and correct error message produced
                        // i.e account verify will take place in the business rules
                        entity.Business = new Business
                        {
                            BusinessTypeId = (model.Business.BusinessTypeId == null) ? BusinessTypeEnum.Other : (BusinessTypeEnum)model.Business.BusinessTypeId,
                            AccountId = model.Business.AccountId,
                            BusinessName = model.Business.BusinessName
                        };
                    }

                    this.Response.PropertyReference = "";
                }
                else// is New Business
                {
                    //Copy address to the business
                    model.Business.Address = model.Address;

                    model.Business.BusinessId = null; // clear to make sure new record is added

                    businessService.BeginPropertyReference(this, "Business");
                    entity.Business = businessService.ModelToEntity(admin, model.Business, false);
                    if (model.Business.ParentBusinessId != null && model.Business.ParentBusinessId != 0 && entity.Business.BusinessId != 0)
                    {
                        //add BusinessLinks record
                        BusinessLinkModel businessLinkModel = new BusinessLinkModel()
                        {
                            BusinessId = (long)entity.Business.BusinessId,
                            ParentBusinessId = model.Business.ParentBusinessId
                        };
                        businessLinkService.ModelToEntity(businessLinkModel);
                    }
                    businessService.EndPropertyReference();

                    // Setup default business permissions
                    if (!string.IsNullOrEmpty(entity.Business.BusinessId.ToString()) &&
                        !string.IsNullOrEmpty(entity.Business.BusinessTypeId.ToString()))
                    {
                        var bId = entity.Business.BusinessId;
                        var btId = entity.Business.BusinessTypeId;
                        Db.ReplacePermissions(EntityEnum.BusinessType, (long)btId, EntityEnum.Business, bId, PermissionTypeEnum.Brand);
                        Db.ReplacePermissions(EntityEnum.BusinessType, (long)btId, EntityEnum.Business, bId, PermissionTypeEnum.CityArea);
                        Db.ReplacePermissions(EntityEnum.BusinessType, (long)btId, EntityEnum.Business, bId, PermissionTypeEnum.ProductFamily);
                        Db.ReplacePermissions(EntityEnum.BusinessType, (long)btId, EntityEnum.Business, bId, PermissionTypeEnum.Tool);
                    }
                }
            }
            else
            {
                // Own user cant change user type.
                if (admin.UserId != model.UserId)
                {
                    entity.UserTypeId = (UserTypeEnum)model.UserTypeId;
                }

                entity.BusinessId = model.Business.BusinessId;

                if (entity.BusinessId.HasValue) // Need business for business permission rules
                {
                    this.Db.GetBusinessByBusinessId(entity.BusinessId.Value);
                }

                if (model.Business.ParentBusinessId != 0) {
                    businessService.UpdateParentBusiness(model.Business);
                }

                if (this.Context.Entry(entity).State == EntityState.Added)
                {
                    entity.Salt = Crypto.GenerateSalt();

                    entity.Password = Crypto.Hash("%^$ggtfr", entity.Salt);

                    entity.Enabled = true;
                }

                entity.ShowPricing = model.ShowPricing;

                entity.Enabled = model.Enabled ?? entity.Enabled;

                entity.Approved = model.Approved ?? entity.Approved;

                if (model.UserId != admin.UserId) // cant update your own access
                {
                    permissionService.ApplyBusinessRules(entity, model, admin);
                }
            }

            return entity;
        }

        #endregion

        private User GetEntity(UserSessionModel admin, UserModel model)
        {
            var entity = model.UserId.HasValue ? this.Db.QueryUserViewableByUserId(admin, model.UserId).FirstOrDefault() : Db.UserCreate(model.Business.BusinessId ?? 0, 0, UserTypeEnum.NotSet);

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM004);
            }

            return entity;
        }
    }
}