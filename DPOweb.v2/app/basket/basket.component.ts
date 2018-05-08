import {Component, OnInit, Input} from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';

import { ProductService } from '../products/services/product.service';
import { BasketService } from './services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'basket',
    templateUrl: 'app/basket/basket.component.html'
})

export class BasketComponent implements OnInit {
    @Input() userBasket: any;
    @Input() productFamilyId: any;
    @Input() productModelTypeId: any;
    @Input() productData: any;
    public basketQuoteId: any;

    constructor(private toastrSvc: ToastrService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService, private basketSvc: BasketService) {

    }

    ngOnInit() {
        
        this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
    }
      

    ngDoCheck() {
       
    }

    ngAfterViewChecked() {
        var hash = window.location.hash;

        if (hash.includes("#/products/(productDetails:")) {
            $("#addProductsToQuoteBtn").hide();
        } else {
            $("#addProductsToQuoteBtn").show();
        }

    }

     public getBasketQuoteIdCallback(resp: any) {
        if (resp.isok) {
            this.basketQuoteId = resp.model;

        }
    }
};