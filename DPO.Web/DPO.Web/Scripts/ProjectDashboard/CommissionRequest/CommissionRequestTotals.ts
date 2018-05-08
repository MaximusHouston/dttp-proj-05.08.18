module ProjectDashboard {
    export class CommissionRequestTotals {
        public TotalList: number;
        public TotalNet: number;
        public Multiplier: number;
        public CommissionPercentage: number;
        public CommissionAmount: number;
        public NetMaterialValue: number;
        public NetMaterialValueMultiplier: number;

        constructor(totalList: number, totalNet: number, multiplier: number, commissionPercentage: number,
            commissionAmount: number, netMaterialValue: number, netMaterialValueMultiplier: number) {
            this.TotalList = isNaN(totalList) ? 0 : totalList;
            this.TotalNet = isNaN(totalNet) ? 0 : totalNet;
            this.Multiplier = isNaN(multiplier) ? 0 : multiplier;
            this.CommissionPercentage = isNaN(commissionPercentage) ? 0 : commissionPercentage;
            this.CommissionAmount = isNaN(commissionAmount) ? 0 : commissionAmount;
            this.NetMaterialValue = isNaN(netMaterialValue) ? 0 : netMaterialValue;
            this.NetMaterialValueMultiplier = isNaN(netMaterialValueMultiplier) ? 0 : netMaterialValueMultiplier;
        }
    }
}