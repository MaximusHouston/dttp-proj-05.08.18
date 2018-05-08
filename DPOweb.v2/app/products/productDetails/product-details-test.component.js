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
var toastr_service_1 = require("../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../shared/services/loadingIcon.service");
var user_service_1 = require("../../shared/services/user.service");
var systemAccessEnum_1 = require("../../shared/services/systemAccessEnum");
var product_service_1 = require("../services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var ProductDetailsTestComponent = /** @class */ (function () {
    function ProductDetailsTestComponent(toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
    }
    ProductDetailsTestComponent.prototype.ngOnChanges = function () {
        //this.product = this.productSvc.product;
    };
    ProductDetailsTestComponent.prototype.ngOnInit = function () {
    };
    ProductDetailsTestComponent.prototype.ngDoCheck = function () {
        this.product = this.productSvc.product;
        this.userBasket = this.basketSvc.userBasket;
    };
    ProductDetailsTestComponent.prototype.ngAfterContentInit = function () {
        $('#userBasket').insertBefore('#main-container');
        $('#productFamilyTabs').insertBefore('#main-container');
    };
    ProductDetailsTestComponent.prototype.ngAfterViewChecked = function () {
    };
    ProductDetailsTestComponent.prototype.ngOnDestroy = function () {
        $('#content > #userBasket').remove();
        $('#content > #productFamilyTabs').remove();
    };
    ProductDetailsTestComponent = __decorate([
        core_1.Component({
            selector: 'product-details-test',
            templateUrl: 'app/products/productDetails/product-details.component.html',
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService, basket_service_1.BasketService])
    ], ProductDetailsTestComponent);
    return ProductDetailsTestComponent;
}());
exports.ProductDetailsTestComponent = ProductDetailsTestComponent;
;
//# sourceMappingURL=product-details-test.component.js.map