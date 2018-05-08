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
var router_1 = require("@angular/router");
var toastr_service_1 = require("../shared/services/toastr.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var product_service_1 = require("./services/product.service");
var ProductDetailsListViewComponent = /** @class */ (function () {
    function ProductDetailsListViewComponent(router, elementRef, toastrSvc, userSvc, systemAccessEnum, enums, productSvc) {
        this.router = router;
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        //@Output() changeQty: EventEmitter<any> = new EventEmitter();
        this.viewProductDetailsEvent = new core_1.EventEmitter();
    }
    ProductDetailsListViewComponent.prototype.ngOnChanges = function () {
    };
    ProductDetailsListViewComponent.prototype.ngOnInit = function () {
        //this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
    };
    ProductDetailsListViewComponent.prototype.productDetails = function (event, product, activeTab) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        };
        this.viewProductDetailsEvent.emit(eventParams);
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsListViewComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsListViewComponent.prototype, "product", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductDetailsListViewComponent.prototype, "viewProductDetailsEvent", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsListViewComponent.prototype, "basketQuoteId", void 0);
    ProductDetailsListViewComponent = __decorate([
        core_1.Component({
            selector: 'product-details-listView',
            templateUrl: 'app/products/product-details-listView.component.html',
        }),
        __metadata("design:paramtypes", [router_1.Router, core_1.ElementRef, toastr_service_1.ToastrService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, enums_1.Enums,
            product_service_1.ProductService])
    ], ProductDetailsListViewComponent);
    return ProductDetailsListViewComponent;
}());
exports.ProductDetailsListViewComponent = ProductDetailsListViewComponent;
;
//# sourceMappingURL=product-details-listView.component.js.map