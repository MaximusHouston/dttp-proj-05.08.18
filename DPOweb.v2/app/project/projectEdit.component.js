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
var forms_1 = require("@angular/forms");
require("rxjs/Rx");
var common_service_1 = require("../shared/services/common.service");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var project_service_1 = require("../projects/services/project.service");
var address_service_1 = require("../address/services/address.service");
var ProjectEditComponent = /** @class */ (function () {
    function ProjectEditComponent(router, route, commonSvc, toastrSvc, loadingIconSvc, userSvc, enums, systemAccessEnum, http, projectSvc, addressSvc) {
        var _this = this;
        this.router = router;
        this.route = route;
        this.commonSvc = commonSvc;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.enums = enums;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.projectSvc = projectSvc;
        this.addressSvc = addressSvc;
        this.formIsValid = false;
        this.canViewPipelineData = false;
        this.canEditPipelineData = false;
        this.useSuggestedAddress = false;
        //public ignoreAddressValidation: boolean = false;
        this.defaultItem = { text: "Select ...", value: null };
        router.events
            .filter(function (event) { return event instanceof router_1.NavigationEnd; })
            .subscribe(function (e) {
            _this.previousUrl = e.url;
        });
        this.action = this.route.snapshot.url[0].path;
        this.project = this.route.snapshot.data['projectModel'].model;
        this.user = this.route.snapshot.data['currentUser'].model;
        // this.project.projectDate = new Date(Date.parse(this.project.projectDate));
        if (this.action == "projectCreate") {
            this.projectDate = new Date();
            this.project.bidDate = null;
            this.project.estimatedClose = null;
            this.project.estimatedDelivery = null;
            this.project.projectStatusTypeId = 1;
        }
        else {
            this.projectDate = new Date(Date.parse(this.project.projectDate));
            this.project.bidDate = new Date(Date.parse(this.project.bidDate));
            this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
            this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));
        }
    }
    ProjectEditComponent.prototype.ngOnInit = function () {
        this.validateForm();
        this.canViewPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewPipelineData);
        this.canEditPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditPipelineData);
    };
    ProjectEditComponent.prototype.ngDoCheck = function () {
        //if (this.projectEditForm.invalid) {
        //    $("#projectEditSubmitBtn").attr("disabled", "disabled");
        //} else {
        //    $("#projectEditSubmitBtn").removeAttr("disabled");
        //}
        console.log("ProjectEditForm.invalid: " + this.projectEditForm.invalid);
        if (this.formIsValid == false) {
            $("#projectEditSubmitBtn").attr("disabled", "disabled");
        }
        else {
            $("#projectEditSubmitBtn").removeAttr("disabled");
        }
    };
    ProjectEditComponent.prototype.validateForm = function () {
        if (this.projectEditForm.invalid) {
            this.formIsValid = false;
        }
        else {
            this.formIsValid = true;
        }
    };
    ProjectEditComponent.prototype.projectNameChange = function (event) {
        this.project.name = event;
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.constructionTypeChange = function (event) {
        //this.project.constructionTypeId = event.value;
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.projectStatusTypeChange = function (event) {
        //this.project.projectStatusTypeId = event.value;
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.projectTypeChange = function (event) {
        //this.project.projectTypeId = event.value;
        this.setDefaultDates();
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.projectOpenStatusTypeChange = function (event) {
        //this.project.projectOpenStatusTypeId = event.value;
        this.setDefaultDates();
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.verticalMarketTypeChange = function (event) {
        //this.project.verticalMarketTypeId = event.value;
        //this.validateForm();
        setTimeout(this.validateForm.bind(this), 200);
    };
    ProjectEditComponent.prototype.countryCodeChange = function (event) {
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
    // If project type && open status selected and no dates have been set 
    ProjectEditComponent.prototype.setDefaultDates = function () {
        if (this.project.projectTypeId && this.project.projectOpenStatusTypeId && !this.project.bidDate && !this.project.estimatedClose && !this.project.estimatedDelivery) {
            if (this.project.projectTypeId == "6") {
                // Design build 	
                //  1. Bid: should be same month as registration date
                //  2. Close: Add 60 days to bid date	
                //  3. Delivery: Add 30 days to estimated close
                //  4. Many time the customer marks D/B as budget or design and they should be all bidding 
                this.project.bidDate = new Date(this.projectDate);
                this.project.estimatedClose = new Date(this.project.bidDate);
                this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);
                this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);
            }
            else {
                switch (this.project.projectOpenStatusId) {
                    case "1": // Budget
                    case "2":// Design
                        // 1. Bid: Add 9 months to reg date	
                        // 2. Close: Add 60 days to bid date
                        // 3. Delivery: Add 30 days to close date	
                        this.project.bidDate = new Date(this.projectDate);
                        this.project.bidDate.setDate(this.project.bidDate.getDate() + (30 * 9));
                        this.project.estimatedClose = new Date(this.project.bidDate);
                        this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);
                        this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                        this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);
                        break;
                    case "3": // Quote
                    default:
                        // 1. Bid: Quote 
                        // 2. Close: Add 60 days 
                        // 3. Delivery: Add 30 days
                        this.project.bidDate = new Date(this.projectDate);
                        this.project.estimatedClose = new Date(this.project.bidDate);
                        this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);
                        this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                        this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);
                        break;
                }
            }
        }
    };
    ProjectEditComponent.prototype.cancel = function () {
        if (this.action == "projectCreate") {
            this.router.navigateByUrl("/projects");
        }
        else if (this.action == "projectEdit") {
            this.router.navigateByUrl("/project/" + this.project.projectId);
        }
    };
    ProjectEditComponent.prototype.submit = function () {
        var _this = this;
        this.suggestedAddress = null;
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.postProjectAndVerifyAddress(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.project.projectId = resp.model.projectId;
                if (_this.action == "projectCreate") {
                    _this.router.navigateByUrl("/quoteCreate/" + _this.project.projectId);
                }
                else if (_this.action == "projectEdit") {
                    _this.router.navigateByUrl("/project/" + _this.project.projectId);
                }
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessagesFadeOut(resp);
                if (resp.model.error) {
                    jQuery("#shipToAddressLink").click();
                    if (resp.model.isAddressValid == false) {
                        jQuery("#shippingAddressDialog").modal({ backdrop: 'static', keyboard: false });
                    }
                    else if (resp.model.addresses.length > 0) {
                        _this.suggestedAddress = resp.model.addresses[0];
                        jQuery("#shippingAddressDialog").modal({ backdrop: 'static', keyboard: false });
                    }
                }
            }
        }, function (err) {
            _this.loadingIconSvc.Stop(jQuery("#content"));
            console.log("Error: ", err);
        });
    };
    //public onTabSelect(e: any) {
    //    console.log(e);
    //}
    //get diagnostic() { return JSON.stringify(this.project); }
    ProjectEditComponent.prototype.reEnterAddress = function () {
    };
    ProjectEditComponent.prototype.continue = function () {
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
    ProjectEditComponent.prototype.postProject = function () {
        var _this = this;
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.postProject(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.project.projectId = resp.model.projectId;
                if (_this.action == "projectCreate") {
                    _this.router.navigateByUrl("/quoteCreate/" + _this.project.projectId);
                }
                else if (_this.action == "projectEdit") {
                    _this.router.navigateByUrl("/project/" + _this.project.projectId);
                }
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) {
            _this.loadingIconSvc.Stop(jQuery("#content"));
            console.log("Error: ", err);
        });
    };
    __decorate([
        core_1.ViewChild('projectEditForm'),
        __metadata("design:type", forms_1.NgForm)
    ], ProjectEditComponent.prototype, "projectEditForm", void 0);
    ProjectEditComponent = __decorate([
        core_1.Component({
            selector: 'project-edit',
            templateUrl: 'app/project/projectEdit.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, common_service_1.CommonService,
            toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, enums_1.Enums, systemAccessEnum_1.SystemAccessEnum, http_1.Http,
            project_service_1.ProjectService, address_service_1.AddressService])
    ], ProjectEditComponent);
    return ProjectEditComponent;
}());
exports.ProjectEditComponent = ProjectEditComponent;
;
//# sourceMappingURL=projectEdit.component.js.map