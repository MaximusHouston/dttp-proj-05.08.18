import { Component, OnInit, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-details-tableView',
    templateUrl: 'app/products/product-details-tableView.component.html',

})

export class ProductDetailsTableViewComponent implements OnInit {

    //@Input() product: any;
    @Input() user: any;
    @Input() productFamilyId: any;
    @Input() productModelTypeId: any;
    @Input() unitInstallationTypeId: any;
    @Input() productClassPIMId: any;

    @Input() gridViewData: any;
    @Input() skip: any;
    @Input() pageSize: any;
    @Output() viewProductDetailsEvent: EventEmitter<any> = new EventEmitter();
    @Output() pageChangeEvt: EventEmitter<any> = new EventEmitter();

    @Input() basketQuoteId: any;

    public SEERNonDucted = false;
    public EERNonDucted = false;
    public HSPFNonDucted = false;
    public COP47NonDucted = false;
    public IEERNonDucted = false;
    public SCHENonDucted = false;

    public SEERDucted = false;
    public EERDucted = false;
    public HSPFDucted = false;
    public COP47Ducted = false;
    public AFUE = false;




    constructor(private elementRef: ElementRef, private toastrSvc: ToastrService, private userSvc: UserService,
        private systemAccessEnum: SystemAccessEnum,
        private enums: Enums,
        private productSvc: ProductService) {

    }

    ngOnChanges() {
        this.resetColumns();
        this.setupColumns();
    }

    ngOnInit() {
        //this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));

        //this.setupColumns();
        
    }

    protected pageChange(event: PageChangeEvent): void {
        this.pageChangeEvt.emit(event);
    }


    ngAfterViewChecked() {
        //var self = this;
        //var element = this.elementRef.nativeElement;



        //numeric text box
        //var numericTextBox = jQuery(element).find(".numericTextBox");
        //if (numericTextBox[0] != undefined) {
        //    jQuery(numericTextBox[0]).change(function () {
        //        if (this.valueAsNumber < 0) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        } else if (Math.floor(this.valueAsNumber) != this.valueAsNumber) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        }
        //        else {
        //            self.product.quantity = this.valueAsNumber;
        //        }


        //    });


        //}


    }

    public resetColumns() {
        this.SEERNonDucted = false;
        this.EERNonDucted = false;
        this.HSPFNonDucted = false;
        this.COP47NonDucted = false;
        this.IEERNonDucted = false;
        this.SCHENonDucted = false;

        this.SEERDucted = false;
        this.EERDucted = false;
        this.HSPFDucted = false;
        this.COP47Ducted = false;
        this.AFUE = false;
    }

    public setupColumns() {

        if (this.productFamilyId == this.enums.ProductFamilyEnum.MiniSplit //Mini-Split
            || (this.productFamilyId == this.enums.ProductFamilyEnum.AlthermaSplit && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //Altherma
            || (this.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //MultiSplit - Outdoor/All

            || this.productFamilyId == this.enums.ProductFamilyEnum.SkyAir) { //Sky-Air

            this.SEERNonDucted = true;
            this.EERNonDucted = true;
            this.HSPFNonDucted = true;
            this.COP47NonDucted = true;

        }

        if ((this.productFamilyId == this.enums.ProductFamilyEnum.VRV || this.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit) && this.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) { // MultiSplit/VRV - Indoor
            //Show nothing
        }

        if (this.productFamilyId == this.enums.ProductFamilyEnum.VRV && (this.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.productModelTypeId == this.enums.ProductModelTypeEnum.All)) {// VRV - Outdoor/All
            this.IEERNonDucted = true;
            this.EERNonDucted = true;
            this.COP47NonDucted = true;
            this.SCHENonDucted = true;
        }

        if (this.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem) { //	Unitary Split

            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.SplitAC) {// Air Conditioner
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.SplitHP) {// Heat Pump
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler || this.productClassPIMId == this.enums.ProductClassPIMEnum.Coil) {// Air Handler || Evaporator Coil

            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.GasFurnace) { // Gas Furnace
                this.AFUE = true;
            }

            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }

        if (this.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem) { // Unitary Package
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackageAC) {// Package AC
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedHP) {// Package HP
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedGED) { // Packaged GED
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
                        

            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }

        }

        if (this.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem) { // Commercial Split
            
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedAC) {// Air Conditioner
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedHP) {// Heat Pump
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler) {// Air Handler 

            }

            if (this.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                //this.AFUEDucted = true;
            }
        }

    }

    validateQuantity(event: any) {
        var s = 0;
        //if (this.valueAsNumber < 0) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        } else if (Math.floor(this.valueAsNumber) != this.valueAsNumber) {
        //            this.valueAsNumber = 0;
        //            self.toastrSvc.ErrorFadeOut("Please enter an integer greater than zero.");
        //        }
        //        else {
        //            self.product.quantity = this.valueAsNumber;
        //        }
    }


    public productDetails(event: any, product: any, activeTab: any) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        }
        this.viewProductDetailsEvent.emit(eventParams);
    }


};