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
var core_1 = require("@angular/core");
var toastr_service_1 = require("../shared/services/toastr.service");
var ProjectsComponent = (function () {
    //public editMode: boolean = false;
    function ProjectsComponent(elementRef, toastrService) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrService;
        //this.projectsGridDataSource = this.getData();
    }
    ProjectsComponent.prototype.ngOnInit = function () {
        this.projectsGridDataSource = this.getData();
    };
    ProjectsComponent.prototype.ngAfterContentInit = function () {
        //setTimeout(this.removeKIconText, 1000); // wait 1 sec
    };
    //public removeKIconText() {
    //    $(".k-icon").text("");
    //    $(".k-i-refresh").text("");
    //}
    //public EditAll() {
    //    this.editMode = true;
    //}
    //public StopEditAll() {
    //    this.editMode = false;
    //}
    ProjectsComponent.prototype.getData = function () {
        var self = this;
        var projectsDataSource = new kendo.data.DataSource({
            //type: "json",
            transport: {
                read: {
                    url: "/api/Project/GetProjects",
                    dataType: "json",
                    type: "GET",
                    cache: true
                },
                update: {
                    url: "/api/Project/EditProjects",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json"
                },
                parameterMap: function (data, operation) {
                    //if (operation !== "read" && data.models) {// batch edit ( use "data" for single record edit )
                    //    return { models: kendo.stringify(data.models) };
                    //}
                    if (operation !== "read" && data) {
                        return kendo.stringify(data);
                    }
                    else if (operation == "read") {
                        var queryInfo = {
                            take: data.take,
                            skip: data.skip,
                            page: data.page,
                            pageSize: data.pageSize
                        };
                        return queryInfo;
                    }
                }
            },
            //batch: true,
            sort: ({ field: "projectId", dir: "desc" }),
            schema: {
                data: function (response) {
                    self.projectListModelData = response.model;
                    return response.model.items;
                },
                model: {
                    fields: {
                        id: 'projectId',
                        projectId: { type: 'number', editable: false },
                        projectIdStr: { type: 'string', editable: false },
                        name: { type: 'string', editable: false },
                        projectOwner: { type: 'string', editable: false },
                        projectDate: { type: 'date' },
                        projectStatus: { type: 'string' },
                        projectOpenStatus: { type: 'string' },
                        projectType: { type: 'string' },
                        bidDate: { type: 'date' },
                        estimatedClose: { type: 'date' },
                        estimatedDelivery: { type: 'date' }
                    }
                },
                total: "model.totalRecords"
            },
            pageSize: 50,
            serverPaging: true,
        });
        //projectsDataSource.read();
        return projectsDataSource;
    }; // end of get Data
    return ProjectsComponent;
}());
ProjectsComponent = __decorate([
    core_1.Component({
        selector: 'projects-tab',
        //styleUrls: [
        //     'app/content/kendo/kendo.common.min.css',
        //     'app/content/kendo/kendo.default.min.css',
        //     'app/content/kendo/kendo.default.mobile.min.css',
        //     'node_modules/bootstrap/dist/css/bootstrap.min.css'
        //],
        templateUrl: 'app/projects/projectsTab.component.html',
        //directives: [ProjectsGridComponent, ProjectsEditAllGridComponent, ProjectsGridFilterComponent],
        providers: [toastr_service_1.ToastrService]
    }),
    __metadata("design:paramtypes", [core_1.ElementRef, toastr_service_1.ToastrService])
], ProjectsComponent);
exports.ProjectsComponent = ProjectsComponent;
//# sourceMappingURL=projectsTab.component.js.map