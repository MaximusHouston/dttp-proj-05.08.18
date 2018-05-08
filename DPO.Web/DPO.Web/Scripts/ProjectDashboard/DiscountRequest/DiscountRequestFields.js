var ProjectDashboard;
(function (ProjectDashboard) {
    var DiscountRequestFields = /** @class */ (function () {
        function DiscountRequestFields(suffix) {
            this.suffix = "";
            this.suffix = suffix;
            this.loadFields();
        }
        DiscountRequestFields.prototype.getTotalNet = function () {
            return this.$totalNet.val();
        };
        DiscountRequestFields.prototype.getTotalList = function () {
            return this.$totalList.val();
        };
        DiscountRequestFields.prototype.getTotalSell = function () {
            return this.$totalSell.val();
        };
        DiscountRequestFields.prototype.getRequestedCommission = function () {
            return this.$requestedCommission.val();
        };
        DiscountRequestFields.prototype.loadFields = function () {
            this.$requestedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_REQUESTED + this.suffix, null);
            this.$approvedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_APPROVED + this.suffix, null);
            this.$displayNetMultiplier = this.getField(DiscountRequestFields.ID_NET_MULTIPLIER_DISPLAY + this.suffix, null);
            this.$displayNetMaterialValue = this.getField(DiscountRequestFields.ID_NET_MATERIAL_VALUE_DISPLAY + this.suffix, null);
            this.$displayRequestedDiscount = this.getField(DiscountRequestFields.ID_DISCOUNT_AMOUNT_DISPLAY + this.suffix, null);
            this.$totalNet = this.getField(DiscountRequestFields.ID_TOTAL_NET + this.suffix, null);
            this.$totalList = this.getField(DiscountRequestFields.ID_TOTAL_LIST + this.suffix, null);
            this.$totalSell = this.getField(DiscountRequestFields.ID_TOTAL_SELL + this.suffix, null);
            this.$discountRequest = this.getField(DiscountRequestFields.ID_DISCOUNT_REQUESTED_STEPPER + this.suffix, "input");
        };
        DiscountRequestFields.prototype.getField = function (id, selector) {
            var id = '#' + id;
            if (selector != null) {
                id = id + ' ' + selector;
            }
            return $(id);
        };
        DiscountRequestFields.prototype.getFieldValue = function (field) {
            if (field == null) {
                return -1;
            }
            return parseFloat(field.val());
        };
        DiscountRequestFields.prototype.getAmountDisplay = function (amount) {
            if (amount == null) {
                amount = 0;
            }
            return "$" + amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };
        DiscountRequestFields.prototype.roundToNearestHundredths = function (value) {
            if (value == 0) {
                return 0;
            }
            return Math.round(value * 100) / 100;
        };
        DiscountRequestFields.prototype.calculateTotals = function () {
            var requestedDiscount = this.getFieldValue(this.$discountRequest);
            this.$requestedDiscount.val(requestedDiscount);
            this.$approvedDiscount.val(requestedDiscount);
            var totalNet = this.getFieldValue(this.$totalNet);
            var totalList = this.getFieldValue(this.$totalList);
            var totalSell = this.getFieldValue(this.$totalSell);
            //discountHiddenField.val(requestedDiscount);
            var discountAmount = totalNet * (requestedDiscount / 100);
            var revisedTotalNet = totalNet - discountAmount;
            var revisedTotalSell = revisedTotalNet;
            var revisedNetMultiplier = (revisedTotalNet / totalList);
            revisedNetMultiplier = this.roundToNearestHundredths(revisedNetMultiplier);
            if (isNaN(revisedNetMultiplier)) {
                revisedNetMultiplier = 0;
            }
            this.$displayRequestedDiscount.html(this.getAmountDisplay(discountAmount));
            this.$displayNetMultiplier.html(revisedNetMultiplier.toFixed(3));
            this.$displayNetMaterialValue.html(this.getAmountDisplay(revisedTotalSell));
            return new ProjectDashboard.DiscountRequestTotals(totalNet, discountAmount, revisedTotalNet, revisedTotalSell, revisedNetMultiplier);
        };
        DiscountRequestFields.ID_TOTAL_SELL = "TotalSell";
        DiscountRequestFields.ID_TOTAL_NET = "TotalNet";
        DiscountRequestFields.ID_TOTAL_LIST = "TotalList";
        DiscountRequestFields.ID_TOTAL_DISCOUNT_PERCENT_DISPLAY = "TotalDiscountPercentDisplay";
        DiscountRequestFields.ID_NET_MULTIPLIER_DISPLAY = "NetMultiplierDisplay";
        DiscountRequestFields.ID_NET_MATERIAL_VALUE_DISPLAY = "NetMaterialValueDisplay";
        DiscountRequestFields.ID_DISCOUNT_AMOUNT_DISPLAY = "DiscountAmountDisplay";
        DiscountRequestFields.ID_DISCOUNT_REQUESTED_STEPPER = "DiscountRequestStepper";
        DiscountRequestFields.ID_DISCOUNT_REQUESTED = "RequestedDiscount";
        DiscountRequestFields.ID_DISCOUNT_APPROVED = "ApprovedDiscount";
        return DiscountRequestFields;
    }());
    ProjectDashboard.DiscountRequestFields = DiscountRequestFields;
})(ProjectDashboard || (ProjectDashboard = {}));
//# sourceMappingURL=DiscountRequestFields.js.map