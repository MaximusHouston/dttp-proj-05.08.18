(
function () {
    angular.module("DPO.Projects").directive("enum", ['enumService', function (enumService) {
        return {
            restrict: "E",
            scope: {
                keyid: '@'
            },
            replace: true,
            template: "<span>{{DisplayText}}</span>",
            link: function (scope, elem, attr) {

                scope.$watch('keyid', function (newValue, oldValue) {
                    
                    enumService.getEnumValues(attr.url).perform({}).$promise.then(function (result) {
                        if (result.isok) {
                            var enumItems = result.model;
                            var keyid = scope.keyid;

                            for (var i = 0; i < enumItems.length; i++) {
                                if (enumItems[i].keyId == keyid) {
                                    scope.DisplayText = enumItems[i].displayText;
                                }
                            }

                        }
                    });

                });


            }//end of link function
        }
    }])// end of directive

}());