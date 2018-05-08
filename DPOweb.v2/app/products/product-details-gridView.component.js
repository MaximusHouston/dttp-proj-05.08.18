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
var ProductDetailsGridViewComponent = /** @class */ (function () {
    function ProductDetailsGridViewComponent(router, route, elementRef, toastrSvc, userSvc, systemAccessEnum, enums, productSvc) {
        this.router = router;
        this.route = route;
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        //@Output() changeQty: EventEmitter<any> = new EventEmitter();
        this.viewProductDetailsEvent = new core_1.EventEmitter();
        this.SEERNonDucted = false;
        this.EERNonDucted = false;
        this.HSPFNonDucted = false;
        this.COP47NonDucted = false;
        this.IEERNonDucted = false;
        this.SCHENonDucted = false;
        this.SEERDucted = false;
        this.EERDucted = false;
        this.HSPFDucted = false;
        this.COP47Ducted = false;
        this.AFUE = false;
    }
    ProductDetailsGridViewComponent.prototype.ngOnChanges = function () {
        this.resetColumns();
        this.setupColumns();
    };
    ProductDetailsGridViewComponent.prototype.ngOnInit = function () {
    };
    ProductDetailsGridViewComponent.prototype.ngAfterViewChecked = function () {
    };
    //public productDetails(event: any, product: any, activeTab: any){
    //    //this.productSvc.product = product;
    //    //this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }], { queryParams: { tab: activeTab } });
    //    this.viewProductDetailsEvent.emit(product);
    //}
    ProductDetailsGridViewComponent.prototype.productDetails = function (event, product, activeTab) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        };
        this.viewProductDetailsEvent.emit(eventParams);
    };
    ProductDetailsGridViewComponent.prototype.resetColumns = function () {
        this.SEERNonDucted = false;
        this.EERNonDucted = false;
        this.HSPFNonDucted = false;
        this.COP47NonDucted = false;
        this.IEERNonDucted = false;
        this.SCHENonDucted = false;
        this.SEERDucted = false;
        this.EERDucted = false;
        this.HSPFDucted = false;
        this.COP47Ducted = false;
        this.AFUE = false;
    };
    ProductDetailsGridViewComponent.prototype.setupColumns = function () {
        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.MiniSplit //Mini-Split
            || (this.product.productFamilyId == this.enums.ProductFamilyEnum.AlthermaSplit && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //Altherma
            || (this.product.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //MultiSplit - Outdoor/All
            || this.product.productFamilyId == this.enums.ProductFamilyEnum.SkyAir) {
            this.SEERNonDucted = true;
            this.EERNonDucted = true;
            this.HSPFNonDucted = true;
            this.COP47NonDucted = true;
        }
        if ((this.product.productFamilyId == this.enums.ProductFamilyEnum.VRV || this.product.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit) && this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) {
            //Show nothing
        }
        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.VRV && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) {
            this.IEERNonDucted = true;
            this.EERNonDucted = true;
            this.COP47NonDucted = true;
            this.SCHENonDucted = true;
        }
        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem) {
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.SplitAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.SplitHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler || this.product.productClassPIMId == this.enums.ProductClassPIMEnum.Coil) {
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.GasFurnace) {
                this.AFUE = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }
        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem) {
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackageAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedGED) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.AFUE = true;
                if (this.product.productEnergySourceTypeId = this.enums.ProductEnergySourceTypeEnum.DualFuel) {
                    this.HSPFDucted = true;
                    this.COP47Ducted = true;
                }
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }
        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem) {
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler) {
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                //this.AFUEDucted = true;
            }
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsGridViewComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsGridViewComponent.prototype, "product", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductDetailsGridViewComponent.prototype, "viewProductDetailsEvent", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsGridViewComponent.prototype, "basketQuoteId", void 0);
    ProductDetailsGridViewComponent = __decorate([
        core_1.Component({
            selector: 'product-details-gridView',
            templateUrl: 'app/products/product-details-gridView.component.html',
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, core_1.ElementRef, toastr_service_1.ToastrService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums,
            product_service_1.ProductService])
    ], ProductDetailsGridViewComponent);
    return ProductDetailsGridViewComponent;
}());
exports.ProductDetailsGridViewComponent = ProductDetailsGridViewComponent;
;
//# sourceMappingURL=product-details-gridView.component.js.map