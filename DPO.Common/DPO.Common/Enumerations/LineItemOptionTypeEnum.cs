using System.ComponentModel;

namespace DPO.Common
{
    public enum LineItemOptionTypeEnum : byte
    {
        [Description("Base Model")]
        BaseModel = 1,

        [Description("Factory Installed")]
        FactoryInstalled = 2,

        [Description("Field Installed")]
        FieldInstalled = 3
    }
}
