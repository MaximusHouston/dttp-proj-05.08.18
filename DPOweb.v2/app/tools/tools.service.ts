import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';
import { ToastrService } from '../shared/services/toastr.service';
import { Tool } from './tool';
import { BaseErrorHandler } from '../shared/common/BaseErrorHandler.component';

@Injectable()
export class ToolsService extends BaseErrorHandler {

    constructor(private http: Http, private toastrService: ToastrService) {
        super(toastrService);
        console.log('Tools Service Initialized...');
    }
    
    getTools() {

        let data = this.http.get("/api/Tool/GetTools")
                    .map(this.extractData)
                    .catch(this.handleError);
        return data;
    }

    private extractData(res: Response) {
        const body = res.json() as Tool[];
        return body || null;
    }
}
