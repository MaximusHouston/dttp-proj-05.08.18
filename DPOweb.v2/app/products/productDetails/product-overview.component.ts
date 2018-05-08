import { Component, OnInit, OnChanges, SimpleChanges,Input, ElementRef} from '@angular/core';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-overview',
    templateUrl: 'app/products/productDetails/product-overview.component.html',
})

export class ProductOverviewComponent implements OnInit {
    @Input() product: any;
    @Input() userBasket: any;
    @Input() currentUser: any;

    constructor(private elementRef: ElementRef, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService, private basketSvc: BasketService) {

    }

    ngOnChanges(changes: SimpleChanges) {
        console.log("ProductDetail-OverView Component: ngOnChange");
    }

    ngOnInit() {
        
    }

    ngDoCheck() {
        //this.product = this.productSvc.product;

        //load new product
        //if (this.product != undefined && this.product.productId != this.productSvc.product.productId) {
        //    this.product = this.productSvc.product;
        //}

        


    }

    ngAfterViewChecked() {
        
    }
};