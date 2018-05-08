"use strict";
/*
    Deprecated!
    Not working after upgrade kendo grid
*/
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
var TagEditPopupComponent = /** @class */ (function () {
    function TagEditPopupComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, systemAccessEnum, enums) {
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
    TagEditPopupComponent.prototype.ngOnInit = function () {
        this.modalId = this.quoteItem.quoteItemId;
        this.modalTarget = "#" + this.modalId;
        this.oldTagsValue = this.quoteItem.tags;
    };
    TagEditPopupComponent.prototype.closeTagEditor = function () {
        this.quoteItem.tags = this.oldTagsValue;
    };
    TagEditPopupComponent.prototype.saveTagUpdate = function () {
        var self = this;
        var data = {
            'QuoteId': this.quoteItem.quoteId,
            'Items': [this.quoteItem]
        };
        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));
        this.quoteSvc.adjustQuoteItems(data).then(function (resp) {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.toastrSvc.displayResponseMessages(resp);
                self.oldTagsValue = self.quoteItem.tags;
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(function (error) {
            console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TagEditPopupComponent.prototype, "quote", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TagEditPopupComponent.prototype, "quoteItem", void 0);
    TagEditPopupComponent = __decorate([
        core_1.Component({
            selector: "tag-edit-popup",
            templateUrl: "app/quote/tag-edit-popup.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums])
    ], TagEditPopupComponent);
    return TagEditPopupComponent;
}());
exports.TagEditPopupComponent = TagEditPopupComponent;
//# sourceMappingURL=tag-edit-popup.component.js.map