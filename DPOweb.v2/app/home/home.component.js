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
var http_1 = require("@angular/http");
var router_1 = require("@angular/router");
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var account_service_1 = require("../account/services/account.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var HomeComponent = /** @class */ (function () {
    function HomeComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, systemAccessEnum, enums, http) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.links = [];
        this.canViewProjects = false;
        this.canEditProjects = false;
        this.canViewOrders = false;
        this.canViewUsers = false;
        this.canManageGroups = false;
        this.canViewBusiness = false;
        this.canAccessLibrary = false;
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    HomeComponent.prototype.ngOnInit = function () {
        //debugger
        //document.location.href = "/";
        this.preventDisplayMenuCloseOnClick();
        this.canViewProjects = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewProject);
        this.canEditProjects = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditProject);
        this.canViewOrders = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewOrder);
        this.canViewUsers = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewUsers);
        this.canManageGroups = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ManageGroups);
        this.canViewBusiness = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewBusiness);
        this.getLinks();
        this.accountSvc.resetBasketQuoteId();
    };
    HomeComponent.prototype.getLinks = function () {
        this.links = [{
                text: "Library",
                image: "/v2/app/images/library.png",
                altText: "library",
                url: "/library",
                show: true
            }, {
                text: "Products",
                image: "/v2/app/images/products.png",
                altText: "Products",
                url: "/v2/#/products",
                show: true
            }, {
                text: "Tools",
                image: "/v2/app/images/tools.png",
                altText: "Home",
                url: "/v2/#/tools",
                show: true
            }, {
                text: "University",
                image: "/v2/app/images/university.png",
                altText: "Home",
                url: "/Training",
                show: true
            }, {
                text: "User Manual",
                image: "/v2/app/images/user_manual.png",
                altText: "Home",
                url: "/Content/pdf/DaikinCityUserGuide.pdf",
                show: true
            }];
        if (this.canViewProjects || this.canEditProjects) {
            var projectNode = {
                text: "Projects",
                image: "/v2/app/images/projects.png",
                altText: "Projects",
                url: "/v2/#/projects",
                show: true
            };
            this.links.splice(1, 0, projectNode);
            var overviewNode = {
                text: "Reports",
                image: "/v2/app/images/reporting.png",
                altText: "Reports",
                url: "/ProjectDashboard/Overview",
                show: true
            };
            this.links.splice(3, 0, overviewNode);
        }
        if (this.canViewOrders) {
            var orderNode = {
                text: "Orders",
                image: "/v2/app/images/orders.png",
                altText: "Orders",
                url: "/ProjectDashboard/Orders",
                show: true
            };
            this.links.splice(4, 0, orderNode);
        }
        if (this.canViewUsers || this.canViewBusiness || this.canManageGroups) {
            if (this.canViewUsers) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/users/",
                    show: true
                };
                this.links.push(managementNode);
            }
            else if (this.canViewBusiness) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/businesses/",
                    show: true
                };
                this.links.push(managementNode);
            }
            else if (this.canManageGroups) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/groups/",
                    show: true
                };
                this.links.push(managementNode);
            }
        }
    };
    //browseProducts() {
    //    this.accountSvc.resetBasketQuoteId()
    //        .then((resp: any) => {
    //            if (resp.isok) {
    //                this.router.navigateByUrl("/products");
    //            }
    //        }).catch(error => {
    //            console.log(error);
    //        });
    //}
    HomeComponent.prototype.preventDisplayMenuCloseOnClick = function () {
        $('.dropdown-menu').on('click', function (e) {
            if ($(this).hasClass('display-menu')) {
                e.stopPropagation();
            }
        });
    };
    HomeComponent = __decorate([
        core_1.Component({
            selector: 'home',
            templateUrl: 'app/home/home.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute,
            toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http])
    ], HomeComponent);
    return HomeComponent;
}());
exports.HomeComponent = HomeComponent;
;
//# sourceMappingURL=home.component.js.map