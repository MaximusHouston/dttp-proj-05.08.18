'use strict';
angular.module("DPO.Projects").factory('quoteService', ['$resource', function ($resource) {
    var quoteService = {
        getQuoteItems: function (quoteIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            method: 'GET',
                            url: '/api/Quote/GetQuoteItems',
                            params: { quoteId: quoteIdStr }
                        }
                    });
        },
        hasOrder: function (quoteIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            method: 'GET',
                            url: '/api/Quote/HasOrder',
                            params: { quoteId: quoteIdStr }
                        }
                    });
        },
        getQuoteOptions: function (projectIdStr, quoteIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            method: 'GET',
                            url: '/api/Quote/GetQuoteOptions',
                            params: { projectId: projectIdStr, quoteId: quoteIdStr }
                        }
                    });
        }

    };
    return quoteService;
}]);