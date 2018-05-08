var ProjectDashboard;
(function (ProjectDashboard) {
    var CommissionRequestFields = /** @class */ (function () {
        // Events
        // TODO: Use a better event model
        function CommissionRequestFields(suffix, parentCommissionRequest) {
            this.suffix = "";
            this.suffix = suffix;
            this.parentCommissionRequest = parentCommissionRequest;
            this.loadFields();
        }
        CommissionRequestFields.prototype.getRequestedMutiplier = function () {
            var val = parseFloat(this.$requestedMultiplierStepper.val());
            return val < 0 ? 0 : val;
        };
        CommissionRequestFields.prototype.setRequestedMultiplier = function (val) {
            if (!val || isNaN(val))
                val = 0;
            this.$requestedMultiplier.val(val.toFixed(3));
            if ($('#RequestedMultiplierVRV').val() < 0.20 || $('#RequestedMultiplierVRV').val() > 1.5) {
                $('#RequestedMultiplierStepperVRV').find('.numbers').css('border', '1px solid red');
                $('.commissionMultiplierVRVRangeMessage').show();
                $('#test').attr('disabled', 'disabled');
                $('#test').addClass('disabled');
            }
            else {
                $('#RequestedMultiplierStepperVRV').find('.numbers').css('border', '');
                $('.commissionMultiplierVRVRangeMessage').hide();
                $('#test').removeAttr('disabled');
                $('#test').removeClass('disabled');
            }
            if ($('#RequestedMultiplierSplit').val() < 0.35 || $('#RequestedMultiplierSplit').val() > 1.5) {
                $('#RequestedMultiplierStepperSplit').find('.numbers').css('border', '1px solid red');
                $('.commissionMultiplierSplitRangeMessage').show();
                $('#test').attr('disabled', 'disabled');
                $('#test').addClass('disabled');
            }
            else {
                $('#RequestedMultiplierStepperSplit').find('.numbers').css('border', '');
                $('.commissionMultiplierSplitRangeMessage').hide();
                $('#test').removeAttr('disabled');
                $('#test').removeClass('disabled');
            }
            //TODO: missing code for Unitary?
            if ($('#RequestedMultiplierLCPackage').val() < 0.75 || $('#RequestedMultiplierLCPackage').val() > 1.0) {
                $('#RequestedMultiplierStepperLCPackage').find('.numbers').css('border', '1px solid red');
                $('.commissionMultiplierLCPackageRangeMessage').show();
                $('#test').attr('disabled', 'disabled');
                $('#test').addClass('disabled');
            }
            else {
                $('#RequestedMultiplierStepperLCPackage').find('.numbers').css('border', '');
                $('.commissionMultiplierLCPackageRangeMessage').hide();
                $('#test').removeAttr('disabled');
                $('#test').removeClass('disabled');
            }
        };
        CommissionRequestFields.prototype.getRequestedCommissionPercentage = function () {
            return parseFloat(this.$requestedCommissionPercentage.val());
        };
        CommissionRequestFields.prototype.setRequestedCommissionPercentage = function (val, suffix) {
            this.$requestedCommissionPercentage.val(val.toFixed(3));
            if (suffix == "VRV") {
                this.$requestedCommissionPercentageVRV.val(val.toFixed(3));
            }
            if (suffix == "Split") {
                this.$requestedCommissionPercentageSplit.val(val.toFixed(3));
            }
            if (suffix == "Unitary") {
                this.$requestedCommissionPercentageUnitary.val(val.toFixed(3));
            }
            if (suffix == "LCPackage") {
                this.$requestedCommissionPercentageLCPackage.val(val.toFixed(3));
            }
        };
        CommissionRequestFields.prototype.getApprovedCommissionPercentage = function () {
            return parseFloat(this.$approvedCommissionPercentage.val());
        };
        CommissionRequestFields.prototype.setApprovedCommissionPercentage = function (val) {
            this.$approvedCommissionPercentage.val(val.toFixed(3));
        };
        CommissionRequestFields.prototype.getRequestedCommissionAmount = function () {
            return parseFloat(this.$requestedCommissionAmount.val());
        };
        CommissionRequestFields.prototype.setRequestedCommissionAmount = function (val) {
            if (!val || isNaN(val))
                val = 0;
            this.$requestedCommissionAmount.val(val.toFixed(3));
            this.$displayRequestedCommissionAmount.html(this.getAmountDisplay(val));
        };
        CommissionRequestFields.prototype.getRequestedNetMaterialValueMultiplier = function () {
            return parseFloat(this.$requestedNetMaterialValueMultiplier.val());
        };
        CommissionRequestFields.prototype.setRequestedNetMaterialValueMultiplier = function (val) {
            if (!val || isNaN(val))
                val = 0;
            this.$requestedNetMaterialValueMultiplier.val(val.toFixed(3));
            this.$displayRequestedNetMaterialValueMultiplier.html(val.toFixed(3));
        };
        CommissionRequestFields.prototype.getRequestedNetMaterialValue = function () {
            return parseFloat(this.$requestedNetMaterialValue.val());
        };
        CommissionRequestFields.prototype.setRequestedNetMaterialValue = function (val) {
            if (!val || isNaN(val))
                val = 0;
            this.$requestedNetMaterialValue.val(val.toFixed(3));
            this.$displayRequestedNetMaterialValue.html(this.getAmountDisplay(val));
        };
        CommissionRequestFields.prototype.getTotalNet = function () {
            return parseFloat(this.$totalNet.val());
        };
        CommissionRequestFields.prototype.setTotalNet = function (val) {
            if (!val || isNaN(val))
                val = 0;
            this.$totalNet.val(val.toFixed(3));
            this.$displayTotalNet.html(this.getAmountDisplay(val));
        };
        CommissionRequestFields.prototype.getTotalList = function () {
            return parseFloat(this.$totalList.val());
        };
        CommissionRequestFields.prototype.getTotalListWithOther = function () {
            return parseFloat(this.$totalListWithOther.val());
        };
        CommissionRequestFields.prototype.getTotalRevised = function () {
            return parseFloat(this.$totalRevised.val());
        };
        CommissionRequestFields.prototype.setTotalRevised = function (totalNet, startUpCosts, ThirdPartyCosts) {
            if (!totalNet || isNaN(totalNet)) {
                totalNet = 0;
            }
            if (!startUpCosts || isNaN(startUpCosts)) {
                startUpCosts = 0;
            }
            if (!ThirdPartyCosts || isNaN(ThirdPartyCosts)) {
                ThirdPartyCosts = 0;
            }
            this.$totalRevised.val(totalNet.toFixed(3) + startUpCosts.toFixed(3) + ThirdPartyCosts.toFixed(3));
        };
        CommissionRequestFields.prototype.getStartUpCosts = function () {
            return parseFloat(this.$startUpCosts.val());
        };
        CommissionRequestFields.prototype.getThirdPartyCosts = function () {
            return parseFloat(this.$thirdPartyCosts.val());
        };
        CommissionRequestFields.prototype.loadFields = function () {
            this.$projectId = this.getField(CommissionRequestFields.ID_PROJECTID, null);
            this.$quoteId = this.getField(CommissionRequestFields.ID_QUOTEID, null);
            this.$requestedMultiplierStepper = this.getField(CommissionRequestFields.ID_REQUESTED_MULTIPLIER_STEPPER + this.suffix, "input");
            this.$requestedMultiplier = this.getField(CommissionRequestFields.ID_REQUESTED_MULTIPLIER + this.suffix, null);
            this.$requestedCommissionPercentage = this.getField(CommissionRequestFields.ID_REQUESTED_COMMISSION_PERCENTAGE +
                this.suffix, null);
            this.$approvedCommissionPercentage = this.getField(CommissionRequestFields.ID_APPROVED_COMMISSION_PERCENTAGE +
                this.suffix, null);
            this.$requestedCommissionPercentageVRV = this.getField(CommissionRequestFields.ID_REQUESTED_COMMISSION_PERCENTAGE_VRV, null);
            this.$requestedCommissionPercentageSplit = this.getField(CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_SPLIT, null);
            this.$requestedCommissionPercentageUnitary = this.getField(CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_UNITARY, null);
            this.$requestedCommissionPercentageLCPackage = this.getField(CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_LCPACKAGE, null);
            // Commission Amount
            this.$requestedCommissionAmount = this.getField(CommissionRequestFields.ID_REQUESTED_COMMISSION_AMOUNT + this.suffix, null);
            this.$displayRequestedCommissionAmount = this.getField("Display" + CommissionRequestFields.ID_REQUESTED_COMMISSION_AMOUNT + this.suffix, null);
            this.$requestedNetMaterialValueMultiplier = this.getField(CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_MULTIPLIER + this.suffix, null);
            this.$displayRequestedNetMaterialValueMultiplier = this.getField("Display" + CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_MULTIPLIER + this.suffix, null);
            this.$requestedNetMaterialValue = this.getField(CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_VALUE + this.suffix, null);
            this.$displayRequestedNetMaterialValue = this.getField("Display" + CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_VALUE + this.suffix, null);
            this.$totalNet = this.getField(CommissionRequestFields.ID_TOTAL_NET + this.suffix, null);
            this.$displayTotalNet = this.getField("Display" + CommissionRequestFields.ID_TOTAL_NET + this.suffix, null);
            this.$totalList = this.getField(CommissionRequestFields.ID_TOTAL_LIST + this.suffix, null);
            this.$startUpCosts = this.getField(CommissionRequestFields.ID_STARTUP_COSTS, null);
            this.$thirdPartyCosts = this.getField(CommissionRequestFields.ID_THIRD_PARTY_COSTS, null);
            this.$totalRevised = this.getField(CommissionRequestFields.ID_TOTAL_REVISED, null);
        };
        CommissionRequestFields.prototype.getField = function (id, selector) {
            var id = '#' + id;
            if (selector != null) {
                id = id + ' ' + selector;
            }
            return $(id);
        };
        CommissionRequestFields.prototype.getFieldValue = function (field) {
            if (field == null) {
                return -1;
            }
            return parseFloat(field.val());
        };
        CommissionRequestFields.prototype.getAmountDisplay = function (amount) {
            if (amount == null) {
                amount = 0;
            }
            return "$" + amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };
        CommissionRequestFields.prototype.getMultiplierCategoryType = function (val) {
            if (!val) {
                return 0;
            }
            val = val.toLowerCase();
            switch (val) {
                case "split":
                    return 1;
                case "vrv":
                    return 2;
                case "unitary":
                    return 3;
                case "lcpackage":
                    return 4;
                default:
                    return 0;
            }
        };
        CommissionRequestFields.prototype.calculateTotals = function () {
            var me = this;
            var val = this.getRequestedMutiplier();
            me.setRequestedMultiplier(val);
            var suffix = this.suffix;
            $.ajax({
                url: '/api/CommissionMultiplier/GetCommissionMultiplier',
                type: "POST",
                data: {
                    multiplierCategoryType: this.getMultiplierCategoryType(this.suffix),
                    multiplier: val
                },
                dataType: 'json',
                success: function (result) {
                    var commissionPercent = 0;
                    if (result && result.isok && result.model) {
                        commissionPercent = result.model.commissionPercentage;
                    }
                    me.setRequestedCommissionPercentage(commissionPercent, suffix);
                    me.parentCommissionRequest.calculateCommissionRequest();
                },
                error: function (message) {
                    console.log('Error: ' + message.statusText);
                }
            });
        };
        CommissionRequestFields.prototype.calculateUnitaryTotals = function () {
            var me = this;
            var val = this.getRequestedMutiplier();
            if (val) {
                val = this.roundToNearestHundredths(val);
            }
            me.setRequestedMultiplier(val);
            var suffix = this.suffix;
            $.ajax({
                url: '/api/CommissionMultiplier/GetUnitaryMultiplier',
                type: "POST",
                data: {
                    ProjectId: (this.$projectId).val(),
                    QuoteId: (this.$quoteId).val(),
                    RequestedMultiplierUnitary: val
                },
                dataType: 'json',
                success: function (result) {
                    var commissionPercent = 0;
                    if (result && result.isok && result.model) {
                        commissionPercent = result.model.commissionPercentage;
                    }
                    me.setRequestedCommissionPercentage(commissionPercent, suffix);
                    me.parentCommissionRequest.calculateCommissionRequest();
                },
                error: function (message) {
                    console.log('Error: ' + message.statusText);
                }
            });
        };
        CommissionRequestFields.prototype.manuallyCalculateTotal = function () {
            var me = this;
            var requestMultiplier = this.getRequestedMutiplier();
            var requestPercent = this.getRequestedCommissionPercentage();
            var suffix = this.suffix;
            me.setRequestedMultiplier(requestMultiplier);
            //me.setRequestedCommissionPercentage(requestPercent, this.suffix);
            me.parentCommissionRequest.calculateCommissionRequest();
        };
        CommissionRequestFields.prototype.roundToNearestHundredths = function (value) {
            if (value == 0) {
                return 0;
            }
            return Math.round(value * 100) / 100;
        };
        CommissionRequestFields.prototype.getTotals = function () {
            // Total List
            var totalList = this.getTotalList();
            // Total Net
            var requestedMultiplier = this.getRequestedMutiplier();
            var totalNet = 0;
            if (requestedMultiplier > 0) {
                totalNet = this.getTotalList() * this.getRequestedMutiplier();
            }
            else {
                totalNet = this.getTotalList();
            }
            this.setTotalNet(totalNet);
            //TotalRevised
            var totalRevised = this.getTotalRevised();
            var startupCosts = this.getStartUpCosts();
            var thirdPartyCosts = this.getThirdPartyCosts();
            this.setTotalRevised(totalNet, startupCosts, thirdPartyCosts);
            // Commission
            var requestedCommission = totalNet * (this.getRequestedCommissionPercentage() / 100);
            this.setRequestedCommissionAmount(requestedCommission);
            // Net material value
            var requestedNetMaterialValue = totalNet - requestedCommission;
            this.setRequestedNetMaterialValue(requestedNetMaterialValue);
            // Net material multiplier
            var requestedNetMaterialMultiplier = requestedNetMaterialValue / totalList;
            this.setRequestedNetMaterialValueMultiplier(requestedNetMaterialMultiplier);
            return new ProjectDashboard.CommissionRequestTotals(totalList, totalNet, this.getRequestedMutiplier(), this.getRequestedCommissionPercentage(), requestedCommission, requestedNetMaterialValue, requestedNetMaterialMultiplier);
        };
        CommissionRequestFields.ID_TOTAL_NET = "TotalNet";
        CommissionRequestFields.ID_TOTAL_LIST = "TotalList";
        CommissionRequestFields.ID_PROJECTID = "ProjectId";
        CommissionRequestFields.ID_QUOTEID = "QuoteId";
        CommissionRequestFields.ID_REQUESTED_MULTIPLIER_STEPPER = "RequestedMultiplierStepper";
        CommissionRequestFields.ID_REQUESTED_COMMISSION_PERCENTAGE = "RequestedCommissionPercentage";
        CommissionRequestFields.ID_APPROVED_COMMISSION_PERCENTAGE = "ApprovedCommissionPercentage";
        CommissionRequestFields.ID_REQUESTED_MULTIPLIER = "RequestedMultiplier";
        CommissionRequestFields.ID_REQUESTED_COMMISSION_AMOUNT = "RequestedCommission";
        CommissionRequestFields.ID_APPROVED_COMMISSION_AMOUNT = "ApprovedCommission";
        CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_MULTIPLIER = "RequestedNetMaterialMultiplier";
        CommissionRequestFields.ID_REQUESTED_NET_MATERIAL_VALUE = "RequestedNetMaterialValue";
        //public static ID_REQUESTED_COMMISSION_PERCENTAGE_VRV: string = "RequestedCommissionPercentageVRV";
        CommissionRequestFields.ID_REQUESTED_COMMISSION_PERCENTAGE_VRV = "hidden_RequestedCommissionPercentageVRV";
        //public static ID_RQUESTED_COMMISSION_PERCENTAGE_SPLIT: string = "RequestedCommissionPercentageSplit";
        CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_SPLIT = "hidden_RequestedCommissionPercentageSplit";
        CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_UNITARY = "hidden_RequestedCommissionPercentageUnitary";
        CommissionRequestFields.ID_RQUESTED_COMMISSION_PERCENTAGE_LCPACKAGE = "hidden_RequestedCommissionPercentageLCPackage";
        CommissionRequestFields.ID_TOTAL_REVISED = "TotalRevised";
        CommissionRequestFields.ID_STARTUP_COSTS = "StartUpCost";
        CommissionRequestFields.ID_THIRD_PARTY_COSTS = "ThirdPartyEquipmentCosts";
        return CommissionRequestFields;
    }());
    ProjectDashboard.CommissionRequestFields = CommissionRequestFields;
})(ProjectDashboard || (ProjectDashboard = {}));
//# sourceMappingURL=CommissionRequestFields.js.map