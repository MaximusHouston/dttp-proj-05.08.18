import { Component, OnInit, Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProductService } from './services/product.service';
declare var jQuery: any;

@Component({
    selector: 'product-spec-bars',
    templateUrl: 'app/products/product-spec-bars.component.html',
})

export class ProductSpecBarsComponent implements OnInit {
    @Input() product: any;
    //@Input() userBasket: any;

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


    constructor(private elementRef: ElementRef, private toastrSvc: ToastrService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums,
        private productSvc: ProductService
    ) {

    }

    ngOnChanges() {
        this.resetColumns();
        this.setupColumns();
    }

    ngOnInit() {
        setTimeout(this.setupSpecBars.bind(this), 200); // wait 0.2 sec

    }

   
    ngAfterViewChecked() {
        

    }

    public setupSpecBars() {
        var self = this;
        var element = this.elementRef.nativeElement;

        var seerRatingBar = jQuery(element).find(".seerRatingBar");
        if (seerRatingBar[0] != undefined && this.product.specifications.all.seerNonDucted) {
            jQuery(seerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 40,
                value: this.product.specifications.all.seerNonDucted.value
            });
        }

        var scheRatingBar = jQuery(element).find(".scheRatingBar");
        if (scheRatingBar[0] != undefined && this.product.specifications.all.scheNonDucted) {
            jQuery(scheRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 35,
                value: this.product.specifications.all.scheNonDucted.value
            });
        }

        var ieerRatingBar = jQuery(element).find(".ieerRatingBar");
        if (ieerRatingBar[0] != undefined && this.product.specifications.all.ieerNonDucted) {
            jQuery(ieerRatingBar[0]).kendoProgressBar({

                showStatus: false,
                max: 40,
                value: this.product.specifications.all.ieerNonDucted.value
            });
        }

        var eerRatingBar = jQuery(element).find(".eerRatingBar");
        if (eerRatingBar[0] != undefined && this.product.specifications.all.eerNonDucted) {
            jQuery(eerRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.eerNonDucted.value
            });
        }

        var hspfRatingBar = jQuery(element).find(".hspfRatingBar");
        if (hspfRatingBar[0] != undefined && this.product.specifications.all.hspfNonDucted) {
            jQuery(hspfRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 20,
                value: this.product.specifications.all.hspfNonDucted.value
            });
        }

        var cop47RatingBar = jQuery(element).find(".cop47RatingBar");
        if (cop47RatingBar[0] != undefined && this.product.specifications.all.coP47NonDucted) {
            jQuery(cop47RatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 10,
                value: this.product.specifications.all.coP47NonDucted.value
            });
        }

        var afueRatingBar = jQuery(element).find(".afueRatingBar");
        if (afueRatingBar[0] != undefined && this.product.specifications.all.afue) {
            jQuery(afueRatingBar[0]).kendoProgressBar({
                showStatus: false,
                max: 100,
                value: this.product.specifications.all.afue.value
            });
        }


        //Unitary
        if (self.product.productFamilyId == this.enums.ProductFamilyEnum.UnitarySplitSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.UnitaryPackagedSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialSplitSystem || self.product.productFamilyId == this.enums.ProductFamilyEnum.LightCommercialPackagedSystem) {
            var seerRatingBarDucted = jQuery(element).find(".seerRatingBarDucted");
            if (seerRatingBarDucted[0] != undefined && this.product.specifications.all.seerDucted) {
                jQuery(seerRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 40,
                    value: this.product.specifications.all.seerDucted.value
                });
            }

            var ieerRatingBarDucted = jQuery(element).find(".ieerRatingBarDucted");
            if (ieerRatingBarDucted[0] != undefined && this.product.specifications.all.ieerDucted) {
                jQuery(ieerRatingBarDucted[0]).kendoProgressBar({

                    showStatus: false,
                    max: 40,
                    value: this.product.specifications.all.ieerDucted.value
                });
            }

            var eerRatingBarDucted = jQuery(element).find(".eerRatingBarDucted");
            if (eerRatingBarDucted[0] != undefined && this.product.specifications.all.eerDucted) {
                jQuery(eerRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 20,
                    value: this.product.specifications.all.eerDucted.value
                });
            }

            var hspfRatingBarDucted = jQuery(element).find(".hspfRatingBarDucted");
            if (hspfRatingBarDucted[0] != undefined && this.product.specifications.all.hspfDucted) {
                jQuery(hspfRatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 20,
                    value: this.product.specifications.all.hspfDucted.value
                });
            }

            var cop47RatingBarDucted = jQuery(element).find(".cop47RatingBarDucted");
            if (cop47RatingBarDucted[0] != undefined && this.product.specifications.all.coP47Ducted) {
                jQuery(cop47RatingBarDucted[0]).kendoProgressBar({
                    showStatus: false,
                    max: 10,
                    value: this.product.specifications.all.coP47Ducted.value
                });
            }
        }
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