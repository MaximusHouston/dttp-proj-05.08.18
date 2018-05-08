import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class QuoteService {

    constructor(private toastrSvc: ToastrService, private http: Http) {
    }

    private headers = new Headers({
        'Content-Type': 'application/json',
        'dataType': 'json',
        'Accept': 'application/json'
    });


    public extractData(res: Response) {
        let body = res.json();
        return body || {};
    }

    public extractHtml(res: any) {
        return res._body;
    }


    public handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message

        //console.error(error); // log to console instead
        console.log(error);
        this.toastrSvc.Error(error.statusText);
        return Observable.throw(error.statusText);
    }

    public setBasketQuoteId(quoteId: any) {
          return this.http.get("/api/Quote/SetBasketQuoteId?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);

    }

    public getQuoteModel(projectId: any, quoteId: any): Observable<any> {
        return this.http.get("/api/Quote/GetQuoteModel?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

    }

    public postQuote(quote: any): Observable<any> {
        return this.http.post("/api/Quote/PostQuote", quote, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getQuote(projectId: any, quoteId: any) {
        return this.http.get("/api/Quote/GetQuoteModel?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers }).toPromise()
        .then(this.extractData)
        .catch(this.handleError);
    }

    //Order Form
    public getQuoteItems(quoteId: any) {
        return this.http.get("/api/Quote/GetQuoteItems?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    //Quote Page
    public getQuoteItemsModel(quoteId: any) {
        return this.http.get("/api/Quote/GetQuoteItemsModel?quoteId=" + quoteId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public getOptionItems(quoteItemId: any) {
        return this.http.get("/api/Quote/GetOptionItems?quoteItemId=" + quoteItemId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public setQuoteActive(data: any) {
        return this.http.post("/api/Quote/QuoteSetActive", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public deleteQuote(data: any) {
        return this.http.post("/api/Quote/deleteQuote", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public undeleteQuote(data: any) {
        return this.http.post("/api/Quote/undeleteQuote", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    //TODO: figure out why this is not working
    //public getQuoteItems(quoteId: any): Observable<any> {
    //    debugger
    //    return this.http.get("/api/Quote/GetQuoteItemsModel?quoteId=" + quoteId, { headers: this.headers })
    //        .map(this.extractData)
    //        .catch(this.handleError);

    //}

    public adjustQuoteItems(data: any) {
        return this.http.post("/api/Quote/AdjustQuoteItems", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public quoteRecalculate(data: any) {
        return this.http.post("/api/Quote/QuoteRecalculate", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

   

    //public getUserLoginModel(): Observable<any> {
    //    return this.http.get("/api/AccountApi/GetUserLoginModel", { headers: this.headers })
    //        .map(this.extractData)
    //        .catch(this.handleError);

    //}

    //public logIn(body: any) {
    //    return this.http.post("/api/AccountApi/Login", body, { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}



    //public getUserRegistrationModel(): Observable<any> {
    //    return this.http.get("/api/AccountApi/UserRegistration", { headers: this.headers })
    //        .map(this.extractData)
    //        .catch(this.handleError);
    //}

    //public userRegistration(body: any) {
    //    return this.http.post("/api/AccountApi/UserRegistration", body, { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}





}