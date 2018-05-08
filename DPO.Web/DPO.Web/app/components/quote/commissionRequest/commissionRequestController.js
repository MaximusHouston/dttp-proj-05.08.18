angular.module("DPO.Projects").controller('quoteCommissionRequestsController', ['$scope', '$resource', 'orderService', function ($scope, $resource, orderService) {

    $scope.vm = new QuoteCommissionRequestsViewModel();

    orderService.getOrderOptions({ projectId: $scope.vm.projectIdStr, quoteId: $scope.vm.quoteIdStr }).$promise.then(function (result) {
        if (result.isok) {
            $scope.canSubmitOrder = result.model.canSubmitOrder;
            $scope.canViewSubmittedOrder = result.model.canViewSubmittedOrder;

            if ($scope.canViewSubmittedOrder) {
                $("#quoteCOM_order_tab").css("visibility", "visible");
            }
        }
    });




}]);