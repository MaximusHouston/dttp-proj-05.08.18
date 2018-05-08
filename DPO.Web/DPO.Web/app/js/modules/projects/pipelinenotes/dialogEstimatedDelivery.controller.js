/// <reference path="../../Scripts/angular.js" />

(function () {
    "use strict";

    var app = angular
        .module("DPO.Projects")
        .controller("dialogEstimatedDeliveryController", dialogEstimatedDeliveryController);

    dialogEstimatedDeliveryController.$inject = ["$log", "projectService", "$window", "$uibModalInstance", "project", "note"];

    function dialogEstimatedDeliveryController($log, projectService, $window, $uibModalInstance, project, note) {
        var vm = this;

        init();

        vm.format = 'MM/dd/yyyy';
        vm.note = note;

        if (project.data.isok) {
            vm.project = project.data.model;
        } else {
            $log.error("Unable to load project.");
            errorHandler(project);
        }

        vm.clear = function () {
            vm.estimatedDeliveryDate = null;
        };

        vm.getDayClass = function (date, mode) {
            return "ui-date-picker";
        }

        vm.open = function ($event) {
            vm.status.opened = true;
        };

        vm.saveEstimatedDeliveryDate = function () {
            projectService.saveProject(
            vm.project,
            function (data) {
                $uibModalInstance.close();
                $window.location.reload();
            },
            errorHandler);
        };

        function errorHandler(data) {
            debugger;
            $log.error(data.status);
        };

        function init() {
            vm.status = {
                opened: false
            };

            if (!vm.note) {
                vm.note = {};
            }
        }
    }
})();