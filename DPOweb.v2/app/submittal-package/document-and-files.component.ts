
import { Component, OnInit, Input, Output, EventEmitter, ViewEncapsulation, ViewChild } from '@angular/core';
import { NgForm, FormControl, FormArray } from '@angular/forms';
import { FormBuilder, FormGroup } from '@angular/forms';
 
import { SubmittalPackageModel } from '../submittal-package/models/submittal-package';
import { QuoteItemListModel } from '../shared/models/quoteItemList';

import { SubmittalPackageService } from './services/submittal-package.service';
import { NgProgress } from 'ngx-progressbar';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

import { ModalComponent } from '../shared/modal/modal.component';

declare var jQuery: any;

@Component({
    selector: 'documents-and-files',
    templateUrl: 'app/submittal-package/documents-and-files.component.html',
    styleUrls: ['app/submittal-package/documents-and-files.component.css'],    
    encapsulation: ViewEncapsulation.None
})
export class DocumentAndFilesComponent implements OnInit {
    @ViewChild('componentInsideModal') componentInsideModal: ModalComponent;
    @BlockUI() blockUI: NgBlockUI;

    @Input() quoteItemsList: QuoteItemListModel[];
    @Input() possibleDocsList: any;
    @Input() quoteId: string;
    @Input() projectId: string

    private files: string[];

    //public submittalPackageForm: FormGroup;     
    //private fb: FormBuilder,
    constructor(private submittalService: SubmittalPackageService,
        private ngProgress: NgProgress) { }

    ngOnInit() {
        //this.submittalPackageForm = new FormGroup({
        //    referenceCheckbox: new FormControl(),
        //    submittalSheetsCheckbox: new FormControl(),
        //    operationManualCheckbox: new FormControl()
        //});
    }
        
    checkAllColumns(event: any, rowId: number) {
        if (event.target.checked)
            jQuery("input[rowIndex='" + rowId + "']").prop('checked', true);
        else {
            jQuery("input[rowIndex='" + rowId + "']").prop('checked', false);
            jQuery("input[rowIndex='0']").prop('checked', false);
        }           
    }

    uncheckRowAndColumnHeaderCheckBox(rowId: number, colIdentifier: string) {
        if (colIdentifier == 'submittalSheets') {
            jQuery("input[name='chkReferenceRow'][rowIndex='" + rowId + "']").prop('checked', false);
            jQuery("input[name='chkSubmittalSheetsHeader']").prop('checked', false);
        }
    }

    checkAllRows(event: any) {

        if (event.target.id == 'submittalSheets') {
            jQuery("input[name='chkSubmittalSheetsRow']").prop('checked', true);
            //this.quoteItemsList.forEach(x => x.isSubmittalSheets = event.target.checked);
        }
        else if (event.target.id == 'installationManual') {
            this.quoteItemsList.forEach(x => x.isInstallationManual = event.target.checked);
        }
    }

    //checkAllCheckboxes(event: any) {
    //    jQuery("input[type=checkbox]:checked").prop('checked', true)
    //}

    isAllRowsChecked(colIdentifier: string) {

        if (colIdentifier == 'submittalSheets')
            return this.quoteItemsList.every(_ => _.isSubmittalSheets);
        else if (colIdentifier == 'installationManual')
            return this.quoteItemsList.every(_ => _.isInstallationManual);


        //jQuery("#uploadFilesModal").modal({ backdrop: 'static', keyboard: false });
    }

    isAllCheckboxesChecked() {
        console.log("all checked");
    }

    openFromComponent() {
        this.componentInsideModal.open();
    }

    public handleEvent(filesArray: any) {
        this.files = filesArray;
    }

    onSubmit() {
        this.blockUI.start('Loading...');
        this.ngProgress.start();

        var values = jQuery("input[type=checkbox]:checked").map(function () {
                return this.value;
            }).get();
        
        var submittalModel = new SubmittalPackageModel();
        submittalModel.quoteId = this.quoteId; //"724134023730921472";
        submittalModel.projectId = this.projectId; //"724133966570946560";

        let productsPlusDocsList = values.map(x => x.split('-'));

        let mappedItems: any = []; 

        productsPlusDocsList.forEach((value: string, key: string) => {
            let productId: string = value[0];
            let documentId: string = value[1];

            mappedItems.push({ productId, documentId });             
        });

        submittalModel.productsAndDocuments = mappedItems;            

        if (submittalModel.productsAndDocuments.length > 0) {
            this.submittalService.createQuotePackage(submittalModel)
                .subscribe((data: any) => {
                    console.log(data);
                    this.ngProgress.done();
                    this.blockUI.stop();
                },
                    (err: any) => {

                    }
                )
        }
        
    }
}