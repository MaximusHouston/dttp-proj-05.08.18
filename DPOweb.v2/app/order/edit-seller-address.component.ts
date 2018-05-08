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
import { AddressService } from '../address/services/address.service';

declare var jQuery: any;


@Component({
    selector: 'edit-seller-address',
    templateUrl: 'app/order/edit-seller-address.component.html'
})
export class EditSellerAddressComponent implements OnInit {

    @Input() project: any;
    public _project: any;


    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http,
        private projectSvc: ProjectService, private addressSvc: AddressService) {


    }

    ngOnInit() {
        this._project = jQuery.extend(true, {}, this.project);
    }

    public countryCodeChange(event: any) {
        var countryCode = event;
        this.addressSvc.getStatesByCountry(countryCode)
            .subscribe(
            (resp: any) => {
                if (resp.isok) {
                    var states = resp.model;
                    this.project.sellerAddress.states.items = resp.model.items;
                    this.project.sellerAddress.stateId = null;
                }
            },
            error => {
                console.log("Error: " + error);
            }
            );
    }

    stateChange(value: any) {
        if (value != null && value != 0) {
            for (var i = 0; i < this.project.sellerAddress.states.items.length; i++) {
                if (this.project.sellerAddress.states.items[i].value == value) {
                    this.project.sellerAddress.stateName = this.project.sellerAddress.states.items[i].text;
                }
            }
        } else {
            this.project.sellerAddress.stateName = null;
        }

    }

    cancel() {

        this.project.sellerName = this._project.sellerName;
        this.project.sellerAddress.addressLine1 = this._project.sellerAddress.addressLine1;
        this.project.sellerAddress.addressLine2 = this._project.sellerAddress.addressLine2;
        this.project.sellerAddress.countryCode = this._project.sellerAddress.countryCode;
        this.project.sellerAddress.location = this._project.sellerAddress.location;
        this.project.sellerAddress.stateId = this._project.sellerAddress.stateId;
        this.project.sellerAddress.postalCode = this._project.sellerAddress.postalCode;

        this.stateChange(this.project.sellerAddress.stateId);
    }

    submit() {
        this.loadingIconSvc.Start(jQuery("#editSellerAddressModal"));

        this.projectSvc.postProject(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#editSellerAddressModal"));
                    this.toastrSvc.displayResponseMessagesFadeOut(resp);
                    $('#editSellerAddressModal').modal('hide');

                } else {
                    this.loadingIconSvc.Stop(jQuery("#editSellerAddressModal"));
                    this.toastrSvc.displayResponseMessagesFadeOut(resp);
                    $('#editSellerAddressModal').modal('hide');

                }

            },
            err => {
                this.loadingIconSvc.Stop(jQuery("#editSellerAddressModal"));
                $('#editSellerAddressModal').modal('hide');
                console.log("Error: ", err)
            }
            );
    }

};

