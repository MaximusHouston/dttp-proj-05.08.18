using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DPO.Common
{
    public class CommissionRequestModel : PageModel
    {
      public long? ProjectId { get; set; }

        public long? QuoteId { get; set; }

        public ProjectModel Project { get; set; }

        public BusinessModel Business { get; set; }

        public UserModel User { get; set; }

        public bool HasQuote { get { return (this.Project != null && this.Project.ActiveQuoteSummary != null); } }

        public QuoteListModel Quote { get { return ((HasQuote) ? this.Project.ActiveQuoteSummary : new QuoteListModel()); } }

        public List<QuoteItemListModel> QuoteItems { get; set; }

        public string BusinessName { get; set; }

        public long? BusinessId { get; set; }

        public string ProjectOwner { get; set; }

        public long? ProjectOwnerId { get; set; }

        public long? CommissionRequestId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? RequestedOn { get; set; } 

        public byte? CommissionRequestStatusTypeId { get; set; }
        public DropDownModel CommissionRequestStatusTypes { get; set; }
        public string CommissionRequestStatusTypeDescription { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? CommissionRequestStatusModifiedOn { get; set; }


        public long CommissionRequestStatusModifiedById { get; set; }
        public string CommissionRequestStatusModifiedBy { get; set; }

        public byte? SystemBasisDesignTypeId { get; set; }
        public DropDownModel SystemBasisDesignTypes { get; set; }
        public string SystemBasisDesignTypeDescription { get; set; }

        public byte? ZoneStrategyTypeId { get; set; }
        public DropDownModel ZoneStrategyTypes { get; set; }
        public string ZoneStrategyTypeDescription { get; set; }

        public byte? BrandSpecifiedTypeId { get; set; }
        public DropDownModel BrandSpecifiedTypes { get; set; }
        public string BrandSpecifiedTypeDescription { get; set; }

        public byte? BrandApprovedTypeId { get; set; }
        public DropDownModel BrandApprovedTypes { get; set; }
        public string BrandApprovedTypeDescription { get; set; }

        public bool HasCompetitorPrice { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? CompetitorPrice { get; set; }

        public byte? UserTypeId { get; set; }
        public DropDownModel UserTypes { get; set; }
        public string UserTypeDescription { get; set; }

        public bool IsConfidentCompetitorQuote { get; set; }

        //Deprecated
        //public bool ApprovalAssuresOrder { get; set; }

        public bool HasCompetitorQuote { get; set; }
        public bool HasCompetitorLineComparsion { get; set; }

        //new props
        public byte? DaikinEquipmentAtAdvantageTypeId { get; set; }
        public DropDownModel DaikinEquipmentAtAdvantageTypes { get; set; }
        public string DaikinEquipmentAtAdvantageDescription { get; set; }

        public byte? ProbabilityOfCloseTypeId { get; set; }
        public DropDownModel ProbabilityOfCloseTypes { get; set; }
        public string ProbabilityOfCloseTypeDescription { get; set; }

        public byte? WinLossConditionTypeId { get; set; }
        public DropDownModel WinLossConditionTypes { get; set; }
        public string WinLossConditionTypeDescription { get; set; }

        public byte? FundingTypeId { get; set; }
        public DropDownModel FundingTypes { get; set; }
        public string FundingTypeDescription { get; set; }

        public byte? CustomerTypeId { get; set; }
        public DropDownModel CustomerTypes { get; set; }
        public string CustomerTypeDescription { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? OrderPlannedFor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? OrderDeliveryDate { get; set; }

        public string Notes { get; set; }

        public string ResponseNotes { get; set; }

        public HttpPostedFileBase CompetitorQuoteFile { get; set; }

        public string CompetitorQuoteFileName { get; set; }

        public HttpPostedFileBase CompetitorLineComparsionFile { get; set; }

        public string CompetitorLineComparsionFileName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal StartUpCosts { get; set; }

        // Commission
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionPercentage { get; set; }

        public QuoteCalculationModel ApprovedTotals { get; set; }
        public QuoteCalculationModel StandardTotals { get; set; }

        public QuoteCalculationModel ApprovedTotalsCommission { get; set;}
        public QuoteCalculationModel StandardTotalsCommission { get; set;}

        public bool ShouldSendEmail { get; set; }

        public string EmailsList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionPercentageSplit { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionPercentageSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionPercentageUnitary { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionPercentageUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionPercentageLCPackage { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionPercentageLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionSplit { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionPercentageVRV { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionPercentageVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionVRV { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionUnitary { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionLCPackage { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedMultiplierVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedMultiplierSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedMultiplierUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedMultiplierLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedMultiplier { get; set; }
 
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedMultiplierVRV { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedMultiplierSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedMultiplierUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedMultiplierLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedMultiplier { get; set; }

       
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialMultiplierVRV { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialMultiplierSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialMultiplierUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialMultiplierLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValueVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValueSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValueUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValueLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedCommissionTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedCommissionTotal { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal TotalNetVRV { get; set; }
        
        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal TotalNetSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal TotalNetUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal TotalNetLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValue { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedNetMaterialValueMultiplier { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal TotalNet { get; set; }

        public decimal? TotalNetOther { get; set; }

        public bool IsValidEmails { get; set; }

        public CommissionRequestModel()
        {
            IsValidEmails = true;
            InvalidEmails = new List<string>();
        }

        public List<string> InvalidEmails { get; set; }

        //public DPO.Common.Enumerations.FundingTypeEnum FundingType { get; set; }
        //public DPO.Common.Enumerations.CustomerTypeEnum CustomerType { get; set; }

        public string RepPhoneNumber { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string RepEmail { get; set; }
        public string SellerEmail { get; set; }
        public string RepFaxNumber { get; set; }
        public string SellerFaxNumber { get; set; }

        public bool IsCommissionCalculation { get; set; }

        //these ptoperties used to take to ProjectEdit Tabs

        public string projectLocation { get; set; }
        public string sellerLocaton { get; set; }
        public string customerLocation { get; set; }

        public decimal ThirdPartyEquipmentCosts { get; set; }

        public decimal TotalRevised { get; set; }

        public decimal TotalCommissionUnitary { get; set; }
        public decimal TotalListPriceUnitary { get; set; }
        
    }
}
