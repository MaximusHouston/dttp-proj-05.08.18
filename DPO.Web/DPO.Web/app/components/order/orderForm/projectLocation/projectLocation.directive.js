angular.module("DPO.Projects").directive("projectLocation", function () {
    return {
        restrict: "E",
        //scope: {},
        controller: "projectLocationController",
        replace: true,
        templateUrl: "/app/components/order/orderForm/projectLocation/projectLocation.html"
    }
});

