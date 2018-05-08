module ProjectDashboard {
    export class DiscountRequestFields {
        public static ID_TOTAL_SELL: string = "TotalSell";
        public static ID_TOTAL_NET: string = "TotalNet";
        public static ID_TOTAL_LIST: string = "TotalList";
        public static ID_TOTAL_DISCOUNT_PERCENT_DISPLAY: string = "TotalDiscountPercentDisplay";
        public static ID_NET_MULTIPLIER_DISPLAY: string = "NetMultiplierDisplay";
        public static ID_NET_MATERIAL_VALUE_DISPLAY: string = "NetMaterialValueDisplay";
        public static ID_DISCOUNT_AMOUNT_DISPLAY: string = "DiscountAmountDisplay";
        public static ID_DISCOUNT_REQUESTED_STEPPER: string = "DiscountRequestStepper";
        public static ID_DISCOUNT_REQUESTED: string = "RequestedDiscount";
        public static ID_DISCOUNT_APPROVED: string = "ApprovedDiscount";

        private suffix: string = "";

        private $discountRequest: any;

        public $displayNetMultiplier: any;
        public $displayNetMaterialValue: any;
        public $displayRequestedDiscount: any;

        private $requestedDiscount: any;
        private $approvedDiscount: any;

        public $totalNet: any;
        public getTotalNet(): number {
            return this.$totalNet.val();
        }

        public $totalList: any;
        public getTotalList(): any {
            return this.$totalList.val();
        }

        public $totalSell: any;
        public getTotalSell(): number {
            return this.$totalSell.val();
        }

        private $requestedCommission: any;
        public getRequestedCommission(): number {
            return this.$requestedCommission.val();
        }

        constructor(suffix) {
            this.suffix = suffix;

            this.loadFields();
        }

        private loadFields() {
            this.$requestedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_REQUESTED + this.suffix, null);
            this.$approvedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_APPROVED + this.suffix, null);
            this.$displayNetMultiplier = this.getField(DiscountRequestFields.ID_NET_MULTIPLIER_DISPLAY + this.suffix, null);
            this.$displayNetMaterialValue = this.getField(DiscountRequestFields.ID_NET_MATERIAL_VALUE_DISPLAY + this.suffix, null);
            this.$displayRequestedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_AMOUNT_DISPLAY + this.suffix, null);
            this.$totalNet = this.getField(DiscountRequestFields.ID_TOTAL_NET + this.suffix, null);
            this.$totalList = this.getField(DiscountRequestFields.ID_TOTAL_LIST + this.suffix, null);
            this.$totalSell = this.getField(DiscountRequestFields.ID_TOTAL_SELL + this.suffix, null);
            this.$discountRequest = this.getField(DiscountRequestFields.ID_DISCOUNT_REQUESTED_STEPPER + this.suffix, "input");
        }

        private getField(id: string, selector: string): any {
            var id = '#' + id;

            if (selector != null) {
                id = id + ' ' + selector;
            }

            return $(id);
        }

        private getFieldValue(field: any): number {
            if (field == null) {
                return -1;
            }

            return parseFloat(field.val());
        }

        private getAmountDisplay(amount: number) {
            if (amount == null) {
                amount = 0;
            }

            return "$" + amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }

        private roundToNearestHundredths(value: number) {
            if (value == 0) {
                return 0;
            }

            return Math.round(value * 100) / 100;
        }

        public calculateTotals(): DiscountRequestTotals {
            var requestedDiscount = this.getFieldValue(this.$discountRequest);

            this.$requestedDiscount.val(requestedDiscount);
            this.$approvedDiscount.val(requestedDiscount);

            var totalNet = this.getFieldValue(this.$totalNet);
            var totalList = this.getFieldValue(this.$totalList);
            var totalSell = this.getFieldValue(this.$totalSell);

            //discountHiddenField.val(requestedDiscount);
            var discountAmount: number = totalNet * (requestedDiscount / 100);
            var revisedTotalNet: number = totalNet - discountAmount;
            var revisedTotalSell: number = revisedTotalNet;
            var revisedNetMultiplier: number = (revisedTotalNet / totalList);
            revisedNetMultiplier = this.roundToNearestHundredths(revisedNetMultiplier);

            if (isNaN(revisedNetMultiplier)) {
                revisedNetMultiplier = 0;
            }

            this.$displayRequestedDiscount.html(this.getAmountDisplay(discountAmount));
            this.$displayNetMultiplier.html(revisedNetMultiplier.toFixed(3));
            this.$displayNetMaterialValue.html(this.getAmountDisplay(revisedTotalSell));

            return new DiscountRequestTotals(totalNet, discountAmount, revisedTotalNet, revisedTotalSell, revisedNetMultiplier);
        }
    }
}