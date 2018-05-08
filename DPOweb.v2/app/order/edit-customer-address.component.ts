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
    selector: 'edit-customer-address',
    templateUrl: 'app/order/edit-customer-address.component.html'
    //styleUrls: ["app/order/edit-project-location.component.css"],
})
export class EditCustomerAddressComponent implements OnInit {
        
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
                    this.project.customerAddress.states.items = resp.model.items;
                    this.project.customerAddress.stateId = null;
                }
            },
            error => {
                console.log("Error: " + error);
            }
            );
    }

    stateChange(value: any) {
        if (value != null && value != 0) {
            for (var i = 0; i < this.project.customerAddress.states.items.length; i++) {
                if (this.project.customerAddress.states.items[i].value == value) {
                    this.project.customerAddress.stateName = this.project.customerAddress.states.items[i].text;
                }
            }
        } else {
            this.project.customerAddress.stateName = null;
        }

    }

    cancel() {

        this.project.dealerContractorName = this._project.dealerContractorName;
        this.project.customerName = this._project.customerName;
        this.project.customerAddress.addressLine1 = this._project.customerAddress.addressLine1;
        this.project.customerAddress.addressLine2 = this._project.customerAddress.addressLine2;
        this.project.customerAddress.countryCode = this._project.customerAddress.countryCode;
        this.project.customerAddress.location = this._project.customerAddress.location;
        this.project.customerAddress.stateId = this._project.customerAddress.stateId;
        this.project.customerAddress.postalCode = this._project.customerAddress.postalCode;

        this.stateChange(this.project.customerAddress.stateId);
    }

    submit() {
        this.loadingIconSvc.Start(jQuery("#editCustomerAddressModal"));

        this.projectSvc.postProject(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
                    this.toastrSvc.displayResponseMessagesFadeOut(resp);
                    $('#editCustomerAddressModal').modal('hide');

                } else {
                    this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
                    this.toastrSvc.displayResponseMessagesFadeOut(resp);
                    $('#editCustomerAddressModal').modal('hide');

                }

            },
            err => {
                this.loadingIconSvc.Stop(jQuery("#editCustomerAddressModal"));
                $('#editCustomerAddressModal').modal('hide');
                console.log("Error: ", err)
            }
            );
    }

};

