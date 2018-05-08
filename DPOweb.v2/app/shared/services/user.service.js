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
var UserService = /** @class */ (function () {
    function UserService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
    }
    UserService.prototype.ngOnInit = function () {
        //this.getCurrentUser()
        //    .then(this.getCurrentUserCallback.bind(this));
    };
    UserService.prototype.isAuthenticated = function () {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.get("/api/User/IsAuthenticated", { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    UserService.prototype.getCurrentUser = function () {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.get("/api/User/GetCurrentUser", { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    UserService.prototype.extractData = function (res) {
        var resp = res.json();
        return resp || {};
    };
    UserService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    };
    UserService.prototype.hasAccess = function (user, accessId) {
        for (var _i = 0, _a = user.systemAccesses; _i < _a.length; _i++) {
            var item = _a[_i];
            if (item == accessId) {
                return true;
            }
        }
        return false;
    };
    UserService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], UserService);
    return UserService;
}());
exports.UserService = UserService;
//# sourceMappingURL=user.service.js.map