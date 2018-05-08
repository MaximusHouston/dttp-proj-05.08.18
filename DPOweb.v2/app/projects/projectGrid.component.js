"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var http_2 = require("@angular/http");
var toastr_service_1 = require("../shared/services/toastr.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var project_service_1 = require("./services/project.service");
var ProjectGridComponent = /** @class */ (function () {
    function ProjectGridComponent(toastrSvc, userSvc, systemAccessEnum, http, projectSvc) {
        this.toastrSvc = toastrSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.projectSvc = projectSvc;
        this.isAuthenticated = false;
        this.canEditProject = false;
        this.canTransferProject = false;
        this.canUnDeleteProject = false;
        this.canViewPipelineData = false;
        this.canEditPipelineData = false;
        this.deleteProjects = [];
        this.gridColumns = [];
        this.showDeletedProjects = false;
    }
    ProjectGridComponent.prototype.ngOnInit = function () {
        this.setGridHeight();
        var currentDate = new Date();
        this.currentDateString = currentDate.toISOString();
        var currentYear = currentDate.getFullYear();
        var lastYear = currentYear - 1;
        var thisDateLastYear = new Date();
        thisDateLastYear.setFullYear(lastYear);
        //if (thisDateLastYear.getDate() == 29) { // not necessarry since Feb 29th will be converted to Mar 1st automatically if the year is not leap year
        //    if (!this.isLeapYear(lastYear)) {
        //        thisDateLastYear.setDate(28);
        //    }
        //}
        this.thisDateLastYearString = thisDateLastYear.toISOString();
        this.userSvc.getCurrentUser()
            .then(this.getCurrentUserCallback.bind(this));
        this.gridColumns = this.setupGridColumns();
        this.ProjectsDataSource = this.getDataSource();
        this.setupSearchBox();
    };
    ProjectGridComponent.prototype.ngAfterContentInit = function () {
    };
    ProjectGridComponent.prototype.ngAfterViewInit = function () {
        this.setupActionsMenu();
        this.setupAlertTooltip();
        //setTimeout(this.removeKIconText, 1000); /*fix jquery kendo grid*/
    };
    ProjectGridComponent.prototype.removeKIconText = function () {
        $(".k-icon").text("");
        $(".k-i-refresh").text("");
    };
    ProjectGridComponent.prototype.isLeapYear = function (year) {
        return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
    };
    ProjectGridComponent.prototype.setGridHeight = function () {
        var daikinHeaderH = jQuery("#daikin-header").height();
        var projectTabsH = jQuery("#projectTabs").height();
        var tabHeaderTitleH = jQuery("div.tab-header").height() + 10 + 10; // margin top & bottom is 10px;
        var gridButtonBarH = jQuery("#projectGridButtonBar").height() + 10; //// margin bottom is 10px;
        var windowH = jQuery(window).height();
        this.gridHeight = windowH - daikinHeaderH - projectTabsH - tabHeaderTitleH - gridButtonBarH - 20;
        if (windowH < 750) {
            this.gridHeight = 550;
        }
    };
    ProjectGridComponent.prototype.getCurrentUserCallback = function (resp) {
        if (resp.isok) {
            this.isAuthenticated = true;
            this.currentUser = resp.model;
            this.canEditProject = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("EditProject"));
            this.canTransferProject = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("TransferProject"));
            this.canUnDeleteProject = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("UndeleteProject"));
            this.canViewPipelineData = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("ViewPipelineData"));
            this.canEditPipelineData = this.userSvc.hasAccess(this.currentUser, this.systemAccessEnum.getSystemAccessValueByName("EditPipelineData"));
            //if (this.canUnDeleteProject && this.canEditProject) {
            //    var deletedProjectColumn = {
            //        field: "deleted",
            //        title: "Deleted",
            //        hidden: true,
            //        filterable: false
            //    };
            //    this.gridColumns.push(deletedProjectColumn);
            //}
            if (this.canEditProject) {
                var deletedProjectColumn = {
                    title: "Delete Project",
                    headerTemplate: "<span class='k-font-icon k-i-trash'></span>",
                    width: "40px",
                    //template: kendo.template("<input type='checkbox' onclick='this.onDeleteProjectCheck(#:projectIdStr#, #:name#)'>"),
                    template: kendo.template("#=this.displayDeleteProjectCheckBox(projectIdStr, name, deleted)#").bind(this),
                    filterable: false,
                    sortable: false,
                    hidden: true
                };
                this.gridColumns.splice(1, 0, deletedProjectColumn);
            }
            if (this.canViewPipelineData) {
                var pipelineStatusColumn = {
                    field: "projectLeadStatusId",
                    title: "Pipeline Status",
                    editor: this.projectPipelineStatusDropDownEditor.bind(this),
                    template: kendo.template("#=this.getprojectPipelineStatus(projectLeadStatusId, projectStatusId, isTransferred, deleted)#").bind(this),
                    filterable: {
                        extra: false,
                        operators: {
                            string: {
                                eq: "Is equal to",
                            }
                        },
                        ui: this.projectPipelineStatusFilter.bind(this)
                    },
                    hidden: true
                };
                this.gridColumns.push(pipelineStatusColumn);
            }
            if (this.currentUser.showPrices) {
                var totalListColumn = {
                    field: "totalList",
                    title: "Total List",
                    format: "{0:c}",
                    hidden: true,
                    width: "13%",
                };
                this.gridColumns.push(totalListColumn);
                var totalSellColumn = {
                    field: "totalSell",
                    title: "Total Sell",
                    format: "{0:c}",
                    hidden: true,
                    width: "13%",
                };
                this.gridColumns.push(totalSellColumn);
                var totalNetColumn = {
                    field: "totalNet",
                    title: "Total Net",
                    format: "{0:c}",
                    hidden: true,
                    width: "13%",
                };
                this.gridColumns.push(totalNetColumn);
                var darComStatusColumn = {
                    field: "darComStatus",
                    title: "Dar/Com Status",
                    hidden: true,
                    filterable: false
                };
                this.gridColumns.push(darComStatusColumn);
                var pricingStrategyColumn = {
                    field: "pricingStrategy",
                    title: "Pricing Strategy",
                    hidden: true,
                    filterable: {
                        extra: false,
                        operators: {
                            string: {
                                eq: "Is equal to",
                            }
                        },
                        ui: this.pricingStrategyFilter.bind(this)
                    }
                };
                this.gridColumns.push(pricingStrategyColumn);
            }
            this.setupGrid();
            //this.LoadGridSettings();
        }
        else {
            window.location.href = "/v2/#/account/login";
        }
    };
    ProjectGridComponent.prototype.getDataSource = function () {
        var self = this;
        var projectsDataSrc = new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/Project/GetProjects",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json",
                    cache: true
                },
                update: {
                    url: "/api/Project/EditProjects",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json"
                },
                parameterMap: function (data, operation) {
                    if (operation !== "read" && data.models) {
                        self.ProjectsGridViewModel.items = data.models;
                        return kendo.stringify(self.ProjectsGridViewModel);
                    }
                    else if (operation == "read") {
                        var queryInfo = {
                            take: data.take,
                            skip: data.skip,
                            page: data.page,
                            pageSize: data.pageSize,
                            sort: data.sort,
                            filter: data.filter,
                            showDeletedProjects: self.showDeletedProjects
                        };
                        return JSON.stringify(queryInfo);
                    }
                }
            },
            sync: function (e) {
                self.reloadGrid();
            },
            change: function (e) {
                //hide grid tool bar when there is no unsaved changes
                if (self.dataSourceIsChanged(this)) {
                    jQuery("#project-grid .k-grid-toolbar").show();
                }
                else {
                    jQuery("#project-grid .k-grid-toolbar").hide();
                }
                self.resizeGrid();
            },
            //requestEnd: function (e:any) {
            //    //check the "response" argument to skip the local operations
            //    if (e.type === "update" && e.response) {
            //        console.log("Current request is 'update'.");
            //    }
            //},
            batch: true,
            sort: ({ field: "projectDate", dir: "desc" }),
            //get projects within last 12 months by default
            filter: {
                filters: [
                    { field: "projectStatusId", operator: "eq", value: "1" }
                    //,
                    //{
                    //    filters: [{ field: "projectDate", operator: "gte", value: self.thisDateLastYearString },
                    //        { field: "projectDate", operator: "lte", value: self.currentDateString }],
                    //    logic: "and"
                    //}
                    //,{ field: "alert", operator: "eq", value: "true" }
                ],
                logic: "and"
            },
            schema: {
                //errors: function (response: any) {
                //    if (response) {
                //        debugger;
                //        console.log(response);
                //        return response;
                //    }
                //    return false;
                //}, 
                data: function (response) {
                    self.displayProjectResponseMessages(response);
                    self.ProjectsGridViewModel = response.model;
                    return response.model.items;
                },
                model: {
                    id: 'projectId',
                    fields: {
                        projectId: { type: 'string', editable: false },
                        projectIdStr: { type: 'string', editable: false },
                        name: { type: 'string', editable: false },
                        alert: { type: 'boolean', editable: false },
                        businessName: { type: 'string', editable: false },
                        projectOwner: { type: 'string', editable: false },
                        customerName: { type: 'string', editable: false },
                        projectType: { type: 'string' },
                        projectStatus: { type: 'string' },
                        projectOpenStatus: { type: 'string' },
                        projectLeadStatus: { type: 'string', editable: false },
                        projectDate: { type: 'date', editable: false },
                        bidDate: { type: 'date' },
                        estimatedClose: { type: 'date' },
                        estimatedDelivery: { type: 'date' },
                        //Quote Info
                        activeQuoteTitle: { type: 'string', editable: false },
                        activeQuoteId: { type: 'string', editable: false },
                        totalList: { type: 'number', editable: false },
                        totalSell: { type: 'number', editable: false },
                        totalNet: { type: 'number', editable: false },
                        totalCountVRVOutDoor: { type: 'number', editable: false },
                        totalCountSplitOutDoor: { type: 'number', editable: false },
                        darComStatus: { type: 'string', editable: false },
                        isCommission: { type: 'boolean', editable: false },
                        pricingStrategy: { type: 'string', editable: false },
                        deleted: { type: 'boolean', editable: false }
                    }
                },
                total: "model.totalRecords"
            },
            pageSize: 50,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
        });
        projectsDataSrc = this.applyFiltersFromUrl(projectsDataSrc);
        return projectsDataSrc;
    }; // end of getDataSource
    ProjectGridComponent.prototype.applyFiltersFromUrl = function (dataSource) {
        var url = window.location.href;
        var urlQueryString = url.split("?")[1];
        if (urlQueryString != undefined) {
            var urlParams = new http_2.URLSearchParams(urlQueryString, new http_2.QueryEncoder());
            //debugger
            //var params = urlParams.paramsMap.entries();
            //for (var i = 0; i < params.length; i++){
            //    //dataSource._filter.filters.push({ field: params[i], operator: "eq", value: params[i].value });
            //}
            if (urlParams.has("alert")) {
                dataSource._filter.filters.push({ field: "alert", operator: "eq", value: urlParams.get("alert") });
            }
            if (urlParams.has("expirationDays")) {
                dataSource._filter.filters.push({ field: "expirationDays", operator: "eq", value: urlParams.get("expirationDays") });
            }
        }
        return dataSource;
    };
    ProjectGridComponent.prototype.dataSourceIsChanged = function (dataSrc) {
        var dirty = false;
        var self = this;
        self.hasUnsavedChanges = false;
        $.each(dataSrc._data, function () {
            if (this.dirty == true) {
                dirty = true;
                self.hasUnsavedChanges = true;
            }
        });
        if (dataSrc._destroyed.length > 0) {
            dirty = true;
            self.hasUnsavedChanges = true;
        }
        return dirty;
    };
    ProjectGridComponent.prototype.setupGrid = function () {
        var self = this;
        this.projectsGrid = jQuery('#project-grid');
        this.projectsGrid.kendoGrid(self.CombinedGridSettings());
        var gridToolbar = jQuery(".k-grid-toolbar");
        if (!self.canEditProject) {
            gridToolbar.hide();
        }
        jQuery(window).resize(function () {
            self.resizeGrid();
        });
    }; // end of setupGrid()
    ProjectGridComponent.prototype.getBaseGridOptions = function () {
        var self = this;
        return {
            //autoBind: false,
            dataSource: this.ProjectsDataSource,
            height: self.gridHeight,
            columnMenu: true,
            scrollable: true,
            resizable: true,
            sortable: true,
            filterable: true,
            toolbar: ["save", "cancel"],
            editable: {
                confirmation: false
            },
            reorderable: true,
            pageable: {
                refresh: true,
                input: true,
                numeric: false
            },
            columns: this.gridColumns,
            columnReorder: function (e) {
                var gridoptions = this;
                setTimeout(function () {
                    //console.log(kendo.stringify(gridoptions.columns));
                    self.SaveGridSettings();
                }, 5);
            },
            columnResize: function (e) {
                self.SaveGridSettings();
            },
            columnShow: function (e) {
                self.SaveGridSettings();
            },
            columnHide: function (e) {
                self.SaveGridSettings();
            },
            //change: function (e: any) {
            //    alert("changed");
            //    if (e.values.name !== e.model.name) {
            //        console.log("name is modified");
            //    }
            //},
            //saveChanges: function (e: any) {
            //    if (!confirm("Are you sure you want to save all changes?")) {
            //        e.preventDefault();
            //    }
            //    //bootbox.confirm("<p>Are you sure you want to save all changes?</p>", function (result) {
            //    //    if (result == false) {
            //    //        e.preventDefault();
            //    //    }
            //    //});
            //},
            dataBound: function (e) {
                self.resizeGrid();
                //Highlight deleted projects
                var items = this._data;
                var tableRows = jQuery(this.table).find("tr");
                tableRows.each(function (index) {
                    var row = jQuery(this);
                    var Item = items[index];
                    if (Item.deleted == true) {
                        row.addClass("projectDeleted");
                    }
                });
                jQuery("#project-grid th[data-field=totalList]").html("Total List<br>" + kendo.toString(self.ProjectsGridViewModel.totalList, "c"));
                jQuery("#project-grid th[data-field=totalNet]").html("Total Net<br> " + kendo.toString(self.ProjectsGridViewModel.totalNet, "c"));
                jQuery("#project-grid th[data-field=totalSell]").html("Total Sell<br>" + kendo.toString(self.ProjectsGridViewModel.totalSell, "c"));
                jQuery("#project-grid th[data-field=totalCountVRVOutDoor]").html("VRV ODU<br>#" + self.ProjectsGridViewModel.totalVRVOutdoorCount);
                jQuery("#project-grid th[data-field=totalCountSplitOutDoor]").html("Split ODU<br>#" + self.ProjectsGridViewModel.totalSplitCount);
                self.setupDeleteProjectCheckBox();
            }
        };
    };
    ProjectGridComponent.prototype.resizeGrid = function () {
        //jQuery("#project-grid").data("kendoGrid").resize();
        var self = this;
        var daikinHeaderH = jQuery("#daikin-header").height();
        var projectTabsH = jQuery("#projectTabs").height();
        var tabHeaderTitleH = jQuery("div.tab-header").height() + 10 + 10; // margin top & bottom is 10px;
        var gridButtonBarH = jQuery("#projectGridButtonBar").height() + 10; //// margin bottom is 10px;
        var windowH = jQuery(window).height();
        var gridElement = jQuery("#project-grid");
        var dataArea = gridElement.find(".k-grid-content"); // kendo grid row data area 
        var gridHeight = gridElement.innerHeight();
        var otherElements = gridElement.children().not(".k-grid-content"); // other Elements inside kendo grid. (ex: toolbar, header, pager ...)
        var otherElementsHeight = 0;
        otherElements.each(function () {
            var elem = jQuery(this);
            if (elem.hasClass("k-grid-toolbar") && !self.hasUnsavedChanges) {
                //continue;
            }
            else {
                var elemH = jQuery(this).outerHeight();
                otherElementsHeight += elemH;
            }
        });
        var gridElementHeight = windowH - daikinHeaderH - projectTabsH - tabHeaderTitleH - gridButtonBarH - 15;
        if (windowH < 750) {
            gridElementHeight = 550;
        }
        var dataAreaHeight = gridElementHeight - otherElementsHeight;
        gridElement.height(gridElementHeight);
        dataArea.height(dataAreaHeight);
    };
    ProjectGridComponent.prototype.setupGridColumns = function () {
        var columns = [
            {
                title: "Action",
                width: "40px",
                headerTemplate: "",
                template: "<img src='/Images/action-icon.png' class='actions actions-icon'  />",
                attributes: {
                    "class": "actions"
                }
            },
            //{
            //    title: "Delete Project",
            //    headerTemplate: "<span class='k-font-icon k-i-trash'></span>",
            //    width: "40px",
            //    //template: kendo.template("<input type='checkbox' onclick='this.onDeleteProjectCheck(#:projectIdStr#, #:name#)'>"),
            //    template: kendo.template("#=this.displayDeleteProjectCheckBox(projectIdStr, name, deleted)#").bind(this),
            //    //editable: false,
            //    filterable: false,
            //    sortable: false,
            //    hidden: true
            //},
            {
                field: "alert",
                title: "Alert",
                headerTemplate: "<span class='k-icon k-i-note'></span>",
                width: "60px",
                //template: "<span *ngIf='#:alert#' class='k-icon k-i-note'></span> <span>#=alertText#</span>",
                template: kendo.template("#=this.displayAlertIcon(alert, alertText, isTransferred)#").bind(this),
                filterable: true,
                sortable: false
            },
            {
                field: "name",
                title: "Project Name",
                //template: "<a href='/projectdashboard/Project/#=projectIdStr#'>#=name#</a>",
                template: "<a href='/v2/\\\\#/project/#=projectIdStr#'>#=name#</a>",
                width: "12%",
                filterable: false
            },
            {
                field: "projectIdStr",
                title: "Project Ref",
                hidden: true,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    }
                },
                sortable: false
            },
            {
                field: "activeQuoteTitle",
                title: "Active Quote",
                template: kendo.template("#=this.displayActiveQuote(activeQuoteTitle, activeQuoteId)#").bind(this),
                hidden: true,
                filterable: false
            },
            {
                field: "projectTypeId",
                title: "Project Type",
                editor: this.projectTypeDropDownEditor.bind(this),
                template: kendo.template("#=this.getProjectType(projectTypeId, projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    },
                    ui: this.projectTypeFilter.bind(this)
                }
            },
            {
                field: "projectStatusId",
                title: "Project Status",
                editor: this.projectStatusDropDownEditor.bind(this),
                template: kendo.template("#=this.getProjectStatus(projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    },
                    ui: this.projectStatusFilter.bind(this)
                }
            },
            {
                field: "projectOpenStatusId",
                title: "Project Open Status",
                editor: this.projectOpenStatusDropDownEditor.bind(this),
                template: kendo.template("#=this.getProjectOpenStatus(projectOpenStatusId, projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    },
                    ui: this.projectOpenStatusFilter.bind(this)
                }
            },
            {
                field: "bidDate",
                title: "Bid Date",
                format: "{0:MM-dd-yyyy}",
                editor: this.datePickerEditor.bind(this),
                template: kendo.template("#=this.displayDate(bidDate, projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    operators: {
                        date: {
                            eq: "Is equal to",
                            gte: "Is after or equal to",
                            lte: "Is before or equal to",
                        }
                    },
                }
            },
            {
                field: "estimatedClose",
                title: "Estimated Close",
                format: "{0:MM-dd-yyyy}",
                editor: this.datePickerEditor.bind(this),
                template: kendo.template("#=this.displayDate(estimatedClose, projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    operators: {
                        date: {
                            eq: "Is equal to",
                            gte: "Is after or equal to",
                            lte: "Is before or equal to",
                        }
                    },
                }
            },
            {
                field: "estimatedDelivery",
                title: "Estimated Delivery",
                format: "{0:MM-dd-yyyy}",
                editor: this.datePickerEditor.bind(this),
                template: kendo.template("#=this.displayDate(estimatedDelivery, projectStatusId, isTransferred, deleted)#").bind(this),
                filterable: {
                    operators: {
                        date: {
                            eq: "Is equal to",
                            gte: "Is after or equal to",
                            lte: "Is before or equal to",
                        }
                    },
                }
            },
            {
                field: "projectOwner",
                title: "Project Owner",
                hidden: true,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    },
                    ui: this.projectOwnerFilter.bind(this)
                },
            },
            {
                field: "businessName",
                title: "Business Name",
                hidden: true,
                filterable: {
                    extra: false,
                    operators: {
                        string: {
                            eq: "Is equal to",
                        }
                    },
                    ui: this.businessNameFilter.bind(this)
                }
            },
            {
                field: "customerName",
                title: "Dealer/Contractor",
                hidden: true,
                filterable: false
            },
            {
                field: "projectDate",
                title: "Project Date",
                format: "{0:MM-dd-yyyy}",
                hidden: true,
                filterable: {
                    operators: {
                        date: {
                            eq: "Is equal to",
                            gte: "Is after or equal to",
                            lte: "Is before or equal to",
                        }
                    },
                }
            },
            {
                field: "totalCountVRVOutDoor",
                title: "VRV ODU #",
                hidden: true,
            },
            {
                field: "totalCountSplitOutDoor",
                title: "Split ODU #",
                hidden: true,
            },
        ];
        return columns;
    };
    ProjectGridComponent.prototype.displayActiveQuote = function (activeQuoteTitle, activeQuoteId) {
        if (activeQuoteTitle != "No Active Quote") {
            //return "<a href='/Projectdashboard/QuoteItems?QuoteId=" + activeQuoteId + "'>" + activeQuoteTitle + "</a>";
            return "<a href='/v2/#/quote/" + activeQuoteId + "/existingRecord'>" + activeQuoteTitle + "</a>";
            //return "<a href='/v2/#/quoteItems/" + activeQuoteId + "/existingRecord'>" + activeQuoteTitle + "</a>";
        }
        else {
            return activeQuoteTitle;
        }
    };
    ProjectGridComponent.prototype.displayAlertIcon = function (alert, alertText, isTransferred) {
        if (isTransferred) {
            return "<span class='k-icon k-i-lock alert-icon'></span>";
        }
        else if (alert) {
            return "<span class='k-icon k-i-note alert-icon'></span>";
        }
        else {
            return "";
        }
    };
    ProjectGridComponent.prototype.setupAlertTooltip = function () {
        jQuery("#project-grid").kendoTooltip({
            //filter: "td:nth-child(2)", //this filter selects the second column's cells
            filter: "tbody tr td span.alert-icon",
            position: "right",
            content: function (e) {
                var dataItem = jQuery("#project-grid").data("kendoGrid").dataItem(e.target.closest("tr"));
                if (dataItem.isTransferred) {
                    return "Project has been transferred.";
                }
                else {
                    var content = dataItem.alertText;
                    return content;
                }
            }
        }).data("kendoTooltip");
    };
    ProjectGridComponent.prototype.setupActionsMenu = function () {
        var self = this;
        jQuery("#actionMenu").kendoContextMenu({
            open: this.onOpenActionMenu.bind(this),
            target: "#project-grid",
            filter: "tbody > tr > td.actions",
            showOn: "click",
            alignToAnchor: true,
            direction: "right",
            select: function (e) {
                self.onContextSelect(e);
            }
        });
    };
    ProjectGridComponent.prototype.onContextSelect = function (e) {
        var self = this;
        //var selectedRow = jQuery(e.target.closest("tr"));
        var selectedCommand = e.item.innerText.trim();
        var actionItem = jQuery(e.item);
        switch (selectedCommand) {
            case "Export Project":
                var actionItemSpan = actionItem.find("span.actionItem");
                this.selectedProjectId = actionItemSpan.attr("projectid");
                this.ProjectsGridViewModel.projectId = this.selectedProjectId;
                var data = {
                    "projectId": this.selectedProjectId,
                };
                this.projectSvc.exportProject(data);
                break;
            case "Edit Project":
                break;
            case "Transfer Project":
                var actionItemSpan = actionItem.find("span.actionItem");
                this.selectedProjectId = actionItemSpan.attr("projectid");
                jQuery("#transferProjectWindow").kendoWindow({
                    width: "500px",
                    title: "Transfer Project",
                    visible: false,
                    modal: true,
                    actions: [
                        "Close"
                    ],
                    position: {
                        top: 100,
                        left: "35%"
                    }
                }).data("kendoWindow").open();
                break;
            case "Delete Project":
                var actionItemSpan = actionItem.find("span.actionItem");
                this.selectedProjectId = actionItemSpan.attr("projectid");
                bootbox.confirm("<p>Are you sure you want to delete this project?</p>", function (result) {
                    if (result) {
                        self.projectSvc.deleteProject(self.selectedProjectId).then(function (resp) {
                            self.displayResponseMessages(resp);
                            var grid = jQuery("#project-grid").data("kendoGrid");
                            self.reloadGrid();
                            //remove deleted project from grid
                            //grid.removeRow(selectedRow);
                            //grid.cancelChanges();
                            //self.hasUnsavedChanges = false;
                        });
                    }
                });
                break;
        }
    };
    ProjectGridComponent.prototype.onOpenActionMenu = function (e) {
        //var self = this;
        var row = jQuery(e.target.parentElement);
        //var grid = this.target.data("kendoGrid");
        var grid = jQuery("#project-grid").data("kendoGrid");
        //var grid = e.target.data("kendoGrid");
        var item = grid.dataItem(row);
        var actionMenuItems = [{
                text: "<span class='actionItem' projectId ='" + item.projectIdStr + "' >Export Project</span>",
                encoded: false
            }
        ];
        if (this.canEditProject && !item.isTransferred && !item.deleted) {
            var editProjectOption = {
                text: "Edit Project",
                encoded: false,
                url: "/v2/#/projectEdit/" + item.projectIdStr
            };
            actionMenuItems.push(editProjectOption);
        }
        if (this.canTransferProject && !item.isTransferred && !item.deleted) {
            var transferProjectOption = {
                text: "<span class='actionItem' projectId ='" + item.projectIdStr + "' >Transfer Project</span>",
                encoded: false
            };
            actionMenuItems.push(transferProjectOption);
        }
        if (this.canEditProject && !item.isTransferred && !item.deleted) {
            var deleteProjectOption = {
                text: "<span class='actionItem' projectId ='" + item.projectIdStr + "' >Delete Project</span>",
                encoded: false
            };
            actionMenuItems.push(deleteProjectOption);
        }
        e.sender.setOptions({
            dataSource: actionMenuItems
        });
    };
    ProjectGridComponent.prototype.setupDeleteProjectCheckBox = function () {
        var self = this;
        jQuery("input.deletePrjChkBox").click(function (event) {
            event.stopImmediatePropagation();
            var chkbox = jQuery(event.target);
            var projectId = chkbox.attr("projectid");
            var projectName = chkbox.attr("projectname");
            var project = {
                "projectId": projectId,
                "projectName": projectName
            };
            if (jQuery(this).is(':checked')) {
                self.deleteProjects.push(project);
            }
            else {
                self.removeFromDeleteProjects(projectId);
            }
        });
    };
    ProjectGridComponent.prototype.removeFromDeleteProjects = function (projectId) {
        var self = this;
        var index = self.findDeleteProjectIndex(projectId);
        self.deleteProjects.splice(index, 1);
    };
    ProjectGridComponent.prototype.findDeleteProjectIndex = function (projectId) {
        var self = this;
        var index = 0;
        for (var i = 0; i < self.deleteProjects.length; i++) {
            var item = self.deleteProjects[i];
            if (item.projectId == projectId) {
                return i;
            }
        }
        return null;
    };
    ProjectGridComponent.prototype.openDeleteProjectsWindow = function () {
        var deleteProjectsWindow = jQuery("#deleteProjectsWindow");
        deleteProjectsWindow.kendoWindow({
            width: "500px",
            title: "Delete Projects",
            visible: false,
            modal: true,
            actions: [
                "Close"
            ],
        }).data("kendoWindow").center().open();
    };
    ProjectGridComponent.prototype.emptyDeleteProjectsArray = function () {
        this.deleteProjects = [];
    };
    ProjectGridComponent.prototype.displayDeleteProjectCheckBox = function (projectIdStr, name, deleted) {
        if (deleted == false) {
            return "<input type='checkbox' class='deletePrjChkBox' projectid='" + projectIdStr + "' projectname='" + name + "'>";
        }
        else {
            return "";
        }
    };
    ProjectGridComponent.prototype.openExportProjectsWindow = function () {
        var exportProjectsWindow = jQuery("#exportProjectsWindow");
        exportProjectsWindow.kendoWindow({
            width: "400px",
            title: "Export Projects",
            visible: false,
            modal: true,
            actions: [
                "Close"
            ],
        }).data("kendoWindow").center().open();
    };
    ProjectGridComponent.prototype.setupSearchBox = function () {
        jQuery("#projectSearchBox").keyup(function (event) {
            if (event.keyCode == 13) {
                jQuery("#projectSearchBtn").click();
            }
        });
        jQuery("#projectSearchBtn").click(function () {
            var value = jQuery("#projectSearchBox")[0].value;
            if (value) {
                //clear all filters
                var filter = jQuery("#project-grid").data("kendoGrid").dataSource.filter();
                if (filter != undefined) {
                    jQuery("#project-grid").data("kendoGrid").dataSource.filter().filters = [];
                }
                //apply new filters
                jQuery("#project-grid").data("kendoGrid").dataSource.filter([{
                        "name": "search",
                        "logic": "or",
                        "filters": [
                            {
                                "field": "projectId",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "name",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "projectOwner",
                                "operator": "contains",
                                "value": value
                            },
                            {
                                "field": "businessName",
                                "operator": "contains",
                                "value": value
                            }
                        ]
                    }]);
            }
        });
    };
    ProjectGridComponent.prototype.clearFilters = function () {
        //jQuery("form.k-filter-menu button[type='reset']").trigger("click");
        var filter = jQuery("#project-grid").data("kendoGrid").dataSource.filter();
        if (filter != undefined) {
            jQuery("#project-grid").data("kendoGrid").dataSource.filter().filters = [];
            this.reloadGrid();
        }
    };
    ProjectGridComponent.prototype.reloadGrid = function () {
        var projectEditAllGridDtaSrc = jQuery('#project-grid').data('kendoGrid').dataSource;
        projectEditAllGridDtaSrc.read();
    };
    ProjectGridComponent.prototype.CloseTransferProjectPopup = function () {
        var transferProjectWindow = jQuery("#transferProjectWindow").data("kendoWindow");
        transferProjectWindow.close();
    };
    //Project Type
    //If Project Status is "Open" then show a dropdown list when user click on the editable cell
    ProjectGridComponent.prototype.projectTypeDropDownEditor = function (container, options) {
        if (options.model.projectStatusId == 1 && this.canEditProject && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDropDownList({
                //autoBind: false,
                dataSource: this.ProjectsGridViewModel.projectTypes.items,
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        else {
            var text = jQuery('<span>' + options.model.projectType + '</span>');
            text.appendTo(container);
        }
    };
    //If Project Status is "Open" then show Project Type as editable field
    ProjectGridComponent.prototype.getProjectType = function (projectTypeId, projectStatusId, isTransferred, deleted) {
        if (this.ProjectsGridViewModel != undefined) {
            var projectTypes = this.ProjectsGridViewModel.projectTypes.items;
            for (var _i = 0, projectTypes_1 = projectTypes; _i < projectTypes_1.length; _i++) {
                var projectType = projectTypes_1[_i];
                if (projectTypeId == projectType.value) {
                    if (projectStatusId == 1 && this.canEditProject && !isTransferred && !deleted) {
                        return "<span class='k-edit-cell k-dropdown-wrap k-state-default' style='overflow: hidden; text-overflow:ellipsis;' >" + projectType.text + "<span>";
                    }
                    else {
                        return projectType.text;
                    }
                }
            }
        }
        return "";
    };
    //Project Status
    //If Project Status is "Open" then show a dropdown list when user click on the editable cell
    ProjectGridComponent.prototype.projectStatusDropDownEditor = function (container, options) {
        if (options.model.projectStatusId == 1 && this.canEditProject && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDropDownList({
                dataSource: this.ProjectsGridViewModel.projectStatusTypes.items,
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        if ((options.model.projectStatusId == 3 || options.model.projectStatusId == 4) && this.canEditProject && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDropDownList({
                dataSource: [{ text: "Open", value: 1 }],
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        else {
            var text = jQuery('<span>' + options.model.projectStatus + '</span>');
            text.appendTo(container);
        }
    };
    //If Project Status is "Open" then show Project Status as editable field
    ProjectGridComponent.prototype.getProjectStatus = function (projectStatusId, isTransferred, deleted) {
        if (this.ProjectsGridViewModel != undefined) {
            var statusList = this.ProjectsGridViewModel.projectStatusTypes.items;
            for (var _i = 0, statusList_1 = statusList; _i < statusList_1.length; _i++) {
                var status_1 = statusList_1[_i];
                if (projectStatusId == status_1.value) {
                    if ((projectStatusId == 1 || projectStatusId == 3 || projectStatusId == 4) && this.canEditProject && !isTransferred && !deleted) {
                        return "<span class='k-edit-cell k-dropdown-wrap k-state-default' style='overflow: hidden; text-overflow:ellipsis;' >" + status_1.text + "<span>";
                    }
                    else {
                        return status_1.text;
                    }
                    //return status.text;
                }
            }
        }
        return "";
    };
    //Project Open Status
    //If Project Status is "Open" then show a dropdown list when user click on the editable cell
    ProjectGridComponent.prototype.projectOpenStatusDropDownEditor = function (container, options) {
        if (options.model.projectStatusId == 1 && this.canEditProject && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDropDownList({
                //autoBind: false,
                dataSource: this.ProjectsGridViewModel.projectOpenStatusTypes.items,
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        else {
            var text = jQuery('<span>' + options.model.projectOpenStatus + '</span>');
            text.appendTo(container);
        }
    };
    //If Project Status is "Open" then show Project Open Status as editable field
    ProjectGridComponent.prototype.getProjectOpenStatus = function (projectOpenStatusId, projectStatusId, isTransferred, deleted) {
        if (this.ProjectsGridViewModel != undefined) {
            var openStatusList = this.ProjectsGridViewModel.projectOpenStatusTypes.items;
            for (var _i = 0, openStatusList_1 = openStatusList; _i < openStatusList_1.length; _i++) {
                var status_2 = openStatusList_1[_i];
                if (projectOpenStatusId == status_2.value) {
                    if (projectStatusId == 1 && this.canEditProject && !isTransferred && !deleted) {
                        return "<span class='k-edit-cell k-dropdown-wrap k-state-default' style='overflow: hidden; text-overflow:ellipsis;' >" + status_2.text + "<span>";
                    }
                    else {
                        return status_2.text;
                    }
                }
            }
        }
        return "";
    };
    //projectPipelineStatusDropDownEditor
    ProjectGridComponent.prototype.projectPipelineStatusDropDownEditor = function (container, options) {
        if (options.model.projectStatusId == 1 && this.canEditProject && this.canEditPipelineData && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDropDownList({
                dataSource: [{ text: "Lead", value: 1 },
                    { text: "Opportunity", value: 2 },
                    { text: "Disqualified", value: 5 }],
                dataTextField: "text",
                dataValueField: "value"
            });
        }
        else {
            var text = jQuery('<span>' + options.model.projectLeadStatus + '</span>');
            text.appendTo(container);
        }
    };
    ProjectGridComponent.prototype.getprojectPipelineStatus = function (projectLeadStatusId, projectStatusId, isTransferred, deleted) {
        if (this.ProjectsGridViewModel != undefined) {
            var pipelineStatusList = this.ProjectsGridViewModel.projectLeadStatusTypes.items;
            for (var _i = 0, pipelineStatusList_1 = pipelineStatusList; _i < pipelineStatusList_1.length; _i++) {
                var status_3 = pipelineStatusList_1[_i];
                if (projectLeadStatusId == status_3.value) {
                    if (projectStatusId == 1 && this.canEditProject && this.canEditPipelineData && !isTransferred && !deleted) {
                        return "<span class='k-edit-cell k-dropdown-wrap k-state-default' style='overflow: hidden; text-overflow:ellipsis;' >" + status_3.text + "<span>";
                    }
                    else {
                        return status_3.text;
                    }
                }
            }
        }
        return "";
    };
    //Date Picker
    //If Project Status is "Open" then show a date picker when user click on the editable cell
    ProjectGridComponent.prototype.datePickerEditor = function (container, options) {
        if (options.model.projectStatusId == 1 && this.canEditProject && !options.model.isTransferred && !options.model.deleted) {
            jQuery('<input data-text-field="text" data-value-field="value" data-bind="value:' + options.field + '" />')
                .appendTo(container)
                .kendoDatePicker();
        }
        else {
            var text = jQuery('<span>' + options.model[options.field].toLocaleDateString('en-US') + '</span>');
            text.appendTo(container);
        }
    };
    //If Project Status is "Open" then show date as editable field
    ProjectGridComponent.prototype.displayDate = function (date, projectStatusId, isTransferred, deleted) {
        if (projectStatusId == 1 && this.canEditProject && !isTransferred && !deleted) {
            //return "<span class='k-picker-wrap k-state-default k-widget'  >" + date.toLocaleDateString('en-US') + " <span unselectable='on' class='k-icon k-i-calendar'>select</span> </span>  ";
            return "<span class='k-edit-cell k-picker-wrap k-state-default'  >" + date.toLocaleDateString('en-US') + " </span> ";
        }
        else {
            return date.toLocaleDateString('en-US');
        }
    };
    //Filters
    ProjectGridComponent.prototype.projectTypeFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.projectTypes.items,
            dataTextField: "text",
            dataValueField: "value"
        });
    };
    ProjectGridComponent.prototype.projectStatusFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.projectStatusTypes.items,
            dataTextField: "text",
            dataValueField: "value"
        });
    };
    ProjectGridComponent.prototype.projectOpenStatusFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.projectOpenStatusTypes.items,
            dataTextField: "text",
            dataValueField: "value"
        });
    };
    ProjectGridComponent.prototype.projectPipelineStatusFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.projectLeadStatusTypes.items,
            dataTextField: "text",
            dataValueField: "value"
        });
    };
    ProjectGridComponent.prototype.businessNameFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.businessesInGroup.items,
            dataTextField: "text",
            dataValueField: "text"
        });
    };
    //projectOwnerFilter
    ProjectGridComponent.prototype.projectOwnerFilter = function (element) {
        element.kendoDropDownList({
            dataSource: this.ProjectsGridViewModel.usersInGroup.items,
            dataTextField: "text",
            dataValueField: "text"
        });
    };
    ProjectGridComponent.prototype.pricingStrategyFilter = function (element) {
        element.kendoDropDownList({
            dataSource: [{ text: "Commission", value: "Commission" },
                { text: "Buy/Sell", value: "Buy/Sell" }],
            dataTextField: "text",
            dataValueField: "value"
        });
    };
    //showDeletedProjectsFilter
    ProjectGridComponent.prototype.showDeletedProjectsFilter = function (element) {
        var self = this;
        element.kendoDropDownList({
            dataSource: [{ text: "Yes", value: true },
                { text: "No", value: false }],
            dataTextField: "text",
            dataValueField: "value",
            select: function (e) {
                var item = e.item;
                var text = item.text();
                if (text == "Yes") {
                    self.showDeletedProjects = true;
                }
                else if (text == "No") {
                    self.showDeletedProjects = false;
                }
            }
        });
    };
    //Export To Excel
    ProjectGridComponent.prototype.exportToExcel = function () {
        alert("export to excel");
    };
    ProjectGridComponent.prototype.displayProjectResponseMessages = function (response) {
        for (var _i = 0, _a = response.model.items; _i < _a.length; _i++) {
            var project = _a[_i];
            if (project.messages != null) {
                for (var _b = 0, _c = project.messages.items; _b < _c.length; _b++) {
                    var message = _c[_b];
                    if (message.type == 40) {
                        this.toastrSvc.Success(message.text);
                    }
                    else if (message.type == 10) {
                        this.toastrSvc.Error(message.text);
                    }
                }
            }
        }
        if (!response.isok) {
            for (var _d = 0, _e = response.messages.items; _d < _e.length; _d++) {
                var message = _e[_d];
                this.toastrSvc.Error(message.text);
            }
        }
    };
    ProjectGridComponent.prototype.displayResponseMessages = function (response) {
        for (var _i = 0, _a = response.messages.items; _i < _a.length; _i++) {
            var message = _a[_i];
            if (message.type == 40) {
                this.toastrSvc.Success(message.text);
            }
            else if (message.type == 10) {
                this.toastrSvc.Error(message.text);
            }
        }
    };
    ProjectGridComponent.prototype.SaveGridSettings = function () {
        var grid = jQuery("#project-grid").data("kendoGrid");
        var gridsettings = kendo.stringify(grid.getOptions());
        localStorage["daikin-project-grid-settings"] = gridsettings;
    };
    ProjectGridComponent.prototype.LoadGridSettings = function () {
        var grid = jQuery("#project-grid").data("kendoGrid");
        var baseSettings = this.getBaseGridOptions();
        var savedSettings = JSON.parse(localStorage["daikin-project-grid-settings"]);
        //var gridSettings = jQuery.extend(true, baseSettings, savedSettings);
        var gridSettings = this.MergeGridSettings(baseSettings, savedSettings);
        grid.setOptions(gridSettings);
    };
    ProjectGridComponent.prototype.MergeGridSettings = function (baseSettings, savedSettings) {
        var columns = [];
        for (var i = 0; i < savedSettings.columns.length; i++) {
            for (var j = 0; j < baseSettings.columns.length; j++) {
                if (savedSettings.columns[i].title == baseSettings.columns[j].title) {
                    //if column title match then merge savedSetting column into baseSetting column
                    var col = jQuery.extend(true, baseSettings.columns[j], savedSettings.columns[i]);
                    columns.push(col);
                }
            }
        }
        var newGridSettings = baseSettings;
        newGridSettings.columns = columns;
        return newGridSettings;
    };
    ProjectGridComponent.prototype.CombinedGridSettings = function () {
        var baseSettings = this.getBaseGridOptions();
        //return baseSettings;
        if (localStorage.getItem("daikin-project-grid-settings") === null) {
            return baseSettings;
        }
        else {
            var savedSettings = JSON.parse(localStorage["daikin-project-grid-settings"]);
            var gridSettings = this.MergeGridSettings(baseSettings, savedSettings);
            return gridSettings;
        }
    };
    ProjectGridComponent = __decorate([
        core_1.Component({
            selector: 'project-grid',
            //styles: ['/deep/ .k-dropdown-wrap {width: initial} /deep/ .k-picker-wrap{width: initial} /deep/ .k-dateinput-wrap{width: initial}'],
            templateUrl: 'app/projects/projectGrid.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, http_1.Http, project_service_1.ProjectService])
    ], ProjectGridComponent);
    return ProjectGridComponent;
}());
exports.ProjectGridComponent = ProjectGridComponent;
;
//# sourceMappingURL=projectGrid.component.js.map