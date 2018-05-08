import { Component, OnInit } from '@angular/core';
import {
    Router,
    // import as RouterEvent to avoid confusion with the DOM Event
    Event as RouterEvent,
    NavigationStart,
    NavigationEnd,
    NavigationCancel,
    NavigationError } from '@angular/router';

//import { ROUTER_DIRECTIVES } from '@angular/router';
import { ToastrService } from './shared/services/toastr.service';
import { LoadingIconService } from './shared/services/loadingIcon.service';
import { UserService } from './shared/services/user.service';
import { SystemAccessEnum } from './shared/services/systemAccessEnum';
import { PasswordService } from './shared/services/password.service';

//import { ProductFamilyEnum } from './shared/enums/productFamilyEnum';
import { Enums } from './shared/enums/enums';
import { AccountService } from './account/services/account.service';
import { BusinessService } from './business/services/business.service';
import { UserResolver } from './account/services/user-resolver.service';

import { ProjectService } from './projects/services/project.service';
import { ProjectsComponent } from './projects/projects.component';
import { HeaderButtonsComponent } from './shared/header/headerButtons.component';
//import { ProductService } from './products/services/product.service';
//import { ProductFamilyService } from './products/services/productFamily.service';

import { ProductService } from './products/services/product.service';
import { BasketService } from './basket/services/basket.service';
import { SystemConfiguratorService } from './tools/systemConfigurator/services/systemConfigurator.service';
import { SplitSystemConfiguratorService } from './tools/splitSystemConfigurator/services/splitSystemConfigurator.service';
import { ToolsService } from './tools/tools.service';

import { WebConfigService } from './shared/services/webconfig.service';
import { OrderService } from './order/services/order.service';

//import { provideRouter, RouterConfig } from '@angular/router';

@Component({
    selector: 'my-app',

    //styleUrls: [
    //    // load the default theme (use correct path to node_modules)
    //    'node_modules/@progress/kendo-theme-default/dist/all.css'
    //],

    //styles: ['/deep/ #testdiv { color: red;}'],

    //styleUrls: [
    //    'app/content/test.css'

    templateUrl: 'app/app.component.html',
    //directives: [ROUTER_DIRECTIVES, HeaderButtonsComponent, ProjectsComponent],
    providers: [
        ToastrService,
        LoadingIconService,
        UserService,
        PasswordService,
        SystemAccessEnum,
        Enums,
        AccountService,
        BusinessService,
        UserResolver,
        ProjectService,
        ProductService,
        OrderService,
        SystemConfiguratorService,
        SplitSystemConfiguratorService,
        ToolsService,
        BasketService,
        WebConfigService
    ]
})

export class AppComponent implements OnInit {
    pageTitle: string = 'Daikin Project Office';

    public isAuthenticated = false;
    public currentUser: any;
    public webconfig: any;

    public loading = true;

    constructor(private router: Router, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService, private userSvc: UserService,
        private systemAccessEnum: SystemAccessEnum, private productSvc: ProductService,
        private systemConfiguratorSvc: SystemConfiguratorService,
        private SplitSystemConfiguratorSvc: SplitSystemConfiguratorService,
        private webconfigSvc: WebConfigService, private passwordSvc: PasswordService
    ) {
        router.events.subscribe((event: RouterEvent) => {
            this.navigationInterceptor(event)
        })
    }

    ngOnInit() {

        //var hash = window.location.hash;
        //this.userSvc.isAuthenticated().then(this.isAuthenticatedCallBack.bind(this));

        this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));


    }

    ngAfterViewChecked() {

        //var routeUrl = this.router.url;

        //this.setupActiveTab();

    }

    

    

    public getCurrentUserCallback(resp: any) {
        if (resp.isok) {

            this.isAuthenticated = true;
            this.userSvc.userIsAuthenticated = true;

            this.currentUser = resp.model;
            this.userSvc.currentUser = resp.model;
                        

            //this.setupActiveTab();


            for (let message of resp.messages.items) {
                if (message.type == 40) {// success
                    this.toastrSvc.Success(message.text);
                }
            }
        } else {
            this.isAuthenticated = false;
            this.userSvc.userIsAuthenticated = false;

            //resp is null
            //for (let message of resp.messages.items) {
            //    if (message.type == 10) {// error
            //        this.toastrSvc.Error(message.text);
            //    }
            //}
        }

    }



    //public setupActiveTab() {
    //    var hash = window.location.hash;

    //    if (hash == "#/overview") {
    //        jQuery("#overviewTab").addClass('selected').siblings().removeClass('selected');
    //    } else if ((hash == "#/projects") || hash.includes("#/project/") || hash.includes("#/projectEdit/") || hash.includes("#/projectCreate") ) {
    //        jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash.includes("#/quote/") || hash.includes("#/quoteCreate/") || hash.includes("#/quoteEdit/")) {
    //        jQuery("#projectsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash.includes("#/tools")) {
    //        jQuery("#toolsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash == "#/products") {
    //        jQuery("#productsTab").addClass('selected').siblings().removeClass('selected');
    //    } else if (hash == "#/orders") {
    //        jQuery("#ordersTab").addClass('selected').siblings().removeClass('selected');
    //    }

    //    jQuery("#nav-bar li").click(function () {
    //        jQuery(this).addClass('selected').siblings().removeClass('selected');
    //    });
    //}

    

    navigationInterceptor(event: RouterEvent): void {
        if (event instanceof NavigationStart) {
            this.loading = true;
            this.loadingIconSvc.Start(jQuery("#content"));
        }
        if (event instanceof NavigationEnd) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }

        // Set loading state to false in both of the below events to hide the spinner in case a request fails
        if (event instanceof NavigationCancel) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }
        if (event instanceof NavigationError) {
            this.loading = false;
            this.loadingIconSvc.Stop(jQuery("#content"));
        }
    }


};
