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
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var product_service_1 = require("./services/product.service");
var basket_service_1 = require("../basket/services/basket.service");
var ProductQuantityAddComponent = /** @class */ (function () {
    function ProductQuantityAddComponent(toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc) {
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
    }
    ProductQuantityAddComponent.prototype.ngOnChange = function (changes) {
        //console.log("Product Quantiy Input: ngOnChange");
    };
    ProductQuantityAddComponent.prototype.ngOnInit = function () {
    };
    ProductQuantityAddComponent.prototype.ngAfterViewChecked = function () {
    };
    ProductQuantityAddComponent.prototype.addProductToQuote = function () {
        var _this = this;
        var self = this;
        if (this.product.quantity > 0) {
            var data = {
                "ProductId": this.product.productId,
                "Quantity": this.product.quantity
            };
            self.loadingIconSvc.Start(jQuery("#productPageContainer"));
            this.productSvc.addProductToQuote(data).then(function (resp) {
                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));
                _this.product.quantity = 0;
                self.basketSvc.getBasket().then(function (resp) {
                    if (resp.isok) {
                        //self.userBasket = resp.model;
                        self.basketSvc.userBasket = resp.model;
                        $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                    }
                });
                self.toastrSvc.displayResponseMessages(resp);
            });
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductQuantityAddComponent.prototype, "product", void 0);
    ProductQuantityAddComponent = __decorate([
        core_1.Component({
            selector: 'product-quantity-add',
            templateUrl: 'app/products/product-quantity-add.component.html',
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], ProductQuantityAddComponent);
    return ProductQuantityAddComponent;
}());
exports.ProductQuantityAddComponent = ProductQuantityAddComponent;
;
//# sourceMappingURL=product-quantity-add.component.js.map