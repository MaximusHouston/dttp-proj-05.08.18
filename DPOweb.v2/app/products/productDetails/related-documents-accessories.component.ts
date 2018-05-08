import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';
import { Enums } from '../../shared/enums/enums';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'related-documents-accessories',
    templateUrl: 'app/products/productDetails/related-documents-accessories.component.html',
})

export class RelatedDocsAndAssrComponent implements OnInit {
    @Input() product: any;
    //@Input() userBasket: any;
    public relatedIndoorUnit: any;
    public relatedOutdoorUnit: any;

    constructor(private router: Router, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private enums: Enums,
                private productSvc: ProductService, private basketSvc: BasketService
                ) 
    {

    }

    ngOnChanges() {
        
    }

    ngOnInit() {
        
        if (this.product.isSystem) {
            for (var i in this.product.subProducts) {
                var item = this.product.subProducts[i];
                if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) { // indoor
                    this.relatedIndoorUnit = item;
                } else if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor) { // outdoor
                    this.relatedOutdoorUnit = item;
                }
            }
        }

        if (this.product.productTypeId == this.enums.ProductTypeEnum.Accessory) { // accessories
            $('#product-accessories a').text("PARENT PRODUCTS");
        } else {
            $('#product-accessories a').text("RELATED ACCESSORIES");
        }


      

    }

    ngDoCheck() {
      
    }

    ngAfterContentInit() {
        

    }

    ngAfterViewInit() {
        $('#viewAllAccessoriesBtn').click(function () {
            $('#productOverviewTab').hide();
            $('#productRelatedAccessoriesTab').show();
            $('#productSpecsTab').hide();

            $('#productDetailsTabs li').each(function () {
                $(this).removeClass('active');
            });

            $('#product-accessories').addClass('active');
        });
    }

    ngAfterViewChecked() {


    }

    ngOnDestroy() {
   
    }


    public productDetails(event: any, product: any) {
        this.productSvc.product = product;

        this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }]);
    }

};