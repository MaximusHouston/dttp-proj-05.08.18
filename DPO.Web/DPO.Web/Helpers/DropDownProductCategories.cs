using System;
using DPO.Common;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DPO.Web.Helpers
{
    public partial class Extensions
    {
        public static MvcHtmlString DropDownProductCategoriesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model)
        {
            return DropDownProductCategoriesFor(htmlHelper, expression, model, null);
        }
        public static MvcHtmlString DropDownProductCategoriesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        {
            return DropDownProductCategoriesFor(htmlHelper, expression, model, "Choose...", "", htmlAttributes);
        }
        public static MvcHtmlString DropDownProductCategoriesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, string optionLabel, string emptyText, object htmlAttributes)
        {
            return htmlHelper.DropDownListFor(expression, model, optionLabel, emptyText, htmlAttributes);
        }
    }
}