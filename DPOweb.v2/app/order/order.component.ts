
import { Component, OnInit } from '@angular/core';
//import {Http, Headers, RequestOptions, Response} from '@angular/http';
//import {Observable} from 'rxjs/Observable';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
//import { ProjectService } from './services/project.service';
import { OrderService } from './services/order.service';
declare var jQuery: any;

@Component({
    selector: 'order',
    templateUrl: 'app/order/order.component.html'

})
export class OrderComponent implements OnInit {


    constructor(private toastrSvc: ToastrService, private orderSvc: OrderService) {
    }

    ngOnInit() {
        
    }

    getSubmittalOrder() {
        //alert("hello");
        this.orderSvc.getSubmittalOrder()
            .then((resp: any) => {
                var result = resp;
                debugger;

            })
            .catch(error => {

                console.log(error);
            });

    }

};

