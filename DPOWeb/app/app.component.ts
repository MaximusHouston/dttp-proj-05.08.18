import { Component } from 'angular2/core'
import { ProductListComponent } from './products/product-list.component';

@Component({
    selector: 'Daikin-app',
    template: '<div><h1>{{pageTitle}}</h1>' +
    '<Daikin-products></Daikin-products>' +
    '</div>',
    directives: [ProductListComponent]
})

export class AppComponent {
    pageTitle: string = "Daikin Product Management";
}