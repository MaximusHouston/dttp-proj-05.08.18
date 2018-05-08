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
var core_1 = require('@angular/core');
var toastr_service_1 = require('../shared/services/toastr.service');
var user_service_1 = require('../shared/services/user.service');
var systemAccessEnum_1 = require('../shared/services/systemAccessEnum');
var product_service_1 = require('./services/product.service');
var ProductDetailsGridViewComponent = (function () {
    function ProductDetailsGridViewComponent(elementRef, toastrSvc, userSvc, systemAccessEnum, productSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.changeQty = new core_1.EventEmitter();
    }
    ProductDetailsGridViewComponent.prototype.ngOnInit = function () {
    };
    ProductDetailsGridViewComponent.prototype.ngAfterViewChecked = function () {
        var self = this;
        var element = this.elementRef.nativeElement;
        var seerRatingBar = jQuery(element).find(".seerRatingBar");
        if (seerRatingBar[0] != undefined && this.product.specifications.all.seerNonducted) {
            jQuery(seerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 40,
                value: this.product.specifications.all.seerNonducted.value
            });
        }
        var ieerRatingBar = jQuery(element).find(".ieerRatingBar");
        if (ieerRatingBar[0] != undefined && this.product.specifications.all.ieerNonducted) {
            jQuery(ieerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 40,
                value: this.product.specifications.all.ieerNonducted.value
            });
        }
        var eerRatingBar = jQuery(element).find(".eerRatingBar");
        if (eerRatingBar[0] != undefined && this.product.specifications.all.eerNonducted) {
            jQuery(eerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.eerNonducted.value
            });
        }
        var hspfRatingBar = jQuery(element).find(".hspfRatingBar");
        if (hspfRatingBar[0] != undefined && this.product.specifications.all.hspfNonducted) {
            jQuery(hspfRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.hspfNonducted.value
            });
        }
        var cop47RatingBar = jQuery(element).find(".cop47RatingBar");
        if (cop47RatingBar[0] != undefined && this.product.specifications.all.coP47Nonducted) {
            jQuery(cop47RatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 10,
                value: this.product.specifications.all.coP47Nonducted.value
            });
        }
        //numeric text box
        var numericTextBox = jQuery(element).find(".numericTextBox");
        if (numericTextBox[0] != undefined) {
            jQuery(numericTextBox[0]).change(function () {
                self.product.quantity = this.valueAsNumber;
                //self.changeQty.emit(self.product);               
                //self.changeQty.emit("qty changed");               
            });
        }
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Object)
    ], ProductDetailsGridViewComponent.prototype, "product", void 0);
    __decorate([
        core_1.Output(), 
        __metadata('design:type', core_1.EventEmitter)
    ], ProductDetailsGridViewComponent.prototype, "changeQty", void 0);
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Object)
    ], ProductDetailsGridViewComponent.prototype, "basketQuoteId", void 0);
    ProductDetailsGridViewComponent = __decorate([
        core_1.Component({
            selector: 'product-details-gridView',
            templateUrl: 'app/products/product-details-gridView.component.html',
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef, toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService])
    ], ProductDetailsGridViewComponent);
    return ProductDetailsGridViewComponent;
}());
exports.ProductDetailsGridViewComponent = ProductDetailsGridViewComponent;
;
//# sourceMappingURL=product-details-gridView.js.map