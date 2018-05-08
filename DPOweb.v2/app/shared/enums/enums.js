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
var Enums = /** @class */ (function () {
    function Enums() {
        this.SystemAccessEnum = new SystemAccessEnums();
        this.UserTypeEnum = new UserTypeEnum();
        this.ExistingBusinessEnum = new ExistingBusinessEnum();
        this.BusinessTypeEnum = new BusinessTypeEnum();
        this.ProductFamilyEnum = new ProductFamilyEnum();
        this.ProductTypeEnum = new ProductTypeEnum();
        this.ProductModelTypeEnum = new ProductModelTypeEnum();
        this.UnitInstallationTypeEnum = new UnitInstallationTypeEnum();
        this.ProductClassPIMEnum = new ProductClassPIMEnum();
        this.ProductEnergySourceTypeEnum = new ProductEnergySourceTypeEnum();
        this.ProductStatusTypeEnum = new ProductStatusTypeEnum();
        this.ProductInventoryStatusTypeEnum = new ProductInventoryStatusTypeEnum();
        this.SubmittalSheetTypeEnum = new SubmittalSheetTypeEnum();
        this.DiscountRequestStatusTypeEnum = new DiscountRequestStatusTypeEnum();
        this.CommissionRequestStatusTypeEnum = new CommissionRequestStatusTypeEnum();
        this.LineItemTypeEnum = new LineItemTypeEnum();
        this.LineItemOptionTypeEnum = new LineItemOptionTypeEnum();
        this.OrderStatusTypeEnum = new OrderStatusTypeEnum();
        this.ToolAccessEnum = new ToolAccessEnum();
    }
    Enums = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [])
    ], Enums);
    return Enums;
}());
exports.Enums = Enums;
var SystemAccessEnums = /** @class */ (function () {
    function SystemAccessEnums() {
        this.None = 1;
        this.ManageGroups = 20;
        this.ApproveUsers = 30;
        this.ViewUsers = 32;
        this.EditUser = 34;
        this.AdminAccessRights = 38;
        this.UndeleteUser = 36;
        this.ViewBusiness = 40;
        this.EditBusiness = 42;
        this.UndeleteBusiness = 44;
        this.ViewProject = 50;
        this.EditProject = 52;
        this.UndeleteProject = 54;
        //ShareProject : any = 56;
        this.TransferProject = 58;
        this.ViewProjectsInGroup = 59;
        this.RequestDiscounts = 60;
        this.ApproveDiscounts = 62;
        this.ViewOrder = 67;
        this.SubmitOrder = 68;
        //CMS access permissions
        this.ContentManagementHomeScreen = 70;
        this.ContentManagementFunctionalBuildings = 71;
        this.ContentManagementApplicationBuildings = 72;
        this.ContentManagementApplicationProducts = 73;
        this.ContentManagementLibrary = 74;
        this.ContentManagementCommsCenter = 75;
        this.ContentManagementProductFamilies = 76;
        this.ContentManagementTools = 77;
        // Pipeline Access Permissions
        this.ViewPipelineData = 80;
        this.EditPipelineData = 82;
        //View Discount Request
        this.ViewDiscountRequest = 63;
        this.RequestCommission = 64;
        this.ApprovedRequestCommission = 65;
        this.ViewRequestedCommission = 66;
    }
    return SystemAccessEnums;
}());
exports.SystemAccessEnums = SystemAccessEnums;
var UserTypeEnum = /** @class */ (function () {
    function UserTypeEnum() {
        this.Systems = 255;
        this.DaikinSuperUser = 190;
        this.DaikinAdmin = 170;
        this.DaikinEmployee = 150;
        this.CustomerSuperUser = 90;
        this.CustomerAdmin = 70;
        this.CustomerUser = 50;
        this.NotSet = 0;
        this.OtherType = 10;
    }
    return UserTypeEnum;
}());
exports.UserTypeEnum = UserTypeEnum;
var ExistingBusinessEnum = /** @class */ (function () {
    function ExistingBusinessEnum() {
        this.New = 0;
        this.Existing = 1;
        this.Duplicate = 2;
    }
    return ExistingBusinessEnum;
}());
exports.ExistingBusinessEnum = ExistingBusinessEnum;
var BusinessTypeEnum = /** @class */ (function () {
    function BusinessTypeEnum() {
        this.Unknown = 1;
        this.Daikin = 100000;
        this.ManufacturerRep = 200003;
        this.Distributor = 200000;
        this.Dealer = 200001;
        this.EngineerArchitect = 200005;
        this.Other = 100000000;
    }
    return BusinessTypeEnum;
}());
exports.BusinessTypeEnum = BusinessTypeEnum;
var ProductFamilyEnum = /** @class */ (function () {
    function ProductFamilyEnum() {
        this.Other = 1;
        this.Accessories = 2;
        this.UnitarySplitSystem = 111521;
        this.UnitaryPackagedSystem = 111522;
        this.MiniSplit = 111523;
        this.MultiSplit = 111524;
        this.SkyAir = 111525;
        this.VRV = 111526;
        this.AlthermaSplit = 111527;
        this.AlthermaMonobloc = 111528;
        this.LightCommercialSplitSystem = 111529;
        this.LightCommercialPackagedSystem = 111530;
    }
    return ProductFamilyEnum;
}());
exports.ProductFamilyEnum = ProductFamilyEnum;
var ProductModelTypeEnum = /** @class */ (function () {
    function ProductModelTypeEnum() {
        this.Other = 1;
        this.All = 100000999;
        this.Indoor = 111531;
        this.Outdoor = 111532;
        this.System = 111533;
        this.Accessory = 112553;
    }
    return ProductModelTypeEnum;
}());
exports.ProductModelTypeEnum = ProductModelTypeEnum;
var ProductTypeEnum = /** @class */ (function () {
    function ProductTypeEnum() {
        this.Equipment = 111226;
        this.Accessory = 111227;
        this.Service = 111228;
    }
    return ProductTypeEnum;
}());
exports.ProductTypeEnum = ProductTypeEnum;
//TODO: to be renamed
var UnitInstallationTypeEnum = /** @class */ (function () {
    function UnitInstallationTypeEnum() {
        // TODO:  Delete after 10/01/2017
        //public None = 0;
        //public Other = 1;
        //public All = 100000999;
        //public AirHandler = 100000000;
        //public EvaporatorCoil = 100000001;
        //public PackageAC = 100000002;
        //public PackageHP = 100000003;
        //public PackageDF = 100000004;
        //public PackageGE = 100000005;
        //public WallMounted = 100000151;
        //public CeilingSuspended = 100000152;
        //public Ducted = 100000153;
        //public FloorStanding = 100000154;
        //public CeilingCassette = 100000155;
        //public GasFurnace = 100000156;
        //public CasedCoil = 100000157;
        //public CoilOnly = 100000158;
        //public CoolingOnly = 100000301;
        //public HeatPump = 100000302;
        //public HeatRecovery = 100000303;
        //public AirConditioner = 100000304;
        // TODO END:  Delete after 10/01/2017
        this.None = 0;
        this.Other = 1;
        this.All = 100000999;
        this.AirHandler = 100000000;
        this.EvaporatorCoil = 100000001;
        this.PackageAC = 100000002;
        this.PackageHP = 100000003;
        this.PackageDF = 100000004;
        this.PackageGE = 100000005;
        this.WallMounted = 111006;
        this.CeilingSuspended = 111008;
        this.Ducted = 111007;
        this.FloorStanding = 111010;
        this.CeilingCassette = 111009;
        this.GasFurnace = 100000156;
        this.CasedCoil = 100000157;
        this.CoilOnly = 100000158;
        this.CoolingOnly = 100000301;
        this.HeatPump = 100000302;
        this.HeatRecovery = 100000303;
        this.AirConditioner = 100000304;
        this.DualFloorCeilingSuspended = 111011;
        this.Rooftop = 111012;
    }
    return UnitInstallationTypeEnum;
}());
exports.UnitInstallationTypeEnum = UnitInstallationTypeEnum;
var ProductClassPIMEnum = /** @class */ (function () {
    function ProductClassPIMEnum() {
        this.None = 0;
        this.All = 100000999;
        this.NewProduct = 111173;
        this.SplitAC = 111174;
        this.SplitHP = 111175;
        this.Coil = 111176;
        this.AirHandler = 111177;
        this.GasFurnace = 111178;
        this.PackageAC = 111179;
        this.PackagedHP = 111180;
        this.PackagedGED = 111181;
        this.LightCommercialPackagedAC = 111182;
        this.LightCommercialPackagedHP = 111183;
        this.LightCommercialPackagedGE = 111184;
        this.MiniSplitAC = 111185;
        this.MiniSplitHP = 111186;
        this.MiniSplitFC = 111187;
        this.MiniSplitSystem = 111188;
        this.MultiSplitAC = 111189;
        this.MultiSplitHP = 111190;
        this.MultiSplitFC = 111191;
        this.SkyAirAC = 111192;
        this.SkyAirHP = 111193;
        this.SkyAirFC = 111194;
        this.SkyAirSystem = 111195;
        this.AlthermaMonoBlocHP = 111196;
        this.AlthermaMonoBlocHeatOnly = 111197;
        this.AlthermaSplitHP = 111198;
        this.AlthermaSplitHeatOnly = 111199;
        this.AlthermaFC = 111200;
        this.AlthermaWaterTank = 111201;
        this.VRVAirCooledHP = 111202;
        this.VRVAirCooledHR = 111203;
        this.VRVFanCoil = 111204;
        this.VRVWaterCooledHP = 111205;
        this.VRVWaterCooledHR = 111206;
        this.VRVVentilation = 111207;
        this.AdapterPCB = 111208;
        this.ElectricHeater = 111209;
        this.ExpansionValveKit = 111210;
        this.Filters = 111211;
        this.GeneralAccessories = 111212;
        this.InstallationBox = 111213;
        this.PipingKit = 111214;
        this.BranchSelector = 111215;
        this.CondensateDrainKit = 111216;
        this.Controllers = 1112117;
        this.DecorationPanel = 111218;
        this.SensorKit = 111219;
        this.VentilationKit = 111220;
        this.PackagedAC = 112261;
    }
    return ProductClassPIMEnum;
}());
exports.ProductClassPIMEnum = ProductClassPIMEnum;
var ProductEnergySourceTypeEnum = /** @class */ (function () {
    function ProductEnergySourceTypeEnum() {
        this.Electric = 110979;
        this.Gas = 110980;
        this.DualFuel = 110981;
    }
    return ProductEnergySourceTypeEnum;
}());
exports.ProductEnergySourceTypeEnum = ProductEnergySourceTypeEnum;
var ProductStatusTypeEnum = /** @class */ (function () {
    function ProductStatusTypeEnum() {
        this.New = 111266;
        this.Active = 111267;
        this.Obsolete = 111268;
        this.HiddenModuleUnit = 111269;
        this.Inactive = 143382;
        this.Other = 1;
    }
    return ProductStatusTypeEnum;
}());
exports.ProductStatusTypeEnum = ProductStatusTypeEnum;
var ProductInventoryStatusTypeEnum = /** @class */ (function () {
    function ProductInventoryStatusTypeEnum() {
        this.Available = 111700;
        this.ContactCSR = 111710;
        //public ContactEquipmentCSR: any = 111720;
        this.NotAvailable = 111730;
    }
    return ProductInventoryStatusTypeEnum;
}());
exports.ProductInventoryStatusTypeEnum = ProductInventoryStatusTypeEnum;
var SubmittalSheetTypeEnum = /** @class */ (function () {
    function SubmittalSheetTypeEnum() {
        this.Other = 1;
        this.CoilsAirHandlers = 111499;
        this.CommercialACAndHP = 111500;
        this.CommercialAH = 111501;
        this.GasFurnace = 111502;
        this.AlthermaIndoor = 111503;
        this.AlthermaOutdoor = 111504;
        this.AlthermaTank = 111505;
        this.MultiSplitIndoor = 111506;
        this.MultiSplitOutdoor = 111507;
        this.SystemCooling = 111508;
        this.SystemHP = 111509;
        this.VRVIIIAirCooled = 111510;
        this.VRVIIIWaterCooled = 111511;
        this.VRVIndoor = 111512;
        this.Controllers = 111513;
        this.Accessories = 111514;
        this.RTU = 111515;
        this.Packaged = 111516;
        this.ACAndHP = 111517;
        this.PackagedACAndHP = 111518;
        this.PackagedDF = 111519;
        this.PackagedGE = 111520;
    }
    return SubmittalSheetTypeEnum;
}());
exports.SubmittalSheetTypeEnum = SubmittalSheetTypeEnum;
var DiscountRequestStatusTypeEnum = /** @class */ (function () {
    function DiscountRequestStatusTypeEnum() {
        this.NewRecord = 0;
        this.Pending = 2;
        this.Rejected = 4;
        this.Approved = 6;
        this.Deleted = 8;
    }
    return DiscountRequestStatusTypeEnum;
}());
exports.DiscountRequestStatusTypeEnum = DiscountRequestStatusTypeEnum;
var CommissionRequestStatusTypeEnum = /** @class */ (function () {
    function CommissionRequestStatusTypeEnum() {
        this.NewRecord = 0;
        this.Pending = 2;
        this.Rejected = 4;
        this.Approved = 6;
        this.Deleted = 8;
    }
    return CommissionRequestStatusTypeEnum;
}());
exports.CommissionRequestStatusTypeEnum = CommissionRequestStatusTypeEnum;
var LineItemTypeEnum = /** @class */ (function () {
    function LineItemTypeEnum() {
        this.Standard = 1;
        this.Configured = 2;
    }
    return LineItemTypeEnum;
}());
exports.LineItemTypeEnum = LineItemTypeEnum;
var LineItemOptionTypeEnum = /** @class */ (function () {
    function LineItemOptionTypeEnum() {
        this.BaseModel = 1;
        this.FactoryInstalled = 2;
        this.FieldInstalled = 3;
    }
    return LineItemOptionTypeEnum;
}());
exports.LineItemOptionTypeEnum = LineItemOptionTypeEnum;
var OrderStatusTypeEnum = /** @class */ (function () {
    function OrderStatusTypeEnum() {
        this.NewRecord = 1;
        this.Submitted = 2;
        this.AwaitingCSR = 3;
        this.Accepted = 4;
        this.InProcess = 5;
        this.Picked = 6;
        this.Shipped = 7;
        this.Canceled = 8;
    }
    return OrderStatusTypeEnum;
}());
exports.OrderStatusTypeEnum = OrderStatusTypeEnum;
var ToolAccessEnum = /** @class */ (function () {
    function ToolAccessEnum() {
        this.WEBXpress = 20;
        this.UnitaryMatchupTool = 35;
        this.CommercialSplitMatchupTool = 36;
        this.LCSubmittalTool = 120;
    }
    return ToolAccessEnum;
}());
exports.ToolAccessEnum = ToolAccessEnum;
//@Injectable()
//export class ProductFamilyEnum {
//    public Other: any = 1;
//    public MiniSplit: any = 100000000;
//    public Altherma: any = 100000001;
//    public MultiSplit: any = 100000002;
//    public SkyAir: any = 100000003;
//    public VRV: any = 100000004;
//    public RTU: any = 100000005;
//    public Packaged: any = 100000006;
//    public PTAC: any = 100000007;
//    public Ventilation: any = 100000008;
//    public IAQ_CleanComfort: any = 100000009;
//    public Accessories: any = 100000010;
//    public UnitarySplit: any = 100000012;
//    public UnitaryPackage: any = 100000013;
//    public CommercialSplit: any = 100000014;
//    constructor() {
//    }
//}
//# sourceMappingURL=enums.js.map