import { Component, OnInit, Input, Output, SimpleChanges, OnChanges, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from './services/quote.service';

declare var jQuery: any;

@Component({
    selector: "quote-button-bar",
    templateUrl: "app/quote/quote-button-bar.component.html"

})

export class QuoteButtonBarComponent implements OnInit {

    @Input() quote: any;
    @Input() quoteItems: any;
    @Input() user: any;

    public message: string;
    public actionUrl: string;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {



    }

    ngOnChanges(changes: SimpleChanges) {
        //console.log("ngOnChanges");
    }

    ngOnInit() {

    }

    openOrderForm() {
        //this.actionUrl = "/ProjectDashboard/OrderForm?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.actionUrl = "/v2/#/orderForm/" + this.quote.projectId + "/" + this.quote.quoteId + "/new" ;
        //this.actionUrl = "/orderForm/" + this.quote.projectId + "/" + this.quote.quoteId + "/new";
        this.setupInventoryCheckModal();
    }

    requestDiscount() {
        this.actionUrl = "/ProjectDashboard/DiscountRequest?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.setupInventoryCheckModal();
    }

    requestCommission() {
        this.actionUrl = "/ProjectDashboard/CommissionRequest?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId;
        this.setupInventoryCheckModal();
    }

    public setupInventoryCheckModal() {
        if (this.quoteItems.hasObsoleteAndUnavailableProduct) {
            this.message = "This quote contains product(s) which are Obsolete and Unavailable. Please review and revise product(s) list to continue processing the quote or contact your Daikin Sales Representative."
            jQuery("#inventoryCheckModal").modal({ backdrop: 'static', keyboard: false });
        }
        else if (this.quoteItems.hasObsoleteProduct || this.quoteItems.hasUnavailableProduct) {
            this.message = "This quote contains obsolete or unavailable product(s). Please review and revise product(s) list or contact your Daikin Sales Representative."
            jQuery("#inventoryCheckModal").modal({ backdrop: 'static', keyboard: false });
        }
        else {
            window.location.href = this.actionUrl;
            //this.router.navigateByUrl(this.actionUrl);
        }
    }

    public redirect() {
        window.location.href = this.actionUrl;
        //this.router.navigateByUrl(this.actionUrl);
    }



    public setQuoteActive() {
        var self = this;

        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
           
        };

        
        self.loadingIconSvc.Start(jQuery("#content"));

        this.quoteSvc.setQuoteActive(data).then((resp: any) => {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + this.quote.projectId;
            } else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(error => {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public deleteQuote() {
        var self = this;

        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
        };


        self.loadingIconSvc.Start(jQuery("#content"));

        this.quoteSvc.deleteQuote(data).then((resp: any) => {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + this.quote.projectId;
            } else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(error => {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public undeleteQuote() {
        var self = this;

        var data = {
            "ProjectId ": this.quote.projectId,
            "QuoteId": this.quote.quoteId
        };


        self.loadingIconSvc.Start(jQuery("#content"));

        this.quoteSvc.undeleteQuote(data).then((resp: any) => {
            if (resp.isok) {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#content"));
                window.location.href = "/ProjectDashboard/ProjectQuotes/" + this.quote.projectId;
            } else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(error => {
            self.loadingIconSvc.Stop(jQuery("#content"));
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public quotePrintNoPrices() {
        //var url = "/ProjectDashboard/QuotePrint?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=false";
        var url = "/Document/QuotePrint/" + this.quote.projectId + "?quoteId=" + this.quote.quoteId;
        window.open(url, '_blank');
    }
    public quotePrintWithPrices() {
        //var url = "/ProjectDashboard/QuotePrint?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=true";
        var url = "/Document/QuotePrintWithCostPrice/" + this.quote.projectId + "?quoteId=" + this.quote.quoteId;
        window.open(url, '_blank');
    }
    public quoteDownloadNoPrices() {
        var url = "/ProjectDashboard/QuotePrintExcel?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=false";
        window.open(url, '_blank');

    }
    public quoteDownloadWithPrices() {
        var url = "/ProjectDashboard/QuotePrintExcel?projectId=" + this.quote.projectId + "&quoteId=" + this.quote.quoteId + "&withCostPrices=true";
        window.open(url, '_blank');

    }


};
