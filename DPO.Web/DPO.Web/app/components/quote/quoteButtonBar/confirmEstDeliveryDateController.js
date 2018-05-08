angular.module("DPO.Projects").controller('confirmEstDeliveryDateController', ['$scope', '$resource', 'DPOprojectService', function ($scope, $resource, DPOprojectService) {
    $scope.HasServerError = false;
    $scope.HasPageMessage = false;
    $scope.ServerErrors = [];
    $scope.PageMessages = [];

    $scope.viewModel = new ViewModel();

    DPOprojectService.getProject($scope.viewModel.projectId).perform({}).$promise.then(function (result) {
        if (result.isok) {
            $scope.vm = result.model;
            $scope.vmloaded = true;
            $scope.originalEstDelivery = $scope.vm.estimatedDelivery;
        }
    });


    
    $scope.updateEstDeliveryDate = function () {
        kendo.ui.progress($("#delivery-date-window"), true);
        var req = $resource('/ProjectDashboard/EditEstimatedDelivery',
                    { projectId: $scope.vm.projectIdStr, quoteId: $scope.viewModel.quoteId, estimatedDelivery: $scope.vm.estimatedDelivery },
                    { action: { method: 'POST' } });
        req.save().$promise.then(function (result) {
            if (result.IsOK) {
                kendo.ui.progress($("#delivery-date-window"), false);
                window.location.replace("/ProjectDashboard/OrderForm?projectId=" + $scope.vm.projectIdStr + "&quoteId=" + $scope.viewModel.quoteId);
            } else {
                kendo.ui.progress($("#delivery-date-window"), false);
                for (var i = 0; i < result.Messages.Items.length; i++) {
                    if (result.Messages.Items[i].Key != "" && result.Messages.Items[i].Key != undefined) {
                        var error = { "key": result.Messages.Items[i].Key, "message": result.Messages.Items[i].Text }
                        $scope.ServerErrors.push(error);
                        $scope.HasServerError = true;
                    } else {
                        var pageMessage = { "type": result.Messages.Items[i].Type, "message": result.Messages.Items[i].Text }
                        $scope.PageMessages.push(pageMessage);
                        $scope.HasPageMessage = true;
                    }

                }
            }
        });// end of req.save()

    }

    $scope.$on("DeliveryDateWindowClosed", function (event, args) {
        $scope.vm.estimatedDelivery = $scope.originalEstDelivery;
        $scope.ServerErrors = [];
        $scope.PageMessages = [];
        $scope.HasServerError = false;
        $scope.HasPageMessage = false;
    });

 
}]);// end of controller
