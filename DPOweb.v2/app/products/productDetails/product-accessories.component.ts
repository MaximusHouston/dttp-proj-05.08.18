import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-accessories',
    templateUrl: 'app/products/productDetails/product-accessories.component.html',
})

export class ProductAccessoriesComponent implements OnInit {
    @Input() product: any;
    @Input() userBasket: any;
    @Input() currentUser: any;
    public showPrices: boolean = false;
    public quoteId: any;
  
    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
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

    ngDoCheck() {

    }

    ngAfterContentInit() {



    }

    ngAfterViewChecked() {
       

    }

    ngOnDestroy() {

    }

    public accessoryDetails(event: any, accessory: any) {
        //this.showProductGrid = false;
        //this.product = product;
        this.productSvc.product = accessory;

        this.router.navigate(['/products', { outlets: { 'productDetails': [accessory.productId] } }], { queryParams: { activeTab: 'product-overview' } });
    }

    

};