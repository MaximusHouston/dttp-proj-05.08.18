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
    public partial class Extensions
    {
        
        public static IHtmlString Concurrency(this HtmlHelper htmlHelper)
        {
            var concurrency = htmlHelper.ViewData.Model as IConcurrency;

            if (concurrency != null)
            {
                return new HtmlString(String.Format("<input type='hidden' id='Concurrency' name='Concurrency' value='{0}' />",
                                   concurrency.Timestamp.Ticks));
            }
            return null;
        }
    }
}