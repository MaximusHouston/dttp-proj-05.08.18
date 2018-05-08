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
require("rxjs/Rx");
var toastr_service_1 = require("../services/toastr.service");
var user_service_1 = require("../services/user.service");
var webconfig_service_1 = require("../services/webconfig.service");
var systemAccessEnum_1 = require("../services/systemAccessEnum");
var HeaderButtonsComponent = /** @class */ (function () {
    function HeaderButtonsComponent(toastrSvc, userSvc, systemAccessEnum, webconfigSvc) {
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.webconfigSvc = webconfigSvc;
    }
    HeaderButtonsComponent.prototype.ngOnChanges = function () {
        //was working ok, but maybe slow
        //this.userSvc.getCurrentUser()
        //    .then(this.getCurrentUserCallback.bind(this));
        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
        }
    };
    HeaderButtonsComponent.prototype.ngOnInit = function () {
        //this.userSvc.isAuthenticated().then(this.isAuthenticatedCallBack.bind(this));
        //was working ok, but maybe slow
        //this.userSvc.getCurrentUser()
        //    .then(this.getCurrentUserCallback.bind(this));
        var self = this;
        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
        }
        this.webconfigSvc.getWebConfig().then(function (resp) {
            self.webconfig = resp;
            self.libraryLink = self.webconfig.routeConfig.libraryLink;
        }).catch(function (error) {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });
    };
    //public isAuthenticatedCallBack(resp: any) {
    //    if (resp.isok) {
    //        if (resp.model == true) {
    //            //setup user header menu
    //            this.isAuthenticated = true;
    //            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
    HeaderButtonsComponent.prototype.isAuthenticatedCallBack = function (resp) {
        if (resp.isok) {
            if (resp.model == true) {
                //setup user header menu
                this.isAuthenticated = true;
                //        } else {
                //            //Go back to login page
                //            //window.location.href = "/Account/Login";
            }
        }
    };
    HeaderButtonsComponent.prototype.getCurrentUserCallback = function (resp) {
        if (resp.isok) {
            this.currentUser = resp.model;
            this.userSvc.currentUser = resp.model;
            var userNameElem = jQuery("#loggedin-username");
            userNameElem.text(this.currentUser.displayName);
            var projectOfficeBtn = jQuery("#projectOfficeBtn");
            projectOfficeBtn.attr("href", this.currentUser.defaultPageUrl);
            var canViewUsers = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ViewUsers"));
            var canManageGroups = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ManageGroups"));
            var canViewBusiness = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ViewBusiness"));
            var userOptsDropdownElem = jQuery("#userOptionsDropdown");
            if (canManageGroups || canViewBusiness || canViewUsers) {
                var management_li = "";
                if (canManageGroups) {
                    management_li = '<li><a href="/Userdashboard/groups/">MANAGEMENT</a></li>';
                }
                if (canViewBusiness) {
                    management_li = '<li><a href="/Userdashboard/businesses/">MANAGEMENT</a></li>';
                }
                if (canViewUsers) {
                    management_li = '<li><a href="/Userdashboard/users/">MANAGEMENT</a></li>';
                }
                //userOptsDropdownElem.append(management_li);
                $("#hidden-management-li").replaceWith(management_li);
            }
            if (this.currentUser.hasAccessToCMS) {
                var CMS_li = "";
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementTools"))) {
                    CMS_li = '<li><a href="/CityCMS/tools/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementProductFamilies"))) {
                    CMS_li = '<li><a href="/CityCMS/productfamilies/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementCommsCenter"))) {
                    CMS_li = '<li><a href="/CityCMS/communicationscenter/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementLibrary"))) {
                    CMS_li = '<li><a href="/CityCMS/library/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementApplicationProducts"))) {
                    CMS_li = '<li><a href="/CityCMS/applicationproducts/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementApplicationBuildings"))) {
                    CMS_li = '<li><a href="/CityCMS/applicationbuildings/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementFunctionalBuildings"))) {
                    CMS_li = '<li><a href="/CityCMS/functionalbuildings/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementHomeScreen"))) {
                    CMS_li = '<li><a href="/CityCMS/homescreen/">CONTENT MANAGEMENT</a></li>';
                }
                //userOptsDropdownElem.append(CMS_li);
                $("#hidden-content-management-li").replaceWith(CMS_li);
            }
            var signOut_li = '<li><a href="/Account/Logoff">SIGN OUT</a></li>';
            //userOptsDropdownElem.append(signOut_li);
            var cityLocationsDropdownElem = jQuery("#cityLocationsDropdown");
            if (this.currentUser.cityAccesses.indexOf(1) > -1) {
                //var cityLibrary_li = '<li><a href="/#library"> <span class="glyphicon glyphicon-menu-right"></span> LIBRARY</a></li>';
                //cityLocationsDropdownElem.append(cityLibrary_li);
                this.canAccessLibrary = true;
            }
            if (this.currentUser.cityAccesses.indexOf(2) > -1) {
                //var logisticsCenter_li = '<li><a href="/#logisticscenter"> <span class="glyphicon glyphicon-menu-right"></span> LOGISTICS CENTER</a></li>';
                //cityLocationsDropdownElem.append(logisticsCenter_li);
                this.canAccessLogistic = true;
            }
            for (var _i = 0, _a = resp.messages.items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.type == 40) {
                    this.toastrSvc.Success(message.text);
                }
            }
        }
        else if (resp.messages) {
            //window.location.href = "/Account/Login";
            for (var _b = 0, _c = resp.messages.items; _b < _c.length; _b++) {
                var message = _c[_b];
                if (message.type == 10) {
                    this.toastrSvc.Error(message.text);
                }
            }
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], HeaderButtonsComponent.prototype, "isAuthenticated", void 0);
    HeaderButtonsComponent = __decorate([
        core_1.Component({
            selector: 'header-buttons',
            templateUrl: "app/shared/header/headerButtons.component.html",
            styleUrls: ["app/shared/header/headerButtons.component.css"],
            providers: [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum]
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            webconfig_service_1.WebConfigService])
    ], HeaderButtonsComponent);
    return HeaderButtonsComponent;
}());
exports.HeaderButtonsComponent = HeaderButtonsComponent;
;
//# sourceMappingURL=headerButtons.component.js.map