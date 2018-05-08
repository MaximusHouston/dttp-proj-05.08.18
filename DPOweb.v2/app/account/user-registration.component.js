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
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var password_service_1 = require("../shared/services/password.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var account_service_1 = require("./services/account.service");
var business_service_1 = require("../business/services/business.service");
var kendo_angular_dropdowns_1 = require("@progress/kendo-angular-dropdowns");
var UserRegistrationComponent = /** @class */ (function () {
    function UserRegistrationComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, businessSvc, passwordSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.businessSvc = businessSvc;
        this.passwordSvc = passwordSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.businessTypeDLLDisabled = false;
        this.foundBusiness = false;
        this.searchBtnClicked = false;
        this.showAccountIdSearch = false;
        this.showDakinAccRadioBtn = false;
        this.useBusinessAddress = false;
        this.hasDaikinAccount = false;
        this.defaultItem = { text: "Select...", value: null };
        this.phoneNumberMask = "(000) 000-0000";
        this.user = this.route.snapshot.data['user'].model;
        //this.accountSvc.getUserRegistrationModel().then((resp: any) => {
        //    //self.loadingIconSvc.Stop(jQuery("#productPageContainer"));
        //    toastrSvc.displayResponseMessages(resp);
        //    if (resp.isok) {
        //        this.user = resp.model;
        //        //window.location.href = resp.model;
        //        //self.userSvc.userIsAuthenticated = true;
        //        //this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
        //        //self.userAuthenticationEvt.emit({});
        //    }
        //});
    }
    UserRegistrationComponent.prototype.ngOnInit = function () {
        //this.pageTitle = this.route.snapshot.data['pageTitle'];
        this.user.business.parentBusinessId = null;
        this.getDistributorsAndReps();
        this.setupPasswordStrengthIndicator();
        this.setupSearchBusiness();
    };
    UserRegistrationComponent.prototype.getDistributorsAndReps = function () {
        var self = this;
        this.businessSvc.getDistributorsAndReps("")
            .subscribe(function (data) {
            self.distributorsAndReps = data.model;
        }, function (err) { return console.log("Error: ", err); });
    };
    UserRegistrationComponent.prototype.distributorsAndRepsFilter = function (value) {
        if (value.length >= 2) {
            //this.distributorsAndRepsList = this.distributorsAndReps.filter((s:any) => s.businessName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
            this.distributorsAndRepsList = this.distributorsAndReps.filter(function (s) { return s.businessName.toLowerCase().startsWith(value.toLowerCase()); });
        }
        else {
            this.distRepsCombo.toggle(false);
        }
    };
    UserRegistrationComponent.prototype.distRepsComboChange = function (event) {
    };
    UserRegistrationComponent.prototype.setupPasswordStrengthIndicator = function () {
        var self = this;
        jQuery("#userPassword").keyup(function (event) {
            event.stopImmediatePropagation();
            var password = jQuery("#userPassword")[0].value;
            self.showPasswordStrength(password);
        });
    };
    UserRegistrationComponent.prototype.showPasswordStrength = function (password) {
        if (this.passwordSvc.PasswordStrengthLevel(password) == 0) {
            $('#passwordStrengthBar').css("background-color", "#ddd");
            $('#passwordStrengthBar').css("width", "0%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 1) {
            $('#passwordStrengthBar').css("background-color", "#ff704d");
            $('#passwordStrengthBar').css("width", "10%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 2) {
            $('#passwordStrengthBar').css("background-color", "#ffcc66");
            $('#passwordStrengthBar').css("width", "30%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 3) {
            $('#passwordStrengthBar').css("background-color", "#ffcc66");
            $('#passwordStrengthBar').css("width", "50%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 4) {
            $('#passwordStrengthBar').css("background-color", "#80bfff");
            $('#passwordStrengthBar').css("width", "70%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) >= 5) {
            $('#passwordStrengthBar').css("background-color", "#5cd65c");
            $('#passwordStrengthBar').css("width", "100%");
        }
    };
    UserRegistrationComponent.prototype.setupSearchBusiness = function () {
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
            self.searchBtnClicked = true;
            event.stopImmediatePropagation();
            var value = jQuery("#businessSearchBox")[0].value;
            if (value) {
                self.accountSvc.businessAddressLookup(value).then(self.businessAddressLookupCallback.bind(self));
            }
            else {
                self.foundBusiness = false;
                self.user.existingBusiness = self.enums.ExistingBusinessEnum.New;
                self.useBusinessAddress = false;
            }
        });
    };
    UserRegistrationComponent.prototype.lookupBusiness = function (event) {
        this.searchBtnClicked = false;
        var self = this;
        //var value = jQuery("#businessSearchBox")[0].value;
        if (this.user.accountId != "") {
            self.accountSvc.businessAddressLookup(this.user.accountId)
                .then(function (resp) {
                if (resp.model.businessId != null && resp.model.businessId != 0) {
                    self.applyAccountId(resp);
                }
                else {
                    self.foundBusiness = false;
                    self.user.existingBusiness = self.enums.ExistingBusinessEnum.New;
                    self.useBusinessAddress = false;
                    console.log("foundBusiness : " + self.foundBusiness);
                }
            }).catch(function (error) {
                console.log(error);
            });
        }
        else {
            self.foundBusiness = false;
            self.user.existingBusiness = self.enums.ExistingBusinessEnum.New;
            self.useBusinessAddress = false;
        }
    };
    UserRegistrationComponent.prototype.businessAddressLookupCallback = function (resp) {
        if (resp.isok) {
            if (resp.model.accountId != null || resp.model.daikinCityId != null) {
                this.applyAccountId(resp);
                //this.business = resp.model;
                //this.user.business.businessId = resp.model.businessId;
                //this.user.business.businessName = resp.model.businessName;
                //this.user.business.accountId = resp.model.accountId;
                //this.user.business.daikinCityId = resp.model.daikinCityId;
                ////this.user.business = resp.model;
                //this.foundBusiness = true;
                //this.user.existingBusiness = this.enums.ExistingBusinessEnum.Existing;
                //if (this.useBusinessAddress) {
                //    this.UseBusinessAddress();
                //}
            }
            else {
                this.toastrSvc.Warning("Business not found!");
                this.foundBusiness = false;
                this.user.existingBusiness = this.enums.ExistingBusinessEnum.New;
                this.useBusinessAddress = false;
            }
        }
    };
    UserRegistrationComponent.prototype.applyAccountId = function (resp) {
        this.business = resp.model;
        this.user.business.businessId = resp.model.businessId;
        this.user.business.businessName = resp.model.businessName;
        this.user.business.accountId = resp.model.accountId;
        this.user.business.daikinCityId = resp.model.daikinCityId;
        //this.user.business = resp.model;
        this.foundBusiness = true;
        this.user.existingBusiness = this.enums.ExistingBusinessEnum.Existing;
        if (this.useBusinessAddress) {
            this.UseBusinessAddress();
        }
    };
    UserRegistrationComponent.prototype.UseBusinessAddressToggle = function (event) {
        if (event.target.checked) {
            this.useBusinessAddress = true;
            this.UseBusinessAddress();
        }
        else {
            this.useBusinessAddress = false;
            this.user.useBusinessAddress = false;
            if (this.user.address != null) {
                this.user.address.addressId = null;
            }
            if (this.user.contact != null) {
                this.user.contact.contactId = null;
            }
        }
    };
    UserRegistrationComponent.prototype.UseBusinessAddress = function () {
        if (this.useBusinessAddress) {
            this.user.useBusinessAddress = true;
            this.user.address = Object.assign({}, this.business.address);
            this.user.address.stateId = this.business.address.stateId.toString();
            this.user.contact = Object.assign({}, this.business.contact);
        }
    };
    UserRegistrationComponent.prototype.BusinessTypeChange = function (selectedItem) {
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
            this.foundBusiness = false;
            this.showDakinAccRadioBtn = true;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
    };
    UserRegistrationComponent.prototype.HasDaikinAccountChange = function (event) {
        if (event == "true") {
            this.showAccountIdSearch = true;
            $('#businessAddressLabel').text("USER ADDRESS DETAILS");
            this.user.existingBusiness = this.enums.ExistingBusinessEnum.Existing;
        }
        else {
            this.user.existingBusiness = this.enums.ExistingBusinessEnum.New;
            this.showAccountIdSearch = false;
            this.foundBusiness = false;
            this.useBusinessAddress = false;
            this.user.useBusinessAddress = false;
            this.businessTypeDLLDisabled = false;
            this.user.accountId = null;
            this.user.business.accountId = null;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
    };
    UserRegistrationComponent.prototype.backToLogin = function () {
        window.location.href = "/v2/#/account/login";
    };
    UserRegistrationComponent.prototype.register = function () {
        var self = this;
        if (this.user.password != this.user.confirmPassword) {
            this.toastrSvc.ErrorFadeOut("Password and confirm password do not match!");
        }
        else if (this.accountIdValid()) {
            self.loadingIconSvc.Start(jQuery("#content"));
            this.accountSvc.userRegistration(this.user).then(function (resp) {
                if (resp.IsOK) {
                    self.loadingIconSvc.Stop(jQuery("#content"));
                    window.location.href = '/v2/#/registrationAcknowledgement';
                }
                else {
                    self.loadingIconSvc.Stop(jQuery("#content"));
                    //self.toastrSvc.displayResponseMessages(resp);
                    if (resp != null && resp.Messages != null) {
                        for (var _i = 0, _a = resp.Messages.Items; _i < _a.length; _i++) {
                            var message = _a[_i];
                            if (message.Type == 40) {
                                self.toastrSvc.Success(message.Text);
                            }
                            else if (message.Type == 10) {
                                self.toastrSvc.ErrorFadeOut(message.Text);
                            }
                        }
                    }
                }
            }).catch(function (error) {
                self.loadingIconSvc.Stop(jQuery("#content"));
                console.log('Retrieval error: ${error}');
                console.log(error);
            });
        }
    };
    UserRegistrationComponent.prototype.accountIdValid = function () {
        if (this.user.business.businessTypeId == this.enums.BusinessTypeEnum.Daikin
            || this.user.business.businessTypeId == this.enums.BusinessTypeEnum.Distributor
            || this.user.business.businessTypeId == this.enums.BusinessTypeEnum.ManufacturerRep) {
            if (this.user.accountId == null || this.user.accountId == "") {
                this.toastrSvc.ErrorFadeOut("Account Id is required.");
                return false;
            }
            else if (!this.foundBusiness) {
                this.toastrSvc.ErrorFadeOut("Account Id is not found.");
                return false;
            }
            else if (this.foundBusiness) {
                return true;
            }
        }
        else {
            if (this.hasDaikinAccount == "false") {
                return true;
            }
            if (this.hasDaikinAccount == "true" && !this.foundBusiness) {
                this.toastrSvc.ErrorFadeOut("Account Id is required.");
                return false;
            }
            else if (this.foundBusiness) {
                return true;
            }
        }
    };
    __decorate([
        core_1.ViewChild('DistRepsCombo'),
        __metadata("design:type", kendo_angular_dropdowns_1.ComboBoxComponent)
    ], UserRegistrationComponent.prototype, "distRepsCombo", void 0);
    UserRegistrationComponent = __decorate([
        core_1.Component({
            selector: "user-registration",
            templateUrl: "app/account/user-registration.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService,
            business_service_1.BusinessService, password_service_1.PasswordService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], UserRegistrationComponent);
    return UserRegistrationComponent;
}());
exports.UserRegistrationComponent = UserRegistrationComponent;
;
//# sourceMappingURL=user-registration.component.js.map