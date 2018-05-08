(
function () {

    angular.module("DPO.Projects").controller('verifyAddressController', ['$scope', '$resource', 'DPOprojectService', 'stateService', function ($scope, $resource, DPOprojectService, stateService) {
        
        $scope.apply = function () {
            //var enteredAddress = $("#enteredAddress");
            if ($("#suggestedAddresse")[0].checked) {

                stateService.getStateIdByStateCode().perform({ stateCode: $scope.$parent.$parent.suggestedAddresses[0].stateProvince }).$promise.then(function (result) {
                    if (result.isok) {
                        var stateId = result.model;
                                                
                        $scope.$parent.$parent.$parent.$parent.projectVM.shipToAddress.addressLine1 = $scope.$parent.$parent.suggestedAddresses[0].line1;
                        $scope.$parent.$parent.$parent.$parent.projectVM.shipToAddress.addressLine2 = $scope.$parent.$parent.suggestedAddresses[0].line2;
                        $scope.$parent.$parent.$parent.$parent.projectVM.shipToAddress.location = $scope.$parent.$parent.suggestedAddresses[0].city;
                        $scope.$parent.$parent.$parent.$parent.projectVM.shipToAddress.stateId = stateId;
                        $scope.$parent.$parent.$parent.$parent.projectVM.shipToAddress.postalCode = $scope.$parent.$parent.suggestedAddresses[0].zipCode;

                        $scope.$parent.$parent.updateProjectLocation();
                        $scope.$parent.$parent.$parent.$parent.PageMessages = []
                        $scope.$parent.$parent.$parent.$parent.HasPageMessage = false;
                        $scope.$parent.$parent.VerifyAddressWindow.close();

                    }
                });

            } else {
                $scope.$parent.$parent.updateProjectLocation();
                $scope.$parent.$parent.VerifyAddressWindow.close();
            }
            
            
            
        }

        $scope.continue = function () {// use invalid address anyway
            $scope.$parent.$parent.updateProjectLocation();
            $scope.$parent.$parent.VerifyAddressWindow.close();
        }

        $scope.cancel = function () {
            $scope.$parent.$parent.VerifyAddressWindow.close();
        }
        


    }])

}());
