using System;
using System.ComponentModel;

namespace DPO.Common
{
    public static class EnumsHelper
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                var attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;
                return attribute == null ? value.ToString() : attribute.Description;
            }
            else
            {
                return "N/A";
            }
        }

    }
}

