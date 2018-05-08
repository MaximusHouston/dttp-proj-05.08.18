import { Component, ElementRef, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { ToastrService } from '../shared/services/toastr.service';
import { UserService } from '../shared/services/user.service';
import { ProjectGridComponent } from './projectGrid.component';
import { Enums } from '../shared/enums/enums';
declare var jQuery: any;

@Component({
    selector: 'projects',
    //styleUrls: [
    //     'app/content/kendo/kendo.common.min.css',
    //     'app/content/kendo/kendo.default.min.css',
    //     'app/content/kendo/kendo.default.mobile.min.css',
    //     'node_modules/bootstrap/dist/css/bootstrap.min.css'
    //],
    templateUrl: 'app/projects/projects.component.html',
    //directives: [ProjectsGridComponent, ProjectsEditAllGridComponent, ProjectsGridFilterComponent],
    providers: [ToastrService]
    
})
export class ProjectsComponent implements OnInit {
    elementRef: ElementRef;
    private toastrSvc: any;

    public user: any;
    public canViewProject: boolean = false;

    public projectsGridDataSource: any;
    public projectListModelData: any;

    //public editMode: boolean = false;

    constructor(elementRef: ElementRef, private router: Router, private route: ActivatedRoute,
        toastrService: ToastrService, private userSvc: UserService, private enums: Enums) {
        this.elementRef = elementRef;
        this.toastrSvc = toastrService;
        //this.projectsGridDataSource = this.getData();

        this.user = this.route.snapshot.data['currentUser'].model;
    }

    ngOnInit() {

        this.userSvc.isAuthenticated().then((resp: any) => {
            if (!resp.isok || resp.model != true) {
                //Go back to login page
                window.location.href = "/v2/#/account/login";

            }
        });

        this.canViewProject = this.userSvc.hasAccess(this.user, this.enums.SystemAccessEnum.ViewProject);

        //if (!this.userSvc.userIsAuthenticated) {
        //    window.location.href = "/Account/Login";
        //    //window.location.href = "/v2/#/account/login";
        //}

        this.projectsGridDataSource = this.getData();
    }
    ngAfterContentInit() {
        //setTimeout(this.removeKIconText, 1000); // wait 1 sec
    }

    //public removeKIconText() {
    //    $(".k-icon").text("");
    //    $(".k-i-refresh").text("");
    //}
        

    //public EditAll() {
    //    this.editMode = true;
    //}

    //public StopEditAll() {
    //    this.editMode = false;
    //}

    

    public getData() {

        var self = this;
        var projectsDataSource = new kendo.data.DataSource({
            //type: "json",
            transport: {
                read: {
                    url: "/api/Project/GetProjects",
                    dataType: "json",
                    type: "GET",
                    cache: true
                },
                update: {
                    url: "/api/Project/EditProjects",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json"
                },

               

                parameterMap: function (data, operation) {
                    //if (operation !== "read" && data.models) {// batch edit ( use "data" for single record edit )
                    //    return { models: kendo.stringify(data.models) };
                    //}
                    if (operation !== "read" && data) {// single edit 
                        return kendo.stringify(data);
                    }
                    else if (operation == "read") {
                        var queryInfo = {
                            take: data.take,
                            skip: data.skip,
                            page: data.page,
                            pageSize: data.pageSize

                        };
                        return queryInfo;
                    }
                }

              
            },
            //batch: true,
            sort: ({ field: "projectId", dir: "desc" }),

            schema: {
                data: function (response : any) {

                    self.projectListModelData = response.model;
                    return response.model.items;
                    
                },
                model: {
                    fields: {
                        id: 'projectId',
                        projectId: { type: 'number', editable: false },
                        projectIdStr: { type: 'string', editable: false },
                        name: { type: 'string', editable: false },
                        projectOwner: { type: 'string', editable: false },
                        projectDate: { type: 'date' },
                        projectStatus: { type: 'string' },
                        projectOpenStatus: { type: 'string' },
                        projectType: { type: 'string' },
                        bidDate: { type: 'date' },
                        estimatedClose: { type: 'date' },
                        estimatedDelivery: { type: 'date' }
                      
                        //totalListPrice: { type: 'number' },
                        //totalNetPrice: { type: 'number' },
                        //totalSellPrice: { type: 'number' },
                        //darComStatus: { type: 'string' },
                        //vrvODUcount: { type: 'number' },
                        //splitODUcount: { type: 'number' },
                        //pricingTypeId: { type: 'number' },
                        //pricingTypeDescription: { type: 'string' },
                        //poAttachmentName: { type: 'string' },
                    }
                },
                total: "model.totalRecords"
            },
            pageSize: 50,
            serverPaging: true,
        });

        //projectsDataSource.read();
        return projectsDataSource;
       

        
    }// end of get Data

   
    

}