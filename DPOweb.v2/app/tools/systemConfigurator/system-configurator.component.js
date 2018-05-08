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
var webconfig_service_1 = require("../../shared/services/webconfig.service");
var product_service_1 = require("../../products/services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var systemConfigurator_service_1 = require("./services/systemConfigurator.service");
//import { SortDescriptor } from '@progress/kendo-data-query';
var SystemConfiguratorComponent = /** @class */ (function () {
    function SystemConfiguratorComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, webconfigSvc, productSvc, basketSvc, SystemConfiguratorSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.webconfigSvc = webconfigSvc;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.SystemConfiguratorSvc = SystemConfiguratorSvc;
        this.model = "N";
        //Search by System Needs
        this.packageType = { text: "Dual Fuel", value: "pkg1" };
        this.packageTypes = [{ "text": "Dual Fuel", value: "pkg1" },
            { "text": "Heat Pump", value: "pkg2" },
            { "text": "Gas/Electric", value: "pkg3" },
            { "text": "Cool Only", value: "pkg4" }];
        this.txv = { text: "No Preference", value: "T" };
        this.txvOptions = [{ "text": "Yes", value: "Y" },
            { "text": "No", value: "N" },
            { "text": "No Preference", value: "T" }];
        this.status = { text: "No Preference", value: "S" };
        this.statusOptions = [{ "text": "Active", value: "Y" },
            { "text": "Discontinued", value: "N" },
            { "text": "No Preference", value: "S" }];
        this.minAFUE = { text: "Select...", value: null };
        this.minAFUEOptions = [{ "text": "80%", value: "80" },
            { "text": "90%", value: "90" },
            { "text": "92%", value: "92" },
            { "text": "95%", value: "95" },
            { "text": "96%", value: "96" },
            { "text": "97%", value: "97" }
        ];
        this.maxAFUE = { text: "Select...", value: null };
        this.maxAFUEOptions = [{ "text": "80%", value: "80" },
            { "text": "90%", value: "90" },
            { "text": "92%", value: "92" },
            { "text": "95%", value: "95" },
            { "text": "96%", value: "96" },
            { "text": "97%", value: "97" }
        ];
        //Dropdownlist options
        this.defaultItem = { "text": "Select...", value: null };
        var self = this;
        this.webconfigSvc.getWebConfig().then(function (resp) {
            self.webconfig = resp;
            self.unitaryMCToolURL = self.webconfig.routeConfig.unitaryMatchupToolURL;
        }).catch(function (error) {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    SystemConfiguratorComponent.prototype.ngOnChanges = function () {
        console.log("system config: OnChange");
    };
    SystemConfiguratorComponent.prototype.ngOnInit = function () {
        var _this = this;
        console.log("system config: OnInit");
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
        //$(activeSubTabId).addClass("active-tab");
    };
    //ngDoCheck() {
    //}
    SystemConfiguratorComponent.prototype.ngAfterContentInit = function () {
        console.log("system config: AfterContentInit");
        //this.userSvc.isAuthenticated().then((resp: any) => {
        //    if (resp.isok && resp.model == true) {
        //        this.isAuthenticated = true;
        //    }
        //    else{
        //       //Go back to login page
        //        window.location.href = "/v2/#/account/login";
        //    }
        //});
        //setTimeout(function () {
        //    $('#userBasket').insertBefore('#projectTabs');
        //}, 1000);
        //$('#userBasket').insertBefore('#projectTabs');
    };
    SystemConfiguratorComponent.prototype.ngAfterViewChecked = function () {
        console.log("system config: AfterViewChecked");
    };
    SystemConfiguratorComponent.prototype.getBasketCallback = function (resp) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
        }
    };
    SystemConfiguratorComponent.prototype.ngOnDestroy = function () {
        $('#content > #userBasket').remove();
        //reset session["BasketQuoteId"] = 0
        this.productSvc.resetBasketQuoteId();
        console.log("system config: OnDestroy");
    };
    SystemConfiguratorComponent.prototype.setupDefaults = function () {
        this.model = "N";
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
        this.outdoorModelNumber = null;
    };
    SystemConfiguratorComponent.prototype.reset = function () {
        this.setupDefaults();
        $("#searchBySystemNeeds").addClass('active-tab');
        $("#searchByModelNumber").removeClass('active-tab');
    };
    SystemConfiguratorComponent.prototype.searchBy = function (value) {
        if (value == "systemNeeds") {
            jQuery("#searchBySystemNeeds").addClass('active-tab').siblings().removeClass('active-tab');
            if (this.model != "N") {
                this.model = "N";
                this.searchByOnChange();
            }
        }
        else if (value == "modelNumber") {
            jQuery("#searchByModelNumber").addClass('active-tab').siblings().removeClass('active-tab');
            if (this.model != "Y") {
                this.model = "Y";
                this.searchByOnChange();
            }
        }
        //this.searchByOnChange();
    };
    SystemConfiguratorComponent.prototype.searchByOnChange = function () {
        var self = this;
        if (this.model == "Y") {
            //wait until $("#outdoorModelAutoComplete") is available
            setTimeout(this.setupOutdoorModelAutoComplete.bind(this), 200); // wait 0.2 sec
            this.outdoorModelNumber = null;
        }
        if (this.model == "N") {
            //wait until element is available
            setTimeout(this.setupTonnageDDL.bind(this), 200); // wait 0.2 sec
            setTimeout(this.setupCeeTierDDL.bind(this), 200); // wait 0.2 sec
        }
        this.indoorUnitType = null;
    };
    SystemConfiguratorComponent.prototype.packageTypeOnChange = function () {
        $("#outdoorModelAutoComplete").val(null);
        setTimeout(this.setupRegionDLL.bind(this), 200); // wait 0.2 sec
    };
    SystemConfiguratorComponent.prototype.validateInput = function () {
        var isValidated = true;
        if (this.model == "N") {
            if (this.tonnage == null || this.tonnage == "null") {
                this.toastrSvc.ErrorFadeOut("Tonnage is required.");
                isValidated = false;
                //this.inputValidated = false;
            }
            if (this.minSEER == null) {
                this.toastrSvc.ErrorFadeOut("Min SEER is required.");
                isValidated = false;
            }
            if (this.outDoorUnitType != 'pkg') {
                if (this.coil == null && this.furnace == null && this.airHandler == null) {
                    this.toastrSvc.ErrorFadeOut("Indoor Unit Type is required.");
                    isValidated = false;
                }
            }
        }
        else if (this.model == "Y") {
            if (this.outdoorModelNumber == null) {
                this.toastrSvc.ErrorFadeOut("Outdoor Unit Model is required.");
                isValidated = false;
            }
        }
        return isValidated;
    };
    SystemConfiguratorComponent.prototype.getResult = function () {
        var _this = this;
        var self = this;
        if (this.validateInput()) {
            var params = this.mapInputToParams();
            self.loadingIconSvc.Start(jQuery("#systemConfiguratorTool"));
            this.SystemConfiguratorSvc.getSystemMatchupList(params).then(function (resp) {
                if (!resp.error) {
                    var result = resp.result;
                    //this.concatResult(resp.result);
                    _this.matchupResult = result;
                    $('#systemConfigForm').hide();
                    $('#matchupResultGrid').show();
                    self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
                }
                else {
                    self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
                }
            });
        }
        //this.SystemConfiguratorSvc.getSystemMatchupList(data).then(this.getSystemMatchupListCallBack.bind(this));
    };
    SystemConfiguratorComponent.prototype.concatResult = function (result) {
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
    SystemConfiguratorComponent.prototype.mapInputToParams = function () {
        var params = {};
        //map selections to params
        if (this.model == "N") {
            //Search by System Needs
            params = {
                "user": "",
                "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                "packageName": "SystemMatchupDaikin",
                "servicesName": "doGetSystemMatchupList",
                "accountId": "goodman1",
                "params": {
                    "model": this.model,
                    "type": this.outDoorUnitType,
                    "pkgtype": this.packageType.value,
                    "region": this.region,
                    "tonnage": this.tonnage,
                    "min_seer": this.minSEER,
                    "max_seer": this.maxSEER,
                    "min_eer": this.minEER,
                    "max_eer": this.maxEER,
                    "min_hspf": this.minHSPF,
                    "max_hspf": this.maxHSPF,
                    "txv": this.txv.value,
                    "status": this.status.value,
                    "coil": this.coil,
                    "airhandler": this.airHandler,
                    "furnace": this.furnace,
                    "min_afue": this.minAFUE.value,
                    "max_afue": this.maxAFUE.value,
                    "flushfit": this.flushfit
                }
            };
        }
        else if (this.model = "Y") {
            //search by model #
            params = {
                "user": "",
                "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                "packageName": "SystemMatchupDaikin",
                "servicesName": "doGetSystemMatchupList",
                "accountId": "goodman1",
                "params": {
                    "model": this.model,
                    "type": this.outDoorUnitType,
                    "pkgtype": this.packageType.value,
                    "region": this.region,
                    "modelnumber": this.outdoorModelNumber,
                    "coil": this.coilModelNumber,
                    "furnace": this.furnaceModelNumber,
                    "furnaceCoil": this.furnaceCoilModelNumber,
                    "airhandler": this.airHandlerModelNumber,
                    "blower": this.airHandlerBlowerModelNumber
                }
            };
        }
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
    SystemConfiguratorComponent.prototype.getTonnageList = function () {
        this.SystemConfiguratorSvc.getTonnageList().then(function (resp) {
            if (resp) {
                var tonnageList = resp;
                debugger;
            }
        });
    };
    SystemConfiguratorComponent.prototype.getEqModelList = function () {
        //Test api
        this.SystemConfiguratorSvc.getEqModelList({}).then(function (resp) {
            if (!resp.error) {
                var list = resp.result.modelList;
                debugger;
            }
        });
    };
    SystemConfiguratorComponent.prototype.setupDropDownLists = function () {
        this.outDoorUnitTypes = [
            { "text": "Air Conditioner", value: "ac" },
            { "text": "Heat Pump", value: "hp" },
            { "text": "Package", value: "pkg" }
        ];
        var self = this;
        $("#outDoorUnitTypeDDL").kendoDropDownList({
            dataSource: self.outDoorUnitTypes,
            dataTextField: "text",
            dataValueField: "value",
            change: function (e) {
                var value = this.value();
                self.outDoorUnitType = value;
                if (self.model == "N") {
                    self.ceeTier = "b4";
                    self.onCEETierChange();
                }
                if (self.model == "Y") {
                    //reset selections
                    self.outdoorModelNumber = null;
                    $("#outdoorModelAutoComplete").val(null);
                    //Reset outdoorModelAutoComplete dataSource
                    var outdoorModelDLL = $("#outdoorModelAutoComplete").data("kendoAutoComplete");
                    var emptyDataSrc = new kendo.data.DataSource();
                    outdoorModelDLL.setDataSource(emptyDataSrc);
                    self.indoorUnitType = null;
                }
                if (value == "pkg") {
                    self.packageType.text = "Dual Fuel";
                    self.packageType.value = "pkg1";
                    setTimeout(self.setupCeeTierDDL.bind(self), 200);
                    self.region = null;
                    self.txv.value = "T";
                    self.indoorUnitType = null;
                }
                else {
                    setTimeout(self.setupRegionDLL.bind(self), 200);
                    setTimeout(self.setupCeeTierDDL.bind(self), 200);
                }
            }
        });
        var outDoorUnitTypeDDL = $("#outDoorUnitTypeDDL").data("kendoDropDownList");
        outDoorUnitTypeDDL.select(0);
        outDoorUnitTypeDDL.trigger("change");
        setTimeout(self.setupCeeTierDDL.bind(self), 200);
        setTimeout(self.setupRegionDLL.bind(self), 200);
        setTimeout(self.setupTonnageDDL.bind(self), 200);
    };
    //public resetCeeTierDDL() {
    //    var ceeTierDDL = $("#ceeTierDDL").data("kendoDropDownList");
    //    if (this.outDoorUnitType == "ac" || this.outDoorUnitType == "hp") {
    //        var ceeTierDS = new kendo.data.DataSource({
    //            data: [
    //                { "text": "No Preference", value: "b4" },
    //                { "text": "CEE Tier 0", value: "b0" },
    //                { "text": "CEE Tier 1", value: "b1" },
    //                { "text": "CEE Tier 2", value: "b2" },
    //                { "text": "CEE Tier 3", value: "b3" }
    //            ]
    //        });
    //        ceeTierDDL.setDataSource(ceeTierDS);
    //    } else if (this.outDoorUnitType = "pkg"){
    //        var ceeTierDS = new kendo.data.DataSource({
    //            data: [
    //                { "text": "No Preference", value: "b4" },
    //                { "text": "CEE Tier 1", value: "b1" },
    //                { "text": "CEE Tier 2", value: "b2" }
    //            ]
    //        });
    //        ceeTierDDL.setDataSource(ceeTierDS);
    //    }
    //}
    SystemConfiguratorComponent.prototype.setupOutdoorModelAutoComplete = function () {
        var self = this;
        $("#outdoorModelAutoComplete").kendoAutoComplete({
            filter: "contains",
            placeholder: " Enter model#",
            minLength: 2,
            dataSource: [],
            change: function (e) {
                var value = this.value();
                self.outdoorModelNumber = value;
                // need to reset indoor DDLs
                self.indoorUnitType = null;
            }
        });
        $("#outdoorModelAutoComplete").keyup(function (e) {
            var _this = this;
            if (this.value.toString().length >= 2) {
                if (e.keyCode != 38 && e.keyCode != 40 && e.keyCode != 13) {
                    //debugger
                    var params = {
                        "user": "",
                        "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                        "packageName": "SystemMatchupDaikin",
                        "servicesName": "doGetEqModelList",
                        "accountId": "goodman1",
                        "params": {
                            "model": "N",
                            "type": self.outDoorUnitType,
                            "modelnumber": this.value,
                            "pkgtype": self.packageType.value,
                            "region": self.region
                        }
                    };
                    self.SystemConfiguratorSvc.getEqModelList(params).then(function (resp) {
                        if (!resp.error) {
                            var dataSrc = resp.result.modelList;
                            if (dataSrc.length > 0) {
                                // update outdoorModelDLL DataSource
                                var outdoorModelDLL = $("#outdoorModelAutoComplete").data("kendoAutoComplete");
                                outdoorModelDLL.setDataSource(dataSrc);
                                outdoorModelDLL.search(_this.value);
                            }
                            else {
                                self.toastrSvc.ErrorFadeOut("Model# does not match with selected type and brand");
                            }
                        }
                    });
                }
            }
        }); // end of outdoorModelAutoComplete keyUp
    };
    SystemConfiguratorComponent.prototype.setupCeeTierDDL = function () {
        var self = this;
        var ceeTierDS = {};
        if (this.outDoorUnitType == "pkg") {
            ceeTierDS = [
                { "text": "No Preference", value: "b4" },
                { "text": "CEE Tier 1", value: "b1" },
                { "text": "CEE Tier 2", value: "b2" }
            ];
        }
        else {
            ceeTierDS = [
                { "text": "No Preference", value: "b4" },
                { "text": "CEE Tier 0", value: "b0" },
                { "text": "CEE Tier 1", value: "b1" },
                { "text": "CEE Tier 2", value: "b2" },
                { "text": "CEE Tier 3", value: "b3" }
            ];
        }
        $("#ceeTierDDL").kendoDropDownList({
            dataSource: ceeTierDS,
            dataTextField: "text",
            dataValueField: "value",
            change: function (e) {
                var value = this.value();
                self.ceeTier = value;
                self.onCEETierChange();
            }
        });
    };
    SystemConfiguratorComponent.prototype.setupRegionDLL = function () {
        var self = this;
        this.regions = [
            { "text": "North", value: "north" },
            { "text": "SouthEast", value: "se" },
            { "text": "SouthWest", value: "sw" }
        ];
        $("#regionDDL").kendoDropDownList({
            dataSource: self.regions,
            dataTextField: "text",
            dataValueField: "value",
            change: function (e) {
                var value = this.value();
                self.region = value;
                if (value == 'se') {
                    self.minSEER = "14";
                }
                else {
                    self.minSEER = "13";
                }
            }
        });
        var regionDDL = $("#regionDDL").data("kendoDropDownList");
        regionDDL.select(0);
        regionDDL.trigger("change");
    };
    SystemConfiguratorComponent.prototype.setupTonnageDDL = function () {
        var self = this;
        this.SystemConfiguratorSvc.getTonnageList().then(function (resp) {
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
    SystemConfiguratorComponent.prototype.setupRadioButtons = function () {
        this.model = "N";
        this.ceeTier = "b4";
        this.txv.value = "T";
        this.status.value = "S";
    };
    SystemConfiguratorComponent.prototype.onCEETierChange = function () {
        if (this.outDoorUnitType == "ac") {
            if (this.ceeTier == 'b4') {
                this.minSEER = 13;
                this.minEER = "";
                //this.minHSPF = "";
                $('#minSEER').removeProp('readonly');
                $('#minEER').removeProp('readonly');
            }
            else if (this.ceeTier == 'b0') {
                this.minSEER = 14.5;
                this.minEER = 12;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            }
            else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            }
            else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 13;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            }
            else if (this.ceeTier == 'b3') {
                this.minSEER = 18;
                this.minEER = 13;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            }
        }
        else if (this.outDoorUnitType == "hp") {
            if (this.ceeTier == 'b4') {
                this.minSEER = 14;
                this.minEER = "";
                this.minHSPF = "";
                $('#minSEER').removeProp('readonly');
                $('#minEER').removeProp('readonly');
                $('#minHSPF').removeProp('readonly');
            }
            else if (this.ceeTier == 'b0') {
                this.minSEER = 14.5;
                this.minEER = 12;
                this.minHSPF = 8.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
            else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                this.minHSPF = 8.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
            else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 13;
                this.minHSPF = 9;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
            else if (this.ceeTier == 'b3') {
                this.minSEER = 18;
                this.minEER = 13;
                this.minHSPF = 10;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
        }
        else if (this.outDoorUnitType == "pkg") {
            if (this.ceeTier == 'b4') {
                this.minSEER = 14;
                this.minEER = "";
                this.minHSPF = "";
                $('#minSEER').removeProp('readonly');
                $('#minEER').removeProp('readonly');
                $('#minHSPF').removeProp('readonly');
            }
            else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                this.minHSPF = 8.2;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
            else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 12;
                this.minHSPF = 8.2;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
        }
    };
    SystemConfiguratorComponent.prototype.resetIndoorUnit = function () {
        //this function get called before value is bound to model
        this.coil = null;
        this.furnace = null;
        this.minAFUE = { text: "Select ...", value: null };
        this.maxAFUE = { text: "Select ...", value: null };
        this.flushfit = null;
        this.airHandler = null;
        this.indoorUnitType = null;
    };
    SystemConfiguratorComponent.prototype.indoorUnitTypeOnChange = function () {
        if (this.model == 'Y') {
            this.coilModelNumber = null;
            this.furnaceModelNumber = null;
            this.furnaceCoilModelNumber = null;
            this.airHandlerModelNumber = null;
            this.airHandlerBlowerModelNumber = null;
            if (this.outdoorModelNumber != null && this.outdoorModelNumber != "") {
                if (this.indoorUnitType == 'coilOnly') {
                    //wait until element is available
                    setTimeout(this.setupCoilDDL.bind(this), 200); // wait 0.2 sec
                }
                if (this.indoorUnitType == 'furnaceCoil') {
                    //wait until element is available
                    setTimeout(this.setupFurnaceDDL.bind(this), 200); // wait 0.2 sec
                }
                if (this.indoorUnitType == 'airHandler') {
                    //wait until element is available
                    setTimeout(this.setupAirHandlerDDL.bind(this), 200); // wait 0.2 sec
                }
            }
            else {
                this.indoorUnitType = null;
                this.toastrSvc.ErrorFadeOut("Please Enter Outdoor Unit Model Number");
                var indoorUnitTypeRadios = document.getElementsByName("indoorUnitType");
                for (var i = 0; i < indoorUnitTypeRadios.length; i++) {
                    //indoorUnitTypeRadios[i].checked = false
                    $(indoorUnitTypeRadios[i]).prop('checked', false);
                }
            }
        }
        if (this.model == 'N') {
            //reset
            this.coil = null;
            this.furnace = null;
            this.minAFUE = { text: "Select ...", value: null };
            this.maxAFUE = { text: "Select ...", value: null };
            this.flushfit = null;
            this.airHandler = null;
            if (this.indoorUnitType == 'coilOnly') {
                this.coil = 'coil';
            }
            if (this.indoorUnitType == 'furnaceCoil') {
                this.furnace = 'furnace';
            }
            if (this.indoorUnitType == 'airHandler') {
                this.airHandler = 'airhandler';
            }
        }
    };
    SystemConfiguratorComponent.prototype.setupCoilDDL = function () {
        var self = this;
        if (this.outdoorModelNumber != null) {
            var coilListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
                        url: self.unitaryMCToolURL,
                        type: "post",
                        contentType: "application/json",
                        dataType: "json",
                        data: {
                            "user": "",
                            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                            "packageName": "SystemMatchupDaikin",
                            "servicesName": "doGetEqCoilList",
                            "accountId": "goodman1",
                            "params": {
                                "type": self.outDoorUnitType,
                                "modelnumber": self.outdoorModelNumber
                            }
                        }
                    },
                    parameterMap: function (data, operation) {
                        if (operation == "read") {
                            return kendo.stringify(data);
                        }
                    }
                },
                schema: {
                    data: "result.coilList"
                }
            });
            if ($("#coilDDL").length > 0) {
                $("#coilDDL").kendoDropDownList({
                    dataSource: coilListDataSrc,
                    optionLabel: "Select...",
                    dataTextField: "coill_Model",
                    dataValueField: "coill_Model",
                    change: function (e) {
                        var value = this.value();
                        self.coilModelNumber = value;
                    }
                });
                //debugger
                //var tonnageDDL = $("#tonnageDDL").data("kendoDropDownList");
                //tonnageDDL.select(0);
                //tonnageDDL.trigger("change");
            }
        }
    };
    SystemConfiguratorComponent.prototype.setupFurnaceDDL = function () {
        var self = this;
        if (this.outdoorModelNumber != null) {
            var furnaceListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
                        url: self.unitaryMCToolURL,
                        type: "post",
                        contentType: "application/json",
                        dataType: "json",
                        data: {
                            "user": "",
                            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                            "packageName": "SystemMatchupDaikin",
                            "servicesName": "doGetEqFurnaceList",
                            "accountId": "goodman1",
                            "params": {
                                "type": self.outDoorUnitType,
                                "modelnumber": self.outdoorModelNumber
                            }
                        }
                    },
                    parameterMap: function (data, operation) {
                        if (operation == "read") {
                            return kendo.stringify(data);
                        }
                    }
                },
                schema: {
                    data: "result.furnaceList"
                }
            });
            if ($("#furnaceDDL").length > 0) {
                $("#furnaceDDL").kendoDropDownList({
                    dataSource: furnaceListDataSrc,
                    optionLabel: "Select...",
                    dataTextField: "furnace_Model",
                    dataValueField: "furnace_Model",
                    change: function (e) {
                        var value = this.value();
                        self.furnaceModelNumber = value;
                        if (self.furnaceModelNumber != null) {
                            setTimeout(self.setupFurnaceCoilDDL.bind(self), 200);
                        }
                    }
                });
            }
        }
    };
    SystemConfiguratorComponent.prototype.setupFurnaceCoilDDL = function () {
        var self = this;
        if (this.outdoorModelNumber != null && this.furnaceModelNumber != null) {
            var furnaceCoilListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
                        url: self.unitaryMCToolURL,
                        type: "post",
                        contentType: "application/json",
                        dataType: "json",
                        data: {
                            "user": "",
                            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                            "packageName": "SystemMatchupDaikin",
                            "servicesName": "doGetEqFurnaceCoilList",
                            "accountId": "goodman1",
                            "params": {
                                "type": self.outDoorUnitType,
                                "modelnumber": self.outdoorModelNumber,
                                "furnace": self.furnaceModelNumber
                            }
                        }
                    },
                    parameterMap: function (data, operation) {
                        if (operation == "read") {
                            return kendo.stringify(data);
                        }
                    }
                },
                schema: {
                    data: "result.furnaceCoilList"
                }
            });
            if ($("#furnaceCoilDDL").length > 0) {
                $("#furnaceCoilDDL").kendoDropDownList({
                    dataSource: furnaceCoilListDataSrc,
                    optionLabel: "Select...",
                    dataTextField: "coill_Model",
                    dataValueField: "coill_Model",
                    change: function (e) {
                        var value = this.value();
                        self.furnaceCoilModelNumber = value; // this might be bound to coilModelNumber instead
                    }
                });
            }
        }
    };
    SystemConfiguratorComponent.prototype.setupAirHandlerDDL = function () {
        var self = this;
        if (this.outdoorModelNumber != null) {
            var airHandlerListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
                        url: self.unitaryMCToolURL,
                        type: "post",
                        contentType: "application/json",
                        dataType: "json",
                        data: {
                            "user": "",
                            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                            "packageName": "SystemMatchupDaikin",
                            "servicesName": "doGetEqAirHandlerList",
                            "accountId": "goodman1",
                            "params": {
                                "type": self.outDoorUnitType,
                                "modelnumber": self.outdoorModelNumber
                            }
                        }
                    },
                    parameterMap: function (data, operation) {
                        if (operation == "read") {
                            return kendo.stringify(data);
                        }
                    }
                },
                schema: {
                    data: "result.airhandlerList"
                }
            });
            if ($("#airHandlerDDL").length > 0) {
                $("#airHandlerDDL").kendoDropDownList({
                    dataSource: airHandlerListDataSrc,
                    optionLabel: "Select...",
                    dataTextField: "coill_Model",
                    dataValueField: "coill_Model",
                    change: function (e) {
                        var value = this.value();
                        self.airHandlerModelNumber = value;
                        if (self.airHandlerModelNumber != null) {
                            setTimeout(self.setupAirHandlerBlowerDDL.bind(self), 200);
                        }
                    }
                });
            }
        }
    };
    SystemConfiguratorComponent.prototype.setupAirHandlerBlowerDDL = function () {
        var self = this;
        if (this.outdoorModelNumber != null && this.airHandlerModelNumber != null) {
            var airHandlerBlowerListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        //url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
                        url: self.unitaryMCToolURL,
                        type: "post",
                        contentType: "application/json",
                        dataType: "json",
                        data: {
                            "user": "",
                            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
                            "packageName": "SystemMatchupDaikin",
                            "servicesName": "doGetEqAirHandlerBlowerList",
                            "accountId": "goodman1",
                            "params": {
                                "type": self.outDoorUnitType,
                                "modelnumber": self.outdoorModelNumber,
                                "airhandler": self.airHandlerModelNumber
                            }
                        }
                    },
                    parameterMap: function (data, operation) {
                        if (operation == "read") {
                            return kendo.stringify(data);
                        }
                    }
                },
                schema: {
                    data: "result.airhandlerBlowerList"
                }
            });
            if ($("#airHandlerBlowerDDL").length > 0) {
                $("#airHandlerBlowerDDL").kendoDropDownList({
                    dataSource: airHandlerBlowerListDataSrc,
                    optionLabel: "Select...",
                    dataTextField: "blower_Model",
                    dataValueField: "blower_Model",
                    change: function (e) {
                        var value = this.value();
                        self.airHandlerBlowerModelNumber = value;
                    }
                });
            }
        }
    };
    SystemConfiguratorComponent = __decorate([
        core_1.Component({
            selector: 'system-configurator',
            //styleUrls: [
            //    // load the default theme (use correct path to node_modules)
            //    'node_modules/@progress/kendo-theme-default/dist/all.css'
            //],
            styleUrls: [
                'app/content/kendo/all.css'
            ],
            templateUrl: 'app/tools/systemConfigurator/system-configurator.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, webconfig_service_1.WebConfigService,
            product_service_1.ProductService, basket_service_1.BasketService,
            systemConfigurator_service_1.SystemConfiguratorService])
    ], SystemConfiguratorComponent);
    return SystemConfiguratorComponent;
}());
exports.SystemConfiguratorComponent = SystemConfiguratorComponent;
;
//# sourceMappingURL=system-configurator.component.js.map