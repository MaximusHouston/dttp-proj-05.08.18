import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class AccountService {

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

        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Observable.throw(error.statusText);
    }

    //public getUserLoginModel() {

    //    return this.http.get("/api/AccountApi/GetUserLoginModel", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    //}

    public getUserLoginModel(): Observable<any> {
        return this.http.get("/api/AccountApi/GetUserLoginModel", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

    }

    public logIn(body: any) {
        return this.http.post("/api/AccountApi/Login", body, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    //public getUserRegistrationModel() {
    //    return this.http.get("/api/AccountApi/UserRegistration", { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}

    public getUserRegistrationModel(): Observable<any> {
        return this.http.get("/api/AccountApi/UserRegistration", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getCurrentUser(): Observable<any> {
        return this.http.get("/api/User/GetCurrentUser", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    //not working
    //public userRegistration(data: any) {
    //    return this.http.post("/api/AccountApi/UserRegistration", data, { headers: this.headers }).toPromise()
    //        .then(this.extractData)
    //        .catch(this.handleError);
    //}



    public userRegistration(data: any) {
        return this.http.post("/Account/RegisterUser", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public businessAddressLookup(accountId: any) {
        return this.http.get("/api/AccountApi/BusinessAddressLookup?accountId=" + accountId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public resetBasketQuoteId() {
        return this.http.get("/api/AccountApi/ResetBasketQuoteId", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

}