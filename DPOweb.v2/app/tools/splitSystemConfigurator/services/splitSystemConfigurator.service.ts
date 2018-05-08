import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../../shared/services/toastr.service';
import { WebConfigService } from '../../../shared/services/webconfig.service';

@Injectable()
export class SplitSystemConfiguratorService {

    private headers = new Headers({
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    });

    public webconfig: any;
    public commercialSplitMCToolURL: any;

    constructor(private toastrSvc: ToastrService, private http: Http, private webconfigSvc: WebConfigService) {
        var self = this;
        this.webconfigSvc.getWebConfig().then((resp: any) => {
            self.webconfig = resp;
            self.commercialSplitMCToolURL = self.webconfig.routeConfig.commercialSplitMatchupToolURL;
        }).catch(error => {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });
    }




    public getSystemMatchupList(params: any) {
        //var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(this.commercialSplitMCToolURL, params, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public getTonnageList() {
        var body = {
            "user": "user",
            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
            "packageName": "SystemMatchupDaikinSplSt",
            "servicesName": "doGetTonnageList",
            "accountId": "goodman1",
            "params": {}
        }

        //var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(this.commercialSplitMCToolURL, body, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

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