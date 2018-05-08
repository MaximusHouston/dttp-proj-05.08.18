(
function () {
    angular.module("DPO.Projects").directive("sellerInfo", function () {
        return {
            restrict: "E",
            //scope: {},
            controller: "sellerInfoController",
            replace: true,
            templateUrl: "/app/components/order/orderForm/sellerInfo/sellerInfo.html"
        }
    });

}());