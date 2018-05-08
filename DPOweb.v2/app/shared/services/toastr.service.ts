import {Injectable} from '@angular/core';
//import * as toastr from "toastr";
//import * as toastr from "../../script/toastr.min";
//import {Toastr} from "toastr";

@Injectable()
export class ToastrService {
    Error(message: any) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "timeOut": 0,
            "extendedTimeOut": 0,
            "preventDuplicates": true
        }

        toastr.error(message + "<br /><br /><button type'button' class='btn clear' style='color: black'>Ok</button>");
    }

    ErrorFadeOut(message: any) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        }

        toastr.error(message);
    }

    Success(message: any) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        }

        toastr.success(message);
    }

    Info(message: any) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        }
        toastr.info(message);
    }

    Warning(message: any) {
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true
        }
        toastr.warning(message);
    }


    displayResponseMessages(response: any) {
        if (response != null && response.messages != null) {
            for (let message of response.messages.items) {
                if (message.type == 40) {// success
                    this.Success(message.text);
                } else if (message.type == 10) {// error
                    this.Error(message.text);
                }

            }
        }
        
    }

    displayResponseMessagesFadeOut(response: any) {
        if (response != null && response.messages != null) {
            for (let message of response.messages.items) {
                if (message.type == 40) {// success
                    this.Success(message.text);
                } else if (message.type == 10) {// error
                    this.ErrorFadeOut(message.text);
                }

            }
        }

    }
}