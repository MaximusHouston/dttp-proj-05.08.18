import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
    selector: "option-items",
    templateUrl: "app/quote/option-items.component.html"

})

export class OptionItemsComponent implements OnInit {

    //@Input() quote: any;
    //@Input() user: any;

    @Input() configuredItem: any;
    @Input() unitQuantity: any;
    public optionItems: any;


    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {



    }
    ngOnInit() {
        this.loadOptionItems();
    }

    public loadOptionItems() {
        var self = this;
        this.quoteSvc.getOptionItems(this.configuredItem.quoteItemId).then((resp: any) => {
            if (resp.isok) {
                self.optionItems = resp.model;
            } else {
            }
        }).catch(error => {
            console.log(error);
        });
    }
}