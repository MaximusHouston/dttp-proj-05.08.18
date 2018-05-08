angular.module("DPO.Projects").controller('projectEditDealerContractorController', ['$scope', '$resource', 'DPOprojectService', 'stateService', function ($scope, $resource, DPOprojectService, stateService) {
    $scope.HasServerError = false;
    $scope.HasPageMessage = false;
    $scope.ValidationError = [];
    $scope.ServerErrors = [];
    $scope.PageMessages = [];

    //$scope.projectId = new ProjectId();
    //$scope.oldProjectVM = angular.copy($scope.$parent.$parent.projectVM);

    //DPOprojectService.getProject($scope.projectId.value).perform({}).$promise.then(function (result) {
    //    if (result.isok) {
    //        $scope.vm = result.model;
    //        $scope.vmloaded = true;

    //    }
    //});


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
        //$scope.ProjectDealerContractorValidator.validate();

        if ($scope.countryDLL.value() == "CA") {
            $scope.stateDLL.setOptions({ dataTextField: "name", dataValueField: "stateId" });
            $scope.stateDLL.setDataSource($scope.CAstatesDataSource);


        } else if ($scope.countryDLL.value() == "US") {
            $scope.stateDLL.setOptions({ dataTextField: "name", dataValueField: "stateId" });
            $scope.stateDLL.setDataSource($scope.USstatesDataSource);

        }


    }


    $scope.update = function () {
        if ($scope.ProjectDealerContractorValidator.validate()) {
            kendo.ui.progress($("#editDealerContractorPopup"), true);

            DPOprojectService.projectUpdate().perform($scope.$parent.$parent.projectVM).$promise.then(function (result) {
                if (result.isok) {
                    $scope.clearErrors();
                    kendo.ui.progress($("#editDealerContractorPopup"), false);

                    $scope.$parent.$parent.projectVM.timestamp = result.model.timestamp;
                    $scope.$parent.$parent.oldProjectVM = angular.copy($scope.$parent.$parent.projectVM);

                    $scope.$parent.$parent.DealerContractorEditWindow.close();
                    $scope.$parent.$parent.validateProjectVM($scope.$parent.$parent.projectVM);


                } else {
                    //alert(result.messages.items[0].text)
                    kendo.ui.progress($("#editDealerContractorPopup"), false);
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
                            //var p = $("#order-form").position();
                            $(window).scrollTop(0);


                        }

                    }
                }
            });// end of Project Update
        }
    }

    $scope.cancel = function () {
        $scope.$parent.$parent.DealerContractorEditWindow.close();
        $scope.$parent.$parent.projectVM = angular.copy($scope.$parent.$parent.oldProjectVM);
    }

    $scope.clearErrors = function () {
        $scope.ServerErrors = [];
        $scope.HasServerError = false;
        $scope.PageMessages = [];
        $scope.HasPageMessage = false;
        $scope.ValidationError = [];
    }

}]);// end of controller
