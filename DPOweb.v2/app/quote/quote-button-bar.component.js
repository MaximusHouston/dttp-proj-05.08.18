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
var QuoteButtonBarComponent = /** @class */ (function () {
    function QuoteButtonBarComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, systemAccessEnum, enums) {
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
    QuoteButtonBarComponent.prototype.ngOnChanges = function (changes) {
        //console.log("ngOnChanges");
    };
    QuoteButtonBarComponent.prototype.ngOnInit = function () {
    };
    QuoteButtonBarComponent.prototype.openOrderForm = function () {
        //this.actionUrl = "/ProjectDashboard/OrderForm?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.actionUrl = "/v2/#/orderForm/" + this.quote.projectId + "/" + this.quote.quoteId + "/new";
        //this.actionUrl = "/orderForm/" + this.quote.projectId + "/" + this.quote.quoteId + "/new";
        this.setupInventoryCheckModal();
    };
    QuoteButtonBarComponent.prototype.requestDiscount = function () {
        this.actionUrl = "/ProjectDashboard/DiscountRequest?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.setupInventoryCheckModal();
    };
    QuoteButtonBarComponent.prototype.requestCommission = function () {
        this.actionUrl = "/ProjectDashboard/CommissionRequest?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.setupInventoryCheckModal();
    };
    QuoteButtonBarComponent.prototype.setupInventoryCheckModal = function () {
        if (this.quoteItems.hasObsoleteAndUnavailableProduct) {
            this.message = "This quote contains product(s) which are Obsolete and Unavailable. Please review and revise product(s) list to continue processing the quote or contact your Daikin Sales Representative.";
            jQuery("#inventoryCheckModal").modal({ backdrop: 'static', keyboard: false });
        }
        else if (this.quoteItems.hasObsoleteProduct || this.quoteItems.hasUnavailableProduct) {
            this.message = "This quote contains obsolete or unavailable product(s). Please review and revise product(s) list or contact your Daikin Sales Representative.";
            jQuery("#inventoryCheckModal").modal({ backdrop: 'static', keyboard: false });
        }
        else {
            window.location.href = this.actionUrl;
            //this.router.navigateByUrl(this.actionUrl);
        }
    };
    QuoteButtonBarComponent.prototype.redirect = function () {
        window.location.href = this.actionUrl;
        //this.router.navigateByUrl(this.actionUrl);
    };
    QuoteButtonBarComponent.prototype.setQuoteActive = function () {
        var _this = this;
        var self = this;
        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
        };
        self.loadingIconSvc.Start(jQuery("#content"));
        this.quoteSvc.setQuoteActive(data).then(function (resp) {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + _this.quote.projectId;
            }
            else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteButtonBarComponent.prototype.deleteQuote = function () {
        var _this = this;
        var self = this;
        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
        };
        self.loadingIconSvc.Start(jQuery("#content"));
        this.quoteSvc.deleteQuote(data).then(function (resp) {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + _this.quote.projectId;
            }
            else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteButtonBarComponent.prototype.undeleteQuote = function () {
        var _this = this;
        var self = this;
        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
        };
        self.loadingIconSvc.Start(jQuery("#content"));
        this.quoteSvc.undeleteQuote(data).then(function (resp) {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + _this.quote.projectId;
            }
            else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteButtonBarComponent.prototype.quotePrintNoPrices = function () {
        //var url = "/ProjectDashboard/QuotePrint?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=false";
        var url = "/Document/QuotePrint/" + this.quote.projectId + "?quoteId=" + this.quote.quoteId;
        window.open(url, '_blank');
    };
    QuoteButtonBarComponent.prototype.quotePrintWithPrices = function () {
        //var url = "/ProjectDashboard/QuotePrint?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=true";
        var url = "/Document/QuotePrintWithCostPrice/" + this.quote.projectId + "?quoteId=" + this.quote.quoteId;
        window.open(url, '_blank');
    };
    QuoteButtonBarComponent.prototype.quoteDownloadNoPrices = function () {
        var url = "/ProjectDashboard/QuotePrintExcel?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=false";
        window.open(url, '_blank');
    };
    QuoteButtonBarComponent.prototype.quoteDownloadWithPrices = function () {
        var url = "/ProjectDashboard/QuotePrintExcel?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=true";
        window.open(url, '_blank');
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteButtonBarComponent.prototype, "quote", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteButtonBarComponent.prototype, "quoteItems", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteButtonBarComponent.prototype, "user", void 0);
    QuoteButtonBarComponent = __decorate([
        core_1.Component({
            selector: "quote-button-bar",
            templateUrl: "app/quote/quote-button-bar.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], QuoteButtonBarComponent);
    return QuoteButtonBarComponent;
}());
exports.QuoteButtonBarComponent = QuoteButtonBarComponent;
;
//# sourceMappingURL=quote-button-bar.component.js.map