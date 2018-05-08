var ProjectDashboard;
(function (ProjectDashboard) {
    var DiscountRequest = /** @class */ (function () {
        function DiscountRequest(viewOnly, numericStepperHelpers, confirmModal, scService) {
            this.viewOnly = viewOnly;
            this.numericStepperHelpers = numericStepperHelpers;
            this.confirmModal = confirmModal;
            this.scService = scService;
            //this.numericStepperHelpers.enableNumericSteppers({ trailingChars: '%' });
            this.checkBoxTextBoxCheck('attachLineByLineRow', 'attachLineByLine');
            this.checkBoxTextBoxCheck('competitorPriceAvailableRow', 'competitorPriceAvailable');
            this.checkBoxTextBoxCheck('copyOfCompQuoteRow', 'copyOfCompQuote');
            this.loadFields();
            this.setupEvents();
            //this.calculateDiscountRequest();
        }
        DiscountRequest.prototype.getField = function (id, selector) {
            var id = '#' + id;
            if (selector != null) {
                id = id + ' ' + selector;
            }
            return $(id);
        };
        DiscountRequest.prototype.getFieldValue = function (field) {
            if (field == null) {
                return -1;
            }
            return parseFloat(field.val());
        };
        DiscountRequest.prototype.loadFields = function () {
            this.vrvFields = new ProjectDashboard.DiscountRequestFields("VRV");
            this.splitFields = new ProjectDashboard.DiscountRequestFields("Split");
            this.totalFields = new ProjectDashboard.DiscountRequestFields("");
            this.unitaryFields = new ProjectDashboard.DiscountRequestFields("Unitary");
            this.lcPackageFields = new ProjectDashboard.DiscountRequestFields("LCPackage");
            this.$totalList = this.getField(ProjectDashboard.DiscountRequestFields.ID_TOTAL_LIST, null);
            this.$totalNet = this.getField(ProjectDashboard.DiscountRequestFields.ID_TOTAL_NET, null);
            this.$totalSell = this.getField(ProjectDashboard.DiscountRequestFields.ID_TOTAL_SELL, null);
            this.$startupCosts = this.getField(DiscountRequest.ID_STARTUP_COSTS, null);
            this.$totalFreight = this.getField(DiscountRequest.ID_TOTAL_FREIGHT, null);
            this.$totalDiscountPercent = this.getField(DiscountRequest.ID_TOTAL_DISCOUNT_PERCENT, "input");
            this.$totalDiscountPercentDisplay = this.getField(DiscountRequest.ID_TOTAL_DISCOUNT_PERCENT_DISPLAY, null);
            this.$requestedCommission = this.getField(DiscountRequest.ID_REQUESTED_COMMISSION, null);
            this.$requestedCommissionStepper = this.getField(DiscountRequest.ID_REQUESTED_COMMISSION_STEPPER, "input");
            this.$displayRequestedCommissionAmount = this.getField(DiscountRequest.ID_REQUESTED_COMMISSION_AMOUNT_DISPLAY, null);
            this.$totalSaleValue = this.getField(DiscountRequest.ID_TOTAL_SALE_VALUE, null);
            this.$displayTotalSale = this.getField(DiscountRequest.ID_REVISED_TOTAL_SALE_DISPLAY, null);
            this.$dac = this.getField(DiscountRequest.ID_DAC, null);
            this.$discountRequestForm = this.getField(DiscountRequest.ID_DISCOUNT_REQUEST_FORM, null);
        };
        DiscountRequest.prototype.getAmountDisplay = function (amount) {
            if (amount == null) {
                amount = 0;
            }
            return "$" + amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };
        DiscountRequest.prototype.checkBoxTextBoxCheck = function (rowId, containerId) {
            var $checkbox = $('#' + rowId + ' input[type="checkbox"]');
            var $containerEl = $('#' + containerId);
            var $inputEl = $containerEl.find('input, button');
            $checkbox.on('change', function () {
                var checked = $checkbox.is(':checked');
                (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled');
                $inputEl.prop('disabled', !checked);
            });
            var checked = $checkbox.is(':checked');
            (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled');
            $inputEl.prop('disabled', !checked);
        };
        DiscountRequest.prototype.validatedStartUpCost = function () {
            var val = this.$startupCosts.val();
            var sanitised = (val.length > 0) ? val.replace(",", "") : "0";
            return parseFloat(sanitised);
        };
        DiscountRequest.prototype.getRequestedCommission = function () {
            if (this.viewOnly) {
                return this.getFieldValue(this.$requestedCommission);
            }
            else {
                return this.getFieldValue(this.$requestedCommissionStepper);
            }
        };
        DiscountRequest.prototype.calculateDiscountRequest = function () {
            var requestedCommission = this.getRequestedCommission();
            var vrvTotals = this.vrvFields.calculateTotals();
            var splitTotals = this.splitFields.calculateTotals();
            var unitaryTotals = this.unitaryFields.calculateTotals();
            var lcPackageTotals = this.lcPackageFields.calculateTotals();
            var totalStartupCost = this.getFieldValue(this.$startupCosts);
            var totalFreight = this.getFieldValue(this.$totalFreight);
            var totalList = this.getFieldValue(this.$totalList);
            var totalNet = this.getFieldValue(this.$totalNet);
            var discountAmount = 0;
            if (!isNaN(vrvTotals.DiscountAmount)) {
                discountAmount += vrvTotals.DiscountAmount;
            }
            if (!isNaN(splitTotals.DiscountAmount)) {
                discountAmount += splitTotals.DiscountAmount;
            }
            if (!isNaN(unitaryTotals.DiscountAmount)) {
                discountAmount += unitaryTotals.DiscountAmount;
            }
            if (!isNaN(lcPackageTotals.DiscountAmount)) {
                discountAmount += lcPackageTotals.DiscountAmount;
            }
            var revisedTotalNet = totalNet - discountAmount;
            var revisedTotalSell = revisedTotalNet + (revisedTotalNet * (requestedCommission / 100));
            var revisedNetMultiplier = revisedTotalNet / totalList;
            var revisedDiscountPercent = (discountAmount / totalNet) * 100;
            revisedDiscountPercent = this.roundToNearestHundredths(revisedDiscountPercent);
            var requestedCommissionAmount = revisedTotalSell - revisedTotalNet;
            var totalSaleValue = revisedTotalSell + totalStartupCost + totalFreight;
            this.$requestedCommission.val(requestedCommission.toString());
            this.$displayRequestedCommissionAmount.val(this.getAmountDisplay(requestedCommission));
            if (isNaN(revisedNetMultiplier))
                revisedNetMultiplier = 0;
            this.$totalDiscountPercent.val((revisedDiscountPercent.toFixed(3)));
            this.$totalDiscountPercentDisplay.html(revisedDiscountPercent.toFixed(2) + "%");
            this.totalFields.$totalList.val(totalList);
            this.totalFields.$totalNet.val(totalNet);
            this.totalFields.$displayRequestedDiscount.html(this.getAmountDisplay(discountAmount));
            this.totalFields.$displayNetMaterialValue.html(this.getAmountDisplay(revisedTotalNet));
            this.totalFields.$displayNetMultiplier.html(revisedNetMultiplier.toFixed(3));
            this.$displayRequestedCommissionAmount.html(this.getAmountDisplay(requestedCommissionAmount));
            this.$displayTotalSale.html(this.getAmountDisplay(totalSaleValue));
        };
        DiscountRequest.prototype.roundToNearestHundredths = function (value) {
            if (value == 0) {
                return 0;
            }
            return Math.round(value * 100) / 100;
        };
        DiscountRequest.prototype.setupEvents = function () {
            var me = this;
            me.numericStepperHelpers.enableNumericSteppers({ trailingChars: '%' });
            $('.numeric-stepper button').on('click', function () {
                me.calculateDiscountRequest.call(me);
            });
            $('.numeric-stepper input').on('keyup', function () {
                me.calculateDiscountRequest.call(me);
            });
            if (!this.viewOnly) {
                me.checkBoxTextBoxCheck('attachLineByLineRow', 'attachLineByLine');
                me.checkBoxTextBoxCheck('competitorPriceAvailableRow', 'competitorPriceAvailable');
                me.checkBoxTextBoxCheck('copyOfCompQuoteRow', 'copyOfCompQuote');
                this.$startupCosts.on('keyup', function () {
                    var total = (me.getFieldValue(me.$totalSell) + me.validatedStartUpCost() + me.getFieldValue(me.$totalFreight));
                    me.$totalSaleValue.html(me.getAmountDisplay(total));
                    me.calculateDiscountRequest();
                });
                $('.submit_dar_btn').on('click', function () {
                    me.confirmModal.showConfirmMsg('Submit discount request', 'Once the discount request is submitted if any editing of the quote takes place the discount request will be made invalid. Submit this discount request?', function () {
                        me.scService.DataScPostAfterConfirm($('.submit_dar_btn'), me.$discountRequestForm);
                    });
                });
            }
            else {
                this.$dac.find("input[type!=hidden], select")
                    .not('.js-alwaysactive')
                    .attr('disabled', 'true');
                this.$dac.find('.cb-switch-label')
                    .addClass('disabled');
                $('.delete_dar_btn').on('click', function () {
                    me.confirmModal.showConfirmMsg('Delete discount request', 'Are you sure you want to delete this Discount Request?', function () {
                        me.scService.DataScPostAfterConfirm($('.delete_dar_btn'), me.$discountRequestForm);
                    });
                });
                //if($('.reject_dar_btn').length > 0)
                //{
                //    $('.reject_dar_btn').on('click', function () {
                //        confirmModal.showConfirmMsg('Reject discount request', 'Are you sure you want to reject this Discount Request?', function () { DataScPostAfterConfirm($('.reject_dar_btn'), $('#DiscountRequestForm')) });
                //    });
                //}
                if ($('.approve_dar_btn').length > 0) {
                    $('.approve_dar_btn').on('click', function () {
                        me.confirmModal.showConfirmMsg('Approve discount request', 'Are you sure you want to approve this Discount Request?', function () {
                            me.scService.DataScPostAfterConfirm($('.approve_dar_btn'), me.$discountRequestForm);
                        });
                    });
                }
            }
        };
        DiscountRequest.ID_DISCOUNT_REQUEST_FORM = "DiscountRequestForm";
        DiscountRequest.ID_TOTAL_FREIGHT = "TotalFreight";
        DiscountRequest.ID_REVISED_TOTAL_SALE_DISPLAY = "RevisedTotalSaleDisplay";
        DiscountRequest.ID_STARTUP_COSTS = "StartUpCosts";
        DiscountRequest.ID_TOTAL_SALE_VALUE = "TotalSaleValue";
        DiscountRequest.ID_DISCOUNT_REQUEST_HIDDEN = "RequestedDiscount";
        DiscountRequest.ID_REQUESTED_DISCOUNT_AMOUNT_DISPLAY = "RequestedDiscountAmountDisplay";
        DiscountRequest.ID_REQUESTED_DISCOUNT_STEPPER = "DiscountRequestStepper";
        DiscountRequest.ID_REQUESTED_COMMISSION = "RequestedCommission";
        DiscountRequest.ID_REQUESTED_COMMISSION_STEPPER = "CommissionRequestStepper";
        DiscountRequest.ID_REQUESTED_COMMISSION_AMOUNT_DISPLAY = "RequestedCommissionAmountDisplay";
        DiscountRequest.ID_REQUESTED_DISCOUNT = "RequestedDiscount";
        DiscountRequest.ID_APPROVED_DISCOUNT = "ApprovedDiscount";
        DiscountRequest.ID_TOTAL_DISCOUNT_PERCENT = "TotalDiscountPercent";
        DiscountRequest.ID_TOTAL_DISCOUNT_PERCENT_DISPLAY = "DisplayTotalDiscountPercent";
        DiscountRequest.ID_DAC = 'DAC';
        return DiscountRequest;
    }());
    ProjectDashboard.DiscountRequest = DiscountRequest;
})(ProjectDashboard || (ProjectDashboard = {}));
//# sourceMappingURL=DiscountRequest.js.map