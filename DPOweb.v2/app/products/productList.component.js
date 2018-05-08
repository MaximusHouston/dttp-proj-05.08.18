"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var product_service_1 = require("./services/product.service");
var kendo_data_query_1 = require("@progress/kendo-data-query");
var kendo_data_query_2 = require("@progress/kendo-data-query");
/*
interface Item {
  text: string,
  value: number
}*/
var ProductListComponent = /** @class */ (function () {
    function ProductListComponent(toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, productSvc) {
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.productSvc = productSvc;
        //public productFamilyId: any;
        //public productModelTypeId: any;
        //public unitInstallationTypeId: any;
        this.updateBasketEvent = new core_1.EventEmitter();
        this.showProductDetailsEvent = new core_1.EventEmitter();
        this.pageSize = 100;
        this.skip = 0;
        this.viewOption = 1;
        //Dropdowns
        //public sortByDDLValues: any = [{ text: "Model No.", value: "product.productNumber" },
        //                                { text: "Product description", value: "product.name" }];
        this.productSortBy = "product.productNumber";
        this.productFilters = [];
        this.defaultItem = { text: "Select...", value: null };
        this.productStatusOptions = [];
        this.inventoryStatusOptions = [];
        /*
        public productStatusOptions : Array<{ text: string, value: any }> = [
            { text: "Released", value: 111267 },
            { text: "Obsolete", value: 111268 },
            { text: "Hidden Module Unit", value: 111269 },
            { text: "All", value: null }
        ];*/
        //public selectedProductStatus: { text: string, value: any } = { text: "Released", value: 111267 };
        this.selectedProductStatus = 111267;
        this.selectedInventoryStatus = 0;
        this.firstViewCheckAfterOnChange = true;
    }
    ProductListComponent.prototype.ngOnChanges = function () {
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
        }
        else {
            this.selectedProductStatus = this.enums.ProductStatusTypeEnum.Active;
        }
        this.selectedInventoryStatus = 0; // All
        this.setDefaultFilters(this.productsModel.isSearch);
    };
    ProductListComponent.prototype.ngOnInit = function () {
        //console.log("ProductList: ngOnInit");
        var self = this;
        this.productSvc.getBasketQuoteId().then(this.getBasketQuoteIdCallback.bind(this));
        //var data = this.productSvc.getProducts().then(this.getProductsCallback.bind(this));
        this.setupAddProductsBtn();
        this.getProductStatusOptions();
        this.getInventoryStatusOptions();
    };
    ProductListComponent.prototype.ngDoCheck = function () {
        //console.log("ProductList: ngDoCheck");
    };
    ProductListComponent.prototype.ngAfterContentInit = function () {
        //console.log("ProductList: ngAfterContentInit");
    };
    ProductListComponent.prototype.ngAfterContentChecked = function () {
        //console.log("ProductList: ngAfterContentChecked");
    };
    ProductListComponent.prototype.ngAfterViewInit = function () {
        //console.log("ProductList: ngAfterViewInit");
        setTimeout(this.setupDropDownFilters.bind(this), 200); // wait 0.2 sec
        this.setActiveViewOption();
        //this.setupPager();
    };
    ProductListComponent.prototype.ngAfterViewChecked = function () {
        //console.log("ProductList: ngAfterViewChecked");
        if (this.firstViewCheckAfterOnChange) {
            this.updateDropDownLists(this.gridData);
            this.firstViewCheckAfterOnChange = false;
        }
    };
    ProductListComponent.prototype.getProductStatusOptions = function () {
        var _this = this;
        this.productSvc.getProductStatuses().then(function (resp) {
            if (resp.isok) {
                _this.productStatusOptions = resp.model;
            }
            else {
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            console.log(error);
        });
    };
    ProductListComponent.prototype.getInventoryStatusOptions = function () {
        var _this = this;
        this.productSvc.getInventoryStatuses().then(function (resp) {
            if (resp.isok) {
                _this.inventoryStatusOptions = resp.model;
            }
            else {
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            console.log(error);
        });
    };
    ProductListComponent.prototype.setDefaultFilters = function (isSearch) {
        if (!isSearch) {
            this.productFilters.push({ field: "product.productStatusTypeId", operator: "eq", value: this.enums.ProductStatusTypeEnum.Active });
        }
        this.applyFilters();
    };
    ProductListComponent.prototype.setupAddProductsBtn = function () {
        var self = this;
        jQuery("#addProductsToQuoteBtn").click(function () {
            var data = {
                "Products": self.gridData
            };
            self.loadingIconSvc.Start(jQuery("#productPageContainer"));
            self.productSvc.addProductsToQuote(data).then(function (resp) {
                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));
                self.toastrSvc.displayResponseMessages(resp);
                self.clearQuantities();
                self.updateBasketEvent.emit();
                //self.reloadGrid();
            }, function (resp) {
                self.loadingIconSvc.Stop(jQuery("#productPageContainer"));
                self.toastrSvc.displayResponseMessages(resp);
                //TODO: create a log service for this
                for (var _i = 0, _a = resp.messages.items; _i < _a.length; _i++) {
                    var message = _a[_i];
                    console.log(message.text);
                }
            });
        });
    };
    ProductListComponent.prototype.clearQuantities = function () {
        for (var i = 0; i < this.gridViewData.data.length; i++) {
            this.gridViewData.data[i].product.quantity = 0;
        }
    };
    ProductListComponent.prototype.setupPager = function () {
        if (this.viewOption == 1) {
            this.setupGridHeight();
        }
        else {
            if (this.viewOption == 0) {
                var grid = $("#product-list-view");
            }
            else if (this.viewOption == 2) {
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
                var offset = $(this).scrollTop(), tableOffsetTop = grid.offset().top, tableOffsetBottom = tableOffsetTop + grid.height() - pager.height();
                if (offset < tableOffsetTop || offset > tableOffsetBottom) {
                    pager.removeClass("fixed-pager");
                    $("#scrollUpBtn").css("display", "none");
                }
                else if (offset >= tableOffsetTop && offset <= tableOffsetBottom) {
                    pager.addClass("fixed-pager");
                    $("#scrollUpBtn").css("display", "block");
                }
            }
        }
    };
    ProductListComponent.prototype.scrollUp = function () {
        $('html, body').animate({ scrollTop: 0 }, 300);
    };
    ProductListComponent.prototype.setupGridHeight = function () {
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
    };
    ProductListComponent.prototype.setupDropDownFilters = function () {
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
    };
    ProductListComponent.prototype.pageChange = function (event) {
        this.skip = event.skip;
        this.loadProducts();
        var elem;
        if (this.viewOption == 1) {
            elem = document.querySelector("#product-table-view table tbody tr");
            elem.scrollIntoView();
        }
        else {
            elem = document.getElementById("productFamilyName");
            window.scrollTo(0, elem.offsetTop);
        }
    };
    ProductListComponent.prototype.getDistinctValues = function (valueId, description, data) {
        var flags = []; // flags[] is to keep track of what value is already added to distinctValues[]
        var distinctValues = [];
        var productArray = data;
        for (var i = 0; i < productArray.length; i++) {
            var item = { text: "", value: 0 };
            var value = productArray[i].product[valueId];
            if (description == null) {
                var text = productArray[i].product[valueId];
            }
            else {
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
    };
    ProductListComponent.prototype.sortDistinctValues = function (distinctValues) {
        var sortedValues = [];
        if (distinctValues.length > 0) {
            if (isNaN(distinctValues[0].text)) {
                sortedValues = distinctValues.sort(function (a, b) {
                    if (a.text < b.text)
                        return -1;
                    if (a.text > b.text)
                        return 1;
                    return 0;
                });
            }
            else {
                sortedValues = distinctValues.sort(function (a, b) { return a.text - b.text; });
            }
        }
        else {
            return distinctValues;
        }
        return sortedValues;
    };
    ProductListComponent.prototype.getBasketQuoteIdCallback = function (resp) {
        if (resp.isok) {
            this.basketQuoteId = resp.model;
        }
    };
    ProductListComponent.prototype.loadProducts = function () {
        if (this.productsModel != undefined) {
            this.gridViewData = {
                data: this.gridFilteredData.slice(this.skip, this.skip + this.pageSize),
                total: this.gridFilteredData.length
            };
        }
    };
    ProductListComponent.prototype.loadProductsNoFilter = function () {
        if (this.productsModel != undefined) {
            this.gridViewData = {
                data: this.gridData.slice(this.skip, this.skip + this.pageSize),
                total: this.gridData.length
            };
        }
        this.updateDropDownLists(this.gridData);
        //this.setupDropDownFilters();
    };
    //This function is used for kendo-dropdownlist (angular 2)
    ProductListComponent.prototype.productFilter = function (selectedObj, field) {
        var fieldName = "product." + field;
        if (selectedObj == undefined || selectedObj.value == null) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            if (this.productFilters.length > 0) {
                this.applyFilters();
            }
            else {
                this.loadProductsNoFilter();
            }
        }
        else {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: selectedObj.value });
            this.applyFilters();
        }
    };
    ProductListComponent.prototype.productFilterByDecimalValue = function (selectedObj, field) {
        var fieldName = "product." + field;
        if (selectedObj == undefined || selectedObj.value == null) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            if (this.productFilters.length > 0) {
                this.applyFilters();
            }
            else {
                this.loadProductsNoFilter();
            }
        }
        else {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: selectedObj.valueDecimal });
            this.applyFilters();
        }
    };
    ProductListComponent.prototype.productFilterPrimitive = function (value, field) {
        var fieldName = "product." + field;
        if (value == null || value == 0) {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            if (this.productFilters.length > 0) {
                this.applyFilters();
            }
            else {
                this.loadProductsNoFilter();
            }
        }
        else {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: value });
            this.applyFilters();
        }
    };
    //This function is used for kendo-DDL (JQuery)
    ProductListComponent.prototype.productFilterJQ = function (value, field) {
        var fieldName = "product." + field;
        if (value == undefined || value == null || value == "") {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            if (this.productFilters.length > 0) {
                this.applyFilters();
            }
            else {
                this.loadProductsNoFilter();
            }
        }
        else {
            //Look for the filter by name and remove it from productFilters
            this.removeFilterByName(fieldName);
            //add new filter value
            this.productFilters.push({ field: fieldName, operator: "eq", value: value });
            this.applyFilters();
        }
    };
    ProductListComponent.prototype.removeFilterByName = function (fieldName) {
        for (var i = 0; i < this.productFilters.length; i++) {
            if (this.productFilters[i].field == fieldName) {
                this.productFilters.splice(i, 1);
            }
        }
    };
    ProductListComponent.prototype.applyFilters = function () {
        this.skip = 0; // reset to page 1
        var result = kendo_data_query_1.process(this.gridData, {
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
    };
    //sortBy JQuery style
    ProductListComponent.prototype.sortBy = function (fieldName) {
        var result = kendo_data_query_2.orderBy(this.gridFilteredData, [{ field: fieldName, dir: "asc" }]);
        this.gridFilteredData = result; // filtered and sorted data
        this.gridViewData = {
            data: result.slice(this.skip, this.skip + this.pageSize),
            total: result.length
        };
    };
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
    ProductListComponent.prototype.clearFilters = function () {
        this.resetFilters();
        this.loadProductsNoFilter();
        this.resetSortBy();
    };
    ProductListComponent.prototype.resetFilters = function () {
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
        this.selectedProductStatus = 0; // All
        this.selectedInventoryStatus = 0; // All
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
    };
    ProductListComponent.prototype.resetSortBy = function () {
        var sortDDL = $("#sortProductDLL").data("kendoDropDownList");
        if (sortDDL != undefined) {
            sortDDL.select(0);
            sortDDL.trigger("change");
        }
    };
    ProductListComponent.prototype.updateDropDownLists = function (data) {
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
    };
    //public startSpinning(target: any) {
    //    var element = jQuery(target);
    //    kendo.ui.progress(element, true);
    //    //setTimeout(function () {
    //    //    kendo.ui.progress(element, false);
    //    //}, 5000);
    //}
    ProductListComponent.prototype.viewProductDetails = function (eventParams) {
        this.showProductDetailsEvent.emit(eventParams);
    };
    ProductListComponent.prototype.setViewOption = function (viewOpt) {
        this.viewOption = viewOpt;
        setTimeout(this.setupPager.bind(this), 200);
    };
    ProductListComponent.prototype.setActiveViewOption = function () {
        $('.view-options button').click(function () {
            $('.view-options button').each(function () {
                $(this).removeClass('selected');
            });
            $(this).addClass('selected');
        });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "productsModel", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "productFamilyId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "productTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "productModelTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "unitInstallationTypeId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProductListComponent.prototype, "productClassPIMId", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductListComponent.prototype, "updateBasketEvent", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProductListComponent.prototype, "showProductDetailsEvent", void 0);
    ProductListComponent = __decorate([
        core_1.Component({
            selector: 'product-list',
            templateUrl: 'app/products/productList.component.html',
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService, user_service_1.UserService,
            systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums,
            product_service_1.ProductService])
    ], ProductListComponent);
    return ProductListComponent;
}());
exports.ProductListComponent = ProductListComponent;
;
//# sourceMappingURL=productList.component.js.map