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
var ProductOverviewInfoComponent = /** @class */ (function () {
    function ProductOverviewInfoComponent(elementRef, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.showPrices = false;
    }
    ProductOverviewInfoComponent.prototype.ngOnChanges = function () {
        if (this.userBasket != undefined) {
            this.quoteId = this.userBasket.quoteId;
        }
        if (this.currentUser != undefined) {
            this.showPrices = this.currentUser.showPrices;
        }
    };
    ProductOverviewInfoComponent.prototype.ngOnInit = function () {
    };
    ProductOverviewInfoComponent.prototype.ngAfterViewInit = function () {
        console.log("product-overview-info: ngAfterViewInit");
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductOverviewInfoComponent.prototype, "product", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductOverviewInfoComponent.prototype, "userBasket", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductOverviewInfoComponent.prototype, "currentUser", void 0);
    ProductOverviewInfoComponent = __decorate([
        core_1.Component({
            selector: 'product-overview-info',
            templateUrl: 'app/products/productDetails/product-overview-info.component.html',
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], ProductOverviewInfoComponent);
    return ProductOverviewInfoComponent;
}());
exports.ProductOverviewInfoComponent = ProductOverviewInfoComponent;
;
//# sourceMappingURL=product-overview-info.component.js.map