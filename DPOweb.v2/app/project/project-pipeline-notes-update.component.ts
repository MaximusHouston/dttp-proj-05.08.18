
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
    selector: 'project-pipeline-notes-update',
    templateUrl: 'app/project/project-pipeline-notes-update.component.html'

})
export class ProjectPipelineNotesUpdateComponent implements OnInit {

    @Input() project: any;
    @Input() user: any;

    @Input() canViewPipelineData: any;
    @Input() canEditPipelineData: any;
    
    public newProjectPipelineNote: any;
    public projectPipelineNoteOptions: any;
    public selectedPipelineNoteTypeId: any;// temp
    public projectPipelineNotes: any = [];
    public defaultItem: { name: string, value: any } = { name: "Select ...", value: null };
    public currentEstDeliveryDate: any;
    
    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum, private http: Http, private projectSvc: ProjectService) {

    }

    ngOnChanges() {
        this.project.bidDate = new Date(Date.parse(this.project.bidDate));
        this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
        this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));
        this.currentEstDeliveryDate = new Date(Date.parse(this.project.estimatedDelivery));
    }

    ngOnInit() {
        this.getNewProjectPipelineNote();
        this.getPipelineNoteOptions();
        this.getProjectPipelineNotes();

        //this.project.bidDate = new Date(Date.parse(this.project.bidDate));
        //this.project.estimatedClose = new Date(Date.parse(this.project.estimatedClose));
        //this.project.estimatedDelivery = new Date(Date.parse(this.project.estimatedDelivery));
        //this.currentEstDeliveryDate = new Date(Date.parse(this.project.estimatedDelivery));


    }


    public getNewProjectPipelineNote() {
        var self = this;

        this.projectSvc.getNewProjectPipelineNote(this.project.projectId)
            .subscribe(
            (resp :any) => {
                if (resp.isok) {
                    self.newProjectPipelineNote = resp.model;

                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => console.log("Error: ", err)
            );
    }

    public getPipelineNoteOptions() {
        var self = this;

        this.projectSvc.getProjectPipelineNoteOptions()
            .subscribe(
            (resp: any) => {
                if (resp.isok) {
                    self.projectPipelineNoteOptions = resp.model;

                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => console.log("Error: ", err)
            );
    }

    public getProjectPipelineNotes() {
        var self = this;

        this.projectSvc.getProjectPipelineNotes(this.project.projectId)
            .subscribe(
            (resp: any) => {
                if (resp.isok) {
                    //self.projectPipelineNotes = resp.model;
                    this.pipelineNotesReverse(resp.model);

                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => console.log("Error: ", err)
            );
    }

    public pipelineNotesReverse(notes: any) {
        for (var i = 0; i < notes.items.length; i++){
            this.projectPipelineNotes.unshift(notes.items[i]);
        }

    }

    public pipelineNoteChange(value: any) {
        for (var i = 0; i < this.projectPipelineNoteOptions.items.length; i++) {
            if (this.projectPipelineNoteOptions.items[i].projectPipelineNoteTypeId == value) {
                this.newProjectPipelineNote.projectPipelineNoteType = this.projectPipelineNoteOptions.items[i];
            }
        }
    }

    public addPipelineNote() {

        //TODO: Estimate Delivery date push forward/back
        if (this.newProjectPipelineNote.projectPipelineNoteId == 4 || this.newProjectPipelineNote.projectPipelineNoteId == 5) {
            jQuery("#estimateDeliveryDialog").modal({ backdrop: 'static', keyboard: false });
        } else if (this.newProjectPipelineNote.projectPipelineNoteId == 1) {// convert to Opportunity
            this.project.projectLeadStatusTypeId = 2;// Opportunity
            this.projectSvc.postProject(this.project)
                .subscribe(
                resp => {
                    if (resp.isok) {
                        this.project.projectLeadStatusTypeDescription = "Opportunity";
                        this.postPipelineNote();
                    } else {
                        this.toastrSvc.displayResponseMessages(resp);
                    }

                },
                err => console.log("Error: ", err)
                );
        }
        else {
            this.postPipelineNote();
        }

        
    }

    public updateDeliveryDate() {
        this.projectSvc.postProject(this.project)
            .subscribe(
            resp => {
                if (resp.isok) {
                    this.postPipelineNote();
                    this.currentEstDeliveryDate = this.project.estimatedDelivery;
                } else {
                    this.toastrSvc.displayResponseMessages(resp);
                    this.project.estimatedDelivery = this.currentEstDeliveryDate;
                }

            },
            err => console.log("Error: ", err)
            );
    }

    public postPipelineNote() {
        var self = this;
        this.projectSvc.postProjectPipelineNote(this.newProjectPipelineNote)
            .subscribe(
            resp => {
                if (resp.isok) {
                    self.newProjectPipelineNote = resp.model;
                    self.getProjectPipelineNotes();
                    self.getNewProjectPipelineNote();
                    self.toastrSvc.displayResponseMessages(resp);
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }

            },
            err => console.log("Error: ", err)
            );
    }
		

};

