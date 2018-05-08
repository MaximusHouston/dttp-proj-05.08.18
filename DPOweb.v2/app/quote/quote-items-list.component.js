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
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var account_service_1 = require("../account/services/account.service");
var quote_service_1 = require("./services/quote.service");
var option_items_component_1 = require("./option-items.component");
var kendo_angular_dialog_1 = require("@progress/kendo-angular-dialog");
var QuoteItemsListComponent = /** @class */ (function () {
    function QuoteItemsListComponent(router, route, toastrSvc, loadingIconSvc, userSvc, accountSvc, quoteSvc, dialogSvc, systemAccessEnum, enums, formBuilder) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.accountSvc = accountSvc;
        this.quoteSvc = quoteSvc;
        this.dialogSvc = dialogSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.formBuilder = formBuilder;
        this.reloadDataEvent = new core_1.EventEmitter();
        this.reloadQuoteEvent = new core_1.EventEmitter();
        this.gridIsModified = false;
    }
    QuoteItemsListComponent.prototype.ngOnInit = function () {
        //this.originalQuoteItems = this.quoteItems.items;
        //this.originalQuoteItems = Object.assign({}, this.quoteItems.items);
        //this.originalQuoteItems = Object.create(this.quoteItems.items);
        //this.cloneQuoteItems();
        //this.originalQuoteItems = this.cloneQuoteItems(this.quoteItems.items);
    };
    QuoteItemsListComponent.prototype.ngAfterViewChecked = function () {
        if (!this.gridIsModified) {
            jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
        }
    };
    QuoteItemsListComponent.prototype.validateQuantity = function (event) {
        this.gridIsModified = true;
        jQuery("#quoteItemsGrid .k-grid-toolbar").show();
        //alert("this.quoteItems.items[0].quantity: " + this.quoteItems.items[0].quantity
        //    + "\n this.originalQuoteItems[0].quantity: " + this.originalQuoteItems[0].quantity)
        var value = parseFloat(event.target.value);
        if (value == null || isNaN(value)) {
            event.target.value = 0;
        }
        else if ((value < 0) || (Math.floor(value) != value)) {
            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    };
    //public onStateChange(state: State) {
    //    //this.gridState = state;
    //    //this.editService.read();
    //}
    QuoteItemsListComponent.prototype.saveChanges = function (grid) {
        var self = this;
        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));
        this.quoteSvc.adjustQuoteItems(this.quoteItems).then(function (resp) {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                //reload QuoteItems Grid if needed
                //for (let message of resp.messages.items) {
                //    if (message.type == 30 && message.text == "Item(s)-Removed") {
                //        self.reloadQuoteItems();
                //        break;
                //    }
                //}
                //Update optionItems quantity
                //self.optionItemsComponent.loadOptionItems(); // use ViewChildren instead
                //self.reloadQuoteEvent.emit();
                self.reloadDataEvent.emit();
                self.toastrSvc.displayResponseMessages(resp);
                self.gridIsModified = false;
                jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        });
    };
    //public saveChanges(grid: any) {
    //    var self = this;
    //    self.reloadDataEvent.emit();
    //}
    QuoteItemsListComponent.prototype.cancelChanges = function (grid) {
        this.reloadQuoteItems();
        //this.quoteItems.items = Object.assign({}, this.originalQuoteItems);
        //this.quoteItems.items = this.originalQuoteItems;
    };
    QuoteItemsListComponent.prototype.reloadQuote = function () {
        var self = this;
        //self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));
        self.quoteSvc.getQuote(self.quote.projectId, self.quote.quoteId).then(function (resp) {
            if (resp.isok) {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.quote = resp.model;
            }
            else {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteItemsListComponent.prototype.reloadQuoteItems = function () {
        var self = this;
        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));
        self.quoteSvc.getQuoteItemsModel(self.quote.quoteId).then(function (resp) {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.quoteItems = resp.model;
                self.gridIsModified = false;
                jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            }
            else {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteItemsListComponent.prototype.productDetails = function (dataItem) {
        //this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });
        var _this = this;
        //this.accountSvc.resetBasketQuoteId()
        //    .then((resp: any) => {
        //        if (resp.isok) {
        //            this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });
        //        }
        //    })
        //    .catch(error => {
        //        console.log(error);
        //    });
        this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then(function (resp) {
            if (resp.isok) {
                _this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });
            }
            else {
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteItemsListComponent.prototype.hasOptionItems = function (dataItem, index) {
        return dataItem.lineItemTypeId == 2;
    };
    //Tag Editor
    QuoteItemsListComponent.prototype.openTagEditor = function (dataItem) {
        this.quoteItem = dataItem;
        this.oldTagsValue = this.quoteItem.tags;
        $("#tagEditor").modal();
    };
    QuoteItemsListComponent.prototype.closeTagEditor = function () {
        this.quoteItem.tags = this.oldTagsValue;
    };
    QuoteItemsListComponent.prototype.saveTagUpdate = function () {
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
                //self.oldTagsValue = self.quoteItem.tags;
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
    ], QuoteItemsListComponent.prototype, "quote", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteItemsListComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], QuoteItemsListComponent.prototype, "quoteItems", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], QuoteItemsListComponent.prototype, "reloadDataEvent", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], QuoteItemsListComponent.prototype, "reloadQuoteEvent", void 0);
    __decorate([
        core_1.ViewChildren(option_items_component_1.OptionItemsComponent),
        __metadata("design:type", core_1.QueryList)
    ], QuoteItemsListComponent.prototype, "OptionItemsComponents", void 0);
    QuoteItemsListComponent = __decorate([
        core_1.Component({
            selector: "quote-items-list",
            templateUrl: "app/quote/quote-items-list.component.html"
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, account_service_1.AccountService, quote_service_1.QuoteService,
            kendo_angular_dialog_1.DialogService,
            systemAccessEnum_1.SystemAccessEnum, enums_1.Enums,
            forms_1.FormBuilder])
    ], QuoteItemsListComponent);
    return QuoteItemsListComponent;
}());
exports.QuoteItemsListComponent = QuoteItemsListComponent;
;
//# sourceMappingURL=quote-items-list.component.js.map