import { Component, OnInit, Input, Output, EventEmitter, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { Observable } from 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from '../quote/services/quote.service';
import { CommissionRequestService } from './services/commissionRequest.service';


declare var jQuery: any;

@Component({
    selector: "calculate-commission-dialog",
    templateUrl: "app/commissionRequest/calculate-commission-dialog.component.html"

})

export class CalculateCommissionDialogComponent implements OnInit {
    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService, private commissionRequestSvc: CommissionRequestService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {
    }

    @Input() user: any;
    @Input() quote: any;
    @Input() quoteItems: any;
    @Output() reloadQuoteEvent: EventEmitter<any> = new EventEmitter();
    
    public commissionRequest: any;
    //public originalvalues: any;

    ngOnInit() {
        this.getCommissionRequest();
      
    }

    public getCommissionRequest() {
        var self = this;
        this.commissionRequestSvc.getCommissionRequestModel(this.quote.projectId, this.quote.quoteId, this.quote.commissionRequestId, this.quote.commissionRequestStatusTypeId).then((resp: any) => {
            if (resp.isok) {
                self.commissionRequest = resp.model;
                //self.originalvalues = resp.model;

            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    } 

    public calculateDiscountAmountVRV(event: any) {
        var self = this;

        this.commissionRequest.totalNetVRV = this.commissionRequest.approvedTotalsCommission.totalListVRV * this.commissionRequest.requestedMultiplierVRV;
        var data = {
            MultiplierCategoryTypeId: 2,
            Multiplier: this.commissionRequest.requestedMultiplierVRV
        }

        this.commissionRequestSvc.getCommissionPercentage(data)
            .then((resp: any) => {
                if (resp.isok) {
                    self.commissionRequest.requestedCommissionPercentageVRV = resp.model.commissionPercentage;
                    self.commissionRequest.requestedCommissionVRV = self.commissionRequest.totalNetVRV * self.commissionRequest.requestedCommissionPercentageVRV/100;
                    self.commissionRequest.requestedNetMaterialValueVRV = self.commissionRequest.totalNetVRV - self.commissionRequest.requestedCommissionVRV;
                    self.commissionRequest.requestedNetMaterialMultiplierVRV = self.commissionRequest.requestedNetMaterialValueVRV / self.commissionRequest.approvedTotalsCommission.totalListVRV;
                    self.calculateTotals();
                }
            })
            .catch(error => {
                console.log(error);
            });
    }

    public calculateDiscountAmountSplit(event: any) {
        var self = this;

        this.commissionRequest.totalNetSplit = this.commissionRequest.approvedTotalsCommission.totalListSplit * this.commissionRequest.requestedMultiplierSplit;
        var data = {
            MultiplierCategoryTypeId: 1,
            Multiplier: this.commissionRequest.requestedMultiplierSplit
        }

        this.commissionRequestSvc.getCommissionPercentage(data)
            .then((resp: any) => {
                if (resp.isok) {
                    self.commissionRequest.requestedCommissionPercentageSplit = resp.model.commissionPercentage;
                    self.commissionRequest.requestedCommissionSplit = self.commissionRequest.totalNetSplit * self.commissionRequest.requestedCommissionPercentageSplit/100;
                    self.commissionRequest.requestedNetMaterialValueSplit = self.commissionRequest.totalNetSplit - self.commissionRequest.requestedCommissionSplit;
                    self.commissionRequest.requestedNetMaterialMultiplierSplit = self.commissionRequest.requestedNetMaterialValueSplit / self.commissionRequest.approvedTotalsCommission.totalListSplit;
                    self.calculateTotals();
                }
            })
            .catch(error => {
                console.log(error);
            });
    }

    public calculateDiscountAmountLCPackage(event: any) {
        var self = this;

        this.commissionRequest.totalNetLCPackage = this.commissionRequest.approvedTotalsCommission.totalListLCPackage * this.commissionRequest.requestedMultiplierLCPackage;
        var data = {
            MultiplierCategoryTypeId: 4,
            Multiplier: this.commissionRequest.requestedMultiplierLCPackage
        }

        this.commissionRequestSvc.getCommissionPercentage(data)
            .then((resp: any) => {
                if (resp.isok) {
                    self.commissionRequest.requestedCommissionPercentageLCPackage = resp.model.commissionPercentage;
                    self.commissionRequest.requestedCommissionLCPackage = self.commissionRequest.totalNetLCPackage * self.commissionRequest.requestedCommissionPercentageLCPackage / 100;
                    self.commissionRequest.requestedNetMaterialValueLCPackage = self.commissionRequest.totalNetLCPackage - self.commissionRequest.requestedCommissionLCPackage;
                    self.commissionRequest.requestedNetMaterialMultiplierLCPackage = self.commissionRequest.requestedNetMaterialValueLCPackage / self.commissionRequest.approvedTotalsCommission.totalListLCPackage;
                    self.calculateTotals();
                }
            })
            .catch(error => {
                console.log(error);
            });
    }

    public calculateDiscountAmountUnitary(event: any) {
        var self = this;

        //Todo: Why do we have to do this?
        //this.commissionRequest.requestedMultiplier = this.commissionRequest.requestedMultiplierUnitary;

        this.commissionRequest.totalNetUnitary = this.commissionRequest.approvedTotalsCommission.totalListUnitary * this.commissionRequest.requestedMultiplierUnitary;

        this.commissionRequestSvc.getUnitaryCommissionPercentage(this.commissionRequest)
            .then((resp: any) => {
                if (resp.isok) {
                    self.commissionRequest.requestedCommissionPercentageUnitary = resp.model.commissionPercentage;
                    self.commissionRequest.requestedCommissionUnitary = self.commissionRequest.totalNetUnitary * self.commissionRequest.requestedCommissionPercentageUnitary / 100;
                    self.commissionRequest.requestedNetMaterialValueUnitary = self.commissionRequest.totalNetUnitary - self.commissionRequest.requestedCommissionUnitary;
                    self.commissionRequest.requestedNetMaterialMultiplierUnitary = self.commissionRequest.requestedNetMaterialValueUnitary / self.commissionRequest.approvedTotalsCommission.totalListUnitary;
                    self.calculateTotals();
                }
            })
            .catch(error => {
                console.log(error);
            });

       
    }

    public calculateTotals() {
        this.commissionRequest.totalNet = this.commissionRequest.totalNetVRV + this.commissionRequest.totalNetSplit + this.commissionRequest.totalNetUnitary + this.commissionRequest.totalNetLCPackage;
        this.commissionRequest.requestedMultiplier = this.commissionRequest.totalNet / this.commissionRequest.approvedTotalsCommission.totalList;
        this.commissionRequest.requestedCommissionTotal = this.commissionRequest.requestedCommissionVRV + this.commissionRequest.requestedCommissionSplit + this.commissionRequest.requestedCommissionUnitary + this.commissionRequest.requestedCommissionLCPackage;
        this.commissionRequest.requestedCommissionPercentage = this.commissionRequest.requestedCommissionTotal / this.commissionRequest.totalNet * 100;
        this.commissionRequest.requestedNetMaterialValue = this.commissionRequest.requestedNetMaterialValueVRV + this.commissionRequest.requestedNetMaterialValueSplit + this.commissionRequest.requestedNetMaterialValueUnitary + this.commissionRequest.requestedNetMaterialValueLCPackage;
        this.commissionRequest.requestedNetMaterialValueMultiplier = this.commissionRequest.requestedNetMaterialValue / this.commissionRequest.approvedTotalsCommission.totalList;
        
    }
    public save() {
        var self = this;
        self.loadingIconSvc.Start(jQuery("#content"));
        this.commissionRequestSvc.postCommissionCalculation(this.commissionRequest)
            .then((resp: any) => {
                if (resp.isok) {
                    self.toastrSvc.displayResponseMessages(resp);
                    self.loadingIconSvc.Stop(jQuery("#content"));
                    self.reloadQuoteEvent.emit();
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                    self.loadingIconSvc.Stop(jQuery("#content"));
                }
            })
            .catch(error => {
                console.log(error);
                self.loadingIconSvc.Stop(jQuery("#content"));
            });

    }
    public cancel() {
        //this.commissionRequest = this.originalvalues;
        this.getCommissionRequest();
    }



}