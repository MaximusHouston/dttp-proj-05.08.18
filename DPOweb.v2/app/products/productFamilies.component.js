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
var productFamily_service_1 = require("./services/productFamily.service");
var ProductFamilyComponent = /** @class */ (function () {
    function ProductFamilyComponent(_productFamilyService) {
        this._productFamilyService = _productFamilyService;
        this.pageTitle = 'Product Families';
        this.imageWidth = 50;
        this.imageMargin = 2;
        this.showImage = true;
        this.listFilter = 'All';
    }
    ProductFamilyComponent.prototype.ngOnInit = function () {
        this.getProductFamilies();
    };
    ProductFamilyComponent.prototype.getProductFamilies = function () {
        var _this = this;
        this._productFamilyService.getProductFamilies()
            .subscribe(function (data) { return _this.productFamilies = data; }, function (error) { return _this.errorMessage = error; });
    };
    ProductFamilyComponent = __decorate([
        core_1.Component({
            selector: 'dk-productFamilies',
            templateUrl: 'app/products/productFamilyGrid.component.html',
            styleUrls: ['app/products/productList.component.css'],
            providers: [productFamily_service_1.ProductFamilyService]
        }),
        __metadata("design:paramtypes", [productFamily_service_1.ProductFamilyService])
    ], ProductFamilyComponent);
    return ProductFamilyComponent;
}());
exports.ProductFamilyComponent = ProductFamilyComponent;
//# sourceMappingURL=productFamilies.component.js.map