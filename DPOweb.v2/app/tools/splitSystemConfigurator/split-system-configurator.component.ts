import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../../products/services/product.service';
import { BasketService } from '../../basket/services/basket.service';
import { SplitSystemConfiguratorService } from './services/splitSystemConfigurator.service';
declare var jQuery: any;

//grid
//import { Observable } from 'rxjs/Rx';
import { GridDataResult } from '@progress/kendo-angular-grid';

//import { SortDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'split-system-configurator',
    
    styleUrls: [
        'app/content/kendo/all.css'
    ],
    templateUrl: 'app/tools/splitSystemConfigurator/split-system-configurator.component.html'
})

export class SplitSystemConfiguratorComponent implements OnInit {

    public user: any;
    public isAuthenticated: boolean;

    public userBasket: any;

    //public model: any = "N";
    public outDoorUnitType: any;

    //Search by System Needs
    //public packageType: { text: string, value: string } = { text: "Dual Fuel", value: "pkg1" };
    //public packageTypes: any = [{ "text": "Dual Fuel", value: "pkg1" },
    //{ "text": "Heat Pump", value: "pkg2" },
    //{ "text": "Gas/Electric", value: "pkg3" },
    //{ "text": "Cool Only", value: "pkg4" }];
    //public ceeTier: any;

    public region: any;
    public tonnage: any;
    public tonnageValues: any;

    public voltage: { text: string, value: string } = { text: "No Preference", value: "" };
    public voltageOptions: any = [{ text: "No Preference", value: "" },
                                { "text": "208/230", value: "208/230" },
                                { "text": "460", value: "460" }];
    public minSEER: any;
    public maxSEER: any;
    public minIEER: any;
    public maxIEER: any;
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
    //public furnace: any;
    public airHandler: any;
   

  

    //Search by Model#
    //public outdoorModelNumber: any;
    //public outdoorUnitSearchTerm: any;
    public indoorUnitType: any;
    public coilModelNumber: any;
    //public furnaceModelNumber: any;
    //public furnaceCoilModelNumber: any;
    public airHandlerModelNumber: any;
    public airHandlerBlowerModelNumber: any;

    //Dropdownlist options
    public defaultItem: { text: string, value: any } = { "text": "Select...", value: null };

    //public defaultPackageType: { text: string, value: string } = { "text": "Dual Fuel", value: "pkg1" };
    public outDoorUnitTypes: any;
    public regions: any;
   
    //public inputValidated: any = true;
    //grid
    public matchupResult: any;
    //public skip: any;
    //public pageSize: any;
    //public sort: any;

    public userGroupTree: any;

    //public testListItems: Array<string> = ["Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball"];

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SplitSystemConfiguratorSvc: SplitSystemConfiguratorService
    ) {

        this.user = this.route.snapshot.data['currentUser'].model;

    }

    ngOnChanges() {
        console.log("split system config: OnChange");
    }

    ngOnInit() {
        console.log("split system config: OnInit");

        this.userSvc.isAuthenticated().then((resp: any) => {
            if (resp.isok && resp.model == true) {
                this.isAuthenticated = true;
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




    }

    //ngDoCheck() {

    //}

    ngAfterContentInit() {
        //console.log("system config: AfterContentInit");

        //setTimeout(function () {
        //    $('#userBasket').insertBefore('#projectTabs');
        //}, 1000);




    }

    ngAfterViewChecked() {
        console.log("split system config: AfterViewChecked");


    }


    public getBasketCallback(resp: any) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");

        }
    }

    ngOnDestroy() {
        //$('#content > #userBasket').remove();
        ////reset session["BasketQuoteId"] = 0
        //this.productSvc.resetBasketQuoteId();
        //console.log("system config: OnDestroy");

    }

    public setupDefaults() {
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
    }

    public reset() {
        this.setupDefaults();
        //$("#searchBySystemNeeds").addClass('active-tab');
        //$("#searchByModelNumber").removeClass('active-tab');

    }

   

    public validateInput() {
        var isValidated = true;

        if (this.tonnage == null || this.tonnage == "null") {
            this.toastrSvc.ErrorFadeOut("Tonnage is required.")
            isValidated = false;
        }

        if (this.minSEER == null) {
            this.toastrSvc.ErrorFadeOut("Min SEER is required.")
            isValidated = false;
        }

        if (this.coil == null && this.airHandler == null) {
            this.toastrSvc.ErrorFadeOut("Indoor Unit Type is required.")
            isValidated = false;
        }
       
       
        return isValidated;
    }

    public getResult() {
        var self = this;

        if (this.validateInput()) {

            var params = this.mapInputToParams();

            self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));

            this.SplitSystemConfiguratorSvc.getSystemMatchupList(params).then((resp: any) => {
                if (!resp.error) {
                    var result = resp.result;
                    
                    ////this.concatResult(resp.result);
                    this.matchupResult = result;
                    $('#systemConfigForm').hide();
                    $('#splitMatchupResultGrid').show();

                    self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
                } else {
                    self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
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
        //this.SystemConfiguratorSvc.getTonnageList().then((resp: any) => {
        //    if (resp) {
        //        var tonnageList = resp;
        //        debugger
        //    }
        //});
    }

    public getEqModelList() {

        //Test api
        //this.SystemConfiguratorSvc.getEqModelList({}).then((resp: any) => {
        //    if (!resp.error) {
        //        var list = resp.result.modelList;
        //        debugger
        //    }
        //});
    }


    public setupDropDownLists() {

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



    }

    

    

    public setupTonnageDDL() {
        var self = this;

        this.SplitSystemConfiguratorSvc.getTonnageList().then((resp: any) => {
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
        //this.model = "N";
        //this.ceeTier = "b4";
        this.txv.value = "T";
        this.status.value = "S";
    }

    

    public resetIndoorUnit() {
        //this function get called before value is bound to model
        this.coil = null;
        //this.furnace = null;
        //this.minAFUE = { text: "Select ...", value: null };
        //this.maxAFUE = { text: "Select ...", value: null };
        //this.flushfit = null;
        this.airHandler = null;
        this.indoorUnitType = null;
    }


    public indoorUnitTypeOnChange() {

        
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
            this.airHandler = 'airhandler'
        }
        



    }



};