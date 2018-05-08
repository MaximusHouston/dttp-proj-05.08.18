using DPO.Common;
using DPO.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DPO.Web.Helpers
{
    //comment
    public static partial class Extensions
    {
        public static bool IsPage(this HtmlHelper htmlHelper, string page) 
        {
            return (string.Compare(htmlHelper.ViewContext.RouteData.Values["action"].ToString(), page, true) == 0);
        }
    }
}