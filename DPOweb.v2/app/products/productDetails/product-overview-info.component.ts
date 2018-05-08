import { Component, OnInit, Input, ElementRef } from '@angular/core';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-overview-info',
    templateUrl: 'app/products/productDetails/product-overview-info.component.html',
})

export class ProductOverviewInfoComponent implements OnInit {
    @Input() product: any;
    @Input() userBasket: any;
    @Input() currentUser: any;
    public showPrices: boolean = false;
    public quoteId: any;


    constructor(private elementRef: ElementRef, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService
    ) {

    }

    ngOnChanges() {
        
        if (this.userBasket != undefined) {
            this.quoteId = this.userBasket.quoteId;
        }

        if (this.currentUser != undefined) {
            this.showPrices = this.currentUser.showPrices;
        }
        
    }

    ngOnInit() {

      
    }

    ngAfterViewInit() {
        console.log("product-overview-info: ngAfterViewInit");
        
    }
   
    

};