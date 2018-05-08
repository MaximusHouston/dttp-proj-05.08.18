
angular.module("DPO.Projects").controller('quoteButtonBarController', ['$scope', '$resource', 'orderService', function ($scope, $resource, orderService) {

    $scope.vm = new QuoteButtonBarViewModel();


    orderService.getOrderOptions({ projectId: $scope.vm.projectIdStr, quoteId: $scope.vm.quoteIdStr }).$promise.then(function (result) {
        if (result.isok) {
            $scope.canSubmitOrder = result.model.canSubmitOrder;
            $scope.canViewSubmittedOrder = result.model.canViewSubmittedOrder;

            if ($scope.canSubmitOrder) {
                $("#ShowOrderFormBtn").css("visibility", "visible");
            }
            
        }
    });


    $scope.ConfirmEstDeliveryDate = function () {
            
        $scope.DeliveryDateWindow.center();
        $scope.DeliveryDateWindow.open();
      
    }

    $scope.closeDeliveryDateWindow = function () {
        $scope.$broadcast('DeliveryDateWindowClosed');
    }



}]);// end of controller
