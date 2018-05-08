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
//This component is not used.Delete after 01/ 06 / 2017
var core_1 = require("@angular/core");
var toastr_service_1 = require("../../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../../shared/services/loadingIcon.service");
var user_service_1 = require("../../../shared/services/user.service");
var systemAccessEnum_1 = require("../../../shared/services/systemAccessEnum");
var product_service_1 = require("../../services/product.service");
var basket_service_1 = require("../../../basket/services/basket.service");
var TechnicalSpecificationsComponent = /** @class */ (function () {
    function TechnicalSpecificationsComponent(elementRef, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.specs = [];
    }
    TechnicalSpecificationsComponent.prototype.ngOnChanges = function () {
    };
    TechnicalSpecificationsComponent.prototype.ngOnInit = function () {
        for (var key in this.specifications.all) {
            this.specs.push(this.specifications.all[key]);
        }
    };
    TechnicalSpecificationsComponent.prototype.ngAfterViewChecked = function () {
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TechnicalSpecificationsComponent.prototype, "specifications", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TechnicalSpecificationsComponent.prototype, "userBasket", void 0);
    TechnicalSpecificationsComponent = __decorate([
        core_1.Component({
            selector: 'technical-specifications',
            templateUrl: 'app/products/productDetails/technicalSpecifications/technical-specifications.component.html',
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], TechnicalSpecificationsComponent);
    return TechnicalSpecificationsComponent;
}());
exports.TechnicalSpecificationsComponent = TechnicalSpecificationsComponent;
;
//# sourceMappingURL=technical-specifications.component.js.map