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
var Rx_1 = require("rxjs/Rx");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
var UploadFileService = /** @class */ (function () {
    function UploadFileService(http) {
        this.http = http;
        this._baseURL = 'http://localhost:xxxx/api/fileupload/';
    }
    UploadFileService.prototype.upload = function (files, parameters) {
        var headers = new http_1.Headers();
        var options = new http_1.RequestOptions({ headers: headers });
        options.params = parameters;
        return this.http.post(this._baseURL + 'upload', files, options)
            .map(function (response) { return response.json(); })
            .catch(function (error) { return Rx_1.Observable.throw(error); });
    };
    UploadFileService.prototype.getImages = function () {
        return this.http.get(this._baseURL + "getimages")
            .map(function (response) { return response.json(); })
            .catch(function (error) { return Rx_1.Observable.throw(error); });
    };
    UploadFileService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http])
    ], UploadFileService);
    return UploadFileService;
}());
exports.UploadFileService = UploadFileService;
//# sourceMappingURL=upload-files.service.js.map