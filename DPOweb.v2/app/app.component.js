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
//import { ROUTER_DIRECTIVES } from '@angular/router';
var toastr_service_1 = require("./shared/services/toastr.service");
var loadingIcon_service_1 = require("./shared/services/loadingIcon.service");
var user_service_1 = require("./shared/services/user.service");
var systemAccessEnum_1 = require("./shared/services/systemAccessEnum");
var password_service_1 = require("./shared/services/password.service");
//import { ProductFamilyEnum } from './shared/enums/productFamilyEnum';
var enums_1 = require("./shared/enums/enums");
var account_service_1 = require("./account/services/account.service");
var business_service_1 = require("./business/services/business.service");
var user_resolver_service_1 = require("./account/services/user-resolver.service");
var project_service_1 = require("./projects/services/project.service");
//import { ProductService } from './products/services/product.service';
//import { ProductFamilyService } from './products/services/productFamily.service';
var product_service_1 = require("./products/services/product.service");
var basket_service_1 = require("./basket/services/basket.service");
var systemConfigurator_service_1 = require("./tools/systemConfigurator/services/systemConfigurator.service");
var splitSystemConfigurator_service_1 = require("./tools/splitSystemConfigurator/services/splitSystemConfigurator.service");
var tools_service_1 = require("./tools/tools.service");
var webconfig_service_1 = require("./shared/services/webconfig.service");
var order_service_1 = require("./order/services/order.service");
//import { provideRouter, RouterConfig } from '@angular/router';
var AppComponent = /** @class */ (function () {
    function AppComponent(router, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, systemConfiguratorSvc, SplitSystemConfiguratorSvc, webconfigSvc, passwordSvc) {
        var _this = this;
        this.router = router;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.systemConfiguratorSvc = systemConfiguratorSvc;
        this.SplitSystemConfiguratorSvc = SplitSystemConfiguratorSvc;
        this.webconfigSvc = webconfigSvc;
        this.passwordSvc = passwordSvc;
        this.pageTitle = 'Daikin Project Office';
        this.isAuthenticated = false;
        this.loading = true;
        router.events.subscribe(function (event) {
            _this.navigationInterceptor(event);
        });
    }
    AppComponent.prototype.ngOnInit = function () {
        //var hash = window.location.hash;
        //this.userSvc.isAuthenticated().then(this.isAuthenticatedCallBack.bind(this));
        this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
    };
    AppComponent.prototype.ngAfterViewChecked = function () {
        //var routeUrl = this.router.url;
        //this.setupActiveTab();
    };
    AppComponent.prototype.getCurrentUserCallback = function (resp) {
        if (resp.isok) {
            this.isAuthenticated = true;
            this.userSvc.userIsAuthenticated = true;
            this.currentUser = resp.model;
            this.userSvc.currentUser = resp.model;
            //this.setupActiveTab();
            for (var _i = 0, _a = resp.messages.items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.type == 40) {
                    this.toastrSvc.Success(message.text);
                }
            }
        }
        else {
            this.isAuthenticated = false;
            this.userSvc.userIsAuthenticated = false;
            //resp is null
            //for (let message of resp.messages.items) {
            //    if (message.type == 10) {// error
            //        this.toastrSvc.Error(message.text);
            //    }
            //}
        }
    };
    //public setupActiveTab() {
    //    var hash = window.location.hash;
    //    if (hash == "#/overview") {
    //        jQuery("#overviewTab").addClass('selected').siblings().removeClass('selected');
    //    } else if ((hash == "#/projects") || hash.includes("#/project/") || hash.includes("#/projectEdit/") || hash.includes("#/projectCreate") ) {
    //        jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash.includes("#/quote/") || hash.includes("#/quoteCreate/") || hash.includes("#/quoteEdit/")) {
    //        jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash.includes("#/tools")) {
    //        jQuery("#toolsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash == "#/products") {
    //        jQuery("#productsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash == "#/orders") {
    //        jQuery("#ordersTab").addClass('selected').siblings().removeClass('selected');
    //    }
    //    jQuery("#nav-bar li").click(function () {
    //        jQuery(this).addClass('selected').siblings().removeClass('selected');
    //    });
    //}
    AppComponent.prototype.navigationInterceptor = function (event) {
        if (event instanceof router_1.NavigationStart) {
            this.loading = true;
            this.loadingIconSvc.Start(jQuery("#content"));
        }
        if (event instanceof router_1.NavigationEnd) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }
        // Set loading state to false in both of the below events to hide the spinner in case a request fails
        if (event instanceof router_1.NavigationCancel) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }
        if (event instanceof router_1.NavigationError) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            //styleUrls: [
            //    // load the default theme (use correct path to node_modules)
            //    'node_modules/@progress/kendo-theme-default/dist/all.css'
            //],
            //styles: ['/deep/ #testdiv { color: red;}'],
            //styleUrls: [
            //    'app/content/test.css'
            templateUrl: 'app/app.component.html',
            //directives: [ROUTER_DIRECTIVES, HeaderButtonsComponent, ProjectsComponent],
            providers: [
                toastr_service_1.ToastrService,
                loadingIcon_service_1.LoadingIconService,
                user_service_1.UserService,
                password_service_1.PasswordService,
                systemAccessEnum_1.SystemAccessEnum,
                enums_1.Enums,
                account_service_1.AccountService,
                business_service_1.BusinessService,
                user_resolver_service_1.UserResolver,
                project_service_1.ProjectService,
                product_service_1.ProductService,
                order_service_1.OrderService,
                systemConfigurator_service_1.SystemConfiguratorService,
                splitSystemConfigurator_service_1.SplitSystemConfiguratorService,
                tools_service_1.ToolsService,
                basket_service_1.BasketService,
                webconfig_service_1.WebConfigService
            ]
        }),
        __metadata("design:paramtypes", [router_1.Router, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum, product_service_1.ProductService,
            systemConfigurator_service_1.SystemConfiguratorService,
            splitSystemConfigurator_service_1.SplitSystemConfiguratorService,
            webconfig_service_1.WebConfigService, password_service_1.PasswordService])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
;
//# sourceMappingURL=app.component.js.map