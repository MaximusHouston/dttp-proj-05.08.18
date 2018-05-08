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
var submittal_package_1 = require("../submittal-package/models/submittal-package");
var submittal_package_service_1 = require("./services/submittal-package.service");
var ngx_progressbar_1 = require("ngx-progressbar");
var ng_block_ui_1 = require("ng-block-ui");
var modal_component_1 = require("../shared/modal/modal.component");
var DocumentAndFilesComponent = /** @class */ (function () {
    //public submittalPackageForm: FormGroup;     
    //private fb: FormBuilder,
    function DocumentAndFilesComponent(submittalService, ngProgress) {
        this.submittalService = submittalService;
        this.ngProgress = ngProgress;
    }
    DocumentAndFilesComponent.prototype.ngOnInit = function () {
        //this.submittalPackageForm = new FormGroup({
        //    referenceCheckbox: new FormControl(),
        //    submittalSheetsCheckbox: new FormControl(),
        //    operationManualCheckbox: new FormControl()
        //});
    };
    DocumentAndFilesComponent.prototype.checkAllColumns = function (event, rowId) {
        if (event.target.checked)
            jQuery("input[rowIndex='" + rowId + "']").prop('checked', true);
        else {
            jQuery("input[rowIndex='" + rowId + "']").prop('checked', false);
            jQuery("input[rowIndex='0']").prop('checked', false);
        }
    };
    DocumentAndFilesComponent.prototype.uncheckRowAndColumnHeaderCheckBox = function (rowId, colIdentifier) {
        if (colIdentifier == 'submittalSheets') {
            jQuery("input[name='chkReferenceRow'][rowIndex='" + rowId + "']").prop('checked', false);
            jQuery("input[name='chkSubmittalSheetsHeader']").prop('checked', false);
        }
    };
    DocumentAndFilesComponent.prototype.checkAllRows = function (event) {
        if (event.target.id == 'submittalSheets') {
            jQuery("input[name='chkSubmittalSheetsRow']").prop('checked', true);
            //this.quoteItemsList.forEach(x => x.isSubmittalSheets = event.target.checked);
        }
        else if (event.target.id == 'installationManual') {
            this.quoteItemsList.forEach(function (x) { return x.isInstallationManual = event.target.checked; });
        }
    };
    //checkAllCheckboxes(event: any) {
    //    jQuery("input[type=checkbox]:checked").prop('checked', true)
    //}
    DocumentAndFilesComponent.prototype.isAllRowsChecked = function (colIdentifier) {
        if (colIdentifier == 'submittalSheets')
            return this.quoteItemsList.every(function (_) { return _.isSubmittalSheets; });
        else if (colIdentifier == 'installationManual')
            return this.quoteItemsList.every(function (_) { return _.isInstallationManual; });
        //jQuery("#uploadFilesModal").modal({ backdrop: 'static', keyboard: false });
    };
    DocumentAndFilesComponent.prototype.isAllCheckboxesChecked = function () {
        console.log("all checked");
    };
    DocumentAndFilesComponent.prototype.openFromComponent = function () {
        this.componentInsideModal.open();
    };
    DocumentAndFilesComponent.prototype.handleEvent = function (filesArray) {
        this.files = filesArray;
    };
    DocumentAndFilesComponent.prototype.onSubmit = function () {
        var _this = this;
        this.blockUI.start('Loading...');
        this.ngProgress.start();
        var values = jQuery("input[type=checkbox]:checked").map(function () {
            return this.value;
        }).get();
        var submittalModel = new submittal_package_1.SubmittalPackageModel();
        submittalModel.quoteId = this.quoteId; //"724134023730921472";
        submittalModel.projectId = this.projectId; //"724133966570946560";
        var productsPlusDocsList = values.map(function (x) { return x.split('-'); });
        var mappedItems = [];
        productsPlusDocsList.forEach(function (value, key) {
            var productId = value[0];
            var documentId = value[1];
            mappedItems.push({ productId: productId, documentId: documentId });
        });
        submittalModel.productsAndDocuments = mappedItems;
        if (submittalModel.productsAndDocuments.length > 0) {
            this.submittalService.createQuotePackage(submittalModel)
                .subscribe(function (data) {
                console.log(data);
                _this.ngProgress.done();
                _this.blockUI.stop();
            }, function (err) {
            });
        }
    };
    __decorate([
        core_1.ViewChild('componentInsideModal'),
        __metadata("design:type", modal_component_1.ModalComponent)
    ], DocumentAndFilesComponent.prototype, "componentInsideModal", void 0);
    __decorate([
        ng_block_ui_1.BlockUI(),
        __metadata("design:type", Object)
    ], DocumentAndFilesComponent.prototype, "blockUI", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Array)
    ], DocumentAndFilesComponent.prototype, "quoteItemsList", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], DocumentAndFilesComponent.prototype, "possibleDocsList", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", String)
    ], DocumentAndFilesComponent.prototype, "quoteId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", String)
    ], DocumentAndFilesComponent.prototype, "projectId", void 0);
    DocumentAndFilesComponent = __decorate([
        core_1.Component({
            selector: 'documents-and-files',
            templateUrl: 'app/submittal-package/documents-and-files.component.html',
            styleUrls: ['app/submittal-package/documents-and-files.component.css'],
            encapsulation: core_1.ViewEncapsulation.None
        }),
        __metadata("design:paramtypes", [submittal_package_service_1.SubmittalPackageService,
            ngx_progressbar_1.NgProgress])
    ], DocumentAndFilesComponent);
    return DocumentAndFilesComponent;
}());
exports.DocumentAndFilesComponent = DocumentAndFilesComponent;
//# sourceMappingURL=document-and-files.component.js.map