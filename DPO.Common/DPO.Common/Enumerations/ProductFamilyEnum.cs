using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public enum ProductFamilyEnum : int
    {
        Other = 1,

        #region Delete after 10/01/2017
        //MiniSplit = 100000000,
        //Altherma = 100000001,
        //MultiSplit = 100000002,
        //SkyAir = 100000003,
        //VRV = 100000004,
        //RTU = 100000005,
        //Packaged = 100000006,
        //PTAC = 100000007,
        //Ventilation = 100000008,
        //IAQ = 100000009,
        ////Unitary = 100000011,
        //UnitarySplit = 100000012,
        //UnitaryPackage = 100000013,
        //CommercialSplit = 100000014,
        #endregion 

        Accessories = 2,

        //New Values
        UnitarySplitSystem = 111521,
        UnitaryPackagedSystem = 111522,
        MiniSplit = 111523,
        MultiSplit = 111524,
        SkyAir = 111525,
        VRV = 111526,
        AlthermaSplit = 111527,
        AlthermaMonobloc = 111528,
        LightCommercialSplitSystem = 111529,
        LightCommercialPackagedSystem = 111530
    }
}
