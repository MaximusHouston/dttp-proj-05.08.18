using System.ComponentModel;

namespace DPO.Common
{
    public enum InstallationConfigurationTypeEnum : int
    {
        None,
        [Description("Upflow/Horizontal")]
        UpflowHorizontal = 111002,

        [Description("Upflow/Downflow/Horizontal")]
        UpflowDownflowHorizontal = 111001,

        [Description("Downflow/Horizontal")]
        DownflowHorizontal = 110999,

        [Description("Dedicated Downflow")]
        DedicatedDownflow = 111000,

        [Description("Horizontal Right")]
        HorizontalRight = 111003,

        [Description("Horizontal Left")]
        HorizontalLeft = 111004,

        [Description("Vertical")]
        Vertical = 111005

    }
}
