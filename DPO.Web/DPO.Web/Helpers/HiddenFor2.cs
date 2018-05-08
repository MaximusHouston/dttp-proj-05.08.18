using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DPO.Web.Helpers
{
    public partial class Extensions
    {
        /// <summary>
        /// Prevents ModelState (Posted Values) from overriding actual Model values on postback
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString HiddenFor2<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ReplacePropertyState(htmlHelper, expression);

            return htmlHelper.HiddenFor(expression);
        }

        /// <summary>
        /// Prevents ModelState (Posted Values) from overriding actual Model values on postback
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString HiddenFor2<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            ReplacePropertyState(htmlHelper, expression);
            return htmlHelper.HiddenFor(expression, htmlAttributes);
        }

        /// <summary>
        /// Prevents ModelState (Posted Values) from overriding actual Model values on postback
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString HiddenFor2<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            ReplacePropertyState(htmlHelper, expression);
            return htmlHelper.HiddenFor(expression, htmlAttributes);
        }

        private static void ReplacePropertyState<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            string text = ExpressionHelper.GetExpressionText(expression);
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(text);
            ModelStateDictionary modelState = htmlHelper.ViewContext.ViewData.ModelState;

            if (modelState.ContainsKey(fullName))
            {
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                ValueProviderResult currentValue = modelState[fullName].Value;
                modelState[fullName].Value = new ValueProviderResult(metadata.Model, Convert.ToString(metadata.Model), currentValue.Culture);
            }
        }
    }
}