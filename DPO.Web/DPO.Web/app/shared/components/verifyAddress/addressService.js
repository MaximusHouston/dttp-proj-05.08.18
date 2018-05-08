'use strict';
angular.module("DPO.Projects").factory('addressService', ['$resource', function ($resource) {
    return $resource('', {},
            {
                verifyAddress: {
                    method: 'POST',
                    url: '/api/Address/VerifyAddress'
                }

            });

    

}]);