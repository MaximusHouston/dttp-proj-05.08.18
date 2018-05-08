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
var product_service_1 = require("../products/services/product.service");
var basket_service_1 = require("./services/basket.service");
var BasketComponent = /** @class */ (function () {
    function BasketComponent(toastrSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
    }
    BasketComponent.prototype.ngOnInit = function () {
        this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
    };
    BasketComponent.prototype.ngDoCheck = function () {
    };
    BasketComponent.prototype.ngAfterViewChecked = function () {
        var hash = window.location.hash;
        if (hash.includes("#/products/(productDetails:")) {
            $("#addProductsToQuoteBtn").hide();
        }
        else {
            $("#addProductsToQuoteBtn").show();
        }
    };
    BasketComponent.prototype.getBasketQuoteIdCallback = function (resp) {
        if (resp.isok) {
            this.basketQuoteId = resp.model;
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], BasketComponent.prototype, "userBasket", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], BasketComponent.prototype, "productFamilyId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], BasketComponent.prototype, "productModelTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], BasketComponent.prototype, "productData", void 0);
    BasketComponent = __decorate([
        core_1.Component({
            selector: 'basket',
            templateUrl: 'app/basket/basket.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService, basket_service_1.BasketService])
    ], BasketComponent);
    return BasketComponent;
}());
exports.BasketComponent = BasketComponent;
;
//# sourceMappingURL=basket.component.js.map