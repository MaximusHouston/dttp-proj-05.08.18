import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../../products/services/product.service';
import { BasketService } from '../../basket/services/basket.service';
import { SystemConfiguratorService } from './services/systemConfigurator.service';
declare var jQuery: any;



@Component({
    selector: 'furnaceDDL',
    templateUrl: 'app/tools/systemConfigurator/furnaceDDL.component.html'
})

export class FurnaceDDLComponent implements OnInit {

    @Input() rowItem: any;
    @Input() rowIndex: any;
    //@Input() seer: any;
    //@Input() indoorUnitType: any;
    //@Input() outDoorUnitType: any;
    //@Input() userBasket: any;

    @Output() furnaceSelectedEvent: EventEmitter<any> = new EventEmitter();

    //public system: any; // system to be added to quote

        

    public furnaceList: any = [];

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
        var t = this.rowItem;
        var s = this.rowIndex;

        var params = this.mapParams();
       

        this.SystemConfiguratorSvc.getEEPFurnaceList(params).then((resp: any) => {
            if (!resp.error) {
                var furnaces = resp.result.eEFurnaceMatchUpList;
                for (var i in furnaces) {

                    //decode fit values
                    if (furnaces[i].fit != undefined && furnaces[i].fit != null) {
                        if (furnaces[i].fit == 0) {
                            furnaces[i].fit = "Flush";
                        } else if (furnaces[i].fit == 1) {
                            furnaces[i].fit = "1.75";
                        }
                    }

                    var item: any = { text: furnaces[i].furnace_Model, value: furnaces[i].furnace_Model, fit: furnaces[i].fit, afue: furnaces[i].afue };
                    this.furnaceList.push(item);

                }
            } else {
               
            }
        });
    }

    public mapParams() {
        var params = {}
      

        params = {
            "user": "",
            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
            "packageName": "SystemMatchupDaikin",
            "servicesName": "doGetEEPFurnacesList",
            "accountId": "goodman1",
            "params": {
                "aRIRefNumber": this.rowItem.arirefNumber,
                "coil": this.rowItem.coill_Model, //"CAPF4961C6D*",
                "coilWidth": this.rowItem.coil_Width, //"C",
                "coilValue": this.rowItem.coil_Value, //"3",
                "airFlow": this.rowItem.airFlow, //"1000",
                "minAfue": this.rowItem.minAfue == undefined ? "" : this.rowItem.afue,
                "maxAfue": this.rowItem.maxAfue == undefined ? "" : this.rowItem.afue,
                "fit": this.rowItem.fit == "N/A" ? "" : this.rowItem.fit,
                "model": this.rowItem.modelNumber,//"DX16SA0301A*",
                "tonnage": this.rowItem.tonnage,//"2.5",
                "txv": this.rowItem.txv,//"TXV-30",
                "seer": this.rowItem.seer,//"15.0",
                "eer": this.rowItem.eer, //"12.5",
                "cooling": this.rowItem.cooling, //"29400",
                "status": this.rowItem.status
            }
        }
        return params;
    }

 

  

    public PSCFunarceChange(selectedItem: any) {
        
        //var eventParams = {
        //    'selectedItem': selectedItem,
        //    'rowItem': this.rowItem,
        //    'rowIndex': this.rowIndex
        //}

        this.rowItem.furnace_Model = selectedItem.value;
        this.furnaceSelectedEvent.emit(selectedItem);
      

    }

   

 

    

   
}