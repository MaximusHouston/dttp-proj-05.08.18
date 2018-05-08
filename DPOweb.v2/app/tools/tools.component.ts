import { Component, ElementRef, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from '../shared/services/toastr.service';
import { ToolsService } from './tools.service'; 
import { KeepHtmlPipe } from '../shared/pipes/keep-html.pipe';
import { UserService } from '../shared/services/user.service';
import { User } from '../shared/models/user';
 
declare var jQuery: any;

@Component({
    selector: "tools",
    templateUrl: "app/tools/tools.component.html",
    styleUrls: ["app/tools/tools.component.css"],
    providers: [ToolsService]
})
export class ToolsComponent{
 
    private tools: any = [];
    //public user: User;
    public user: any;
    
    constructor(private router: Router, private route: ActivatedRoute, private toolsService: ToolsService, private userSvc: UserService) {
        this.tools = this.getAllTools();
        this.user = this.route.snapshot.data['currentUser'].model;
    }

    getAllTools() {

        //var user = this.userSvc.getCurrentUser().then(data => {
        //    this.user = data.model;
        //});

        return this.toolsService.getTools().subscribe(data => {
            this.tools = data;

            for (var i = 0; i < this.tools.length; i++) {
                if (this.tools[i].accessUrl != null) {
                    this.tools[i].clickable = true;

                    //accessUrl.indexOf("http://securenet.goodmanmfg.com/litconfig-DCTool/listDealers.html?sessionId=123") !== -1) {

                    if (this.tools[i].toolId == 120) {
                        this.tools[i].accessUrl = "http://securenet.goodmanmfg.com/litconfig-DCTool/listDealers.html?sessionId=123" + "&userId=" + this.user.userId + "&firstName=" + this.user.firstName + "&lastName=" + this.user.lastName + "&email=" + this.user.email;
                    }
                }
                else {
                    this.tools[i].downloadable = true;
                }
            }
        });       
    }

    convertToSafeHtml(description:string) {
        return 
    }

    ngOnDestroy() {
        
    }
}
    

     