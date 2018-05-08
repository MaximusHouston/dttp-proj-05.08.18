//TODO: This component is not used because signUpForm valiation does not work

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from '../shared/services/toastr.service';
import { LoadingIconService } from '../shared/services/loadingIcon.service';
import { PasswordService } from '../shared/services/password.service';

import { UserService } from '../shared/services/user.service';
import { SystemAccessEnum } from '../shared/services/systemAccessEnum';
import { Enums } from '../shared/enums/enums';

import { AccountService } from './services/account.service';

declare var jQuery: any;

@Component({
    selector: "user-personal-details",
    templateUrl: "app/account/user-personal-details.component.html"

})

export class UserPersonalDetailsComponent implements OnInit {
    @Input() user: any;
    public defaultItem: { text: string, value: string } = { text: "Select...", value: null };
    public phoneNumberMask: string = "(000) 000-0000";

    constructor(private router: Router, private route: ActivatedRoute, private toastrSvc: ToastrService,
        private loadingIconSvc: LoadingIconService, private passwordSvc: PasswordService,
        private userSvc: UserService, private accountSvc: AccountService,
        private systemAccessEnum: SystemAccessEnum, private enums: Enums) {
      
    }

    ngOnInit() {
        this.setupPasswordStrengthIndicator();
               
    }

    public setupPasswordStrengthIndicator() {
        var self = this;
        jQuery("#userPassword").keyup(function (event: any) {
            event.stopImmediatePropagation();
            var password = jQuery("#userPassword")[0].value;
            self.showPasswordStrength(password);
        });
    }

    
    public showPasswordStrength(password: string) {

        if (this.passwordSvc.PasswordStrengthLevel(password) == 0) {// empty
            $('#passwordStrengthBar').css("background-color", "#ddd");
            $('#passwordStrengthBar').css("width", "0%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 1) {// weak
            $('#passwordStrengthBar').css("background-color", "#ff704d");
            $('#passwordStrengthBar').css("width", "10%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 2) {// medium 1 
            $('#passwordStrengthBar').css("background-color", "#ffcc66");
            $('#passwordStrengthBar').css("width", "30%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 3) {// medium 2
            $('#passwordStrengthBar').css("background-color", "#ffcc66");
            $('#passwordStrengthBar').css("width", "50%");
        }
        else if (this.passwordSvc.PasswordStrengthLevel(password) == 4) {// strong
            $('#passwordStrengthBar').css("background-color", "#80bfff");
            $('#passwordStrengthBar').css("width", "70%");
        } else if (this.passwordSvc.PasswordStrengthLevel(password) >= 5) {// very strong
            $('#passwordStrengthBar').css("background-color", "#5cd65c");
            $('#passwordStrengthBar').css("width", "100%");
        }
    }

    

  

};