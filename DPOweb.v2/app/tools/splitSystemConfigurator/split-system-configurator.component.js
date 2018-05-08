"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var toastr_service_1 = require("../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../shared/services/loadingIcon.service");
var user_service_1 = require("../../shared/services/user.service");
var systemAccessEnum_1 = require("../../shared/services/systemAccessEnum");
var product_service_1 = require("../../products/services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var splitSystemConfigurator_service_1 = require("./services/splitSystemConfigurator.service");
//import { SortDescriptor } from '@progress/kendo-data-query';
var SplitSystemConfiguratorComponent = /** @class */ (function () {
    //public testListItems: Array<string> = ["Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball"];
    function SplitSystemConfiguratorComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc, SplitSystemConfiguratorSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.SplitSystemConfiguratorSvc = SplitSystemConfiguratorSvc;
        this.voltage = { text: "No Preference", value: "" };
        this.voltageOptions = [{ text: "No Preference", value: "" },
            { "text": "208/230", value: "208/230" },
            { "text": "460", value: "460" }];
        this.txv = { text: "No Preference", value: "T" };
        this.txvOptions = [{ "text": "Yes", value: "Y" },
            { "text": "No", value: "N" },
            { "text": "No Preference", value: "T" }];
        this.status = { text: "No Preference", value: "S" };
        this.statusOptions = [{ "text": "Active", value: "Y" },
            { "text": "Discontinued", value: "N" },
            { "text": "No Preference", value: "S" }];
        //Dropdownlist options
        this.defaultItem = { "text": "Select...", value: null };
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    SplitSystemConfiguratorComponent.prototype.ngOnChanges = function () {
        console.log("split system config: OnChange");
    };
    SplitSystemConfiguratorComponent.prototype.ngOnInit = function () {
        var _this = this;
        console.log("split system config: OnInit");
        this.userSvc.isAuthenticated().then(function (resp) {
            if (resp.isok && resp.model == true) {
                _this.isAuthenticated = true;
            }
            else {
                //Go back to login page
                window.location.href = "/v2/#/account/login";
            }
        });
        this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
        //wait until elements are available
        setTimeout(this.setupDefaults.bind(this), 200); // wait 0.2 sec
        //set up active tab
        jQuery(".systemConfig-tab-bar li").click(function () {
            jQuery(this).addClass('active-tab').siblings().removeClass('active-tab');
        });
    };
    //ngDoCheck() {
    //}
    SplitSystemConfiguratorComponent.prototype.ngAfterContentInit = function () {
        //console.log("system config: AfterContentInit");
        //setTimeout(function () {
        //    $('#userBasket').insertBefore('#projectTabs');
        //}, 1000);
    };
    SplitSystemConfiguratorComponent.prototype.ngAfterViewChecked = function () {
        console.log("split system config: AfterViewChecked");
    };
    SplitSystemConfiguratorComponent.prototype.getBasketCallback = function (resp) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
        }
    };
    SplitSystemConfiguratorComponent.prototype.ngOnDestroy = function () {
        //$('#content > #userBasket').remove();
        ////reset session["BasketQuoteId"] = 0
        //this.productSvc.resetBasketQuoteId();
        //console.log("system config: OnDestroy");
    };
    SplitSystemConfiguratorComponent.prototype.setupDefaults = function () {
        //this.model = "N";
        this.setupRadioButtons();
        this.setupDropDownLists();
        this.minSEER = "13";
        this.maxSEER = null;
        this.minEER = null;
        this.maxEER = null;
        this.minHSPF = null;
        this.maxHSPF = null;
        $('#minSEER').removeProp('readonly');
        $('#minEER').removeProp('readonly');
        this.resetIndoorUnit();
        //this.outdoorModelNumber = null;
    };
    SplitSystemConfiguratorComponent.prototype.reset = function () {
        this.setupDefaults();
        //$("#searchBySystemNeeds").addClass('active-tab');
        //$("#searchByModelNumber").removeClass('active-tab');
    };
    SplitSystemConfiguratorComponent.prototype.validateInput = function () {
        var isValidated = true;
        if (this.tonnage == null || this.tonnage == "null") {
            this.toastrSvc.ErrorFadeOut("Tonnage is required.");
            isValidated = false;
        }
        if (this.minSEER == null) {
            this.toastrSvc.ErrorFadeOut("Min SEER is required.");
            isValidated = false;
        }
        if (this.coil == null && this.airHandler == null) {
            this.toastrSvc.ErrorFadeOut("Indoor Unit Type is required.");
            isValidated = false;
        }
        return isValidated;
    };
    SplitSystemConfiguratorComponent.prototype.getResult = function () {
        var _this = this;
        var self = this;
        if (this.validateInput()) {
            var params = this.mapInputToParams();
            self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));
            this.SplitSystemConfiguratorSvc.getSystemMatchupList(params).then(function (resp) {
                if (!resp.error) {
                    var result = resp.result;
                    ////this.concatResult(resp.result);
                    _this.matchupResult = result;
                    $('#systemConfigForm').hide();
                    $('#splitMatchupResultGrid').show();
                    self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
                }
                else {
                    self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
                }
            });
        }
        //this.SystemConfiguratorSvc.getSystemMatchupList(data).then(this.getSystemMatchupListCallBack.bind(this));
    };
    SplitSystemConfiguratorComponent.prototype.concatResult = function (result) {
        var data = [];
        for (var key in result) {
            if (!result.hasOwnProperty(key))
                continue;
            var obj = result[key];
            data = data.concat(obj);
        }
        this.matchupResult = data;
        $('#systemConfigForm').hide();
        $('#matchupResultGrid').show();
    };
    SplitSystemConfiguratorComponent.prototype.mapInputToParams = function () {
        var params = {
            "user": "",
            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
            "packageName": "SystemMatchupDaikinSplSt",
            "servicesName": "doGetSystemMatchupList",
            "accountId": "goodman1",
            "params": {
                "type": this.outDoorUnitType,
                "tonnage": this.tonnage,
                "voltage": this.voltage.value,
                "min_seer": this.minSEER,
                "max_seer": this.maxSEER,
                "min_ieer": this.minIEER,
                "max_ieer": this.maxIEER,
                "min_eer": this.minEER,
                "max_eer": this.maxEER,
                "min_hspf": this.minHSPF,
                "max_hspf": this.maxHSPF,
                "txv": this.txv.value,
                "status": this.status.value,
                "coil": this.coil,
                "airhandler": this.airHandler
            }
        };
        return params;
    };
    //public getSystemMatchupListCallBack(resp: any) {
    //    if (!resp.error) {
    //        var result = resp.result;
    //        let data: any = [];
    //        for (var key in result) {
    //            if (!result.hasOwnProperty(key)) continue;
    //            var obj = result[key];
    //            data = data.concat(obj);
    //        }
    //        this.matchupResult = data;
    //    }
    //}
    SplitSystemConfiguratorComponent.prototype.getTonnageList = function () {
        //this.SystemConfiguratorSvc.getTonnageList().then((resp: any) => {
        //    if (resp) {
        //        var tonnageList = resp;
        //        debugger
        //    }
        //});
    };
    SplitSystemConfiguratorComponent.prototype.getEqModelList = function () {
        //Test api
        //this.SystemConfiguratorSvc.getEqModelList({}).then((resp: any) => {
        //    if (!resp.error) {
        //        var list = resp.result.modelList;
        //        debugger
        //    }
        //});
    };
    SplitSystemConfiguratorComponent.prototype.setupDropDownLists = function () {
        this.outDoorUnitTypes = [
            { "text": "Air Conditioner", value: "ac" },
            { "text": "Heat Pump", value: "hp" }
        ];
        var self = this;
        $("#outDoorUnitTypeDDL").kendoDropDownList({
            dataSource: self.outDoorUnitTypes,
            dataTextField: "text",
            dataValueField: "value",
            change: function (e) {
                var value = this.value();
                self.outDoorUnitType = value;
                //if (self.model == "N") {
                //    self.ceeTier = "b4";
                //    self.onCEETierChange();
                //}
            }
        });
        var outDoorUnitTypeDDL = $("#outDoorUnitTypeDDL").data("kendoDropDownList");
        outDoorUnitTypeDDL.select(0);
        outDoorUnitTypeDDL.trigger("change");
        //setTimeout(self.setupCeeTierDDL.bind(self), 200);
        //setTimeout(self.setupRegionDLL.bind(self), 200);
        setTimeout(self.setupTonnageDDL.bind(self), 200);
    };
    SplitSystemConfiguratorComponent.prototype.setupTonnageDDL = function () {
        var self = this;
        this.SplitSystemConfiguratorSvc.getTonnageList().then(function (resp) {
            if (resp) {
                var tonnageList = resp.result.tonnageList;
                //debugger
                var tonnageListDataSrc = [];
                for (var i in tonnageList) {
                    var obj = {
                        "text": tonnageList[i],
                        "value": tonnageList[i]
                    };
                    //debugger
                    tonnageListDataSrc.push(obj);
                }
                if ($("#tonnageDDL").length > 0) {
                    $("#tonnageDDL").kendoDropDownList({
                        dataSource: tonnageListDataSrc,
                        dataTextField: "text",
                        dataValueField: "value",
                        optionLabel: {
                            text: "Select ...",
                            value: null
                        },
                        change: function (e) {
                            var value = this.value();
                            self.tonnage = value;
                            //debugger
                        }
                    });
                    //debugger
                    var tonnageDDL = $("#tonnageDDL").data("kendoDropDownList");
                    tonnageDDL.select(0);
                    tonnageDDL.trigger("change");
                }
            }
        });
    };
    SplitSystemConfiguratorComponent.prototype.setupRadioButtons = function () {
        //this.model = "N";
        //this.ceeTier = "b4";
        this.txv.value = "T";
        this.status.value = "S";
    };
    SplitSystemConfiguratorComponent.prototype.resetIndoorUnit = function () {
        //this function get called before value is bound to model
        this.coil = null;
        //this.furnace = null;
        //this.minAFUE = { text: "Select ...", value: null };
        //this.maxAFUE = { text: "Select ...", value: null };
        //this.flushfit = null;
        this.airHandler = null;
        this.indoorUnitType = null;
    };
    SplitSystemConfiguratorComponent.prototype.indoorUnitTypeOnChange = function () {
        //reset
        this.coil = null;
        //this.furnace = null;
        //this.minAFUE = { text: "Select ...", value: null };
        //this.maxAFUE = { text: "Select ...", value: null };
        //this.flushfit = null;
        this.airHandler = null;
        if (this.indoorUnitType == 'coilOnly') {
            this.coil = 'coil';
        }
        //if (this.indoorUnitType == 'furnaceCoil') {
        //    this.furnace = 'furnace'
        //}
        if (this.indoorUnitType == 'airHandler') {
            this.airHandler = 'airhandler';
        }
    };
    SplitSystemConfiguratorComponent = __decorate([
        core_1.Component({
            selector: 'split-system-configurator',
            styleUrls: [
                'app/content/kendo/all.css'
            ],
            templateUrl: 'app/tools/splitSystemConfigurator/split-system-configurator.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService,
            splitSystemConfigurator_service_1.SplitSystemConfiguratorService])
    ], SplitSystemConfiguratorComponent);
    return SplitSystemConfiguratorComponent;
}());
exports.SplitSystemConfiguratorComponent = SplitSystemConfiguratorComponent;
;
//# sourceMappingURL=split-system-configurator.component.js.map