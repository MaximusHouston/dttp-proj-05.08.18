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
var ProductQuantityInputComponent = /** @class */ (function () {
    //@Input() quantity: any;
    function ProductQuantityInputComponent(toastrSvc, userSvc, systemAccessEnum, productSvc) {
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
    }
    ProductQuantityInputComponent.prototype.ngOnChange = function (changes) {
        //console.log("Product Quantiy Input: ngOnChange");
    };
    ProductQuantityInputComponent.prototype.ngOnInit = function () {
    };
    ProductQuantityInputComponent.prototype.ngAfterViewChecked = function () {
    };
    ProductQuantityInputComponent.prototype.validateQuantity = function (event) {
        var value = parseFloat(event.target.value);
        if (value == null || isNaN(value)) {
            this.product.quantity = 0;
            event.target.value = 0;
        }
        else if ((value < 0) || (Math.floor(value) != value)) {
            this.product.quantity = 0;
            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductQuantityInputComponent.prototype, "product", void 0);
    ProductQuantityInputComponent = __decorate([
        core_1.Component({
            selector: 'product-quantity-input',
            templateUrl: 'app/products/product-quantity-input.component.html',
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService])
    ], ProductQuantityInputComponent);
    return ProductQuantityInputComponent;
}());
exports.ProductQuantityInputComponent = ProductQuantityInputComponent;
;
//# sourceMappingURL=product-quantity-input.component.js.map