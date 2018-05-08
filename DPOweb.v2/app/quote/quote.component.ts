import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from './services/quote.service';
import { ProductService } from '.././products/services/product.service';

declare var jQuery: any;

@Component({
    selector: "quote",
    templateUrl: "app/quote/quote.component.html"

})

export class QuoteComponent implements OnInit {

    public quoteId: any;
    public recordState: any = false;
    public quote: any;
    public user: any;
    public quoteItems: any;



    public overViewActive: boolean;
    public quoteItemsActive: boolean;
    public quoteDiscountRequestsActive: boolean;
    public quoteCommissionRequestsActive: boolean;
    public quoteOrderActive: boolean;


    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService, private productSvc: ProductService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {

        //this.accountSvc.getUserLoginModel().then(this.getUserLoginModelCallback.bind(this));

        //this.accountSvc.getUserLoginModel()
        //    .subscribe(
        //    data => {
        //        this.user = data.model
        //    },
        //    err => console.log("Error: ", err)
        //    );

        this.quote = this.route.snapshot.data['quoteModel'].model;
        this.user = this.route.snapshot.data['currentUser'].model;
        //var test = this.route.snapshot.url[0].path;
        //var id = this.route.snapshot.paramMap.get('id');

        //this.route.params.subscribe((params: { id: string }) => {
        //    var temp = params.id;
        //    debugger

        //});


    }
    ngOnInit() {

        let path = this.route.snapshot.url[0].path;
        this.quoteId = this.route.snapshot.paramMap.get('id');
        
        this.recordState = this.route.snapshot.paramMap.get('recordState');

        this.setActiveTab(path);

    }

    public setActiveTab(path: string) {

        if (path == 'quote') {
            this.overViewActive = true;
            this.quoteItemsActive = false;
            this.quoteDiscountRequestsActive = false;
            this.quoteCommissionRequestsActive = false;
            this.quoteOrderActive = false;

            this.getQuoteItems(this.quote.quoteId);
        } else if (path == 'quoteItems') {
            this.overViewActive = false;
            this.quoteItemsActive = true;
            this.quoteDiscountRequestsActive = false;
            this.quoteCommissionRequestsActive = false;
            this.quoteOrderActive = false;

            if (this.route.snapshot.data['quoteItems'] == undefined) {
                this.getQuoteItems(this.quote.quoteId);
            } else {
                this.quoteItems = this.route.snapshot.data['quoteItems'].model;
            }
        }
    }

    public showQuoteOverview() {
        $('#k-tabstrip-tab-0').click();
    }


    public showQuoteItems() {
        $('#k-tabstrip-tab-1').click();
    }

    public getQuoteItems(quoteId: any) {
        var self = this;

        console.log("get QuoteItems");


        self.quoteSvc.getQuoteItemsModel(this.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                self.quoteItems = resp.model;
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });

    }

    public browseProductsWithQuoteId() {
        this.productSvc.browseProductsWithQuoteId(this.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                //self.quoteItems = resp.model;
                window.location.href = "/v2/#/products";
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public reloadData() {
        this.reloadQuote();
        this.reloadQuoteItems();
    }
    public reloadQuote() {
        var self = this;

        //self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));

        self.quoteSvc.getQuote(self.quote.projectId, self.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.quote = resp.model;
                //self.gridIsModified = false;
                //jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            } else {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public reloadQuoteItems() {
        var self = this;

        self.loadingIconSvc.Start(jQuery("#content"));

        self.quoteSvc.getQuoteItemsModel(self.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.quoteItems = resp.model;
                //self.gridIsModified = false;
                jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            } else {
                self.loadingIconSvc.Stop(jQuery("#content"));
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });


    }

};
