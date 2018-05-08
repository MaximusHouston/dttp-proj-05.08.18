import {Injectable} from '@angular/core';


@Injectable()
export class LoadingIconService {

    //start spinning icon
    //var productGrid = jQuery("#productGrid");
    //if (productGrid != undefined) {
    //    kendo.ui.progress(productGrid, true);
    //}

    //stop spinning icon
    //var productGrid = jQuery("#productGrid");
    //if (productGrid != undefined) {
    //    kendo.ui.progress(productGrid, false);
    //}

    Start(target: any) {
        var element = jQuery(target);
        if (element != undefined) {
            kendo.ui.progress(element, true);
        }
        this.AppendBackDrop();
    }

    Stop(target: any) {
        var element = jQuery(target);
        if (element != undefined) {
            kendo.ui.progress(element, false);
        }
        this.RemoveBackDrop();
    }

    AppendBackDrop() {
        $('<div class="modal-backdrop fade in"></div>').appendTo(document.body);
    }

    RemoveBackDrop() {
        $(".modal-backdrop").remove();
    }


}