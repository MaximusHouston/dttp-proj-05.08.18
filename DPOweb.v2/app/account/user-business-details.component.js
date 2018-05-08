"use strict";
//TODO: This component is not used because signUpForm valiation does not work
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
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var account_service_1 = require("./services/account.service");
var UserBusinessDetailsComponent = /** @class */ (function () {
    function UserBusinessDetailsComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.businessTypeDLLDisabled = false;
        this.foundBusinessAddress = false;
        this.showAccountIdSearch = false;
        this.showDakinAccRadioBtn = false;
        this.useBusinessAddress = false;
        this.hasDaikinAccount = false;
        this.defaultItem = { text: "Select...", value: null };
        this.phoneNumberMask = "(000) 000-0000";
    }
    UserBusinessDetailsComponent.prototype.ngOnInit = function () {
        this.setupSearchBusiness();
    };
    //public searchBusiness() {
    //    this.accountSvc.businessAddressLookup({ accountId: "A11198" }).then((resp: any) => {
    //        this.toastrSvc.displayResponseMessages(resp);
    //        if (resp.isok) {
    //            debugger
    //            //this.user = resp.model;
    //        }
    //    });
    //}
    UserBusinessDetailsComponent.prototype.setupSearchBusiness = function () {
        var self = this;
        jQuery("#businessSearchBox").keyup(function (event) {
            event.stopImmediatePropagation();
            var value = jQuery("#businessSearchBox")[0].value;
            if (value) {
                self.businessTypeDLLDisabled = true;
            }
            else {
                self.businessTypeDLLDisabled = false;
                self.useBusinessAddress = false;
            }
            if (event.keyCode == 13) {
                jQuery("#businessSearchBtn").click();
            }
        });
        jQuery("#businessSearchBtn").click(function (event) {
            event.stopImmediatePropagation();
            var value = jQuery("#businessSearchBox")[0].value;
            if (value) {
                self.accountSvc.businessAddressLookup(value).then(self.businessAddressLookupCallback.bind(self));
            }
            else {
                self.foundBusinessAddress = false;
                self.useBusinessAddress = false;
            }
        });
    };
    UserBusinessDetailsComponent.prototype.businessAddressLookupCallback = function (resp) {
        if (resp.isok) {
            if (resp.model.accountId != null) {
                this.business = resp.model;
                this.user.business.accountId = resp.model.accountId;
                this.foundBusinessAddress = true;
                if (this.useBusinessAddress) {
                    this.UseBusinessAddress();
                }
            }
            else {
                this.toastrSvc.Warning("Business not found!");
                this.foundBusinessAddress = false;
                this.useBusinessAddress = false;
            }
        }
    };
    UserBusinessDetailsComponent.prototype.UseBusinessAddress = function () {
        if (this.useBusinessAddress) {
            this.user.address = this.business.address;
            this.user.address.stateId = this.business.address.stateId.toString();
            this.user.contact = this.business.contact;
        }
    };
    UserBusinessDetailsComponent.prototype.BusinessTypeChange = function (selectedItem) {
        if (selectedItem.value == this.enums.BusinessTypeEnum.Daikin
            || selectedItem.value == this.enums.BusinessTypeEnum.Distributor
            || selectedItem.value == this.enums.BusinessTypeEnum.ManufacturerRep) {
            this.showAccountIdSearch = true;
            this.showDakinAccRadioBtn = false;
            this.hasDaikinAccount = true;
            $('#businessAddressLabel').text("USER ADDRESS DETAILS");
        }
        else {
            this.showAccountIdSearch = false;
            this.foundBusinessAddress = false;
            this.showDakinAccRadioBtn = true;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
        //this.rowItem.furnace_Model = selectedItem.value;
        //this.furnaceSelectedEvent.emit(selectedItem);
    };
    UserBusinessDetailsComponent.prototype.HasDaikinAccountChange = function (event) {
        if (event == "true") {
            this.showAccountIdSearch = true;
            $('#businessAddressLabel').text("USER ADDRESS DETAILS");
        }
        else {
            this.showAccountIdSearch = false;
            this.foundBusinessAddress = false;
            this.useBusinessAddress = false;
            this.businessTypeDLLDisabled = false;
            this.user.accountId = null;
            this.user.business.accountId = null;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], UserBusinessDetailsComponent.prototype, "user", void 0);
    UserBusinessDetailsComponent = __decorate([
        core_1.Component({
            selector: "user-business-details",
            templateUrl: "app/account/user-business-details.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], UserBusinessDetailsComponent);
    return UserBusinessDetailsComponent;
}());
exports.UserBusinessDetailsComponent = UserBusinessDetailsComponent;
;
//# sourceMappingURL=user-business-details.component.js.map