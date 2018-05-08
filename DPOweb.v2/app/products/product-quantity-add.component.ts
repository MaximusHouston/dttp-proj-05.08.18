import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';

import { ProductService } from './services/product.service';
import { BasketService } from '../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-quantity-add',
    templateUrl: 'app/products/product-quantity-add.component.html',

})

export class ProductQuantityAddComponent implements OnInit {

    @Input() product: any;


    constructor(private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private productSvc: ProductService, private basketSvc: BasketService) {

    }


    ngOnChange(changes: SimpleChanges) {
        //console.log("Product Quantiy Input: ngOnChange");
    }

    ngOnInit() {

    }

    ngAfterViewChecked() {

    }

    public addProductToQuote() {
        var self = this;

        if (this.product.quantity > 0) {
            var data = {
                "ProductId": this.product.productId,
                "Quantity": this.product.quantity
            }

            self.loadingIconSvc.Start(jQuery("#productPageContainer"));

            this.productSvc.addProductToQuote(data).then((resp: any) => {
                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));
                
                this.product.quantity = 0;

                self.basketSvc.getBasket().then((resp: any) => {
                    if (resp.isok) {
                        //self.userBasket = resp.model;
                        self.basketSvc.userBasket = resp.model;
                        $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                    }
                });

                self.toastrSvc.displayResponseMessages(resp);

            });
        }


    }
   




};