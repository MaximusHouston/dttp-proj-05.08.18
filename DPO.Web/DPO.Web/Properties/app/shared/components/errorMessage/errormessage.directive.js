angular.module("DPO.Projects").directive("errorMessage", [function (enumService) {
    return {
        restrict: "E",
        scope: {
            key: '@'
        },
        replace: true,
        templateUrl: "/app/shared/components/errormessage/errormessage.html",
        link: function (scope, elem, attr) {

            scope.$watch('$parent.ServerErrors', function (newValue, oldValue) {
                scope.ErrorMessages = [];
                for (var i = 0; i < scope.$parent.ServerErrors.length; i++) {
                    if (scope.$parent.ServerErrors[i].key == scope.key) {
                        scope.ErrorMessages.push(scope.$parent.ServerErrors[i].message);
                    }
                }
            });


        }
    }
}]);
