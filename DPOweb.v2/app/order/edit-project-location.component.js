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
var common_service_1 = require("../shared/services/common.service");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var project_service_1 = require("../projects/services/project.service");
var address_service_1 = require("../address/services/address.service");
var EditProjectLocationComponent = /** @class */ (function () {
    function EditProjectLocationComponent(router, route, commonSvc, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc, addressSvc) {
        this.router = router;
        this.route = route;
        this.commonSvc = commonSvc;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
        this.addressSvc = addressSvc;
        this.useSuggestedAddress = false;
    }
    EditProjectLocationComponent.prototype.ngOnInit = function () {
        this._project = jQuery.extend(true, {}, this.project);
    };
    EditProjectLocationComponent.prototype.countryCodeChange = function (event) {
        var _this = this;
        var countryCode = event;
        this.addressSvc.getStatesByCountry(countryCode)
            .subscribe(function (resp) {
            if (resp.isok) {
                var states = resp.model;
                _this.project.shipToAddress.states.items = resp.model.items;
                _this.project.shipToAddress.stateId = null;
            }
        }, function (error) {
            console.log("Error: " + error);
        });
    };
    EditProjectLocationComponent.prototype.stateChange = function (value) {
        if (value != null && value != 0) {
            for (var i = 0; i < this.project.shipToAddress.states.items.length; i++) {
                if (this.project.shipToAddress.states.items[i].value == value) {
                    this.project.shipToAddress.stateName = this.project.shipToAddress.states.items[i].text;
                }
            }
        }
        else {
            this.project.shipToAddress.stateName = null;
        }
    };
    EditProjectLocationComponent.prototype.cancel = function () {
        //this.project = this._project;
        //this.project = jQuery.extend(true, {}, this._project);
        //this.project = Object.assign({}, this._project);
        this.project.shipToName = this._project.shipToName;
        this.project.shipToAddress.addressLine1 = this._project.shipToAddress.addressLine1;
        this.project.shipToAddress.addressLine2 = this._project.shipToAddress.addressLine2;
        this.project.shipToAddress.countryCode = this._project.shipToAddress.countryCode;
        this.project.shipToAddress.location = this._project.shipToAddress.location;
        this.project.shipToAddress.stateId = this._project.shipToAddress.stateId;
        this.project.shipToAddress.postalCode = this._project.shipToAddress.postalCode;
        this.project.squareFootage = this._project.squareFootage;
        this.project.numberOfFloors = this._project.numberOfFloors;
        this.stateChange(this.project.shipToAddress.stateId);
    };
    EditProjectLocationComponent.prototype.reEnterAddress = function () {
    };
    EditProjectLocationComponent.prototype.continue = function () {
        if (this.useSuggestedAddress) {
            this.project.shipToAddress.addressLine1 = this.suggestedAddress.line1;
            this.project.shipToAddress.addressLine2 = this.suggestedAddress.line2;
            this.project.shipToAddress.location = this.suggestedAddress.city;
            this.project.shipToAddress.stateName = this.suggestedAddress.stateProvince;
            this.project.shipToAddress.postalCode = this.suggestedAddress.zipCode;
            var self = this;
            this.commonSvc.getStateIdByStateCode(this.suggestedAddress.stateProvince)
                .subscribe(function (resp) {
                if (resp.isok) {
                    self.project.shipToAddress.stateId = resp.model;
                    self.postProject();
                }
                else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            }, function (err) { return console.log("Error: ", err); });
        }
        else {
            this.postProject();
        }
    };
    EditProjectLocationComponent.prototype.submit = function () {
        var _this = this;
        this.suggestedAddress = null;
        this.loadingIconSvc.Start(jQuery("#editShipToAddressModal"));
        this.projectSvc.postProjectAndVerifyAddress(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                _this.toastrSvc.displayResponseMessagesFadeOut(resp);
                $('#editShipToAddressModal').modal('hide');
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                _this.toastrSvc.displayResponseMessagesFadeOut(resp);
                if (resp.model.error) {
                    if (resp.model.isAddressValid == false) {
                        jQuery("#verifyAddressDialog").modal({ backdrop: 'static', keyboard: false });
                    }
                    else if (resp.model.addresses.length > 0) {
                        _this.suggestedAddress = resp.model.addresses[0];
                        jQuery("#verifyAddressDialog").modal({ backdrop: 'static', keyboard: false });
                    }
                }
            }
        }, function (err) {
            _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
            console.log("Error: ", err);
        });
    };
    EditProjectLocationComponent.prototype.postProject = function () {
        var _this = this;
        this.loadingIconSvc.Start(jQuery("#editShipToAddressModal"));
        this.projectSvc.postProject(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                $('#editShipToAddressModal').modal('hide');
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) {
            _this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
            console.log("Error: ", err);
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], EditProjectLocationComponent.prototype, "project", void 0);
    EditProjectLocationComponent = __decorate([
        core_1.Component({
            selector: 'edit-project-location',
            templateUrl: 'app/order/edit-project-location.component.html',
            styleUrls: ["app/order/edit-project-location.component.css"],
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, common_service_1.CommonService, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http,
            project_service_1.ProjectService, address_service_1.AddressService])
    ], EditProjectLocationComponent);
    return EditProjectLocationComponent;
}());
exports.EditProjectLocationComponent = EditProjectLocationComponent;
;
//# sourceMappingURL=edit-project-location.component.js.map