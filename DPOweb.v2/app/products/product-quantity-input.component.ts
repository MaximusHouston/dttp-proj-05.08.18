import { Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-quantity-input',
    templateUrl: 'app/products/product-quantity-input.component.html',

})

export class ProductQuantityInputComponent implements OnInit {
    
    @Input() product: any;
    //@Input() quantity: any;
    
    constructor(private toastrSvc: ToastrService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService) {

    }


    ngOnChange(changes: SimpleChanges) {
        //console.log("Product Quantiy Input: ngOnChange");
    }

    ngOnInit() {

    }
    
    ngAfterViewChecked() {
        
    }

   
    validateQuantity(event: any) {
        
               
        var value = parseFloat(event.target.value);
        
        if (value == null || isNaN(value)) {
            this.product.quantity = 0;
            event.target.value = 0;
        } else if ((value < 0) || (Math.floor(value) != value)) {
            this.product.quantity = 0;
            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    }




};