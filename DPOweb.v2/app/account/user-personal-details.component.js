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
var password_service_1 = require("../shared/services/password.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var account_service_1 = require("./services/account.service");
var UserPersonalDetailsComponent = /** @class */ (function () {
    function UserPersonalDetailsComponent(router, route, toastrSvc, loadingIconSvc, passwordSvc, userSvc, accountSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.passwordSvc = passwordSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.defaultItem = { text: "Select...", value: null };
        this.phoneNumberMask = "(000) 000-0000";
    }
    UserPersonalDetailsComponent.prototype.ngOnInit = function () {
        this.setupPasswordStrengthIndicator();
    };
    UserPersonalDetailsComponent.prototype.setupPasswordStrengthIndicator = function () {
        var self = this;
        jQuery("#userPassword").keyup(function (event) {
            event.stopImmediatePropagation();
            var password = jQuery("#userPassword")[0].value;
            self.showPasswordStrength(password);
        });
    };
    UserPersonalDetailsComponent.prototype.showPasswordStrength = function (password) {
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
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], UserPersonalDetailsComponent.prototype, "user", void 0);
    UserPersonalDetailsComponent = __decorate([
        core_1.Component({
            selector: "user-personal-details",
            templateUrl: "app/account/user-personal-details.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService, password_service_1.PasswordService,
            user_service_1.UserService, account_service_1.AccountService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], UserPersonalDetailsComponent);
    return UserPersonalDetailsComponent;
}());
exports.UserPersonalDetailsComponent = UserPersonalDetailsComponent;
;
//# sourceMappingURL=user-personal-details.component.js.map