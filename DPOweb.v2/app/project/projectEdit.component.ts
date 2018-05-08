
import { Component, OnInit, ViewChild } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { NgForm } from '@angular/forms';

import 'rxjs/Rx';

import { parseDate } from '@telerik/kendo-intl';

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
    selector: 'project-edit',
    templateUrl: 'app/project/projectEdit.component.html'
})
export class ProjectEditComponent implements OnInit {
    @ViewChild('projectEditForm') public projectEditForm: NgForm;

    public previousUrl: string;

    public action: any;

    public formIsValid: any = false;

    public project: any;

    public user: any;
    public canViewPipelineData: boolean = false;
    public canEditPipelineData: boolean = false;

    public suggestedAddress: any;
    public useSuggestedAddress: boolean = false;
    //public ignoreAddressValidation: boolean = false;

    public defaultItem: { text: string, value: number } = { text: "Select ...", value: null };

    public projectDate: any;

    constructor(private router: Router, private route: ActivatedRoute, private commonSvc: CommonService,
        private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private enums: Enums, private systemAccessEnum: SystemAccessEnum, private http: Http,
        private projectSvc: ProjectService, private addressSvc: AddressService) {

        router.events
            .filter(event => event instanceof NavigationEnd)
            .subscribe((e: any) => {
                this.previousUrl = e.url;
            });

        this.action = this.route.snapshot.url[0].path;

        this.project = this.route.snapshot.data['projectModel'].model;
        this.user = this.route.snapshot.data['currentUser'].model;
        // this.project.projectDate = new Date(Date.parse(this.project.projectDate));

        if (this.action == "projectCreate") {
            this.projectDate = new Date();
            this.project.bidDate = null;
            this.project.estimatedClose = null;
            this.project.estimatedDelivery = null;
            this.project.projectStatusTypeId = 1;
        } else {
            this.projectDate = new Date(Date.parse(this.project.projectDate));
            this.project.bidDate = new Date(Date.parse(this.project.bidDate));
            this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
            this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));

        }


    }

    ngOnInit() {
        this.validateForm();
        this.canViewPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewPipelineData);
        this.canEditPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditPipelineData);
    }

    ngDoCheck() {
        //if (this.projectEditForm.invalid) {
        //    $("#projectEditSubmitBtn").attr("disabled", "disabled");
        //} else {
        //    $("#projectEditSubmitBtn").removeAttr("disabled");
        //}

        console.log("ProjectEditForm.invalid: " + this.projectEditForm.invalid);

        if (this.formIsValid == false) {
            $("#projectEditSubmitBtn").attr("disabled", "disabled");
        } else {
            $("#projectEditSubmitBtn").removeAttr("disabled");
        }
    }

    validateForm() { //custom
        if (this.projectEditForm.invalid) {
            this.formIsValid = false;
        }
        else {
            this.formIsValid = true;
        }
    }

    public projectNameChange(event: any) {
        this.project.name = event;
        setTimeout(this.validateForm.bind(this), 200);
    }

    public constructionTypeChange(event: any) {
        //this.project.constructionTypeId = event.value;
        setTimeout(this.validateForm.bind(this), 200);
    }

    public projectStatusTypeChange(event: any) {
        //this.project.projectStatusTypeId = event.value;
        setTimeout(this.validateForm.bind(this), 200);
    }

    public projectTypeChange(event: any) {
        //this.project.projectTypeId = event.value;
        this.setDefaultDates();
        setTimeout(this.validateForm.bind(this), 200);
    }

    public projectOpenStatusTypeChange(event: any) {
        //this.project.projectOpenStatusTypeId = event.value;
        this.setDefaultDates();
        setTimeout(this.validateForm.bind(this), 200);
    }

    public verticalMarketTypeChange(event: any) {
        //this.project.verticalMarketTypeId = event.value;
        //this.validateForm();
        setTimeout(this.validateForm.bind(this), 200);
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

    // If project type && open status selected and no dates have been set 
    public setDefaultDates() {
        if (this.project.projectTypeId && this.project.projectOpenStatusTypeId && !this.project.bidDate && !this.project.estimatedClose && !this.project.estimatedDelivery) {
            if (this.project.projectTypeId == "6") { // Design/Build
                // Design build 	
                //  1. Bid: should be same month as registration date
                //  2. Close: Add 60 days to bid date	
                //  3. Delivery: Add 30 days to estimated close
                //  4. Many time the customer marks D/B as budget or design and they should be all bidding 
                this.project.bidDate = new Date(this.projectDate);
                this.project.estimatedClose = new Date(this.project.bidDate);
                this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);

                this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);
            } else {


                switch (this.project.projectOpenStatusId) {
                    case "1": // Budget
                    case "2": // Design
                        // 1. Bid: Add 9 months to reg date	
                        // 2. Close: Add 60 days to bid date
                        // 3. Delivery: Add 30 days to close date	
                        this.project.bidDate = new Date(this.projectDate);
                        this.project.bidDate.setDate(this.project.bidDate.getDate() + (30 * 9));

                        this.project.estimatedClose = new Date(this.project.bidDate);
                        this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);

                        this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                        this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);

                        break;

                    case "3": // Quote
                    default:
                        // 1. Bid: Quote 
                        // 2. Close: Add 60 days 
                        // 3. Delivery: Add 30 days

                        this.project.bidDate = new Date(this.projectDate);

                        this.project.estimatedClose = new Date(this.project.bidDate);
                        this.project.estimatedClose.setDate(this.project.estimatedClose.getDate() + 60);

                        this.project.estimatedDelivery = new Date(this.project.estimatedClose);
                        this.project.estimatedDelivery.setDate(this.project.estimatedDelivery.getDate() + 30);
                        break;
                }

            }
        }

    }

    public cancel() {
        if (this.action == "projectCreate") {
            this.router.navigateByUrl("/projects");
        } else if (this.action == "projectEdit") {
            this.router.navigateByUrl("/project/" + this.project.projectId);
        }
    }

    public submit() {
        this.suggestedAddress = null;
        this.loadingIconSvc.Start(jQuery("#content"));

        this.projectSvc.postProjectAndVerifyAddress(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#content"));

                    this.project.projectId = resp.model.projectId;

                    if (this.action == "projectCreate") {
                        this.router.navigateByUrl("/quoteCreate/" + this.project.projectId);
                    } else if (this.action == "projectEdit") {
                        this.router.navigateByUrl("/project/" + this.project.projectId);
                    }

                } else {
                    this.loadingIconSvc.Stop(jQuery("#content"));

                    this.toastrSvc.displayResponseMessagesFadeOut(resp);

                    if (resp.model.error) {//Address service error
                        jQuery("#shipToAddressLink").click();
                        if (resp.model.isAddressValid == false) {// Address is not verified
                            jQuery("#shippingAddressDialog").modal({ backdrop: 'static', keyboard: false });
                        } else if (resp.model.addresses.length > 0) {// Address does not match suggested address
                            this.suggestedAddress = resp.model.addresses[0];
                            jQuery("#shippingAddressDialog").modal({ backdrop: 'static', keyboard: false });
                        }

                    }

                }

            },
            err => {
                this.loadingIconSvc.Stop(jQuery("#content"));
                console.log("Error: ", err)
            }
            );
    }

    //public onTabSelect(e: any) {
    //    console.log(e);
    //}

    //get diagnostic() { return JSON.stringify(this.project); }

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

    postProject() {// this will by pass address validation

        this.loadingIconSvc.Start(jQuery("#content"));

        this.projectSvc.postProject(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.project.projectId = resp.model.projectId
                    if (this.action == "projectCreate") {
                        this.router.navigateByUrl("/quoteCreate/" + this.project.projectId);
                    } else if (this.action == "projectEdit") {
                        this.router.navigateByUrl("/project/" + this.project.projectId);
                    }

                } else {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => {
                this.loadingIconSvc.Stop(jQuery("#content"));
                console.log("Error: ", err)
            }
            );
    }

};

