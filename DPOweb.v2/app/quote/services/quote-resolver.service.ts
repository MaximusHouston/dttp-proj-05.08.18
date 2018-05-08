import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRoute, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
import { QuoteService } from './quote.service';

@Injectable()
export class QuoteResolver {
    constructor(private quoteSvc: QuoteService) {
    }
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        let quoteId = route.params['id'];
        
        return this.quoteSvc.getQuoteModel(null, quoteId)
            .map(quoteModel => {
                if (quoteModel) {
                    return quoteModel;
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

@Injectable()
export class QuoteEditResolver {
    constructor(private quoteSvc: QuoteService) {
    }
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        let quoteId = route.params['quoteid'];
        let projectId = route.params['projectid'];
        return this.quoteSvc.getQuoteModel(projectId, quoteId)
            .map(quoteModel => {
                if (quoteModel) {
                    return quoteModel;
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

@Injectable()
export class QuoteItemsResolver {
    constructor(private quoteSvc: QuoteService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let quoteId = route.params['id'];

        return this.quoteSvc.getQuoteItemsModel(quoteId)
            .then(quoteItems => {
                if (quoteItems) {
                    return quoteItems;
                } else {
                    return null;
                }
            }).catch(error => {
                console.log('Retrieval error: ${error}');
                console.log(error);
            });


    }
    
}