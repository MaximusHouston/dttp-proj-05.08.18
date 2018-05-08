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
require("rxjs/Rx");
var upload_files_service_1 = require("./services/upload-files.service");
var UploadFilesComponent = /** @class */ (function () {
    // @HostBinding('style.background') private background = '#eee';
    function UploadFilesComponent(http, fileService) {
        this.http = http;
        this.fileService = fileService;
        this.fileList = [];
        this.errors = [];
        this.dragAreaClass = 'dragarea';
        this.projectId = 0;
        this.sectionId = 0;
        this.fileExt = "JPG, GIF, PNG, txt";
        this.maxFiles = 5;
        this.maxSize = 5; // 5MB
        this.uploadStatus = new core_1.EventEmitter();
        this.outgoingData = new core_1.EventEmitter();
        this.apiEndPoint = "";
    }
    UploadFilesComponent.prototype.ngOnInit = function () {
    };
    UploadFilesComponent.prototype.fileChange = function (event) {
        var fileList = event.target.files;
        if (fileList.length > 0) {
            var file = fileList[0];
            var formData = new FormData();
            formData.append('uploadFile', file, file.name);
            var headers = new http_1.Headers();
            /** In Angular 5, including the header Content-Type can invalidate your request */
            headers.append('Content-Type', 'multipart/form-data');
            headers.append('Accept', 'application/json');
            var options = new http_1.RequestOptions({ headers: headers });
            // this.http.post(`${this.apiEndPoint}`, formData, options)
            //     .map(res => res.json())
            //     .catch(error => Observable.throw(error))
            //     .subscribe(
            //         data => console.log('success'),
            //         error => console.log(error)
            //     )
        }
    };
    UploadFilesComponent.prototype.onFileChange = function (event) {
        var files = event.target.files;
        //this.saveFiles(files);
        this.fileList.push(files);
        //this.fileList = files;
    };
    UploadFilesComponent.prototype.onDragOver = function (event) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    };
    UploadFilesComponent.prototype.onDragEnter = function (event) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    };
    UploadFilesComponent.prototype.onDragEnd = function (event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    };
    UploadFilesComponent.prototype.onDragLeave = function (event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    };
    UploadFilesComponent.prototype.onDrop = function (event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
        event.stopPropagation();
        var files = event.dataTransfer.files;
        //let file: File = files[0];
        this.fileList.push(files);
        //this.saveFiles(files);
    };
    UploadFilesComponent.prototype.saveFiles = function (files) {
        var _this = this;
        this.errors = []; // Clear error
        // Validate file size and allowed extensions
        if (files.length > 0 && (!this.isValidFiles(files))) {
            this.uploadStatus.emit(false);
            return;
        }
        if (files.length > 0) {
            var formData = new FormData();
            for (var j = 0; j < files.length; j++) {
                formData.append("file[]", files[j], files[j].name);
            }
            var parameters = {
                projectId: this.projectId,
                sectionId: this.sectionId
            };
            this.fileService.upload(formData, parameters)
                .subscribe(function (success) {
                _this.uploadStatus.emit(true);
                console.log(success);
            }, function (error) {
                _this.uploadStatus.emit(true);
                _this.errors.push(error.ExceptionMessage);
            });
        }
    };
    UploadFilesComponent.prototype.isValidFiles = function (files) {
        // Check Number of files
        if (files.length > this.maxFiles) {
            this.errors.push("Error: At a time you can upload only " + this.maxFiles + " files");
            return;
        }
        this.isValidFileExtension(files);
        return this.errors.length === 0;
    };
    UploadFilesComponent.prototype.isValidFileExtension = function (files) {
        // Make array of file extensions
        var extensions = (this.fileExt.split(','))
            .map(function (x) { return x.toLocaleUpperCase().trim(); });
        for (var i = 0; i < files.length; i++) {
            // Get file extension
            var ext = files[i].name.toUpperCase().split('.').pop() || files[i].name;
            // Check the extension exists
            var exists = extensions.includes(ext);
            if (!exists) {
                this.errors.push("Error (Extension): " + files[i].name);
            }
            // Check file size
            this.isValidFileSize(files[i]);
        }
    };
    UploadFilesComponent.prototype.isValidFileSize = function (file) {
        var fileSizeinMB = file.size / (1024 * 1000);
        var size = Math.round(fileSizeinMB * 100) / 100; // convert upto 2 decimal place
        if (size > this.maxSize)
            this.errors.push("Error (File Size): " + file.name + ": exceed file size limit of " + this.maxSize + "MB ( " + size + "MB )");
    };
    __decorate([
        core_1.Input(),
        __metadata("design:type", Number)
    ], UploadFilesComponent.prototype, "projectId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Number)
    ], UploadFilesComponent.prototype, "sectionId", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", String)
    ], UploadFilesComponent.prototype, "fileExt", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Number)
    ], UploadFilesComponent.prototype, "maxFiles", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Number)
    ], UploadFilesComponent.prototype, "maxSize", void 0);
    __decorate([
        core_1.Output(),
        __metadata("design:type", Object)
    ], UploadFilesComponent.prototype, "uploadStatus", void 0);
    __decorate([
        core_1.Output('filesArray'),
        __metadata("design:type", Object)
    ], UploadFilesComponent.prototype, "outgoingData", void 0);
    __decorate([
        core_1.HostListener('dragover', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], UploadFilesComponent.prototype, "onDragOver", null);
    __decorate([
        core_1.HostListener('dragenter', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], UploadFilesComponent.prototype, "onDragEnter", null);
    __decorate([
        core_1.HostListener('dragend', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], UploadFilesComponent.prototype, "onDragEnd", null);
    __decorate([
        core_1.HostListener('dragleave', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], UploadFilesComponent.prototype, "onDragLeave", null);
    __decorate([
        core_1.HostListener('drop', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], UploadFilesComponent.prototype, "onDrop", null);
    UploadFilesComponent = __decorate([
        core_1.Component({
            selector: 'upload-files',
            templateUrl: 'app/submittal-package/upload-files.component.html',
            styleUrls: ['app/submittal-package/upload-files.component.css'],
            providers: [upload_files_service_1.UploadFileService]
        }),
        __metadata("design:paramtypes", [http_1.Http, upload_files_service_1.UploadFileService])
    ], UploadFilesComponent);
    return UploadFilesComponent;
}());
exports.UploadFilesComponent = UploadFilesComponent;
//# sourceMappingURL=upload-files.component.js.map