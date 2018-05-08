"use strict";
angular.module("DPO.Projects").controller('ordersController', ['$scope', '$resource', '$http', 'quoteService', function ($scope, $resource, $http, quoteService) {

    var detailExportPromises = [];

    /************************* Master Grid ***********************/

    $scope.ordersDataSource = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: "/api/Order/GetOrdersForGrid"
        },
        sort: ({ field: "orderId", dir: "desc" }),
        pageSize: 20,
        schema: {
            data: function (response) {
                return response.model;
            },
            model: {
                fields: {
                    orderId: { type: 'number' },
                    orderIdStr: { type: 'string' },
                    poNumber: { type: 'string' },
                    erpOrderNumber: { type: 'string' },
                    projectName: { type: 'string' },
                    activeQuoteTitle: { type: 'string' },
                    businessName: { type: 'string' },
                    projectOwnerName: { type: 'string' },
                    dealerContractorName: { type: 'string' },
                    projectId: { type: 'number' },
                    orderStatusTypeId: { type: 'number' },
                    orderStatusDescription: { type: 'string' },
                    projectDate: { type: 'date' },
                    submitDate: { type: 'date' },
                    estimatedReleaseDate: { type: 'date' },
                    estimatedDeliveryDate: { type: 'date' },
                    orderReleaseDate: { type: 'date' },
                    totalListPrice: { type: 'number' },
                    totalNetPrice: { type: 'number' },
                    totalSellPrice: { type: 'number' },
                    darComStatus: { type: 'string' },
                    vrvODUcount: { type: 'number' },
                    splitODUcount: { type: 'number' },
                    pricingTypeId: { type: 'number' },
                    pricingTypeDescription: { type: 'string' },
                    poAttachmentName: { type: 'string' },
                }
            },
            total: "model.length"
        }
    });


    $scope.gridOptions = {
        //toolbar: ["excel"],
        excel: {
            fileName: "Export.xlsx",
            allPages: true,
            filterable: true
        },
        dataSource: $scope.ordersDataSource,
        columnMenu: true,
        pageable: {
            refresh: true,
            buttonCount: 5,
            pageSizes: ["All", 10, 20, 30, 40, 50]
        },
        reorderable: true,
        resizable: true,
        sortable: true,
        filterable: true,

        excelExport: function (e) {
            e.preventDefault();

            //var sheet = e.workbook.sheets[0];
            //for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            //    var row = sheet.rows[rowIndex];
            //    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
            //        row.cells[cellIndex].background = "#D0D7D9";
            //    }
            //}

            //var workbook = e.workbook;

            detailExportPromises = [];

            var masterData = e.data;

            for (var rowIndex = 0; rowIndex < masterData.length; rowIndex++) {
                exportChildData(masterData[rowIndex].quoteId, rowIndex);
            }

            $.when.apply(null, detailExportPromises)
            .then(function () {
                // get the export results
                var detailExports = $.makeArray(arguments);

                // sort by masterRowIndex
                detailExports.sort(function (a, b) {
                    return a.masterRowIndex - b.masterRowIndex;
                });


                /**********Build Excel Sheet***************/
                var dataSource = $("#orders-grid").data("kendoGrid").dataSource;
                var filters = dataSource.filter();
                var sort = dataSource.sort();
                var allData = dataSource.data();
                //var query = new kendo.data.Query(allData);
                //var data = query.filter(filters).data;

                var query = kendo.data.Query.process(allData, {
                    sort: sort,
                    filter: filters
                });

                var data = query.data;

                var orderGrid = $scope.orderGrid;

                var masterRows = [{
                    cells: []
                }];

                var cols = [];

                // build header
                for (var i = 0; i < $scope.orderGrid.columns.length; i++) {
                    if ($scope.orderGrid.columns[i].hidden != true) {
                        var masterheadercell = { value: $scope.orderGrid.columns[i].title, background: "#aabbcc" }
                        masterRows[0].cells.push(masterheadercell);

                        cols.push({ autoWidth: true });
                    }

                }

                for (var i = 0; i < detailExports[0].sheet.rows[0].cells.length; i++) {
                    var detailheadercell = { value: detailExports[0].sheet.rows[0].cells[i].value, background: "#D3E3E8" }
                    masterRows[0].cells.push(detailheadercell);

                    cols.push({ autoWidth: true });
                }

                var extListPriceHeadercell = { value: "Ext. List Pice", background: "#D3E3E8" };
                var extNetPriceHeadercell = { value: "Ext. Net Pice", background: "#D3E3E8" };
                masterRows[0].cells.push(extListPriceHeadercell);
                masterRows[0].cells.push(extNetPriceHeadercell);
                cols.push({ autoWidth: true });
                cols.push({ autoWidth: true });



                //build body
                for (var i = 0; i < data.length; i++) {

                    //get masterbodycells
                    var masterbodycells = [];
                    for (var k = 0; k < $scope.orderGrid.columns.length; k++) {
                        if ($scope.orderGrid.columns[k].hidden != true) {
                            var masterbodycell = { value: data[i][$scope.orderGrid.columns[k].field] }
                            masterbodycells.push(masterbodycell);
                            //row.cells.push(bodycell);
                        }
                    }

                    //get detailbodycells
                    var detailsheet = detailExports[i].sheet;

                    for (var j = 1; j < detailsheet.rows.length; j++) {
                        var row = {     //masterRow
                            cells: []
                        };
                        var detailbodycells = detailsheet.rows[j].cells;

                        //concat masterbodycells & detailbodycells
                        row.cells = masterbodycells.concat(detailbodycells);

                        //Calculate Ext. List Price & Ext. Net Price
                        var qty = detailsheet.rows[j].cells[2];
                        var extListPrice = { value: detailsheet.rows[j].cells[3].value * qty.value }
                        var extNetPrice = { value: detailsheet.rows[j].cells[4].value * qty.value }
                        row.cells.push(extListPrice);
                        row.cells.push(extNetPrice);

                        masterRows.push(row);
                    }

                }

                var workbook = new kendo.ooxml.Workbook({
                    sheets: [
                      {
                          columns: cols,
                          title: "Orders",
                          rows: masterRows
                      }
                    ]
                });
                //save the file as Excel file with extension xlsx
                kendo.saveAs({ dataURI: workbook.toDataURL(), fileName: "ExportDetail.xlsx" });

                /**********End of Build Excel Sheet***************/

                /**********************************/
                //// add an empty column
                //workbook.sheets[0].columns.unshift({
                //    width: 30
                //});

                //// prepend an empty cell to each row
                //for (var i = 0; i < workbook.sheets[0].rows.length; i++) {
                //    workbook.sheets[0].rows[i].cells.unshift({});
                //}


                //// merge the detail export sheet rows with the master sheet rows
                //// loop backwards so the masterRowIndex doesn't need to be updated

                //for (var i = detailExports.length - 1; i >= 0; i--) {
                //    var masterRowIndex = detailExports[i].masterRowIndex + 1; // compensate for the header row

                //    var sheet = detailExports[i].sheet;

                //    // prepend an empty cell to each row
                //    for (var ci = 0; ci < sheet.rows.length; ci++) {
                //        if (sheet.rows[ci].cells[0].value) {
                //            sheet.rows[ci].cells.unshift({});
                //        }
                //    }

                //    // insert the detail sheet rows after the master row
                //    [].splice.apply(workbook.sheets[0].rows, [masterRowIndex + 1, 0].concat(sheet.rows));
                //}
                /**********************************/
                // save the workbook
                //kendo.saveAs({
                //    dataURI: new kendo.ooxml.Workbook(workbook).toDataURL(),
                //    fileName: "ExportDetails.xlsx"
                //});


            });
        },// end of Excel export
        columns: [
            //{
            //    field: "orderIdStr",
            //    title: "Order Number",
            //    width: "12%",
            //    template: "<a href='/Projectdashboard/OrderForm?projectId=#: projectIdStr#&quoteId=#: quoteIdStr#' data-ng-click='Edit(dataItem);'>#: orderIdStr#</a>"
            //},
        {
            field: "projectName",
            title: "Project Name",
            template: "<a href='/Projectdashboard/Project?projectId=#: projectIdStr#&quoteId=#: quoteIdStr#'>#: projectName#</a>"
        },
        {
            field: "activeQuoteTitle",
            title: "Active Quote"
        },
        {
            field: "businessName",
            title: "Business Name",
            hidden: true
        },
        {
            field: "projectOwnerName",
            title: "Project Owner Name",
            hidden: true
        },
        {
            field: "dealerContractorName",
            title: "Dealer/Contractor Name",
            hidden: true
        },
        
        //{
        //    field: "projectIdStr",
        //    title: "Project Ref",
        //    hidden: true
        //},
        {
            field: "projectDate",
            title: "Project Date",
            format: "{0:MM-dd-yyyy}",
            //hidden: true
        },
        {
            field: "orderStatusDescription",
            title: "Order Status",
            template: "<a href='/Projectdashboard/SubmittedOrderForm?projectId=#: projectIdStr#&quoteId=#: quoteIdStr#'>#: orderStatusDescription#</a>",
            attributes: {
                Style: "#if(orderStatusDescription == 'Submitted') { #background-color:\\#D3E7F2;# }" +
                       "else if(orderStatusDescription == 'Picked'){ #background-color:\\#D7D1EB;# }" +
                       "else if(orderStatusDescription == 'Awaiting CSR'){#background-color:\\#F5F0BC;# }" +
                       "else if(orderStatusDescription == 'Accepted'){#background-color:\\#C1D4BE;# }" +
                       "else if(orderStatusDescription == 'Shipped'){#background-color:\\#BCF5D3;# }" +
                       "else if(orderStatusDescription == 'Canceled'){#background-color:\\#C2C8CF;# }" +
                       "else if(orderStatusDescription == 'In Process'){#background-color:\\#EBD1D8;#} #"

            }
        },
        {
            field: "poNumber",
            title: "PO Number",
        },
        {
            field: "erpOrderNumber",
            title: "ERP Order Number",
        },
        {
            field: "poAttachmentName",
            title: "PO Attachment",
            template: "<a href='/ProjectDashboard/GetPOAttachment?quoteId=#:quoteId#&poAttachmentFileName=#:poAttachmentName#'>#: poAttachmentName#</a>"
        },
        {
            field: "submitDate",
            title: "Order Date",
            format: "{0:MM-dd-yyyy}"
        },
        //{
        //    field: "estimatedReleaseDate",
        //    title: "Est. Release Date",
        //    format: "{0:MM-dd-yyyy}",
        //    hidden: true
        //},
        {
            field: "estimatedDeliveryDate",
            title: "Est. Delivery Date",
            format: "{0:MM-dd-yyyy}",
            hidden: true
        },
        {
            field: "orderReleaseDate",
            title: "Order Release Date",
            format: "{0:MM-dd-yyyy}",
            hidden: true
        },
        {
            field: "totalListPrice",
            title: "Total List",
            format: "{0:c}",
            hidden: true
        },
        {
            field: "totalNetPrice",
            title: "Total Net",
            format: "{0:c}"
        },
        {
            field: "totalSellPrice",
            title: "Total Sell",
            format: "{0:c}",
            hidden: true
        },
        {
            field: "darComStatus",
            title: "DAR/COM Status",
            hidden: true
        },
        {
            field: "vrvODUcount",
            title: "VRV ODU #",
            hidden: true
        },
        {
            field: "splitODUcount",
            title: "Split ODU #",
            hidden: true
        },
        {
            field: "pricingTypeDescription",
            title: "Pricing Strategy",
            hidden: true
        }],
        dataBinding: function (e) {
        },// end of dataBinding
        dataBound: function (e) {
            kendo.ui.progress($("#orders-grid"), false);
        }// end of dataBound



    };// end of grid option

    /******************** End of Master Grid ***********************/


    /************************* Detail Grid ***********************/

    $scope.getDetailDataSource = function (quoteId) {
        $scope.DetailDataSource = new kendo.data.DataSource({
            type: "json",
            transport: {
                read: "/api/Quote/GetQuoteItems",
                parameterMap: function (options, operation) {
                    if (operation !== "read") {
                        return null;
                    } else if (operation == "read") {
                        return { quoteId: quoteId };

                    }
                }
            },
            serverPaging: false,
            //pageSize: 10,
            schema: {
                data: function (response) {
                    return response.model;
                },
                //model: {
                //    fields: {
                //        id: 'Id'
                //    }
                //},
                //total: "TotalRowCount"
            }
        });
        return $scope.DetailDataSource;
    };

    $scope.detailGridOptions = function (dataItem) {
        return {
            dataSource: $scope.getDetailDataSource(dataItem.quoteId),
            //toolbar: ["excel"],
            //excel: {
            //    fileName: "Order Detail Export.xlsx",
            //    filterable: true
            //},
            excelExport: function (e) {
                // prevent saving the file
                e.preventDefault();
                // resolve the deferred
                //deferred.resolve({
                //  masterRowIndex: masterRowIndex,
                //  sheet: e.workbook.sheets[0]
                //});
            },
            sortable: false,
            //filterable: true,
            pageable: false,
            reorderable: false,
            resizable: true,
            columns: [{
                field: "productNumber",
                title: "Product",
                width: "10%"
            }, {
                field: "description",
                title: "Product Description",

            }, {
                field: "quantity",
                title: "Qty",
                width: "5%"
            }, {
                field: "listPrice",
                title: "List Price Each",
                format: "{0:c}"
            }, {
                field: "netPrice",
                title: "Net Price Each",
                format: "{0:c}"
            }, {
                title: "Ext. List Price",
                template: function (data) { return kendo.toString(data.listPrice * data.quantity, 'c'); }
            }, {
                title: "Ext. Net Price",
                template: function (data) { return kendo.toString(data.netPrice * data.quantity, 'c'); }
            }]
        };
    };// end of detailGridOptions

    /********************End of Detail Grid ***********************/

    $scope.ExportExcelOptions = function () {
        $scope.ExcelExportWindow.center();
        $scope.ExcelExportWindow.open();
    }

    /********* Export master grid only **********/
    //$scope.ExportExcel = function () {
    //    var grid = $("#orders-grid").data("kendoGrid");
    //    grid.saveAsExcel();
    //    $scope.ExcelExportWindow.close();
    //}

    $scope.ExportExcel = function () {

        //dataSource.filter({ field: "quoteId", operator: "eq", value: quoteId });

        var dataSource = $("#orders-grid").data("kendoGrid").dataSource;
        var filters = dataSource.filter();
        var sort = dataSource.sort();
        var allData = dataSource.data();
        //var query = new kendo.data.Query(allData);
        //var data = query.filter(filters).data;

        var query = kendo.data.Query.process(allData, {
            sort: sort,
            filter: filters
        });

        var data = query.data;

        var orderGrid = $scope.orderGrid;

        var rows = [{
            cells: []
        }];

        var cols = [];
        // build header

        for (var i = 0; i < $scope.orderGrid.columns.length; i++) {
            if ($scope.orderGrid.columns[i].hidden != true) {
                var headercell = { value: $scope.orderGrid.columns[i].title, background: "#aabbcc" }
                rows[0].cells.push(headercell);

                cols.push({ autoWidth: true });
            }

        }

        //build body

        for (var j = 0; j < data.length; j++) {
            var row = {
                cells: []
            };

            for (var k = 0; k < $scope.orderGrid.columns.length; k++) {
                if ($scope.orderGrid.columns[k].hidden != true) {
                    var bodycell = { value: data[j][$scope.orderGrid.columns[k].field] }
                    row.cells.push(bodycell);
                }
            }

            rows.push(row);

        }


        var workbook = new kendo.ooxml.Workbook({
            sheets: [
              {
                  columns: cols,
                  title: "Orders",
                  rows: rows
              }
            ]
        });
        //save the file as Excel file with extension xlsx
        kendo.saveAs({ dataURI: workbook.toDataURL(), fileName: "Export.xlsx" });

        $scope.ExcelExportWindow.close();

    }// end of export to excel

    /*********End of Export master grid **********/

    /********* Export master and detail grid **********/
    $scope.ExportExcelDetailed = function () {
        var grid = $("#orders-grid").data("kendoGrid");
        grid.saveAsExcel();
        $scope.ExcelExportWindow.close();

        //    //dataSource.filter({ field: "quoteId", operator: "eq", value: quoteId });

        //var dataSource = $("#orders-grid").data("kendoGrid").dataSource;
        //var filters = dataSource.filter();
        //var sort = dataSource.sort();
        //var allData = dataSource.data();
        ////var query = new kendo.data.Query(allData);
        ////var data = query.filter(filters).data;

        //var query = kendo.data.Query.process(allData, {
        //    sort: sort,
        //    filter: filters
        //});

        //var masterdata = query.data;


        //var detailDataSource = $("#orders-grid").data("kendoGrid").dataSource;

        //var orderGrid = $scope.orderGrid;

        //var rows = [{
        //    cells: []
        //}];

        ////$scope.rows = [
        ////                { cells: [] }
        ////                ];

        ////var cols = [];
        //// build header

        //var masterGridColumnHeaders = []
        //for (var i = 0; i < $scope.orderGrid.columns.length; i++) {
        //    if ($scope.orderGrid.columns[i].hidden != true) {
        //        var headercell = { value: $scope.orderGrid.columns[i].title, background: "#aabbcc" }
        //        //rows[0].cells.push(headercell);
        //        masterGridColumnHeaders.push(headercell);

        //        //cols.push({ autoWidth: true });
        //    }

        //}

        //var detailGridColumnHeaders = [{ value: "Product", background: "#aabbcc" },
        //                                    { value: "Product Description", background: "#aabbcc" },
        //                                    { value: "Qty", background: "#aabbcc" },
        //                                    { value: "List Price Each", background: "#aabbcc" },
        //                                    { value: "Net Price Each", background: "#aabbcc" },
        //                                    { value: "Ext. List Price", background: "#aabbcc" },
        //                                    { value: "Ext. Net Price", background: "#aabbcc" }];

        //rows[0].cells = masterGridColumnHeaders.concat(detailGridColumnHeaders);

        ////build body

        //var masterRowCount = 0;
        //for (var rowIndex = 0; rowIndex < masterdata.length; rowIndex++) {
        //    var row = {
        //        cells: []
        //    };

        //    //for each row in detail grid 

        //    //push master grid cells
        //    for (var k = 0; k < $scope.orderGrid.columns.length; k++) {
        //        if ($scope.orderGrid.columns[k].hidden != true) {
        //            var bodycell = { value: masterdata[rowIndex][$scope.orderGrid.columns[k].field] }
        //            row.cells.push(bodycell);

        //        }
        //    }

        //    //push detail grid cells
        //    //get detail datasource
        //    exportChildData(masterdata[rowIndex].quoteId, rowIndex);

        //    //var detailDS = $scope.getDetailDataSource(masterdata[j].quoteId);

        //    //quoteService.getQuoteItems(masterdata[j].quoteId).perform({}).$promise.then(function (result) {
        //    //    if (result.isok) {
        //    //        var detailDS = result.model;

        //    //        //for (var l = 0; l < detailDS.length; l++) {
        //    //        //    var detailcell = { value: detailDS.productNumber }
        //    //        //    row.cells.push(detailcell);
        //    //        //}

        //    //        //push items to row.cells

        //    //        //push row to rows
        //    //        //$scope.rows.push(row);

        //    //        //masterRowCount++;

        //    //        ////
        //    //        //if (masterRowCount == masterdata.length) {
        //    //        //    var workbook = new kendo.ooxml.Workbook({
        //    //        //        sheets: [
        //    //        //          {
        //    //        //              //columns: cols,
        //    //        //              title: "Orders",
        //    //        //              rows: $scope.rows
        //    //        //          }
        //    //        //        ]
        //    //        //    });
        //    //        //    //save the file as Excel file with extension xlsx
        //    //        //    kendo.saveAs({ dataURI: workbook.toDataURL(), fileName: "ExportDetail.xlsx" });

        //    //        //    $scope.ExcelExportWindow.close();
        //    //        //}

        //    //    }
        //    //});




        //    rows.push(row);

        //}


        //var workbook = new kendo.ooxml.Workbook({
        //    sheets: [
        //      {
        //          //columns: cols,
        //          title: "Orders",
        //          rows: rows
        //      }
        //    ]
        //});
        ////save the file as Excel file with extension xlsx
        //kendo.saveAs({ dataURI: workbook.toDataURL(), fileName: "ExportDetail.xlsx" });

        //$scope.ExcelExportWindow.close();
    }

    function exportChildData(quoteId, rowIndex) {
        var deferred = $.Deferred();

        detailExportPromises.push(deferred);

        //var rows = [{
        //    cells: [
        //      { value: "Product" },
        //      { value: "Description" },
        //      { value: "Quantity" },
        //      { value: "List Price" },
        //      { value: "Net Price" }
        //        ]
        //}];

        //dataSource.filter({ field: "quoteId", operator: "eq", value: quoteId });

        var exporter = new kendo.ExcelExporter({
            columns: [{
                field: "productNumber",
                title: "Product"
            }, {
                field: "description",
                title: "Description"
            }, {
                field: "quantity",
                title: "Quantity"
            }, {
                field: "listPrice",
                title: "List Price"
            }, {
                field: "netPrice",
                title: "Net Price"
            }
            ],
            dataSource: $scope.getDetailDataSource(quoteId)
        });

        exporter.workbook().then(function (book, data) {
            deferred.resolve({
                masterRowIndex: rowIndex,
                sheet: book.sheets[0]
            });
        });
    }


    /********* End of Export master and detail grid **********/

    $("#OrderSearchBtn").click(function () {
        var value = $("#orderSearchBox")[0].value;
        if (value) {// apply search filter
            if ($("#orders-grid").data("kendoGrid").dataSource.filter() == undefined) { // No filter existed
                $("#orders-grid").data("kendoGrid").dataSource.filter([{
                    "name": "search",
                    "logic": "or",
                    "filters": [
                        {
                            "field": "orderIdStr",
                            "operator": "contains",
                            "value": value
                        },
                        {
                            "field": "poNumber",
                            "operator": "contains",
                            "value": value
                        },
                        {
                            "field": "projectIdStr",
                            "operator": "contains",
                            "value": value
                        },
                        {
                            "field": "projectName",
                            "operator": "contains",
                            "value": value
                        }
                    ]
                }]);
            } else {
                // remove existing filter
                var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                var searchfilterArray = null;
                for (var i = 0; i < filterArray.length; i++) {// look for search filter

                    if (filterArray[i].name == "search") { // found
                        searchfilterArray = filterArray[i].filters;
                        if (searchfilterArray != undefined) { //searchfilterA existed

                            var count = searchfilterArray.length;
                            for (var j = 0; j < count; j++) {
                                searchfilterArray.pop();
                            }

                            filterArray.splice(i, 1);
                            // apply new search filter
                            $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({
                                "name": "search",
                                "logic": "or",
                                "filters": [
                                    {
                                        "field": "orderIdStr",
                                        "operator": "contains",
                                        "value": value
                                    },
                                    {
                                        "field": "poNumber",
                                        "operator": "contains",
                                        "value": value
                                    },
                                    {
                                        "field": "projectIdStr",
                                        "operator": "contains",
                                        "value": value
                                    },
                                    {
                                        "field": "projectName",
                                        "operator": "contains",
                                        "value": value
                                    }
                                ]
                            });
                        }


                    }
                }// end of for looking for search filter
                if (searchfilterArray == null) { // search filter not existed
                    $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({
                        "name": "search",
                        "logic": "or",
                        "filters": [
                            {
                                "field": "orderIdStr",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "poNumber",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "projectIdStr",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "projectName",
                                "operator": "contains",
                                "value": value
                            }
                        ]
                    });
                }
            }

        } else { // clear search filer
            var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
            if (filterArray) {
                for (var i = 0; i < filterArray.length; i++) {// look for search filter
                    if (filterArray[i].name == "search") {
                        var searchfilterArray = filterArray[i].filters;

                        var count = searchfilterArray.length;
                        for (var j = 0; j < count; j++) {
                            searchfilterArray.pop();
                        }

                        filterArray.splice(i, 1);

                    }
                }
            }



        }
        var curr_filters = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
        $("#orders-grid").data("kendoGrid").dataSource.filter(curr_filters);
        //$('#orders-grid').data('kendoGrid').dataSource.read();

    });


    var userAutoComplete = $("#userAutoComplete").kendoAutoComplete({ // projectOwnerName
        minLength: 1,
        dataTextField: "userFullName",
        width: "200px",
        dataSource: {
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/User/GetUsersViewable"
                }
            },
            schema: {
                data: function (response) {
                    return response.model;
                }
            }
        },
        change: function () {
            var value = this.value();
            if (value) {
                if ($("#orders-grid").data("kendoGrid").dataSource.filter() == undefined) {
                    // add new filter
                    $("#orders-grid").data("kendoGrid").dataSource.filter({ field: "projectOwnerName", operator: "eq", value: value });
                } else {
                    // remove existing filter
                    var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                    for (var i = 0; i < filterArray.length; i++) {
                        if (filterArray[i].field == "projectOwnerName") {
                            filterArray.splice(i, 1);
                        }
                    }
                    // add new filter
                    $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({ field: "projectOwnerName", operator: "eq", value: value });
                }

            } else {
                var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                for (var i = 0; i < filterArray.length; i++) {
                    if (filterArray[i].field == "projectOwnerName") {
                        filterArray.splice(i, 1);
                    }
                }
            }
            var curr_filters = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
            $("#orders-grid").data("kendoGrid").dataSource.filter(curr_filters);
            //$('#orders-grid').data('kendoGrid').dataSource.read();
        }
    });



    var orderStatusDDL = $("#orderStatusDDL").kendoDropDownList({
        dataTextField: "displayText",
        dataValueField: "keyId",
        autoBind: false,
        optionLabel: "All",
        dataSource: {
            type: "json",
            transport: {
                read: "/api/Order/GetOrderStatusTypes"
            },
            schema: {
                data: function (response) {
                    return response.model;
                }
            }

        },
        change: function () {
            //var value = this.value();
            var value = this.text();
            if (value && value != "All") {
                if ($("#orders-grid").data("kendoGrid").dataSource.filter() == undefined) {
                    // add new filter
                    $("#orders-grid").data("kendoGrid").dataSource.filter([{ field: "orderStatusDescription", operator: "eq", value: value }]);
                } else {
                    // remove existing filter
                    var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                    for (var i = 0; i < filterArray.length; i++) {
                        if (filterArray[i].field == "orderStatusDescription") {
                            filterArray.splice(i, 1);
                        }
                    }
                    // add new filter
                    $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({ field: "orderStatusDescription", operator: "eq", value: value });

                }
            } else {
                // remove existing filter
                var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                for (var i = 0; i < filterArray.length; i++) {
                    if (filterArray[i].field == "orderStatusDescription") {
                        filterArray.splice(i, 1);
                    }
                }

            }
            var curr_filters = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
            $("#orders-grid").data("kendoGrid").dataSource.filter(curr_filters);
            //$('#orders-grid').data('kendoGrid').dataSource.read();
        }
    });// end of dropdown list




    var businessAutoComplete = $("#businessAutoComplete").kendoAutoComplete({
        minLength: 1,
        dataTextField: "businessName",
        width: "200px",
        dataSource: {
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/Business/GetBusinessList"
                }
            },
            schema: {
                data: function (response) {
                    return response.model;
                }
            }
        },
        change: function () {
            var value = this.value();
            if (value) {
                if ($("#orders-grid").data("kendoGrid").dataSource.filter() == undefined) {
                    $("#orders-grid").data("kendoGrid").dataSource.filter({ field: "businessName", operator: "eq", value: value });
                } else {
                    // remove existing filter
                    var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                    for (var i = 0; i < filterArray.length; i++) {
                        if (filterArray[i].field == "businessName") {
                            filterArray.splice(i, 1);
                        }
                    }
                    // add new filter
                    $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({ field: "businessName", operator: "eq", value: value });
                }

            } else {
                // remove existing filter
                var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
                for (var i = 0; i < filterArray.length; i++) {
                    if (filterArray[i].field == "businessName") {
                        filterArray.splice(i, 1);
                    }
                }

            }
            var curr_filters = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
            $("#orders-grid").data("kendoGrid").dataSource.filter(curr_filters);
            //$('#orders-grid').data('kendoGrid').dataSource.read();
        }
    });

    //var pricingTypeDDL = $("#pricingTypeDDL").kendoDropDownList({
    //    dataTextField: "displayText",
    //    dataValueField: "keyId",
    //    autoBind: false,
    //    optionLabel: "All",
    //    dataSource: [{keyId: 1, displayText: "Buy/Sell"},
    //                 {keyId: 2, displayText: "Commission"}],
    //    change: function () {
    //        //var value = this.value();
    //        var value = this.text();
    //        if (value && value != "All") {
    //            if ($("#orders-grid").data("kendoGrid").dataSource.filter() == undefined) {
    //                // add new filter
    //                $("#orders-grid").data("kendoGrid").dataSource.filter([{ field: "pricingTypeDescription", operator: "eq", value: value }]);
    //            } else {
    //                // remove existing filter
    //                var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
    //                for (var i = 0; i < filterArray.length; i++) {
    //                    if (filterArray[i].field == "pricingTypeDescription") {
    //                        filterArray.splice(i, 1);
    //                    }
    //                }
    //                // add new filter
    //                $("#orders-grid").data("kendoGrid").dataSource.filter().filters.push({ field: "pricingTypeDescription", operator: "eq", value: value });

    //            }
    //        } else {
    //            // remove existing filter
    //            var filterArray = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
    //            for (var i = 0; i < filterArray.length; i++) {
    //                if (filterArray[i].field == "pricingTypeDescription") {
    //                    filterArray.splice(i, 1);
    //                }
    //            }
    //        }
    //        var curr_filters = $("#orders-grid").data("kendoGrid").dataSource.filter().filters;
    //        $("#orders-grid").data("kendoGrid").dataSource.filter(curr_filters);
    //        //$('#orders-grid').data('kendoGrid').dataSource.read();
    //    }
    //});// end of dropdown list


    $scope.Edit = function (dataitem) {
        var order = dataitem;
        //$resource('/ProjectDashboard/OrderForm', { projectId: order.projectIdStr, quoteId: order .quoteIdStr}).get().$promise.then(function (result) {
        //    if (!result.isok) {
        //    }
        //});
    }

}]);