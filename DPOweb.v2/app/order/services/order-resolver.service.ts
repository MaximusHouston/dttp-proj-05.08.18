import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRoute, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
import { OrderService } from './order.service';

@Injectable()
export class OrderResolver {
    constructor(private orderSvc: OrderService) {
    }
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        let projectId = route.params['projectid'];
        let quoteId = route.params['quoteid'];
        let recordState = route.params['recordState'];

        if (recordState == "new") {
            return this.orderSvc.orderForm(projectId, quoteId)
                .map(orderFormModel => {
                    if (orderFormModel) {
                        return orderFormModel;
                    } else {
                        return null;
                    }
                }).catch(error => {
                    //console.log('Retrieval error: ${error}');
                    console.log(error);
                    return Observable.of(null);
                });
        } else if (recordState == "submitted") {
            return this.orderSvc.getSubmittedOrderForm(quoteId)
                .map(orderFormModel => {
                    if (orderFormModel) {
                        return orderFormModel;
                    } else {
                        return null;
                    }
                }).catch(error => {
                    //console.log('Retrieval error: ${error}');
                    console.log(error);
                    return Observable.of(null);
                });
        }
        
    }
}