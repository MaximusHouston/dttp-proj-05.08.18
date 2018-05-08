//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class UserListModel : PageModel, IUser
    {
        public UserListModel()
        {
            this.UserTypeId = UserTypeEnum.NotSet;
        }

        public long? UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public bool IsGroupOwner { get; set; }
        public string GroupName { get; set; }
        public long GroupId { get; set; }

        public string DisplayName
        {
            get
            {
                return Helpers.DisplayName(this);
            }
        }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? RegisteredOn { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? ApprovedOn { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? LastLoginOn { get; set; }

        public string AccountId { get; set; }
        public string DaikinCityId { get; set; }

        public long? BusinessId { get; set; }
        public string BusinessName { get; set; }

        public string BusinessTypeDescription { get; set; }

        [DefaultValue(UserTypeEnum.NotSet)]
        public UserTypeEnum? UserTypeId { get; set; }

        public string UserTypeDescription { get; set; }

        public bool Approved { get; set; }

        public bool Rejected { get; set; }

        public bool Enabled { get; set; }

        public DropDownModel UserTypes { get; set; }
    }
}
