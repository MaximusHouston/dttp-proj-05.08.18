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
var toastr_service_1 = require("../../shared/services/toastr.service");
var loadingIcon_service_1 = require("../../shared/services/loadingIcon.service");
var user_service_1 = require("../../shared/services/user.service");
var systemAccessEnum_1 = require("../../shared/services/systemAccessEnum");
var product_service_1 = require("../../products/services/product.service");
var basket_service_1 = require("../../basket/services/basket.service");
var splitSystemConfigurator_service_1 = require("./services/splitSystemConfigurator.service");
var SplitMatchupMasterGridComponent = /** @class */ (function () {
    function SplitMatchupMasterGridComponent(router, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc, SplitSystemConfiguratorSvc) {
        this.router = router;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.SplitSystemConfiguratorSvc = SplitSystemConfiguratorSvc;
        this.seerCategoriesData = [];
    }
    SplitMatchupMasterGridComponent.prototype.ngOnChanges = function () {
        //console.log("On Change");
        this.loadData();
    };
    SplitMatchupMasterGridComponent.prototype.ngOnInit = function () {
        //console.log("On Init");
        //this.loadData();
    };
    //public dataStateChange({ skip, take, sort }: DataStateChangeEvent): void {
    //    this.skip = skip;
    //    this.pageSize = take;
    //    this.sort = sort;
    //    this.loadData();
    //}
    SplitMatchupMasterGridComponent.prototype.loadData = function () {
        this.seerCategoriesData = []; // clear old data
        for (var key in this.matchupResult) {
            if (!this.matchupResult.hasOwnProperty(key))
                continue;
            if (this.matchupResult[key].length > 0) {
                var obj = {
                    "seer": key,
                    "numberOfItem": this.matchupResult[key].length
                };
                this.seerCategoriesData.push(obj);
            }
        }
        if (this.seerCategoriesData != undefined) {
            this.gridViewData = {
                data: this.seerCategoriesData,
                total: this.seerCategoriesData.length
            };
        }
    };
    SplitMatchupMasterGridComponent.prototype.backToSearchPage = function () {
        $('#systemConfigForm').show();
        $('#splitMatchupResultGrid').hide();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupMasterGridComponent.prototype, "matchupResult", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupMasterGridComponent.prototype, "indoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupMasterGridComponent.prototype, "outDoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], SplitMatchupMasterGridComponent.prototype, "userBasket", void 0);
    SplitMatchupMasterGridComponent = __decorate([
        core_1.Component({
            selector: 'split-matchup-master-grid',
            styles: ['/deep/ .k-grid th.k-header, .k-grid-header { background: #5397cf; color: #fff}'],
            templateUrl: 'app/tools/splitSystemConfigurator/split-matchup-master-grid.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService,
            splitSystemConfigurator_service_1.SplitSystemConfiguratorService])
    ], SplitMatchupMasterGridComponent);
    return SplitMatchupMasterGridComponent;
}());
exports.SplitMatchupMasterGridComponent = SplitMatchupMasterGridComponent;
//# sourceMappingURL=split-matchup-master-grid.component.js.map