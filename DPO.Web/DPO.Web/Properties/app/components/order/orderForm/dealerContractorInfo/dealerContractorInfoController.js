angular.module("DPO.Projects").controller('dealerContractorInfoController', ['$scope', '$resource', '$state', 'DPOprojectService', function ($scope, $resource, $state, DPOprojectService) {

    //$scope.OrderVM = new OrderViewModel();

    //$scope.dealerContractorVM = {}

    //DPOprojectService.getDealerContractorInfo($scope.OrderVM.projectIdStr).perform({}).$promise.then(function (result) {
    //    if (result.isok) {

    //        $scope.dealerContractorVM = result.model;
    //        $scope.dealerContractorVMLoaded = true;
    //    }
    //});

    //$resource('/Project/GetProject', { projectId: $scope.projectId }).get().$promise.then(function (result) {

    //    if (result.Data != undefined) {
    //        $scope.project = result.Data;
    //    } 

    //});

    //$scope.dealerContractorValid = false;

    //$scope.$watch('projectVM', function (newValue, oldValue) {
    //    if ($scope.projectVMLoaded) {
    //        var valid = true;
    //        if (newValue != null) {
    //            if (!newValue.dealerContractorName) {
    //                var error = { "key": "projectVM.dealerContractorName", "message": "Dealer/Contractor Name is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerName) {
    //                var error = { "key": "projectVM.customerName", "message": "Business Name is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerAddress.countryCode) {
    //                var error = { "key": "projectVM.customerAddress.countryCode", "message": "Country Code is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerAddress.addressLine1) {
    //                var error = { "key": "projectVM.customerAddress.addressLine1", "message": "Address Line 1 is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerAddress.location) {
    //                var error = { "key": "projectVM.customerAddress.location", "message": "Location is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerAddress.stateId) {
    //                var error = { "key": "projectVM.customerAddress.stateId", "message": "State is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //            if (!newValue.customerAddress.postalCode) {
    //                var error = { "key": "projectVM.customerAddress.postalCode", "message": "Zip Code is required." }
    //                $scope.ValidationError.push(error);
    //                valid = false;
    //            };
    //        } else {
    //            $scope.ValidationError.push(
    //                    { "key": "projectVM.dealerContractorName", "message": "Dealer/Contractor Name is required." },
    //                    { "key": "projectVM.customerName", "message": "Business Name is required." },
    //                    { "key": "projectVM.customerAddress.countryCode", "message": "Country Code is required." },
    //                    { "key": "projectVM.customerAddress.addressLine1", "message": "Address Line 1 is required." },
    //                    { "key": "projectVM.customerAddress.location", "message": "Project Location is required." },
    //                    { "key": "projectVM.customerAddress.stateId", "message": "State is required." },
    //                    { "key": "projectVM.customerAddress.postalCode", "message": "Zip Code is required." })
    //            valid = false;
    //        }
    //        $scope.dealerContractorValid = valid;
    //    } 

       
    //});// end of watch

    $scope.DealerContractorEdit = function () {
        //localStorage["ActiveTab"] = "customerAddress";
        $scope.DealerContractorEditWindow.center();
        $scope.DealerContractorEditWindow.open();

    }


}]);// end of controller
