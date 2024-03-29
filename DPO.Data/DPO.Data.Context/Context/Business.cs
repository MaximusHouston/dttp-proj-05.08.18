
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DPO.Data
{

using System;
    using System.Collections.Generic;
    using DPO.Common;
    
public partial class Business
 : IConcurrency 
{

    public Business()
    {

        this.AccountMultipliers = new HashSet<AccountMultiplier>();

        this.Users = new HashSet<User>();

        this.BusinessLinks = new HashSet<BusinessLink>();

        this.BusinessLinks1 = new HashSet<BusinessLink>();

        this.Orders = new HashSet<Order>();

    }


    public long BusinessId { get; set; }

    public string BusinessName { get; set; }

    public string AccountId { get; set; }

    public Nullable<long> AddressId { get; set; }

    public Nullable<long> ShipToAddressId { get; set; }

    public Nullable<long> ContactId { get; set; }

    public DPO.Common.BusinessTypeEnum BusinessTypeId { get; set; }

    public bool CommissionSchemeAllowed { get; set; }

    public bool ShowPricing { get; set; }

    public decimal YearToDateSales { get; set; }

    public decimal OpenOrdersTotal { get; set; }

    public bool Enabled { get; set; }

    public System.DateTime DaikinModifiedOn { get; set; }

    public System.DateTime Timestamp { get; set; }

    public string AccountOwnerEmail { get; set; }

    public string AccountManagerEmail { get; set; }

    public string AccountOwningGroupName { get; set; }

    public string AccountManagerFirstName { get; set; }

    public string AccountManagerLastName { get; set; }

    public string AccountOwnerFirstName { get; set; }

    public string AccountOwnerLastName { get; set; }

    public string ERPAccountId { get; set; }

    public int BusinessLookupId { get; set; }

    public string DaikinCityId { get; set; }

    public Nullable<bool> IsVRVPro { get; set; }

    public Nullable<bool> IsDaikinComfortPro { get; set; }

    public Nullable<long> ModifyBy { get; set; }

    public Nullable<System.DateTime> ModifyOn { get; set; }



    public virtual ICollection<AccountMultiplier> AccountMultipliers { get; set; }

    public virtual BusinessType BusinessType { get; set; }

    public virtual Contact Contact { get; set; }

    public virtual Address Address { get; set; }

    public virtual ICollection<User> Users { get; set; }

    public virtual ICollection<BusinessLink> BusinessLinks { get; set; }

    public virtual ICollection<BusinessLink> BusinessLinks1 { get; set; }

    public virtual ICollection<Order> Orders { get; set; }

}

}
