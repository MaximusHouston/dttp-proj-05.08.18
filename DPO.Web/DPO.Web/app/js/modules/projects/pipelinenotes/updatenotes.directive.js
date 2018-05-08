/// <reference path="../Scripts/angular.js" />
(
    function () {
    "use strict";

    angular
        .module("DPO.Projects")
        .directive("updateNotes", directive);

    function directive() {
        return {
            restrict: "EA",
            scope: {},
            transclude: true,
            controller: "updateNotesController",
            controllerAs: "vm",
            bindToController: true,
            replace: true,
            templateUrl: "/app/views/projects/pipelinenotes/updatenotes.html"
        }
    }


})();