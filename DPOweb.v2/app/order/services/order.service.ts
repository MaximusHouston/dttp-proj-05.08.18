import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class OrderService {

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

    //Test
    public getSubmittalOrder() {
        return this.http.get("/api/Order/GetSubmittalOrder", { headers: this.headers }).toPromise()
            .then((resp: Response) => {
                debugger
                return resp;

            })
            .catch(this.handleError);
    }

    public orderForm(projectId: any, quoteId: any): Observable<any> {
        return this.http.get("/api/Order/OrderForm?projectId=" + projectId + "&quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getSubmittedOrderForm(quoteId: any): Observable<any> {
        return this.http.get("/api/Order/GetSubmittedOrder?quoteId=" + quoteId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public postOrder(order: any): Observable<any> {
        return this.http.post("/api/Order/PostOrder", order, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public sendOrderEmail(order: any): Observable<any> {
        return this.http.post("/ProjectDashboard/SendEmailOrderSubmit", order, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    
}