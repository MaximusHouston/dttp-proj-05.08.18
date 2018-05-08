
import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { ToastrService } from '../services/toastr.service';
import { UserService } from '../services/user.service';
import { WebConfigService } from '../services/webconfig.service';
import { SystemAccessEnum } from '../services/systemAccessEnum';

declare var jQuery: any;

@Component({
    selector: 'header-buttons',
    templateUrl: "app/shared/header/headerButtons.component.html",
    styleUrls: ["app/shared/header/headerButtons.component.css"],
    providers: [ToastrService, UserService, SystemAccessEnum]
})
export class HeaderButtonsComponent implements OnInit {

    @Input() isAuthenticated: any;
    public currentUser: any;

    public webconfig: any;
    public libraryLink: any;

    constructor(private toastrSvc: ToastrService, private userSvc: UserService, private systemAccessEnum: SystemAccessEnum,
        private webconfigSvc: WebConfigService) {
    }

    public canViewUsers: any;
    public canManageGroups: any;
    public canViewBusiness: any;

    public canAccessLibrary: any;
    public canAccessLogistic: any;



    ngOnChanges() {

        //was working ok, but maybe slow
        //this.userSvc.getCurrentUser()
        //    .then(this.getCurrentUserCallback.bind(this));

        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
        }
    }
    ngOnInit() {

        //this.userSvc.isAuthenticated().then(this.isAuthenticatedCallBack.bind(this));

        //was working ok, but maybe slow
        //this.userSvc.getCurrentUser()
        //    .then(this.getCurrentUserCallback.bind(this));

        var self = this;

        if (this.userSvc.currentUser == undefined) {
            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));
        }

        this.webconfigSvc.getWebConfig().then((resp: any) => {
            self.webconfig = resp;
            self.libraryLink = self.webconfig.routeConfig.libraryLink;
        }).catch(error => {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });

    }

    //public isAuthenticatedCallBack(resp: any) {
    //    if (resp.isok) {
    //        if (resp.model == true) {
    //            //setup user header menu
    //            this.isAuthenticated = true;

    //            this.userSvc.getCurrentUser().then(this.getCurrentUserCallback.bind(this));





    public isAuthenticatedCallBack(resp: any) {
        if (resp.isok) {
            if (resp.model == true) {
                //setup user header menu
                this.isAuthenticated = true;

                //        } else {
                //            //Go back to login page
                //            //window.location.href = "/Account/Login";

            }
        }
    }


    public getCurrentUserCallback(resp: any) {
        if (resp.isok) {
            this.currentUser = resp.model;
            this.userSvc.currentUser = resp.model;

            var userNameElem = jQuery("#loggedin-username");
            userNameElem.text(this.currentUser.displayName);


            var projectOfficeBtn = jQuery("#projectOfficeBtn");
            projectOfficeBtn.attr("href", this.currentUser.defaultPageUrl);

            var canViewUsers = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ViewUsers"));
            var canManageGroups = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ManageGroups"));
            var canViewBusiness = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ViewBusiness"));




            var userOptsDropdownElem = jQuery("#userOptionsDropdown");


            if (canManageGroups || canViewBusiness || canViewUsers) {
                var management_li = "";
                if (canManageGroups) {
                    management_li = '<li><a href="/Userdashboard/groups/">MANAGEMENT</a></li>';
                }
                if (canViewBusiness) {
                    management_li = '<li><a href="/Userdashboard/businesses/">MANAGEMENT</a></li>';
                }
                if (canViewUsers) {
                    management_li = '<li><a href="/Userdashboard/users/">MANAGEMENT</a></li>';
                }
                //userOptsDropdownElem.append(management_li);
                $("#hidden-management-li").replaceWith(management_li);
            }


            if (this.currentUser.hasAccessToCMS) {
                var CMS_li = "";
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementTools"))) {
                    CMS_li = '<li><a href="/CityCMS/tools/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementProductFamilies"))) {
                    CMS_li = '<li><a href="/CityCMS/productfamilies/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementCommsCenter"))) {
                    CMS_li = '<li><a href="/CityCMS/communicationscenter/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementLibrary"))) {
                    CMS_li = '<li><a href="/CityCMS/library/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementApplicationProducts"))) {
                    CMS_li = '<li><a href="/CityCMS/applicationproducts/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementApplicationBuildings"))) {
                    CMS_li = '<li><a href="/CityCMS/applicationbuildings/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementFunctionalBuildings"))) {
                    CMS_li = '<li><a href="/CityCMS/functionalbuildings/">CONTENT MANAGEMENT</a></li>';
                }
                if (this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ContentManagementHomeScreen"))) {
                    CMS_li = '<li><a href="/CityCMS/homescreen/">CONTENT MANAGEMENT</a></li>';
                }

                //userOptsDropdownElem.append(CMS_li);
                $("#hidden-content-management-li").replaceWith(CMS_li);
            }

            var signOut_li = '<li><a href="/Account/Logoff">SIGN OUT</a></li>';
            //userOptsDropdownElem.append(signOut_li);


            var cityLocationsDropdownElem = jQuery("#cityLocationsDropdown");

            if (this.currentUser.cityAccesses.indexOf(1) > -1) {
                //var cityLibrary_li = '<li><a href="/#library"> <span class="glyphicon glyphicon-menu-right"></span> LIBRARY</a></li>';
                //cityLocationsDropdownElem.append(cityLibrary_li);
                this.canAccessLibrary = true;

            }
            if (this.currentUser.cityAccesses.indexOf(2) > -1) {
                //var logisticsCenter_li = '<li><a href="/#logisticscenter"> <span class="glyphicon glyphicon-menu-right"></span> LOGISTICS CENTER</a></li>';
                //cityLocationsDropdownElem.append(logisticsCenter_li);
                this.canAccessLogistic = true;
            }

            for (let message of resp.messages.items) {
                if (message.type == 40) {// success
                    this.toastrSvc.Success(message.text);
                }
            }
        } else if (resp.messages) {

            //window.location.href = "/Account/Login";

            for (let message of resp.messages.items) {
                if (message.type == 10) {// error
                    this.toastrSvc.Error(message.text);
                }
            }
        }

    }


};

