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
var router_1 = require("@angular/router");
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
var loadingIcon_service_1 = require("../shared/services/loadingIcon.service");
var user_service_1 = require("../shared/services/user.service");
var systemAccessEnum_1 = require("../shared/services/systemAccessEnum");
var project_service_1 = require("../projects/services/project.service");
var quote_service_1 = require("../quote/services/quote.service");
var ProjectQuotesComponent = /** @class */ (function () {
    function ProjectQuotesComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, http, quoteSvc, projectSvc) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.quoteSvc = quoteSvc;
        this.projectSvc = projectSvc;
        this.reloadDataEvent = new core_1.EventEmitter();
        //private showActionWindow: boolean = false;
        this.actionOptions = [{}];
        this.defaultItem = { text: "Select ...", value: null };
    }
    ProjectQuotesComponent.prototype.ngOnInit = function () {
        //this.actionOptions = [{
        //    text: 'Export Quote',
        //    url: "/ProjectDashboard/QuotePrintExcel?projectId=" + this.project.projectId + "&quoteId=" + this.project.quoteId + "&withCostPrices=true"
        //}, {
        //    text: 'Edit Quote',
        //    url: "/v2/#/quoteEdit/" + this.project.projectId + "/" + this.project.quoteId
        //}, {
        //    text: 'Duplicate Quote',
        //    url: "/ProjectDashboard/QuoteDuplicate?projectId=" + this.project.projectId + "&quoteId=" + this.project.quoteId
        //}];
    };
    //public cancel() {
    //}
    //public submit() {
    //}
    //public actionWindowToggle(): void {
    //    this.showActionWindow = !this.showActionWindow;
    //}
    ProjectQuotesComponent.prototype.setQuoteActive = function (quote) {
        var self = this;
        var data = {
            "ProjectId ": this.project.projectId,
            "QuoteId": quote.quoteId
        };
        self.loadingIconSvc.Start(jQuery("#content"));
        this.quoteSvc.setQuoteActive(data).then(function (resp) {
            if (resp.isok) {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
                self.reloadData();
            }
            else {
                self.loadingIconSvc.Stop(jQuery("#content"));
                self.toastrSvc.displayResponseMessages(resp);
            }
        }).catch(function (error) {
            self.loadingIconSvc.Stop(jQuery("#content"));
            console.log(error);
        });
    };
    ProjectQuotesComponent.prototype.reloadData = function () {
        this.reloadDataEvent.emit();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectQuotesComponent.prototype, "project", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectQuotesComponent.prototype, "projectQuotes", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ProjectQuotesComponent.prototype, "user", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", core_1.EventEmitter)
    ], ProjectQuotesComponent.prototype, "reloadDataEvent", void 0);
    ProjectQuotesComponent = __decorate([
        core_1.Component({
            selector: 'project-quotes',
            styles: ['/deep/ .k-grid-content .k-button {margin: 0px} /deep/ .k-list .k-item:hover{background-color: white}'],
            templateUrl: 'app/project/projectQuotes.component.html'
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute, toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum, http_1.Http,
            quote_service_1.QuoteService, project_service_1.ProjectService])
    ], ProjectQuotesComponent);
    return ProjectQuotesComponent;
}());
exports.ProjectQuotesComponent = ProjectQuotesComponent;
;
//# sourceMappingURL=projectQuotes.component.js.map