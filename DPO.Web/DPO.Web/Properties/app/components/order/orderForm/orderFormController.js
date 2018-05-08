"use strict";
angular.module("DPO.Projects").controller('orderFormController', ['$scope', '$log', '$resource', 'quoteService', 'orderService', 'DPOprojectService', 'addressService', function ($scope, $log, $resource, quoteService, orderService, DPOprojectService, addressService, $templateCache) {

    $scope.OrderVM = new OrderViewModel();
    $scope.HasServerError = false;
    $scope.HasPageMessage = false;
    $scope.UploadPOSuccess = false;
    $scope.ValidationError = [];
    $scope.ServerErrors = [
        //{ "key": "PONumber", "message": "PONumber invalid 1" },
        //{ "key": "PONumber", "message": "PONumber invalid 2" },
        //{ "key": "DeliveryContactPhone", "message": "Delivery Contact Phone invalid" }
    ];
    $scope.PageMessages = [];
    var productDataSource = {}

    DPOprojectService.getProject($scope.OrderVM.projectIdStr).perform({}).$promise.then(function (result) {
        if (result.isok) {
            $scope.projectVM = result.model;
            $scope.oldProjectVM = angular.copy($scope.projectVM);
            $scope.projectVMLoaded = true;

        }
    });

    quoteService.getQuoteItems($scope.OrderVM.quoteIdStr).perform({}).$promise.then(function (result) {
        if (result.isok) {
            productDataSource = result.model;

            $scope.productGridOptions = {
                dataSource: productDataSource,
                sortable: false,
                pageable: false,
                reorderable: false,
                resizable: true,
                columns: [{
                    field: "productNumber",
                    title: "Product",
                    width: "15%"
                }, {
                    field: "description",
                    title: "Product Description",
                    width: "32%"
                }, {
                    field: "quantity",
                    title: "Qty",
                    width: "5%"
                }, {
                    field: "listPrice",
                    title: "List Price Each",
                    format: "{0:c}",
                    width: "12%"
                }, {
                    field: "netPrice",
                    title: "Net Price Each",
                    format: "{0:c}",
                    width: "12%"
                }, {
                    title: "Ext. List Price",
                    width: "12%",
                    template: function (data) { return kendo.toString(data.listPrice * data.quantity, 'c'); }
                }, {
                    title: "Ext. Net Price",
                    width: "12%",
                    template: function (data) { return kendo.toString(data.netPrice * data.quantity, 'c'); }
                }
                ]

            };// end of grid options
        }
    });// end of getQuoteItems service

    $scope.SubmitOrder = function () {

        $scope.submitAttempted = true;

        var readyToSubmit = $scope.validate();

        if (readyToSubmit == false)
        {
            $(window).scrollTop(800);
        }

        //var readyToSubmit = true;
        //==========================
        if (readyToSubmit) {    
            $scope.postOrder();
        }
    }

    $scope.postOrder = function () {

        bootbox.confirm("<p>Are you sure you want to submit Order? <br/>No further changes will be available on this project after it has been submitted.</p>", function (result) {

            if (result) {

                kendo.ui.progress($("#order-form"), true);

                orderService.postOrder($scope.OrderVM).$promise.then(function (result) { // api controller 

                    if (result.isok) {
                        kendo.ui.progress($("#order-form"), false);
                       
                        $scope.clearErrors();

                        orderService.sendEmailOrderSubmit($scope.OrderVM);

                        bootbox.alert("<p>Thank you for submitting the order. Your Daikin Customer Service Representative will review the order and get back to you within 24 hours.<br/> <br/>To cancel the order, please contact your Daikin Customer Service Representative.</p>", function () {
                            window.location.replace("/ProjectDashboard/Quote?projectId=" + $scope.OrderVM.projectIdStr + "&quoteId=" + $scope.OrderVM.quoteIdStr);
                        });

                    } else {

                        kendo.ui.progress($("#order-form"), false);

                        $scope.clearErrors();

                        for (var i = 0; i < result.messages.items.length; i++) {
                            if (result.messages.items[i].key != "") {
                                var error = { "key": result.messages.items[i].key, "message": result.messages.items[i].text }
                                $scope.ServerErrors.push(error);
                                $scope.HasServerError = true;
                            } else {
                                var pageMessage = { "type": result.messages.items[i].type, "message": result.messages.items[i].text }
                                $scope.PageMessages.push(pageMessage);
                                $scope.HasPageMessage = true;
                                $(window).scrollTop(0);
                            }
                        }
                    }
                });

            }
        });
    }

    $scope.clearErrors = function () {
        $scope.ServerErrors = [];
        $scope.HasServerError = false;
        $scope.PageMessages = [];
        $scope.HasPageMessage = false;
    }

    $scope.onSelect = function (e) {// kendo upload

        $scope.OrderVM.poAttachmentFileName = e.files[0].name;
      
    }

    $scope.onUploadError = function (e) {
        $scope.UploadPOSuccess = false;
        if (e.operation == "upload") {
            console.log("Failed to upload " + e.files[0].name + ". Response message: " + e.XMLHttpRequest.statusText);
            $('#uploadError').text(e.XMLHttpRequest.statusText);
            $('#uploadError').attr('display', 'block');
        }
    }

    $scope.onUploadSuccess = function (e) {
        $scope.UploadPOSuccess = true;
        console.log("Upload Successfull. Response Status: " + e.XMLHttpRequest.status);
        $('#uploadError').text('');
        $('#uploadError').attr('display', 'none');
    }

    $("#order-form").kendoValidator({
        rules: {
            dateValidation: function (input) {
                if (input.is("[kendo-date-picker=OrderReleaseDate]")) {
                    //Check if Date parse is successful
                    if (!Date.parse(input[0].value)) {
                        return false;
                    }
                    return true;
                } else {
                    return true;
                }

            },

            orderReleaseDateRule: function (input) {
                if (input.is("[kendo-date-picker=OrderReleaseDate]")) {
                    if (Date.parse(input[0].value) < Date.parse($scope.OrderVM.submitDate)) {
                        return false;
                    }
                    return true;
                } else {
                    return true;
                }
            },

            upload: function (input) {
                if (input.is("[kendo-upload=POAttachmentUpload]")) {
                    if (input.closest(".k-upload").find(".k-file").length > 0 ) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }

            },
            POFileName: function (input) {
                if (input.is("[kendo-upload=POAttachmentUpload]")) {
                    if (input.closest(".k-upload").find(".k-file").length > 0) {
                        if (/[~`@!#$%\^&*+=\[\]\\';,/{}|\\":<>\?]/.test($scope.OrderVM.poAttachmentFileName) == true) {
                            $('.k-upload-selected').css('visibility', 'hidden');
                            return false;
                        }
                        else {
                            $('.k-upload-selected').css('visibility', 'visible');
                            return true;
                        }
                           
                    } 
                } else {
                    return true;
                }

            }
     
                
        },
        messages: {
            //custom validation messages
            required: "This field is required",
            dateValidation: "Invalid date format",
            orderReleaseDateRule: "Order Release Date must be greater than or equal to Order Submit Date",
            upload: "Purchase Order Attachment is required",
            POFileName: "File name can not include special characters like '@,#,$,%,&...'"
        }
    });


    $scope.validate = function (event) {
         //validate ProjectVM
        $scope.validateProjectVM($scope.projectVM);

        //validate Orderdetails
        var orderValidator = $("#order-form").data("kendoValidator");

        if (orderValidator.validate() && $scope.projectVMValid && $scope.UploadPOSuccess == true) {
            return true;
        } else {
            return false;
        }
    }

    $scope.validateProjectVM = function (projectVM) {
        $scope.projectVMValid = true;
        $scope.ValidationError =[];
       
        //ShipTo Address validation
       if (!projectVM.shipToName) {
            var error = { "key": "projectVM.shipToName", "message": "Business Name is required."
        }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
        };
       if (!projectVM.shipToAddress.countryCode) {
            var error = { "key": "projectVM.shipToAddress.countryCode", "message": "Country Code is required."
        }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
        };
       if (!projectVM.shipToAddress.addressLine1) {
            var error = { "key": "projectVM.shipToAddress.addressLine1", "message": "Address Line 1 is required."
        }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.shipToAddress.location) {
            var error = { "key": "projectVM.shipToAddress.location", "message": "Project Location is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.shipToAddress.stateId) {
            var error = { "key": "projectVM.shipToAddress.stateId", "message": "State is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.shipToAddress.postalCode) {
            var error = { "key": "projectVM.shipToAddress.postalCode", "message": "Zip Code is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       //End ShipTo Address validation

        // Customer Address validation
       if (!projectVM.dealerContractorName) {
            var error = { "key": "projectVM.dealerContractorName", "message": "Dealer/Contractor Name is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.customerName) {
            var error = { "key": "projectVM.customerName", "message": "Business Name is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.customerAddress.countryCode) {
            var error = { "key": "projectVM.customerAddress.countryCode", "message": "Country Code is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.customerAddress.addressLine1) {
            var error = { "key": "projectVM.customerAddress.addressLine1", "message": "Address Line 1 is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.customerAddress.location) {
            var error = { "key": "projectVM.customerAddress.location", "message": "Location is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
       };
       if (!projectVM.customerAddress.stateId) {
            var error = { "key": "projectVM.customerAddress.stateId", "message": "State is required."
       }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
      };
       if (!projectVM.customerAddress.postalCode) {
            var error = { "key": "projectVM.customerAddress.postalCode", "message": "Zip Code is required."
      }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
      };
        //end Customer Address validation

        //seller address validation
       if (!projectVM.sellerName) {
            var error = { "key": "projectVM.sellerName", "message": "Business Name is required."
      }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
      };
       if (!projectVM.sellerAddress.countryCode) {
            var error = { "key": "projectVM.sellerAddress.countryCode", "message": "Country Code is required."
      }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
    };
       if (!projectVM.sellerAddress.addressLine1) {
            var error = { "key": "projectVM.sellerAddress.addressLine1", "message": "Address Line 1 is required."
    }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
    };
       if (!projectVM.sellerAddress.location) {
            var error = { "key": "projectVM.sellerAddress.location", "message": "Location is required."
    }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
    };
       if (!projectVM.sellerAddress.stateId) {
            var error = { "key": "projectVM.sellerAddress.stateId", "message": "State is required."
    }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
    };
       if (!projectVM.sellerAddress.postalCode) {
            var error = { "key": "projectVM.sellerAddress.postalCode", "message": "Zip Code is required."
    }
            $scope.ValidationError.push(error);
            $scope.projectVMValid = false;
    };
       //end seller address validation

    }

}]);


