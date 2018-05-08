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
    selector: 'split-matchup-master-grid',
    styles: ['/deep/ .k-grid th.k-header, .k-grid-header { background: #5397cf; color: #fff}'],
    templateUrl: 'app/tools/splitSystemConfigurator/split-matchup-master-grid.component.html'
})

export class SplitMatchupMasterGridComponent implements OnInit {

    @Input() matchupResult: any;
    @Input() indoorUnitType: any;
    @Input() outDoorUnitType: any;
    @Input() userBasket: any;

    public seerCategoriesData: any = [];

    public gridViewData: GridDataResult;

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService,
        private SplitSystemConfiguratorSvc: SplitSystemConfiguratorService
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
        $('#splitMatchupResultGrid').hide();
    }
}