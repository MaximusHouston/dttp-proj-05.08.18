import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRoute, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
import { ProjectService } from '../../projects/services/project.service';

@Injectable()
export class ProjectResolver {

    constructor(private projectSvc: ProjectService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

        let projectId = route.params['id'];

        return this.projectSvc.getProject(projectId)
            .map(resp => {
                if (resp) {
                    return resp;
                } else {
                    return null;
                }
            }).catch(error => {
                //console.log('Retrieval error: ${error}');
                console.log(error);
                return Observable.of(null);
            });
    }
}

@Injectable()
export class ProjectQuotesResolver {

    constructor(private projectSvc: ProjectService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

        let projectId = route.params['id'];

        return this.projectSvc.getProjectQuotes(projectId)
            .map(resp => {
                if (resp) {
                    return resp;
                } else {
                    return null;
                }
            }).catch(error => {
                //console.log('Retrieval error: ${error}');
                console.log(error);
                return Observable.of(null);
            });
    }
}

