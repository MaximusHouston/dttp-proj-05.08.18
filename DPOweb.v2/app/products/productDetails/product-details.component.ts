import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from '../../shared/services/toastr.service';
import { LoadingIconService } from '../../shared/services/loadingIcon.service';
import { UserService } from '../../shared/services/user.service';
import { SystemAccessEnum } from '../../shared/services/systemAccessEnum';
import { Enums } from '../../shared/enums/enums';

import { ProductService } from '../services/product.service';
import { BasketService } from '../../basket/services/basket.service';
declare var jQuery: any;

@Component({
    selector: 'product-details',
    templateUrl: 'app/products/productDetails/product-details.component.html',
})

export class ProductDetailsComponent implements OnInit {
    //@Input() product: any;
    //@Input() userBasket: any;

    public product: any;
    public userBasket: any;
    public currentUser: any;
    public showPrices: boolean = false;

    public relatedIndoorUnit: any;
    public relatedOutdoorUnit: any;

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService, private userSvc: UserService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums,
        private productSvc: ProductService, private basketSvc: BasketService) {

    }

    ngOnChange() {
        //console.log("ProductDetail Component: ngOnChange");
    }

    ngOnInit() {

        this.route.params.subscribe((params: { id: string }) => {
            this.product = null; // this is to invoke change detection
            var data = {
                "ProductId": params.id,
            };
            this.productSvc.getProduct(data).then(this.GetProductCallback.bind(this));
        });


        if (this.basketSvc.userBasket == undefined) {
            this.basketSvc.getBasket().then(this.getBasketCallback.bind(this));
        } else {
            this.userBasket = this.basketSvc.userBasket;
        }


        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then((resp: any) => {
                if (resp.isok) {
                    this.userSvc.currentUser = resp.model;
                    this.currentUser = resp.model;
                    this.showPrices = this.userSvc.currentUser.showPrices;
                }
            });
        } else {
            this.currentUser = this.userSvc.currentUser;
        } 


    }

    ngDoCheck() {
        
    }

    ngAfterContentInit() {
        //console.log("product Detail: ngAfterContentInit");
        //this.setupActiveTab();
    }

    ngAfterContentChecked() {
        //console.log("product Detail: ngAfterContentChecked");
        //this.setupActiveTab();
    }

    ngAfterViewInit() {
        //console.log("product Detail: ngAfterViewInit");
        //this.setupActiveTab();

        //this.route.queryParams.subscribe((params: { tab: string }) => {
        //    var subTabId = '#' + params.tab;
        //    $(subTabId).click();
        //});

        
    }



    ngAfterViewChecked() {
        //console.log("product Detail: ngAfterViewChecked");

        //this.setupActiveTab();


        if ($('#accessoryFilters').length) {
            $('input[name="accessory-filter-type"]').on('change', function () {
                $('#indoorAccessories, #outdoorAccessories').hide();

                switch ($(this).val()) {
                    case "indoor":
                        $('#indoorAccessories').show();
                        break;
                    case "outdoor":
                        $('#outdoorAccessories').show();
                        break;
                    default:
                        $('#indoorAccessories, #outdoorAccessories').show();
                }
            });
        }

    }

    public setupActiveTab() {
        //Product family tabs
        $('.productFamilyTab li').each(function () {
            $(this).removeClass('active');
        });

        if (this.product.productFamilyId != null) {
            var activeFamilyTabId = "#product-family-tab-" + this.product.productFamilyId;
        } else {
            var activeFamilyTabId = "#product-family-tab-home";
        }

        $(activeFamilyTabId).addClass("active");


        //Product details tabs
        $('#productDetailsTabs li').click(function () {
            $('#productDetailsTabs li').each(function () {
                $(this).removeClass('active');
            });

            $(this).addClass('active');

        });

        $('#product-overview').click(function () {
            //location.href = "#productOverviewTab";
            $('#productOverviewTab').show();
            $('#productRelatedAccessoriesTab').hide();
            $('#productSpecsTab').hide();
        });
        $('#product-accessories').click(function () {

            //location.href = "#productRelatedAccessoriesTa  
            $('#productOverviewTab').hide();
            $('#productRelatedAccessoriesTab').show();
            $('#productSpecsTab').hide();
        });
        $('#product-specs').click(function () {

            //location.href = "#productSpecsTab";
            $('#productOverviewTab').hide();
            $('#productRelatedAccessoriesTab').hide();
            $('#productSpecsTab').show();
        });

        

    }

    public setActiveTabByUrlParam() {
        this.route.queryParams.subscribe((params: { activeTab: string }) => {
            var activeTabId = '#' + params.activeTab;
            $(activeTabId).click();

        });


    }

    public GetProductCallback(resp: any) {
        if (resp.isok) {
            this.product = resp.model;
            //this.productSvc.product = resp.model;

            this.getSubmittalDataSheet(this.product);

            this.setupActiveTab();


        }

        if (this.product.isSystem) {
            for (var i in this.product.subProducts) {
                var item = this.product.subProducts[i];
                if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Indoor) { // indoor
                    this.relatedIndoorUnit = item;
                } else if (item.productModelTypeId == this.enums.ProductModelTypeEnum.Outdoor) { // outdoor
                    this.relatedOutdoorUnit = item;
                }
            }
        }

        console.log("************ SubmittalSheetTypeId: " + this.product.submittalSheetTypeId + " *******************");
    }

    public getBasketCallback(resp: any) {
        if (resp.isok) {
            this.userBasket = resp.model;
            this.basketSvc.userBasket = resp.model;

        }
    }

    public productDetails(event: any, product: any) {
        this.productSvc.product = product;

        this.router.navigate(['/products', { outlets: { 'productDetails': [product.productId] } }], { queryParams: { activeTab: 'product-overview' } });
    }


    public getSubmittalDataSheet(product: any) {

        this.productSvc.getSubmittalDataSheet(product.productNumber).then((resp: any) => {
            if (resp) {
                var HtmlString = resp;
                $("#technical-specs").replaceWith(HtmlString);
                this.setActiveTabByUrlParam();

            }
        });
    }

};