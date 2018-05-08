
import { Component, OnInit, Input, Output, EventEmitter, ViewChildren } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { ProjectService } from '../projects/services/project.service';
declare var jQuery: any;

@Component({
    selector: 'project',
    templateUrl: 'app/project/project.component.html'
})
export class ProjectComponent implements OnInit {

    public activeTab: any;

    public project: any;
    public projectQuotes: any;
    public user: any;

    public canViewPipelineData: boolean = false;
    public canEditPipelineData: boolean = false;
    public canEditProject: boolean = false;

    //public newProjectPipelineNote: any;
    //public projectPipelineNoteOptions: any;
    //public selectedPipelineNoteTypeId: any;// temp
    //public projectPipelineNotes: any;

    public defaultItem: { text: string, value: number } = { text: "Select ...", value: null };

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http, private projectSvc: ProjectService) {

        this.activeTab = this.route.snapshot.url[0].path;

        this.project = this.route.snapshot.data['projectModel'].model;
        this.projectQuotes = this.route.snapshot.data['projectQuotesModel'].model;
        this.user = this.route.snapshot.data['currentUser'].model;
         

    }

    ngOnInit() {
        if (this.activeTab == "projectQuotes") {
            jQuery("#projectQuotesTabLink").click();
            //jQuery("#projectQuotesTabHeader").addClass("active");
            //jQuery("#projectOverviewTabHeader").removeClass("active");
        }

        this.canViewPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewPipelineData);
        this.canEditPipelineData = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditPipelineData);
        this.canEditProject = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditProject);
    }

    onTabSelect(event: any) {
        
    }
       

    public reloadData() {
        this.reloadProject();
        this.reloadProjectQuotes();
    }

    public reloadProject() {
        var self = this;
        this.projectSvc.getProject(this.project.projectId)
            .subscribe(
            resp => {
                if (resp.isok) {
                    self.project = resp.model
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            },
            err => console.log("Error: ", err)
            );
    }

    public reloadProjectQuotes() {
        var self = this;
        this.projectSvc.getProjectQuotes(this.project.projectId)
            .subscribe(
            resp => {
                if (resp.isok) {
                    self.projectQuotes = resp.model
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            },
            err => console.log("Error: ", err)
            );
    }

    public showQuoteOverview() {
        this.router.navigateByUrl("/quote/" + this.project.activeQuoteSummary.quoteId + "/existingRecord");
    }

    public deleteProject() {
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.deleteProject(this.project.projectId)
            .then((resp: any) => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                    this.project.deleted = true;
                } else {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(error => {
                console.log(error);
            });
    }

    public undeleteProject() {
        this.loadingIconSvc.Start(jQuery("#content"));
        this.projectSvc.undeleteProject(this.project)
            .then((resp: any) => {
                if (resp.isok) {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                    this.project.deleted = false;
                } else {
                    this.loadingIconSvc.Stop(jQuery("#content"));
                    this.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(error => {
                console.log(error);
            });
    }




};

