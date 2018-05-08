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
var http_1 = require("@angular/http");
var router_1 = require("@angular/router");
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var project_service_1 = require("../projects/services/project.service");
var AddressEditComponent = /** @class */ (function () {
    function AddressEditComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
    }
    AddressEditComponent.prototype.ngOnInit = function () {
        //this.quoteSvc.getQuoteItems(this.quoteId).then((resp: any) => {
        //    if (resp.isok) {
        //        this.quoteItems = resp.model;
        //    }
        //}).catch(error => {
        //    console.log(error);
        //});
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], AddressEditComponent.prototype, "address", void 0);
    AddressEditComponent = __decorate([
        core_1.Component({
            selector: 'address-edit',
            templateUrl: 'app/address/address-edit.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http,
            project_service_1.ProjectService])
    ], AddressEditComponent);
    return AddressEditComponent;
}());
exports.AddressEditComponent = AddressEditComponent;
//# sourceMappingURL=address-edit.component.js.map