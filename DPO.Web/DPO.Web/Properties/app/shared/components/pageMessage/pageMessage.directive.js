angular.module("DPO.Projects").directive("pageMessage", [function () {
    return {
        restrict: "E",
        scope: {
            //key: '@'
        },
        replace: true,
        templateUrl: "/app/shared/components/pageMessage/pageMessage.html",
        link: function (scope, elem, attr) {
            scope.PageMessages = [];
            for (var i = 0; i < scope.$parent.PageMessages.length; i++) {
                scope.PageMessages.push(scope.$parent.PageMessages[i]);
            }


        }
    }
}]);
