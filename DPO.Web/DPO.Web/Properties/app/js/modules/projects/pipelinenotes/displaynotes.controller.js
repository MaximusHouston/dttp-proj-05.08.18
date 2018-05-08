/// <reference path="../../Scripts/angular.js" />

(function () {
    "use strict";

    var app = angular
        .module("DPO.Projects")
        .controller("displayNotesController", displayNotesController);

    displayNotesController.$inject = ["$scope", "$log", "$location", "$window", "pipelineNotesService"];

    function displayNotesController($scope, $log, $location, $window, pipelineNotesService) {
        var vm = this;

        vm.projectId = getProjectId();

        pipelineNotesService.projectPipelineNotes(vm.projectId,
            function (data) {
                if (data && data.data && data.data.items) {
                    addNotes(data.items);
                }
            },
            errorHandler
        );

        function addNotes(notes)
        {
            for (var note in notes)
            {
                addNote(note);
            }
        }

        function addNote(note)
        {
            if (!vm.notes)
            {
                vm.notes = [];
            }

            vm.notes.push(note);
            //$scope.$apply();
        }

        function errorHandler(data) {
            alert('errors happened when try to add notes.Please contact administrator for more details.');
        }

        function getProjectId() {
            var parts = $window.location.href.split('/');

            if (parts) {
                return parts[parts.length - 1];
            }
        }
    }
})();