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
var router_1 = require("@angular/router");
var toastr_service_1 = require("../shared/services/toastr.service");
var user_service_1 = require("../shared/services/user.service");
var enums_1 = require("../shared/enums/enums");
var ProjectsComponent = /** @class */ (function () {
    //public editMode: boolean = false;
    function ProjectsComponent(elementRef, router, route, toastrService, userSvc, enums) {
        this.router = router;
        this.route = route;
        this.userSvc = userSvc;
        this.enums = enums;
        this.canViewProject = false;
        this.elementRef = elementRef;
        this.toastrSvc = toastrService;
        //this.projectsGridDataSource = this.getData();
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    ProjectsComponent.prototype.ngOnInit = function () {
        this.userSvc.isAuthenticated().then(function (resp) {
            if (!resp.isok || resp.model != true) {
                //Go back to login page
                window.location.href = "/v2/#/account/login";
            }
        });
        this.canViewProject = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewProject);
        //if (!this.userSvc.userIsAuthenticated) {
        //    window.location.href = "/Account/Login";
        //    //window.location.href = "/v2/#/account/login";
        //}
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
                        //totalListPrice: { type: 'number' },
                        //totalNetPrice: { type: 'number' },
                        //totalSellPrice: { type: 'number' },
                        //darComStatus: { type: 'string' },
                        //vrvODUcount: { type: 'number' },
                        //splitODUcount: { type: 'number' },
                        //pricingTypeId: { type: 'number' },
                        //pricingTypeDescription: { type: 'string' },
                        //poAttachmentName: { type: 'string' },
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
    ProjectsComponent = __decorate([
        core_1.Component({
            selector: 'projects',
            //styleUrls: [
            //     'app/content/kendo/kendo.common.min.css',
            //     'app/content/kendo/kendo.default.min.css',
            //     'app/content/kendo/kendo.default.mobile.min.css',
            //     'node_modules/bootstrap/dist/css/bootstrap.min.css'
            //],
            templateUrl: 'app/projects/projects.component.html',
            //directives: [ProjectsGridComponent, ProjectsEditAllGridComponent, ProjectsGridFilterComponent],
            providers: [toastr_service_1.ToastrService]
        }),
        __metadata("design:paramtypes", [core_1.ElementRef, router_1.Router, router_1.ActivatedRoute,
            toastr_service_1.ToastrService, user_service_1.UserService, enums_1.Enums])
    ], ProjectsComponent);
    return ProjectsComponent;
}());
exports.ProjectsComponent = ProjectsComponent;
//# sourceMappingURL=projects.component.js.map