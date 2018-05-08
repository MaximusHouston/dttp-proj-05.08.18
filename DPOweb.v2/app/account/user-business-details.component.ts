//TODO: This component is not used because signUpForm valiation does not work

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from './services/account.service';

declare var jQuery: any;

@Component({
    selector: "user-business-details",
    templateUrl: "app/account/user-business-details.component.html"

})

export class UserBusinessDetailsComponent implements OnInit {
    @Input() user: any;
    public business: any;
    public businessTypeDLLDisabled = false;
    public foundBusinessAddress = false;
    public showAccountIdSearch = false;
    public showDakinAccRadioBtn = false;
    public useBusinessAddress = false;
    public hasDaikinAccount = false;
    public defaultItem: { text: string, value: string } = { text: "Select...", value: null };
    public phoneNumberMask: string = "(000) 000-0000";

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {
      
    }

    ngOnInit() {
        this.setupSearchBusiness();
    }

    //public searchBusiness() {
    //    this.accountSvc.businessAddressLookup({ accountId: "A11198" }).then((resp: any) => {
    //        this.toastrSvc.displayResponseMessages(resp);
    //        if (resp.isok) {
    //            debugger
    //            //this.user = resp.model;
    //        }
    //    });
    //}

    public setupSearchBusiness() {
        var self = this;

        jQuery("#businessSearchBox").keyup(function (event: any) {
            event.stopImmediatePropagation();
            var value = jQuery("#businessSearchBox")[0].value;
            if (value) {
                self.businessTypeDLLDisabled = true;
            } else {
                self.businessTypeDLLDisabled = false;
                self.useBusinessAddress = false;
            }
            if (event.keyCode == 13) {// enter key
                jQuery("#businessSearchBtn").click();
            }

        });

        jQuery("#businessSearchBtn").click(function (event: any) {
            event.stopImmediatePropagation();

            var value = jQuery("#businessSearchBox")[0].value;
            if (value) {
                self.accountSvc.businessAddressLookup(value).then(self.businessAddressLookupCallback.bind(self));

            } else {
                self.foundBusinessAddress = false;
                self.useBusinessAddress = false;
            }
        });
    }

    public businessAddressLookupCallback(resp: any) {
        if (resp.isok) {
            if (resp.model.accountId != null) {
                this.business = resp.model;
                this.user.business.accountId = resp.model.accountId;
                this.foundBusinessAddress = true;
                if (this.useBusinessAddress) {
                    this.UseBusinessAddress();
                }
            } else {
                this.toastrSvc.Warning("Business not found!");
                this.foundBusinessAddress = false;
                this.useBusinessAddress = false;
            }
        }
    }

    public UseBusinessAddress() {
        if (this.useBusinessAddress) {
            this.user.address = this.business.address;
            this.user.address.stateId = this.business.address.stateId.toString();
            this.user.contact = this.business.contact;
        }
        
    }

    public BusinessTypeChange(selectedItem: any) {
        if (selectedItem.value == this.enums.BusinessTypeEnum.Daikin
                || selectedItem.value == this.enums.BusinessTypeEnum.Distributor
                || selectedItem.value == this.enums.BusinessTypeEnum.ManufacturerRep) {
            this.showAccountIdSearch = true;
            this.showDakinAccRadioBtn = false;
            this.hasDaikinAccount = true;
            $('#businessAddressLabel').text("USER ADDRESS DETAILS");
        } else {
            this.showAccountIdSearch = false;
            this.foundBusinessAddress = false;
            this.showDakinAccRadioBtn = true;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
        //this.rowItem.furnace_Model = selectedItem.value;
        //this.furnaceSelectedEvent.emit(selectedItem);


    }

    public HasDaikinAccountChange(event: any) {
        if (event == "true") {// Yes
            this.showAccountIdSearch = true;
            $('#businessAddressLabel').text("USER ADDRESS DETAILS");
        } else {// No
            this.showAccountIdSearch = false;
            this.foundBusinessAddress = false;
            this.useBusinessAddress = false;
            this.businessTypeDLLDisabled = false;
            this.user.accountId = null;
            this.user.business.accountId = null;
            $('#businessAddressLabel').text("BUSINESS ADDRESS");
        }
    }

    //public UseBusinessAddress(event: any) {
    //    debugger
    //}



};