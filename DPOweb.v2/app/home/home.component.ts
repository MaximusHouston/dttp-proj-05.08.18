import { Component, OnInit, Input, Output, EventEmitter, ViewChildren } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';

import 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { AccountService } from '../account/services/account.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';
declare var jQuery: any;

@Component({
    selector: 'home',
    templateUrl: 'app/home/home.component.html'

})
export class HomeComponent implements OnInit {
    public links: any = [];
    public user: any;

    public canViewProjects: boolean = false;
    public canEditProjects: boolean = false;
    public canViewOrders: boolean = false;

    public canViewUsers: boolean = false;
    public canManageGroups: boolean = false;
    public canViewBusiness: boolean = false;

    public canAccessLibrary: boolean = false;

    constructor(private router: Router, private route: ActivatedRoute,
        private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private systemAccessEnum: SystemAccessEnum,
        private enums: Enums, private http: Http) {

        this.user = this.route.snapshot.data['currentUser'].model;

        
    }

    ngOnInit() {
        //debugger
        //document.location.href = "/";
        this.preventDisplayMenuCloseOnClick();

        this.canViewProjects = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewProject);
        this.canEditProjects = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.EditProject);
        this.canViewOrders = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewOrder);

        this.canViewUsers = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewUsers);
        this.canManageGroups = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ManageGroups);
        this.canViewBusiness = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewBusiness);

        this.getLinks();
        this.accountSvc.resetBasketQuoteId();
    }

    getLinks() {
        this.links = [{
            text: "Library",
            image: "/v2/app/images/library.png",
            altText: "library",
            url: "/library",
            show: true
        }, {
            text: "Products",
            image: "/v2/app/images/products.png",
            altText: "Products",
            url: "/v2/#/products",
            show: true
        }, {
            text: "Tools",
            image: "/v2/app/images/tools.png",
            altText: "Home",
            url: "/v2/#/tools",
            show: true
        }, {
            text: "University",
            image: "/v2/app/images/university.png",
            altText: "Home",
            url: "/Training",
            show: true
        }, {
            text: "User Manual",
            image: "/v2/app/images/user_manual.png",
            altText: "Home",
            url: "/Content/pdf/DaikinCityUserGuide.pdf",
            show: true
        }]

        if (this.canViewProjects || this.canEditProjects) {
            var projectNode = {
                text: "Projects",
                image: "/v2/app/images/projects.png",
                altText: "Projects",
                url: "/v2/#/projects",
                show: true
            };
            this.links.splice(1, 0, projectNode);

            var overviewNode = {
                text: "Reports",
                image: "/v2/app/images/reporting.png",
                altText: "Reports",
                url: "/ProjectDashboard/Overview",
                show: true
            }

            this.links.splice(3, 0, overviewNode);
        }

        if (this.canViewOrders) {
            var orderNode = {
                text: "Orders",
                image: "/v2/app/images/orders.png",
                altText: "Orders",
                url: "/ProjectDashboard/Orders",
                show: true
            };
            this.links.splice(4, 0, orderNode);
        }

        if (this.canViewUsers || this.canViewBusiness || this.canManageGroups) {
            if (this.canViewUsers) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/users/",
                    show: true
                }
                this.links.push(managementNode);
            }else if (this.canViewBusiness) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/businesses/",
                    show: true
                }
                this.links.push(managementNode);
            } else if (this.canManageGroups) {
                var managementNode = {
                    text: "Management",
                    image: "/v2/app/images/management.png",
                    altText: "Management",
                    url: "/Userdashboard/groups/",
                    show: true
                }
                this.links.push(managementNode);
            }
           
        }
    }

    //browseProducts() {
    //    this.accountSvc.resetBasketQuoteId()
    //        .then((resp: any) => {
    //            if (resp.isok) {
    //                this.router.navigateByUrl("/products");
    //            }
    //        }).catch(error => {
    //            console.log(error);
    //        });
        
    //}

    preventDisplayMenuCloseOnClick() {
        $('.dropdown-menu').on('click', function (e) {
            if ($(this).hasClass('display-menu')) {
                e.stopPropagation();
            }
        });
    }
};

