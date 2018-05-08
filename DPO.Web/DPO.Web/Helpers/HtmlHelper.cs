using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using System.Reflection;

namespace DPO.Web.Helpers
{
    public static partial class Extensions
    {

        public static object MergeObjects(this HtmlHelper htmlHelper, params object[] objects)
        {
            dynamic expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;

            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    continue;
                }

                foreach (PropertyInfo fi in obj.GetType().GetProperties())
                {
                    result[fi.Name] = fi.GetValue(obj, null);
                }

                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    result[fi.Name] = fi.GetValue(obj);
                }
            }

            return result;
        }
    }
}