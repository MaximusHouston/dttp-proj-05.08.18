import { Component, OnInit, Input } from '@angular/core';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-details-test',
    templateUrl: 'app/products/productDetails/product-details.component.html',
})

export class ProductDetailsTestComponent implements OnInit {
    private product: any;
    private userBasket: any;


    constructor(private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService, private basketSvc: BasketService) {

    }

    ngOnChanges() {
        //this.product = this.productSvc.product;
    }

    ngOnInit() {
       
    }

    ngDoCheck() {
        this.product = this.productSvc.product;
        this.userBasket = this.basketSvc.userBasket;
    }

    ngAfterContentInit() {
       
        $('#userBasket').insertBefore('#main-container');
        $('#productFamilyTabs').insertBefore('#main-container');


    }

    ngAfterViewChecked() {

     
    }

    ngOnDestroy() {
        $('#content > #userBasket').remove();
        $('#content > #productFamilyTabs').remove();
    }


};