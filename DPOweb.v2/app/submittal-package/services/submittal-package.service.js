"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
var toastr_service_1 = require("../../shared/services/toastr.service");
var BaseErrorHandler_component_1 = require("../../shared/common/BaseErrorHandler.component");
var SubmittalPackageService = /** @class */ (function (_super) {
    __extends(SubmittalPackageService, _super);
    function SubmittalPackageService(http, toastrService) {
        var _this = _super.call(this, toastrService) || this;
        _this.http = http;
        _this.toastrService = toastrService;
        _this.headers = new http_1.Headers({ 'Content-Type': 'application/json' });
        _this.options = new http_1.RequestOptions({ headers: _this.headers });
        console.log('Submittal Package Service Initialized...');
        return _this;
    }
    SubmittalPackageService.prototype.getQuotePackage = function (quoteId) {
        //let body = model;
        //let options = new RequestOptions({ headers: headers, withCredentials: false });
        var data = this.http.get("/api/SubmittalPackage/GetQuotePackage?quoteId=" + quoteId)
            .map(this.extractData)
            .catch(this.handleError);
        return data;
    };
    SubmittalPackageService.prototype.createQuotePackage = function (model) {
        this.apiUrl = "/api/SubmittalPackage/QuotePackageCreate";
        return this.http.post(this.apiUrl, model, this.options)
            .map(function (res) {
            return res.json();
        })
            .catch(this.handleError);
    };
    SubmittalPackageService.prototype.extractData = function (res) {
        var body = res.json().model;
        return body || null;
    };
    SubmittalPackageService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http, toastr_service_1.ToastrService])
    ], SubmittalPackageService);
    return SubmittalPackageService;
}(BaseErrorHandler_component_1.BaseErrorHandler));
exports.SubmittalPackageService = SubmittalPackageService;
//# sourceMappingURL=submittal-package.service.js.map