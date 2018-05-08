using System.ComponentModel;

namespace DPO.Common
{
    public enum GasValveTypeEnum : int
    {
        None,
        [Description("Modulating")]
        Modulating = 110991,

        [Description("Two Stage")]
        TwoStage = 110992,

        [Description("Two-Stage Convertible")]
        TwoStageConvertible = 110990,

        [Description("Single Stage")]
        SingleStage = 110993,

        [Description("Single Stage Convertible")]
        SingleStageConvertible = 110989

    }
}
