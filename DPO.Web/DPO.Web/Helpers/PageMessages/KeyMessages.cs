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
        public static MvcHtmlString KeyMessages<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.KeyMessages<TModel, TProperty>(expression,null);
        }
        public static MvcHtmlString KeyMessages<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            var messages = htmlHelper.ViewData["KeyMessages"] as Messages;


            if (messages == null)
            {
                return null;
            }

            var messagehtml = "";

            var id = htmlHelper.NameFor(expression).ToString();

            foreach (IMessage message in messages.Items)
            {
               if (string.Compare(message.Key,id,true) == 0)
               {
                    TagBuilder divBuilder = new TagBuilder("span");

                    divBuilder.Attributes.Add("id", "keymessage-" + id);

                    divBuilder.MergeAttributes(htmlAttributes);

                    divBuilder.AddCssClass("input-validation-error");

                    divBuilder.InnerHtml = message.Text;

                    messagehtml += divBuilder.ToString();
                      
                }
                
            }

            return new MvcHtmlString(messagehtml);
            
        }

    }
}