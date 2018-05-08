import { Component, OnInit, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-details-listView',
    templateUrl: 'app/products/product-details-listView.component.html',

})

export class ProductDetailsListViewComponent implements OnInit {
    @Input() user: any;
    @Input() product: any;
    //@Output() changeQty: EventEmitter<any> = new EventEmitter();
    @Output() viewProductDetailsEvent: EventEmitter<any> = new EventEmitter();
    @Input() basketQuoteId: any;


    constructor(private router: Router, private elementRef: ElementRef, private toastrSvc: ToastrService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private enums: Enums,
        private productSvc: ProductService) {

    }

  
    ngOnChanges() {
        
    }
    ngOnInit() {
        //this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
        
    }

    

    

    public productDetails(event: any, product: any, activeTab: any) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        }
        this.viewProductDetailsEvent.emit(eventParams);
    }

   
};