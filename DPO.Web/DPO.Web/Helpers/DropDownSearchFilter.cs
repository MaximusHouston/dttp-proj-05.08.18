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

        public static MvcHtmlString DropDownSearchFilterFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            return htmlHelper.DropDownListFor(expression, model, "SHOWALLOPTION", "None found", htmlAttributes);
        }

        public static MvcHtmlString DropDownSearchFilterFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes, string ShowOption)
        {
            return htmlHelper.DropDownListFor(expression, model, ShowOption, "None found", htmlAttributes);
        }

        public static MvcHtmlString DropDownSearchFilterProjectLeadTypesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            // Allow search drop down to select all project lead types
            foreach(var item in model.Items)
            {
                item.Disabled = false;
            }
            return htmlHelper.DropDownListFor(expression, model, "SHOWALLOPTION", "None found", htmlAttributes);
        }

        public static MvcHtmlString DropDownSearchFilterProjectDarComTypesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            // Allow search drop down to select all project lead types
            foreach (var item in model.Items)
            {
                item.Disabled = false;
            }
            return htmlHelper.DropDownListFor(expression, model,null,null, htmlAttributes);
        }
    }


	
}