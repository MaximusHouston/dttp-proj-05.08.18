using System.ComponentModel;
using System;

namespace DPO.Common
{

    // TODO:  Do we need to use variable?
    public enum CompressorTypeEnum : int
    {
        None,
        [Description("Standard"), System.Obsolete("Use SingleStage instead of Standard", false)]
        Standard = 110966,

        [Description("Inverter")]
        Inverter = 110965,

        [Description("Two Stage")]
        Stage = 110964,

        [Description("Single Stage")]
        SingleStage = 110966,

        [Description("Variable")]
        Variable = 110967


    }
}
