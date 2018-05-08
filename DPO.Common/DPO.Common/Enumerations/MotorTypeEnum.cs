using System.ComponentModel;

namespace DPO.Common
{
    public enum MotorTypeEnum : int
    {
        None,

        [Description("Fixed Speed ECM")]
        FixedSpeedECM = 112920, 

        [Description("PSC")]
        PSC = 112921,

        [Description("Variable")]
        Variable = 112923,

        [Description("Variable Speed ECM")]
        VariableSpeedECM = 112924,

        [Description("Multi-Speed ECM")]
        MultiSpeedECM = 112922,

        [Description("Single Speed")]
        SingleSpeed = 112925

    }
}
