import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
//import 'rxjs/add/operator/toPromise';

@Injectable()
export class ProductService {
    public product: any;
    public productsModel: any;
    //public userBasket: any;

    //public productFamilyId: any;
    //public productModelTypeId: any;
    //public unitInstallationTypeId: any;

    public productFamilyTabs: any;
    public unitInstallationTypeTabs: any;

    private headers = new Headers({
        'Content-Type': 'application/json',
        'dataType': 'json',
        'Accept': 'application/json'
    });

    constructor(private toastrSvc: ToastrService, private http: Http) {
    }

    public getBasketQuoteId() {
        return this.http.get("/api/Product/GetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    //TODO: deprecated, delete after 01/31/2018
    public resetBasketQuoteId() {
        return this.http.get("/api/Product/ResetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    //Go to product page & reset session["BasketQuoteId"] = 0
    public products() {

        return this.http.get("/api/Product/Products", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public browseProductsWithQuoteId(quoteId: any) {

        return this.http.get("/api/Product/BrowseProductsWithQuoteId?quoteId=" + quoteId, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getProductFamilies() {

        return this.http.get("/api/Product/GetProductFamilies", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getInstallationTypes(data: any) {

        return this.http.post("/api/Product/GetInstallationTypes", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

	public getProductStatuses() {

        return this.http.get("/api/Product/GetProductStatuses", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getInventoryStatuses() {

        return this.http.get("/api/Product/GetInventoryStatuses", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }
    
    //public getProducts() {

    //    let headers = new Headers({
    //        'Content-Type': 'application/json',
    //        'dataType': 'json',
    //        'Accept': 'application/json'
    //    });

    //    //return this.http.get("/api/Product/GetProducts").map(this.extractData).catch(this.handleError);
    //    return this.http.get("/api/Product/GetProducts", { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    //}

    public getProducts(data: any) {
        return this.http.post("/api/Product/GetProducts", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getProduct(data: any) {
        return this.http.post("/api/Product/GetProduct", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    //public getAccessories(data: any) {
    //    return this.http.post("/api/Product/GetAccessories", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    //}

    //add multiple products
    public addProductsToQuote(data: any) {

        return this.http.post("/api/Product/AddProductsToQuote", data, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    //add single product
    public addProductToQuote(product: any) {
        return this.http.post("/api/Product/AddProductToQuote", product, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    
    public addProductToQuoteByProductNumber(product: any) {
        return this.http.post("/api/Product/AddProductToQuoteByProductNumber", product, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public addSystemToQuote(system: any) {
        return this.http.post("/api/Product/AddSystemToQuote", system, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    //This service returns HTML as string 
    public getSubmittalDataSheet(ProductNumber: any) {
        //return this.http.get("/ProductDashboard/SubmittalTemplateHtml?ProductNumber=FDXS12LVJURXS12LVJU&PdfMode=true").toPromise().then(this.extractHtml).catch(this.handleError);
        return this.http.get("/ProductDashboard/SubmittalTemplateHtml?ProductNumber=" + ProductNumber +"&PdfMode=true").toPromise().then(this.extractHtml).catch(this.handleError);
    }


    public extractData(res: Response) {
        let resp = res.json();
        return resp || {};
    }

    public extractHtml(res: any) {
        return res._body;
    }


    public handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message

        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    }



}