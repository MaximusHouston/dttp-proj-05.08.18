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
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var account_service_1 = require("../account/services/account.service");
var quote_service_1 = require("./services/quote.service");
var QuoteCommissionRequestsComponent = /** @class */ (function () {
    function QuoteCommissionRequestsComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.quoteSvc = quoteSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
    }
    QuoteCommissionRequestsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.commissionRequests = this.quoteItems.commissionRequests.filter(function (cr) { return cr.commissionRequestStatusTypeId != _this.enums.CommissionRequestStatusTypeEnum.NewRecord; });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteCommissionRequestsComponent.prototype, "quote", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteCommissionRequestsComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteCommissionRequestsComponent.prototype, "quoteItems", void 0);
    QuoteCommissionRequestsComponent = __decorate([
        core_1.Component({
            selector: "quote-commission-requests",
            templateUrl: "app/quote/quote-commission-requests.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], QuoteCommissionRequestsComponent);
    return QuoteCommissionRequestsComponent;
}());
exports.QuoteCommissionRequestsComponent = QuoteCommissionRequestsComponent;
;
//# sourceMappingURL=quote-commission-requests.component.js.map