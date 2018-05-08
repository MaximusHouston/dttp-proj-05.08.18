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
//import {Http, Headers, RequestOptions, Response} from '@angular/http';
//import {Observable} from 'rxjs/Observable';
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
//import { ProjectService } from './services/project.service';
var order_service_1 = require("./services/order.service");
var OrderComponent = /** @class */ (function () {
    function OrderComponent(toastrSvc, orderSvc) {
        this.toastrSvc = toastrSvc;
        this.orderSvc = orderSvc;
    }
    OrderComponent.prototype.ngOnInit = function () {
    };
    OrderComponent.prototype.getSubmittalOrder = function () {
        //alert("hello");
        this.orderSvc.getSubmittalOrder()
            .then(function (resp) {
            var result = resp;
            debugger;
        })
            .catch(function (error) {
            console.log(error);
        });
    };
    OrderComponent = __decorate([
        core_1.Component({
            selector: 'order',
            templateUrl: 'app/order/order.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, order_service_1.OrderService])
    ], OrderComponent);
    return OrderComponent;
}());
exports.OrderComponent = OrderComponent;
;
//# sourceMappingURL=order.component.js.map