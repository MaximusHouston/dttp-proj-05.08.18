angular.module("DPO.Projects").controller('orderDetailsController', ['$scope', '$resource', function ($scope, $resource) {
    $scope.openFile = function (fileName) {
        var url = "/document/QuoteOrder/" + $scope.OrderVM.quoteIdStr + "/" + "?filename=" + fileName;
        var newWindow = window.open(url);
    }
    $scope.checkPONumber = function () { 
    }
}]);
