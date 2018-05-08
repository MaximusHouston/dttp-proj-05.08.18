(
function () {
    "use strict";
    var app = angular.module("DPO.Projects");

    app.controller('orderExportExcelController', ['$scope', function ($scope) {
        
        $scope.export = function () {
            if ($scope.ExportExcelTypeDll.value() == "Orders") {
                $scope.$parent.$parent.ExportExcel();
            } else {
                $scope.$parent.$parent.ExportExcelDetailed();
            }
            
        }

        

    }]);// end of controller



}());