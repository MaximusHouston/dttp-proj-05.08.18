/// <reference path="../Scripts/angular.js" />
(
    function () {
    "use strict";

    angular
        .module("DPO.Projects")
        .directive("displayNotes", directive);

    function directive() {
        return {
            restrict: "EA",
            scope: {},
            transclude: true,
            controller: "displayNotesController",
            controllerAs: "vm",
            bindToController: true,
            replace: true,
            templateUrl: "/app/views/projects/pipelinenotes/displaynotes.html"
        }
    }


})();