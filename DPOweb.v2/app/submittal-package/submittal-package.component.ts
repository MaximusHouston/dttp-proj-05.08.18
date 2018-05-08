import { Component, OnInit, Input, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';
import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';
import { User } from '../shared/models/user';

import { ProjectService } from '../projects/services/project.service';
import { QuoteService } from '../quote/services/quote.service';
import { SubmittalPackageService } from './services/submittal-package.service';

import { SubmittalPackageModel } from '../submittal-package/models/submittal-package';
import { QuoteItemListModel } from '../shared/models/quoteItemList';

declare var jQuery: any;

@Component({
    selector: 'submittal-package',
    templateUrl: 'app/submittal-package/submittal-package.component.html',
    styleUrls: ['app/submittal-package/submittal-package.component.css']
})
export class SubmittalPackageComponent implements OnInit {
    public action: any;
    public user: User;
    public quote: any;
    public hasConfiguredItem: boolean;    

    //public gridView: GridDataResult;
    //<editor-fold desc="Description">
    //public submittalPackageModel: SubmittalPackageModel; 
    //</editor-fold>
    public quoteItemsList: QuoteItemListModel[] = [];    
    public configuredItems: QuoteItemListModel[] = [];
    public mySelection: number;
    @ViewChild('chkSubmittal') submittalCheckbox: ElementRef;

    private possibleDocsList: any = [       
        { colId: 2, name: "submittalSheets", inputname: "chkSubmittalSheetsHeader", label: "Submittal Sheets" },
        { colId: 3, name: "installationManual", inputname: "chkInstallationManualHeader", label: "Installation Manual" },
        { colId: 4, name: "operationManual", inputname: "chkOperationManualHeader", label: "Operation Manual" },
        { colId: 5, name: "guideSpecs", inputname: "chkGuideSpecsHeader", label: "Guide Specs" },
        { colId: 6, name: "productBrochure", inputname: "chkProductBrochureHeader", label: "Product Brochure" },
        { colId: 7, name: "revitDrawing", inputname: "chkRevitDrawingHeader", label: "Revit Drawing" },
        { colId: 8, name: "cadDrawing", inputname: "chkCadDrawingHeader", label: "CAD Drawing" },
        { colId: 9, name: "productFlyer", inputname: "chkProductFlyerHeader", label: "Product Flyer" },
    ];
    
    constructor(private router: Router, private route: ActivatedRoute,
        private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private http: Http, private enums: Enums, private submittalPackageSvc: SubmittalPackageService,
        private projectSvc: ProjectService, private quoteSvc: QuoteService, private elRef: ElementRef,
        ) {
        
        this.action = this.route.snapshot.url[0].path;
        this.user = this.route.snapshot.data['currentUser'].model;
        this.quote = this.route.snapshot.data['quoteModel'].model;        
    }

    ngOnInit() {
        this.loadItems();        
    }    

    private loadItems(): any { 
                
        return this.submittalPackageSvc.getQuotePackage(this.quote.quoteId)
            .subscribe(data => {
                
                this.quoteItemsList = data.items;
                
                this.quoteItemsList.forEach((x, index) => {

                    //filter out configured items to bind to different table
                    if (x.lineItemTypeId === 2) {

                        this.configuredItems.push[index];
                        this.quoteItemsList.splice(index, 1);
                    }
                });

                //loop through again to set the values and ids for checkboxes
                this.quoteItemsList.forEach((x, index) => {

                    x.documents.forEach((y, index) => {
                        if (y.productId === x.productId && y.documentTypeId === 100000008) {
                            x.hasSubmittalSheets = true;

                            x.submittalSheetsDocObject = y;

                            if (x.submittalSheetsDocObject != null) {
                                x.submittalSheetsDocId = x.submittalSheetsDocObject.productId + "-" + x.submittalSheetsDocObject.documentTypeId;
                            } 

                            x.documents.splice(index, 1);
                        }     

                        if (y.productId === x.productId && y.documentTypeId === 100000002) {
                            x.hasInstallationManual = true;

                            x.installationManualDocObject = y;

                            if (x.installationManualDocObject != null) {
                                x.installationManualDocId = x.installationManualDocObject.productId + "-" + x.installationManualDocObject.documentTypeId;
                            }

                            x.documents.splice(index, 1);
                        }                      

                    });

                    //set values for checkboxes
                    //if (x.documents.find((y, index) => (y.productId === x.productId && y.documentTypeId === 100000008))) {
                    //    //let index = x.documents.findIndex(y => y.productId === x.productId && y.documentTypeId === 100000008); //find index in your array                        

                    //    x.hasSubmittalSheets == true;

                    //    x.submittalDocument = x.documents[index];

                        

                    //    x.documents.splice(index, 1);//remove element from array
                    //}           
                })                   
            });
    }

}
