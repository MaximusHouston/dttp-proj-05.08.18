
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { IProductFamily } from '../productFamily';
import {
    GridComponent,
    GridDataResult,
    DataStateChangeEvent
} from '@progress/kendo-angular-grid';

@Injectable()
export class ProductFamilyService  {
    private _productFamilyUrl = '/api/Product/GetProductFamilies';

    constructor(private _http: Http) { }

    //getProductFamilies(): Observable<IProductFamily[]> {
    //    return this._http.get(this._productFamilyUrl)
    //        .map((response: Response) => <IProductFamily[]>response.json())
    //        .do(data => console.log('All: ' + JSON.stringify(data)))
    //        .catch(this.handleError);
    //}

    getProductFamilies(): Observable<IProductFamily[]> {
        return this._http.get(this._productFamilyUrl)
            .map((response: Response) => response.json().data)
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}

