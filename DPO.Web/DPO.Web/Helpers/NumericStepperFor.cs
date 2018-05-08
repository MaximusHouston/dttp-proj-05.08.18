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
    public static partial class Extensions
    {
        // integer values
        public static MvcHtmlString NumericStepperFor(this HtmlHelper htmlHelper, long? value, object htmlAttributes = null)
        {
            return NumericStepperFor(htmlHelper, (float?)value, htmlAttributes);
        }

        ////decimal values
        public static MvcHtmlString NumericStepperFor(this HtmlHelper htmlHelper, float? value, object htmlAttributes = null)
        {
            TagBuilder stepper = new TagBuilder("div");

            RouteValueDictionary attributes = new RouteValueDictionary();

            if (htmlAttributes != null)
            {
                attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            }

            //if (value == null)
            //{
            //    value = 0;
            //}

            TagBuilder input = new TagBuilder("input");
            input.Attributes.Add("value", value.ToString());
            input.AddCssClass("numbers");

            if (attributes.ContainsKey("class"))
            {
                var classVal = attributes["class"];
                input.AddCssClass(classVal != null ? classVal.ToString() : String.Empty);
            }

            stepper.InnerHtml = String.Format(@"
                <button type='button' class='minus'><img src='/Images/numeric-stepper-minus-icon.png'/></button>
                {0}
                <button type='button' class='plus'><img src='/Images/numeric-stepper-plus-icon.png' /></button>", input.ToString());

            stepper.MergeAttributes(attributes, true);

            stepper.AddCssClass("numeric-stepper");

            return new MvcHtmlString(stepper.ToString());
        }

        //public static MvcHtmlString NumericStepperFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        //{
        //    return NumericStepperFor(htmlHelper, expression, null);
        //}
        //public static MvcHtmlString NumericStepperFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, object htmlAttributes)
        //{
        //    TagBuilder stepper = new TagBuilder("div");

        //    stepper.InnerHtml = String.Format("<button type='button' class='minus'><img src='/Images/numeric-stepper-minus-icon.png'/></button> <input type='text' class='numbers' value='{0}'/> <button type='button' class='plus'><img src='/Images/numeric-stepper-plus-icon.png' /></button>", expression);

        //    if (htmlAttributes != null)
        //    {
        //        var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        //        stepper.MergeAttributes(attributes, true);
        //    }

        //    stepper.AddCssClass("numeric-stepper");

        //    return new MvcHtmlString(stepper.ToString());
        //}

    }
}