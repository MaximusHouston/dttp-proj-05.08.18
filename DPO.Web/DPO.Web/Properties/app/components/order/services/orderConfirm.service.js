(function () {
    "use strict";
    var serviceId = 'orderConfirmService';
    angular.module('DPO.Projects')
    .factory(serviceId, ['$q', orderSubmitConfirm]);

    function orderSubmitConfirm($q) {
       
        return {
            showOrderConfirmDialog: function showDialog() {
                var deferred = $q.defer();
                var orderConfirmHtml = 'orderSubmitConfirm.html';
                return orderConfirmHtml;
            }
        }
    };

 }())