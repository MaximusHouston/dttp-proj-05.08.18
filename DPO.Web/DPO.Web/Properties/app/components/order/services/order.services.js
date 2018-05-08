'use strict';
angular.module("DPO.Projects").factory('orderService', ['$resource', function ($resource) {
            return $resource('', {},
                    {
                        postOrder: {
                            method: 'POST',
                            url: '/api/Order/PostOrder'
                        },

                        UpdateOrderStatus: {
                            method: 'POST',
                            url: '/api/Order/UpdateOrderStatus'
                        },
                        getOrderOptions: {
                            method: 'GET',
                            url: '/api/Order/GetOrderOptions',
                        },
                        sendEmailOrderSubmit: {
                            method: 'GET',
                            url: '/ProjectDashboard/sendEmailOrderSubmit'
                        }     
            });

}]);