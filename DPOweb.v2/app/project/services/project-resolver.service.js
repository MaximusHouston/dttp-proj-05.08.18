"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var Observable_1 = require("rxjs/Observable");
require("rxjs/Rx");
var project_service_1 = require("../../projects/services/project.service");
var ProjectResolver = /** @class */ (function () {
    function ProjectResolver(projectSvc) {
        this.projectSvc = projectSvc;
    }
    ProjectResolver.prototype.resolve = function (route, state) {
        var projectId = route.params['id'];
        return this.projectSvc.getProject(projectId)
            .map(function (resp) {
            if (resp) {
                return resp;
            }
            else {
                return null;
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
            return Observable_1.Observable.of(null);
        });
    };
    ProjectResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [project_service_1.ProjectService])
    ], ProjectResolver);
    return ProjectResolver;
}());
exports.ProjectResolver = ProjectResolver;
var ProjectQuotesResolver = /** @class */ (function () {
    function ProjectQuotesResolver(projectSvc) {
        this.projectSvc = projectSvc;
    }
    ProjectQuotesResolver.prototype.resolve = function (route, state) {
        var projectId = route.params['id'];
        return this.projectSvc.getProjectQuotes(projectId)
            .map(function (resp) {
            if (resp) {
                return resp;
            }
            else {
                return null;
            }
        }).catch(function (error) {
            //console.log('Retrieval error: ${error}');
            console.log(error);
            return Observable_1.Observable.of(null);
        });
    };
    ProjectQuotesResolver = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [project_service_1.ProjectService])
    ], ProjectQuotesResolver);
    return ProjectQuotesResolver;
}());
exports.ProjectQuotesResolver = ProjectQuotesResolver;
//# sourceMappingURL=project-resolver.service.js.map