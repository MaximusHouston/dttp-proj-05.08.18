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
var Observable_1 = require("rxjs/Observable");
require("rxjs/Rx");
var toastr_service_1 = require("../../shared/services/toastr.service");
var OrderService = /** @class */ (function () {
    function OrderService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    OrderService.prototype.extractData = function (res) {
        var body = res.json();
        return body || {};
    };
    OrderService.prototype.extractHtml = function (res) {
        return res._body;
    };
    OrderService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Observable_1.Observable.throw(error.statusText);
    };
    //Test
    OrderService.prototype.getSubmittalOrder = function () {
        return this.http.get("/api/Order/GetSubmittalOrder", { headers: this.headers }).toPromise()
            .then(function (resp) {
            debugger;
            return resp;
        })
            .catch(this.handleError);
    };
    OrderService.prototype.orderForm = function (projectId, quoteId) {
        return this.http.get("/api/Order/OrderForm?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    OrderService.prototype.getSubmittedOrderForm = function (quoteId) {
        return this.http.get("/api/Order/GetSubmittedOrder?quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    OrderService.prototype.postOrder = function (order) {
        return this.http.post("/api/Order/PostOrder", order, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    OrderService.prototype.sendOrderEmail = function (order) {
        return this.http.post("/ProjectDashboard/SendEmailOrderSubmit", order, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    OrderService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], OrderService);
    return OrderService;
}());
exports.OrderService = OrderService;
//# sourceMappingURL=order.service.js.map