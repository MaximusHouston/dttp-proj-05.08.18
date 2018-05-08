"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var toastr_service_1 = require("../shared/services/toastr.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var product_service_1 = require("./services/product.service");
var ProductImageComponent = /** @class */ (function () {
    function ProductImageComponent(toastrSvc, userSvc, systemAccessEnum, productSvc) {
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.images = [];
    }
    ProductImageComponent.prototype.ngOnInit = function () {
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
    };
    ProductImageComponent.prototype.ngAfterViewInit = function () {
        this.setupImageToggleBtn();
    };
    ProductImageComponent.prototype.ngAfterViewChecked = function () {
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
    };
    ProductImageComponent.prototype.setupImageToggleBtn = function () {
        jQuery(".product-img-btn").click(function (event) {
            //get productNumberIdx
            var targetId = event.target.id; // ex: "img-btn-FDXS09LVJURXS09LVJU-0"
            var productNumberIdx = targetId.substring(8, targetId.length);
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
                }
                else {
                    jQuery(subProductImgaes[i]).addClass("hidden");
                }
            }
            var imgBtns = jQuery(parentDiv).find(".product-img-btn");
            jQuery(imgBtns).removeClass("active");
            imgBtn.addClass("active");
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductImageComponent.prototype, "product", void 0);
    ProductImageComponent = __decorate([
        core_1.Component({
            selector: 'product-image',
            templateUrl: 'app/products/product-image.component.html',
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService])
    ], ProductImageComponent);
    return ProductImageComponent;
}());
exports.ProductImageComponent = ProductImageComponent;
;
//# sourceMappingURL=product-image.component.js.map