
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
    
public partial class Quote
 : IConcurrency 
{

    public Quote()
    {

        this.DiscountRequests = new HashSet<DiscountRequest>();

        this.CommissionRequests = new HashSet<CommissionRequest>();

        this.CommissionCalculations = new HashSet<CommissionCalculation>();

        this.QuoteItems = new HashSet<QuoteItem>();

        this.QuoteItemOptions = new HashSet<QuoteItemOption>();

        this.Orders = new HashSet<Order>();

    }


    public long QuoteId { get; set; }

    public int Revision { get; set; }

    public long ProjectId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal Multiplier { get; set; }

    public decimal DiscountPercentage { get; set; }

    public bool RecalculationRequired { get; set; }

    public bool AwaitingDiscountRequest { get; set; }

    public string Notes { get; set; }

    public bool Active { get; set; }

    public decimal TotalCountCommission { get; set; }

    public decimal TotalCountNonCommission { get; set; }

    public decimal TotalCountService { get; set; }

    public decimal TotalCountMisc { get; set; }

    public decimal TotalListCommission { get; set; }

    public decimal TotalListNonCommission { get; set; }

    public decimal TotalListService { get; set; }

    public decimal TotalList { get; set; }

    public decimal TotalNetCommission { get; set; }

    public decimal TotalNetNonCommission { get; set; }

    public decimal TotalNetService { get; set; }

    public decimal TotalNet { get; set; }

    public decimal TotalMisc { get; set; }

    public decimal TotalSell { get; set; }

    public decimal TotalService { get; set; }

    public decimal TotalFreight { get; set; }

    public bool IsCommissionScheme { get; set; }

    public bool IsGrossMargin { get; set; }

    public decimal CommissionPercentage { get; set; }

    public Nullable<long> DiscountRequestId { get; set; }

    public decimal ApprovedDiscountPercentage { get; set; }

    public decimal ApprovedCommissionPercentage { get; set; }

    public bool Deleted { get; set; }

    public System.DateTime CreatedDate { get; set; }

    public System.DateTime Timestamp { get; set; }

    public int VRVOutdoorCount { get; set; }

    public int SplitCount { get; set; }

    public int RTUCount { get; set; }

    public int VRVIndoorCount { get; set; }

    public decimal TotalCountVRV { get; set; }

    public decimal TotalCountVRVIndoor { get; set; }

    public decimal TotalCountVRVOutdoor { get; set; }

    public decimal TotalCountSplit { get; set; }

    public decimal TotalListVRV { get; set; }

    public decimal TotalListSplit { get; set; }

    public decimal TotalNetVRV { get; set; }

    public decimal TotalNetSplit { get; set; }

    public decimal DiscountPercentageSplit { get; set; }

    public decimal DiscountPercentageVRV { get; set; }

    public decimal TotalCountSplitOutdoor { get; set; }

    public decimal TotalSellSplit { get; set; }

    public decimal TotalSellVRV { get; set; }

    public decimal ApprovedDiscountPercentageVRV { get; set; }

    public decimal ApprovedDiscountPercentageSplit { get; set; }

    public bool IsCommission { get; set; }

    public Nullable<long> CommissionRequestId { get; set; }

    public bool AwaitingCommissionRequest { get; set; }

    public Nullable<bool> CommissionConvertNo { get; set; }

    public Nullable<bool> CommissionConvertYes { get; set; }

    public Nullable<bool> AwaitingOrder { get; set; }

    public Nullable<decimal> TotalNetUnitary { get; set; }

    public Nullable<decimal> TotalListUnitary { get; set; }

    public Nullable<decimal> TotalCountUnitary { get; set; }

    public Nullable<decimal> ApprovedDiscountPercentageUnitary { get; set; }

    public Nullable<decimal> TotalSellUnitary { get; set; }

    public Nullable<decimal> DiscountPercentageUnitary { get; set; }

    public Nullable<decimal> TotalListLCPackage { get; set; }

    public Nullable<decimal> TotalNetLCPackage { get; set; }

    public Nullable<decimal> TotalSellLCPackage { get; set; }

    public Nullable<decimal> DiscountPercentageLCPackage { get; set; }

    public Nullable<decimal> ApprovedDiscountPercentageLCPackage { get; set; }

    public Nullable<long> ModifyBy { get; set; }

    public Nullable<System.DateTime> ModifyOn { get; set; }



    public virtual Project Project { get; set; }

    public virtual ICollection<DiscountRequest> DiscountRequests { get; set; }

    public virtual ICollection<CommissionRequest> CommissionRequests { get; set; }

    public virtual ICollection<CommissionCalculation> CommissionCalculations { get; set; }

    public virtual ICollection<QuoteItem> QuoteItems { get; set; }

    public virtual ICollection<QuoteItemOption> QuoteItemOptions { get; set; }

    public virtual ICollection<Order> Orders { get; set; }

}

}
