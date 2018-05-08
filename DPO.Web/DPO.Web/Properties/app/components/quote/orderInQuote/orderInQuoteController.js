(
function () {
    "use strict";
    var app = angular.module("DPO.Projects");

    app.controller('orderInQuoteController', ['$rootScope', '$scope', '$resource', '$http', function ($rootScope, $scope, $resource, $http) {

        $scope.vm = new QuoteItemsViewModel();

        $scope.ordersDataSource = new kendo.data.DataSource({
            type: "json",
            transport: {
                read: "/api/Order/GetOrderInQuote?quoteId=" + $scope.vm.quoteIdStr,
                //parameterMap: function (options, operation) {
                //    if (operation !== "read" && options.models) {
                //        return { models: kendo.stringify(options.models) };
                //    } else if (operation == "read") {
                //        return { quoteId: $scope.Data.Id };
                //    }
                //}
                printOrder: {
                    url: "@Url.Action('OrderPrint','Document')"
                }
            },
            serverPaging: true,
            serverSorting: true,
            schema: {
                data: function (response) {
                    return response.model; 
                },
                model: {
                    fields: {
                        submitDate: { type: 'date' },
                        estimatedReleaseDate: { type: 'date' },
                        orderReleaseDate: { type: 'date'},
                        erpShipDate: { type: 'date' },
                        poAttachmentFileName: {type: 'string'},
                    }
                },
            }
        });

        $scope.ordersGridOptions = {
            dataSource: $scope.ordersDataSource,
            //reorderable: true,
            resizable: true,
            pageable: false,
            columns: [
            {
                field: "orderId",
                title: "Action",
                width: "80px",
                template: "<button type='button' onclick='triggerAction();' target='_blank' ='Print Order'>##Print Order##</button>"
            },
            {
                field: "orderId",
                title: "Order Reference",
                width: "100px",
                template: "<a href='/Projectdashboard/SubmittedOrderForm?projectId=#: projectId#&quoteId=#: quoteId#' data-ng-click='Edit(dataItem);'>#: orderId#</a>"
            }, {
                field: "submitDate",
                title: "Order Date",
                width: "100px",
                format: "{0:MM-dd-yyyy}"
            }, {
                field: "estimatedReleaseDate",
                title: "Release Date",
                width: "100px",
                format: "{0:MM-dd-yyyy}"
            }, {
                field: "orderReleaseDate",
                title: "Order Release Date",
                width: "100px",
                format: "{0:MM-dd-yyyy}"
            }, {
                field: "orderStatusDescription",
                title: "Order Status",
                width: "80px",
                attributes: {
                    Style: "#if(orderStatusDescription == 'Submitted') { #background-color:\\#5499C7;color:white# }" +
                           "else if(orderStatusDescription == 'Shipped'){ #background-color:\\#16A085;color:white# }" +
                           "else if(orderStatusDescription == 'Awaiting CSR'){#background-color:\\#F1C40F;color:white# }" +
                           "else if(orderStatusDescription == 'Accepted'){#background-color:\\#1B4F72;color:white# }" +
                           "else if(orderStatusDescription == 'Shipped'){#background-color:\\#52BE80;color:white# }" +
                           "else if(orderStatusDescription == 'Canceled'){#background-color:\\#95A5A6;color:white# }" +
                           "else if(orderStatusDescription == 'In Process'){#background-color:\\#EDBB99;color:white#} #"
                }
            }, {
                field: "poNumber",
                title: "Purchase Order",
                width: "80px"
            }, {

                field: "poAttachmentFileName",
                title: "PO Attachment",
                width: "120px;",
                template: "<a href='/ProjectDashboard/GetPOAttachment?quoteId=#:quoteId#&poAttachmentFileName=#:poAttachmentFileName#' target='_blank'>#: poAttachmentFileName#</a>"
            }]

        };

      

        $scope.Edit = function (dataitem) {
            var order = dataitem;
            //$resource('/ProjectDashboard/OrderForm', { projectId: order.projectIdStr, quoteId: order .quoteIdStr}).get().$promise.then(function (result) {
            //    if (!result.isok) {
            //    }
            //});
        }

        

    }]);// end of controller



})();