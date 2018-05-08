(
function () {

    angular.module("DPO.Projects").controller('sellerInfoController', ['$scope', '$resource', 'DPOprojectService', function ($scope, $resource, sDPOprojectService) {

        //$scope.OrderVM = new OrderViewModel();

        //$scope.sellerVM = {}

        //DPOprojectService.getSellerInfo($scope.OrderVM.projectIdStr).perform({}).$promise.then(function (result) {
        //    if (result.isok) {

        //        $scope.sellerVM = result.model;
        //        $scope.sellerVMLoaded = true;
                                
        //    }
        //});

        //$scope.sellerValid = false;

        //$scope.$watch('projectVM', function (newValue, oldValue) {
        //    if ($scope.projectVMLoaded) {
        //        var valid = true;
        //        if (newValue != null) {
        //            if (!newValue.sellerName) {
        //                var error = { "key": "projectVM.sellerName", "message": "Business Name is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //            if (!newValue.sellerAddress.countryCode) {
        //                var error = { "key": "projectVM.sellerAddress.countryCode", "message": "Country Code is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //            if (!newValue.sellerAddress.addressLine1) {
        //                var error = { "key": "projectVM.sellerAddress.addressLine1", "message": "Address Line 1 is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //            if (!newValue.sellerAddress.location) {
        //                var error = { "key": "projectVM.sellerAddress.location", "message": "Location is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //            if (!newValue.sellerAddress.stateId) {
        //                var error = { "key": "projectVM.sellerAddress.stateId", "message": "State is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //            if (!newValue.sellerAddress.postalCode) {
        //                var error = { "key": "projectVM.sellerAddress.postalCode", "message": "Zip Code is required." }
        //                $scope.ValidationError.push(error);
        //                valid = false;
        //            };
        //        } else {
        //            $scope.ValidationError.push(
        //                { "key": "projectVM.sellerName", "message": "Seller Name is required." },
        //                { "key": "projectVM.sellerAddress.countryCode", "message": "Country Code is required." },
        //                { "key": "projectVM.sellerAddress.addressLine1", "message": "Address Line 1 is required." },
        //                { "key": "projectVM.sellerAddress.location", "message": "Project Location is required." },
        //                { "key": "projectVM.sellerAddress.stateId", "message": "State is required." },
        //                { "key": "projectVM.sellerAddress.postalCode", "message": "Zip Code is required." })
        //            valid = false;
        //        }
                
        //        $scope.sellerValid = valid;
        //    }
        //});// end of watch

        //$resource('/Project/GetSellerInfo', { projectId: $scope.projectId }).get().$promise.then(function (result) {

        //    if (result.Data != undefined) {
        //        $scope.vm = result.Data;
        //    } 

        //});

        $scope.SellerEdit = function () {
            //localStorage["ActiveTab"] = "sellerAddress";

            $scope.SellerEditWindow.center();
            $scope.SellerEditWindow.open();

        }


    }])

}());
