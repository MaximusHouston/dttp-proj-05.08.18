import { Injectable } from '@angular/core';

@Injectable()
export class Enums {

    public SystemAccessEnum: SystemAccessEnums = new SystemAccessEnums();
    public UserTypeEnum: UserTypeEnum = new UserTypeEnum();
    public ExistingBusinessEnum: ExistingBusinessEnum = new ExistingBusinessEnum();
    public BusinessTypeEnum: BusinessTypeEnum = new BusinessTypeEnum();
    public ProductFamilyEnum: ProductFamilyEnum = new ProductFamilyEnum();
    public ProductTypeEnum: ProductTypeEnum = new ProductTypeEnum();
    public ProductModelTypeEnum: ProductModelTypeEnum = new ProductModelTypeEnum();
    public UnitInstallationTypeEnum: UnitInstallationTypeEnum = new UnitInstallationTypeEnum();
    public ProductClassPIMEnum: ProductClassPIMEnum = new ProductClassPIMEnum();
    public ProductEnergySourceTypeEnum: ProductEnergySourceTypeEnum = new ProductEnergySourceTypeEnum();
    public ProductStatusTypeEnum: ProductStatusTypeEnum = new ProductStatusTypeEnum();
    public ProductInventoryStatusTypeEnum: ProductInventoryStatusTypeEnum = new ProductInventoryStatusTypeEnum();

    public SubmittalSheetTypeEnum: SubmittalSheetTypeEnum = new SubmittalSheetTypeEnum();
    public DiscountRequestStatusTypeEnum: DiscountRequestStatusTypeEnum = new DiscountRequestStatusTypeEnum();
    public CommissionRequestStatusTypeEnum: CommissionRequestStatusTypeEnum = new CommissionRequestStatusTypeEnum();
    public LineItemTypeEnum: LineItemTypeEnum = new LineItemTypeEnum();
    public LineItemOptionTypeEnum: LineItemOptionTypeEnum = new LineItemOptionTypeEnum();

    public OrderStatusTypeEnum: OrderStatusTypeEnum = new OrderStatusTypeEnum();
    public ToolAccessEnum: ToolAccessEnum = new ToolAccessEnum();


    constructor() {
    }
}

export class SystemAccessEnums {
    public None: any = 1;
    public ManageGroups: any = 20;
     
    public ApproveUsers: any = 30;
    public ViewUsers: any = 32;
    public EditUser: any = 34;
    public AdminAccessRights: any = 38;
    public UndeleteUser: any = 36;
     
    public ViewBusiness: any = 40;
    public EditBusiness: any = 42;
    public UndeleteBusiness: any = 44;
     
    public ViewProject: any = 50;
    public EditProject: any = 52;
    public UndeleteProject: any = 54;
    //ShareProject : any = 56;
    public TransferProject: any = 58;
    public ViewProjectsInGroup: any = 59;
     
    public RequestDiscounts: any = 60;
    public ApproveDiscounts: any = 62;
     
    public ViewOrder: any = 67;
    public SubmitOrder: any = 68;
     
    //CMS access permissions
    public ContentManagementHomeScreen: any = 70;
    public ContentManagementFunctionalBuildings: any = 71;
    public ContentManagementApplicationBuildings: any = 72;
    public ContentManagementApplicationProducts: any = 73;
    public ContentManagementLibrary: any = 74;
    public ContentManagementCommsCenter: any = 75;
    public ContentManagementProductFamilies: any = 76;
    public ContentManagementTools: any = 77;

    // Pipeline Access Permissions
    public ViewPipelineData: any = 80;
    public EditPipelineData: any = 82;
   
    //View Discount Request
    public ViewDiscountRequest: any = 63;
    
    public RequestCommission: any = 64;
    public ApprovedRequestCommission: any = 65;
    public ViewRequestedCommission: any = 66;
}

