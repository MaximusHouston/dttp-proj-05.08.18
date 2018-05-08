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
var toastr_service_1 = require("../../shared/services/toastr.service");
//import 'rxjs/add/operator/toPromise';
var ProductService = /** @class */ (function () {
    function ProductService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    ProductService.prototype.getBasketQuoteId = function () {
        return this.http.get("/api/Product/GetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //TODO: deprecated, delete after 01/31/2018
    ProductService.prototype.resetBasketQuoteId = function () {
        return this.http.get("/api/Product/ResetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //Go to product page & reset session["BasketQuoteId"] = 0
    ProductService.prototype.products = function () {
        return this.http.get("/api/Product/Products", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.browseProductsWithQuoteId = function (quoteId) {
        return this.http.get("/api/Product/BrowseProductsWithQuoteId?quoteId=" + quoteId, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.getProductFamilies = function () {
        return this.http.get("/api/Product/GetProductFamilies", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.getInstallationTypes = function (data) {
        return this.http.post("/api/Product/GetInstallationTypes", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.getProductStatuses = function () {
        return this.http.get("/api/Product/GetProductStatuses", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.getInventoryStatuses = function () {
        return this.http.get("/api/Product/GetInventoryStatuses", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //public getProducts() {
    //    let headers = new Headers({
    //        'Content-Type': 'application/json',
    //        'dataType': 'json',
    //        'Accept': 'application/json'
    //    });
    //    //return this.http.get("/api/Product/GetProducts").map(this.extractData).catch(this.handleError);
    //    return this.http.get("/api/Product/GetProducts", { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    //}
    ProductService.prototype.getProducts = function (data) {
        return this.http.post("/api/Product/GetProducts", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.getProduct = function (data) {
        return this.http.post("/api/Product/GetProduct", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //public getAccessories(data: any) {
    //    return this.http.post("/api/Product/GetAccessories", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    //}
    //add multiple products
    ProductService.prototype.addProductsToQuote = function (data) {
        return this.http.post("/api/Product/AddProductsToQuote", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //add single product
    ProductService.prototype.addProductToQuote = function (product) {
        return this.http.post("/api/Product/AddProductToQuote", product, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.addProductToQuoteByProductNumber = function (product) {
        return this.http.post("/api/Product/AddProductToQuoteByProductNumber", product, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProductService.prototype.addSystemToQuote = function (system) {
        return this.http.post("/api/Product/AddSystemToQuote", system, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    //This service returns HTML as string 
    ProductService.prototype.getSubmittalDataSheet = function (ProductNumber) {
        //return this.http.get("/ProductDashboard/SubmittalTemplateHtml?ProductNumber=FDXS12LVJURXS12LVJU&PdfMode=true").toPromise().then(this.extractHtml).catch(this.handleError);
        return this.http.get("/ProductDashboard/SubmittalTemplateHtml?ProductNumber=" + ProductNumber + "&PdfMode=true").toPromise().then(this.extractHtml).catch(this.handleError);
    };
    ProductService.prototype.extractData = function (res) {
        var resp = res.json();
        return resp || {};
    };
    ProductService.prototype.extractHtml = function (res) {
        return res._body;
    };
    ProductService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    };
    ProductService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], ProductService);
    return ProductService;
}());
exports.ProductService = ProductService;
//# sourceMappingURL=product.service.js.map