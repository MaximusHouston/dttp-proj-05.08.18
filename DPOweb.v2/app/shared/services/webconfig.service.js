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
require("rxjs/Rx");
var toastr_service_1 = require("./toastr.service");
var WebConfigService = /** @class */ (function () {
    function WebConfigService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    WebConfigService.prototype.extractData = function (res) {
        var resp = res.json();
        return resp || {};
    };
    WebConfigService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    };
    WebConfigService.prototype.getWebConfig = function () {
        return this.http.get("/v2/webconfig.v2.json", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    WebConfigService.prototype.getLCSTApiToken = function () {
        return this.http.get("/api/Product/GetLCSTApiToken", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    WebConfigService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], WebConfigService);
    return WebConfigService;
}());
exports.WebConfigService = WebConfigService;
//# sourceMappingURL=webconfig.service.js.map