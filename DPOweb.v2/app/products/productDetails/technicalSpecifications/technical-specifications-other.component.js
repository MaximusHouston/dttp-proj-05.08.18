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
var toastr_service_1 = require("../../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../../shared/services/loadingIcon.service");
var user_service_1 = require("../../../shared/services/user.service");
var systemAccessEnum_1 = require("../../../shared/services/systemAccessEnum");
var product_service_1 = require("../../services/product.service");
var basket_service_1 = require("../../../basket/services/basket.service");
var TechnicalSpecificationsOtherComponent = /** @class */ (function () {
    function TechnicalSpecificationsOtherComponent(elementRef, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.specs = [];
    }
    TechnicalSpecificationsOtherComponent.prototype.ngOnInit = function () {
        this.specs = this.product.specifications.all;
        //for (var key in this.product.specifications.all) {
        //    var item: any = {
        //        key: key,
        //        value: this.product.specifications.all[key]
        //    }
        //    this.specs.push(item);
        //}
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TechnicalSpecificationsOtherComponent.prototype, "product", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TechnicalSpecificationsOtherComponent.prototype, "userBasket", void 0);
    TechnicalSpecificationsOtherComponent = __decorate([
        core_1.Component({
            selector: 'technical-specifications-other',
            templateUrl: 'app/products/productDetails/technicalSpecifications/technical-specifications-other.component.html',
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], TechnicalSpecificationsOtherComponent);
    return TechnicalSpecificationsOtherComponent;
}());
exports.TechnicalSpecificationsOtherComponent = TechnicalSpecificationsOtherComponent;
;
//# sourceMappingURL=technical-specifications-other.component.js.map