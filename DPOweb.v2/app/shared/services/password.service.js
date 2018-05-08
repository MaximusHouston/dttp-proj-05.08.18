"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var PasswordService = /** @class */ (function () {
    function PasswordService() {
    }
    PasswordService.prototype.PasswordStrengthLevel = function (password) {
        var strength = 0;
        if (this.ContainsLowerCase(password)) {
            strength++;
        }
        if (this.ContainsUpperCase(password)) {
            strength++;
        }
        if (this.ContainsNumber(password)) {
            strength++;
        }
        if (this.ContainsSpecialCharacter(password)) {
            strength++;
        }
        if (password.length >= 8) {
            strength++;
        }
        return strength;
    };
    PasswordService.prototype.ContainsLowerCase = function (password) {
        var patt = /[a-z]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        }
        else
            return false;
    };
    PasswordService.prototype.ContainsUpperCase = function (password) {
        var patt = /[A-Z]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        }
        else
            return false;
    };
    PasswordService.prototype.ContainsNumber = function (password) {
        var patt = /[0-9]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        }
        else
            return false;
    };
    PasswordService.prototype.ContainsSpecialCharacter = function (password) {
        var patt = /[`~!@#$%\^&\*\(\)\-_=\+\{\}\[\]\|\\:;"',\.\/<>\?]/g;
        var result = password.match(patt);
        if (result != null && result.length > 0) {
            return true;
        }
        else
            return false;
    };
    PasswordService = __decorate([
        core_1.Injectable()
    ], PasswordService);
    return PasswordService;
}());
exports.PasswordService = PasswordService;
//# sourceMappingURL=password.service.js.map