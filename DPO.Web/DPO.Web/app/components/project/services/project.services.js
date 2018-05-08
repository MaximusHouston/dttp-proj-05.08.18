'use strict';
angular.module("DPO.Projects").factory('DPOprojectService', ['$resource', function ($resource) {
    var DPOprojectService = {
        getProject: function (projectIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/GetProject',
                            params: { projectId: projectIdStr }
                        }
                    });
        },
        getProjectWithQuote: function (projectIdStr, quoteIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/GetProjectWithQuote',
                            params: { projectId: projectIdStr, quoteId: quoteIdStr }
                        }
                    });
        },
        getProjectLocation: function (projectIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/GetProjectLocation',
                            params: { projectId: projectIdStr }
                            
                        }
                    });
        },
        getSellerInfo: function (projectIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/GetSellerInfo',
                            params: { projectId: projectIdStr }
                           
                        }
                    });
        },
        getDealerContractorInfo: function (projectIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/GetDealerContractorInfo',
                            params: { projectId: projectIdStr }
                           
                        }
                    });
        },
        hasOrder: function (projectIdStr) {
            return $resource('', {},
                    {
                        perform: {
                            cache: true,
                            method: 'GET',
                            url: '/api/Project/HasOrder',
                            params: { projectId: projectIdStr }
                            
                        }
                    });
        },

        //projectUpdate: function () {
        //    return $resource('', {},
        //            {
        //                perform: {
        //                    cache: true,
        //                    method: 'POST',
        //                    url: '/ProjectDashboard/ProjectUpdate',
        //                    params: {}
                           
        //                }
        //            });
        //},

        projectUpdate: function () {
            return $resource('', {},
                    {
                        perform: {
                            method: 'POST',
                            url: '/api/Project/PostProject'
                        }
                    });
        },

        
    };
    return DPOprojectService;
}]);