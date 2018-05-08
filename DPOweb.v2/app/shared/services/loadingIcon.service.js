"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var LoadingIconService = /** @class */ (function () {
    function LoadingIconService() {
    }
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
    LoadingIconService.prototype.Start = function (target) {
        var element = jQuery(target);
        if (element != undefined) {
            kendo.ui.progress(element, true);
        }
        this.AppendBackDrop();
    };
    LoadingIconService.prototype.Stop = function (target) {
        var element = jQuery(target);
        if (element != undefined) {
            kendo.ui.progress(element, false);
        }
        this.RemoveBackDrop();
    };
    LoadingIconService.prototype.AppendBackDrop = function () {
        $('<div class="modal-backdrop fade in"></div>').appendTo(document.body);
    };
    LoadingIconService.prototype.RemoveBackDrop = function () {
        $(".modal-backdrop").remove();
    };
    LoadingIconService = __decorate([
        core_1.Injectable()
    ], LoadingIconService);
    return LoadingIconService;
}());
exports.LoadingIconService = LoadingIconService;
//# sourceMappingURL=loadingIcon.service.js.map