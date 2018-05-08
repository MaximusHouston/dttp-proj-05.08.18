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
var systemConfigurator_service_1 = require("./services/systemConfigurator.service");
var MatchupMasterGridComponent = /** @class */ (function () {
    //public sort: Array<SortDescriptor> = [];
    //public pageSize: number = 15;
    //public skip: number = 0;
    //public testListItems: Array<string> = ["Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball"];
    function MatchupMasterGridComponent(router, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, productSvc, basketSvc, SystemConfiguratorSvc) {
        this.router = router;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.productSvc = productSvc;
        this.basketSvc = basketSvc;
        this.SystemConfiguratorSvc = SystemConfiguratorSvc;
        this.seerCategoriesData = [];
    }
    MatchupMasterGridComponent.prototype.ngOnChanges = function () {
        //console.log("On Change");
        this.loadData();
    };
    MatchupMasterGridComponent.prototype.ngOnInit = function () {
        //console.log("On Init");
        //this.loadData();
    };
    //public dataStateChange({ skip, take, sort }: DataStateChangeEvent): void {
    //    this.skip = skip;
    //    this.pageSize = take;
    //    this.sort = sort;
    //    this.loadData();
    //}
    MatchupMasterGridComponent.prototype.loadData = function () {
        //if (this.seerCategoriesData != undefined) {
        //    this.gridViewData = {
        //        data: this.seerCategoriesData.slice(this.skip, this.skip + this.pageSize),
        //        total: this.seerCategoriesData.length
        //    };
        //}
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
    MatchupMasterGridComponent.prototype.backToSearchPage = function () {
        $('#systemConfigForm').show();
        $('#matchupResultGrid').hide();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], MatchupMasterGridComponent.prototype, "matchupResult", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], MatchupMasterGridComponent.prototype, "indoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], MatchupMasterGridComponent.prototype, "outDoorUnitType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], MatchupMasterGridComponent.prototype, "userBasket", void 0);
    MatchupMasterGridComponent = __decorate([
        core_1.Component({
            selector: 'matchup-master-grid',
            styles: ['/deep/ .k-grid th.k-header, .k-grid-header { background: #5397cf; color: #fff}'],
            templateUrl: 'app/tools/systemConfigurator/matchup-master-grid.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            product_service_1.ProductService, basket_service_1.BasketService,
            systemConfigurator_service_1.SystemConfiguratorService])
    ], MatchupMasterGridComponent);
    return MatchupMasterGridComponent;
}());
exports.MatchupMasterGridComponent = MatchupMasterGridComponent;
//# sourceMappingURL=matchup-master-grid.component.js.map