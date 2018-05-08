
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
import { AddressService } from './services/address.service';
declare var jQuery: any;

@Component({
    selector: 'address',
    templateUrl: 'app/address/address.component.html'

})
export class AddressComponent implements OnInit {
    //public project: any;

    @Input() project: any;
    @Input() address: any;
    //@Input() contactName: any; //Error: Cannot assign to a reference or variable
    //@Input() businessName: any;

    //no being used
    @Input() addressType: any;
    @Input() projectEditForm: any;
      

    //public selectedState: { text: string, value: number } = { text: null, value: null };

    public defaultItem: { text: string, value: number } = { text: "Select ...", value: null };

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private http: Http,
        private projectSvc: ProjectService, private addressSvc: AddressService) {

        //this.project = this.route.snapshot.data['projectModel'].model;
        //this.project.projectDate = new Date(Date.parse(this.project.projectDate));
        //this.project.bidDate = null;
        //this.project.estimatedClose = null;
        //this.project.estimatedDelivery = null;

    }

    ngOnInit() {

        var type = this.addressType;
        //this.selectedState.text = this.address.stateName;
        //this.selectedState.value = this.address.stateId;

    }

    public countryCodeChange(event: any) {
        var countryCode = event;
        this.addressSvc.getStatesByCountry(countryCode)
            .subscribe(
            (resp: any) => {
                if (resp.isok) {
                    var states = resp.model;
                    this.address.states.items = resp.model.items;
                    this.address.stateId = null;
                }
            },
            error => {
                console.log("Error: " + error);
            }
            );
    }

    stateChange(value: any) {
        for (var i = 0; i < this.address.states.items.length; i++) {
            if (this.address.states.items[i].value == value) {
                this.address.stateName = this.address.states.items[i].text;
            }
        }
    }

    //public cancel() {
    //}

    //public submit() {

    //}

    //public onTabSelect(e: any) {
    //    console.log(e);
    //}

    


};

