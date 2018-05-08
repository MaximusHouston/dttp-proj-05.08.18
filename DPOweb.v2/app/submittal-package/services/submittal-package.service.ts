import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
import { BaseErrorHandler } from '../../shared/common/BaseErrorHandler.component';
import { SubmittalPackageModel } from '../../submittal-package/models/submittal-package';

@Injectable()
export class SubmittalPackageService extends BaseErrorHandler {
    private apiUrl: string;

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private options = new RequestOptions({ headers: this.headers });

    constructor(private http: Http, private toastrService: ToastrService) {
        super(toastrService);
        console.log('Submittal Package Service Initialized...');
    }     

    getQuotePackage(quoteId: string) {        
        //let body = model;
        //let options = new RequestOptions({ headers: headers, withCredentials: false });
        let data = this.http.get("/api/SubmittalPackage/GetQuotePackage?quoteId=" + quoteId)
            .map(this.extractData)
            .catch(this.handleError);    

        return data;
    }

    public createQuotePackage(model: SubmittalPackageModel): Observable<any> {
        this.apiUrl = "/api/SubmittalPackage/QuotePackageCreate";         

        return this.http.post(this.apiUrl, model, this.options)
            .map((res) => {
                return res.json();
            })
            .catch(this.handleError)
    }

    private extractData(res: Response) {
        const body = res.json().model as SubmittalPackageModel;
        return body || null;
    }
}

    