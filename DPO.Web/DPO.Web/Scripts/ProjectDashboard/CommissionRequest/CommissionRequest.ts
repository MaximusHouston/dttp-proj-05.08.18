module ProjectDashboard {
    export class CommissionRequest {
        public static ID_COMMISSION_REQUEST_FORM: string = "CommissionRequestForm";
        public static ID_TOTAL_FREIGHT: string = "TotalFreight";
        public static ID_STARTUP_COSTS: string = "StartUpCosts";
        public static ID_THIRDPARTY_EQUIPMENT_COSTS: string = "ThirdPartyEquipmentCosts";

        public static ID_TOTAL_REVISED: string = "TotalRevised";

        public static ID_TOTAL_LIST: string = "TotalList";
        public static ID_TOTAL_NET: string = "TotalNet";

        public static ID_REQUESTED_MULTIPLIER: string = "RequestedMultiplier";
        public static ID_REQUESTED_COMMISSION_PERCENTAGE: string = "RequestedCommissionPercentage";
        public static ID_REQUESTED_COMMISSION_TOTAL: string = "RequestedCommissionTotal";

        public static ID_REQUESTED_NET_MATERIAL_VALUE: string = "RequestedNetMaterialValue";
        public static ID_REQUESTED_NET_MATERIAL_VALUE_MULTIPLIER: string = "RequestedNetMaterialValueMultiplier";

        public static ID_PROJECTID: string = "ProjectId";
        public static ID_QUOTEID: string = "QuoteId";

        public static ID_COMMISSION_REQUESTID: string = "CommissionRequestId";
        public static ID_COMMISSION_MANUALLY_INPUT: string = "CommissionManuallyInput";

        public static ID_COM: string = 'COM';

        private viewOnly: boolean;
        private numericStepperHelpers: any;
        private confirmModal: any;
        private scService: any;

        private vrvFields: CommissionRequestFields;
        private splitFields: CommissionRequestFields;
        private unitaryFields: CommissionRequestFields;
        private lcPackageFields: CommissionRequestFields;

        // DOM Elements
        private $commissionRequestForm: JQuery;
        private $commissionManuallyInput: JQuery;

        private $totalList: JQuery;
        public getTotalList(): number {
            return parseFloat(this.$totalList.val());
        }

        private $totalListWithOther: JQuery;
        public getTotalListWithOther(): number
        {
            return parseFloat(this.$totalListWithOther.val());
        }

        private $totalNet: JQuery;
        private $displayTotalNet: JQuery;

        public getTotalNet(): number {
            return parseFloat(this.$totalNet.val());
        }
        public setTotalNet(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$totalNet.val(val.toFixed(3));
            this.$displayTotalNet.html(this.getAmountDisplay(val));
        }

        private $totalRevised: JQuery;
        private $displayTotalRevised: JQuery;

        public setTotalRevised(val:number) {
            if (!val || isNaN(val)) {
                val = 0;
            }

            this.$totalRevised.val(val.toFixed(4));
            this.$displayTotalRevised.html(this.getAmountDisplay(val));
        }

        private $requestedMultiplier: JQuery;
        private $displayRequestedMultiplier: JQuery;

        public getRequestedMultiplier(): number {
            return parseFloat(this.$requestedMultiplier.val());
        }
        public setRequestedMultiplier(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedMultiplier.val(val.toFixed(3));
            this.$displayRequestedMultiplier.html(val.toFixed(3));
        }

        private $requestedCommissionPercentage: JQuery;
        private $displayRequestedCommissionPercentage: JQuery;

        public getRequestedCommissionPercentage(): number {
            return parseFloat(this.$requestedCommissionPercentage.val());
        }
        public setRequestedCommissionPercentage(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedCommissionPercentage.val(val.toFixed(3));
            this.$displayRequestedCommissionPercentage.html(val.toFixed(3) + "%");
        }

        private $requestedCommissionTotal: JQuery;
        private $displayRequestedCommissionTotal: JQuery;
        public getRequestedCommissionTotal(): number {
            return parseFloat(this.$requestedCommissionTotal.val());
        }
        public setRequestedCommissionTotal(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedCommissionTotal.val(val.toFixed(3));
            this.$displayRequestedCommissionTotal.html(this.getAmountDisplay(val));
        }

        private $requestedNetMaterialValue: JQuery;
        private $displayRequestedNetMaterialValue: JQuery;
        public getRequestedNetMaterialValue(): number {
            return parseFloat(this.$requestedNetMaterialValue.val());
        }
        public setRequestedNetMaterialValue(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedNetMaterialValue.val(val.toFixed(3));
            this.$displayRequestedNetMaterialValue.html(this.getAmountDisplay(val));
        }

        private $requestedNetMaterialValueMultiplier: JQuery;
        private $displayRequestedNetMaterialValueMultiplier: JQuery;
        public getRequestedNetMaterialValueMultiplier(): number {
            return parseFloat(this.$requestedNetMaterialValueMultiplier.val());
        }
        public setRequestedNetMaterialValueMultiplier(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedNetMaterialValueMultiplier.val(val.toFixed(3));
            this.$displayRequestedNetMaterialValueMultiplier.html(val.toFixed(3));
        }

        private $startupCosts: JQuery;
        public getStartupCosts(): number {

            var val = this.$startupCosts.val();
            if (!val || isNaN(val)) {
                val = 0;
            }
            return parseFloat(val);
        }
        private $totalFreight: JQuery;
        public getTotalFreight(): number {
            return parseFloat(this.$totalFreight.val());
        }

        private $thirdPartyEquipmentCosts: JQuery;
        public getThirdPartyEquipmentCosts(): number {
            var val = this.$thirdPartyEquipmentCosts.val();
            if (!val || isNaN(val)) {
                val = 0;
            }
            return parseFloat(val);
        }

        private $com: JQuery;

        private $projectId: JQuery;
        private getProjectId(): number {
            return this.getFieldValue(this.$projectId);
        }

        private $quoteId: JQuery;
        private getQuoteId(): number {
            return this.getFieldValue(this.$quoteId);
        }

        private $commissionRequestId: JQuery;
        private getCommissionRequestId(): number {
            return this.getFieldValue(this.$commissionRequestId);
        }

        constructor(viewOnly, numericStepperHelpers, confirmModal, scService) {
            this.viewOnly = viewOnly;
            this.numericStepperHelpers = numericStepperHelpers;
            this.confirmModal = confirmModal;
            this.scService = scService;

            //this.numericStepperHelpers.enableNumericSteppers();

            this.checkBoxTextBoxCheck('attachLineByLineRow', 'attachLineByLine');
            this.checkBoxTextBoxCheck('competitorPriceAvailableRow', 'competitorPriceAvailable');
            this.checkBoxTextBoxCheck('copyOfCompQuoteRow', 'copyOfCompQuote');

            this.loadFields();
            this.setupEvents();
            //this.calculateDiscountRequest();
        }

        private getValueOrDefault(field: JQuery, defaultVal): any {
            var val = this.getFieldValue(field);

            if (!val) {
                return defaultVal;
            }

            return val;
        }

        private getField(id: string, selector: string): JQuery {
            var id = '#' + id;

            if (selector != null) {
                id = id + ' ' + selector;
            }

            return $(id);
        }

        private getFieldValue(field: JQuery): number {
            if (field == null) {
                return -1;
            }

            return parseFloat(field.val());
        }

        private loadFields() {
            this.vrvFields = new CommissionRequestFields("VRV", this);
            this.splitFields = new CommissionRequestFields("Split", this);
            this.unitaryFields = new CommissionRequestFields("Unitary", this);
            this.lcPackageFields = new CommissionRequestFields("LCPackage", this);

            this.$startupCosts = this.getField(CommissionRequest.ID_STARTUP_COSTS, null);
            this.$totalFreight = this.getField(CommissionRequest.ID_TOTAL_FREIGHT, null);

            this.$thirdPartyEquipmentCosts = this.getField(CommissionRequest.ID_THIRDPARTY_EQUIPMENT_COSTS, null);

            this.$totalList = this.getField(CommissionRequest.ID_TOTAL_LIST, null);
           
            this.$requestedMultiplier = this.getField(CommissionRequest.ID_REQUESTED_MULTIPLIER, null);
            this.$displayRequestedMultiplier = this.getField("Display" + CommissionRequest.ID_REQUESTED_MULTIPLIER, null);
            
            this.$totalNet = this.getField(CommissionRequest.ID_TOTAL_NET, null);
            this.$displayTotalNet = this.getField("Display" + CommissionRequest.ID_TOTAL_NET, null);

            this.$totalRevised = this.getField(CommissionRequest.ID_TOTAL_REVISED, null);
            this.$displayTotalRevised = this.getField("Display" + CommissionRequest.ID_TOTAL_REVISED, null);

            this.$requestedCommissionPercentage = this.getField(CommissionRequest.ID_REQUESTED_COMMISSION_PERCENTAGE, null);
            this.$displayRequestedCommissionPercentage = this.getField("Display" + CommissionRequest.ID_REQUESTED_COMMISSION_PERCENTAGE, null);

            this.$requestedCommissionTotal = this.getField(CommissionRequest.ID_REQUESTED_COMMISSION_TOTAL, null);
            this.$displayRequestedCommissionTotal = this.getField("Display" + CommissionRequest.ID_REQUESTED_COMMISSION_TOTAL, null);

            this.$requestedNetMaterialValue = this.getField(CommissionRequest.ID_REQUESTED_NET_MATERIAL_VALUE, null);
            this.$displayRequestedNetMaterialValue = this.getField("Display" + CommissionRequest.ID_REQUESTED_NET_MATERIAL_VALUE, null);

            this.$requestedNetMaterialValueMultiplier = this.getField(CommissionRequest.ID_REQUESTED_NET_MATERIAL_VALUE_MULTIPLIER, null);
            this.$displayRequestedNetMaterialValueMultiplier = this.getField("Display" + CommissionRequest.ID_REQUESTED_NET_MATERIAL_VALUE_MULTIPLIER, null);

            this.$commissionRequestForm = this.getField(CommissionRequest.ID_COMMISSION_REQUEST_FORM, null);
            this.$commissionManuallyInput = this.getField(CommissionRequest.ID_COMMISSION_MANUALLY_INPUT, null);

            this.$projectId = this.getField(CommissionRequest.ID_PROJECTID, null);
            this.$quoteId = this.getField(CommissionRequest.ID_QUOTEID, null);
            this.$commissionRequestId = this.getField(CommissionRequest.ID_COMMISSION_REQUESTID, null);

            this.$com = this.getField(CommissionRequest.ID_COM, null);
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

                $('#CommissionManuallyInput').removeClass("disabled");
                $('#RequestedCommissionVRVPercentage').removeClass('disabled');
                $('#RequestedCommissionSplitPercentage').removeClass('disabled');
                $('#RequestedCommissionUnitaryPercentage').removeClass('disabled');
                $('#RequestedCommissionLCPackagePercentage').removeClass('disabled');
            });

            var checked: any = $checkbox.is(':checked');
            (checked) ? $containerEl.removeClass('disabled') : $containerEl.addClass('disabled')
            $inputEl.prop('disabled', !checked);
        }

        private validatedStartUpCost(): number {
            var val = this.$startupCosts.val();
            if (isNaN(val)) {
                val = 0;
            }
            if (val.length !== "undefined") {
                var sanitised = (val.length > 0) ? val.replace(",", "") : "0";
            }

            return parseFloat(sanitised);
        }


        private roundToNearestHundredths(value: number) {
            if (value == 0) {
                return 0;
            }

            return Math.round(value * 100) / 100;
        }

        public calculateCommissionRequest() {
            var me = this;

            var vrvTotals = this.vrvFields.getTotals();
            var splitTotals = this.splitFields.getTotals();
            var unitaryTotals = this.unitaryFields.getTotals();
            var lcPackageTotals = this.lcPackageFields.getTotals();

            var startupCost: number = this.getStartupCosts();
            var totalFreight: number = this.getTotalFreight();

            var thirdpartyEquipmentCosts: number = this.getThirdPartyEquipmentCosts();

            var totalList: number = vrvTotals.TotalList + splitTotals.TotalList + unitaryTotals.TotalList + lcPackageTotals.TotalList;
            var totalListWithOther: number = this.$totalList.val();

            var otherTotalList: number = totalListWithOther - totalList;

            var totalNet: number = vrvTotals.TotalNet + splitTotals.TotalNet + unitaryTotals.TotalNet + lcPackageTotals.TotalNet + otherTotalList;

            var totalRevised: number = totalNet + startupCost + thirdpartyEquipmentCosts;

            var totalCommission: number = vrvTotals.CommissionAmount + splitTotals.CommissionAmount + unitaryTotals.CommissionAmount + lcPackageTotals.CommissionAmount;
            var commissionPercentage: number = (totalCommission / totalNet) * 100;

            var netMaterialValue: number = totalNet - totalCommission;
            var netMaterialValueMultiplier: number = netMaterialValue / totalList;

            var netMultiplier: number = totalNet / totalList;

            this.setTotalNet(totalNet);

            console.log("total revised: " + totalRevised);
            this.setTotalRevised(totalRevised);

            this.setRequestedCommissionTotal(totalCommission);
            this.setRequestedCommissionPercentage(commissionPercentage);

            this.setRequestedNetMaterialValue(netMaterialValue);
            this.setRequestedNetMaterialValueMultiplier(netMaterialValueMultiplier);

            this.setRequestedMultiplier(netMultiplier);
        }

        private redirect() {
            window.location.replace("/ProjectDashboard/QuoteItems/" + this.$projectId.val() + "/" + this.$quoteId.val());
        }

        private setupEvents() {
            var me = this;
            var keypress = 0;

            me.numericStepperHelpers.enableNumericSteppers({ trailingChars: '' });

            if (!this.viewOnly) {
                me.checkBoxTextBoxCheck('attachLineByLineRow', 'attachLineByLine');
                me.checkBoxTextBoxCheck('competitorPriceAvailableRow', 'competitorPriceAvailable');
                me.checkBoxTextBoxCheck('copyOfCompQuoteRow', 'copyOfCompQuote');

                this.$startupCosts.on('keyup', function () {
                    //var total = (me.getFieldValue(me.$totalSell) + me.validatedStartUpCost() + me.getFieldValue(me.$totalFreight));
                    //me.$totalSaleValue.html(me.getAmountDisplay(total));
                    me.calculateCommissionRequest();
                });

                this.$thirdPartyEquipmentCosts.on('keyup', function () {
                    me.calculateCommissionRequest();
                });

                var test = $('.numeric-stepper input');

                $('.numeric-stepper button').on('click', function (e) {
                    me.vrvFields.calculateTotals();
                    me.splitFields.calculateTotals();
                    me.unitaryFields.calculateTotals();
                    me.lcPackageFields.calculateTotals();
                });

                $('.numeric-stepper input').on('keyup', function (e) {
                   
                    // if (e.keyCode == 110 ||e.keyCode == 190) {
                       
                    //    $('.numeric-stepper input').val("0.");
                    //}

                    //me.vrvFields.calculateTotals();
                    //me.splitFields.calculateTotals();

                   
                });

                $('#RequestedMultiplierStepperVRV').find('.numbers').on('keyup', function (e) {
                    if (e.keyCode == 110 || e.keyCode == 190) {

                        //This code was removed to allow input greater than 1
                        //$('#RequestedMultiplierStepperVRV').find('.numbers').val("0.");
                    }

                    var ManuallyInput = $('#CommissionManuallyInput')
                   
                    if (me.$commissionManuallyInput.val() == "true") {
                        
                        me.vrvFields.manuallyCalculateTotal();
                    }
                    else {
                        
                        me.vrvFields.calculateTotals();
                        if ($('#RequestedMultiplierVRV').val() < 0.20 || $('#RequestedMultiplierVRV').val() > 1.5) {

                            $('#RequestedMultiplierStepperVRV').find('.numbers').css('border', '1px solid red');
                            $('.commissionMultiplierVRVRangeMessage').show();
                            $('#test').attr('disabled', 'disabled');
                            $('#test').addClass('disabled');
                           
                        }
                        else {
                            $('#RequestedMultiplierStepperVRV').find('.numbers').css('border', '');
                            $('.commissionMultiplierVRVRangeMessage').hide();

                        }
                    }
                   
                });

                $('#RequestedMultiplierStepperSplit').find('.numbers').on('keyup', function (e) {
                    if (e.keyCode == 110 || e.keyCode == 190) {

                        //This code has been remobed to allow input greater than 1
                        //$('#RequestedMultiplierStepperSplit').find('.numbers').val("0.");
                    }

                    if (me.$commissionManuallyInput.val() =="true") {
                        me.splitFields.manuallyCalculateTotal();
                    }
                    else {
                        me.splitFields.calculateTotals();
                        if ($('#RequestedMultiplierSplit').val() < 0.35 || $('#RequestedMultiplierSplit').val() > 1.5) {

                            $('#RequestedMultiplierStepperSplit').find('.numbers').css('border', '1px solid red');
                            $('.commissionMultiplierSplitRangeMessage').show();
                            $('#test').attr('disabled', 'disabled');
                            $('#test').addClass('disabled');
                        }
                        else {
                            $('#RequestedMultiplierStepperSplit').find('.numbers').css('border', '');
                            $('.commissionMultiplierSplitRangeMessage').hide();
                        }
                    }
                });

                $('#RequestedMultiplierStepperUnitary').find('.numbers').on('keyup', function (e) {
                    
                    if (me.$commissionManuallyInput.val() == "true") {
                        me.unitaryFields.manuallyCalculateTotal();
                    }
                    else {

                         me.unitaryFields.calculateUnitaryTotals();

                        if ($('#RequestedMultiplierUnitary').val() < 0.8 || $('#RequestedMultiplierUnitary').val() > 1.0) {

                            $('#RequestedMultiplierStepperUnitary').find('.numbers').css('border', '1px solid red');
                            $('.commissionMultiplierUnitaryRangeMessage').show();
                            $('#test').attr('disabled', 'disabled');
                            $('#test').addClass('disabled');
                        }
                        else {
                            $('#RequestedMultiplierStepperUnitary').find('.numbers').css('border', '');
                            $('.commissionMultiplierUnitaryRangeMessage').hide();
                        }
                    }
                });

                $('#RequestedMultiplierStepperLCPackage').find('.numbers').on('keyup', function (e) {
                    if (e.keyCode == 110 || e.keyCode == 190) {

                        //This code was removed to allow input greater than 1
                        //$('#RequestedMultiplierStepperLCPackage').find('.numbers').val("0.");
                    }

                    var ManuallyInput = $('#CommissionManuallyInput')

                    if (me.$commissionManuallyInput.val() == "true") {

                        me.lcPackageFields.manuallyCalculateTotal();
                    }
                    else {

                        me.lcPackageFields.calculateTotals();
                        if ($('#RequestedMultiplierLCPackage').val() < 0.75 || $('#RequestedMultiplierLCPackage').val() > 1.0) {

                            $('#RequestedMultiplierStepperLCPackage').find('.numbers').css('border', '1px solid red');
                            $('.commissionMultiplierLCPackageRangeMessage').show();
                            $('#test').attr('disabled', 'disabled');
                            $('#test').addClass('disabled');

                        }
                        else {
                            $('#RequestedMultiplierStepperLCPackage').find('.numbers').css('border', '');
                            $('.commissionMultiplierLCPackageRangeMessage').hide();

                        }
                    }

                });

                $('#RequestedCommissionPercentageVRV').on('keyup', function(e) {
                    if (me.$commissionManuallyInput.val() == "true") {
                        me.vrvFields.manuallyCalculateTotal();
                    }
                    else {
                        me.vrvFields.calculateTotals();
                    }
                });

                $('#RequestedCommissionPercentageSplit').on('keyup', function (e) {
                    if (me.$commissionManuallyInput.val() == "true") {
                        me.splitFields.manuallyCalculateTotal();
                    }
                    else {
                        me.splitFields.calculateTotals();
                    }
                });

                $('#RequestedCommissionPercentageUnitary').on('keyup', function (e) {
                    if (me.$commissionManuallyInput.val() == "true") {
                        me.unitaryFields.manuallyCalculateTotal();
                    }
                    else {
                        me.unitaryFields.calculateTotals();
                    }
                });

                $('#RequestedCommissionPercentageLCPackage').on('keyup', function (e) {
                    if (me.$commissionManuallyInput.val() == "true") {
                        me.lcPackageFields.manuallyCalculateTotal();
                    }
                    else {
                        me.lcPackageFields.calculateTotals();
                    }
                });

                $('.commissionPercent input').on('keyup', function (e) {
                    keypress += 1;

                        //me.vrvFields.calculateTotals();
                        //me.splitFields.calculateTotals();
                        keypress = 0;
                    //TODO: missing code for Unitary?
                    if (me.$commissionManuallyInput.val() == "true") {
                      
                        me.splitFields.manuallyCalculateTotal();
                        me.vrvFields.manuallyCalculateTotal();
                        me.lcPackageFields.manuallyCalculateTotal();
                    }
                    else {
                        
                        me.splitFields.calculateTotals();
                        me.vrvFields.calculateTotals();
                        me.lcPackageFields.calculateTotals();
                    }
                    
                });

                //submit the Commission Calculation Form
                $('.submit_commission_btn').on('click', function () {

                    var vrvTotals = me.vrvFields.getTotals();
                    var splitTotals = me.splitFields.getTotals();
                    var unitaryTotals = me.unitaryFields.getTotals();
                    var lcPackageTotals = me.lcPackageFields.getTotals();

                    console.log('unitaryTotals: ', unitaryTotals);

                    // TODO: Calc in back-end
                    var model = {
                        vrvTotalList: vrvTotals.TotalList,
                        splitTotalList: splitTotals.TotalList,
                        unitaryTotalList: unitaryTotals.TotalList,
                        lcPackageTotalList: lcPackageTotals.TotalList,

                        requestedCommissionVRVPercentage: vrvTotals.CommissionPercentage,
                        requestedCommissionSplitPercentage: splitTotals.CommissionPercentage,
                        requestedCommissionUnitaryPercentage: unitaryTotals.CommissionPercentage,
                        requestedCommissionLCPackagePercentage: lcPackageTotals.CommissionPercentage,

                        commissionAmountVRV: vrvTotals.CommissionAmount,
                        commissionAmountSplit: splitTotals.CommissionAmount,
                        commissionAmountUnitary: unitaryTotals.CommissionAmount,
                        commissionAmountLCPackage: lcPackageTotals.CommissionAmount,

                        netMaterialCostMultiplierVRV: vrvTotals.NetMaterialValueMultiplier,
                        netMaterialCostMultiplierSplit: splitTotals.NetMaterialValueMultiplier,
                        netMaterialCostMultiplierUnitary: unitaryTotals.NetMaterialValueMultiplier,
                        netMaterialCostMultiplierLCPackage: lcPackageTotals.NetMaterialValueMultiplier,

                        netMaterialValueVRV: vrvTotals.NetMaterialValue,
                        netMaterialValueSplit: splitTotals.NetMaterialValue,
                        netMaterialValueUnitary: unitaryTotals.NetMaterialValue,
                        netMaterialValueLCPackage: lcPackageTotals.NetMaterialValue,

                        //commissionTotalList: parseFloat(me.$commissionTotalList.text()),
                        totalCommissionPercentage: me.getRequestedCommissionPercentage(),
                        totalCommissionAmount: me.getRequestedCommissionTotal(),
                        totalNetMaterialValue: me.getRequestedNetMaterialValue(),
                        totalNetMaterialValueMultiplier: me.getTotalNet() / me.getTotalList(),

                        commissionRequestMultiplierVRV: vrvTotals.Multiplier,
                        commissionRequestMultiplierSplit: splitTotals.Multiplier,
                        commissionRequestMultiplierUnitary: unitaryTotals.Multiplier,
                        commissionRequestMultiplierLCPackage: lcPackageTotals.Multiplier,
                        // TODO:  Is this right?
                        totalNetMultiplier: me.getRequestedNetMaterialValueMultiplier(),

                        totalNetVRV: vrvTotals.TotalNet,
                        totalNetSplit: splitTotals.TotalNet,
                        totalNetUnitary: unitaryTotals.TotalNet,
                        totalNetLCPackage: lcPackageTotals.TotalNet,

                        totalNet: me.getTotalNet(),
                        totalList: me.getTotalList(),
                        projectId: me.$projectId.val(),
                        quoteId: me.$quoteId.val(),
                        commissionRequestId: me.$commissionRequestId.val()
                    };

                    console.log(model);

                    $.ajax({
                        url: '/projectDashboard/SubmitCommissionCalculation',
                        type: "POST",
                        data: JSON.stringify(model),
                        contentType: 'application/json',
                        dataType: 'json',
                        success: function (result) {
                            console.log(result);
                            me.redirect.call(me);
                            
                        },
                        error: function (message) {
                            console.log('Error: ' + message.statusText);
                        }
                    });
                });
            } else {
                me.$com.find("input[type!=hidden], select")
                    .not('.js-alwaysactive')
                    .attr('disabled', 'true');

                $('#CommissionManuallyInput').removeAttr("disabled");

                me.$com.find('.cb-switch-label')
                    .addClass('disabled');
            }

            if($('.reject_commission_btn').length > 0)
            {
                $('.reject_commission_btn').on('click', function () {
                    me.confirmModal.showConfirmMsg('Reject commission request', 'Are you sure you want to reject this Commission Request?',
                        function () { me.scService.DataScPostAfterConfirm($('.reject_commission_btn'), $('#CommissionRequestForm')) });
                });
            }

            $('#CommissionManuallyInput').on("click", function (e) {

                var $checkbox = $(this);
                //TODO: missing code for Unitary?
                if ($checkbox.is(':checked')) {
                    me.$commissionManuallyInput.val("true");
                   
                    $('#RequestedCommissionVRVPercentage').removeAttr('disabled');
                    $('#RequestedCommissionSplitPercentage').removeAttr('disabled');
                    $('#RequestedCommissionLCPackagePercentage').removeAttr('disabled');
                }
                else {
                    me.$commissionManuallyInput.val("false");
                   
                    $('#RequestedCommissionVRVPercentage').attr('disabled', 'true');
                    $('#RequestedCommissionSplitPercentage').attr('disabled', 'true');
                    $('#RequestedCommissionLCPackagePercentage').attr('disabled', 'true');

                }

            });

            if ($('#btnApproveRequest').length > 0) {
                $('#btnApproveRequest').on('click', function () {
                    me.confirmModal.showConfirmMsg(
                        'Approve commission request',
                        'Are you sure you want to approve this Commission Request ?',
                        function () {
                            me.scService.DataScPostAfterConfirm($('#btnApproveRequest'), me.$commissionRequestForm)
                        });
                });
            }

            $('#btnApproveRequest').removeAttr('disabled');

            
            $('#test').on('click', function () {
                me.confirmModal.showConfirmMsg(
                    'Submit commission request',
                    'Once the commission request is submitted if any editing of the quote takes place the commission request will be made invalid. Submit this commission request?',
                    function () {
                        me.scService.DataScPostAfterConfirm($('#test'),
                            me.$commissionRequestForm)
                    });
            });

            function handler() {
                 //TODO: missing code for Unitary?
                if ($('#RequestedMultiplierSplit').val() > 0.35 && $('#RequestedMultiplierSplit').val() < 1.5
                    && $('#RequestedMultiplierVRV').val() > 0.20 && $('#RequestedMultiplierVRV').val() < 1.5
                    && $('#RequestedMultiplierLCPackage').val() > 0.75 && $('#RequestedMultiplierLCPackage').val() < 1.0) {

                    $('#test').removeAttr('disabled');
                    $('#test').removeClass('disabled');

                    me.confirmModal.showConfirmMsg(
                        'Submit commission request',
                        'Once the commission request is submitted if any editing of the quote takes place the commission request will be made invalid. Submit this commission request?',
                        function () {
                            me.scService.DataScPostAfterConfirm($('#test'),
                                me.$commissionRequestForm)
                        });
                }
                else {
                    alert('Multiplier out of range');
                }
            };

            $('.delete_commission_btn').on('click', function () {
               
                me.confirmModal.showConfirmMsg(
                    'Delete commission request',
                    'Are you sure you want to delete this Commission Request?',
                    function () {
                        me.scService.DataScPostAfterConfirm($('.delete_commission_btn'),
                            me.$commissionRequestForm)
                    });
            });
        }
    }
}