angular.module("DPO.Projects").controller('quoteDiscountRequestsController', ['$scope', '$resource', 'orderService', function ($scope, $resource, orderService) {

    $scope.vm = new QuoteDiscountRequestsViewModel();


    //quoteService.getQuoteOptions($scope.vm.projectIdStr, $scope.vm.quoteIdStr).perform({}).$promise.then(function (result) {
    //    if (result.isok) {
    //        $scope.canSubmitOrder = result.model.canSubmitOrder;
    //        $scope.canViewOrder = result.model.canViewOrder;

    //    }
    //});

    orderService.getOrderOptions({ projectId: $scope.vm.projectIdStr, quoteId: $scope.vm.quoteIdStr }).$promise.then(function (result) {
        if (result.isok) {
            $scope.canSubmitOrder = result.model.canSubmitOrder;
            $scope.canViewSubmittedOrder = result.model.canViewSubmittedOrder;

            if ($scope.canViewSubmittedOrder) {
                $("#quoteDAR_order_tab").css("visibility", "visible");
            }
        }
    });




}]);