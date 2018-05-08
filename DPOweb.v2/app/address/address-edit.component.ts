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

import { ProjectService } from '../projects/services/project.service';
declare var jQuery: any;

@Component({
    selector: 'address-edit',
    templateUrl: 'app/address/address-edit.component.html'
})

export class AddressEditComponent implements OnInit {
    @Input() address: any;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http,
        private projectSvc: ProjectService) {


    }

    ngOnInit() {
        //this.quoteSvc.getQuoteItems(this.quoteId).then((resp: any) => {
        //    if (resp.isok) {
        //        this.quoteItems = resp.model;
        //    }
        //}).catch(error => {
        //    console.log(error);
        //});
    }
}