export class UserTypeEnum {
    public Systems : any = 255;
    public DaikinSuperUser : any = 190;
    public DaikinAdmin : any = 170;
    public DaikinEmployee : any = 150;
    public CustomerSuperUser : any = 90;
    public CustomerAdmin : any = 70;
    public CustomerUser : any = 50;
    public NotSet : any = 0;
    public OtherType: any = 10;
}

export class ExistingBusinessEnum {
    public New: any = 0;
    public Existing: any = 1;
    public Duplicate: any = 2;
}

export class BusinessTypeEnum {

    public Unknown: any = 1;
    public Daikin: any = 100000;
    public ManufacturerRep: any = 200003;
    public Distributor: any = 200000;
    public Dealer: any = 200001;
    public EngineerArchitect: any = 200005;
    public Other: any = 100000000

    constructor() {
    }


}

export class ProductFamilyEnum {

    public Other: any = 1;
    public Accessories: any = 2;
   
    public UnitarySplitSystem : any = 111521;
    public UnitaryPackagedSystem : any = 111522;
    public MiniSplit : any = 111523;
    public MultiSplit : any = 111524;
    public SkyAir : any = 111525;
    public VRV : any = 111526;
    public AlthermaSplit : any = 111527;
    public AlthermaMonobloc : any = 111528;
    public LightCommercialSplitSystem : any = 111529;
    public LightCommercialPackagedSystem : any = 111530

    constructor() {
    }


}

export class ProductModelTypeEnum {

    public Other: any = 1;
    public All: any = 100000999;
    
    public Indoor: any = 111531;
    public Outdoor: any = 111532;
    public System: any = 111533;
    public Accessory: any = 112553;

    constructor() {
    }


}

export class ProductTypeEnum {

    public Equipment: any = 111226;
  
    public Accessory: any = 111227;

    public Service: any = 111228;

    constructor() {
    }


}


//TODO: to be renamed
export class UnitInstallationTypeEnum {

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


    public None = 0;
    public Other = 1;
    public All = 100000999;
    public AirHandler = 100000000;
    public EvaporatorCoil = 100000001;
    public PackageAC = 100000002;
    public PackageHP = 100000003;
    public PackageDF = 100000004;
    public PackageGE = 100000005;
    public WallMounted = 111006;
    public CeilingSuspended = 111008;
    public Ducted = 111007;
    public FloorStanding = 111010;
    public CeilingCassette = 111009;
    public GasFurnace = 100000156;
    public CasedCoil = 100000157;
    public CoilOnly = 100000158;
    public CoolingOnly = 100000301;
    public HeatPump = 100000302;
    public HeatRecovery = 100000303;
    public AirConditioner = 100000304;
    public DualFloorCeilingSuspended = 111011;
    public Rooftop = 111012;

    constructor() {
    }


}

export class ProductClassPIMEnum {

    public None = 0;
    public All = 100000999;
    public NewProduct = 111173;
    public SplitAC = 111174;
    public SplitHP = 111175;
    public Coil = 111176;
    public AirHandler = 111177;
    public GasFurnace = 111178;
    public PackageAC = 111179;
    public PackagedHP = 111180;
    public PackagedGED = 111181;
    public LightCommercialPackagedAC = 111182;
    public LightCommercialPackagedHP = 111183;
    public LightCommercialPackagedGE = 111184;
    public MiniSplitAC = 111185;
    public MiniSplitHP = 111186;
    public MiniSplitFC = 111187;
    public MiniSplitSystem = 111188;
    public MultiSplitAC = 111189;
    public MultiSplitHP = 111190;
    public MultiSplitFC = 111191;
    public SkyAirAC = 111192;
    public SkyAirHP = 111193;
    public SkyAirFC = 111194;
    public SkyAirSystem = 111195;
    public AlthermaMonoBlocHP = 111196;
    public AlthermaMonoBlocHeatOnly = 111197;
    public AlthermaSplitHP = 111198;
    public AlthermaSplitHeatOnly = 111199;
    public AlthermaFC = 111200;
    public AlthermaWaterTank = 111201;
    public VRVAirCooledHP = 111202;
    public VRVAirCooledHR = 111203;
    public VRVFanCoil = 111204;
    public VRVWaterCooledHP = 111205;
    public VRVWaterCooledHR = 111206;
    public VRVVentilation = 111207;
    public AdapterPCB = 111208;
    public ElectricHeater = 111209;
    public ExpansionValveKit = 111210;
    public Filters = 111211;
    public GeneralAccessories = 111212;
    public InstallationBox = 111213;
    public PipingKit = 111214;
    public BranchSelector = 111215;
    public CondensateDrainKit = 111216;
    public Controllers = 1112117;
    public DecorationPanel = 111218;
    public SensorKit = 111219;
    public VentilationKit = 111220;
    public PackagedAC = 112261

