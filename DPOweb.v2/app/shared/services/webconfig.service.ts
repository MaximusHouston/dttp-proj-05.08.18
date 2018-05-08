import { Injectable, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from './toastr.service';

@Injectable()
export class WebConfigService {

    constructor(private toastrSvc: ToastrService, private http: Http) {
    }

    public extractData(res: Response) {
        let resp = res.json();
        return resp || {};
    }

    public handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message

        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    }

    private headers = new Headers({
        'Content-Type': 'application/json',
        'dataType': 'json',
        'Accept': 'application/json'
    });

    public getWebConfig() {
        return this.http.get("/v2/webconfig.v2.json", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getLCSTApiToken() {
        return this.http.get("/api/Product/GetLCSTApiToken", { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

}