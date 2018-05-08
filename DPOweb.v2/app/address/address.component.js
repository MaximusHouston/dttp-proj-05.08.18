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
var project_service_1 = require("../projects/services/project.service");
var address_service_1 = require("./services/address.service");
var AddressComponent = /** @class */ (function () {
    function AddressComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, http, projectSvc, addressSvc) {
        //this.project = this.route.snapshot.data['projectModel'].model;
        //this.project.projectDate = new Date(Date.parse(this.project.projectDate));
        //this.project.bidDate = null;
        //this.project.estimatedClose = null;
        //this.project.estimatedDelivery = null;
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.projectSvc = projectSvc;
        this.addressSvc = addressSvc;
        //public selectedState: { text: string, value: number } = { text: null, value: null };
        this.defaultItem = { text: "Select ...", value: null };
    }
    AddressComponent.prototype.ngOnInit = function () {
        var type = this.addressType;
        //this.selectedState.text = this.address.stateName;
        //this.selectedState.value = this.address.stateId;
    };
    AddressComponent.prototype.countryCodeChange = function (event) {
        var _this = this;
        var countryCode = event;
        this.addressSvc.getStatesByCountry(countryCode)
            .subscribe(function (resp) {
            if (resp.isok) {
                var states = resp.model;
                _this.address.states.items = resp.model.items;
                _this.address.stateId = null;
            }
        }, function (error) {
            console.log("Error: " + error);
        });
    };
    AddressComponent.prototype.stateChange = function (value) {
        for (var i = 0; i < this.address.states.items.length; i++) {
            if (this.address.states.items[i].value == value) {
                this.address.stateName = this.address.states.items[i].text;
            }
        }
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], AddressComponent.prototype, "project", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], AddressComponent.prototype, "address", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], AddressComponent.prototype, "addressType", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], AddressComponent.prototype, "projectEditForm", void 0);
    AddressComponent = __decorate([
        core_1.Component({
            selector: 'address',
            templateUrl: 'app/address/address.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, http_1.Http,
            project_service_1.ProjectService, address_service_1.AddressService])
    ], AddressComponent);
    return AddressComponent;
}());
exports.AddressComponent = AddressComponent;
;
//# sourceMappingURL=address.component.js.map