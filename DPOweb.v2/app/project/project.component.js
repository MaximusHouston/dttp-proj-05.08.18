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
var router_1 = require("@angular/router");
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var enums_1 = require("../shared/enums/enums");
var project_service_1 = require("../projects/services/project.service");
var ProjectComponent = /** @class */ (function () {
    function ProjectComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, enums, http, projectSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.enums = enums;
        this.http = http;
        this.projectSvc = projectSvc;
        this.canViewPipelineData = false;
        this.canEditPipelineData = false;
        this.canEditProject = false;
        //public newProjectPipelineNote: any;
        //public projectPipelineNoteOptions: any;
        //public selectedPipelineNoteTypeId: any;// temp
        //public projectPipelineNotes: any;
        this.defaultItem = { text: "Select ...", value: null };
        this.activeTab = this.route.snapshot.url[0].path;
        this.project = this.route.snapshot.data['projectModel'].model;
        this.projectQuotes = this.route.snapshot.data['projectQuotesModel'].model;
        this.user = this.route.snapshot.data['currentUser'].model;
    }
    ProjectComponent.prototype.ngOnInit = function () {
        if (this.activeTab == "projectQuotes") {
            jQuery("#projectQuotesTabLink").click();
            //jQuery("#projectQuotesTabHeader").addClass("active");
            //jQuery("#projectOverviewTabHeader").removeClass("active");
        }
        this.canViewPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewPipelineData);
        this.canEditPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditPipelineData);
        this.canEditProject = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditProject);
    };
    ProjectComponent.prototype.onTabSelect = function (event) {
    };
    ProjectComponent.prototype.reloadData = function () {
        this.reloadProject();
        this.reloadProjectQuotes();
    };
    ProjectComponent.prototype.reloadProject = function () {
        var self = this;
        this.projectSvc.getProject(this.project.projectId)
            .subscribe(function (resp) {
            if (resp.isok) {
                self.project = resp.model;
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectComponent.prototype.reloadProjectQuotes = function () {
        var self = this;
        this.projectSvc.getProjectQuotes(this.project.projectId)
            .subscribe(function (resp) {
            if (resp.isok) {
                self.projectQuotes = resp.model;
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectComponent.prototype.showQuoteOverview = function () {
        this.router.navigateByUrl("/quote/" + this.project.activeQuoteSummary.quoteId + "/existingRecord");
    };
    ProjectComponent.prototype.deleteProject = function () {
        var _this = this;
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.deleteProject(this.project.projectId)
            .then(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessages(resp);
                _this.project.deleted = true;
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            console.log(error);
        });
    };
    ProjectComponent.prototype.undeleteProject = function () {
        var _this = this;
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.undeleteProject(this.project)
            .then(function (resp) {
            if (resp.isok) {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessages(resp);
                _this.project.deleted = false;
            }
            else {
                _this.loadingIconSvc.Stop(jQuery("#content"));
                _this.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            console.log(error);
        });
    };
    ProjectComponent = __decorate([
        core_1.Component({
            selector: 'project',
            templateUrl: 'app/project/project.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            enums_1.Enums, http_1.Http, project_service_1.ProjectService])
    ], ProjectComponent);
    return ProjectComponent;
}());
exports.ProjectComponent = ProjectComponent;
;
//# sourceMappingURL=project.component.js.map