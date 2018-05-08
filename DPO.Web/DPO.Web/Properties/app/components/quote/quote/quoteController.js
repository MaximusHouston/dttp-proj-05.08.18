angular.module("DPO.Projects").controller('quoteController', ['$scope', '$resource', 'quoteService', 'orderService', function ($scope, $resource, quoteService, orderService) {

    $scope.vm = new QuoteViewModel();

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
                $("#quote_order_tab").css("visibility", "visible");
            }
            
        }
    });


    //DPOprojectService.getSellerInfo($scope.OrderVM.projectIdStr).perform({}).$promise.then(function (result) {
    //    if (result.isok) {

    //        $scope.vm = result.model;
    //    }
    //});


    //$resource('/Project/GetSellerInfo', { projectId: $scope.projectId }).get().$promise.then(function (result) {

    //    if (result.Data != undefined) {
    //        $scope.vm = result.Data;
    //    } 

    //});

  

}]);