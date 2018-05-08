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
var discountRequest_service_1 = require("./services/discountRequest.service");
var DiscountRequestComponent = /** @class */ (function () {
    function DiscountRequestComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc, discountRequestSvc) {
        var _this = this;
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
        this.discountRequestSvc = discountRequestSvc;
        this.user = this.route.snapshot.data['currentUser'].model;
        this.quoteId = this.route.snapshot.paramMap.get('quoteId');
        this.projectId = this.route.snapshot.paramMap.get('projectId');
        this.discountRequestSvc.getDiscountRequest(0, this.projectId, this.quoteId).subscribe(function (resp) {
            if (resp.isok) {
                _this.discountRequest = resp.model;
                _this.discountRequest.requestedCommission = _this.discountRequest.standardTotals.totalCommissionPercentage;
                _this.calculateStandardGrossProfit();
                _this.calculateRevisedGrossProfit();
                _this.discountRequest.orderPlannedFor = null;
                _this.discountRequest.project.estimatedDelivery = new Date(Date.parse(_this.discountRequest.project.estimatedDelivery));
            }
        }, function (error) {
            console.log("Error: " + error);
        });
    }
    DiscountRequestComponent.prototype.ngOnInit = function () {
    };
    DiscountRequestComponent.prototype.hasCompetitorPriceChange = function (event) {
        if (event == false) {
            this.discountRequest.competitorPrice = null;
        }
    };
    DiscountRequestComponent.prototype.hasCompetitorQuoteChange = function (event) {
    };
    DiscountRequestComponent.prototype.hasCompetitorLineComparsionChange = function (event) {
    };
    DiscountRequestComponent.prototype.selectCompetitorQuoteFile = function (e) {
        //this.competitorQuoteFiles = e.files;
        //this.discountRequest.competitorQuoteFile = e.files[0];
        this.discountRequest.competitorQuoteFileName = e.files[0].name;
    };
    DiscountRequestComponent.prototype.selectLineComparsionFile = function (e) {
        //this.discountRequest.competitorLineComparsionFile = e.files[0];
        this.discountRequest.competitorLineComparsionFileName = e.files[0].name;
    };
    DiscountRequestComponent.prototype.uploadEventHandler = function (e) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.discountRequest.quoteId,
        };
    };
    DiscountRequestComponent.prototype.successEventHandler = function (e) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
        }
    };
    DiscountRequestComponent.prototype.errorEventHandler = function (e) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    };
    //competitorQuoteFileChange(e: any) {
    //    //var files = e.srcElement.files;
    //    this.discountRequest.competitorQuoteFile = e.srcElement.files[0];
    //    //let formData: FormData = new FormData();
    //    //formData.append('competitorQuoteFile', e.srcElement.files[0], e.srcElement.files[0].name);
    //}
    //public test(event: any) {
    //    this.discountRequest.requestedDiscountVRV = event / 100;
    //}
    DiscountRequestComponent.prototype.startupCostChange = function () {
        this.calculateRevisedTotalSell();
    };
    //Kendo numeric input
    //calculateDiscountAmountVRV(event: any) {
    //    //update Net Material 
    //    this.discountRequest.approvedTotals.netMaterialValueVRV = this.discountRequest.approvedTotals.totalNetVRV * (1 - this.discountRequest.requestedDiscountVRV);
    //    //update Net Multiplier
    //    this.discountRequest.approvedTotals.netMultiplierVRV = this.discountRequest.approvedTotals.netMaterialValueVRV / this.discountRequest.approvedTotals.totalListVRV;
    //    //show/update Discount Ammount
    //    this.discountRequest.approvedTotals.totalDiscountAmountVRV = this.discountRequest.approvedTotals.totalNetVRV - this.discountRequest.approvedTotals.netMaterialValueVRV;
    //    this.calculateTotalDiscount();
    //}
    //HTML numeric input
    DiscountRequestComponent.prototype.calculateDiscountAmountVRV = function (value) {
        this.discountRequest.requestedDiscountVRV = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueVRV = this.discountRequest.approvedTotals.totalNetVRV * (1 - this.discountRequest.requestedDiscountVRV);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierVRV = this.discountRequest.approvedTotals.netMaterialValueVRV / this.discountRequest.approvedTotals.totalListVRV;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountVRV = this.discountRequest.approvedTotals.totalNetVRV - this.discountRequest.approvedTotals.netMaterialValueVRV;
        this.calculateTotalDiscount();
    };
    DiscountRequestComponent.prototype.calculateDiscountAmountSplit = function (value) {
        this.discountRequest.requestedDiscountSplit = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueSplit = this.discountRequest.approvedTotals.totalNetSplit * (1 - this.discountRequest.requestedDiscountSplit);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierSplit = this.discountRequest.approvedTotals.netMaterialValueSplit / this.discountRequest.approvedTotals.totalListSplit;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountSplit = this.discountRequest.approvedTotals.totalNetSplit - this.discountRequest.approvedTotals.netMaterialValueSplit;
        this.calculateTotalDiscount();
    };
    DiscountRequestComponent.prototype.calculateDiscountAmountUnitary = function (value) {
        this.discountRequest.requestedDiscountUnitary = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueUnitary = this.discountRequest.approvedTotals.totalNetUnitary * (1 - this.discountRequest.requestedDiscountUnitary);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierUnitary = this.discountRequest.approvedTotals.netMaterialValueUnitary / this.discountRequest.approvedTotals.totalListUnitary;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountUnitary = this.discountRequest.approvedTotals.totalNetUnitary - this.discountRequest.approvedTotals.netMaterialValueUnitary;
        this.calculateTotalDiscount();
    };
    DiscountRequestComponent.prototype.calculateDiscountAmountLCPackage = function (value) {
        this.discountRequest.requestedDiscountLCPackage = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueLCPackage = this.discountRequest.approvedTotals.totalNetLCPackage * (1 - this.discountRequest.requestedDiscountLCPackage);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierLCPackage = this.discountRequest.approvedTotals.netMaterialValueLCPackage / this.discountRequest.approvedTotals.totalListLCPackage;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountLCPackage = this.discountRequest.approvedTotals.totalNetLCPackage - this.discountRequest.approvedTotals.netMaterialValueLCPackage;
        this.calculateTotalDiscount();
    };
    DiscountRequestComponent.prototype.calculateTotalDiscount = function () {
        this.discountRequest.approvedTotals.totalDiscountAmount =
            this.discountRequest.approvedTotals.totalDiscountAmountVRV +
                this.discountRequest.approvedTotals.totalDiscountAmountSplit +
                this.discountRequest.approvedTotals.totalDiscountAmountUnitary +
                this.discountRequest.approvedTotals.totalDiscountAmountLCPackage;
        this.discountRequest.approvedTotals.netMaterialValue =
            this.discountRequest.approvedTotals.netMaterialValueVRV +
                this.discountRequest.approvedTotals.netMaterialValueSplit +
                this.discountRequest.approvedTotals.netMaterialValueUnitary +
                this.discountRequest.approvedTotals.netMaterialValueLCPackage;
        this.discountRequest.approvedTotals.netMultiplier = this.discountRequest.approvedTotals.netMaterialValue / this.discountRequest.approvedTotals.totalList;
        this.discountRequest.requestedDiscount = this.discountRequest.approvedTotals.totalDiscountAmount / this.discountRequest.standardTotals.totalNet;
        this.calculateRevisedTotalSell();
    };
    DiscountRequestComponent.prototype.calculateStandardGrossProfit = function () {
        //this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.standardTotals.totalCommissionPercentage * this.discountRequest.standardTotals.totalNet;
        this.discountRequest.standardTotals.totalCommissionAmount = this.discountRequest.standardTotals.totalCommissionPercentage * this.discountRequest.standardTotals.totalNet;
    };
    DiscountRequestComponent.prototype.calculateRevisedGrossProfit = function () {
        this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.requestedCommission * this.discountRequest.approvedTotals.netMaterialValue;
        this.calculateRevisedTotalSell();
    };
    DiscountRequestComponent.prototype.recalculateRevisedGrossProfit = function (value) {
        this.discountRequest.requestedCommission = value / 100;
        this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.requestedCommission * this.discountRequest.approvedTotals.netMaterialValue;
        this.calculateRevisedTotalSell();
    };
    DiscountRequestComponent.prototype.calculateRevisedTotalSell = function () {
        this.discountRequest.approvedTotals.totalSell =
            this.discountRequest.quote.totalFreight +
                this.discountRequest.startUpCosts +
                this.discountRequest.approvedTotals.totalCommissionAmount +
                this.discountRequest.approvedTotals.netMaterialValue;
    };
    DiscountRequestComponent.prototype.submit = function () {
        this.discountRequestSvc.postDiscountRequest(this.discountRequest).subscribe();
    };
    //====This is to fix kendo date picker view jump on open===
    DiscountRequestComponent.prototype.datePickerOpen = function () {
        setTimeout(this.jumpToDatePicker.bind(this), 10); // wait 0.01 sec
    };
    DiscountRequestComponent.prototype.jumpToDatePicker = function () {
        document.getElementById("orderIssueDate").scrollIntoView();
    };
    DiscountRequestComponent = __decorate([
        core_1.Component({
            selector: 'discount-request',
            templateUrl: 'app/discountRequest/discount-request.component.html',
            styleUrls: ["app/discountRequest/discount-request.component.css"]
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService,
            loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http,
            project_service_1.ProjectService, discountRequest_service_1.DiscountRequestService])
    ], DiscountRequestComponent);
    return DiscountRequestComponent;
}());
exports.DiscountRequestComponent = DiscountRequestComponent;
//# sourceMappingURL=discount-request.component.js.map