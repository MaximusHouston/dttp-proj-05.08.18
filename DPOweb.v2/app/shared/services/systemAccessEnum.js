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
var SystemAccessEnum = /** @class */ (function () {
    function SystemAccessEnum() {
        this.SystemAccess = {
            "None": 1,
            "ManageGroups": 20,
            "ApproveUsers": 30,
            "ViewUsers": 32,
            "EditUser": 34,
            "AdminAccessRights": 38,
            "UndeleteUser": 36,
            "ViewBusiness": 40,
            "EditBusiness": 42,
            "UndeleteBusiness": 44,
            "ViewProject": 50,
            "EditProject": 52,
            "UndeleteProject": 54,
            //"ShareProject" : 56,
            "TransferProject": 58,
            "ViewProjectsInGroup": 59,
            "RequestDiscounts": 60,
            "ApproveDiscounts": 62,
            "ViewOrder": 67,
            "SubmitOrder": 68,
            //CMS access permissions
            "ContentManagementHomeScreen": 70,
            "ContentManagementFunctionalBuildings": 71,
            "ContentManagementApplicationBuildings": 72,
            "ContentManagementApplicationProducts": 73,
            "ContentManagementLibrary": 74,
            "ContentManagementCommsCenter": 75,
            "ContentManagementProductFamilies": 76,
            "ContentManagementTools": 77,
            // Pipeline Access Permissions
            "ViewPipelineData": 80,
            "EditPipelineData": 82,
            //View Discount Request
            "ViewDiscountRequest": 63,
            "RequestCommission": 64,
            "ApprovedRequestCommission": 65,
            "ViewRequestedCommission": 66
        };
    }
    //this function return integer value of SystemAccessEnum
    SystemAccessEnum.prototype.getSystemAccessValueByName = function (systemAccessName) {
        return this.SystemAccess[systemAccessName];
    };
    SystemAccessEnum = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [])
    ], SystemAccessEnum);
    return SystemAccessEnum;
}());
exports.SystemAccessEnum = SystemAccessEnum;
//# sourceMappingURL=systemAccessEnum.js.map