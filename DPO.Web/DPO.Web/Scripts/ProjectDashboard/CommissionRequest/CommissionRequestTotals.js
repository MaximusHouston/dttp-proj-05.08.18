var ProjectDashboard;
(function (ProjectDashboard) {
    var CommissionRequestTotals = /** @class */ (function () {
        function CommissionRequestTotals(totalList, totalNet, multiplier, commissionPercentage, commissionAmount, netMaterialValue, netMaterialValueMultiplier) {
            this.TotalList = isNaN(totalList) ? 0 : totalList;
            this.TotalNet = isNaN(totalNet) ? 0 : totalNet;
            this.Multiplier = isNaN(multiplier) ? 0 : multiplier;
            this.CommissionPercentage = isNaN(commissionPercentage) ? 0 : commissionPercentage;
            this.CommissionAmount = isNaN(commissionAmount) ? 0 : commissionAmount;
            this.NetMaterialValue = isNaN(netMaterialValue) ? 0 : netMaterialValue;
            this.NetMaterialValueMultiplier = isNaN(netMaterialValueMultiplier) ? 0 : netMaterialValueMultiplier;
        }
        return CommissionRequestTotals;
    }());
    ProjectDashboard.CommissionRequestTotals = CommissionRequestTotals;
})(ProjectDashboard || (ProjectDashboard = {}));
//# sourceMappingURL=CommissionRequestTotals.js.map