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
var quote_service_1 = require("./quote.service");
var QuoteResolver = /** @class */ (function () {
    function QuoteResolver(quoteSvc) {
        this.quoteSvc = quoteSvc;
    }
    QuoteResolver.prototype.resolve = function (route, state) {
        var quoteId = route.params['id'];
        return this.quoteSvc.getQuoteModel(null, quoteId)
            .map(function (quoteModel) {
            if (quoteModel) {
                return quoteModel;
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
    QuoteResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [quote_service_1.QuoteService])
    ], QuoteResolver);
    return QuoteResolver;
}());
exports.QuoteResolver = QuoteResolver;
var QuoteEditResolver = /** @class */ (function () {
    function QuoteEditResolver(quoteSvc) {
        this.quoteSvc = quoteSvc;
    }
    QuoteEditResolver.prototype.resolve = function (route, state) {
        var quoteId = route.params['quoteid'];
        var projectId = route.params['projectid'];
        return this.quoteSvc.getQuoteModel(projectId, quoteId)
            .map(function (quoteModel) {
            if (quoteModel) {
                return quoteModel;
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
    QuoteEditResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [quote_service_1.QuoteService])
    ], QuoteEditResolver);
    return QuoteEditResolver;
}());
exports.QuoteEditResolver = QuoteEditResolver;
var QuoteItemsResolver = /** @class */ (function () {
    function QuoteItemsResolver(quoteSvc) {
        this.quoteSvc = quoteSvc;
    }
    QuoteItemsResolver.prototype.resolve = function (route, state) {
        var quoteId = route.params['id'];
        return this.quoteSvc.getQuoteItemsModel(quoteId)
            .then(function (quoteItems) {
            if (quoteItems) {
                return quoteItems;
            }
            else {
                return null;
            }
        }).catch(function (error) {
            console.log('Retrieval error: ${error}');
            console.log(error);
        });
    };
    QuoteItemsResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [quote_service_1.QuoteService])
    ], QuoteItemsResolver);
    return QuoteItemsResolver;
}());
exports.QuoteItemsResolver = QuoteItemsResolver;
//# sourceMappingURL=quote-resolver.service.js.map