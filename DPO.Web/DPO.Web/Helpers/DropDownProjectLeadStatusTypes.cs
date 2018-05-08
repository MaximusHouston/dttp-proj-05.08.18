using DPO.Common;
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
    public static partial class Extensions
    {
        public static MvcHtmlString DropDownProjectLeadStatusTypeListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model)
        {
            return DropDownProjectTypeListFor(htmlHelper, expression, model, null);
        }

        public static MvcHtmlString DropDownProjectLeadStatusTypeListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            TProperty propVal = expression.Compile()(htmlHelper.ViewData.Model);

            return htmlHelper.DropDownListFor(expression, model , "Choose....", "No project lead statuses found", htmlAttributes, true);
        }
    }
}