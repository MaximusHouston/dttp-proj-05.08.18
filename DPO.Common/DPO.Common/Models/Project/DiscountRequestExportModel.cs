 

namespace DPO.Common
{
    public class DiscountRequestExportModel
    {
        public DiscountRequestExportModel()
        { 
        }

        public string ProjectName { get; set; }

        public string QuoteName { get; set; }

        public string BusinessName { get; set; }

        public string ProjectOwner { get; set; }

        public string DiscountRequestStatusType{ get; set; }

        public string LastModified { get; set; }

        public string LastModifiedBy { get; set; }

        public string SystemBasisDesignTypeDescription { get; set; }

        public string ZoneStrategyTypeDescription { get; set; }

        public string BrandSpecifiedTypeDescription { get; set; }

        public string BrandApprovedTypeDescription { get; set; }

        public bool HasCompetitorPrice { get; set; }

        public decimal? CompetitorPrice { get; set; }

        public bool IsConfidentCompetitorQuote { get; set; }

        //deprecated
        //public bool ApprovalAssuresOrder { get; set; }

        public bool HasCompetitorQuote { get; set; }

        public bool HasCompetitorLineComparsion { get; set; }

        public string OrderPlannedFor { get; set; }

        public string OrderDeliveryDate { get; set; }

        public string Notes { get; set; }

        public string ResponseNotes { get; set; }

        public decimal StartUpCosts { get; set; }

        public decimal RequestedDiscount { get; set; }

        public decimal RequestedCommission { get; set; }
    }
}
