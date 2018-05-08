using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;


namespace DPO.Common
{
    public class DiscountRequestModel : PageModel
    {
        public long? ProjectId { get; set; }

        public long? QuoteId { get; set; }

        public ProjectModel Project { get; set; }

        public bool HasQuote { get { return (this.Project != null && this.Project.ActiveQuoteSummary != null); } }

        public QuoteListModel Quote { get { return ((HasQuote) ? this.Project.ActiveQuoteSummary : new QuoteListModel()); } }

        public List<QuoteItemListModel> QuoteItems { get; set; }

        public string BusinessName { get; set; }

        public long? BusinessId { get; set; }

        public string ProjectOwner { get; set; }

        public long? ProjectOwnerId { get; set; }

        public long? DiscountRequestId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? RequestedOn { get; set; } 

        public byte? DiscountRequestStatusTypeId { get; set; }

        public DropDownModel DiscountRequestStatusTypes { get; set; }

        public string DiscountRequestStatusTypeDescription { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? DiscountRequestStatusModifiedOn { get; set; }

        public long DiscountRequestStatusModifiedById { get; set; }

        public string DiscountRequestStatusModifiedBy { get; set; }

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

        // DAR

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedDiscount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N3}")]
        public decimal RequestedCommission { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedDiscount { get; set; }

        public QuoteCalculationModel ApprovedTotals { get; set; }

        public QuoteCalculationModel StandardTotals { get; set; }

        public bool ShouldSendEmail { get; set; }

        public string EmailsList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedDiscountSplit 
        { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedDiscountVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedDiscountUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal ApprovedDiscountLCPackage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedDiscountSplit { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedDiscountVRV { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedDiscountUnitary { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, HtmlEncode = false, DataFormatString = "{0:N3}")]
        public decimal RequestedDiscountLCPackage { get; set; }

        public bool IsValidEmails { get; set; }

        public DiscountRequestModel()
        {
            IsValidEmails = true;
            InvalidEmails = new List<string>();
        }

        public List<string> InvalidEmails { get; set; }
    }
}
