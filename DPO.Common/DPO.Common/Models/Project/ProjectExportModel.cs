 
using System;

namespace DPO.Common
{
    public class ProjectExportModel
    {
        public string ProjectReference { get; set; }

        //for Daikin users only
        public string CRMAccountId { get; set; }

        public string Region { get; set; }

        public string RSM { get; set; }

        public string CSM { get; set; }

        // Project

        public string BusinessName { get; set; }

        public string SellerName { get; set; }

        public string ProjectOwnerName { get; set; }

        public string CustomerBusinessName { get; set; }

        public string EngineerFirm { get; set; }

        public string ProjectName { get; set; }

        public DateTime ProjectDate { get; set; }

        public string ProjectType { get; set; }

        public string ProjectOpenStatus { get; set; }

        public string ProjectStatus { get; set; }

        public string VerticalMarketDescription { get; set; }

        public DateTime BidDate { get; set; }

        public DateTime EstimatedCloseDate { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public string EstimatedDeliveryMonth { get; set; }

        public DateTime ProjectExpirationDate { get; set; }

        public string Transferred { get; set; }

        public string ProjectNotes { get; set; }

        public decimal TotalList { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalSell { get; set; }

        public int VRVOutdoorUnitQty { get; set; }

        public int RTUQty { get; set; }

        public int SplitOutdoorUnitQty { get; set; }

        public int VRVIndoorUnitQty { get; set; }

        // Quote 

        public string QuoteReference { get; set; }

        public string QuoteName { get; set; }

        public string QuoteNotes { get; set; }

        public int Revision { get; set; }

        public string IsGrossMargin { get; set; }

        public string IsCommissionScheme { get; set; }

        public decimal TotalFreight { get; set; }

        public decimal CommissionPercentage { get; set; }

        public decimal DiscountPercentage { get; set; }

        // Product

        public string ProductNumber { get; set; }

        public string ProductDescription { get; set; }

        public string ProductModelType { get; set; }

        public string ProductType { get; set; }

        public string HpHr { get; set; }

        public string Voltage { get; set; }

        public decimal Quantity { get; set; }

        public decimal PriceList { get; set; }

        public decimal PriceNet { get; set; }

        public decimal ExtendedNetPrice { get; set; }

        public string ProductClassCode { get; set; }

        public decimal RequestedCommissionPercent { get; set; }
        public decimal ApprovedCommissionPercent { get; set; }
        public decimal RequestedMultiplier { get; set; }
        public decimal ApprovedMultiplier { get; set; }

        public decimal RequestedCommissionVRVPercent { get; set; }
        public decimal ApprovedCommissionVRVPercent { get; set; }
        public decimal RequestedCommissionMultiplierVRV { get; set; }
        public decimal ApprovedCommissionMultiplierVRV { get; set; }

        public decimal RequestedCommissionSplitPercent { get; set; }
        public decimal ApprovedCommissionSplitPercent { get; set; }
        public decimal RequestedCommissionMultiplierSplit { get; set; }
        public decimal ApprovedCommissionMultiplierSplit { get; set; }

        public bool? IsCommissionRequest { get; set; }

        public string PricingStrategy {
            get {
                if (IsCommissionRequest != null)
                {
                    return IsCommissionRequest == true? "Commission" : "Buy/Sell";
                }
                else return "";
            }
                
        }
      
        public string ProjectLeadStatus { get; set; }

        public string ProjectPipelineNote { get; set; }

        public string ProjectPipelineNoteType { get; set; }

        public DateTime ProjectPipelineNoteDate { get; set; }

    }
}
