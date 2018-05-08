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
var address_service_1 = require("../address/services/address.service");
var EditCustomerAddressComponent = /** @class */ (function () {
    function EditCustomerAddressComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc, addressSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
        this.addressSvc = addressSvc;
    }
    EditCustomerAddressComponent.prototype.ngOnInit = function () {
        this._project = jQuery.extend(true, {}, this.project);
    };
    EditCustomerAddressComponent.prototype.countryCodeChange = function (event) {
        var _this = this;
        var countryCode = event;
        this.addressSvc.getStatesByCountry(countryCode)
            .subscribe(function (resp) {
            if (resp.isok) {
                var states = resp.model;
                _this.project.customerAddress.states.items = resp.model.items;
                _this.project.customerAddress.stateId = null;
            }
        }, function (error) {
            console.log("Error: " + error);
        });
    };
    EditCustomerAddressComponent.prototype.stateChange = function (value) {
        if (value != null && value != 0) {
            for (var i = 0; i < this.project.customerAddress.states.items.length; i++) {
                if (this.project.customerAddress.states.items[i].value == value) {
                    this.project.customerAddress.stateName = this.project.customerAddress.states.items[i].text;
                }
            }
        }
        else {
            this.project.customerAddress.stateName = null;
        }
    };
    EditCustomerAddressComponent.prototype.cancel = function () {
        this.project.dealerContractorName = this._project.dealerContractorName;
        this.project.customerName = this._project.customerName;
        this.project.customerAddress.addressLine1 = this._project.customerAddress.addressLine1;
        this.project.customerAddress.addressLine2 = this._project.customerAddress.addressLine2;
        this.project.customerAddress.countryCode = this._project.customerAddress.countryCode;
        this.project.customerAddress.location = this._project.customerAddress.location;
        this.project.customerAddress.stateId = this._project.customerAddress.stateId;
        this.project.customerAddress.postalCode = this._project.customerAddress.postalCode;
        this.stateChange(this.project.customerAddress.stateId);
    };
    EditCustomerAddressComponent.prototype.submit = function () {
        var _this = this;
        this.loadingIconSvc.Start(jQuery("#editCustomerAddressModal"));
        this.projectSvc.postProject(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
                _this.toastrSvc.displayResponseMessagesFadeOut(resp);
                $('#editCustomerAddressModal').modal('hide');
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
                _this.toastrSvc.displayResponseMessagesFadeOut(resp);
                $('#editCustomerAddressModal').modal('hide');
            }
        }, function (err) {
            _this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
            $('#editCustomerAddressModal').modal('hide');
            console.log("Error: ", err);
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], EditCustomerAddressComponent.prototype, "project", void 0);
    EditCustomerAddressComponent = __decorate([
        core_1.Component({
            selector: 'edit-customer-address',
            templateUrl: 'app/order/edit-customer-address.component.html'
            //styleUrls: ["app/order/edit-project-location.component.css"],
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http,
            project_service_1.ProjectService, address_service_1.AddressService])
    ], EditCustomerAddressComponent);
    return EditCustomerAddressComponent;
}());
exports.EditCustomerAddressComponent = EditCustomerAddressComponent;
;
//# sourceMappingURL=edit-customer-address.component.js.map