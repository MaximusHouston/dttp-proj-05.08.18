//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
namespace DPO.Common
{
    public class BusinessModel : PageModel, IAddressContactModel
    {
        public long? BusinessId { get; set; }
        public string DaikinCityId { get; set; }
        public bool ShowPricing { get; set; }
        public bool Enabled { get; set; }
        public string BusinessName { get; set; }
        public string AccountId { get; set; }
        public bool CommissionSchemeAllowed { get; set; }
        public AddressModel Address { get; set; }
        public ContactModel Contact { get; set; }
        public int? BusinessTypeId { get; set; }
        public string BusinessTypeDescription { get; set; }
        public bool IsRegistering { get; set; }
        public DropDownModel BusinessTypes { get; set; }
        public CheckBoxListModel CityAreas { get; set; }
        public CheckBoxListModel ProductFamilies { get; set; }
        public CheckBoxListModel Brands { get; set; }
        public CheckBoxListModel Tools { get; set; }
        public string AccountOwnerEmail { get; set; }
        public string AccountManagerEmail { get; set; }
        public string ERPAccountId { get; set; }
        public long ParentBusinessId { get; set; }
        public string ParentBusinessName { get; set; }
        public bool? IsVRVPro { get; set; }
        public bool? IsDaikinComfortPro { get; set; }
        public bool? IsDaikinBranch { get; set; }
    }
}
