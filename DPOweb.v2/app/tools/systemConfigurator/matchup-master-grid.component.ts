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
    selector: 'matchup-master-grid',
    styles: ['/deep/ .k-grid th.k-header, .k-grid-header { background: #5397cf; color: #fff}'],
    templateUrl: 'app/tools/systemConfigurator/matchup-master-grid.component.html'
})

export class MatchupMasterGridComponent implements OnInit {

    @Input() matchupResult: any;
    @Input() indoorUnitType: any;
    @Input() outDoorUnitType: any;
    @Input() userBasket: any;

    public seerCategoriesData: any = [];

    public gridViewData: GridDataResult;
    //public sort: Array<SortDescriptor> = [];
    //public pageSize: number = 15;
    //public skip: number = 0;

    //public testListItems: Array<string> = ["Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball"];

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SystemConfiguratorSvc: SystemConfiguratorService
    ) {

    }

    ngOnChanges() {
        //console.log("On Change");
        
        this.loadData();
    }

    ngOnInit() {
        
        //console.log("On Init");
        //this.loadData();
        
    }

    //public dataStateChange({ skip, take, sort }: DataStateChangeEvent): void {
    //    this.skip = skip;
    //    this.pageSize = take;
    //    this.sort = sort;
    //    this.loadData();
    //}

    public loadData(): void {

        //if (this.seerCategoriesData != undefined) {
        //    this.gridViewData = {
        //        data: this.seerCategoriesData.slice(this.skip, this.skip + this.pageSize),
        //        total: this.seerCategoriesData.length
        //    };
        //}

        this.seerCategoriesData = []; // clear old data

        for (var key in this.matchupResult) {
            if (!this.matchupResult.hasOwnProperty(key)) continue;
            
            if (this.matchupResult[key].length > 0) {
                var obj = {
                    "seer": key,
                    "numberOfItem": this.matchupResult[key].length
                }
                this.seerCategoriesData.push(obj);
            }
            
        }

        if (this.seerCategoriesData != undefined) {
            this.gridViewData = {
                data: this.seerCategoriesData,
                total: this.seerCategoriesData.length
            };
        }
    }

    public backToSearchPage() {
        $('#systemConfigForm').show();
        $('#matchupResultGrid').hide();
    }
}