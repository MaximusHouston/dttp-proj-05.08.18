
import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from '@angular/http';
import {Observable} from 'rxjs/Observable';
//import './rxjs-operators';
//import 'rxjs/add/operator/map';
//import 'rxjs/add/operator/catch';
//import 'rxjs/add/operator/toPromise';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { ProjectService } from './services/project.service';
declare var jQuery: any;

@Component({
    selector: 'transferProject-popup',
    templateUrl: 'app/projects/transferProjectPopup.component.html'

})
export class TransferProjectPopupComponent implements OnInit {
    @Input() selectedProjectId: any;
    @Output() closeWindow = new EventEmitter();
    transferToEmail: string = "";

    constructor(private toastrSvc: ToastrService, private http: Http, private projectSvc: ProjectService) {

    }

    ngOnInit() {
        //this.projectSvc
    }

    public closePopup() {
        this.closeWindow.emit({});
    }

    public transferProject() {

        var data = {
               "projectId": this.selectedProjectId,
               "email": this.transferToEmail
        };

        this.projectSvc.transferProject(data)
            .then(this.transferProjectCallback.bind(this) );
     

        //reset email
        this.transferToEmail = "";
    }

    public transferProjectCallback(resp: any) {
        if (resp.IsOK) {
            for (let message of resp.Messages.Items) {
                if (message.Type == 40) {// success
                    this.toastrSvc.Success(message.Text);
                }
            }

            //reload projects grid
            var projectEditAllGridDtaSrc = jQuery('#project-grid').data('kendoGrid').dataSource
            projectEditAllGridDtaSrc.read();

        } else {
            for (let message of resp.Messages.Items) {
                if (message.Type == 10) {// error
                    this.toastrSvc.Error(message.Text);
                }
            }
        }
        this.closePopup();
    }


};

