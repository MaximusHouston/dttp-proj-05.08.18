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

        //public static MvcHtmlString NumericStepperFor3<TModel, TProperty>(this HtmlHelper htmlHelper, Expression<Func<TModel, TProperty>> expression, TModel source,int i, object htmlAttributes = null)
        //{
        //    TagBuilder stepper = new TagBuilder("div");

        //    var quantity = ((LambdaExpression)expression).Compile().DynamicInvoke(source);

        //    string textBoxName = "DiscountRequest[" + i + "]" + ".RequestDiscount";

        //    //stepper.InnerHtml = String.Format("<button type='button' class='minus' id='stepDecrease'><img src='/Images/numeric-stepper-minus-icon.png'/></button>" +
        //    //                                 htmlHelper.TextBox(textBoxName, quantity, new { @class = "numbers", @id = i }) +
        //    //                                 "<button type='button' class='plus' id='stepIncrease'><img src='/Images/numeric-stepper-plus-icon.png' /></button>");

        //    stepper.InnerHtml=  String.Format("<button type='button' class='minus'  id='stepDecrease'>" +
        //                 "<img src='/Images/numeric-stepper-minus-icon.png'/></button>" +
        //                  //htmlHelper.TextBox(textBoxName, quantity, new { @class = "numbers", @id = i }) +
                          
        //                 "<button type='button' class='plus' id='stepIncrease'>" +
        //                 "<img src='/Images/numeric-stepper-plus-icon.png' /></button>");

        //    if (htmlAttributes != null)
        //    {
        //        var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        //        stepper.MergeAttributes(attributes, true);
        //    }

        //    stepper.AddCssClass("numeric-stepper");

        //    return new MvcHtmlString(stepper.ToString());

        //}

        public static MvcHtmlString NumericStepperFor3<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, TModel source, int i, object htmlAttributes = null)
        {
            TagBuilder stepper = new TagBuilder("div");

            var quantity = ((LambdaExpression)expression).Compile().DynamicInvoke(source);

            string textBoxName = "DiscountRequest[" + i + "]" + ".RequestDiscount";

            //stepper.InnerHtml = String.Format("<button type='button' class='minus' id='stepDecrease'><img src='/Images/numeric-stepper-minus-icon.png'/></button>" +
            //                                 htmlHelper.TextBox(textBoxName, quantity, new { @class = "numbers", @id = i }) +
            //                                 "<button type='button' class='plus' id='stepIncrease'><img src='/Images/numeric-stepper-plus-icon.png' /></button>");

            stepper.InnerHtml = String.Format("<button type='button' class='minus'  id='stepDecrease' style='margin-right:3px;'>" +
                         "<img src='/Images/numeric-stepper-minus-icon.png'/></button>" +
                //htmlHelper.TextBox(textBoxName, quantity, new { @class = "numbers", @id = i }) +
                   htmlHelper.TextBoxFor(expression, new { @class = "numbers" }) +
                         "<button type='button' class='plus' id='stepIncrease' style='margin-left:3px;'>" +
                         "<img src='/Images/numeric-stepper-plus-icon.png' /></button>");

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                stepper.MergeAttributes(attributes, true);
            }

            stepper.AddCssClass("numeric-stepper");

            return new MvcHtmlString(stepper.ToString());

        }
        

    }
}