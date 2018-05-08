angular.module("DPO.Projects").directive("orderDetails", function () {
    return {
        restrict: "E",
        //scope: {},
        controller: "orderDetailsController",
        replace: true,
        templateUrl: "/app/components/order/orderForm/orderDetails/orderDetails.html"
    }
});

