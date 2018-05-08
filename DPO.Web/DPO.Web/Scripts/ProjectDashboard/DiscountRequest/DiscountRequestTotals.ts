module ProjectDashboard {
    export class DiscountRequestTotals {
        public TotalNet: number;
        public DiscountAmount: number;
        public RevisedTotalNet: number;
        public RevisedTotalSell: number;
        public RevisedNetMultiplier: number;

        constructor(totalNet: number, discountAmount: number, revisedTotalNet: number, revisedTotalSell: number, revisedNetMultiplier: number) {
            this.TotalNet = isNaN(totalNet) ? 0 : totalNet;
            this.DiscountAmount = isNaN(discountAmount) ? 0 : discountAmount;
            this.RevisedTotalNet = isNaN(revisedTotalNet) ? 0 : revisedTotalNet;
            this.RevisedTotalSell = isNaN(revisedTotalSell) ? 0 : revisedTotalSell;
            this.RevisedNetMultiplier = isNaN(revisedNetMultiplier) ? 0 : revisedNetMultiplier;
        }
    }
}