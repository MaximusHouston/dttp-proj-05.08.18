angular.module("DPO.Projects").directive("projectDetails", function () {
    return {
        restrict: "E",
        //scope: {},
        transclude: true,
        controller: "projectDetailsController",
        controllerAs: "vm",
        bindToController: true,
        replace: true,
        templateUrl: "/app/components/order/orderForm/projectDetails/projectDetails.html",
        //link: function (scope, elem, attr) {
        //    kendo.ui.progress($("#order-form"), true);
        //}
    }

});




