import { Component } from '@angular/core';
import { ToastrService } from '../services/toastr.service';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'base-errorhandler', 
    templateUrl: 'app/shared/common/BaseErrorHandler.component.html'
})
export class BaseErrorHandler {

    constructor(private toastrSvc: ToastrService) { };

    public handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message

        //console.error(error); // log to console instead
        console.log(error);
        this.toastrSvc.Error(error.statusText);
        return Observable.throw(error.statusText);
    }
};