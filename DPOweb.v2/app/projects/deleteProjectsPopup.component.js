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
var toastr_service_1 = require("../shared/services/toastr.service");
var project_service_1 = require("./services/project.service");
var DeleteProjectsPopupComponent = /** @class */ (function () {
    function DeleteProjectsPopupComponent(toastrSvc, http, projectSvc) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.projectSvc = projectSvc;
        this.clearDeleteProjectsArray = new core_1.EventEmitter();
    }
    DeleteProjectsPopupComponent.prototype.ngOnInit = function () {
    };
    DeleteProjectsPopupComponent.prototype.deleteSelectedProjects = function () {
        var self = this;
        var selectedProjectIds = [];
        for (var _i = 0, _a = this.deleteProjects; _i < _a.length; _i++) {
            var project = _a[_i];
            selectedProjectIds.push(project.projectId);
        }
        this.projectSvc.deleteProjects(selectedProjectIds).then(this.deleteProjectsCallback.bind(this));
        ;
    };
    DeleteProjectsPopupComponent.prototype.closeDeleteProjectsWindow = function () {
        var deleteProjectsWindow = jQuery("#deleteProjectsWindow").data("kendoWindow");
        deleteProjectsWindow.close();
    };
    DeleteProjectsPopupComponent.prototype.deleteProjectsCallback = function (resp) {
        if (resp.isok) {
            for (var _i = 0, _a = resp.messages.items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.type == 40) {
                    this.toastrSvc.Success(message.text);
                }
            }
            //reload projects grid
            var projectEditAllGridDtaSrc = jQuery('#project-grid').data('kendoGrid').dataSource;
            projectEditAllGridDtaSrc.read();
            //clear deleteProjects Array
            this.clearDeleteProjectsArray.emit({});
        }
        else {
            for (var _b = 0, _c = resp.messages.items; _b < _c.length; _b++) {
                var message = _c[_b];
                if (message.type == 10) {
                    this.toastrSvc.Error(message.text);
                }
            }
        }
        this.closeDeleteProjectsWindow();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], DeleteProjectsPopupComponent.prototype, "deleteProjects", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", Object)
    ], DeleteProjectsPopupComponent.prototype, "clearDeleteProjectsArray", void 0);
    DeleteProjectsPopupComponent = __decorate([
        core_1.Component({
            selector: 'deleteProjects-popup',
            templateUrl: 'app/projects/deleteProjectsPopup.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http, project_service_1.ProjectService])
    ], DeleteProjectsPopupComponent);
    return DeleteProjectsPopupComponent;
}());
exports.DeleteProjectsPopupComponent = DeleteProjectsPopupComponent;
;
//# sourceMappingURL=deleteProjectsPopup.component.js.map