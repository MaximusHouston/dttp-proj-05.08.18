angular.module("DPO.Projects").directive("validationMessage", [function () {
    return {
        restrict: "E",
        scope: {
            key: '@'
        },
        replace: true,
        templateUrl: "/app/shared/components/validationMessage/validationMessage.html",
        link: function (scope, elem, attr) {

            scope.$watch('$parent.ValidationError', function (newValue, oldValue) {
                scope.ErrorMessages = [];
                for (var i = 0; i < scope.$parent.ValidationError.length; i++) {
                    if (scope.$parent.ValidationError[i].key == scope.key) {
                        scope.ErrorMessages.push(scope.$parent.ValidationError[i].message);
                    }
                }
            });


        }
    }
}]);
