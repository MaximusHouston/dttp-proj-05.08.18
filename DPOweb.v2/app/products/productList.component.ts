import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProductService } from './services/product.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { process } from '@progress/kendo-data-query';
import { orderBy } from '@progress/kendo-data-query';
declare var jQuery: any;

/*
interface Item {
  text: string,
  value: number
}*/

@Component({
    selector: 'product-list',
    templateUrl: 'app/products/productList.component.html',

})

export class ProductListComponent implements OnInit {

    @Input() user: any;
    @Input() productsModel: any;

    @Input() productFamilyId: any;
    @Input() productTypeId: any;
    @Input() productModelTypeId: any;
    @Input() unitInstallationTypeId: any;
    @Input() productClassPIMId: any;

    //public productFamilyId: any;
    //public productModelTypeId: any;
    //public unitInstallationTypeId: any;


    @Output() updateBasketEvent: EventEmitter<any> = new EventEmitter();

    @Output() showProductDetailsEvent: EventEmitter<any> = new EventEmitter();

    public basketQuoteId: any;

    public gridData: any;


    public gridFilteredData: any;
    private gridViewData: GridDataResult;

    private pageSize: number = 100;
    private skip: number = 0;

    public viewOption: any = 1;

    //Dropdowns

    //public sortByDDLValues: any = [{ text: "Model No.", value: "product.productNumber" },
    //                                { text: "Product description", value: "product.name" }];

    public productSortBy: any = "product.productNumber";

    public productFilters: any = [];
    public defaultItem: { text: string, value: any } = { text: "Select...", value: null };

    public productStatusOptions: Array<{ text: string, value: any }> = [];
    public inventoryStatusOptions: Array<{ text: string, value: any }> = [];
	/*
	public productStatusOptions : Array<{ text: string, value: any }> = [
		{ text: "Released", value: 111267 },
		{ text: "Obsolete", value: 111268 },
		{ text: "Hidden Module Unit", value: 111269 },
		{ text: "All", value: null }
	];*/

    //public selectedProductStatus: { text: string, value: any } = { text: "Released", value: 111267 };
    public selectedProductStatus: number = 111267;
    public selectedInventoryStatus: number = 0;

	/*
	public listItems: Array<Item> = [
		{ text: "Small", value: 1 },
		{ text: "Medium", value: 2 },
		{ text: "Large", value: 3 }
	];

	public selectedItem: Item = this.listItems[1];
	*/


    public productSubFamilyDDLValues: any;
    public productSubFamilySelectedValue: any;

    public productHeatExchangerTypeDDLValues: any;
    public productHeatExchangerTypeSelectedValue: any;

    public unitInstallationTypeDDLValues: any;
    public unitInstallationTypeSelectedValue: any;

    public productCategoryDDLValues: any;
    public productCategorySelectedValue: any;

    public productFunctionCategoryDDLValues: any;
    public productFunctionCategorySelectedValue: any

    public tonnageDDLValues: any;
    public tonnageSelectedValue: any;

    public coolingCapacityRatedDDLValues: any;
    public coolingCapacityRatedSelectedValue: any;

    public coolingCapacityNominalDDLValues: any;
    public coolingCapacityNominalSelectedValue: any;

    public heatingCapacityRatedDDLValues: any;
    public heatingCapacityRatedSelectedValue: any;

    public productPowerVoltageDDLValues: any;
    public productPowerVoltageSelectedValue: any;

    public productCompressorTypeDDLValues: any;
    public productCompressorTypeSelectedValue: any;

    public productGasValveTypeDDLValues: any;
    public productGasValveTypeSelectedValue: any;

    public productMotorSpeedTypeDDLValues: any;
    public productMotorSpeedTypeSelectedValue: any;

    public productInstallationConfigurationTypeDDLValues: any;
    public productInstallationConfigurationTypeSelectedValue: any;

    public airFlowRateHighCoolingDDLValues: any;
    public airFlowRateHighCoolingSelectedValue: any;

    public airFlowRateHighHeatingDDLValues: any;
    public airFlowRateHighHeatingSelectedValue: any;

    public productAccessoryDDLValues: any;
    public productAccessorySelectedValue: any;

    public firstViewCheckAfterOnChange = true;



