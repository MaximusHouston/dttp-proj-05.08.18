import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';



declare var jQuery: any;

@Component({
    selector: "quote-details",
    templateUrl: "app/quote/quote-details.component.html"

})

export class QuoteDetailsComponent implements OnInit {

    @Input() quote: any;
    @Input() user: any;


    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {



    }
    ngOnInit() {



    }

    


};
