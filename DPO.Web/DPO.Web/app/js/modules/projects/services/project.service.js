/// <reference path="../Scripts/angular.js" />

(function () {
    "use strict";

    angular
        .module("DPO.Projects")
        .factory("projectService", projectService);

    projectService.$inject = ["$resource", "$http"];

    function projectService($resource, $http) {

        var urlPrefix = "/api/Project";

        function processData(data) {
            if (data.data) {
                return data.data;
            }

            return data;
        }

        function execute(method, action, data, success, fail) {
            var req = {
                url: urlPrefix + "/" + action,
                method: method,
            };

            if (data) {
                if (method.toLowerCase() == "get") {
                    req.params = data;
                } else {
                    req.data = data;
                }
            }

            $http(req).then(function (data, status, headers, config) {
                if (success) {
                    success({
                        data: processData(data),
                        status: status,
                        headers: headers,
                        config: config,
                        isError: false
                    });
                }
            }, function (data, status, headers, config) {
                if (fail) {
                    fail({
                        data: processData(data),
                        status: status,
                        headers: headers,
                        config: config,
                        isError: true
                    });
                }
            });
        }

        return {
            saveProject: function (project, success, fail) {
                execute("POST", "PostProject", project, success, fail);
            },

            getProject: function (projectId, success, fail) {
                execute("GET", "GetProject", { projectId: projectId }, success, fail);
            },

            projectPipelineNote: function (note, success, fail) {
                execute("POST", "PostProjectPipelineNote", note, success, fail);
            },

            projectPipelineNoteTypes: function (success, fail) {
                execute("GET", "GetProjectPipelineNoteTypes", null, success, fail)
            },

            projectPipelineNotes: function (projectId, success, fail) {
                execute("GET", "GetProjectPipelineNotes", { projectId: projectId }, success, fail)
            },

            getAddress: function (addressid, success, fail) {
                execute("GET", "GetAddress", { addressid: addressid }, success, fail)
            }
        }
    }
})();