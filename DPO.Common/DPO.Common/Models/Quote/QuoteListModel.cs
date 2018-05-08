using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    [JsonObject(IsReference = false)]
    public class QuoteListModel
    {
        public bool Active { get; set; }
        public bool Alert { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedCommissionPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentageSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentageVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentageUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentageLCPackage { get; set; }

        public bool AwaitingDiscountRequest { get; set; }
        public bool AwaitingCommissionRequest { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal CommissionPercentage { get; set; }

        public bool Deleted { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentageSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentageVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentageUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentageLCPackage { get; set; }

        public bool HasDAR { get; set; }
        public bool HasCOM { get; set; }

        public byte DiscountRequestStatusTypeId { get; set; }
        public byte CommissionRequestStatusTypeId { get; set; }

        public bool HasPendingDar { get; set; }
        public bool IsGrossMargin { get; set; }
        public decimal ItemCount { get; set; }
        public long? ProjectId { get; set; }
        public long? QuoteId { get; set; }
        public bool RecalculationRequired { get; set; }
        public long? Revision { get; set; }
        public int? SplitCount { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? Timestamp { get; set; }

        public string Title { get; set; }
        public decimal TotalCountSplit { get; set; }

        public decimal TotalCountVRV { get; set; }

        public decimal TotalCountVRVIndoor { get; set; }

        public decimal TotalCountVRVOutdoor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalFreight { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalListLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalMisc { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalNet { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalNetCommission { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalNetNonCommission { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalSell { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalSellSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalSellVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalSellUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? TotalSellLCPackage { get; set; }

        public decimal? TotalVRVODUAccessoryCount { get; set; }
        public decimal? TotalVRVODUCount { get; set; }
        public int? VRVOutdoorCount { get; set; }
        /* DC added */
        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? CommissionAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? NetMultiplierValue { get; set; }

        public bool IsCommission { get; set; }

        public bool CommissionConvertNo { get; set; }
        public bool CommissionConvertYes { get; set; }

        public long OrderId { get; set; }

        public byte OrderStatusTypeId { get; set; }
        
    }
}