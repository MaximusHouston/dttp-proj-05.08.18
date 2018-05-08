import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../../products/services/product.service';
import { BasketService } from '../../basket/services/basket.service';
import { SystemConfiguratorService } from './services/systemConfigurator.service';
declare var jQuery: any;

//grid
//import { Observable } from 'rxjs/Rx';
import { GridDataResult } from '@progress/kendo-angular-grid';

//import { SortDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'system-configurator',
    //styleUrls: [
    //    // load the default theme (use correct path to node_modules)
    //    'node_modules/@progress/kendo-theme-default/dist/all.css'
    //],
    styleUrls: [
        'app/content/kendo/all.css'
    ],
    templateUrl: 'app/tools/systemConfigurator/system-configurator.component.html'
})

export class SystemConfiguratorComponent implements OnInit {

    public userBasket: any;

    public model: any = "N";
    public outDoorUnitType: any;

    //Search by System Needs
    public packageType: { text: string, value: string } = { text: "Dual Fuel", value: "pkg1" };
    public packageTypes: any = [{ "text": "Dual Fuel", value: "pkg1" },
                                { "text": "Heat Pump", value: "pkg2" },
                                { "text": "Gas/Electric", value: "pkg3" },
                                { "text": "Cool Only", value: "pkg4" }];
    public ceeTier: any;
    public region: any;
    public tonnage: any;
    public minSEER: any;
    public maxSEER: any;
    public minEER: any;
    public maxEER: any;
    public minHSPF: any;
    public maxHSPF: any;
    public txv: { text: string, value: string } = { text: "No Preference", value: "T" };
    public txvOptions: any = [{ "text": "Yes", value: "Y" },
                              { "text": "No", value: "N" },
                              { "text": "No Preference", value: "T" }];
    public status: { text: string, value: string } = { text: "No Preference", value: "S" };
    public statusOptions: any = [{ "text": "Active", value: "Y" },
                                { "text": "Discontinued", value: "N" },
                                { "text": "No Preference", value: "S" }];
    public coil: any;
    public furnace: any;
    public airHandler: any;
    public minAFUE: { text: string, value: any } = { text: "Select...", value: null };
    public minAFUEOptions: any = [{ "text": "80%", value: "80" },
                                  { "text": "90%", value: "90" },
                                  { "text": "92%", value: "92" },
                                  { "text": "95%", value: "95" },
                                  { "text": "96%", value: "96" },
                                  { "text": "97%", value: "97" }
                                 ];
    public maxAFUE: { text: string, value: any } = { text: "Select...", value: null };
    public maxAFUEOptions: any = [{ "text": "80%", value: "80" },
                                  { "text": "90%", value: "90" },
                                  { "text": "92%", value: "92" },
                                  { "text": "95%", value: "95" },
                                  { "text": "96%", value: "96" },
                                  { "text": "97%", value: "97" }
                                 ];

    public flushfit: boolean;

    //Search by Model#
    public outdoorModelNumber: any;
    //public outdoorUnitSearchTerm: any;
    public indoorUnitType: any;
    public coilModelNumber: any;
    public furnaceModelNumber: any;
    public furnaceCoilModelNumber: any;
    public airHandlerModelNumber: any;
    public airHandlerBlowerModelNumber: any;

    //Dropdownlist options
    public defaultItem: { text: string, value: any } = { "text": "Select...", value: null };
   
    //public defaultPackageType: { text: string, value: string } = { "text": "Dual Fuel", value: "pkg1" };
    public outDoorUnitTypes: any;
    public regions: any;
    public tonnageValues: any;

    //public inputValidated: any = true;
    //grid
    public matchupResult: any;
    //public skip: any;
    //public pageSize: any;
    //public sort: any;

    public userGroupTree: any;

