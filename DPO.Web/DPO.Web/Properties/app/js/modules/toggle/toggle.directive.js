(function () {
    "use strict";

    angular
        .module("DPO.toggle")
        .directive("DPOToggle", Toggle);

    Toggle.$inject = ["ToggleCollapsedService"];

    function Toggle(ToggleCollapsedService) {
        return {
            restrict: "EA",
            scope: true,
            controller: controller,
            controllerAs: "tg",
            bindToController: true
        }

        function controller($scope, ToggleCollapsedService) {

            this.toggle = toggle;

            function toggle() {
                return ToggleCollapsedService.getIsCollapsed();
            }
        }
    }

})()