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
   
        /// <summary>
        /// Custom HiddenFor that addresses the issues noted here:
        /// http://stackoverflow.com/questions/594600/possible-bug-in-asp-net-mvc-with-form-values-being-replaced
        /// We will only ever want values pulled from the model passed to the page instead of 
        /// pulling from modelstate.  
        /// Note, do not use 'ValueFor' in this method for these reasons.
        /// </summary>
        public static IHtmlString HiddenFromModelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                    Expression<Func<TModel, TProperty>> expression,
                                                    object value = null,
                                                    bool withValidation = false)
        {
            if (value == null)
            {
                value = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;
            }

            return new HtmlString(String.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                                    htmlHelper.IdFor(expression),
                                    htmlHelper.NameFor(expression),
                                    value));
        }

    }
}