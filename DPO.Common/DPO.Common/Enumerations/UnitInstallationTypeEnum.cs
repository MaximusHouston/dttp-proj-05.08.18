using System.ComponentModel;
using System;

namespace DPO.Common
{
    public enum UnitInstallationTypeEnum: int
    {
        None,
        [Description("Other")]
        Other = 1,
        
        [Description("All")]
        All = 100000999,

        [Description("Air Handler"), Obsolete("No longer used", false)]
        AirHandler = 100000000,

        [Description("Evaporator Coil"), Obsolete("No longer used", false)]
        EvaporatorCoil = 100000001,

        [Description("Package AC"), Obsolete("No longer used", false)]
        PackageAC = 100000002,

        [Description("Package HP"), Obsolete("No longer used", false)]
        PackageHP = 100000003,

        [Description("Package D.F."), Obsolete("No longer used", false)]
        PackageDF = 100000004,

        [Description("Package G.E."), Obsolete("No longer used", false)]
        PackageGE = 100000005,

        [Description("Wall Mounted")]
        WallMounted = 111006,

        [Description("Ceiling Suspended")]
        CeilingSuspended = 111008,

        [Description("Ducted")]
        Ducted = 111007,

        [Description("Floor Standing")]
        FloorStanding = 111010,

        // TODO:  We should name this simply Cassette
        [Description("Ceiling Cassette")]
        CeilingCassette = 111009,

        [Description("Gas Furnace"), Obsolete("No longer used", false)]
        GasFurnace = 100000156,

        [Description("Cased Coil"), Obsolete("No longer used", false)]
        CasedCoil = 100000157,

        [Description("Coil Only"), Obsolete("No longer used", false)]
        CoilOnly = 100000158,

        [Description("Cooling Only"), Obsolete("No longer used", false)]
        CoolingOnly = 100000301,

        [Description("Heat Pump"), Obsolete("No longer used", false)]
        HeatPump = 100000302,

        [Description("Heat Recovery"), Obsolete("No longer used", false)]
        HeatRecovery = 100000303,

        [Description("Air Conditioner"), Obsolete("No longer used", false)]
        AirConditioner = 100000304,
             
        [Description("Dual Floor / Ceiling Suspended")]
        DualFloorCeilingSuspended = 111011,

        [Description("Rooftop")]
        Rooftop = 111012

    }
}
