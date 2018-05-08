import { Injectable, OnInit } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from './toastr.service';

@Injectable()
export class CommonService {

    constructor(private toastrSvc: ToastrService, private http: Http) {

    }

    private headers = new Headers({
        'Content-Type': 'application/json',
        'dataType': 'json',
        'Accept': 'application/json'
    });

    
    public getStateIdByStateCode(stateCode: any): Observable<any> {
        return this.http.get("/api/Common/GetStateIdByStateCode?stateCode=" + stateCode, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
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
}