
import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from '@angular/http';
import {Observable} from 'rxjs/Observable';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { ProjectService } from './services/project.service';
declare var jQuery: any;

@Component({
    selector: 'deleteProjects-popup',
    templateUrl: 'app/projects/deleteProjectsPopup.component.html'

})
export class DeleteProjectsPopupComponent implements OnInit {
    @Input() deleteProjects: any;
    @Output() clearDeleteProjectsArray = new EventEmitter();
    

    constructor(private toastrSvc: ToastrService, private http: Http, private projectSvc: ProjectService) {

    }

    ngOnInit() {
        
    }

    public deleteSelectedProjects() {
        var self = this;
        var selectedProjectIds: any = [];
        for (let project of this.deleteProjects) {
            selectedProjectIds.push(project.projectId);
        }
        
        this.projectSvc.deleteProjects(selectedProjectIds).then(this.deleteProjectsCallback.bind(this));;
        
    }

    public closeDeleteProjectsWindow() {
        var deleteProjectsWindow = jQuery("#deleteProjectsWindow").data("kendoWindow");
        deleteProjectsWindow.close();
    }

    public deleteProjectsCallback(resp: any) {
        if (resp.isok) {
            for (let message of resp.messages.items) {
                if (message.type == 40) {// success
                    this.toastrSvc.Success(message.text);
                }
            }
            //reload projects grid
            var projectEditAllGridDtaSrc = jQuery('#project-grid').data('kendoGrid').dataSource
            projectEditAllGridDtaSrc.read();

            //clear deleteProjects Array
            this.clearDeleteProjectsArray.emit({});

        } else {
            for (let message of resp.messages.items) {
                if (message.type == 10) {// error
                    this.toastrSvc.Error(message.text);
                }
            }
        }
        this.closeDeleteProjectsWindow();
    }

   


};

