
import { Component, OnInit, Input, Output, EventEmitter, ViewChildren } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProjectService } from '../projects/services/project.service';
import { QuoteService } from './services/quote.service';
declare var jQuery: any;

@Component({
    selector: 'quote-edit',
    templateUrl: 'app/quote/quote-edit.component.html'

})
export class QuoteEditComponent implements OnInit {

    //@Input() project: any;
    //@Input() projectEditForm: any;

    public action: any;
    public user: any;
    public quote: any;
    public canRequestCommission: any;


    public defaultItem: { text: string, value: number } = { text: "Select ...", value: null };

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private http: Http,
        private projectSvc: ProjectService, private quoteSvc: QuoteService, private enums: Enums) {

        this.action = this.route.snapshot.url[0].path;
        this.user = this.route.snapshot.data['currentUser'].model;
        this.quote = this.route.snapshot.data['quoteModel'].model;
    }

    ngOnChanges() {

    }

    ngOnInit() {
        this.canRequestCommission = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.RequestCommission);

    }

    cancel() {
        if (this.action == "quoteCreate") {
            this.router.navigateByUrl("/project/" + this.quote.projectId);
        } else if (this.action == "quoteEdit") {
            this.router.navigateByUrl("/quote/" + this.quote.quoteId + "/existingRecord");
        }
    }

    submit() {
        this.loadingIconSvc.Start(jQuery("#content"));
        this.quoteSvc.postQuote(this.quote)
            .subscribe(resp => {
                //debugger
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.quote = resp.model;
                    if (this.action == "quoteCreate") {
                        this.router.navigateByUrl("/quote/" + this.quote.quoteId + "/newRecord");
                    } else {
                        this.router.navigateByUrl("/quote/" + this.quote.quoteId + "/existingRecord");
                    }

                } else {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                }
                
            },
            err => {
                this.loadingIconSvc.Start(jQuery("#content"));
                console.log("Error: ", err)
            });
    }



};

