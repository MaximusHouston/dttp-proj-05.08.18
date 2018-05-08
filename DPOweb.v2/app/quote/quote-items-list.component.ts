import { Component, OnInit, Input, Output, EventEmitter, ViewChildren, QueryList, Inject, TemplateRef } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';



import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { Observable } from 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from './services/quote.service';

//import { QuoteItem } from './quoteItems';
//import { QuoteItems } from './quoteItems';

import { TagEditPopupComponent } from './tag-edit-popup.component';
import { OptionItemsComponent } from './option-items.component';

import {
    DialogService,
    DialogRef,
    DialogCloseResult
} from '@progress/kendo-angular-dialog';


declare var jQuery: any;

@Component({
    selector: "quote-items-list",
    templateUrl: "app/quote/quote-items-list.component.html"

})

export class QuoteItemsListComponent implements OnInit {

    @Input() quote: any;
    @Input() user: any;
    @Input() quoteItems: any;
    @Output() reloadDataEvent: EventEmitter<any> = new EventEmitter();
    @Output() reloadQuoteEvent: EventEmitter<any> = new EventEmitter();
    //public originalQuoteItems: any = [];
    @ViewChildren(OptionItemsComponent) OptionItemsComponents: QueryList<OptionItemsComponent>;

    public gridIsModified: boolean = false;
    //public tagEditorOpened: boolean = false;

    //private editTagPopup: any;


    //for tag Editor
    public quoteItem: any; 
    public oldTagsValue: any;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService,
        private dialogSvc: DialogService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums,
        private formBuilder: FormBuilder) {



    }
    ngOnInit() {
        //this.originalQuoteItems = this.quoteItems.items;

        //this.originalQuoteItems = Object.assign({}, this.quoteItems.items);

        //this.originalQuoteItems = Object.create(this.quoteItems.items);

        //this.cloneQuoteItems();
        //this.originalQuoteItems = this.cloneQuoteItems(this.quoteItems.items);



    }

    ngAfterViewChecked() {
        if (!this.gridIsModified) {
            jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
        }

    }

    public validateQuantity(event: any) {
        this.gridIsModified = true;

        jQuery("#quoteItemsGrid .k-grid-toolbar").show();

        //alert("this.quoteItems.items[0].quantity: " + this.quoteItems.items[0].quantity
        //    + "\n this.originalQuoteItems[0].quantity: " + this.originalQuoteItems[0].quantity)

        var value = parseFloat(event.target.value);

        if (value == null || isNaN(value)) {

            event.target.value = 0;
        } else if ((value < 0) || (Math.floor(value) != value)) {

            event.target.value = 0;
            this.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        }
    }

    //public onStateChange(state: State) {
    //    //this.gridState = state;

    //    //this.editService.read();
    //}




    public saveChanges(grid: any) {
        var self = this;
        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));

        this.quoteSvc.adjustQuoteItems(this.quoteItems).then((resp: any) => {
            if (resp.isok) {

                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));

                //reload QuoteItems Grid if needed
                //for (let message of resp.messages.items) {
                //    if (message.type == 30 && message.text == "Item(s)-Removed") {
                //        self.reloadQuoteItems();

                //        break;
                //    }
                //}

                //Update optionItems quantity
                //self.optionItemsComponent.loadOptionItems(); // use ViewChildren instead

                //self.reloadQuoteEvent.emit();

                self.reloadDataEvent.emit();

                self.toastrSvc.displayResponseMessages(resp);
                self.gridIsModified = false;
                jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            } else {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        });
    }

    //public saveChanges(grid: any) {
    //    var self = this;
    //    self.reloadDataEvent.emit();
                
    //}

    public cancelChanges(grid: any) {

        this.reloadQuoteItems();

        //this.quoteItems.items = Object.assign({}, this.originalQuoteItems);
        //this.quoteItems.items = this.originalQuoteItems;
    }

    public reloadQuote() {
        var self = this;

        //self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));

        self.quoteSvc.getQuote(self.quote.projectId, self.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.quote = resp.model;
            } else {
                //self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    public reloadQuoteItems() {
        var self = this;

        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));


        self.quoteSvc.getQuoteItemsModel(self.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.quoteItems = resp.model;
                self.gridIsModified = false;
                jQuery("#quoteItemsGrid .k-grid-toolbar").hide();
            } else {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(error => {
            //console.log('Retrieval error: ${error}');
            console.log(error);
        });




    }

    public productDetails(dataItem: any) {

        //this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });

        //this.accountSvc.resetBasketQuoteId()
        //    .then((resp: any) => {
        //        if (resp.isok) {
        //            this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });
        //        }
        //    })
        //    .catch(error => {
        //        console.log(error);
        //    });

        this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then((resp: any) => {
            if (resp.isok) {
                this.router.navigate(['/products', { outlets: { 'productDetails': [dataItem.productId] } }], { queryParams: { activeTab: "product-overview" } });
            } else {
                this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(error => {
            console.log('Retrieval error: ${error}');
            console.log(error);
        });


    }
    public hasOptionItems(dataItem: any, index: number): boolean {
        return dataItem.lineItemTypeId == 2;
    }


    //Tag Editor
    public openTagEditor(dataItem: any) {
        this.quoteItem = dataItem;
        this.oldTagsValue = this.quoteItem.tags;
        $("#tagEditor").modal();
    }

    public closeTagEditor() {
        this.quoteItem.tags = this.oldTagsValue;
    }

    public saveTagUpdate() {
        var self = this;

        var data = {
            'QuoteId': this.quoteItem.quoteId,
            'Items': [this.quoteItem]
        };

        self.loadingIconSvc.Start(jQuery("#quoteItemsGrid"));

        this.quoteSvc.adjustQuoteItems(data).then((resp: any) => {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
                self.toastrSvc.displayResponseMessages(resp);
                //self.oldTagsValue = self.quoteItem.tags;
            } else {
                self.toastrSvc.displayResponseMessages(resp);
                self.loadingIconSvc.Stop(jQuery("#quoteItemsGrid"));
            }
        }).catch(error => {
            console.log('Retrieval error: ${error}');
            console.log(error);
        });
    }

    //kendo dialog
    //public openTagEditor(dataItem: any, actionTemplate: TemplateRef<any>) {
    //    this.editTagPopup = this.dialogSvc.open({
    //        title: "UPDATE QUOTE ITEM TAGS",

    //        // Show component
    //        content: TagEditPopupComponent,
    //        //content: dataItem.tags,

    //        //actions: [
    //        //    { text: "Cancel" },
    //        //    { text: "Update", primary: true }
    //        //]

    //        actions: actionTemplate
    //    });

    //    const quoteItemInfo = this.editTagPopup.content.instance;
    //    quoteItemInfo.quoteItem = dataItem;

    //}

    //public openTagEditor(data: any) {
    //    this.tagEditorOpened = true;

    //}

    //kendo dialog
    //public closeTagEditor() {
    //    //this.tagEditorOpened = false;
    //    this.editTagPopup.close();
    //}

    //public saveTagUpdate(quoteItem: any) {
    //    //this.tagEditorOpened = false;
    //    alert(quoteItem.productNumber);
    //    this.editTagPopup.close();

    //}








};
