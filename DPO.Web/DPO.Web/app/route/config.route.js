(function () {
    'use strict';
    var app = angular.module('DPO.Routes');

    // Configure the routes and route resolvers
    app.config(['$stateProvider', '$urlRouterProvider', routeConfigurator]);

    function routeConfigurator($stateProvider, $urlRouterProvider) {
        $stateProvider.state('projectedit', {
            //url: "/ProjectDashboard/EditProject?projectIdStr=247285421665419264"  248072689682481152
            url: "/ProjectDashboard/ProjectEdit?projectId=",            
            views: {
                "": {
                    templateUrl: function ($stateParams) {
                        return '/ProjectDashboard/ProjectEdit?projectId=' + $stateParams.projectId;
                    }
                }
            }
        });
       
       

        $urlRouterProvider.otherwise("/");
    }



})();