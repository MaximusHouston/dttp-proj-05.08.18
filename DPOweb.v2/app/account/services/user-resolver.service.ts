import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRoute, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../../shared/services/toastr.service';
import { AccountService } from './account.service';
import { UserService } from '../../shared/services/user.service';

@Injectable()
export class UserResolver  {
    constructor(private accountSvc: AccountService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        return this.accountSvc.getUserRegistrationModel()
            .map(user => {
                if (user) {
                    return user;
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
export class CurrentUserResolver {
    constructor(private accountSvc: AccountService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        return this.accountSvc.getCurrentUser()
            .map(currentUser => {
                if (currentUser) {
                    return currentUser;
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