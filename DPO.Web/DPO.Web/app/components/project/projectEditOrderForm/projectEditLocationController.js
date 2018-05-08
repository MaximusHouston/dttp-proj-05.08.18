angular.module("DPO.Projects").controller('projectEditLocationController', ['$scope', '$resource', '$http', 'DPOprojectService', 'stateService', 'addressService', function ($scope, $resource, $http, DPOprojectService, stateService, addressService) {
    $scope.HasServerError = false;
    $scope.HasPageMessage = false;
    $scope.ValidationError = [];
    $scope.ServerErrors = [];
    $scope.PageMessages = [];
    $scope.invalidAddress = false;

    $scope.suggestedAddresses = [
        {
            "$id": null,
            "addressee": null,
            "barcode": null,
            "city": null,
            "cityStateZip": null,
            "country": null,
            "county": null,
            "countyFips": null,
            "dpvConfirmCode": null,
            "line1": null,
            "line2": null,
            "line3": null,
            "line4": null,
            "stateProvince": null,
            "unitNumber": null,
            "urbanization": null,
            "zip4": null,
            "zip5": null,
            "zipCode": null,
            "latitude": null,
            "longitude": null
        }
    ];

    

    //$scope.oldProjectVM = angular.copy($scope.$parent.$parent.projectVM);

    stateService.getStatesByCountry().perform({ countryCode: 'CA' }).$promise.then(function (result) {
        if (result.isok) {
            $scope.CAstatesDataSource = result.model;

        }
    });


    stateService.getStatesByCountry().perform({ countryCode: 'US' }).$promise.then(function (result) {
        if (result.isok) {
            $scope.USstatesDataSource = result.model;

        }
    });

    //Load new DataSource for States DropdownList when Country changes.
    $scope.updateStateDDL = function () {
        $scope.stateDLL.select(0);
        $scope.stateDLL.trigger("change");
        //$scope.ProjectLocationValidator.validate();

        if ($scope.countryDLL.value() == "CA") {
            $scope.stateDLL.setOptions({ dataTextField: "name", dataValueField: "stateId" });
            $scope.stateDLL.setDataSource($scope.CAstatesDataSource);


        } else if ($scope.countryDLL.value() == "US") {
            $scope.stateDLL.setOptions({ dataTextField: "name", dataValueField: "stateId" });
            $scope.stateDLL.setDataSource($scope.USstatesDataSource);

        }


    }



    $scope.update = function () {
        //$scope.addressValid = false;
        $scope.$parent.$parent.projectVM.shipToAddress.addressLine1 = $scope.$parent.$parent.projectVM.shipToAddress.addressLine1.replace(".", "");
        var projectLocation = $scope.$parent.$parent.projectVM.shipToAddress;
        if (projectLocation.countryCode != "CA") {
            if ($scope.ProjectLocationValidator.validate()) {
                addressService.verifyAddress(projectLocation).$promise.then(function (result) {
                    if (result.isok) { // Address exactly matches suggestion
                        $scope.invalidAddress = false;
                        $scope.$parent.$parent.PageMessages = []
                        $scope.$parent.$parent.HasPageMessage = false;
                        $scope.clearErrors();
                        if ($scope.ProjectLocationValidator.validate()) {
                            $scope.updateProjectLocation();
                        }

                    } else {
                        $scope.clearErrors();

                        if (result.model.addresses == null) {//invalid address
                            $scope.collectErrors(result);
                            $scope.invalidAddress = true;
                            $scope.VerifyAddressWindow.center();
                            $scope.VerifyAddressWindow.open();
                        } else {
                            if (result.model.addresses.length > 0) { // Address does not match suggesstion, show sugesstions
                                $scope.invalidAddress = false;
                                $scope.suggestedAddresses = result.model.addresses;
                                $scope.VerifyAddressWindow.center();
                                $scope.VerifyAddressWindow.open();
                            } else {
                                $scope.collectErrors(result);
                                $scope.invalidAddress = true;
                                $scope.VerifyAddressWindow.center();
                                $scope.VerifyAddressWindow.open();
                            }
                        }

                    }
                }); // end of addressService
            }

        } else if ($scope.ProjectLocationValidator.validate()) {// Canada
            $scope.updateProjectLocation();
        }




        //if ($scope.ProjectLocationValidator.validate()) {

        //    $scope.addressServiceResponse = $scope.verifyAddress();

        //    //$scope.VerifyAddressWindow.center();
        //    //$scope.VerifyAddressWindow.open();

        //    //if ($scope.verifyAddressResp.d.IsAddressValid) {
        //    //    $scope.updateProjectLocation();
        //    //} else {
        //    //    $scope.VerifyAddressWindow.center();
        //    //    $scope.VerifyAddressWindow.open();

        //    //}


        //}
    }

    $scope.updateProjectLocation = function () {
        kendo.ui.progress($("#editLocationPopup"), true);

        DPOprojectService.projectUpdate().perform($scope.$parent.$parent.projectVM).$promise.then(function (result) {
            if (result.isok) {
                $scope.clearErrors();

                kendo.ui.progress($("#editLocationPopup"), false);
                $scope.$parent.$parent.projectVM.timestamp = result.model.timestamp;
                //$scope.oldProjectVM = angular.copy($scope.$parent.$parent.projectVM);
                $scope.$parent.$parent.oldProjectVM = angular.copy($scope.$parent.$parent.projectVM);


                $scope.$parent.$parent.ProjectLocationEditWindow.close();
                $scope.$parent.$parent.validateProjectVM($scope.$parent.$parent.projectVM);

            } else {
                //alert(result.messages.items[0].text)
                $scope.clearErrors();
                kendo.ui.progress($("#editLocationPopup"), false);


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
        });// end of projectUpdate
    }

    $scope.cancel = function () {
        $scope.$parent.$parent.projectVM = angular.copy($scope.$parent.$parent.oldProjectVM);
        $scope.$parent.$parent.ProjectLocationEditWindow.close();
    }

   

    $scope.clearErrors = function () {
        $scope.ServerErrors = [];
        $scope.HasServerError = false;
        $scope.PageMessages = [];
        $scope.HasPageMessage = false;
        $scope.ValidationError = [];
    }

    $scope.collectErrors = function (result) {
        for (var i = 0; i < result.messages.items.length; i++) {
            if (result.messages.items[i].key != "") {
                var error = { "key": result.messages.items[i].key, "message": result.messages.items[i].text }
                $scope.ServerErrors.push(error);
                $scope.HasServerError = true;
            } else {
                var pageMessage = { "type": result.messages.items[i].type, "message": result.messages.items[i].text }
                $scope.PageMessages.push(pageMessage);
                $scope.HasPageMessage = true;

            }

        }
    }


    $scope.windowOpen = function () {
        //$scope.addressServiceResponse = $scope.$parent.$parent.addressServiceResponse;
    }

    



}]);// end of controller
