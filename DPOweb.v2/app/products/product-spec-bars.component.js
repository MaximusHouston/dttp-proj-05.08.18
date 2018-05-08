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
var ProductSpecBarsComponent = /** @class */ (function () {
    function ProductSpecBarsComponent(elementRef, toastrSvc, userSvc, systemAccessEnum, enums, productSvc) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        //@Input() userBasket: any;
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
    ProductSpecBarsComponent.prototype.ngOnChanges = function () {
        this.resetColumns();
        this.setupColumns();
    };
    ProductSpecBarsComponent.prototype.ngOnInit = function () {
        setTimeout(this.setupSpecBars.bind(this), 200); // wait 0.2 sec
    };
    ProductSpecBarsComponent.prototype.ngAfterViewChecked = function () {
    };
    ProductSpecBarsComponent.prototype.setupSpecBars = function () {
        var self = this;
        var element = this.elementRef.nativeElement;
        var seerRatingBar = jQuery(element).find(".seerRatingBar");
        if (seerRatingBar[0] != undefined && this.product.specifications.all.seerNonDucted) {
            jQuery(seerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 40,
                value: this.product.specifications.all.seerNonDucted.value
            });
        }
        var scheRatingBar = jQuery(element).find(".scheRatingBar");
        if (scheRatingBar[0] != undefined && this.product.specifications.all.scheNonDucted) {
            jQuery(scheRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 35,
                value: this.product.specifications.all.scheNonDucted.value
            });
        }
        var ieerRatingBar = jQuery(element).find(".ieerRatingBar");
        if (ieerRatingBar[0] != undefined && this.product.specifications.all.ieerNonDucted) {
            jQuery(ieerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 40,
                value: this.product.specifications.all.ieerNonDucted.value
            });
        }
        var eerRatingBar = jQuery(element).find(".eerRatingBar");
        if (eerRatingBar[0] != undefined && this.product.specifications.all.eerNonDucted) {
            jQuery(eerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.eerNonDucted.value
            });
        }
        var hspfRatingBar = jQuery(element).find(".hspfRatingBar");
        if (hspfRatingBar[0] != undefined && this.product.specifications.all.hspfNonDucted) {
            jQuery(hspfRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.hspfNonDucted.value
            });
        }
        var cop47RatingBar = jQuery(element).find(".cop47RatingBar");
        if (cop47RatingBar[0] != undefined && this.product.specifications.all.coP47NonDucted) {
            jQuery(cop47RatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 10,
                value: this.product.specifications.all.coP47NonDucted.value
            });
        }
        var afueRatingBar = jQuery(element).find(".afueRatingBar");
        if (afueRatingBar[0] != undefined && this.product.specifications.all.afue) {
            jQuery(afueRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 100,
                value: this.product.specifications.all.afue.value
            });
        }
        //Unitary
        if (self.product.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialPackagedSystem) {
            var seerRatingBarDucted = jQuery(element).find(".seerRatingBarDucted");
            if (seerRatingBarDucted[0] != undefined && this.product.specifications.all.seerDucted) {
                jQuery(seerRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 40,
                    value: this.product.specifications.all.seerDucted.value
                });
            }
            var ieerRatingBarDucted = jQuery(element).find(".ieerRatingBarDucted");
            if (ieerRatingBarDucted[0] != undefined && this.product.specifications.all.ieerDucted) {
                jQuery(ieerRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 40,
                    value: this.product.specifications.all.ieerDucted.value
                });
            }
            var eerRatingBarDucted = jQuery(element).find(".eerRatingBarDucted");
            if (eerRatingBarDucted[0] != undefined && this.product.specifications.all.eerDucted) {
                jQuery(eerRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 20,
                    value: this.product.specifications.all.eerDucted.value
                });
            }
            var hspfRatingBarDucted = jQuery(element).find(".hspfRatingBarDucted");
            if (hspfRatingBarDucted[0] != undefined && this.product.specifications.all.hspfDucted) {
                jQuery(hspfRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 20,
                    value: this.product.specifications.all.hspfDucted.value
                });
            }
            var cop47RatingBarDucted = jQuery(element).find(".cop47RatingBarDucted");
            if (cop47RatingBarDucted[0] != undefined && this.product.specifications.all.coP47Ducted) {
                jQuery(cop47RatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 10,
                    value: this.product.specifications.all.coP47Ducted.value
                });
            }
        }
    };
    ProductSpecBarsComponent.prototype.resetColumns = function () {
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
    ProductSpecBarsComponent.prototype.setupColumns = function () {
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
    ], ProductSpecBarsComponent.prototype, "product", void 0);
    ProductSpecBarsComponent = __decorate([
        core_1.Component({
            selector: 'product-spec-bars',
            templateUrl: 'app/products/product-spec-bars.component.html',
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums,
            product_service_1.ProductService])
    ], ProductSpecBarsComponent);
    return ProductSpecBarsComponent;
}());
exports.ProductSpecBarsComponent = ProductSpecBarsComponent;
;
//# sourceMappingURL=product-spec-bars.component.js.map