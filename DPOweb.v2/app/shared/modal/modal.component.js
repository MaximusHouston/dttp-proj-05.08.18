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
var ModalComponent = /** @class */ (function () {
    function ModalComponent(elementRef, changeDetectorRef) {
        this.elementRef = elementRef;
        this.changeDetectorRef = changeDetectorRef;
        this.closeOnOutsideClick = true;
        this.visible = false;
        this.visibleAnimate = false;
    }
    ModalComponent.prototype.ngOnDestroy = function () {
        // Prevent modal from not executing its closing actions if the user navigated away (for example,
        // through a link).
        this.close();
    };
    ModalComponent.prototype.open = function () {
        var _this = this;
        document.body.classList.add('modal-open');
        this.visible = true;
        setTimeout(function () {
            _this.visibleAnimate = true;
        });
    };
    ModalComponent.prototype.close = function () {
        var _this = this;
        document.body.classList.remove('modal-open');
        this.visibleAnimate = false;
        setTimeout(function () {
            _this.visible = false;
            _this.changeDetectorRef.markForCheck();
        }, 200);
    };
    ModalComponent.prototype.onContainerClicked = function (event) {
        if (event.target.classList.contains('modal') && this.isTopMost() && this.closeOnOutsideClick) {
            this.close();
        }
    };
    ModalComponent.prototype.onKeyDownHandler = function (event) {
        // If ESC key and TOP MOST modal, close it.
        if (event.key === 'Escape' && this.isTopMost()) {
            this.close();
        }
    };
    /**
     * Returns true if this modal is the top most modal.
     */
    ModalComponent.prototype.isTopMost = function () {
        return !this.elementRef.nativeElement.querySelector(':scope modal > .modal');
    };
    __decorate([
        core_1.ContentChild('modalHeader'),
        __metadata("design:type", core_1.TemplateRef)
    ], ModalComponent.prototype, "header", void 0);
    __decorate([
        core_1.ContentChild('modalBody'),
        __metadata("design:type", core_1.TemplateRef)
    ], ModalComponent.prototype, "body", void 0);
    __decorate([
        core_1.ContentChild('modalFooter'),
        __metadata("design:type", core_1.TemplateRef)
    ], ModalComponent.prototype, "footer", void 0);
    __decorate([
        core_1.Input(),
        __metadata("design:type", Object)
    ], ModalComponent.prototype, "closeOnOutsideClick", void 0);
    __decorate([
        core_1.HostListener('click', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [MouseEvent]),
        __metadata("design:returntype", void 0)
    ], ModalComponent.prototype, "onContainerClicked", null);
    __decorate([
        core_1.HostListener('document:keydown', ['$event']),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [KeyboardEvent]),
        __metadata("design:returntype", void 0)
    ], ModalComponent.prototype, "onKeyDownHandler", null);
    ModalComponent = __decorate([
        core_1.Component({
            selector: 'modal',
            templateUrl: 'app/shared/modal/modal.component.html',
            styleUrls: ['app/shared/modal/modal.component.css']
        }),
        __metadata("design:paramtypes", [core_1.ElementRef,
            core_1.ChangeDetectorRef])
    ], ModalComponent);
    return ModalComponent;
}());
exports.ModalComponent = ModalComponent;
//# sourceMappingURL=modal.component.js.map