    //public testListItems: Array<string> = ["Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball"];

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SystemConfiguratorSvc: SystemConfiguratorService
    ) {

    }

    ngOnChanges() {
        console.log("system config: OnChange");
    }

    ngOnInit() {
        console.log("system config: OnInit");

        this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
        //wait until elements are available
        setTimeout(this.setupDefaults.bind(this), 200); // wait 0.2 sec
        


        //set up active tab
        jQuery(".systemConfig-tab-bar li").click(function () {
            jQuery(this).addClass('active-tab').siblings().removeClass('active-tab');
        });

        //$(activeSubTabId).addClass("active-tab");


    }

    //ngDoCheck() {

    //}

    ngAfterContentInit() {
        console.log("system config: AfterContentInit");

        setTimeout(function () {
            $('#userBasket').insertBefore('#projectTabs');
        }, 1000);

        //$('#userBasket').insertBefore('#projectTabs');
        

    }

    ngAfterViewChecked() {
        console.log("system config: AfterViewChecked");


    }


    public getBasketCallback(resp: any) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");

        }
    }

    ngOnDestroy() {
        $('#content > #userBasket').remove();
        //reset session["BasketQuoteId"] = 0
        this.productSvc.resetBasketQuoteId();
        console.log("system config: OnDestroy");

    }

    public setupDefaults() {
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
    }

    public reset() {
        this.setupDefaults();
        $("#searchBySystemNeeds").addClass('active-tab');
        $("#searchByModelNumber").removeClass('active-tab');

    }

    public searchBy(value: any) {
        if (value == "systemNeeds") {
            if (this.model != "N") {
                this.model = "N";
                this.searchByOnChange();
            }
            
        } else if (value == "modelNumber") {
            if (this.model != "Y") {
                this.model = "Y";
                this.searchByOnChange();
            }
        }
        //this.searchByOnChange();
    }

    public searchByOnChange() {
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

    }

    public packageTypeOnChange() {
        $("#outdoorModelAutoComplete").val(null);
        setTimeout(this.setupRegionDLL.bind(this), 200); // wait 0.2 sec
    }

    public validateInput() {
        var isValidated = true;

        if (this.model == "N") {
            if (this.tonnage == null || this.tonnage == "null") {
                this.toastrSvc.ErrorFadeOut("Tonnage is required.")
                isValidated = false;
                //this.inputValidated = false;
            }
            if (this.minSEER == null) {
                this.toastrSvc.ErrorFadeOut("Min SEER is required.")
                isValidated = false;
            }

            if (this.outDoorUnitType != 'pkg') {
                if (this.coil == null && this.furnace == null && this.airHandler == null) {
                    this.toastrSvc.ErrorFadeOut("Indoor Unit Type is required.")
                    isValidated = false;
                }
            }

        } else if (this.model == "Y") {
            if (this.outdoorModelNumber == null) {
                this.toastrSvc.ErrorFadeOut("Outdoor Unit Model is required.")
                isValidated = false;
            }

        }
        return isValidated;
    }

    public getResult() {
        var self = this;

        if (this.validateInput()) {

            var params = this.mapInputToParams();

            self.loadingIconSvc.Start(jQuery("#systemConfiguratorTool"));

            this.SystemConfiguratorSvc.getSystemMatchupList(params).then((resp: any) => {
                if (!resp.error) {
                    var result = resp.result;
                    //this.concatResult(resp.result);
                    this.matchupResult = result;
                    $('#systemConfigForm').hide();
                    $('#matchupResultGrid').show();

                    self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
                } else {
                    self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
                }
            });
        }



        //this.SystemConfiguratorSvc.getSystemMatchupList(data).then(this.getSystemMatchupListCallBack.bind(this));



    }

    public concatResult(result: any) {
        let data: any = [];
        for (var key in result) {
            if (!result.hasOwnProperty(key)) continue;
            var obj = result[key];
            data = data.concat(obj);

        }
        this.matchupResult = data;
        $('#systemConfigForm').hide();
        $('#matchupResultGrid').show();

    }

    public mapInputToParams() {
        var params = {}
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
            }
        } else if (this.model = "Y") {
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
            }
        }


        return params;
    }

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



    public getTonnageList() {
        this.SystemConfiguratorSvc.getTonnageList().then((resp: any) => {
            if (resp) {
                var tonnageList = resp;
                debugger
            }
        });
    }

    public getEqModelList() {

        //Test api
        this.SystemConfiguratorSvc.getEqModelList({}).then((resp: any) => {
            if (!resp.error) {
                var list = resp.result.modelList;
                debugger
            }
        });
    }


    public setupDropDownLists() {

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
                } else {
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
     


    }

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

    public setupOutdoorModelAutoComplete() {

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

            if (this.value.toString().length >= 2) {

                if (e.keyCode != 38 && e.keyCode != 40 && e.keyCode != 13) { //up or down or enter
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

                    self.SystemConfiguratorSvc.getEqModelList(params).then((resp: any) => {
                        if (!resp.error) {
                            var dataSrc = resp.result.modelList;

                            if (dataSrc.length > 0) {
                                // update outdoorModelDLL DataSource
                                var outdoorModelDLL = $("#outdoorModelAutoComplete").data("kendoAutoComplete");

                                outdoorModelDLL.setDataSource(dataSrc);
                                outdoorModelDLL.search(this.value);
                            } else {
                                self.toastrSvc.ErrorFadeOut("Model# does not match with selected type and brand");
                            }



                        }
                    });
                }

            }


        });// end of outdoorModelAutoComplete keyUp




    }

    public setupCeeTierDDL() {
        var self = this;
        var ceeTierDS = {};
        if (this.outDoorUnitType == "pkg") {
            ceeTierDS = [
                { "text": "No Preference", value: "b4" },
                { "text": "CEE Tier 1", value: "b1" },
                { "text": "CEE Tier 2", value: "b2" }
            ];
        } else {
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
    }

    public setupRegionDLL() {
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
                } else {
                    self.minSEER = "13";
                }
            }
        });

        var regionDDL = $("#regionDDL").data("kendoDropDownList");
        regionDDL.select(0);
        regionDDL.trigger("change");
    }

    public setupTonnageDDL() {
        var self = this;

        this.SystemConfiguratorSvc.getTonnageList().then((resp: any) => {
            if (resp) {
                var tonnageList = resp.result.tonnageList;
                //debugger
                let tonnageListDataSrc: any = [];
                for (var i in tonnageList) {
                    var obj = {
                        "text": tonnageList[i],
                        "value": tonnageList[i]
                    }
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



    }

    public setupRadioButtons() {
        this.model = "N";
        this.ceeTier = "b4";
        this.txv.value = "T";
        this.status.value = "S";
    }

    public onCEETierChange() {
        if (this.outDoorUnitType == "ac") {

            if (this.ceeTier == 'b4') {
                this.minSEER = 13;
                this.minEER = "";
                //this.minHSPF = "";
                $('#minSEER').removeProp('readonly');
                $('#minEER').removeProp('readonly');
            } else if (this.ceeTier == 'b0') {
                this.minSEER = 14.5;
                this.minEER = 12;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            } else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            } else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 13;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            } else if (this.ceeTier == 'b3') {
                this.minSEER = 18;
                this.minEER = 13;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
            }
        } else if (this.outDoorUnitType == "hp") {

            if (this.ceeTier == 'b4') {
                this.minSEER = 14;
                this.minEER = "";
                this.minHSPF = "";
                $('#minSEER').removeProp('readonly');
                $('#minEER').removeProp('readonly');
                $('#minHSPF').removeProp('readonly');
            } else if (this.ceeTier == 'b0') {
                this.minSEER = 14.5;
                this.minEER = 12;
                this.minHSPF = 8.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            } else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                this.minHSPF = 8.5;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            } else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 13;
                this.minHSPF = 9;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            } else if (this.ceeTier == 'b3') {
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
            } else if (this.ceeTier == 'b1') {
                this.minSEER = 15;
                this.minEER = 12.5;
                this.minHSPF = 8.2;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            } else if (this.ceeTier == 'b2') {
                this.minSEER = 16;
                this.minEER = 12;
                this.minHSPF = 8.2;
                $('#minSEER').prop('readonly', true);
                $('#minEER').prop('readonly', true);
                $('#minHSPF').prop('readonly', true);
            }
        }
    }

    public resetIndoorUnit() {
        //this function get called before value is bound to model
        this.coil = null;
        this.furnace = null;
        this.minAFUE = { text: "Select ...", value: null };
        this.maxAFUE = { text: "Select ...", value: null };
        this.flushfit = null;
        this.airHandler = null;
        this.indoorUnitType = null;
    }


    public indoorUnitTypeOnChange() {

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
            } else {
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
                this.furnace = 'furnace'
            }
            if (this.indoorUnitType == 'airHandler') {
                this.airHandler = 'airhandler'
            }

        }



    }

    public setupCoilDDL() {
        var self = this;

        if (this.outdoorModelNumber != null) {
            var coilListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
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


    }




    public setupFurnaceDDL() {
        var self = this;

        if (this.outdoorModelNumber != null) {
            var furnaceListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
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
    }

    public setupFurnaceCoilDDL() {
        var self = this;

        if (this.outdoorModelNumber != null && this.furnaceModelNumber != null) {
            var furnaceCoilListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
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
    }


    public setupAirHandlerDDL() {
        var self = this;

        if (this.outdoorModelNumber != null) {
            var airHandlerListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
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
    }


    public setupAirHandlerBlowerDDL() {
        var self = this;

        if (this.outdoorModelNumber != null && this.airHandlerModelNumber != null) {
            var airHandlerBlowerListDataSrc = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "https://testapi.goodmanmfg.com/EBizWebServices/requestEntry",
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
    }

    ////grid result
    //public dataStateChange({ skip, take, sort }: DataStateChangeEvent): void {
    //    this.skip = skip;
    //    this.pageSize = take;
    //    this.sort = sort;

    //    //this.loadData();
    //}


    //public Test() {
    //    var self = this;
    //    this.userSvc.userGroupsList({ "filter": "" }).then((resp: any) => {
    //        if (!resp.error) {
    //            self.userGroupTree = resp.model;
    //            $("#userGroup")
    //            debugger
    //        }
    //    });
    //}
};