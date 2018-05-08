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
//import './rxjs-operators';
//import 'rxjs/add/operator/map';
//import 'rxjs/add/operator/catch';
//import 'rxjs/add/operator/toPromise';
require("rxjs/Rx");
var toastr_service_1 = require("../shared/services/toastr.service");
var project_service_1 = require("./services/project.service");
var TransferProjectPopupComponent = /** @class */ (function () {
    function TransferProjectPopupComponent(toastrSvc, http, projectSvc) {
        this.toastrSvc = toastrSvc;
        this.http = http;
        this.projectSvc = projectSvc;
        this.closeWindow = new core_1.EventEmitter();
        this.transferToEmail = "";
    }
    TransferProjectPopupComponent.prototype.ngOnInit = function () {
        //this.projectSvc
    };
    TransferProjectPopupComponent.prototype.closePopup = function () {
        this.closeWindow.emit({});
    };
    TransferProjectPopupComponent.prototype.transferProject = function () {
        var data = {
            "projectId": this.selectedProjectId,
            "email": this.transferToEmail
        };
        this.projectSvc.transferProject(data)
            .then(this.transferProjectCallback.bind(this));
        //reset email
        this.transferToEmail = "";
    };
    TransferProjectPopupComponent.prototype.transferProjectCallback = function (resp) {
        if (resp.IsOK) {
            for (var _i = 0, _a = resp.Messages.Items; _i < _a.length; _i++) {
                var message = _a[_i];
                if (message.Type == 40) {
                    this.toastrSvc.Success(message.Text);
                }
            }
            //reload projects grid
            var projectEditAllGridDtaSrc = jQuery('#project-grid').data('kendoGrid').dataSource;
            projectEditAllGridDtaSrc.read();
        }
        else {
            for (var _b = 0, _c = resp.Messages.Items; _b < _c.length; _b++) {
                var message = _c[_b];
                if (message.Type == 10) {
                    this.toastrSvc.Error(message.Text);
                }
            }
        }
        this.closePopup();
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], TransferProjectPopupComponent.prototype, "selectedProjectId", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", Object)
    ], TransferProjectPopupComponent.prototype, "closeWindow", void 0);
    TransferProjectPopupComponent = __decorate([
        core_1.Component({
            selector: 'transferProject-popup',
            templateUrl: 'app/projects/transferProjectPopup.component.html'
        }),
        __metadata("design:paramtypes", [toastr_service_1.ToastrService, http_1.Http, project_service_1.ProjectService])
    ], TransferProjectPopupComponent);
    return TransferProjectPopupComponent;
}());
exports.TransferProjectPopupComponent = TransferProjectPopupComponent;
;
//# sourceMappingURL=transferProjectPopup.component.js.map