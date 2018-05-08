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
var ExportProjectsPopupComponent = /** @class */ (function () {
    function ExportProjectsPopupComponent(toastrSvc, http, projectSvc) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.projectSvc = projectSvc;
        this.projectExportType = 1;
    }
    ExportProjectsPopupComponent.prototype.ngOnInit = function () {
        this.setupExportTypeDDL();
    };
    ExportProjectsPopupComponent.prototype.setupExportTypeDDL = function () {
        var self = this;
        jQuery("#projectExportTypeDDL").kendoDropDownList({
            dataSource: [{ text: "Project Pipeline Export", value: 1 },
                { text: "Project Pipeline Export - Detailed", value: 2 }],
            dataTextField: "text",
            dataValueField: "value",
            change: function (e) {
                var value = this.value();
                self.projectExportType = value;
            }
        });
    };
    ExportProjectsPopupComponent.prototype.closeExportProjectWindow = function () {
        var exportProjectsWindow = jQuery("#exportProjectsWindow").data("kendoWindow");
        exportProjectsWindow.close();
    };
    ExportProjectsPopupComponent.prototype.exportProjects = function () {
        var filterString = "";
        var sortString = "";
        var prjectsDataSrc = jQuery("#project-grid").data("kendoGrid").dataSource;
        if (prjectsDataSrc.filter() != undefined) {
            filterString = JSON.stringify(prjectsDataSrc.filter()).replace(/\"/g, '\'');
        }
        if (prjectsDataSrc.sort() != undefined) {
            sortString = JSON.stringify(prjectsDataSrc.sort()).replace(/\"/g, '\'');
        }
        var data = {
            "projectExportType": this.projectExportType,
            "showDeletedProjects": this.showDeletedProjects,
            "filter": filterString,
            "sort": sortString
        };
        this.projectSvc.exportProject(data);
        this.closeExportProjectWindow();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ExportProjectsPopupComponent.prototype, "showDeletedProjects", void 0);
    ExportProjectsPopupComponent = __decorate([
        core_1.Component({
            selector: 'exportProjects-popup',
            templateUrl: 'app/projects/exportProjectsPopup.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http, project_service_1.ProjectService])
    ], ExportProjectsPopupComponent);
    return ExportProjectsPopupComponent;
}());
exports.ExportProjectsPopupComponent = ExportProjectsPopupComponent;
;
//# sourceMappingURL=exportProjectsPopup.component.js.map