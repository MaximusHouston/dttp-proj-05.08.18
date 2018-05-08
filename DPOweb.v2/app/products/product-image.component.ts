import {Component, OnInit, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-image',
    templateUrl: 'app/products/product-image.component.html',

})

export class ProductImageComponent implements OnInit {
    @Input() product: any;
    private images: any = [];

    constructor(private toastrSvc: ToastrService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService) {
        
    }

    ngOnInit() {
        //if (this.product.image == undefined) {
        //    debugger
        //}
     
        if (this.product.image.hasImage) {
            this.images.push(this.product.image);
        }
        if (this.product.subProducts.length > 0) {

            for (var i = 0; i < this.product.subProducts.length; i++) {
                this.images.push(this.product.subProducts[i].image);
            }

        }
        
    }

    ngAfterViewInit() {
        this.setupImageToggleBtn();
    }

    ngAfterViewChecked() {
        //if (this.product.image.hasImage) {
        //    this.images.push(this.product.images);
        //}
        //if (this.product.subProducts.length > 0) {

        //    for (var i = 0; i < this.product.subProducts.length; i++) {
        //        this.images.push(this.product.subProducts[i].image);
        //    }
            
        //}

        //this.setupImageToggleBtn();

        //jQuery(".product-img-btn").click(function (event: any) {

        //    //get productNumberIdx
        //    var targetId = event.target.id; // ex: "img-btn-FDXS09LVJURXS09LVJU-0"
        //    var productNumberIdx = targetId.substring(8, targetId.length)

        //    //find sub-product Imgabe Id by productNumberIdx
        //    var imgBtn = jQuery(event.target);
        //    //imgBtn.addClass("active");
        //    var parentDiv = imgBtn.parents(".sub-product-img-container");

        //    var imgId = "img-" + productNumberIdx;
        //    //var img = jQuery(parentDiv).find(imgId)[0];

        //    var subProductImgaes = jQuery(parentDiv).find(".sub-product-img");

        //    for (var i = 0; i < subProductImgaes.length; i++) {
        //        if (subProductImgaes[i].id == imgId) {
        //            jQuery(subProductImgaes[i]).removeClass("hidden");
        //        } else {
        //            jQuery(subProductImgaes[i]).addClass("hidden");
        //        }
        //    }

        //    var imgBtns = jQuery(parentDiv).find(".product-img-btn");
        //    jQuery(imgBtns).removeClass("active");

        //    imgBtn.addClass("active");

        //});
    }

    public setupImageToggleBtn() {
        jQuery(".product-img-btn").click(function (event: any) {

            //get productNumberIdx
            var targetId = event.target.id; // ex: "img-btn-FDXS09LVJURXS09LVJU-0"
            var productNumberIdx = targetId.substring(8, targetId.length)

            //find sub-product Imgabe Id by productNumberIdx
            var imgBtn = jQuery(event.target);
            //imgBtn.addClass("active");
            var parentDiv = imgBtn.parents(".sub-product-img-container");

            var imgId = "img-" + productNumberIdx;
            //var img = jQuery(parentDiv).find(imgId)[0];

            var subProductImgaes = jQuery(parentDiv).find(".sub-product-img");

            for (var i = 0; i < subProductImgaes.length; i++) {
                if (subProductImgaes[i].id == imgId) {
                    jQuery(subProductImgaes[i]).removeClass("hidden");
                } else {
                    jQuery(subProductImgaes[i]).addClass("hidden");
                }
            }

            var imgBtns = jQuery(parentDiv).find(".product-img-btn");
            jQuery(imgBtns).removeClass("active");

            imgBtn.addClass("active");

        });
    }
};