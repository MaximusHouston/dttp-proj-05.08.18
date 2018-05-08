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
var order_service_1 = require("./services/order.service");
var OrderFormComponent = /** @class */ (function () {
    function OrderFormComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc, orderSvc) {
        //this.activeTab = this.route.snapshot.url[0].path;
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
        this.orderSvc = orderSvc;
        this.user = this.route.snapshot.data['currentUser'].model;
        this.order = this.route.snapshot.data['orderFormModel'].model;
        this.recordState = this.route.snapshot.paramMap.get('recordState');
        if (this.recordState == "new") {
            if (this.order.hasConfiguredModel) {
                this.releaseDateMin = new Date();
                this.releaseDateMin.setDate((new Date()).getDate() + 28);
                this.order.orderReleaseDate = this.releaseDateMin;
                //this.order.orderReleaseDate = new Date();
                //this.order.orderReleaseDate.setDate((new Date()).getDate() + 28);
            }
            else {
                this.order.orderReleaseDate = null;
            }
        }
        else {
            this.order.orderReleaseDate = new Date(Date.parse(this.order.orderReleaseDate));
        }
        //this.order.orderReleaseDate = new Date(Date.parse(this.order.orderReleaseDate)); // 1/1/1
        //this.order.orderReleaseDate = new Date();// current date
        this.poUploadUrl = "/api/Order/UploadPOAttachment";
    }
    OrderFormComponent.prototype.ngOnInit = function () {
        //this.recordState = this.route.snapshot.paramMap.get('recordState');
    };
    OrderFormComponent.prototype.selectEventHandler = function (e) {
        //console.log("File selected");
        this.order.poAttachmentFileName = e.files[0].name;
    };
    OrderFormComponent.prototype.uploadEventHandler = function (e) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.order.quoteId,
        };
    };
    OrderFormComponent.prototype.successEventHandler = function (e) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
            //this.toastrSvc.Success("Quote '" + this.quote.title + "' has been updated.");
            //this.reloadDataEvent.emit();
            //$('button.close[data-dismiss=modal]').click();
        }
    };
    OrderFormComponent.prototype.errorEventHandler = function (e) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    };
    //====This is to fix kendo date picker view jump on open===
    OrderFormComponent.prototype.datePickerOpen = function () {
        setTimeout(this.jumpToDatePicker.bind(this), 50); // wait 0.05 sec
    };
    OrderFormComponent.prototype.jumpToDatePicker = function () {
        //location.href = "#orderReleaseDate";
        //this.datepicker.open();
        document.getElementById("orderReleaseDate").scrollIntoView();
    };
    //======================================================
    OrderFormComponent.prototype.submit = function () {
        this.order.shipToName = this.order.project.shipToName;
        var self = this;
        bootbox.confirm("<p>Are you sure you want to submit Order? <br/>No further changes will be available on this project after it has been submitted.</p>", function (result) {
            if (result) {
                self.loadingIconSvc.Start(jQuery("#main-container"));
                //Post Order
                self.orderSvc.postOrder(self.order)
                    .subscribe(function (resp) {
                    if (resp.isok) {
                        self.loadingIconSvc.Stop(jQuery("#main-container"));
                        //Send order email notification
                        self.orderSvc.sendOrderEmail(self.order).subscribe();
                        if (self.order.hasConfiguredModel) {
                            bootbox.alert("<p>Some message for LC configured Products</p>", function () {
                                self.router.navigateByUrl("/quote/" + self.order.quoteId + "/existingRecord");
                            });
                        }
                        else {
                            bootbox.alert("<p>Thank you for submitting the order. Your Daikin Customer Service Representative will review the order and get back to you within 24 hours.<br/> <br/>To cancel the order, please contact your Daikin Customer Service Representative.</p>", function () {
                                self.router.navigateByUrl("/quote/" + self.order.quoteId + "/existingRecord");
                            });
                        }
                    }
                    else {
                        self.loadingIconSvc.Stop(jQuery("#main-container"));
                        self.toastrSvc.displayResponseMessages(resp);
                    }
                }, function (err) {
                    self.loadingIconSvc.Stop(jQuery("#main-container"));
                    console.log("Error: ", err);
                });
            }
        });
    };
    //validate() {
    //    this.validateProjectInfo();
    //    this.validateOrderDetails();
    //}
    //validateProjectInfo() {
    //}
    //validateOrderDetails() {
    //    if (this.order.orderReleaseDate == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.poAttachment == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.order.poNumber == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.order.comments == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //}
    OrderFormComponent.prototype.stateChange = function (value) {
        for (var i = 0; i < this.order.project.shipToAddress.states.items.length; i++) {
            if (this.order.project.shipToAddress.states.items[i].value == value) {
                this.order.project.shipToAddress.stateName = this.order.project.shipToAddress.states.items[i].text;
            }
        }
    };
    OrderFormComponent = __decorate([
        core_1.Component({
            selector: 'order-form',
            templateUrl: 'app/order/order-form.component.html',
            styleUrls: ["app/order/order-form.component.css"],
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http,
            project_service_1.ProjectService, order_service_1.OrderService])
    ], OrderFormComponent);
    return OrderFormComponent;
}());
exports.OrderFormComponent = OrderFormComponent;
;
//# sourceMappingURL=order-form.component.js.map