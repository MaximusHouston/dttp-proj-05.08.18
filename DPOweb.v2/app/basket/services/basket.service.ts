import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';

@Injectable()
export class BasketService {
    public userBasket: any;

    constructor(private toastrSvc: ToastrService, private http: Http) {
    }

    public getBasket() {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });

        return this.http.get("/api/User/getBasket", { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
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