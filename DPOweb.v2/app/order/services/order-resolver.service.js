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
var order_service_1 = require("./order.service");
var OrderResolver = /** @class */ (function () {
    function OrderResolver(orderSvc) {
        this.orderSvc = orderSvc;
    }
    OrderResolver.prototype.resolve = function (route, state) {
        var projectId = route.params['projectid'];
        var quoteId = route.params['quoteid'];
        var recordState = route.params['recordState'];
        if (recordState == "new") {
            return this.orderSvc.orderForm(projectId, quoteId)
                .map(function (orderFormModel) {
                if (orderFormModel) {
                    return orderFormModel;
                }
                else {
                    return null;
                }
            }).catch(function (error) {
                //console.log('Retrieval error: ${error}');
                console.log(error);
                return Observable_1.Observable.of(null);
            });
        }
        else if (recordState == "submitted") {
            return this.orderSvc.getSubmittedOrderForm(quoteId)
                .map(function (orderFormModel) {
                if (orderFormModel) {
                    return orderFormModel;
                }
                else {
                    return null;
                }
            }).catch(function (error) {
                //console.log('Retrieval error: ${error}');
                console.log(error);
                return Observable_1.Observable.of(null);
            });
        }
    };
    OrderResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [order_service_1.OrderService])
    ], OrderResolver);
    return OrderResolver;
}());
exports.OrderResolver = OrderResolver;
//# sourceMappingURL=order-resolver.service.js.map