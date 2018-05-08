angular.module("DPO.Projects")
       .controller('projectEditDetailsController',
                   ['$scope', '$resource', 'DPOprojectService',
                   function ($scope, $resource, DPOprojectService) {

                       $scope.projectVM = new ProjectViewModel();

                       $scope.cancel = function () {
                           $scope.$parent.$parent.ProjectDetailsEditWindow.close();
                       }

                   }]);// end of controller

