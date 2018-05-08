import { Component, OnInit, Input, Output, EventEmitter, Inject } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';

import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { Observable } from 'rxjs/Rx';

import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from '../account/services/account.service';
import { QuoteService } from './services/quote.service';
import { WebConfigService } from '../shared/services/webconfig.service';


declare var jQuery: any;

@Component({
    selector: "select-products-dialog",
    templateUrl: "app/quote/select-products-dialog.component.html"

})

export class SelectProductsDialogComponent implements OnInit {
    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService, private loadingIconSvc: LoadingIconService,
        private userSvc: UserService, private accountSvc: AccountService, private quoteSvc: QuoteService, private webconfigSvc: WebConfigService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {
    }

    public webconfig: any;
    public lcstApiToken: any;

    @Input() user: any;
    @Input() quote: any;
    public selectedTool: { text: string, value: number } ;
     
    public toolList: Array<{ text: string, value: number }> = [
       
    ];
    public defaultItem: { text: string, value: null } = { text: "Select ...", value: null };


    ngOnInit() {
        //var tools = [];
        var self = this;

        this.webconfigSvc.getWebConfig().then((resp: any) => {
            self.webconfig = resp;
            
        }).catch(error => {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });


        this.webconfigSvc.getLCSTApiToken().then((resp: any) => {
            self.lcstApiToken = resp.model;

        }).catch(error => {
            console.log("error message: " + error.message);
            console.log("error stack: " + error.stack);
        });

        for (var i in this.user.toolAccesses) {
            if (this.user.toolAccesses[i].addToQuote == 1) {
                this.toolList.push({ text: this.user.toolAccesses[i].name, value: this.user.toolAccesses[i].toolId });
            }
        }
        if (this.toolList.length == 0) {
            $("#selectProductsBtn").hide();
        }


    }

    public openTool() {
        var self = this;
        //alert("selected tool: " + this.selectedTool.text);
        if (this.selectedTool.value == this.enums.ToolAccessEnum.UnitaryMatchupTool) {
            //window.location.href = "/api/Tool/SystemConfigurator?quoteId=" + this.quote.quoteId;
            this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then((resp: any) => {
                if (resp.isok) {
                    window.location.href = "/v2/#/tools/systemConfigurator";
                    self.toastrSvc.displayResponseMessages(resp);
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(error => {
                console.log('Retrieval error: ${error}');
                console.log(error);
            });

        } else if (this.selectedTool.value == this.enums.ToolAccessEnum.CommercialSplitMatchupTool) {
            //window.location.href = "/api/Tool/SplitSystemConfigurator?quoteId=" + this.quote.quoteId;
            this.quoteSvc.setBasketQuoteId(this.quote.quoteId).then((resp: any) => {
                if (resp.isok) {
                    window.location.href = "/v2/#/tools/splitSystemConfigurator"
                    self.toastrSvc.displayResponseMessages(resp);
                } else {
                    self.toastrSvc.displayResponseMessages(resp);
                }
            }).catch(error => {
                console.log('Retrieval error: ${error}');
                console.log(error);
            });
        } else if (this.selectedTool.value == this.enums.ToolAccessEnum.LCSubmittalTool) {
            window.location.href = this.webconfig.routeConfig.lcstURL + "&quoteId=" + this.quote.quoteId + "&projectId=" + this.quote.projectId + "&projectName=" + this.quote.project.name + "&userId=" + this.user.userId + "&firstName=" + this.user.firstName + "&lastName=" + this.user.lastName + "&token=" + this.lcstApiToken;
        }
    }
  

    
}