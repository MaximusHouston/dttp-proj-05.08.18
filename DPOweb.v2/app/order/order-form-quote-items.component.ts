import { Component, OnInit, Input, Output, EventEmitter, ViewChildren } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { QuoteService } from '../quote/services/quote.service';
declare var jQuery: any;

@Component({
    selector: 'order-form-quote-items',
    templateUrl: 'app/order/order-form-quote-items.component.html'
})

export class OrderFormQuoteItemsComponent implements OnInit {
    @Input() quoteId: any;
    public quoteItems: any;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http,
        private quoteSvc: QuoteService) {

        
        //this.quoteSvc.getQuoteItems(this.quoteId).then((resp: any) => {
        //    if (resp.isok) {
        //        this.quoteItems = resp.model;
        //    } 
        //}).catch(error => {
        //    console.log(error);
        //});
        

    }

    ngOnInit() {
        this.quoteSvc.getQuoteItems(this.quoteId).then((resp: any) => {
            if (resp.isok) {
                this.quoteItems = resp.model;
            }
        }).catch(error => {
            console.log(error);
        });
    }
}