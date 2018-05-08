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
//import { AppRoutingModule }  from '../app.routes';
//import { ROUTER_DIRECTIVES } from '@angular/router';
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var product_service_1 = require("./services/product.service");
var basket_service_1 = require("../basket/services/basket.service");
var ProductsComponent = /** @class */ (function () {
    function ProductsComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, productSvc, basketSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.productSearchTextHolder = "All";
        this.showProductGrid = true;
        this.showProductHomeContent = true;
        this.product = null;
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    ProductsComponent.prototype.ngOnChanges = function () {
        //console.log("products: ngOnChanges");
    };
    ProductsComponent.prototype.ngOnInit = function () {
        //console.log("products: ngOnInit");
        var _this = this;
        //if (!this.userSvc.userIsAuthenticated) {
        //    window.location.href = "/Account/Login";
        //}
        //if (this.route.url.value[0].path == 'products') {
        //    this.route.params.subscribe((params: { id: string }) => {
        //        debugger;
        //        var id = params.id;
        //    });
        //}
        var hash = window.location.hash;
        if (hash.includes("#/products/(productDetails:")) {
            this.showProductHomeContent = false;
            //$("#addProductsToQuoteBtn").hide();
        }
        else {
        }
        //if (this.productSvc.productFamilyTabs == undefined) {
        //    this.productSvc.getProductFamilies().then(this.getProductFamiliesCallback.bind(this));
        //} else {
        //    this.productFamilyTabs = this.productSvc.productFamilyTabs;
        //}
        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(function (resp) {
                if (resp.isok) {
                    _this.userSvc.currentUser = resp.model;
                    _this.productFamilyTabs = resp.model.productFamilyAccesses;
                }
            });
        }
        else {
            this.productFamilyTabs = this.userSvc.currentUser.productFamilyAccesses;
        }
        //$("#numeric").kendoNumericTextBox({
        //    change: function () {
        //        var value = this.value();
        //        debugger
        //        alert(value);
        //    }
        //});
        this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
    };
    ProductsComponent.prototype.ngDoCheck = function () {
        //console.log("products: ngDoCheck");
        var hash = window.location.hash;
        if (hash == "#/products" && this.productFamilyId != null) {
            this.showProductGrid = true;
        }
        else if (hash.includes("#/products/(productDetails:")) {
            this.showProductGrid = false;
        }
    };
    ProductsComponent.prototype.ngAfterContentInit = function () {
        //if (!this.userSvc.userIsAuthenticated) {
        //    window.location.href = "/Account/Login";
        //}
        this.userSvc.isAuthenticated().then(function (resp) {
            if (!resp.isok || resp.model != true) {
                //Go back to login page
                window.location.href = "/v2/#/account/login";
            }
        });
        console.log("products: ngAfterContentInit");
        $('#userBasket').insertBefore('#main-container');
        $('#productTabs').insertBefore('#main-container');
        //this.productFamilyTabs = this.userSvc.currentUser.productFamilyAccesses;
        //working ok, but slow
        //if (this.productSvc.productFamilyTabs == undefined || this.productSvc.productFamilyTabs.length == 0) {
        //    this.productSvc.getProductFamilies().then(this.getProductFamiliesCallback.bind(this));
        //} else {
        //    this.productFamilyTabs = this.productSvc.productFamilyTabs;
        //}
    };
    ProductsComponent.prototype.ngAfterContentChecked = function () {
        //console.log("products: ngAfterContentChecked");
    };
    ProductsComponent.prototype.ngAfterViewInit = function () {
        //console.log("products: ngAfterViewInit");
    };
    ProductsComponent.prototype.ngAfterViewChecked = function () {
        //console.log("products: ngAfterViewChecked");
        //this.setupActiveTab();
        this.setupSearchProduct();
    };
    ProductsComponent.prototype.ngOnDestroy = function () {
        //console.log("products: ngOnDestroy");
        $('#content > #userBasket').remove();
        $('#content > #productTabs').remove();
    };
    ProductsComponent.prototype.getBasketCallback = function (resp) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;
            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
        }
    };
    ProductsComponent.prototype.updateUserBasket = function () {
        this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
        //TODO: remove GetProducts, clear quantities on gridViewData instead
        //this.GetProducts(this.productFamilyId, this.productModelTypeId);
        //kendo.ui.progress(productGrid, true);
    };
    ProductsComponent.prototype.getProductFamiliesCallback = function (resp) {
        if (resp.isok) {
            this.productFamilyTabs = resp.model.productFamilyTabs;
        }
    };
    //To Do: delete after 4/1/2017
    //public GetProductsByFamilyId(productFamilyId: any) {
    //    this.productFamilyId = productFamilyId;
    //    var data = {
    //        "ProductFamilyId": productFamilyId,
    //    };
    //    this.productSvc.getProducts(data).then(this.GetProductsByFamilyIdCallback.bind(this));
    //    //this.productSvc.getProductsByProductFamilyId(productFamilyId).then(this.GetProductsByFamilyIdCallback.bind(this));
    //}
    //public GetProductsByFamilyIdCallback(resp: any) { 
    //    if (resp.isok) {
    //        //this.productData = resp.model.products;
    //        this.productData = resp.model;
    //        this.productFamilyName = resp.model.productFamilyName;
    //    }
    //}
    ProductsComponent.prototype.GetProducts = function (productFamilyId, productModelTypeId) {
        this.showProductGrid = true;
        this.showProductHomeContent = false;
        this.productFamilyId = productFamilyId;
        this.productModelTypeId = productModelTypeId;
        this.productTypeId = this.enums.ProductTypeEnum.Equipment;
        jQuery("#productSearchBox")[0].value = "";
        //this.productSvc.productFamilyId = productFamilyId;
        //this.productSvc.productModelTypeId = productModelTypeId;
        var data = {
            "ProductFamilyId": this.productFamilyId,
            "ProductModelTypeId": this.productModelTypeId
        };
        //start spinning icon
        //var productGrid = jQuery("#productGrid");
        //if (productGrid != undefined) {
        //    kendo.ui.progress(productGrid, true);
        //}
        //this.loadingIconSvc.Start(jQuery("#productGrid"));
        this.loadingIconSvc.Start(jQuery("#productPageContainer"));
        this.productSvc.getProducts(data).then(this.GetProductsCallback.bind(this));
    };
    //deprecated
    ProductsComponent.prototype.GetProductsByUnitInstallationType = function (productFamilyId, unitInstallationTypeId) {
        this.productFamilyId = productFamilyId;
        this.unitInstallationTypeId = unitInstallationTypeId;
        var data = {
            "ProductFamilyId": this.productFamilyId,
            "UnitInstallationTypeId": this.unitInstallationTypeId
        };
        var productGrid = jQuery("#productGrid");
        if (productGrid != undefined) {
            kendo.ui.progress(productGrid, true);
        }
        this.productSvc.getProducts(data).then(this.GetProductsCallback.bind(this));
    };
    ProductsComponent.prototype.GetProductsByProductClassPIMId = function (productFamilyId, productClassPIMId) {
        this.productFamilyId = productFamilyId;
        this.productClassPIMId = productClassPIMId;
        //this.productSvc.productFamilyId = productFamilyId;
        //this.productSvc.unitInstallationTypeId = unitInstallationTypeId;
        var data = {
            "ProductFamilyId": this.productFamilyId,
            "ProductClassPIMId": this.productClassPIMId
        };
        var productGrid = jQuery("#productGrid");
        if (productGrid != undefined) {
            kendo.ui.progress(productGrid, true);
        }
        this.productSvc.getProducts(data).then(this.GetProductsCallback.bind(this));
    };
    ProductsComponent.prototype.GetProductsCallback = function (resp) {
        if (resp.isok) {
            this.productData = resp.model;
            this.productFamilyName = resp.model.productFamilyName;
            //this.productTypeId = this.enums.ProductTypeEnum.Equipment;
            this.productSearchTextHolder = resp.model.productFamilyName;
            this.unitInstallationTypeTabs = resp.model.unitInstallationTypeTabs;
            this.productClassPIMTabs = resp.model.productClassPIMTabs;
            this.productModelTypeId = this.productData.productModelTypeId;
            this.unitInstallationTypeId = this.productData.unitInstallationTypeId; // deprecated
            this.productClassPIMId = this.productData.productClassPIMId;
            setTimeout(this.setupActiveTab.bind(this), 200); // wait 0.2 sec
        }
        //this.loadingIconSvc.Stop(jQuery("#productGrid"));
        this.loadingIconSvc.Stop(jQuery("#productPageContainer"));
        //this.setupActiveTab();
    };
    ProductsComponent.prototype.showProductHome = function () {
        this.showProductHomeContent = true;
        this.productFamilyId = null;
        this.setupActiveTab();
        //this.productSvc.productFamilyId = null;
        this.productFamilyName = null;
        this.productSearchTextHolder = "All";
        jQuery("#productSearchBox")[0].value = "";
        this.productData = null;
    };
    ProductsComponent.prototype.setupActiveTab = function () {
        //Product family tabs
        //$('.productFamilyTab li').click(function () {
        //    $('.productFamilyTab li').each(function () {
        //        $(this).removeClass('active');
        //    });
        //    $(this).addClass('active');
        //})
        //Product family tabs
        $('.productFamilyTab li').each(function () {
            $(this).removeClass('active');
        });
        if (this.productFamilyId != null) {
            if (this.productTypeId == this.enums.ProductTypeEnum.Accessory) {
                var activeFamilyTabId = "#product-family-tab-accessories";
            }
            else {
                var activeFamilyTabId = "#product-family-tab-" + this.productFamilyId;
            }
        }
        else {
            var activeFamilyTabId = "#product-family-tab-home";
        }
        $(activeFamilyTabId).addClass("active");
        //Sub tabs
        $('.sub-tab-bar li').each(function () {
            $(this).removeClass('active-tab');
        });
        if (this.productModelTypeId != null) {
            var activeSubTabId = "#subTab-" + this.productModelTypeId;
        }
        else if (this.productClassPIMId != null) {
            var activeSubTabId = "#subTab-" + this.productClassPIMId;
        }
        $(activeSubTabId).addClass("active-tab");
    };
    ProductsComponent.prototype.setupSearchProduct = function () {
        var self = this;
        jQuery("#productSearchBox").keyup(function (event) {
            event.stopImmediatePropagation();
            if (event.keyCode == 13) {
                jQuery("#productSearchBtn").click();
            }
        });
        jQuery("#productSearchBtn").click(function (event) {
            event.stopImmediatePropagation();
            self.showProductHomeContent = false;
            self.showProductGrid = true;
            window.location.href = "/v2/#/products";
            var value = jQuery("#productSearchBox")[0].value;
            if (value) {
                var data = {
                    "ProductFamilyId": self.productFamilyId ? self.productFamilyId : null,
                    "IsSearch": true,
                    "Filter": value
                };
                var productGrid = jQuery("#productGrid");
                if (productGrid != undefined) {
                    kendo.ui.progress(productGrid, true);
                }
                self.productSvc.getProducts(data).then(self.GetProductsCallback.bind(self));
            }
        });
    };
    ProductsComponent.prototype.productDetails = function (eventParams) {
        this.showProductGrid = false;
        this.product = eventParams.product; // may not needed
        this.productSvc.product = eventParams.product;
        //this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }]);
        this.router.navigate(['/products', { outlets: { 'productDetails': [eventParams.product.productId] } }], { queryParams: { activeTab: eventParams.activeTab } });
    };
    ProductsComponent = __decorate([
        core_1.Component({
            selector: 'products',
            templateUrl: 'app/products/products.component.html',
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums,
            product_service_1.ProductService, basket_service_1.BasketService])
    ], ProductsComponent);
    return ProductsComponent;
}());
exports.ProductsComponent = ProductsComponent;
;
//# sourceMappingURL=products.component.js.map