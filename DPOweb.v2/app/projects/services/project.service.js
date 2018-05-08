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
var http_1 = require("@angular/http");
require("rxjs/Rx");
var toastr_service_1 = require("../../shared/services/toastr.service");
//import 'rxjs/add/operator/toPromise';
var ProjectService = /** @class */ (function () {
    function ProjectService(toastrSvc, http) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
    }
    ProjectService.prototype.getProject = function (projectId) {
        return this.http.get("/api/Project/GetProject?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.postProject = function (project) {
        return this.http.post("/api/Project/PostProject", project, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.postProjectAndVerifyAddress = function (project) {
        return this.http.post("/api/Project/PostProjectAndVerifyAddress", project, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.getProjectQuotes = function (projectId) {
        return this.http.get("/api/Project/GetProjectQuotes?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.exportProject = function (data) {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        this.getAttachment("/ProjectDashBoard/ExportProject", data);
    };
    ProjectService.prototype.getAttachment = function (url, params) {
        var form = jQuery('<form method="POST" id="ExportSingleProject" action="' + url + '">');
        jQuery.each(params, function (k, v) {
            form.append(jQuery('<input type="hidden" name="' + k +
                '" value="' + v + '">'));
        });
        var body = jQuery('body');
        body.append(form);
        form.submit();
        body.remove('#ExportSingleProject');
    };
    ProjectService.prototype.transferProject = function (data) {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.post("/ProjectDashBoard/TransferProject", data, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProjectService.prototype.deleteProject = function (projectId) {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.delete("/api/Project/DeleteProject?projectId=" + projectId, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProjectService.prototype.undeleteProject = function (project) {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.post("/api/Project/UndeleteProject", project, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProjectService.prototype.deleteProjects = function (data) {
        var headers = new http_1.Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        return this.http.post("/api/Project/DeleteProjects", data, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    };
    ProjectService.prototype.getNewProjectPipelineNote = function (projectId) {
        return this.http.get("/api/Project/GetNewProjectPipelineNote?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.getProjectPipelineNotes = function (projectId) {
        return this.http.get("/api/Project/GetProjectPipelineNotes?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.getProjectPipelineNoteOptions = function () {
        return this.http.get("/api/Project/GetProjectPipelineNoteTypes", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.postProjectPipelineNote = function (data) {
        return this.http.post("/api/Project/PostProjectPipelineNote", data, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    };
    ProjectService.prototype.extractData = function (res) {
        var resp = res.json();
        return resp || {};
    };
    //public extractFile(res: Response) {
    //    var blob = new Blob([res._body], { type: "application/vnd.ms-excel" });
    //    var objectUrl = URL.createObjectURL(blob);
    //    window.open(objectUrl);
    //}
    ProjectService.prototype.handleError = function (error) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    };
    ProjectService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http])
    ], ProjectService);
    return ProjectService;
}());
exports.ProjectService = ProjectService;
//# sourceMappingURL=project.service.js.map