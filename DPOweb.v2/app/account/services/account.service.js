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
var AccountService = /** @class */ (function () {
    function AccountService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    AccountService.prototype.extractData = function (res) {
        var body = res.json();
        return body || {};
    };
    AccountService.prototype.extractHtml = function (res) {
        return res._body;
    };
    AccountService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Observable_1.Observable.throw(error.statusText);
    };
    //public getUserLoginModel() {
    //    return this.http.get("/api/AccountApi/GetUserLoginModel", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    //}
    AccountService.prototype.getUserLoginModel = function () {
        return this.http.get("/api/AccountApi/GetUserLoginModel", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    AccountService.prototype.logIn = function (body) {
        return this.http.post("/api/AccountApi/Login", body, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    //public getUserRegistrationModel() {
    //    return this.http.get("/api/AccountApi/UserRegistration", { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}
    AccountService.prototype.getUserRegistrationModel = function () {
        return this.http.get("/api/AccountApi/UserRegistration", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    AccountService.prototype.getCurrentUser = function () {
        return this.http.get("/api/User/GetCurrentUser", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    //not working
    //public userRegistration(data: any) {
    //    return this.http.post("/api/AccountApi/UserRegistration", data, { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}
    AccountService.prototype.userRegistration = function (data) {
        return this.http.post("/Account/RegisterUser", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    AccountService.prototype.businessAddressLookup = function (accountId) {
        return this.http.get("/api/AccountApi/BusinessAddressLookup?accountId=" + accountId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    };
    AccountService.prototype.resetBasketQuoteId = function () {
        return this.http.get("/api/AccountApi/ResetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    AccountService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], AccountService);
    return AccountService;
}());
exports.AccountService = AccountService;
//# sourceMappingURL=account.service.js.map