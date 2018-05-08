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
var webconfig_service_1 = require("../shared/services/webconfig.service");
var SelectProductsDialogComponent = /** @class */ (function () {
    function SelectProductsDialogComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, webconfigSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.quoteSvc = quoteSvc;
        this.webconfigSvc = webconfigSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.toolList = [];
        this.defaultItem = { text: "Select ...", value: null };
    }
    SelectProductsDialogComponent.prototype.ngOnInit = function () {
        //var tools = [];
        var self = this;
        this.webconfigSvc.getWebConfig().then(function (resp) {
            self.webconfig = resp;
        }).catch(function (error) {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });
        this.webconfigSvc.getLCSTApiToken().then(function (resp) {
            self.lcstApiToken = resp.model;
        }).catch(function (error) {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });
        for (var i in this.user.toolAccesses) {
            if (this.user.toolAccesses[i].addToQuote == 1) {
                this.toolList.push({ text: this.user.toolAccesses[i].name, value: this.user.toolAccesses[i].toolId });
            }
        }
        if (this.toolList.length == 0) {
            $("#selectProductsBtn").hide();
        }
    };
    SelectProductsDialogComponent.prototype.openTool = function () {
        var self = this;
        //alert("selected tool: " + this.selectedTool.text);
        if (this.selectedTool.value == this.enums.ToolAccessEnum.UnitaryMatchupTool) {
            //window.location.href = "/api/Tool/SystemConfigurator?quoteId=" + this.quote.quoteId;
            this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then(function (resp) {
                if (resp.isok) {
                    window.location.href = "/v2/#/tools/systemConfigurator";
                    self.toastrSvc.displayResponseMessages(resp);
                }
                else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(function (error) {
                console.log('Retrieval error: ${error}');
                console.log(error);
            });
        }
        else if (this.selectedTool.value == this.enums.ToolAccessEnum.CommercialSplitMatchupTool) {
            //window.location.href = "/api/Tool/SplitSystemConfigurator?quoteId=" + this.quote.quoteId;
            this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then(function (resp) {
                if (resp.isok) {
                    window.location.href = "/v2/#/tools/splitSystemConfigurator";
                    self.toastrSvc.displayResponseMessages(resp);
                }
                else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(function (error) {
                console.log('Retrieval error: ${error}');
                console.log(error);
            });
        }
        else if (this.selectedTool.value == this.enums.ToolAccessEnum.LCSubmittalTool) {
            window.location.href = this.webconfig.routeConfig.lcstURL + "&quoteId=" + this.quote.quoteId + "&projectId=" + this.quote.projectId + "&projectName=" + this.quote.project.name + "&userId=" + this.user.userId + "&firstName=" + this.user.firstName + "&lastName=" + this.user.lastName + "&token=" + this.lcstApiToken;
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SelectProductsDialogComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SelectProductsDialogComponent.prototype, "quote", void 0);
    SelectProductsDialogComponent = __decorate([
        core_1.Component({
            selector: "select-products-dialog",
            templateUrl: "app/quote/select-products-dialog.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService, webconfig_service_1.WebConfigService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], SelectProductsDialogComponent);
    return SelectProductsDialogComponent;
}());
exports.SelectProductsDialogComponent = SelectProductsDialogComponent;
//# sourceMappingURL=select-products-dialog.component.js.map