    constructor() {
    }


}

export class ProductEnergySourceTypeEnum {

    public Electric: any = 110979;

    public Gas: any = 110980;

    public DualFuel: any = 110981;

    constructor() {
    }
}

export class ProductStatusTypeEnum {

    public New: any = 111266;
    public Active: any = 111267;
    public Obsolete: any = 111268;
    public HiddenModuleUnit: any = 111269;
    public Inactive: any = 143382;
    public Other: any = 1;

    constructor() {
    }
}

export class ProductInventoryStatusTypeEnum {

    public Available: any = 111700;
    public ContactCSR: any = 111710;
    //public ContactEquipmentCSR: any = 111720;
    public NotAvailable: any = 111730;

    constructor() {
    }
}


export class SubmittalSheetTypeEnum {

    public Other: any = 1;
    public CoilsAirHandlers: any = 111499;
    public CommercialACAndHP: any = 111500;
    public CommercialAH: any = 111501;
    public GasFurnace: any = 111502
    public AlthermaIndoor: any = 111503;
    public AlthermaOutdoor: any = 111504;
    public AlthermaTank: any = 111505;
    public MultiSplitIndoor: any = 111506;
    public MultiSplitOutdoor: any = 111507;
    public SystemCooling: any = 111508;
    public SystemHP: any = 111509;
    public VRVIIIAirCooled: any = 111510;
    public VRVIIIWaterCooled: any = 111511;
    public VRVIndoor: any = 111512;
    public Controllers: any = 111513;
    public Accessories: any = 111514;
    public RTU: any = 111515;
    public Packaged: any = 111516;
    public ACAndHP: any = 111517;
    public PackagedACAndHP: any = 111518;
    public PackagedDF: any = 111519;
    public PackagedGE: any = 111520;
   

    constructor() {
    }


}

export class DiscountRequestStatusTypeEnum {

    public NewRecord: any = 0;

    public Pending: any = 2;

    public Rejected: any = 4;

    public Approved: any = 6;

    public Deleted: any = 8;

    constructor() {
    }
}

export class CommissionRequestStatusTypeEnum {

    public NewRecord: any = 0;

    public Pending: any = 2;

    public Rejected: any = 4;

    public Approved: any = 6;

    public Deleted: any = 8;

    constructor() {
    }
}

export class LineItemTypeEnum {

    public Standard: any = 1;
    public Configured : any = 2;

    constructor() {
    }
}

export class LineItemOptionTypeEnum {

    public BaseModel: any = 1;
    public FactoryInstalled: any = 2;
    public FieldInstalled: any = 3;

    constructor() {
    }
}



export class OrderStatusTypeEnum {

    public NewRecord: any = 1;

    public Submitted: any = 2;

    public AwaitingCSR: any = 3;

    public Accepted: any = 4;

    public InProcess: any = 5;

    public Picked: any = 6;

    public Shipped: any = 7;

    public Canceled: any = 8;

    constructor() {
    }


}

export class ToolAccessEnum {

    public WEBXpress: any = 20;
    public UnitaryMatchupTool: any = 35;
    public CommercialSplitMatchupTool: any = 36;
    public LCSubmittalTool: any = 120;

    constructor() {
    }
}

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