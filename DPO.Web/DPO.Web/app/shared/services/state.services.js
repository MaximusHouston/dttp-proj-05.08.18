'use strict';
angular.module("DPO.Projects").factory('stateService', ['$resource', function ($resource) {
    var stateService = {
        getStatesByCountry: function () {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Common/GetStatesByCountry',
                            params: {}
                        }
                    });
        },
        getStateIdByStateCode: function () {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Common/GetStateIdByStateCode',
                            params: {}
                        }
                    });
        }

    };
    return stateService;
}]);