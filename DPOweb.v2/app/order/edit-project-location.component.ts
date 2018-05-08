import { Component, OnInit, Input, Output, EventEmitter, ViewChildren } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';

import 'rxjs/Rx';

import { CommonService } from '../shared/services/common.service';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProjectService } from '../projects/services/project.service';
import { AddressService } from '../address/services/address.service';

declare var jQuery: any;


@Component({
    selector: 'edit-project-location',
    templateUrl: 'app/order/edit-project-location.component.html',
    styleUrls: ["app/order/edit-project-location.component.css"],
})
export class EditProjectLocationComponent implements OnInit {

    @Input() project: any;
    public _project: any;

    public suggestedAddress: any;
    public useSuggestedAddress: boolean = false;


    constructor(private router: Router, private route: ActivatedRoute, private commonSvc: CommonService, private toastrSvc: ToastrService,
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
                    this.project.shipToAddress.states.items = resp.model.items;
                    this.project.shipToAddress.stateId = null;
                }
            },
            error => {
                console.log("Error: " + error);
            }
            );
    }

    stateChange(value: any) {
        if (value != null && value != 0) {
            for (var i = 0; i < this.project.shipToAddress.states.items.length; i++) {
                if (this.project.shipToAddress.states.items[i].value == value) {
                    this.project.shipToAddress.stateName = this.project.shipToAddress.states.items[i].text;
                }
            }
        } else {
            this.project.shipToAddress.stateName = null;
        }
        
    }

    cancel() {
        //this.project = this._project;
        //this.project = jQuery.extend(true, {}, this._project);
        //this.project = Object.assign({}, this._project);

        this.project.shipToName = this._project.shipToName;
        this.project.shipToAddress.addressLine1 = this._project.shipToAddress.addressLine1;
        this.project.shipToAddress.addressLine2 = this._project.shipToAddress.addressLine2;
        this.project.shipToAddress.countryCode = this._project.shipToAddress.countryCode;
        this.project.shipToAddress.location = this._project.shipToAddress.location;
        this.project.shipToAddress.stateId = this._project.shipToAddress.stateId;
        this.project.shipToAddress.postalCode = this._project.shipToAddress.postalCode;
        this.project.squareFootage = this._project.squareFootage;
        this.project.numberOfFloors = this._project.numberOfFloors;

        this.stateChange(this.project.shipToAddress.stateId);
    }

    reEnterAddress() {
    }


    continue() {
        if (this.useSuggestedAddress) {

            this.project.shipToAddress.addressLine1 = this.suggestedAddress.line1;
            this.project.shipToAddress.addressLine2 = this.suggestedAddress.line2;
            this.project.shipToAddress.location = this.suggestedAddress.city;
            this.project.shipToAddress.stateName = this.suggestedAddress.stateProvince;
            this.project.shipToAddress.postalCode = this.suggestedAddress.zipCode;

            var self = this;
            this.commonSvc.getStateIdByStateCode(this.suggestedAddress.stateProvince)
                .subscribe(
                resp => {
                    if (resp.isok) {
                        self.project.shipToAddress.stateId = resp.model
                        self.postProject();
                    } else {
                        self.toastrSvc.displayResponseMessages(resp);
                    }
                },
                err => console.log("Error: ", err)
                );



        } else {
            this.postProject();
        }

    }

    submit() {
        this.suggestedAddress = null;

        this.loadingIconSvc.Start(jQuery("#editShipToAddressModal"));

        this.projectSvc.postProjectAndVerifyAddress(this.project)
            .subscribe(
                resp => {
                    if (resp.isok) {

                        this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                        this.toastrSvc.displayResponseMessagesFadeOut(resp);
                        $('#editShipToAddressModal').modal('hide');

                    } else {
                        this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));

                        this.toastrSvc.displayResponseMessagesFadeOut(resp);

                        if (resp.model.error) {//Address service error
                            
                            if (resp.model.isAddressValid == false) {// Address is not verified
                                jQuery("#verifyAddressDialog").modal({ backdrop: 'static', keyboard: false });
                            } else if (resp.model.addresses.length > 0) {// Address does not match suggested address
                                this.suggestedAddress = resp.model.addresses[0];
                                jQuery("#verifyAddressDialog").modal({ backdrop: 'static', keyboard: false });
                            }

                        }

                    }

                },
                err => {
                    this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                    console.log("Error: ", err)
                }
            );
    }

    postProject() {// this will by pass address validation

        this.loadingIconSvc.Start(jQuery("#editShipToAddressModal"));

        this.projectSvc.postProject(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                    $('#editShipToAddressModal').modal('hide');
                } else {
                    this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                    this.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => {
                this.loadingIconSvc.Stop(jQuery("#editShipToAddressModal"));
                console.log("Error: ", err)
            }
            );
    }

};

