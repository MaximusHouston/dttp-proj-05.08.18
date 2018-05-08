using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DPO.Common
{
    public enum VerticalMarketTypeEnum : byte
    {
        [Description("Assisted Living")]
        AssistedLiving = 1,

        [Description("Bank")]
        Bank = 2,

        [Description("Church")]
        Church = 3,

        [Description("Commercial (Other)")]
        CommercialOther = 4,

        //[Description("Condominium")]
        //Condominium = 5,

        [Description("Fire/Police Stations")]
        FirePoliceStations = 6,

        [Description("Fitness Center")]
        FitnessCenter = 7,

        [Description("Government")]
        Government = 8,

        [Description("Healthcare")]
        Healthcare = 9,

        [Description("Hotel")]
        Hotel = 10,

        [Description("Military")]
        Military = 11,

        [Description("Office")]
        Office = 12,

        //[Description("Residential")]
        //Residential = 13,

        [Description("Restaurant")]
        Restaurant = 14,

        [Description("School")]
        School = 15,

        [Description("Shop/retail")]
        ShopRetail = 16,

        [Description("Theater")]
        Theater = 17,

        [Description("Single Family Homes")]
        SingleFamilyHomes = 18,

        [Description("Multifamily")]
        Multifamily = 19,

        [Description("Dormitories")]
        Dormitories = 20
    }
}

