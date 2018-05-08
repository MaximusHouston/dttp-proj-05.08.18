
(function () {
    angular.module('DPO.ToggleCollapsedService')
        .factory("ToggleCollapsedService", ToggleCollapsedService);

    ToggleCollapsedService.$inject = [];

    function ToggleCollapsedService() {

        var isCollapsed = false;

        var ToggleCollapsedService = {
            setIsCollapsed: setIsCollapsed,
            getIsCollapsed: getIsCollapsed
        };

        return ToggleCollapsedService;


        function setIsCollapsed(value) {
            isCollapsed = value;
        }

        function getIsCollapsed() {
            return isCollapsed;
        }

    }
})();