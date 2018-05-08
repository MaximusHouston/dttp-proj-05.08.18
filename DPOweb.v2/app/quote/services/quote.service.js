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
var QuoteService = /** @class */ (function () {
    function QuoteService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    QuoteService.prototype.extractData = function (res) {
        var body = res.json();
        return body || {};
    };
    QuoteService.prototype.extractHtml = function (res) {
        return res._body;
    };
    QuoteService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        //console.error(error); // log to console instead
        console.log(error);
        this.toastrSvc.Error(error.statusText);
        return Observable_1.Observable.throw(error.statusText);
    };
    QuoteService.prototype.setBasketQuoteId = function (quoteId) {
        return this.http.get("/api/Quote/SetBasketQuoteId?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.getQuoteModel = function (projectId, quoteId) {
        return this.http.get("/api/Quote/GetQuoteModel?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.postQuote = function (quote) {
        return this.http.post("/api/Quote/PostQuote", quote, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.getQuote = function (projectId, quoteId) {
        return this.http.get("/api/Quote/GetQuoteModel?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    //Order Form
    QuoteService.prototype.getQuoteItems = function (quoteId) {
        return this.http.get("/api/Quote/GetQuoteItems?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    //Quote Page
    QuoteService.prototype.getQuoteItemsModel = function (quoteId) {
        return this.http.get("/api/Quote/GetQuoteItemsModel?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.getOptionItems = function (quoteItemId) {
        return this.http.get("/api/Quote/GetOptionItems?quoteItemId=" + quoteItemId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.setQuoteActive = function (data) {
        return this.http.post("/api/Quote/QuoteSetActive", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.deleteQuote = function (data) {
        return this.http.post("/api/Quote/deleteQuote", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.undeleteQuote = function (data) {
        return this.http.post("/api/Quote/undeleteQuote", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    //TODO: figure out why this is not working
    //public getQuoteItems(quoteId: any): Observable<any> {
    //    debugger
    //    return this.http.get("/api/Quote/GetQuoteItemsModel?quoteId=" + quoteId, { headers: this.headers })
    //        .map(this.extractData)
    //        .catch(this.handleError);
    //}
    QuoteService.prototype.adjustQuoteItems = function (data) {
        return this.http.post("/api/Quote/AdjustQuoteItems", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService.prototype.quoteRecalculate = function (data) {
        return this.http.post("/api/Quote/QuoteRecalculate", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    QuoteService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], QuoteService);
    return QuoteService;
}());
exports.QuoteService = QuoteService;
//# sourceMappingURL=quote.service.js.map