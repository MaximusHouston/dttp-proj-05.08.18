using System.ComponentModel;

namespace DPO.Common
{
    public enum ProductClassPIMEnum : int
    {
        None,
        All = 100000999,
        [Description("New Product")]
        NewProduct = 111173,

        [Description("Split Air Conditioner")]
        SplitAC = 111174,

        [Description("Split Heat Pump")]
        SplitHP = 111175,

        [Description("Coil")]
        Coil = 111176,

        [Description("Air Handler")]
        AirHandler = 111177,

        [Description("Gas Furnace")]
        GasFurnace = 111178,

        [Description("Package Air Conditioner")]
        PackageAC = 111179,

        [Description("Packaged Heat Pump")]
        PackagedHP = 111180,

        [Description("Packaged Gas/Electric/Dual")]
        PackagedGED = 111181,

        [Description("Light Commercial Packaged Air Conditioner")]
        LightCommercialPackagedAC = 111182,

        [Description("Light Commercial Packaged Heat Pump")]
        LightCommercialPackagedHP = 111183,

        [Description("Light Commercial Packaged Gas/Electric")]
        LightCommercialPackagedGE = 111184,

        [Description("Mini-Split Air Conditioner")]
        MiniSplitAC = 111185,

        [Description("Mini-Split Heat Pump")]
        MiniSplitHP = 111186,

        [Description("Mini-Split Fan Coils")]
        MiniSplitFC = 111187,

        [Description("Mini-Split System")]
        MiniSplitSystem = 111188,

        [Description("Multi-Split Air Conditioner")]
        MultiSplitAC = 111189,

        [Description("Multi-Split Heat Pump")]
        MultiSplitHP = 111190,

        [Description("Multi-Split Fan Coil")]
        MultiSplitFC = 111191,

        [Description("Sky Air Air Conditioner")]
        SkyAirAC = 111192,

        [Description("Sky Air Heat Pump")]
        SkyAirHP = 111193,

        [Description("Sky Air Fan Coil")]
        SkyAirFC = 111194,

        [Description("Sky Air System")]
        SkyAirSystem = 111195,

        [Description("Altherma MonoBloc Heat Pump")]
        AlthermaMonoBlocHP = 111196,

        [Description("Altherma MonoBloc Heat Only")]
        AlthermaMonoBlocHeatOnly = 111197,

        [Description("Altherma Split Heat Pump")]
        AlthermaSplitHP = 111198,

        [Description("Altherma Split Heat Only")]
        AlthermaSplitHeatOnly = 111199,

        [Description("Altherma Fan Coil")]
        AlthermaFC = 111200,

        [Description("Altherma Water Tank")]
        AlthermaWaterTank = 111201,

        [Description("VRV Air Cooled Heat Pump")]
        VRVAirCooledHP = 111202,

        [Description("VRV Air Cooled Heat Recovery")]
        VRVAirCooledHR = 111203,

        [Description("VRV Fan Coil")]
        VRVFanCoil = 111204,

        [Description("VRV Water Cooled Heat Pump")]
        VRVWaterCooledHP = 111205,

        [Description("VRV Water Cooled Heat Recovery")]
        VRVWaterCooledHR = 111206,

        [Description("VRV Ventilation")]
        VRVVentilation = 111207,

        [Description("Adapter PCB")]
        AdapterPCB = 111208,

        [Description("Electric Heater")]
        ElectricHeater = 111209,

        [Description("Expansion Valve Kit")]
        ExpansionValveKit = 111210,

        [Description("Filters")]
        Filters = 111211,

        [Description("General Accessories")]
        GeneralAccessories = 111212,

        [Description("Installation Box")]
        InstallationBox = 111213,

        [Description("Piping Kit")]
        PipingKit = 111214,

        [Description("Branch Selector")]
        BranchSelector = 111215,

        [Description("Condensate Drain Kit")]
        CondensateDrainKit = 111216,

        [Description("Controllers")]
        Controllers = 1112117,

        [Description("Decoration Panel")]
        DecorationPanel = 111218,

        [Description("Sensor Kit")]
        SensorKit = 111219,

        [Description("Ventilation Kit")]
        VentilationKit = 111220,

        [Description("Packaged Air Conditioner")]
        PackagedAC = 112261


    }
}

