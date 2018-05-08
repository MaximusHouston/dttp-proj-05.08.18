import { Component, OnInit, Input, Output, EventEmitter, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { UploadModule } from '@progress/kendo-angular-upload';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { UploadEvent } from '@progress/kendo-angular-upload';
import { SuccessEvent } from '@progress/kendo-angular-upload';
import { FileInfo } from '@progress/kendo-angular-upload';

import { Observable } from 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from './services/quote.service';


declare var jQuery: any;

@Component({
    selector: "import-products-dialog",
    templateUrl: "app/quote/import-products-dialog.component.html"

})

export class ImportProductsDialogComponent implements OnInit {
    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {
    }

    @Input() user: any;
    @Input() quote: any;
    @Input() quoteItems: any;

    @Output() reloadDataEvent: EventEmitter<any> = new EventEmitter();

    public importFiles: Array<FileInfo>;

    //public importUrl: string = "/ProjectDashboard/QuoteImport";
    public importUrl: string = "/api/Quote/QuoteImport";


    ngOnInit() {

    }


    public cancel() {
    }

    public import() {
    }
    public selectEventHandler(e: SelectEvent) {
        console.log("File selected");

    }
    public uploadEventHandler(e: UploadEvent) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.quote.quoteId,
            ProjectId: this.quote.projectId
        };
    }
    successEventHandler(e: SuccessEvent) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
            this.toastrSvc.Success("Quote '" + this.quote.title + "' has been updated.");
            
            this.reloadDataEvent.emit();
            
            $('button.close[data-dismiss=modal]').click();

        }


    }


    errorEventHandler(e: any) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    }

   

}