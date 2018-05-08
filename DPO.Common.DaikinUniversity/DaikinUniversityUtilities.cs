using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common.DaikinUniversity
{
    public static class DaikinUniversityUtilities
    {
        private static string ConvertObjectToString(object value)
        {
            if (value == null)
                return String.Empty;

            var objType = value.GetType();

            if (Nullable.GetUnderlyingType(objType) != null)
            {
                return ConvertObjectToString(value);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objType) && typeof(string) != objType)
            {
                var list = value as IEnumerable;

                StringBuilder sbList = new StringBuilder();
                foreach (var item in list)
                {
                    if (item == null)
                        continue;

                    string itemVal = ConvertObjectToString(item);

                    sbList.Append(item.ToString().Trim()).Append(",");
                }

                return sbList.ToString().Trim(',');
            }
            else if (objType == typeof(bool))
            {
                return value.ToString().ToLower();
            }
            else if (objType == typeof(DateTime))
            {
                DateTime utcDate = ((DateTime)value).ToUniversalTime();
                if (utcDate <= DateTime.MinValue)
                {
                    return String.Empty;
                }

                return utcDate.ToString("yyyy-MM-ddTHH:mm:ss.000");
            }
            else if (objType == typeof(Enum))
            {
                Enum enumVal = (Enum)value;

                return Enum.GetName(enumVal.GetType(), enumVal);

            }
            else
            {
                return value.ToString();
            }
        }

        public static string ConvertObjectToQueryString(object o)
        {
            if (o == null)
                return String.Empty;

            StringBuilder queryString = new StringBuilder();

            var props = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                object value = prop.GetValue(o);

                var stringVal = ConvertObjectToString(value);
                if (String.IsNullOrWhiteSpace(stringVal))
                    continue;

                queryString.Append(prop.Name).Append("=")
                    .Append(stringVal).Append("&");
            }

            return queryString.ToString().Trim('&');
        }
    }
}
