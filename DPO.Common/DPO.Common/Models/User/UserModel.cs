
using System;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class UserModel : SearchUser, IUser, IAddressContactModel
    {
        public UserModel()
        {
            Address = new AddressModel();
            Contact = new ContactModel();
            Business = new BusinessModel();
            Enabled = true;
        }

        public string AccountId { get; set; }
        public AddressModel Address { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? ApprovedOn { get; set; }

        public CheckBoxListModel Brands { get; set; }
        public BusinessModel Business { get; set; }
        //public long ParentBusinessId { get; set; }
        //public string ParentBusinessName { get; set; }
        public DropDownModel Businesses { get; set; }
        public CheckBoxListModel CityAreas { get; set; }
        public string ConfirmPassword { get; set; }
        public ContactModel Contact { get; set; }
        public string DisplayName { get { return Helpers.DisplayName(this); } }

        public string EngineerBusinessName { get; set; }

        public ExistingBusinessEnum? ExistingBusiness { get; set; }
        // User info
        public string FirstName { get; set; }
        public UserGroupsModel Groups { get; set; }

        //modify by aaron
        public bool IsAccountAndUserTypeId { get; set; }

        public bool IsRegistering { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Password { get; set; }

        public CheckBoxListModel ProductFamilies { get; set; }

        public bool ShowPricing { get; set; }

        public CheckBoxListModel SystemAccesses { get; set; }

        // User Edit
        public CheckBoxListModel Tools { get; set; }

        public bool UseBusinessAddress { get; set; }

        public DropDownModel UserTypes { get; set; }

        public bool ValidateAddress
        {
            get
            {
                return (this.ExistingBusiness == ExistingBusinessEnum.New && this.IsRegistering) || this.UseBusinessAddress == false;
            }
        }
        // Registration
    }

}
