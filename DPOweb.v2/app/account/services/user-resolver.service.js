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
var Observable_1 = require("rxjs/Observable");
require("rxjs/Rx");
var account_service_1 = require("./account.service");
var UserResolver = /** @class */ (function () {
    function UserResolver(accountSvc) {
        this.accountSvc = accountSvc;
    }
    UserResolver.prototype.resolve = function (route, state) {
        return this.accountSvc.getUserRegistrationModel()
            .map(function (user) {
            if (user) {
                return user;
            }
            else {
                return null;
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
            return Observable_1.Observable.of(null);
        });
    };
    UserResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [account_service_1.AccountService])
    ], UserResolver);
    return UserResolver;
}());
exports.UserResolver = UserResolver;
var CurrentUserResolver = /** @class */ (function () {
    function CurrentUserResolver(accountSvc) {
        this.accountSvc = accountSvc;
    }
    CurrentUserResolver.prototype.resolve = function (route, state) {
        return this.accountSvc.getCurrentUser()
            .map(function (currentUser) {
            if (currentUser) {
                return currentUser;
            }
            else {
                return null;
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
            return Observable_1.Observable.of(null);
        });
    };
    CurrentUserResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [account_service_1.AccountService])
    ], CurrentUserResolver);
    return CurrentUserResolver;
}());
exports.CurrentUserResolver = CurrentUserResolver;
//# sourceMappingURL=user-resolver.service.js.map