import { Component, OnInit, Input, Output, EventEmitter, ViewChildren, ViewChild } from '@angular/core';
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
import { OrderService } from './services/order.service';
declare var jQuery: any;

import { UploadModule } from '@progress/kendo-angular-upload';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { UploadEvent } from '@progress/kendo-angular-upload';
import { SuccessEvent } from '@progress/kendo-angular-upload';
import { FileInfo } from '@progress/kendo-angular-upload';

@Component({
    selector: 'order-form',
    templateUrl: 'app/order/order-form.component.html',
    styleUrls: ["app/order/order-form.component.css"],
})
export class OrderFormComponent implements OnInit {
    //@ViewChild('datepicker') datepicker: any;
    public user: any;
    public order: any;
    public recordState: any;

    //public poAttachment: any;
    public poUploadUrl: any;
    public poAttachment: Array<FileInfo>;

    //public projectInfoIsValid : boolean = true;
    //public orderInfoIsValid : boolean = true;

    public releaseDateMin: any;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http,
        private projectSvc: ProjectService, private orderSvc: OrderService) {

        //this.activeTab = this.route.snapshot.url[0].path;

        this.user = this.route.snapshot.data['currentUser'].model;
        this.order = this.route.snapshot.data['orderFormModel'].model;

        this.recordState = this.route.snapshot.paramMap.get('recordState');

        if (this.recordState == "new") {
            if (this.order.hasConfiguredModel) {
                this.releaseDateMin = new Date();
                this.releaseDateMin.setDate((new Date()).getDate() + 28);

                this.order.orderReleaseDate = this.releaseDateMin;
                //this.order.orderReleaseDate = new Date();
                //this.order.orderReleaseDate.setDate((new Date()).getDate() + 28);
            } else {
                this.order.orderReleaseDate = null;
            }
        } else {// submitted
            this.order.orderReleaseDate = new Date(Date.parse(this.order.orderReleaseDate));
        }


        //this.order.orderReleaseDate = new Date(Date.parse(this.order.orderReleaseDate)); // 1/1/1
        //this.order.orderReleaseDate = new Date();// current date

        this.poUploadUrl = "/api/Order/UploadPOAttachment";
    }

    ngOnInit() {

        //this.recordState = this.route.snapshot.paramMap.get('recordState');

    }

    public selectEventHandler(e: SelectEvent) {
        //console.log("File selected");
        this.order.poAttachmentFileName = e.files[0].name;

    }
    public uploadEventHandler(e: UploadEvent) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.order.quoteId,
        };
    }

    successEventHandler(e: SuccessEvent) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
            //this.toastrSvc.Success("Quote '" + this.quote.title + "' has been updated.");

            //this.reloadDataEvent.emit();

            //$('button.close[data-dismiss=modal]').click();

        }


    }


    errorEventHandler(e: any) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    }

    //====This is to fix kendo date picker view jump on open===
    public datePickerOpen(): void {
        setTimeout(this.jumpToDatePicker.bind(this), 50); // wait 0.05 sec
    }

    public jumpToDatePicker() {
        //location.href = "#orderReleaseDate";
        //this.datepicker.open();
        document.getElementById("orderReleaseDate").scrollIntoView();
    }
    //======================================================


    submit() {
        this.order.shipToName = this.order.project.shipToName;
        var self = this;

        bootbox.confirm("<p>Are you sure you want to submit Order? <br/>No further changes will be available on this project after it has been submitted.</p>", function (result) {
            if (result) {
                
                self.loadingIconSvc.Start(jQuery("#main-container"));
                //Post Order
                self.orderSvc.postOrder(self.order)
                    .subscribe(
                    resp => {
                        if (resp.isok) {
                            self.loadingIconSvc.Stop(jQuery("#main-container"));
                            //Send order email notification
                            self.orderSvc.sendOrderEmail(self.order).subscribe();
                            if (self.order.hasConfiguredModel) {
                                bootbox.alert("<p>Some message for LC configured Products</p>", function () {
                                    self.router.navigateByUrl("/quote/" + self.order.quoteId + "/existingRecord");
                                });
                            } else {
                                bootbox.alert("<p>Thank you for submitting the order. Your Daikin Customer Service Representative will review the order and get back to you within 24 hours.<br/> <br/>To cancel the order, please contact your Daikin Customer Service Representative.</p>", function () {
                                    self.router.navigateByUrl("/quote/" + self.order.quoteId + "/existingRecord");
                                });
                            }

                        } else {
                            self.loadingIconSvc.Stop(jQuery("#main-container"));
                            self.toastrSvc.displayResponseMessages(resp);
                        }
                    },
                    err => {
                        self.loadingIconSvc.Stop(jQuery("#main-container"));
                        console.log("Error: ", err)
                    }
                    );
            }
        });
    }

    //validate() {
    //    this.validateProjectInfo();
    //    this.validateOrderDetails();
    //}

    //validateProjectInfo() {
    //}

    //validateOrderDetails() {
    //    if (this.order.orderReleaseDate == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.poAttachment == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.order.poNumber == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //    if (this.order.comments == null) {
    //        this.orderInfoIsValid = false;
    //    }
    //}

    stateChange(value: any) {
        for (var i = 0; i < this.order.project.shipToAddress.states.items.length; i++) {
            if (this.order.project.shipToAddress.states.items[i].value == value) {
                this.order.project.shipToAddress.stateName = this.order.project.shipToAddress.states.items[i].text;
            }
        }
    }

};

