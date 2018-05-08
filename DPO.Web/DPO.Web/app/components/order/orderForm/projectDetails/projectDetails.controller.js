(
function () {
    "use strict";
    var app = angular.module("DPO.Projects");

    app.controller('projectDetailsController', ['$scope', '$resource', 'DPOprojectService', function ($scope, $resource, DPOprojectService) {
        //kendo.ui.progress($("#project-details"), true);

        $scope.OrderVM = new OrderViewModel();

        //$scope.vm = {};

        //$scope.vmloaded = false;

        //DPOprojectService.getProjectWithQuote($scope.OrderVM.projectIdStr, $scope.OrderVM.quoteIdStr).perform({}).$promise.then(function (result) {
        //    if (result.isok) {
        //        //kendo.ui.progress($("#project-details"), false);
        //        $scope.vm = result.model;
        //        $scope.vmloaded = true;
        //    }
        //});

        //$scope.ProjectDetailsEdit = function () {
        //    //localStorage["ActiveTab"] = "projectDetails";
        //    $scope.ProjectDetailsEditWindow.center();
        //    $scope.ProjectDetailsEditWindow.open();

        //}

    }]);// end of controller



}());