
import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from '@angular/http';
import {Observable} from 'rxjs/Observable';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { ProjectService } from './services/project.service';
declare var jQuery: any;

@Component({
    selector: 'exportProjects-popup',
    templateUrl: 'app/projects/exportProjectsPopup.component.html'

})
export class ExportProjectsPopupComponent implements OnInit {

    @Input() showDeletedProjects: any;
    public projectExportType = 1;

    constructor(private toastrSvc: ToastrService, private http: Http, private projectSvc: ProjectService) {

    }

    ngOnInit() {
        this.setupExportTypeDDL();
    }

    public setupExportTypeDDL() {
        var self = this;
        jQuery("#projectExportTypeDDL").kendoDropDownList({
            dataSource: [{ text: "Project Pipeline Export", value: 1 },
                { text: "Project Pipeline Export - Detailed", value: 2 }],
            dataTextField: "text",
            dataValueField: "value",
            change: function (e:any) {
                var value = this.value();
                self.projectExportType = value;
            }
        });
    }

    public closeExportProjectWindow() {
        var exportProjectsWindow = jQuery("#exportProjectsWindow").data("kendoWindow");
        exportProjectsWindow.close();
    }

    public exportProjects() {
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

    }


};

