"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
//import * as toastr from "toastr";
//import * as toastr from "../../script/toastr.min";
//import {Toastr} from "toastr";
var ToastrService = /** @class */ (function () {
    function ToastrService() {
    }
    ToastrService.prototype.Error = function (message) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "timeOut": 0,
            "extendedTimeOut": 0,
            "preventDuplicates": true
        };
        toastr.error(message + "<br /><br /><button type'button' class='btn clear' style='color: black'>Ok</button>");
    };
    ToastrService.prototype.ErrorFadeOut = function (message) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        };
        toastr.error(message);
    };
    ToastrService.prototype.Success = function (message) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        };
        toastr.success(message);
    };
    ToastrService.prototype.Info = function (message) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        };
        toastr.info(message);
    };
    ToastrService.prototype.Warning = function (message) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        };
        toastr.warning(message);
    };
    ToastrService.prototype.displayResponseMessages = function (response) {
        if (response != null && response.messages != null) {
            for (var _i = 0, _a = response.messages.items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.type == 40) {
                    this.Success(message.text);
                }
                else if (message.type == 10) {
                    this.Error(message.text);
                }
            }
        }
    };
    ToastrService.prototype.displayResponseMessagesFadeOut = function (response) {
        if (response != null && response.messages != null) {
            for (var _i = 0, _a = response.messages.items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.type == 40) {
                    this.Success(message.text);
                }
                else if (message.type == 10) {
                    this.ErrorFadeOut(message.text);
                }
            }
        }
    };
    ToastrService = __decorate([
        core_1.Injectable()
    ], ToastrService);
    return ToastrService;
}());
exports.ToastrService = ToastrService;
//# sourceMappingURL=toastr.service.js.map