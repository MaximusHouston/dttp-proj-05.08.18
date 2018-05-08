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
        public static MvcHtmlString DropDownCountryListFor<TModel, TProperty, TProperty2>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> countryElementId, Expression<Func<TModel, TProperty2>> stateElementId, DropDownModel model)
        {
            return DropDownCountryListFor(htmlHelper, countryElementId, stateElementId, model, null);
        }
        public static MvcHtmlString DropDownCountryListFor<TModel, TProperty, TProperty2>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> countryElementId, Expression<Func<TModel, TProperty2>> stateElementId, DropDownModel model, object htmlAttributes)
        {
            string countryCode = ExpressionHelper.GetExpressionText(countryElementId);
            string stateId = ExpressionHelper.GetExpressionText(stateElementId);

            object attributes = Utilities.Merge(htmlAttributes, new { onchange = "CountrySelected('" + countryCode + "','" + stateId + "');" });

            return htmlHelper.DropDownListFor(countryElementId, model, "Choose....", "No countries found", attributes);
        }

    }


	
}