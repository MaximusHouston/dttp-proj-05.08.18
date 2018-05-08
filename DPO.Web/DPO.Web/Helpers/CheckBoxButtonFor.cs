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
        public static IHtmlString CheckBoxButtonFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                    Expression<Func<TModel, bool>> expression,
                                                    object value = null,
                                                    bool withValidation = false)
        {
            var input = htmlHelper.CheckBoxFor(expression, new { @class = "cb-switch" });
            var label = String.Format("<label for='{0}' class='cb-switch-label'></label>",htmlHelper.NameFor(expression));

            return new HtmlString(input.ToString() + label.ToString());
        }

    }
}