import { Component, OnInit, Input, Output, EventEmitter, ViewChildren, ViewChild } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProjectService } from '../projects/services/project.service';
import { DiscountRequestService } from './services/discountRequest.service';
declare var jQuery: any;

import { UploadModule } from '@progress/kendo-angular-upload';
import { SelectEvent } from '@progress/kendo-angular-upload';
import { UploadEvent } from '@progress/kendo-angular-upload';
import { SuccessEvent } from '@progress/kendo-angular-upload';
import { FileInfo } from '@progress/kendo-angular-upload';

@Component({
    selector: 'discount-request',
    templateUrl: 'app/discountRequest/discount-request.component.html',
    styleUrls: ["app/discountRequest/discount-request.component.css"]
})
export class DiscountRequestComponent implements OnInit {
    public user: any;
    public quoteId: any;
    public projectId: any;
    public discountRequest: any;
    public competitorQuoteFiles: Array<FileInfo>;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http,
        private projectSvc: ProjectService, private discountRequestSvc: DiscountRequestService) {

        this.user = this.route.snapshot.data['currentUser'].model;

        this.quoteId = this.route.snapshot.paramMap.get('quoteId');
        this.projectId = this.route.snapshot.paramMap.get('projectId');


        this.discountRequestSvc.getDiscountRequest(0, this.projectId, this.quoteId).subscribe(
            (resp: any) => {
                if (resp.isok){
                    this.discountRequest = resp.model;
                    this.discountRequest.requestedCommission = this.discountRequest.standardTotals.totalCommissionPercentage;
                    this.calculateStandardGrossProfit();
                    this.calculateRevisedGrossProfit();

                    this.discountRequest.orderPlannedFor = null;
                    this.discountRequest.project.estimatedDelivery = new Date(Date.parse(this.discountRequest.project.estimatedDelivery));
                }
                
            },
            error => {
                console.log("Error: "+ error);
            }
        );



    }

    ngOnInit() {
    }

    

    hasCompetitorPriceChange(event: any) {
        if (event == false)
        {
            this.discountRequest.competitorPrice = null;
        }
    }

    hasCompetitorQuoteChange(event: any) {
        
    }

    hasCompetitorLineComparsionChange(event: any) {

    }

    selectCompetitorQuoteFile(e: SelectEvent) {
        //this.competitorQuoteFiles = e.files;
        //this.discountRequest.competitorQuoteFile = e.files[0];
        this.discountRequest.competitorQuoteFileName = e.files[0].name;
        

    }

    selectLineComparsionFile(e: SelectEvent) {
        //this.discountRequest.competitorLineComparsionFile = e.files[0];
        this.discountRequest.competitorLineComparsionFileName = e.files[0].name;
    }

    public uploadEventHandler(e: UploadEvent) {
        console.log("File Upload");
        e.data = {
            QuoteId: this.discountRequest.quoteId,
        };
    }

    successEventHandler(e: SuccessEvent) {
        var self = this;
        if (e.response.ok == true) {
            console.log("The " + e.operation + " was successful!");
        }
    }

    errorEventHandler(e: any) {
        console.log("Error: " + e.response.statusText);
        this.toastrSvc.ErrorFadeOut(e.response.statusText);
    }


    //competitorQuoteFileChange(e: any) {
    //    //var files = e.srcElement.files;
    //    this.discountRequest.competitorQuoteFile = e.srcElement.files[0];

    //    //let formData: FormData = new FormData();
    //    //formData.append('competitorQuoteFile', e.srcElement.files[0], e.srcElement.files[0].name);
        
    //}

    //public test(event: any) {
    //    this.discountRequest.requestedDiscountVRV = event / 100;
    //}

    startupCostChange() {
        this.calculateRevisedTotalSell();
    }

    //Kendo numeric input
    //calculateDiscountAmountVRV(event: any) {
    //    //update Net Material 
    //    this.discountRequest.approvedTotals.netMaterialValueVRV = this.discountRequest.approvedTotals.totalNetVRV * (1 - this.discountRequest.requestedDiscountVRV);
    //    //update Net Multiplier
    //    this.discountRequest.approvedTotals.netMultiplierVRV = this.discountRequest.approvedTotals.netMaterialValueVRV / this.discountRequest.approvedTotals.totalListVRV;
    //    //show/update Discount Ammount
    //    this.discountRequest.approvedTotals.totalDiscountAmountVRV = this.discountRequest.approvedTotals.totalNetVRV - this.discountRequest.approvedTotals.netMaterialValueVRV;

    //    this.calculateTotalDiscount();
    //}

    //HTML numeric input
    calculateDiscountAmountVRV(value: any) {
        this.discountRequest.requestedDiscountVRV = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueVRV = this.discountRequest.approvedTotals.totalNetVRV * (1 - this.discountRequest.requestedDiscountVRV);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierVRV = this.discountRequest.approvedTotals.netMaterialValueVRV / this.discountRequest.approvedTotals.totalListVRV;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountVRV = this.discountRequest.approvedTotals.totalNetVRV - this.discountRequest.approvedTotals.netMaterialValueVRV;

        this.calculateTotalDiscount();
    }

    calculateDiscountAmountSplit(value: any) {
        this.discountRequest.requestedDiscountSplit = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueSplit = this.discountRequest.approvedTotals.totalNetSplit * (1 - this.discountRequest.requestedDiscountSplit);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierSplit = this.discountRequest.approvedTotals.netMaterialValueSplit / this.discountRequest.approvedTotals.totalListSplit;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountSplit = this.discountRequest.approvedTotals.totalNetSplit - this.discountRequest.approvedTotals.netMaterialValueSplit;

        this.calculateTotalDiscount();
    }

    calculateDiscountAmountUnitary(value: any) {
        this.discountRequest.requestedDiscountUnitary = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueUnitary = this.discountRequest.approvedTotals.totalNetUnitary * (1 - this.discountRequest.requestedDiscountUnitary);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierUnitary = this.discountRequest.approvedTotals.netMaterialValueUnitary / this.discountRequest.approvedTotals.totalListUnitary;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountUnitary = this.discountRequest.approvedTotals.totalNetUnitary - this.discountRequest.approvedTotals.netMaterialValueUnitary;

        this.calculateTotalDiscount();
    }

    calculateDiscountAmountLCPackage(value: any) {
        this.discountRequest.requestedDiscountLCPackage = value / 100;
        //update Net Material 
        this.discountRequest.approvedTotals.netMaterialValueLCPackage = this.discountRequest.approvedTotals.totalNetLCPackage * (1 - this.discountRequest.requestedDiscountLCPackage);
        //update Net Multiplier
        this.discountRequest.approvedTotals.netMultiplierLCPackage = this.discountRequest.approvedTotals.netMaterialValueLCPackage / this.discountRequest.approvedTotals.totalListLCPackage;
        //show/update Discount Ammount
        this.discountRequest.approvedTotals.totalDiscountAmountLCPackage = this.discountRequest.approvedTotals.totalNetLCPackage - this.discountRequest.approvedTotals.netMaterialValueLCPackage;

        this.calculateTotalDiscount();
    }

    calculateTotalDiscount() {
        this.discountRequest.approvedTotals.totalDiscountAmount =
            this.discountRequest.approvedTotals.totalDiscountAmountVRV +
            this.discountRequest.approvedTotals.totalDiscountAmountSplit +
            this.discountRequest.approvedTotals.totalDiscountAmountUnitary +
            this.discountRequest.approvedTotals.totalDiscountAmountLCPackage;

        this.discountRequest.approvedTotals.netMaterialValue =
            this.discountRequest.approvedTotals.netMaterialValueVRV +
            this.discountRequest.approvedTotals.netMaterialValueSplit +
            this.discountRequest.approvedTotals.netMaterialValueUnitary +
            this.discountRequest.approvedTotals.netMaterialValueLCPackage;

        this.discountRequest.approvedTotals.netMultiplier = this.discountRequest.approvedTotals.netMaterialValue / this.discountRequest.approvedTotals.totalList;

        this.discountRequest.requestedDiscount = this.discountRequest.approvedTotals.totalDiscountAmount / this.discountRequest.standardTotals.totalNet;

        this.calculateRevisedTotalSell();

    }

    calculateStandardGrossProfit() {
        //this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.standardTotals.totalCommissionPercentage * this.discountRequest.standardTotals.totalNet;
        this.discountRequest.standardTotals.totalCommissionAmount = this.discountRequest.standardTotals.totalCommissionPercentage * this.discountRequest.standardTotals.totalNet;
    }

    calculateRevisedGrossProfit() {
        this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.requestedCommission * this.discountRequest.approvedTotals.netMaterialValue;
        this.calculateRevisedTotalSell();
    }

    recalculateRevisedGrossProfit(value: any) {
        this.discountRequest.requestedCommission = value / 100;
        this.discountRequest.approvedTotals.totalCommissionAmount = this.discountRequest.requestedCommission * this.discountRequest.approvedTotals.netMaterialValue;
        this.calculateRevisedTotalSell();
    }

    calculateRevisedTotalSell() {
        this.discountRequest.approvedTotals.totalSell =
            this.discountRequest.quote.totalFreight +
            this.discountRequest.startUpCosts +
            this.discountRequest.approvedTotals.totalCommissionAmount +
            this.discountRequest.approvedTotals.netMaterialValue;
    }

    submit() {
        this.discountRequestSvc.postDiscountRequest(this.discountRequest).subscribe();
    }


    //====This is to fix kendo date picker view jump on open===
    public datePickerOpen(): void {
        setTimeout(this.jumpToDatePicker.bind(this), 10); // wait 0.01 sec
    }

    public jumpToDatePicker() {
        document.getElementById("orderIssueDate").scrollIntoView();
    }
    //======================================================

    //onSubmit() {
    //    //const req = new HttpRequest('POST', '/api/DiscountRequest/PostDiscountRequest', this.discountRequest.competitorQuoteFile, {
    //    //    reportProgress: false;
    //    //});

    //    //let formData: FormData = new FormData();
    //    //formData.append('competitorQuoteFile', this.discountRequest.competitorQuoteFile, this.discountRequest.competitorQuoteFile.name);
    //    //this.discountRequestSvc.postDiscountRequest(formData).subscribe();

    //}
}