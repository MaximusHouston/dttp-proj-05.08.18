var ProjectDashboard;
(function (ProjectDashboard) {
    var DiscountRequestTotals = /** @class */ (function () {
        function DiscountRequestTotals(totalNet, discountAmount, revisedTotalNet, revisedTotalSell, revisedNetMultiplier) {
            this.TotalNet = isNaN(totalNet) ? 0 : totalNet;
            this.DiscountAmount = isNaN(discountAmount) ? 0 : discountAmount;
            this.RevisedTotalNet = isNaN(revisedTotalNet) ? 0 : revisedTotalNet;
            this.RevisedTotalSell = isNaN(revisedTotalSell) ? 0 : revisedTotalSell;
            this.RevisedNetMultiplier = isNaN(revisedNetMultiplier) ? 0 : revisedNetMultiplier;
        }
        return DiscountRequestTotals;
    }());
    ProjectDashboard.DiscountRequestTotals = DiscountRequestTotals;
})(ProjectDashboard || (ProjectDashboard = {}));
//# sourceMappingURL=DiscountRequestTotals.js.map