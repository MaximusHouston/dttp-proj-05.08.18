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
var toastr_service_1 = require("../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../shared/services/loadingIcon.service");
var user_service_1 = require("../../shared/services/user.service");
var systemAccessEnum_1 = require("../../shared/services/systemAccessEnum");
var enums_1 = require("../../shared/enums/enums");
var product_service_1 = require("../services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var ProductDetailsComponent = /** @class */ (function () {
    function ProductDetailsComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, productSvc, basketSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.showPrices = false;
    }
    ProductDetailsComponent.prototype.ngOnChange = function () {
        //console.log("ProductDetail Component: ngOnChange");
    };
    ProductDetailsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.route.params.subscribe(function (params) {
            _this.product = null; // this is to invoke change detection
            var data = {
                "ProductId": params.id,
            };
            _this.productSvc.getProduct(data).then(_this.GetProductCallback.bind(_this));
        });
        if (this.basketSvc.userBasket == undefined) {
            this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
        }
        else {
            this.userBasket = this.basketSvc.userBasket;
        }
        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(function (resp) {
                if (resp.isok) {
                    _this.userSvc.currentUser = resp.model;
                    _this.currentUser = resp.model;
                    _this.showPrices = _this.userSvc.currentUser.showPrices;
                }
            });
        }
        else {
            this.currentUser = this.userSvc.currentUser;
        }
    };
    ProductDetailsComponent.prototype.ngDoCheck = function () {
    };
    ProductDetailsComponent.prototype.ngAfterContentInit = function () {
        //console.log("product Detail: ngAfterContentInit");
        //this.setupActiveTab();
    };
    ProductDetailsComponent.prototype.ngAfterContentChecked = function () {
        //console.log("product Detail: ngAfterContentChecked");
        //this.setupActiveTab();
    };
    ProductDetailsComponent.prototype.ngAfterViewInit = function () {
        //console.log("product Detail: ngAfterViewInit");
        //this.setupActiveTab();
        //this.route.queryParams.subscribe((params: { tab: string }) => {
        //    var subTabId = '#' + params.tab;
        //    $(subTabId).click();
        //});
    };
    ProductDetailsComponent.prototype.ngAfterViewChecked = function () {
        //console.log("product Detail: ngAfterViewChecked");
        //this.setupActiveTab();
        if ($('#accessoryFilters').length) {
            $('input[name="accessory-filter-type"]').on('change', function () {
                $('#indoorAccessories, #outdoorAccessories').hide();
                switch ($(this).val()) {
                    case "indoor":
                        $('#indoorAccessories').show();
                        break;
                    case "outdoor":
                        $('#outdoorAccessories').show();
                        break;
                    default:
                        $('#indoorAccessories, #outdoorAccessories').show();
                }
            });
        }
    };
    ProductDetailsComponent.prototype.setupActiveTab = function () {
        //Product family tabs
        $('.productFamilyTab li').each(function () {
            $(this).removeClass('active');
        });
        if (this.product.productFamilyId != null) {
            var activeFamilyTabId = "#product-family-tab-" + this.product.productFamilyId;
        }
        else {
            var activeFamilyTabId = "#product-family-tab-home";
        }
        $(activeFamilyTabId).addClass("active");
        //Product details tabs
        $('#productDetailsTabs li').click(function () {
            $('#productDetailsTabs li').each(function () {
                $(this).removeClass('active');
            });
            $(this).addClass('active');
        });
        $('#product-overview').click(function () {
            //location.href = "#productOverviewTab";
            $('#productOverviewTab').show();
            $('#productRelatedAccessoriesTab').hide();
            $('#productSpecsTab').hide();
        });
        $('#product-accessories').click(function () {
            //location.href = "#productRelatedAccessoriesTa  
            $('#productOverviewTab').hide();
            $('#productRelatedAccessoriesTab').show();
            $('#productSpecsTab').hide();
        });
        $('#product-specs').click(function () {
            //location.href = "#productSpecsTab";
            $('#productOverviewTab').hide();
            $('#productRelatedAccessoriesTab').hide();
            $('#productSpecsTab').show();
        });
    };
    ProductDetailsComponent.prototype.setActiveTabByUrlParam = function () {
        this.route.queryParams.subscribe(function (params) {
            var activeTabId = '#' + params.activeTab;
            $(activeTabId).click();
        });
    };
    ProductDetailsComponent.prototype.GetProductCallback = function (resp) {
        if (resp.isok) {
            this.product = resp.model;
            //this.productSvc.product = resp.model;
            this.getSubmittalDataSheet(this.product);
            this.setupActiveTab();
        }
        if (this.product.isSystem) {
            for (var i in this.product.subProducts) {
                var item = this.product.subProducts[i];
                if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) {
                    this.relatedIndoorUnit = item;
                }
                else if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor) {
                    this.relatedOutdoorUnit = item;
                }
            }
        }
        console.log("************ SubmittalSheetTypeId: " + this.product.submittalSheetTypeId + " *******************");
    };
    ProductDetailsComponent.prototype.getBasketCallback = function (resp) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
        }
    };
    ProductDetailsComponent.prototype.productDetails = function (event, product) {
        this.productSvc.product = product;
        this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }], { queryParams: { activeTab: 'product-overview' } });
    };
    ProductDetailsComponent.prototype.getSubmittalDataSheet = function (product) {
        var _this = this;
        this.productSvc.getSubmittalDataSheet(product.productNumber).then(function (resp) {
            if (resp) {
                var HtmlString = resp;
                $("#technical-specs").replaceWith(HtmlString);
                _this.setActiveTabByUrlParam();
            }
        });
    };
    ProductDetailsComponent = __decorate([
        core_1.Component({
            selector: 'product-details',
            templateUrl: 'app/products/productDetails/product-details.component.html',
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], ProductDetailsComponent);
    return ProductDetailsComponent;
}());
exports.ProductDetailsComponent = ProductDetailsComponent;
;
//# sourceMappingURL=product-details.component.js.map