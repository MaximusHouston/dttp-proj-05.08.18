//split-matchup-detail-grid.component.ts
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../../products/services/product.service';
import { BasketService } from '../../basket/services/basket.service';
import { SplitSystemConfiguratorService } from './services/splitSystemConfigurator.service';
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
    selector: 'split-matchup-detail-grid',
    templateUrl: 'app/tools/splitSystemConfigurator/split-matchup-detail-grid.component.html'
})

export class SplitMatchupDetailGridComponent implements OnInit {

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



    public defaultItem: any = { text: "Select item...", value: null, fit: "N/A", afue: "N/A" };

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SplitSystemConfiguratorSvc: SplitSystemConfiguratorService
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

   

    //public FurnaceSelected(selectedItem: any, rowIndex: any) {

    //    var fitValueCellId = "#fitValue-" + rowIndex;
    //    $(fitValueCellId).text(selectedItem.fit);

    //    var afueValueCellId = "#afueValue-" + rowIndex;
    //    $(afueValueCellId).text(selectedItem.afue);

    //}

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
        var outdoorModel = item.outdoor_Model.substring(0, item.outdoor_Model.length - 2);
        var coilModel = item.coill_Model.substring(0, item.coill_Model.length - 2);


        if (item.quantity > 0) {
            self.system = {
                "ProductNumbers": [outdoorModel, coilModel],
                "Quantity": item.quantity,
                "AHRI": item.arirefNumber,
                "SEER": item.seer,
                "EER": item.eer,
                "Cooling": item.cooling,
                "Fit": item.fit,
                "AFUE": item.afue,
                "ContinueAdding": false,
                "ValidProducts": [],
                "InValidProducts": []
            }

            if (this.indoorUnitType = "airHandler") {

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

        self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));

        this.productSvc.addSystemToQuote(system).then((resp: any) => {
            self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));

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
                            self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));
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

                                self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
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

 





    
}