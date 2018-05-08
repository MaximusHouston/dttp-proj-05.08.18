/// <reference path="../../Scripts/angular.js" />

(function () {
    "use strict";

    var app = angular
        .module("DPO.Projects")
        .controller("updateNotesController", updateNotesController);

    updateNotesController.$inject = ["$scope", "$log", "$location", "$uibModal",
        "$window", "projectService"];

    function updateNotesController($scope, $log, $location, $uibModal, $window, projectService) {
        var vm = this;

        vm.projectId = getProjectId();
        vm.newNote = getNewNote();

        projectService.projectPipelineNoteTypes(
            function (data) {
                vm.projectPipelineNoteTypes = getResponseItems(data);
            },
            errorHandler
        );

        projectService.projectPipelineNotes(vm.projectId,
            function (data) {
                addNotes(getResponseItems(data));
            },
            errorHandler
        );

        vm.saveNote = function (event) {
            projectService.projectPipelineNote(vm.newNote,
                function (data) {
                    showEstimatedDeliveryModal(vm.newNote);
                    addNote(vm.newNote);
                    vm.newNote = getNewNote();
                }, errorHandler);
        };

        function getResponseItems(data) {
            if (data && data.data && data.data.model) {
                return data.data.model.items;
            }

            return null;
        }

        function addNotes(notes) {
            for (var i in notes) {
                addNote(notes[i]);
            }
        }

        function addNote(note) {
            if (!vm.notes) {
                vm.notes = [];
            }

            vm.notes.unshift(note);
            //$scope.$apply();
        }

        function showEstimatedDeliveryModal(note) {
            if (note == null || note.projectPipelineNoteType == null) {
                return;
            }

            var noteType = note.projectPipelineNoteType;

            var project = projectService.getProject(vm.projectId, function (data) {
                // HACK:  Magic numbers, push or pull of estimated delivery
                if (noteType.projectPipelineNoteTypeId == 4
                    || noteType.projectPipelineNoteTypeId == 5) {
                    $uibModal.open({
                        animation: true,
                        templateUrl: "/app/views/projects/pipelinenotes/dialogEstimatedDelivery.html",
                        backdrop: false,
                        backdropClass: "ui-modal-backdrop",
                        keyboard: false,
                        controller: "dialogEstimatedDeliveryController",
                        controllerAs: "vm",
                        bindToController: true,
                        resolve: {
                            project: function () {
                                return data;
                            },
                            note: function () {
                                return note;
                            }
                        }
                    });
                }
            }, errorHandler);
        }

        function errorHandler(data) {
            $log.error("Unable to load data: " + data.status)
        }

        function getNewNote() {
            return {
                projectId: getProjectId(),
                timestamp: new Date()
            }
        }

        function getProjectId() {
            var parts = $window.location.href.split('/');

            if (parts) {

                var projectId;
                //TODO: need to take a look for better approach
                //Modify by Aaron 09-23-2016
                //remove the last two character from the string return to get only the projectId

                if(parts[parts.length -1] == "")
                {
                    return projectId = (parts[parts.length - 2]).substring(0, parts[parts.length - 2].length - 2 );
                }
                else
                {
                    return projectId = (parts[parts.length - 1]).substring(0, parts[parts.length - 1].length - 2);
                }
            }
            
        }
    }
})();