import { Component, OnInit, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-details-gridView',
    templateUrl: 'app/products/product-details-gridView.component.html',

})

export class ProductDetailsGridViewComponent implements OnInit {
    @Input() user: any;
    @Input() product: any;
    //@Output() changeQty: EventEmitter<any> = new EventEmitter();
    @Output() viewProductDetailsEvent: EventEmitter<any> = new EventEmitter();
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


    constructor(private router: Router, private route: ActivatedRoute, private elementRef: ElementRef, private toastrSvc: ToastrService, private userSvc: UserService,
        private systemAccessEnum: SystemAccessEnum,
        private enums: Enums,
        private productSvc: ProductService) {

    }


    ngOnChanges() {
        this.resetColumns();
        this.setupColumns();
    }

    ngOnInit() {
       
    }

   

    ngAfterViewChecked() {
       

    }

    //public productDetails(event: any, product: any, activeTab: any){
    //    //this.productSvc.product = product;

    //    //this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }], { queryParams: { tab: activeTab } });
    //    this.viewProductDetailsEvent.emit(product);
    //}

    public productDetails(event: any, product: any, activeTab: any) {
        var eventParams = {
            'product': product,
            'activeTab': activeTab
        }
        this.viewProductDetailsEvent.emit(eventParams);
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

        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.MiniSplit //Mini-Split
            || (this.product.productFamilyId == this.enums.ProductFamilyEnum.AlthermaSplit && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //Altherma
            || (this.product.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) //MultiSplit - Outdoor/All

            || this.product.productFamilyId == this.enums.ProductFamilyEnum.SkyAir) { //Sky-Air

            this.SEERNonDucted = true;
            this.EERNonDucted = true;
            this.HSPFNonDucted = true;
            this.COP47NonDucted = true;

        }

        if ((this.product.productFamilyId == this.enums.ProductFamilyEnum.VRV || this.product.productFamilyId == this.enums.ProductFamilyEnum.MultiSplit) && this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) { // MultiSplit/VRV - Indoor
            //Show nothing
        }

        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.VRV && (this.product.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor || this.product.productModelTypeId == this.enums.ProductModelTypeEnum.All)) {// VRV - Outdoor/All
            this.IEERNonDucted = true;
            this.EERNonDucted = true;
            this.COP47NonDucted = true;
            this.SCHENonDucted = true;
        }

        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem) { //	Unitary Split

            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.SplitAC) {// Air Conditioner
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.SplitHP) {// Heat Pump
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler || this.product.productClassPIMId == this.enums.ProductClassPIMEnum.Coil) {// Air Handler || Evaporator Coil

            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.GasFurnace) { // Gas Furnace
                this.AFUE = true;
            }

            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }
        }

        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem) { // Unitary Package
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackageAC) {// Package AC
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedHP) {// Package HP
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
           
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.PackagedGED) { 
                this.SEERDucted = true;
                this.EERDucted = true;
                this.AFUE = true;
                if (this.product.productEnergySourceTypeId = this.enums.ProductEnergySourceTypeEnum.DualFuel) {
                    this.HSPFDucted = true;
                    this.COP47Ducted = true;
                }
            }
                   

            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                this.AFUE = true;
            }

        }

        if (this.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem) { // Commercial Split
            
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedAC) {// Air Conditioner
                this.SEERDucted = true;
                this.EERDucted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.LightCommercialPackagedHP) {// Heat Pump
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
            }
            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.AirHandler) {// Air Handler 

            }

            if (this.product.productClassPIMId == this.enums.ProductClassPIMEnum.All) { // All
                this.SEERDucted = true;
                this.EERDucted = true;
                this.HSPFDucted = true;
                this.COP47Ducted = true;
                //this.AFUEDucted = true;
            }
        }

    }
  
       


};