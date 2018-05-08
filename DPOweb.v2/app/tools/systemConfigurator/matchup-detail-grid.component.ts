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
import { Observable } from 'rxjs/Rx';
import {
    GridComponent,
    GridDataResult,
    DataStateChangeEvent
} from '@progress/kendo-angular-grid';

import { SortDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'matchup-detail-grid',
    //styleUrls: [
    //    // load the default theme (use correct path to node_modules)
    //    'node_modules/@progress/kendo-theme-default/dist/all.css'
    //],
    templateUrl: 'app/tools/systemConfigurator/matchup-detail-grid.component.html'
})

export class MatchupDetailGridComponent implements OnInit {

    @Input() matchupResultDetail: any;
    @Input() seer: any;
    @Input() indoorUnitType: any;
    @Input() outDoorUnitType: any;
    @Input() userBasket: any;

    public system: any; // system to be added to quote

    public gridViewData: GridDataResult;
    public sort: Array<SortDescriptor> = [];
    public pageSize: number = 10;
    public skip: number = 0;

    //public testListItems: any = [
    //    { text: "DM96HS0804CN 3.0", value: "DM96HS0804CN 3.0", fit: 1.75, afue: 96 },
    //    { text: "DC96HS0804CN 3.0", value: "DC96HS0804CN 3.0", fit: 1.75, afue: 96 },
    //    { text: "DC96HS0704CX 3.0", value: "DC96HS0704CX 3.0", fit: 1.75, afue: 96 },
    //    { text: "DC96HS0904CX 3.0", value: "DC96HS0904CX 3.0", fit: 1.75, afue: 96 },
    //    { text: "DK92SS0704CX 3.0", value: "DK92SS0704CX 3.0", fit: 1.75, afue: 90 },
    //    { text: "DK92SS0904CX 3.0", value: "DK92SS0904CX 3.0", fit: 1.75, afue: 90 },
    //    { text: "DM80HS1205DX 3.0", value: "DM80HS1205DX 3.0", fit: "Flush", afue: 80 },
    //    { text: "DM80SS1205DX 3.0", value: "DM80SS1205DX 3.0", fit: "Flush", afue: 80 }
    //];

