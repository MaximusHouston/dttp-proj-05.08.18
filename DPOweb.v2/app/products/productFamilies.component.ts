import { Component, OnInit } from '@angular/core';
import { IProductFamily } from './productFamily';
import { ProductFamilyService } from './services/productFamily.service';

import { Http } from '@angular/http';
import { Observable, BehaviorSubject } from 'rxjs/Rx';

import {
    GridComponent,
    GridDataResult,
    DataStateChangeEvent
} from '@progress/kendo-angular-grid';

@Component({
    selector: 'dk-productFamilies',
    templateUrl: 'app/products/productFamilyGrid.component.html',
    styleUrls: ['app/products/productList.component.css'],
    providers: [ProductFamilyService]
})

export class ProductFamilyComponent implements OnInit {
    pageTitle: string = 'Product Families';
    imageWidth: number = 50;
    imageMargin: number = 2;
    showImage: boolean = true;
    listFilter: string = 'All';
    productFamilies: IProductFamily[];
    errorMessage: string;

    constructor(private _productFamilyService: ProductFamilyService) { }

    ngOnInit() {
        this.getProductFamilies();
    }

    getProductFamilies(): void {
        this._productFamilyService.getProductFamilies()
            .subscribe(data => this.productFamilies = data,
            error => this.errorMessage = <any>error);
    }
}
