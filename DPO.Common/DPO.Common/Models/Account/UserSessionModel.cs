//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace DPO.Common
{
    public class UserSessionModel : IUser
    {
        private UserSettingsModel mUserSettings = new UserSettingsModel();

        public UserSessionModel()
            : base()
        {
        }

        public long? BasketQuoteId { get; set; }
        public long? BusinessId { get; set; }
        public BusinessTypeEnum BusinessTypeId { get; set; }
        public string BusinessLogoUrl { get; set; }
        public string BusinessName { get; set; }
        public List<int> CityAccesses { get; set; }
        public bool CommissionSchemeAllowed { get; set; }
        public string DefaultPageUrl { get; set; }
        public string DisplayName { get { return Helpers.DisplayName(this); } }

        public string DisplaySettings { get; set; }
        public int DisplaySettingsPageSize { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string FirstName { get; set; }
        public long? GroupId { get; set; }

        public bool HasAccessToCMS
        {
            get
            {
                return (this.HasAccess(SystemAccessEnum.ContentManagementApplicationBuildings)
                    || this.HasAccess(SystemAccessEnum.ContentManagementApplicationProducts)
                    || this.HasAccess(SystemAccessEnum.ContentManagementCommsCenter)
                    || this.HasAccess(SystemAccessEnum.ContentManagementFunctionalBuildings)
                    || this.HasAccess(SystemAccessEnum.ContentManagementHomeScreen)
                    || this.HasAccess(SystemAccessEnum.ContentManagementLibrary)
                    || this.HasAccess(SystemAccessEnum.ContentManagementProductFamilies)
                    || this.HasAccess(SystemAccessEnum.ContentManagementTools));
            }
        }

        public long? ImportProjectId { get; set; }
        public long? ImportQuoteId { get; set; }

        public bool isDaikinUser
        {
            get
            {
                return (this.UserTypeId == UserTypeEnum.DaikinAdmin || this.UserTypeId == UserTypeEnum.DaikinEmployee || this.UserTypeId == UserTypeEnum.DaikinSuperUser || this.UserTypeId == UserTypeEnum.Systems);
            }
        }

        public bool IsGroupOwner { get; set; }
        public string LastName { get; set; }

        public SystemAccessEnum[] ManagmentAccess
        {
            get
            {
                return
                    new SystemAccessEnum[]
                  {
                      SystemAccessEnum.ViewBusiness,
                      SystemAccessEnum.ViewUsers,
                      SystemAccessEnum.ManageGroups
                  };
            }
        }

        public string MiddleName { get; set; }
        public bool ShowPrices { get; set; }
        public List<SystemAccessEnum> SystemAccesses { get; set; }
        public DateTime? Timestamp { get; set; }
        public long UserId { get; set; }

        public UserSettingsModel UserSettings
        {
            get
            {
                if (this.mUserSettings == null)
                {
                    this.mUserSettings = new UserSettingsModel();
                }

                return this.mUserSettings;
            }
            set
            {
                this.mUserSettings = value;
            }
        }

        public string UserTypeDescription { get { return UserTypeId.ToString(); } }
        public UserTypeEnum UserTypeId { get; set; }

        public bool HasAccess(SystemAccessEnum? access)
        {
            return (access == null) ? true : HasAccess(new SystemAccessEnum[] { access.Value });
        }

        public bool HasAccess(SystemAccessEnum[] accesses)
        {
            if (accesses == null || accesses.Length == 0) return true;

            if (SystemAccesses == null) return false;

            bool hasaccess = SystemAccesses.Any(s => HasAccess(s));

            return hasaccess;
        }

        public bool HasAccess(SystemAccessEnum accessId)
        {
            // if (accessId == SystemAccessEnum.EditProject && this.ShowPrices == false) return false;

            if (this.SystemAccesses == null)
            {
                return false;
            }

            bool hasaccess = SystemAccesses.Any(s => s == accessId);

            return hasaccess;
        }

        public bool HasAccessTool(ToolAccessEnum toolId) {
            bool hasAccess = false;

            if (this.ToolAccesses == null)
            {
                return false;
            }
            else{
                //hasAccess == ToolAccesses.Any(t => t.ToolId == toolId);

                foreach (ToolModel tool in ToolAccesses) {
                    if ((ToolAccessEnum)tool.ToolId == toolId)
                    {
                        hasAccess = true;
                    }
                }
                
            }
           
            return hasAccess;
        }

        public List<ToolModel> ToolAccesses { get; set; }
        public List<ProductFamilyModel> ProductFamilyAccesses { get; set; }
        public List<BrandModel> BrandAccesses { get; set; }
    }
}