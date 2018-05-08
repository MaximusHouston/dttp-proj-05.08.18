using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DPO.Web.Helpers
{
    public partial class Extensions
    {

        /// <summary>
        /// http://stackoverflow.com/questions/1488890/asp-net-mvc-partial-views-input-name-prefixes
        /// @Html.PartialFor(model => model.Child, "_AnotherViewModelControl")
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="partialViewName"></param>
        /// <returns></returns>
        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression, string partialViewName)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;

            string oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix; 
            if (oldPrefix != "") name = oldPrefix + "." + name;

            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo
                {
                    HtmlFieldPrefix = name
                }
            };

            return helper.Partial(partialViewName, model, viewData);
        }
    }
}