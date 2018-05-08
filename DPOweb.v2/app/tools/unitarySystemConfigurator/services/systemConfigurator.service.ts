import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../../shared/services/toastr.service';

@Injectable()
export class SystemConfiguratorService {

    private headers = new Headers({
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    });

    constructor(private toastrSvc: ToastrService, private http: Http) {
    }




    public getSystemMatchupList(params: any) {
        var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(url, params, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public getTonnageList() {
        var body = {
            "user": "user",
            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
            "packageName": "SystemMatchup",
            "servicesName": "doGetTonnageList",
            "accountId": "goodman1",
            "params": {}
        }

        var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(url, body, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public getEqModelList(params: any) {
        //var body = {
        //    "user": "",
        //    "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
        //    "packageName": "SystemMatchupDaikin",
        //    "servicesName": "doGetEqModelList",
        //    "accountId": "goodman1",
        //    "params": {
        //        "model": "N",
        //        "type": "AC",
        //        "modelnumber": "DX",
        //        "region": "se"
        //    }
        //}

        var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(url, params, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public getEqCoilList(params: any) {
        var body = {
            "user": "",
            "tokenId": "7240794B-6D5A-4AAA-BAE4-7FE3FA07050F",
            "packageName": "SystemMatchupDaikin",
            "servicesName": "doGetEqCoilList",
            "accountId": "goodman1",
            "params": {
                "type": "ac",
                "modelnumber": "DX14SN0251B*"
            }
        }

        var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(url, params, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);

    }

    public getEEPFurnaceList(params: any) {
      
        var url = 'https://testapi.goodmanmfg.com/EBizWebServices/requestEntry';
        return this.http.post(url, params, { headers: this.headers }).toPromise().then(this.extractData).catch(this.handleError);
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