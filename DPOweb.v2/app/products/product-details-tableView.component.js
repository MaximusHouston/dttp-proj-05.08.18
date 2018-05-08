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
var enums_1 = require("../shared/enums/enums");
var product_service_1 = require("./services/product.service");
var ProductDetailsTableViewComponent = /** @class */ (function () {
    function ProductDetailsTableViewComponent(elementRef, toastrSvc, userSvc, systemAccessEnum, enums, productSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        this.viewProductDetailsEvent = new core_1.EventEmitter();
        this.pageChangeEvt = new core_1.EventEmitter();
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
    ProductDetailsTableViewComponent.prototype.ngOnChanges = function () {
        this.resetColumns();
        this.setupColumns();
    };
    ProductDetailsTableViewComponent.prototype.ngOnInit = function () {
        //this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
        //this.setupColumns();
    };
    ProductDetailsTableViewComponent.prototype.pageChange = function (event) {
        this.pageChangeEvt.emit(event);
    };
    ProductDetailsTableViewComponent.prototype.ngAfterViewChecked = function () {
        //var self = this;
        //var element = this.elementRef.nativeElement;
        //numeric text box
        //var numericTextBox = jQuery(element).find(".numericTextBox");
        //if (numericTextBox[0] != undefined) {
        //    jQuery(numericTextBox[0]).change(function () {
        //        if (this.valueAsNumber < 0) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        } else if (Math.floor(this.valueAsNumber) != this.valueAsNumber) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        }
        //        else {
        //            self.product.quantity = this.valueAsNumber;
        //        }
        //    });
        //}
    };
    ProductDetailsTableViewComponent.prototype.resetColumns = function () {
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
    ProductDetailsTableViewComponent.prototype.setupColumns = function () {
        if (this.productFamilyId == this.enums.ProductFamilyEnum.MiniSplit //Mini-Split
            || (this.productFamilyId == this.enums.ProductFamilyEnum.AlthermaSplit && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //Altherma
            || (this.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //MultiSplit - Outdoor/All
            || this.productFamilyId == this.enums.ProductFamilyEnum.SkyAir) {
            this.SEERNonDucted = true;
            this.EERNonDucted = true;
            this.HSPFNonDucted = true;
            this.COP47NonDucted = true;
        }
        if ((this.productFamilyId == this.enums.ProductFamilyEnum.VRV || this.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit) && this.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) {
            //Show nothing
        }
        if (this.productFamilyId == this.enums.ProductFamilyEnum.VRV && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) {
            this.IEERNonDucted = true;
            this.EERNonDucted = true;
            this.COP47NonDucted = true;
            this.SCHENonDucted = true;
        }
        if (this.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem) {
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.SplitAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.SplitHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler || this.productClassPIMId == this.enums.ProductClassPIMEnum.Coil) {
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.GasFurnace) {
                this.AFUE = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }
        if (this.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem) {
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackageAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedGED) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }
        if (this.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem) {
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedAC) {
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedHP) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler) {
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) {
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                //this.AFUEDucted = true;
            }
        }
    };
    ProductDetailsTableViewComponent.prototype.validateQuantity = function (event) {
        var s = 0;
        //if (this.valueAsNumber < 0) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        } else if (Math.floor(this.valueAsNumber) != this.valueAsNumber) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        }
        //        else {
        //            self.product.quantity = this.valueAsNumber;
        //        }
    };
    ProductDetailsTableViewComponent.prototype.productDetails = function (event, product, activeTab) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        };
        this.viewProductDetailsEvent.emit(eventParams);
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "productFamilyId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "productModelTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "unitInstallationTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "productClassPIMId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "gridViewData", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "skip", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "pageSize", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductDetailsTableViewComponent.prototype, "viewProductDetailsEvent", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductDetailsTableViewComponent.prototype, "pageChangeEvt", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductDetailsTableViewComponent.prototype, "basketQuoteId", void 0);
    ProductDetailsTableViewComponent = __decorate([
        core_1.Component({
            selector: 'product-details-tableView',
            templateUrl: 'app/products/product-details-tableView.component.html',
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums,
            product_service_1.ProductService])
    ], ProductDetailsTableViewComponent);
    return ProductDetailsTableViewComponent;
}());
exports.ProductDetailsTableViewComponent = ProductDetailsTableViewComponent;
;
//# sourceMappingURL=product-details-tableView.component.js.map