    constructor(private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService, private userSvc: UserService,
        private systemAccessEnum: SystemAccessEnum,
        private enums: Enums,
        private productSvc: ProductService) {


    }

    ngOnChanges() {
        //console.log("ProductList: ---- ngOnChanges ----");

        this.firstViewCheckAfterOnChange = true;

        this.gridData = this.productsModel.products;
        this.gridFilteredData = this.gridData;

        this.loadProducts();
        this.skip = 0; // restart from page 1



        this.resetFilters();

        setTimeout(this.updateDropDownLists(this.gridData), 200); // wait 0.2 sec



        this.resetSortBy();
        setTimeout(this.setupGridHeight.bind(this), 200);

        //this.selectedProductStatus = { text: "Released", value: 111267 };
        if (this.productsModel.isSearch == true) {
            this.selectedProductStatus = 0; // All
        } else {
            this.selectedProductStatus = this.enums.ProductStatusTypeEnum.Active;
        }
        
        this.selectedInventoryStatus = 0; // All
        this.setDefaultFilters(this.productsModel.isSearch);
       

    }

	

    ngOnInit() {
        //console.log("ProductList: ngOnInit");

        var self = this;

        this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
        //var data = this.productSvc.getProducts().then(this.getProductsCallback.bind(this));

        this.setupAddProductsBtn();

        this.getProductStatusOptions();

        this.getInventoryStatusOptions();

		

		
	}

    ngDoCheck() {
        //console.log("ProductList: ngDoCheck");
    }

    ngAfterContentInit() {
        //console.log("ProductList: ngAfterContentInit");
    }

    ngAfterContentChecked() {
        //console.log("ProductList: ngAfterContentChecked");
    }

    ngAfterViewInit() {
        //console.log("ProductList: ngAfterViewInit");

        setTimeout(this.setupDropDownFilters.bind(this), 200); // wait 0.2 sec

        this.setActiveViewOption();

        //this.setupPager();

    }

    ngAfterViewChecked() {
        //console.log("ProductList: ngAfterViewChecked");


        if (this.firstViewCheckAfterOnChange) {
            this.updateDropDownLists(this.gridData);
            this.firstViewCheckAfterOnChange = false;
        }





    }

	public getProductStatusOptions(){
		this.productSvc.getProductStatuses().then((resp: any) => {
				if (resp.isok) {
					this.productStatusOptions = resp.model;
					}
                else {
                      this.toastrSvc.displayResponseMessages(resp);
					}
                }).catch(error => {
                    console.log(error);
                });
    }

