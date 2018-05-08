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
var toastr_service_1 = require("../services/toastr.service");
var loadingIcon_service_1 = require("../services/loadingIcon.service");
var user_service_1 = require("../services/user.service");
var systemAccessEnum_1 = require("../services/systemAccessEnum");
var enums_1 = require("../enums/enums");
var account_service_1 = require("../../account/services/account.service");
var product_service_1 = require("../../products/services/product.service");
var ProjectTabsComponent = /** @class */ (function () {
    function ProjectTabsComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, productSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.productSvc = productSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.isAuthenticated = false;
        this.canViewProject = false;
    }
    ProjectTabsComponent.prototype.ngOnInit = function () {
        this.userSvc.isAuthenticated().then(this.isAuthenticatedCallBack.bind(this));
        this.canViewProject = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewProject);
    };
    ProjectTabsComponent.prototype.ngAfterViewInit = function () {
        //this.isAuthenticated = this.userSvc.userIsAuthenticated;
    };
    ProjectTabsComponent.prototype.ngAfterViewChecked = function () {
        this.setupActiveTab();
    };
    ProjectTabsComponent.prototype.isAuthenticatedCallBack = function (resp) {
        if (resp.isok) {
            if (resp.model == true) {
                //setup user header menu
                this.isAuthenticated = true;
                this.userSvc.userIsAuthenticated = true;
                //this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
            }
            else {
                //Go back to login page
                //window.location.href = "/Account/Login";
            }
        }
    };
    //Navigate to Product page and set BasketQuoteId = 0 
    ProjectTabsComponent.prototype.Products = function () {
        this.productSvc.products();
    };
    ProjectTabsComponent.prototype.setupActiveTab = function () {
        var hash = window.location.hash;
        if (hash == "#/overview") {
            jQuery("#overviewTab").addClass('selected').siblings().removeClass('selected');
        }
        else if ((hash == "#/projects") || hash.includes("#/project/") || hash.includes("#/projectEdit/") || hash.includes("#/projectCreate")) {
            jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
        }
        else if (hash.includes("#/quote/") || hash.includes("#/quoteCreate/") || hash.includes("#/quoteEdit/")) {
            jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
        }
        else if (hash.includes("#/orderForm/")) {
            jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
        }
        else if (hash.includes("#/tools")) {
            jQuery("#toolsTab").addClass('selected').siblings().removeClass('selected');
        }
        else if (hash == "#/products") {
            jQuery("#productsTab").addClass('selected').siblings().removeClass('selected');
        }
        else if (hash == "#/orders") {
            jQuery("#ordersTab").addClass('selected').siblings().removeClass('selected');
        }
        jQuery("#nav-bar li").click(function () {
            jQuery(this).addClass('selected').siblings().removeClass('selected');
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectTabsComponent.prototype, "user", void 0);
    ProjectTabsComponent = __decorate([
        core_1.Component({
            selector: "project-tabs",
            templateUrl: "app/shared/projectTabs/projectTabs.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, product_service_1.ProductService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], ProjectTabsComponent);
    return ProjectTabsComponent;
}());
exports.ProjectTabsComponent = ProjectTabsComponent;
;
//# sourceMappingURL=projectTabs.component.js.map