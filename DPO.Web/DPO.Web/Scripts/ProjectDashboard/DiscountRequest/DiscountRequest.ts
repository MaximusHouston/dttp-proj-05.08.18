module ProjectDashboard {
    export class DiscountRequest {
        public static ID_DISCOUNT_REQUEST_FORM: string = "DiscountRequestForm";
        public static ID_TOTAL_FREIGHT: string = "TotalFreight";
        public static ID_REVISED_TOTAL_SALE_DISPLAY: string = "RevisedTotalSaleDisplay";
        public static ID_STARTUP_COSTS: string = "StartUpCosts";
        public static ID_TOTAL_SALE_VALUE: string = "TotalSaleValue";
        public static ID_DISCOUNT_REQUEST_HIDDEN: string = "RequestedDiscount";
        public static ID_REQUESTED_DISCOUNT_AMOUNT_DISPLAY: string = "RequestedDiscountAmountDisplay";
        public static ID_REQUESTED_DISCOUNT_STEPPER: string = "DiscountRequestStepper";
        public static ID_REQUESTED_COMMISSION: string = "RequestedCommission";
        public static ID_REQUESTED_COMMISSION_STEPPER: string = "CommissionRequestStepper";
        public static ID_REQUESTED_COMMISSION_AMOUNT_DISPLAY: string = "RequestedCommissionAmountDisplay";
        public static ID_REQUESTED_DISCOUNT: string = "RequestedDiscount";
        public static ID_APPROVED_DISCOUNT: string = "ApprovedDiscount";
        public static ID_TOTAL_DISCOUNT_PERCENT: string = "TotalDiscountPercent";
        public static ID_TOTAL_DISCOUNT_PERCENT_DISPLAY: string = "DisplayTotalDiscountPercent";

        public static ID_DAC: string = 'DAC';

        private viewOnly: boolean;
        private numericStepperHelpers: any;
        private confirmModal: any;
        private scService: any;

        private $discountRequestForm: JQuery;

        private $requestedCommission: JQuery;
        private $displayRequestedCommissionAmount: JQuery;

        private $startupCosts: JQuery;
        private $totalSell: JQuery;
        private $totalSaleValue: JQuery;
        private $totalFreight: JQuery;
        private $totalList: JQuery;
        private $totalNet: JQuery;
        private $displayRequestedCommission: JQuery;
        private $displayTotalSale: JQuery;
        private $requestedCommissionStepper: JQuery;
        private $totalDiscountPercent: JQuery;
        private $totalDiscountPercentDisplay: JQuery;

        private $dac: JQuery;

        private vrvFields: DiscountRequestFields;
        private splitFields: DiscountRequestFields;
        private totalFields: DiscountRequestFields;
        private unitaryFields: DiscountRequestFields;
        private lcPackageFields: DiscountRequestFields;

        constructor(viewOnly, numericStepperHelpers, confirmModal, scService) {
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

        private loadFields() {
            this.vrvFields = new DiscountRequestFields("VRV");
            this.splitFields = new DiscountRequestFields("Split");
            this.totalFields = new DiscountRequestFields("");
            this.unitaryFields = new DiscountRequestFields("Unitary");
            this.lcPackageFields = new DiscountRequestFields("LCPackage");

            this.$totalList = this.getField(DiscountRequestFields.ID_TOTAL_LIST, null);
            this.$totalNet = this.getField(DiscountRequestFields.ID_TOTAL_NET, null);
            this.$totalSell = this.getField(DiscountRequestFields.ID_TOTAL_SELL, null);
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
        }

        private getAmountDisplay(amount: number) {
            if (amount == null) {
                amount = 0;
            }

            return "$" + amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }

        private checkBoxTextBoxCheck(rowId, containerId) {
            var $checkbox = $('#' + rowId + ' input[type="checkbox"]');

            var $containerEl = $('#' + containerId);

            var $inputEl = $containerEl.find('input, button');

            $checkbox.on('change', function () {
                var checked: boolean = $checkbox.is(':checked');
                (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled')
                $inputEl.prop('disabled', !checked);
            });

            var checked: any = $checkbox.is(':checked');
            (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled')
            $inputEl.prop('disabled', !checked);
        }

        private validatedStartUpCost(): number {
            var val = this.$startupCosts.val();
            var sanitised = (val.length > 0) ? val.replace(",", "") : "0";

            return parseFloat(sanitised);
        }

        private getRequestedCommission(): number {
            if (this.viewOnly) {
                return this.getFieldValue(this.$requestedCommission);
            } else {
                return this.getFieldValue(this.$requestedCommissionStepper);
            }
        }

        private calculateDiscountRequest() {

            var requestedCommission = this.getRequestedCommission();

            var vrvTotals = this.vrvFields.calculateTotals();
            var splitTotals = this.splitFields.calculateTotals();
            var unitaryTotals = this.unitaryFields.calculateTotals();
            var lcPackageTotals = this.lcPackageFields.calculateTotals();

            var totalStartupCost = this.getFieldValue(this.$startupCosts);
            var totalFreight = this.getFieldValue(this.$totalFreight);

            var totalList = this.getFieldValue(this.$totalList);
            var totalNet = this.getFieldValue(this.$totalNet);

            var discountAmount: number = 0;

            if (!isNaN(vrvTotals.DiscountAmount)) {
                discountAmount += vrvTotals.DiscountAmount;
            }
            if (!isNaN(splitTotals.DiscountAmount)) {
                discountAmount += splitTotals.DiscountAmount;
            }
            if (!isNaN(unitaryTotals.DiscountAmount))
            {
                discountAmount += unitaryTotals.DiscountAmount;
            }
            if (!isNaN(lcPackageTotals.DiscountAmount)) {
                discountAmount += lcPackageTotals.DiscountAmount;
            }

            var revisedTotalNet: number = totalNet - discountAmount;

            var revisedTotalSell: number = revisedTotalNet + (revisedTotalNet * (requestedCommission / 100));

            var revisedNetMultiplier: number = revisedTotalNet / totalList;

            var revisedDiscountPercent: number = (discountAmount / totalNet) * 100;
            revisedDiscountPercent = this.roundToNearestHundredths(revisedDiscountPercent);

            var requestedCommissionAmount: number = revisedTotalSell - revisedTotalNet;
            var totalSaleValue = revisedTotalSell + totalStartupCost + totalFreight;

            this.$requestedCommission.val(requestedCommission.toString());
            this.$displayRequestedCommissionAmount.val(this.getAmountDisplay(requestedCommission));

            if (isNaN(revisedNetMultiplier)) revisedNetMultiplier = 0;

            this.$totalDiscountPercent.val((revisedDiscountPercent.toFixed(3)));
            this.$totalDiscountPercentDisplay.html(revisedDiscountPercent.toFixed(2) + "%");  

            this.totalFields.$totalList.val(totalList);
            this.totalFields.$totalNet.val(totalNet);

            this.totalFields.$displayRequestedDiscount.html(this.getAmountDisplay(discountAmount));
            this.totalFields.$displayNetMaterialValue.html(this.getAmountDisplay(revisedTotalNet));
            this.totalFields.$displayNetMultiplier.html(revisedNetMultiplier.toFixed(3));
            this.$displayRequestedCommissionAmount.html(this.getAmountDisplay(requestedCommissionAmount));
            this.$displayTotalSale.html(this.getAmountDisplay(totalSaleValue));
        }

        private roundToNearestHundredths(value: number) {
            if (value == 0) {
                return 0;
            }

            return Math.round(value * 100) / 100;
        }

        private setupEvents() {
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
                    me.confirmModal.showConfirmMsg(
                        'Submit discount request',
                        'Once the discount request is submitted if any editing of the quote takes place the discount request will be made invalid. Submit this discount request?',
                        function () {
                            me.scService.DataScPostAfterConfirm($('.submit_dar_btn'),
                                me.$discountRequestForm)
                        });
                });
            } else {
                this.$dac.find("input[type!=hidden], select")
                    .not('.js-alwaysactive')
                    .attr('disabled', 'true');

                this.$dac.find('.cb-switch-label')
                    .addClass('disabled');

                $('.delete_dar_btn').on('click', function () {
                    me.confirmModal.showConfirmMsg(
                        'Delete discount request',
                        'Are you sure you want to delete this Discount Request?',
                        function () {
                            me.scService.DataScPostAfterConfirm($('.delete_dar_btn'),
                                me.$discountRequestForm)
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
                        me.confirmModal.showConfirmMsg(
                            'Approve discount request',
                            'Are you sure you want to approve this Discount Request?',
                            function () {
                                me.scService.DataScPostAfterConfirm($('.approve_dar_btn'),
                                    me.$discountRequestForm)
                            });
                    });
                }
            }

        }
    }
}