    public getInventoryStatusOptions() {
        this.productSvc.getInventoryStatuses().then((resp: any) => {
            if (resp.isok) {
                this.inventoryStatusOptions = resp.model;
            }
            else {
                this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(error => {
            console.log(error);
        });
    }

    public setDefaultFilters(isSearch: any) {
        if (!isSearch) {
            this.productFilters.push({ field: "product.productStatusTypeId", operator: "eq", value: this.enums.ProductStatusTypeEnum.Active });
        }
		
		this.applyFilters();
	}

    public setupAddProductsBtn() {
        var self = this;

        jQuery("#addProductsToQuoteBtn").click(function () {
            var data = {
                "Products": self.gridData
            }

            self.loadingIconSvc.Start(jQuery("#productPageContainer"));


            self.productSvc.addProductsToQuote(data).then((resp: any) => {// on success
                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));

                self.toastrSvc.displayResponseMessages(resp);

                self.clearQuantities();

                self.updateBasketEvent.emit();

                //self.reloadGrid();

            }, (resp: any) => {// on error

                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));

                self.toastrSvc.displayResponseMessages(resp);
                               
                //TODO: create a log service for this
                for (let message of resp.messages.items) {
                    console.log(message.text);
                }

            });
        });
    }

    public clearQuantities() {

        for (var i = 0; i < this.gridViewData.data.length; i++) {
            this.gridViewData.data[i].product.quantity = 0;
        }
    }

    public setupPager() {


        if (this.viewOption == 1) { // product-table-view (default)

            this.setupGridHeight();

        } else {

            if (this.viewOption == 0) {
                var grid = $("#product-list-view");
            } else if (this.viewOption == 2) {
                var grid = $("#product-grid-view");
            }

            grid.find(".k-grid-pager").insertBefore(grid.find(".k-grid-content"));

            var pager = grid.find(".k-grid-pager");
            var viewOption = this.viewOption;

            //TODO: Hacking kendo grid css to fix kendo grid with custom pager
            //need to find a better solution
            var gridContainer = grid.find(".k-grid-container");
            gridContainer.css("display", "block");
            //================================================================

            resizeFixed();
            $(window).resize(resizeFixed);
            $(window).scroll(scrollFixed);


        }


        function resizeFixed() {
            pager.css("width", grid.width());
        }

        function scrollFixed() {
            if (viewOption != 1) {
                var offset = $(this).scrollTop(),
                    tableOffsetTop = grid.offset().top,
                    tableOffsetBottom = tableOffsetTop + grid.height() - pager.height();
                if (offset < tableOffsetTop || offset > tableOffsetBottom) {
                    pager.removeClass("fixed-pager");
                    $("#scrollUpBtn").css("display", "none");
                } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom) {
                    pager.addClass("fixed-pager");
                    $("#scrollUpBtn").css("display", "block");
                }
            }

        }



    }

    public scrollUp() {
        $('html, body').animate({ scrollTop: 0 }, 300);
    }

    public setupGridHeight() {
        if (this.viewOption == 1) {
            var gridContent = $(".k-grid-content");
            var gridHeaderH = $(".k-grid-header").height();
            var gridPagerH = $(".k-grid-header").height();

           
            var offsetTop = $("#product-table-view").position().top;

            var windowHeight = $(window).height();

             //old code - before kendo grid upgrade 
            //var gridHeight = windowHeight - offsetTop - gridHeaderH - gridPagerH - 5;

            //if (windowHeight > 750) {
            //    gridContent.height(gridHeight);
            //}

            //fix broken css after kendo angular grid upgrade to grid 1.2.1
            var gridContentHeight = 570 - gridHeaderH - gridPagerH;
            gridContent.height(gridContentHeight);
          
          
        }

    }

    public setupDropDownFilters() {

        var self = this;
        //delete when Kendo-angular 2 DDL is used
        $("#sortProductDLL").kendoDropDownList({
            dataSource: [{ text: "Model No.", value: "product.productNumber" },
            { text: "Product description", value: "product.name" }],
            dataTextField: "text",
            dataValueField: "value",

            change: function (e) {
                var value = this.value();
                self.productSortBy = value;
                self.sortBy(value);

            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#unitInstallationTypeDDL").kendoDropDownList({
            dataSource: self.unitInstallationTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            //value: self.unitInstallationTypeSelectedValue,
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                //self.unitInstallationTypeSelectedValue = value;
                self.productFilterJQ(value, 'unitInstallationTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#productCategoryDDL").kendoDropDownList({
            dataSource: self.productCategoryDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'productCategoryId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#coolingCapacityRatedDDL").kendoDropDownList({
            dataSource: self.coolingCapacityRatedDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'coolingCapacityRated');
            }
        });

        $("#coolingCapacityNominalDDL").kendoDropDownList({
            dataSource: self.coolingCapacityNominalDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'coolingCapacityNominal');
            }
        });

        $("#heatingCapacityRatedDDL").kendoDropDownList({
            dataSource: self.heatingCapacityRatedDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'heatingCapacityRated');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#powerVoltageDDL").kendoDropDownList({
            dataSource: self.productPowerVoltageDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'productPowerVoltageTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#heatExchangerTypeDDL").kendoDropDownList({
            dataSource: self.productHeatExchangerTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'heatExchangerTypeId'); //heatExchangerTypeId does not have values in DB yet
            }
        });


        //delete when Kendo-angular 2 DDL is used
        $("#compressorTypeDDL").kendoDropDownList({
            dataSource: self.productCompressorTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'compressorTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#gasValveTypeDDL").kendoDropDownList({
            dataSource: self.productGasValveTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'gasValveTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#motorTypeDDL").kendoDropDownList({
            dataSource: self.productMotorSpeedTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'motorTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#installationConfigurationTypeDDL").kendoDropDownList({
            dataSource: self.productInstallationConfigurationTypeDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'installationConfigurationTypeId');
            }
        });

        //delete when Kendo-angular 2 DDL is used
        $("#airFlowRateTypeDDL").kendoDropDownList({
            dataSource: self.airFlowRateHighCoolingDDLValues,
            dataTextField: "text",
            dataValueField: "value",
            optionLabel: {
                text: "Select ...",
                value: null
            },
            change: function (e) {
                var value = this.value();
                self.productFilterJQ(value, 'airFlowRateHighCooling');
            }
        });

    }

    protected pageChange(event: PageChangeEvent): void {
        this.skip = event.skip;
        this.loadProducts();

        var elem;

        if (this.viewOption == 1) {// Table view
            elem = document.querySelector("#product-table-view table tbody tr");
            elem.scrollIntoView();
        } else {
            elem = document.getElementById("productFamilyName");
            window.scrollTo(0, elem.offsetTop);
        }

    }

    public getDistinctValues(valueId: any, description: any, data: any) {
        var flags: any = [];// flags[] is to keep track of what value is already added to distinctValues[]
        var distinctValues: any = [];
        var productArray: any[] = data;

        for (var i = 0; i < productArray.length; i++) {
            var item = { text: "", value: 0 };

            var value = productArray[i].product[valueId];

            if (description == null) {
                var text = productArray[i].product[valueId];
            } else {
                var text = productArray[i].product[description];
            }

            if ((flags[value] == true) || value == null) {
                continue;
            }
            else {
                item.text = text;
                item.value = value;
                distinctValues.push(item);
                flags[value] = true;
            }

        }


        distinctValues = this.sortDistinctValues(distinctValues);

        return distinctValues;
    }

    public sortDistinctValues(distinctValues: any) {

        var sortedValues = [];
        if (distinctValues.length > 0) {
            if (isNaN(distinctValues[0].text)) {// sort strings
                sortedValues = distinctValues.sort(function (a: any, b: any) {
                    if (a.text < b.text) return -1;
                    if (a.text > b.text) return 1;
                    return 0;
                });
            } else {// sort numbers
                sortedValues = distinctValues.sort(function (a: any, b: any) { return a.text - b.text });
            }
        } else {
            return distinctValues;
        }

        return sortedValues;
    }

    public getBasketQuoteIdCallback(resp: any) {
        if (resp.isok) {
            this.basketQuoteId = resp.model;

        }
    }

    private loadProducts(): void {

        if (this.productsModel != undefined) {
            this.gridViewData = {
                data: this.gridFilteredData.slice(this.skip, this.skip + this.pageSize),
                total: this.gridFilteredData.length
            };
        }


    }

    private loadProductsNoFilter() {

        if (this.productsModel != undefined) {
            this.gridViewData = {
                data: this.gridData.slice(this.skip, this.skip + this.pageSize),
                total: this.gridData.length
            };
        }


        this.updateDropDownLists(this.gridData);
        //this.setupDropDownFilters();
    }

    //This function is used for kendo-dropdownlist (angular 2)
    public productFilter(selectedObj: any, field: string) {

        var fieldName = "product." + field;

        if (selectedObj == undefined || selectedObj.value == null) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            if (this.productFilters.length > 0) {
                this.applyFilters();
            } else {
                this.loadProductsNoFilter()
            }

        } else {

            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: selectedObj.value });

            this.applyFilters();
        }
    }

    public productFilterByDecimalValue(selectedObj: any, field: string) {

        var fieldName = "product." + field;

        if (selectedObj == undefined || selectedObj.value == null) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            if (this.productFilters.length > 0) {
                this.applyFilters();
            } else {
                this.loadProductsNoFilter()
            }

        } else {

            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: selectedObj.valueDecimal });

            this.applyFilters();
        }
    }

    public productFilterPrimitive(value: any,field: string) {

        var fieldName = "product." + field;
        if (value == null || value == 0) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            if (this.productFilters.length > 0) {
                this.applyFilters();
            } else {
                this.loadProductsNoFilter()
            }

        } else {

            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: value });

            this.applyFilters();
        }

    }

    //This function is used for kendo-DDL (JQuery)
    public productFilterJQ(value: any, field: string) {

        var fieldName = "product." + field;

        if (value == undefined || value == null || value == "") {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            if (this.productFilters.length > 0) {
                this.applyFilters();
            } else {
                this.loadProductsNoFilter()
            }

        } else {

            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);

            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: value });

            this.applyFilters();
        }

    }


    public removeFilterByName(fieldName: string) {
        for (var i = 0; i < this.productFilters.length; i++) {
            if (this.productFilters[i].field == fieldName) {
                this.productFilters.splice(i, 1);
            }
        }
    }

    public applyFilters() {
        this.skip = 0;// reset to page 1

        const result = process(this.gridData, {
            sort: [{ field: this.productSortBy, dir: "asc" }],
            filter: {
                logic: "and",
                filters: this.productFilters
            }
        });

        this.gridViewData = {
            data: result.data.slice(this.skip, this.skip + this.pageSize),
            total: result.data.length
        };

        this.gridFilteredData = result.data;
        this.updateDropDownLists(result.data);
        //this.setupDropDownFilters();
    }

    //sortBy JQuery style
    public sortBy(fieldName: any) {

        const result = orderBy(this.gridFilteredData, [{ field: fieldName, dir: "asc" }]);

        this.gridFilteredData = result; // filtered and sorted data

        this.gridViewData = {
            data: result.slice(this.skip, this.skip + this.pageSize),
            total: result.length
        };

    }

    //sortBy Angular 2 style
    //public sortBy(option: any) {
    //    var fieldName = option.value;
    //    const result = orderBy(this.gridFilteredData, [{ field: fieldName, dir: "asc" }]);

    //    this.gridFilteredData = result; // filtered and sorted data

    //    this.gridViewData = {
    //        data: result.slice(this.skip, this.skip + this.pageSize),
    //        total: result.length
    //    };

    //}

    public clearFilters() {

        this.resetFilters();

        this.loadProductsNoFilter();

        this.resetSortBy();
    }

    public resetFilters() {
        this.productFilters = [];

        //Kendo DDL angular 2 

        this.productSubFamilySelectedValue = null;
        this.productFunctionCategorySelectedValue = null;
        this.unitInstallationTypeSelectedValue = null;
        this.productPowerVoltageSelectedValue = null;
        this.tonnageSelectedValue = null;
        this.coolingCapacityRatedSelectedValue = null;
        this.coolingCapacityNominalSelectedValue = null;
        this.heatingCapacityRatedSelectedValue = null;
        this.productCompressorTypeSelectedValue = null;
        this.airFlowRateHighCoolingSelectedValue = null;
        this.airFlowRateHighHeatingSelectedValue = null;
        this.productGasValveTypeSelectedValue = null;
        this.productMotorSpeedTypeSelectedValue = null;
        this.productInstallationConfigurationTypeSelectedValue = null;
        this.productHeatExchangerTypeSelectedValue = null;
        this.productAccessorySelectedValue = null;

        //this.selectedProductStatus = this.enums.ProductStatusTypeEnum.Active;
        this.selectedProductStatus = 0;// All
        this.selectedInventoryStatus = 0;// All

        //this.productCategorySelectedValue = null;



        //delete when Kendo DDL angular 2 is used
        if ($("#unitInstallationTypeDDL").data("kendoDropDownList") != undefined) {
            $("#unitInstallationTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#productCategoryDDL").data("kendoDropDownList") != undefined) {
            $("#productCategoryDDL").data("kendoDropDownList").value("");
        }


        if ($("#coolingCapacityRatedDDL").data("kendoDropDownList") != undefined) {
            $("#coolingCapacityRatedDDL").data("kendoDropDownList").value("");
        }

        if ($("#coolingCapacityNominalDDL").data("kendoDropDownList") != undefined) {
            $("#coolingCapacityNominalDDL").data("kendoDropDownList").value("");
        }

        if ($("#heatingCapacityRatedDDL").data("kendoDropDownList") != undefined) {
            $("#heatingCapacityRatedDDL").data("kendoDropDownList").value("");
        }

        if ($("#powerVoltageDDL").data("kendoDropDownList") != undefined) {
            $("#powerVoltageDDL").data("kendoDropDownList").value("");
        }


        if ($("#heatExchangerTypeDDL").data("kendoDropDownList") != undefined) {
            $("#heatExchangerTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#compressorTypeDDL").data("kendoDropDownList") != undefined) {
            $("#compressorTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#gasValveTypeDDL").data("kendoDropDownList") != undefined) {
            $("#gasValveTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#motorTypeDDL").data("kendoDropDownList") != undefined) {
            $("#motorTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#installationConfigurationTypeDDL").data("kendoDropDownList") != undefined) {
            $("#installationConfigurationTypeDDL").data("kendoDropDownList").value("");
        }

        if ($("#airFlowRateTypeDDL").data("kendoDropDownList") != undefined) {
            $("#airFlowRateTypeDDL").data("kendoDropDownList").value("");
        }

        this.gridFilteredData = this.gridData;

        //reset sortby
        //$("#sortProductDLL").kendoDropDownList().select(0);


    }

    public resetSortBy() {
        var sortDDL = $("#sortProductDLL").data("kendoDropDownList");
        if (sortDDL != undefined) {
            sortDDL.select(0);
            sortDDL.trigger("change");
        }


    }

    public updateDropDownLists(data: any) {
        this.productSubFamilyDDLValues = this.getDistinctValues("productSubFamilyId", "productSubFamilyName", data);
        this.productFunctionCategoryDDLValues = this.getDistinctValues("productFunctionCategoryId", "productFunctionCategoryName", data);
        this.unitInstallationTypeDDLValues = this.getDistinctValues("unitInstallationTypeId", "unitInstallationTypeDescription", data);
        this.productPowerVoltageDDLValues = this.getDistinctValues("productPowerVoltageTypeId", "productPowerVoltageTypeDescription", data);
        this.tonnageDDLValues = this.getDistinctValues("tonnage", null, data);
        this.coolingCapacityRatedDDLValues = this.getDistinctValues("coolingCapacityRated", null, data);
        this.coolingCapacityNominalDDLValues = this.getDistinctValues("coolingCapacityNominal", null, data);
        this.heatingCapacityRatedDDLValues = this.getDistinctValues("heatingCapacityRated", null, data);
        this.productCompressorTypeDDLValues = this.getDistinctValues("productCompressorTypeId", "productCompressorTypeDescription", data);
        this.airFlowRateHighCoolingDDLValues = this.getDistinctValues("airFlowRateHighCooling", null, data);
        this.airFlowRateHighHeatingDDLValues = this.getDistinctValues("airFlowRateHighHeating", null, data);
        this.productGasValveTypeDDLValues = this.getDistinctValues("productGasValveTypeId", "productGasValveTypeDescription", data);
        this.productMotorSpeedTypeDDLValues = this.getDistinctValues("productMotorSpeedTypeId", "productMotorSpeedTypeDescription", data);
        this.productInstallationConfigurationTypeDDLValues = this.getDistinctValues("productInstallationConfigurationTypeId", "productInstallationConfigurationTypeDescription", data);
        this.productHeatExchangerTypeDDLValues = this.getDistinctValues("productHeatExchangerTypeId", "productHeatExchangerTypeDescription", data);
        this.productAccessoryDDLValues = this.getDistinctValues("productAccessoryTypeId", "productAccessoryTypeDescription", data);

        //deprecated

        //this.coolingCapacityNominalDDLValues = this.getDistinctValues("coolingCapacityNominal", null, data);
        //this.productCategoryDDLValues = this.getDistinctValues("productCategoryId", "productCategoryName", data);






        this.setupDropDownFilters();
    }

    //public startSpinning(target: any) {
    //    var element = jQuery(target);
    //    kendo.ui.progress(element, true);
    //    //setTimeout(function () {
    //    //    kendo.ui.progress(element, false);
    //    //}, 5000);
    //}

    public viewProductDetails(eventParams: any) {
        this.showProductDetailsEvent.emit(eventParams);
    }

    public setViewOption(viewOpt: any) {
        this.viewOption = viewOpt;
        setTimeout(this.setupPager.bind(this), 200);

    }

    public setActiveViewOption() {
        $('.view-options button').click(function () {
            $('.view-options button').each(function () {
                $(this).removeClass('selected');
            });

            $(this).addClass('selected');

        })
    }



};





