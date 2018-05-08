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
var toastr_service_1 = require("../services/toastr.service");
var Observable_1 = require("rxjs/Observable");
var BaseErrorHandler = /** @class */ (function () {
    function BaseErrorHandler(toastrSvc) {
        this.toastrSvc = toastrSvc;
    }
    ;
    BaseErrorHandler.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        //console.error(error); // log to console instead
        console.log(error);
        this.toastrSvc.Error(error.statusText);
        return Observable_1.Observable.throw(error.statusText);
    };
    BaseErrorHandler = __decorate([
        core_1.Component({
            selector: 'base-errorhandler',
            templateUrl: 'app/shared/common/BaseErrorHandler.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService])
    ], BaseErrorHandler);
    return BaseErrorHandler;
}());
exports.BaseErrorHandler = BaseErrorHandler;
;
//# sourceMappingURL=BaseErrorHandler.component.js.map