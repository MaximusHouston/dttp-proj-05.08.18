import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class CommissionRequestService {
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

    public getCommissionRequestModel(projectId: any, quoteId: any, commissionRequestId: any, commissionRequestStatusTypeId: any){
        return this.http.get("/api/CommissionRequest/GetCommissionRequestModel?projectId=" + projectId + "&quoteId=" + quoteId + "&commissionRequestId=" + commissionRequestId + "&commissionRequestStatusTypeId=" + commissionRequestStatusTypeId, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);

    }

    public postCommissionCalculation(data: any) {
        return this.http.post("/api/CommissionRequest/PostCommissionCalculation", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);

    }

    public getCommissionPercentage(data: any) {
        return this.http.post("/api/CommissionRequest/GetCommissionPercentage", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    public getUnitaryCommissionPercentage(data: any) {
        return this.http.post("/api/CommissionMultiplier/GetUnitaryMultiplier", data, { headers: this.headers }).toPromise()
            .then(this.extractData)
            .catch(this.handleError);
    }

    
}