import { Component, OnInit, Input, Output, EventEmitter, HostListener, HostBinding } from '@angular/core';
import { Headers, Http, RequestOptions } from '@angular/http';
import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { UploadFileService } from './services/upload-files.service';

@Component({
    selector: 'upload-files',
    templateUrl: 'app/submittal-package/upload-files.component.html',
    styleUrls: ['app/submittal-package/upload-files.component.css'],
    providers: [UploadFileService]
})
export class UploadFilesComponent implements OnInit {
    private apiEndPoint: string;
    private fileList: Array<string> = [];

    errors: Array<string> = [];
    dragAreaClass: string = 'dragarea';
    @Input() projectId: number = 0;
    @Input() sectionId: number = 0;
    @Input() fileExt: string = "JPG, GIF, PNG, txt";
    @Input() maxFiles: number = 5;
    @Input() maxSize: number = 5; // 5MB
    @Output() uploadStatus = new EventEmitter();

    @Output('filesArray') outgoingData = new EventEmitter<string[]>();

    // @HostBinding('style.background') private background = '#eee';

    constructor(private http: Http, private fileService: UploadFileService) {
        this.apiEndPoint = ""
    }

    ngOnInit() {
    }

    fileChange(event) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file: File = fileList[0];
            let formData: FormData = new FormData();
            formData.append('uploadFile', file, file.name);
            let headers = new Headers();
            /** In Angular 5, including the header Content-Type can invalidate your request */
            headers.append('Content-Type', 'multipart/form-data');
            headers.append('Accept', 'application/json');
            let options = new RequestOptions({ headers: headers });
            // this.http.post(`${this.apiEndPoint}`, formData, options)
            //     .map(res => res.json())
            //     .catch(error => Observable.throw(error))
            //     .subscribe(
            //         data => console.log('success'),
            //         error => console.log(error)
            //     )
        }
    }

    onFileChange(event: { target: { files: any; }; }) {
        let files = event.target.files;
        //this.saveFiles(files);

        this.fileList.push(files);

        //this.fileList = files;
    }

    @HostListener('dragover', ['$event']) onDragOver(event) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    }

    @HostListener('dragenter', ['$event']) onDragEnter(event) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    }

    @HostListener('dragend', ['$event']) onDragEnd(event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    }

    @HostListener('dragleave', ['$event']) onDragLeave(event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    }

    @HostListener('drop', ['$event']) onDrop(event) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
        event.stopPropagation();
        var files = event.dataTransfer.files;
        //let file: File = files[0];
        
        this.fileList.push(files);
        //this.saveFiles(files);
    }

    saveFiles(files) {
        this.errors = []; // Clear error
        // Validate file size and allowed extensions
        if (files.length > 0 && (!this.isValidFiles(files))) {
            this.uploadStatus.emit(false);
            return;
        }
        if (files.length > 0) {
            let formData: FormData = new FormData();
            for (var j = 0; j < files.length; j++) {
                formData.append("file[]", files[j], files[j].name);
            }
            var parameters = {
                projectId: this.projectId,
                sectionId: this.sectionId
            }
            this.fileService.upload(formData, parameters)
                .subscribe(
                    success => {
                        this.uploadStatus.emit(true);
                        console.log(success)
                    },
                    error => {
                        this.uploadStatus.emit(true);
                        this.errors.push(error.ExceptionMessage);
                    })
        }
    }

    private isValidFiles(files) {
        // Check Number of files
        if (files.length > this.maxFiles) {
            this.errors.push("Error: At a time you can upload only " + this.maxFiles + " files");
            return;
        }
        this.isValidFileExtension(files);
        return this.errors.length === 0;
    }

    private isValidFileExtension(files) {
        // Make array of file extensions
        var extensions = (this.fileExt.split(','))
            .map(function (x) { return x.toLocaleUpperCase().trim() });
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
    }

    private isValidFileSize(file) {
        var fileSizeinMB = file.size / (1024 * 1000);
        var size = Math.round(fileSizeinMB * 100) / 100; // convert upto 2 decimal place
        if (size > this.maxSize)
            this.errors.push("Error (File Size): " + file.name + ": exceed file size limit of " + this.maxSize + "MB ( " + size + "MB )");
    }




    // @HostListener('dragover', ['$event']) public onDragOver(evt){
    //   evt.preventDefault();
    //   evt.stopPropagation();
    //   this.background = '#999';
    // }

    // @HostListener('dragleave', ['$event']) public onDragLeave(evt){
    //   evt.preventDefault();
    //   evt.stopPropagation();
    //   this.background = '#eee'
    // }

    // @HostListener('drop', ['$event']) public onDrop(evt){
    //   evt.preventDefault();
    //   evt.stopPropagation();
    //   this.background = '#eee';
    //   let files = evt.dataTransfer.files;
    //   if(files.length > 0){
    //   }
    // }

}
