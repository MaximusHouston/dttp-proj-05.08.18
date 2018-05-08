module ProjectDashboard {
    export class CommissionRequestFields {
        public static ID_TOTAL_NET: string = "TotalNet";
        public static ID_TOTAL_LIST: string = "TotalList";

        public static ID_PROJECTID: string = "ProjectId";
        public static ID_QUOTEID: string = "QuoteId";

        public static ID_REQUESTED_MULTIPLIER_STEPPER: string = "RequestedMultiplierStepper";
        public static ID_REQUESTED_COMMISSION_PERCENTAGE: string = "RequestedCommissionPercentage";
        public static ID_APPROVED_COMMISSION_PERCENTAGE: string = "ApprovedCommissionPercentage";
        public static ID_REQUESTED_MULTIPLIER: string = "RequestedMultiplier";
        public static ID_REQUESTED_COMMISSION_AMOUNT: string = "RequestedCommission";
        public static ID_APPROVED_COMMISSION_AMOUNT: string = "ApprovedCommission";
        public static ID_REQUESTED_NET_MATERIAL_MULTIPLIER: string = "RequestedNetMaterialMultiplier";
        public static ID_REQUESTED_NET_MATERIAL_VALUE: string = "RequestedNetMaterialValue";

        //public static ID_REQUESTED_COMMISSION_PERCENTAGE_VRV: string = "RequestedCommissionPercentageVRV";
        public static ID_REQUESTED_COMMISSION_PERCENTAGE_VRV: string = "hidden_RequestedCommissionPercentageVRV";
        //public static ID_RQUESTED_COMMISSION_PERCENTAGE_SPLIT: string = "RequestedCommissionPercentageSplit";
        public static ID_RQUESTED_COMMISSION_PERCENTAGE_SPLIT: string = "hidden_RequestedCommissionPercentageSplit";

        public static ID_RQUESTED_COMMISSION_PERCENTAGE_UNITARY: string = "hidden_RequestedCommissionPercentageUnitary";

        public static ID_RQUESTED_COMMISSION_PERCENTAGE_LCPACKAGE: string = "hidden_RequestedCommissionPercentageLCPackage";
        
        public static ID_TOTAL_REVISED: string = "TotalRevised";

        public static ID_STARTUP_COSTS: string = "StartUpCost";

        public static ID_THIRD_PARTY_COSTS: string = "ThirdPartyEquipmentCosts";

        private suffix: string = "";

        private parentCommissionRequest: CommissionRequest;

        // Fields

        private $projectId: JQuery;
        private $quoteId: JQuery;

        private $requestedMultiplierStepper: JQuery;
        private $requestedMultiplier: JQuery;

        public getRequestedMutiplier(): number {
            var val = parseFloat(this.$requestedMultiplierStepper.val());

            return val < 0 ? 0 : val;
        }
        public setRequestedMultiplier(val: number) {
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
                $('#test').attr('disabled','disabled');
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
        }

        private $requestedCommissionPercentage: JQuery;

        private $requestedCommissionPercentageVRV: JQuery;
        private $requestedCommissionPercentageSplit: JQuery;
        private $requestedCommissionPercentageUnitary: JQuery;
        private $requestedCommissionPercentageLCPackage: JQuery;

        public getRequestedCommissionPercentage() {
           
            return parseFloat(this.$requestedCommissionPercentage.val());
        }

        public setRequestedCommissionPercentage(val: number, suffix: string) {

            this.$requestedCommissionPercentage.val(val.toFixed(3));

            if (suffix == "VRV") {
                 this.$requestedCommissionPercentageVRV.val(val.toFixed(3));
            }
            if (suffix == "Split") {
                 this.$requestedCommissionPercentageSplit.val(val.toFixed(3));
            }
            if (suffix == "Unitary")
            {
                this.$requestedCommissionPercentageUnitary.val(val.toFixed(3));
            }
            if (suffix == "LCPackage") {
                this.$requestedCommissionPercentageLCPackage.val(val.toFixed(3));
            }
        }

        private $approvedCommissionPercentage: JQuery;
        private $displayApprovedCommissionAmount: JQuery;

        public getApprovedCommissionPercentage(): number {
            return parseFloat(this.$approvedCommissionPercentage.val());
        }
        public setApprovedCommissionPercentage(val: number) {
            this.$approvedCommissionPercentage.val(val.toFixed(3));
        }

        private $requestedCommissionAmount: JQuery;
        private $displayRequestedCommissionAmount: JQuery;

        public getRequestedCommissionAmount(): number {
            return parseFloat(this.$requestedCommissionAmount.val());
        }
        public setRequestedCommissionAmount(val: number) {
            if (!val || isNaN(val))
                val = 0;

            this.$requestedCommissionAmount.val(val.toFixed(3));
            this.$displayRequestedCommissionAmount.html(this.getAmountDisplay(val));
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

        private $totalList: JQuery;
        public getTotalList(): any {
            return parseFloat(this.$totalList.val());
        }

        private $totalListWithOther: JQuery;
        public getTotalListWithOther(): any {
            return parseFloat(this.$totalListWithOther.val());
        }

        private $totalRevised: JQuery;
        public getTotalRevised(): any
        {
            return parseFloat(this.$totalRevised.val());
        }
        public setTotalRevised(totalNet: number, startUpCosts: number, ThirdPartyCosts: number) {
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
        }


        private $startUpCosts: JQuery;
        public getStartUpCosts(): any {
            return parseFloat(this.$startUpCosts.val());
        }

        private $thirdPartyCosts: JQuery;
        public getThirdPartyCosts(): any {
            return parseFloat(this.$thirdPartyCosts.val());
        }
        
        // Events
        // TODO: Use a better event model
 

        constructor(suffix, parentCommissionRequest) {
            this.suffix = suffix;
            this.parentCommissionRequest = parentCommissionRequest;

            this.loadFields();
        }

        private loadFields() {

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

        private getMultiplierCategoryType(val: string): number {
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
        }

        public calculateTotals() {

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

        }

        public calculateUnitaryTotals() {
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

        }

        public manuallyCalculateTotal() {
            var me = this;

            var requestMultiplier = this.getRequestedMutiplier();
            var requestPercent = this.getRequestedCommissionPercentage();

            var suffix = this.suffix;

            me.setRequestedMultiplier(requestMultiplier);
            //me.setRequestedCommissionPercentage(requestPercent, this.suffix);

            me.parentCommissionRequest.calculateCommissionRequest();
           
        }

        private roundToNearestHundredths(value: number) {
            if (value == 0) {
                return 0;
            }

            return Math.round(value * 100) / 100;
        }


        public getTotals(): CommissionRequestTotals {
            // Total List
            var totalList: number = this.getTotalList();

            // Total Net

            var requestedMultiplier = this.getRequestedMutiplier();
            
            var totalNet: number = 0;

            if (requestedMultiplier > 0) {
                totalNet = this.getTotalList() * this.getRequestedMutiplier();
            }
            else {
                totalNet = this.getTotalList();
            }

            this.setTotalNet(totalNet);

            //TotalRevised

            var totalRevised: number = this.getTotalRevised();

            var startupCosts: number = this.getStartUpCosts();
            var thirdPartyCosts: number = this.getThirdPartyCosts();

            this.setTotalRevised(totalNet, startupCosts, thirdPartyCosts); 

            // Commission
            var requestedCommission: number = totalNet * (this.getRequestedCommissionPercentage() / 100);
            this.setRequestedCommissionAmount(requestedCommission);

            // Net material value
            var requestedNetMaterialValue: number = totalNet - requestedCommission;
            this.setRequestedNetMaterialValue(requestedNetMaterialValue);

            // Net material multiplier
            var requestedNetMaterialMultiplier: number = requestedNetMaterialValue / totalList;
            this.setRequestedNetMaterialValueMultiplier(requestedNetMaterialMultiplier);

            return new CommissionRequestTotals(totalList, totalNet, this.getRequestedMutiplier(), this.getRequestedCommissionPercentage(),
                requestedCommission, requestedNetMaterialValue, requestedNetMaterialMultiplier);
        }
    }
}