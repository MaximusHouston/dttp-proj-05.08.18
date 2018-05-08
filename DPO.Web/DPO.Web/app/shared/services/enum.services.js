'use strict';
angular.module("DPO.Projects").factory('enumService', ['$resource', function ($resource) {
    var enumService = {
        getEnumValues: function (url) {
            return $resource('', {},
                    {
                        perform: {
                            method: 'GET',
                            url: url,
                            params: {}
                        }
                    });
        }

    };
    return enumService;
}]);