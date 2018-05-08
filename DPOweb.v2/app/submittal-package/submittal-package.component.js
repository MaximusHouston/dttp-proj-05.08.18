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
var enums_1 = require("../shared/enums/enums");
var project_service_1 = require("../projects/services/project.service");
var quote_service_1 = require("../quote/services/quote.service");
var submittal_package_service_1 = require("./services/submittal-package.service");
var SubmittalPackageComponent = /** @class */ (function () {
    function SubmittalPackageComponent(router, route, toastrSvc, loadingIconSvc, userSvc, systemAccessEnum, http, enums, submittalPackageSvc, projectSvc, quoteSvc, elRef) {
        this.router = router;
        this.route = route;
        this.toastrSvc = toastrSvc;
        this.loadingIconSvc = loadingIconSvc;
        this.userSvc = userSvc;
        this.systemAccessEnum = systemAccessEnum;
        this.http = http;
        this.enums = enums;
        this.submittalPackageSvc = submittalPackageSvc;
        this.projectSvc = projectSvc;
        this.quoteSvc = quoteSvc;
        this.elRef = elRef;
        //public gridView: GridDataResult;
        //<editor-fold desc="Description">
        //public submittalPackageModel: SubmittalPackageModel; 
        //</editor-fold>
        this.quoteItemsList = [];
        this.configuredItems = [];
        this.possibleDocsList = [
            { colId: 2, name: "submittalSheets", inputname: "chkSubmittalSheetsHeader", label: "Submittal Sheets" },
            { colId: 3, name: "installationManual", inputname: "chkInstallationManualHeader", label: "Installation Manual" },
            { colId: 4, name: "operationManual", inputname: "chkOperationManualHeader", label: "Operation Manual" },
            { colId: 5, name: "guideSpecs", inputname: "chkGuideSpecsHeader", label: "Guide Specs" },
            { colId: 6, name: "productBrochure", inputname: "chkProductBrochureHeader", label: "Product Brochure" },
            { colId: 7, name: "revitDrawing", inputname: "chkRevitDrawingHeader", label: "Revit Drawing" },
            { colId: 8, name: "cadDrawing", inputname: "chkCadDrawingHeader", label: "CAD Drawing" },
            { colId: 9, name: "productFlyer", inputname: "chkProductFlyerHeader", label: "Product Flyer" },
        ];
        this.action = this.route.snapshot.url[0].path;
        this.user = this.route.snapshot.data['currentUser'].model;
        this.quote = this.route.snapshot.data['quoteModel'].model;
    }
    SubmittalPackageComponent.prototype.ngOnInit = function () {
        this.loadItems();
    };
    SubmittalPackageComponent.prototype.loadItems = function () {
        var _this = this;
        return this.submittalPackageSvc.getQuotePackage(this.quote.quoteId)
            .subscribe(function (data) {
            _this.quoteItemsList = data.items;
            _this.quoteItemsList.forEach(function (x, index) {
                //filter out configured items to bind to different table
                if (x.lineItemTypeId === 2) {
                    _this.configuredItems.push[index];
                    _this.quoteItemsList.splice(index, 1);
                }
            });
            //loop through again to set the values and ids for checkboxes
            _this.quoteItemsList.forEach(function (x, index) {
                x.documents.forEach(function (y, index) {
                    if (y.productId === x.productId && y.documentTypeId === 100000008) {
                        x.hasSubmittalSheets = true;
                        x.submittalSheetsDocObject = y;
                        if (x.submittalSheetsDocObject != null) {
                            x.submittalSheetsDocId = x.submittalSheetsDocObject.productId + "-" + x.submittalSheetsDocObject.documentTypeId;
                        }
                        x.documents.splice(index, 1);
                    }
                    if (y.productId === x.productId && y.documentTypeId === 100000002) {
                        x.hasInstallationManual = true;
                        x.installationManualDocObject = y;
                        if (x.installationManualDocObject != null) {
                            x.installationManualDocId = x.installationManualDocObject.productId + "-" + x.installationManualDocObject.documentTypeId;
                        }
                        x.documents.splice(index, 1);
                    }
                });
                //set values for checkboxes
                //if (x.documents.find((y, index) => (y.productId === x.productId && y.documentTypeId === 100000008))) {
                //    //let index = x.documents.findIndex(y => y.productId === x.productId && y.documentTypeId === 100000008); //find index in your array                        
                //    x.hasSubmittalSheets == true;
                //    x.submittalDocument = x.documents[index];
                //    x.documents.splice(index, 1);//remove element from array
                //}           
            });
        });
    };
    __decorate([
        core_1.ViewChild('chkSubmittal'),
        __metadata("design:type", core_1.ElementRef)
    ], SubmittalPackageComponent.prototype, "submittalCheckbox", void 0);
    SubmittalPackageComponent = __decorate([
        core_1.Component({
            selector: 'submittal-package',
            templateUrl: 'app/submittal-package/submittal-package.component.html',
            styleUrls: ['app/submittal-package/submittal-package.component.css']
        }),
        __metadata("design:paramtypes", [router_1.Router, router_1.ActivatedRoute,
            toastr_service_1.ToastrService, loadingIcon_service_1.LoadingIconService,
            user_service_1.UserService, systemAccessEnum_1.SystemAccessEnum,
            http_1.Http, enums_1.Enums, submittal_package_service_1.SubmittalPackageService,
            project_service_1.ProjectService, quote_service_1.QuoteService, core_1.ElementRef])
    ], SubmittalPackageComponent);
    return SubmittalPackageComponent;
}());
exports.SubmittalPackageComponent = SubmittalPackageComponent;
//# sourceMappingURL=submittal-package.component.js.map