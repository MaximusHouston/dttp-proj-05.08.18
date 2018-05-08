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
//split-matchup-detail-grid.component.ts
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var toastr_service_1 = require("../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../shared/services/loadingIcon.service");
var user_service_1 = require("../../shared/services/user.service");
var systemAccessEnum_1 = require("../../shared/services/systemAccessEnum");
var product_service_1 = require("../../products/services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var splitSystemConfigurator_service_1 = require("./services/splitSystemConfigurator.service");
var SplitMatchupDetailGridComponent = /** @class */ (function () {
    function SplitMatchupDetailGridComponent(router, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc, SplitSystemConfiguratorSvc) {
        this.router = router;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.SplitSystemConfiguratorSvc = SplitSystemConfiguratorSvc;
        this.sort = [];
        this.pageSize = 10;
        this.skip = 0;
        this.defaultItem = { text: "Select item...", value: null, fit: "N/A", afue: "N/A" };
    }
    SplitMatchupDetailGridComponent.prototype.ngOnChanges = function () {
    };
    SplitMatchupDetailGridComponent.prototype.ngOnInit = function () {
        this.loadData();
    };
    SplitMatchupDetailGridComponent.prototype.dataStateChange = function (_a) {
        var skip = _a.skip, take = _a.take, sort = _a.sort;
        this.skip = skip;
        this.pageSize = take;
        this.sort = sort;
        this.loadData();
    };
    SplitMatchupDetailGridComponent.prototype.loadData = function () {
        for (var key in this.matchupResultDetail) {
            if (!this.matchupResultDetail[key].furnace_Model) {
                this.matchupResultDetail[key].showFurnaceDDL = true;
            }
            else {
                this.matchupResultDetail[key].showFurnaceDDL = false;
            }
            //add quantity field
            if (this.matchupResultDetail.hasOwnProperty(key)) {
                this.matchupResultDetail[key].quantity = 0;
            }
        }
        if (this.matchupResultDetail != undefined) {
            this.gridViewData = {
                data: this.matchupResultDetail.slice(this.skip, this.skip + this.pageSize),
                total: this.matchupResultDetail.length
            };
        }
    };
    //public FurnaceSelected(selectedItem: any, rowIndex: any) {
    //    var fitValueCellId = "#fitValue-" + rowIndex;
    //    $(fitValueCellId).text(selectedItem.fit);
    //    var afueValueCellId = "#afueValue-" + rowIndex;
    //    $(afueValueCellId).text(selectedItem.afue);
    //}
    SplitMatchupDetailGridComponent.prototype.validateQuantity = function (event) {
        var value = parseFloat(event.target.value);
        if (value == null || isNaN(value)) {
            //this.product.quantity = 0;
            event.target.value = 0;
        }
        else if ((value < 0) || (Math.floor(value) != value)) {
            //this.product.quantity = 0;
            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    };
    SplitMatchupDetailGridComponent.prototype.addToQuote = function (item) {
        var self = this;
        var outdoorModel = item.outdoor_Model.substring(0, item.outdoor_Model.length - 2);
        var coilModel = item.coill_Model.substring(0, item.coill_Model.length - 2);
        if (item.quantity > 0) {
            self.system = {
                "ProductNumbers": [outdoorModel, coilModel],
                "Quantity": item.quantity,
                "AHRI": item.arirefNumber,
                "SEER": item.seer,
                "EER": item.eer,
                "Cooling": item.cooling,
                "Fit": item.fit,
                "AFUE": item.afue,
                "ContinueAdding": false,
                "ValidProducts": [],
                "InValidProducts": []
            };
            if (this.indoorUnitType = "airHandler") {
                if (item.blower_Model != null && item.blower_Model != "") {
                    var blowerModel = item.blower_Model.substring(0, item.blower_Model.length - 2);
                    self.system.ProductNumbers.push(blowerModel);
                }
            }
            this.addSystemToQuote(self.system);
        }
        else {
            this.toastrSvc.Info("Please enter quantity!");
        }
        item.quantity = 0;
    };
    SplitMatchupDetailGridComponent.prototype.addSystemToQuote = function (system) {
        var self = this;
        self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));
        this.productSvc.addSystemToQuote(system).then(function (resp) {
            self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
            if (resp.isok) {
                //update basket item count
                self.basketSvc.getBasket().then(function (resp) {
                    if (resp.isok) {
                        self.basketSvc.userBasket = resp.model;
                        $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                    }
                });
                self.toastrSvc.displayResponseMessages(resp); //all products added successfully
            }
            else {
                if (resp.model.validProducts.length > 0) {
                    var validproducts = "";
                    for (var i = 0; i < resp.model.validProducts.length; i++) {
                        if (i < resp.model.validProducts.length - 1) {
                            validproducts += resp.model.validProducts[i] + ", ";
                        }
                        else {
                            validproducts += resp.model.validProducts[i];
                        }
                    }
                    var inValidProducts = "";
                    for (var i = 0; i < resp.model.inValidProducts.length; i++) {
                        if (i < resp.model.inValidProducts.length - 1) {
                            inValidProducts += resp.model.inValidProducts[i] + ", ";
                        }
                        else {
                            inValidProducts += resp.model.inValidProducts[i];
                        }
                    }
                    bootbox.confirm("Can not find: " + inValidProducts + " <br/>Do you want continue adding " + validproducts + " to quote?", function (result) {
                        if (result) {
                            self.system.ContinueAdding = true;
                            //Continue adding valid products
                            self.loadingIconSvc.Start(jQuery("#splitSystemConfiguratorTool"));
                            self.productSvc.addSystemToQuote(self.system).then(function (resp) {
                                if (resp.isok) {
                                    //update basket item count
                                    self.basketSvc.getBasket().then(function (resp) {
                                        if (resp.isok) {
                                            //self.userBasket = resp.model;
                                            self.basketSvc.userBasket = resp.model;
                                            $("#quoteItemCount").text(resp.model.quoteItemCount + " items in active quote");
                                        }
                                    });
                                    self.toastrSvc.displayResponseMessages(resp); //products added successfully
                                }
                                self.loadingIconSvc.Stop(jQuery("#splitSystemConfiguratorTool"));
                            });
                        }
                    });
                    //self.toastrSvc.displayResponseMessages(resp);// this shows invalid products
                }
                else {
                    self.toastrSvc.displayResponseMessages(resp); // All products are invalid
                }
            }
            //self.toastrSvc.displayResponseMessages(resp);
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupDetailGridComponent.prototype, "matchupResultDetail", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupDetailGridComponent.prototype, "seer", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupDetailGridComponent.prototype, "indoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupDetailGridComponent.prototype, "outDoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupDetailGridComponent.prototype, "userBasket", void 0);
    SplitMatchupDetailGridComponent = __decorate([
        core_1.Component({
            selector: 'split-matchup-detail-grid',
            templateUrl: 'app/tools/splitSystemConfigurator/split-matchup-detail-grid.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService,
            splitSystemConfigurator_service_1.SplitSystemConfiguratorService])
    ], SplitMatchupDetailGridComponent);
    return SplitMatchupDetailGridComponent;
}());
exports.SplitMatchupDetailGridComponent = SplitMatchupDetailGridComponent;
//# sourceMappingURL=split-matchup-detail-grid.component.js.map