angular.module("DPO.Projects").directive("dealercontractorInfo", function () {
    return {
        restrict: "E",
        //scope: {},
        controller: "dealerContractorInfoController",
        replace: true,
        templateUrl: "/app/components/order/orderForm/dealerContractorInfo/dealerContractorInfo.html"
    }
});

