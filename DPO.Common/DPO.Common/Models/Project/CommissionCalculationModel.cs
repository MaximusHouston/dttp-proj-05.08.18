 

namespace DPO.Common
{
    /// <summary>
    /// This should be fixed up to just hold calculations
    /// </summary>
    public class CommissionCalculationModel
    {
        public long? CommissionRequestId { get; set; }
        public CommissionRequestStatusTypeEnum? CommissionRequestStatusTypeId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long? QuoteId { get; set; }
        public string QuoteTitle { get; set; }
        //public decimal commissionTotalList { get; set; }
        public decimal RequestedCommissionPercentage { get; set; }
        public decimal RequestedCommissionPercentageSplit { get; set; }
        public decimal RequestedCommissionPercentageVRV { get; set; }
        public decimal RequestedCommissionPercentageLCPackage { get; set; }
        public decimal RequestedCommissionSplit { get; set; }
        public decimal RequestedCommissionTotal { get; set; }
        public decimal RequestedCommissionVRV { get; set; }
        public decimal RequestedCommissionLCPackage { get; set; }
        public decimal RequestedMultiplier { get; set; }
        public decimal RequestedMultiplierSplit { get; set; }
        public decimal RequestedMultiplierVRV { get; set; }
        public decimal RequestedMultiplierLCPackage { get; set; }
        public decimal RequestedNetMaterialMultiplierSplit { get; set; }
        public decimal RequestedNetMaterialMultiplierVRV { get; set; }
        public decimal RequestedNetMaterialMultiplierLCPackage { get; set; }
        public decimal RequestedNetMaterialValue { get; set; }
        public decimal RequestedNetMaterialValueMultiplier { get; set; }
        public decimal RequestedNetMaterialValueSplit { get; set; }
        public decimal RequestedNetMaterialValueVRV { get; set; }
        public decimal RequestedNetMaterialValueLCPackage { get; set; }
        public decimal TotalList { get; set; }
        public decimal TotalListSplit { get; set; }
        public decimal TotalListVRV { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalNetMultiplier { get; set; }
        public decimal TotalNetSplit { get; set; }
        public decimal TotalNetVRV { get; set; }
        public decimal TotalNetLCPackage { get; set; }
    }
}
