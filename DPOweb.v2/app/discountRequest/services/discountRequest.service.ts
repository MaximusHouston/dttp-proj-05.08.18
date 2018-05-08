import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class DiscountRequestService {

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


    public getDiscountRequest(discountRequestId: any, projectId: any, quoteId: any): Observable<any> {
        return this.http.get("/api/DiscountRequest/GetDiscountRequest?discountRequestId=" + discountRequestId +"&projectId=" + projectId + "&quoteId=" + quoteId)
            .map(this.extractData)
            .catch(this.handleError);
    }

    public postDiscountRequest(discountRequest: any): Observable<any> {

        //API Controller
        //return this.http.post("/api/DiscountRequest/PostDiscountRequest", discountRequest, { headers: this.headers })
        //    .map(this.extractData)
        //    .catch(this.handleError);

        //MVC Controller
        return this.http.post("/ProjectDashboard/DiscountRequest", discountRequest, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

        //let _headers = new Headers({
        //    'Content-Type': 'multipart/form-data',
        //    'Accept': 'application/json'
        //});

        //return this.http.post("/api/DiscountRequest/PostDiscountRequest", discountRequest, { headers: _headers })
        //    .map(this.extractData)
        //    .catch(this.handleError);
    }



}