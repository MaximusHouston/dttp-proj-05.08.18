using DPO.Common;
using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DPO.Web.Helpers
{
    public static partial class Extensions
    {

        public static MvcHtmlString DropDownProductPowerVoltagesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model)
        {
            return DropDownProductPowerVoltagesFor(htmlHelper, expression, model, null);
        }
        public static MvcHtmlString DropDownProductPowerVoltagesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            return DropDownProductPowerVoltagesFor(htmlHelper, expression, model, "Choose...", "", htmlAttributes);
        }
        public static MvcHtmlString DropDownProductPowerVoltagesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, string optionLabel, string emptyText, object htmlAttributes)
        {
            return htmlHelper.DropDownListFor(expression, model, optionLabel, emptyText, htmlAttributes);
        }
    }    
}