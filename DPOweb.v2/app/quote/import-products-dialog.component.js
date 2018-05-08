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
var ImportProductsDialogComponent = /** @class */ (function () {
    function ImportProductsDialogComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, systemAccessEnum, enums) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.quoteSvc = quoteSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.reloadDataEvent = new core_1.EventEmitter();
        //public importUrl: string = "/ProjectDashboard/QuoteImport";
        this.importUrl = "/api/Quote/QuoteImport";
    }
    ImportProductsDialogComponent.prototype.ngOnInit = function () {
    };
    ImportProductsDialogComponent.prototype.cancel = function () {
    };
    ImportProductsDialogComponent.prototype.import = function () {
    };
    ImportProductsDialogComponent.prototype.selectEventHandler = function (e) {
        console.log("File selected");
    };
    ImportProductsDialogComponent.prototype.uploadEventHandler = function (e) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.quote.quoteId,
            ProjectId: this.quote.projectId
        };
    };
    ImportProductsDialogComponent.prototype.successEventHandler = function (e) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
            this.toastrSvc.Success("Quote '" + this.quote.title + "' has been updated.");
            this.reloadDataEvent.emit();
            $('button.close[data-dismiss=modal]').click();
        }
    };
    ImportProductsDialogComponent.prototype.errorEventHandler = function (e) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ImportProductsDialogComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ImportProductsDialogComponent.prototype, "quote", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ImportProductsDialogComponent.prototype, "quoteItems", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ImportProductsDialogComponent.prototype, "reloadDataEvent", void 0);
    ImportProductsDialogComponent = __decorate([
        core_1.Component({
            selector: "import-products-dialog",
            templateUrl: "app/quote/import-products-dialog.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], ImportProductsDialogComponent);
    return ImportProductsDialogComponent;
}());
exports.ImportProductsDialogComponent = ImportProductsDialogComponent;
//# sourceMappingURL=import-products-dialog.component.js.map