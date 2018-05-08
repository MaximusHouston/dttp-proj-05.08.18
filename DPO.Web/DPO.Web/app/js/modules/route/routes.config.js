/// <reference path="../../../views/reports/kpi/sellthrough.html" />
/// <reference path="../../../views/app.html" />


(function () {
    "use strict";

    angular
       .module("DPO.Routes")
       .config(routeConfig);

    routeConfig.$inject = ["$stateProvider", "$urlRouterProvider", "$locationProvider"];

    function routeConfig($stateProvider, $urlRouterProvider, $locationProvider) {
        $locationProvider.html5Mode(false).hashPrefix('!');
       
    }
})();