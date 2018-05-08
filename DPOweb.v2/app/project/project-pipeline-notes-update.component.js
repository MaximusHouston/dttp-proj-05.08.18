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
var project_service_1 = require("../projects/services/project.service");
var ProjectPipelineNotesUpdateComponent = /** @class */ (function () {
    function ProjectPipelineNotesUpdateComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, http, projectSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.projectSvc = projectSvc;
        this.projectPipelineNotes = [];
        this.defaultItem = { name: "Select ...", value: null };
    }
    ProjectPipelineNotesUpdateComponent.prototype.ngOnChanges = function () {
        this.project.bidDate = new Date(Date.parse(this.project.bidDate));
        this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
        this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));
        this.currentEstDeliveryDate = new Date(Date.parse(this.project.estimatedDelivery));
    };
    ProjectPipelineNotesUpdateComponent.prototype.ngOnInit = function () {
        this.getNewProjectPipelineNote();
        this.getPipelineNoteOptions();
        this.getProjectPipelineNotes();
        //this.project.bidDate = new Date(Date.parse(this.project.bidDate));
        //this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
        //this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));
        //this.currentEstDeliveryDate = new Date(Date.parse(this.project.estimatedDelivery));
    };
    ProjectPipelineNotesUpdateComponent.prototype.getNewProjectPipelineNote = function () {
        var self = this;
        this.projectSvc.getNewProjectPipelineNote(this.project.projectId)
            .subscribe(function (resp) {
            if (resp.isok) {
                self.newProjectPipelineNote = resp.model;
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectPipelineNotesUpdateComponent.prototype.getPipelineNoteOptions = function () {
        var self = this;
        this.projectSvc.getProjectPipelineNoteOptions()
            .subscribe(function (resp) {
            if (resp.isok) {
                self.projectPipelineNoteOptions = resp.model;
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectPipelineNotesUpdateComponent.prototype.getProjectPipelineNotes = function () {
        var _this = this;
        var self = this;
        this.projectSvc.getProjectPipelineNotes(this.project.projectId)
            .subscribe(function (resp) {
            if (resp.isok) {
                //self.projectPipelineNotes = resp.model;
                _this.pipelineNotesReverse(resp.model);
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectPipelineNotesUpdateComponent.prototype.pipelineNotesReverse = function (notes) {
        for (var i = 0; i < notes.items.length; i++) {
            this.projectPipelineNotes.unshift(notes.items[i]);
        }
    };
    ProjectPipelineNotesUpdateComponent.prototype.pipelineNoteChange = function (value) {
        for (var i = 0; i < this.projectPipelineNoteOptions.items.length; i++) {
            if (this.projectPipelineNoteOptions.items[i].projectPipelineNoteTypeId == value) {
                this.newProjectPipelineNote.projectPipelineNoteType = this.projectPipelineNoteOptions.items[i];
            }
        }
    };
    ProjectPipelineNotesUpdateComponent.prototype.addPipelineNote = function () {
        var _this = this;
        //TODO: Estimate Delivery date push forward/back
        if (this.newProjectPipelineNote.projectPipelineNoteId == 4 || this.newProjectPipelineNote.projectPipelineNoteId == 5) {
            jQuery("#estimateDeliveryDialog").modal({ backdrop: 'static', keyboard: false });
        }
        else if (this.newProjectPipelineNote.projectPipelineNoteId == 1) {
            this.project.projectLeadStatusTypeId = 2; // Opportunity
            this.projectSvc.postProject(this.project)
                .subscribe(function (resp) {
                if (resp.isok) {
                    _this.project.projectLeadStatusTypeDescription = "Opportunity";
                    _this.postPipelineNote();
                }
                else {
                    _this.toastrSvc.displayResponseMessages(resp);
                }
            }, function (err) { return console.log("Error: ", err); });
        }
        else {
            this.postPipelineNote();
        }
    };
    ProjectPipelineNotesUpdateComponent.prototype.updateDeliveryDate = function () {
        var _this = this;
        this.projectSvc.postProject(this.project)
            .subscribe(function (resp) {
            if (resp.isok) {
                _this.postPipelineNote();
                _this.currentEstDeliveryDate = _this.project.estimatedDelivery;
            }
            else {
                _this.toastrSvc.displayResponseMessages(resp);
                _this.project.estimatedDelivery = _this.currentEstDeliveryDate;
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    ProjectPipelineNotesUpdateComponent.prototype.postPipelineNote = function () {
        var self = this;
        this.projectSvc.postProjectPipelineNote(this.newProjectPipelineNote)
            .subscribe(function (resp) {
            if (resp.isok) {
                self.newProjectPipelineNote = resp.model;
                self.getProjectPipelineNotes();
                self.getNewProjectPipelineNote();
                self.toastrSvc.displayResponseMessages(resp);
            }
            else {
                self.toastrSvc.displayResponseMessages(resp);
            }
        }, function (err) { return console.log("Error: ", err); });
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectPipelineNotesUpdateComponent.prototype, "project", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectPipelineNotesUpdateComponent.prototype, "user", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectPipelineNotesUpdateComponent.prototype, "canViewPipelineData", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectPipelineNotesUpdateComponent.prototype, "canEditPipelineData", void 0);
    ProjectPipelineNotesUpdateComponent = __decorate([
        core_1.Component({
            selector: 'project-pipeline-notes-update',
            templateUrl: 'app/project/project-pipeline-notes-update.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, http_1.Http, project_service_1.ProjectService])
    ], ProjectPipelineNotesUpdateComponent);
    return ProjectPipelineNotesUpdateComponent;
}());
exports.ProjectPipelineNotesUpdateComponent = ProjectPipelineNotesUpdateComponent;
;
//# sourceMappingURL=project-pipeline-notes-update.component.js.map