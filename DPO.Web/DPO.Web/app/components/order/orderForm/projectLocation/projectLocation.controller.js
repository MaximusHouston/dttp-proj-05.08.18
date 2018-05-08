angular.module("DPO.Projects").controller('projectLocationController', ['$scope', '$resource', '$state', '$http', 'DPOprojectService', function ($scope, $resource, $state, $http, DPOprojectService) {

    $scope.OrderVM = new OrderViewModel();
    
   
    $scope.ProjectLocationEdit = function()
    {
        $scope.ProjectLocationEditWindow.center();
        $scope.ProjectLocationEditWindow.open();
       
    }


    $scope.closeWindow = function () {
        $scope.projectVM = angular.copy($scope.oldProjectVM);
    }

}]);