    public defaultItem: any = { text: "Select item...", value: null, fit: "N/A", afue: "N/A" };

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SystemConfiguratorSvc: SystemConfiguratorService
    ) {

    }

    ngOnChanges() {

    }

    ngOnInit() {
        this.loadData();
    }

    public dataStateChange({ skip, take, sort }: DataStateChangeEvent): void {
        this.skip = skip;
        this.pageSize = take;
        this.sort = sort;

        this.loadData();
    }

    public loadData(): void {
        
        for (var key in this.matchupResultDetail) {
            if (!this.matchupResultDetail[key].furnace_Model) {
                this.matchupResultDetail[key].showFurnaceDDL = true;
            } else {
                this.matchupResultDetail[key].showFurnaceDDL = false;
            }

            //add quantity field
            if (this.matchupResultDetail.hasOwnProperty(key)) {
                this.matchupResultDetail[key].quantity = 0;
            }

        }

        if (this.matchupResultDetail != undefined) {
            this.gridViewData = {
                data: this.matchupResultDetail.slice(this.skip, this.skip + this.pageSize),
                total: this.matchupResultDetail.length
            };
        }
    }

    //deprecated
    //public PSCFunarceChange(selectedItem: any, rowIndex: any, dataItem: any) {
    //    var test = selectedItem;
    //    dataItem.furnace_Model = selectedItem.value;
    //    var fitValueCellId = "#fitValue-" + rowIndex;
    //    $(fitValueCellId).text(selectedItem.fit);

    //    var afueValueCellId = "#afueValue-" + rowIndex;
    //    $(afueValueCellId).text(selectedItem.afue);

    //}

    public FurnaceSelected(selectedItem: any, rowIndex: any) {

        var fitValueCellId = "#fitValue-" + rowIndex;
        $(fitValueCellId).text(selectedItem.fit);

        var afueValueCellId = "#afueValue-" + rowIndex;
        $(afueValueCellId).text(selectedItem.afue);
        
    }

    validateQuantity(event: any) {

        
        var value = parseFloat(event.target.value);

        if (value == null || isNaN(value)) {
            //this.product.quantity = 0;
            event.target.value = 0;
        } else if ((value < 0) || (Math.floor(value) != value)) {
            //this.product.quantity = 0;
            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    }

    public addToQuote(item: any) {
        var self = this;
        var productNumbers = [];
        var outdoorModel = item.outdoor_Model.substring(0, item.outdoor_Model.length - 2);
        productNumbers.push(outdoorModel);
        if (self.outDoorUnitType != "pkg") {
            var coilModel = item.coill_Model.substring(0, item.coill_Model.length - 2);
            productNumbers.push(coilModel);
        }
        


        if (item.quantity > 0) {
            self.system = {
                "ProductNumbers": productNumbers,
                "Quantity": item.quantity,
                "AHRI": item.arirefNumber,
                "SEER": item.seer,
                "EER": item.eer,
                "Cooling": item.cooling,
                "Fit": item.fit,
                "AFUE": item.afue,
                "ContinueAdding": false,
                "ValidProducts":[],
                "InValidProducts":[]
            }

            if (this.indoorUnitType = "coilOnly") {

                if (item.furnace_Model != null && item.furnace_Model != "") {
                    
                    if (item.furnace_Model.includes('*')) {
                        var furnaceModel = item.furnace_Model.substring(0, item.furnace_Model.length - 2);
                        self.system.ProductNumbers.push(furnaceModel)
                    } else {
                        var furnaceModel = item.furnace_Model;
                        self.system.ProductNumbers.push(furnaceModel)
                    }
                    
                }

            } else if (this.indoorUnitType = "furnaceCoil") {

                if (item.furnace_Model != null && item.furnace_Model != "") {
                    if (item.furnace_Model.includes('*')) {
                        var furnaceModel = item.furnace_Model.substring(0, item.furnace_Model.length - 2);
                        self.system.ProductNumbers.push(furnaceModel)
                    } else {
                        var furnaceModel = item.furnace_Model;
                        self.system.ProductNumbers.push(furnaceModel)
                    }
                }

            } else if (this.indoorUnitType = "airHandler") {

                if (item.blower_Model != null && item.blower_Model != "") {
                    var blowerModel = item.blower_Model.substring(0, item.blower_Model.length - 2);
                    self.system.ProductNumbers.push(blowerModel)
                }

            }

            this.addSystemToQuote(self.system);

        } else {
            this.toastrSvc.Info("Please enter quantity!");
        }

        item.quantity = 0;

    }

    public addSystemToQuote(system: any) {
        var self = this;

        self.loadingIconSvc.Start(jQuery("#systemConfiguratorTool"));

        this.productSvc.addSystemToQuote(system).then((resp: any) => {
            self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
            
            if (resp.isok) {//all products added successfully

                //update basket item count
                self.basketSvc.getBasket().then((resp: any) => {
                    if (resp.isok) {
                        self.basketSvc.userBasket = resp.model;
                        $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                    }
                });

                self.toastrSvc.displayResponseMessages(resp); //all products added successfully
            } else {
                
                if (resp.model.validProducts.length > 0) {

                    var validproducts = "";
                    for (var i = 0; i < resp.model.validProducts.length; i++) {
                        if (i < resp.model.validProducts.length - 1) {
                            validproducts += resp.model.validProducts[i] + ", ";
                        } else {
                            validproducts += resp.model.validProducts[i];
                        }
                        
                    }
                    var inValidProducts = "";

                    for (var i = 0; i < resp.model.inValidProducts.length; i++) {
                        if (i < resp.model.inValidProducts.length - 1) {
                            inValidProducts += resp.model.inValidProducts[i] + ", ";
                        } else {
                            inValidProducts += resp.model.inValidProducts[i];
                        }

                    }

                    bootbox.confirm("Can not find: " + inValidProducts + " <br/>Do you want continue adding " + validproducts + " to quote?", function (result) {
                        if (result) {
                            self.system.ContinueAdding = true;
                            //Continue adding valid products
                            self.loadingIconSvc.Start(jQuery("#systemConfiguratorTool"));
                            self.productSvc.addSystemToQuote(self.system).then((resp: any) => {
                                if (resp.isok) {
                                    //update basket item count
                                    self.basketSvc.getBasket().then((resp: any) => {
                                        if (resp.isok) {
                                            //self.userBasket = resp.model;
                                            self.basketSvc.userBasket = resp.model;
                                            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                                        }
                                    });

                                    self.toastrSvc.displayResponseMessages(resp); //products added successfully
                                }

                                self.loadingIconSvc.Stop(jQuery("#systemConfiguratorTool"));
                            });
                        }
                    });
                    //self.toastrSvc.displayResponseMessages(resp);// this shows invalid products

                } else {
                    self.toastrSvc.displayResponseMessages(resp);// All products are invalid
                }
                
            }

           

            //self.toastrSvc.displayResponseMessages(resp);

        });
                
    }

    //public addProductToQuote(productNumber: any, quantity: any) {
    //    var self = this;

    //    var product = {
    //        "ProductNumber": productNumber,
    //        "Quantity": quantity
    //    }

    //    //self.loadingIconSvc.Start(jQuery("#productPageContainer"));

    //    this.productSvc.addProductToQuoteByProductNumber(product).then((resp: any) => {
    //        //self.loadingIconSvc.Stop(jQuery("#productPageContainer"));

    //        quantity = 0;

    //        self.basketSvc.getBasket().then((resp: any) => {
    //            if (resp.isok) {
    //                //self.userBasket = resp.model;
    //                self.basketSvc.userBasket = resp.model;
    //                $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
    //            }
    //        });

    //        self.toastrSvc.displayResponseMessages(resp);

    //    });

    //}





    //public backToSearchPage() {
    //    $('#systemConfigForm').show();
    //    $('#matchupResultGrid').hide();
    //}
}