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
var DiscountRequestService = /** @class */ (function () {
    function DiscountRequestService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    DiscountRequestService.prototype.extractData = function (res) {
        var body = res.json();
        return body || {};
    };
    DiscountRequestService.prototype.extractHtml = function (res) {
        return res._body;
    };
    DiscountRequestService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Observable_1.Observable.throw(error.statusText);
    };
    DiscountRequestService.prototype.getDiscountRequest = function (discountRequestId, projectId, quoteId) {
        return this.http.get("/api/DiscountRequest/GetDiscountRequest?discountRequestId=" + discountRequestId + "&projectId=" + projectId + "&quoteId=" + quoteId)
            .map(this.extractData)
            .catch(this.handleError);
    };
    DiscountRequestService.prototype.postDiscountRequest = function (discountRequest) {
        //API Controller
        //return this.http.post("/api/DiscountRequest/PostDiscountRequest", discountRequest, { headers: this.headers })
        //    .map(this.extractData)
        //    .catch(this.handleError);
        //MVC Controller
        return this.http.post("/ProjectDashboard/DiscountRequest", discountRequest, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
        //let _headers = new Headers({
        //    'Content-Type': 'multipart/form-data',
        //    'Accept': 'application/json'
        //});
        //return this.http.post("/api/DiscountRequest/PostDiscountRequest", discountRequest, { headers: _headers })
        //    .map(this.extractData)
        //    .catch(this.handleError);
    };
    DiscountRequestService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], DiscountRequestService);
    return DiscountRequestService;
}());
exports.DiscountRequestService = DiscountRequestService;
//# sourceMappingURL=discountRequest.service.js.map