import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
//import 'rxjs/add/operator/toPromise';

@Injectable()
export class ProjectService {
    constructor(private toastrSvc: ToastrService, private http: Http) {
    }

    private headers = new Headers({
        'Content-Type': 'application/json',
        'dataType': 'json',
        'Accept': 'application/json'
    });
        

    public getProject(projectId: any): Observable<any> {
        return this.http.get("/api/Project/GetProject?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public postProject(project: any): Observable<any> {
        return this.http.post("/api/Project/PostProject", project, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public postProjectAndVerifyAddress(project: any): Observable<any> {
        return this.http.post("/api/Project/PostProjectAndVerifyAddress", project, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public getProjectQuotes(projectId: any): Observable<any> {
        return this.http.get("/api/Project/GetProjectQuotes?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }



    public exportProject(data: any) {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });


        this.getAttachment("/ProjectDashBoard/ExportProject", data);

 
    }

    private getAttachment(url: any, params: any) {
        var form = jQuery('<form method="POST" id="ExportSingleProject" action="' + url + '">');

        jQuery.each(params, function (k, v) {
            form.append(jQuery('<input type="hidden" name="' + k +
                '" value="' + v + '">'));
        });

        var body = jQuery('body');

        body.append(form);
        form.submit();
        body.remove('#ExportSingleProject');
    }


    public transferProject(data: any) {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });
        
        return this.http.post("/ProjectDashBoard/TransferProject", data, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public deleteProject(projectId: any) {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });

        return this.http.delete("/api/Project/DeleteProject?projectId=" + projectId, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public undeleteProject(project: any) {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });

        return this.http.post("/api/Project/UndeleteProject", project , { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public deleteProjects(data: any) {

        let headers = new Headers({
            'Content-Type': 'application/json',
            'dataType': 'json',
            'Accept': 'application/json'
        });

        return this.http.post("/api/Project/DeleteProjects", data, { headers: headers }).toPromise().then(this.extractData).catch(this.handleError);
    }

    public getNewProjectPipelineNote(projectId: any) {
        return this.http.get("/api/Project/GetNewProjectPipelineNote?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

    }

    public getProjectPipelineNotes(projectId: any) {
        return this.http.get("/api/Project/GetProjectPipelineNotes?projectId=" + projectId, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

    }

    public getProjectPipelineNoteOptions() {
        return this.http.get("/api/Project/GetProjectPipelineNoteTypes", { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);

    }

    public postProjectPipelineNote(data: any): Observable<any> {
        return this.http.post("/api/Project/PostProjectPipelineNote", data, { headers: this.headers })
            .map(this.extractData)
            .catch(this.handleError);
    }

    public extractData(res: Response) {
        let resp = res.json();
        return resp || {};
    }


    //public extractFile(res: Response) {

    //    var blob = new Blob([res._body], { type: "application/vnd.ms-excel" });
    //    var objectUrl = URL.createObjectURL(blob);
    //    window.open(objectUrl);
    //}


    public handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message

        console.error(error); // log to console instead
        this.toastrSvc.Error(error.statusText);
        return Promise.reject(error.statusText);
    }



}