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
        public static MvcHtmlString PageMessages(this HtmlHelper htmlHelper, bool showKeyMessages = false)
        {
            return htmlHelper.PageMessages(null, showKeyMessages);
        }
        public static MvcHtmlString PageMessages(this HtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes, bool showKeyMessages)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            var messages = htmlHelper.ViewData["PageMessages"] as Messages;
            var keyMessages = htmlHelper.ViewData["KeyMessages"] as Messages;

            if (showKeyMessages && keyMessages != null)
            {
                if (messages == null)
                {
                    messages = new Messages();
                }
                foreach (var msg in keyMessages.Items)
                {
                    messages.Add(msg);
                }

                htmlHelper.ViewData["KeyMessages"] = null;
            }

            if (messages == null)
            {
                return null;
            }

            var messagehtml = "";

            bool emailMessageHtml = false;

            for (int i = 0; i < messages.Items.Count; i++ )
            {
                if(messages.Items[i].Text.Contains("CC Email"))
                {
                    emailMessageHtml = true;
                }
            }

            if (!emailMessageHtml)
            {
                foreach (IMessage message in messages.Items)
                {
                    TagBuilder divBuilder = new TagBuilder("div");

                    divBuilder.MergeAttributes(htmlAttributes);

                    switch (message.Type)
                    {
                        case MessageTypeEnum.Critial:
                            divBuilder.AddCssClass("pagemessage-critical");
                            break;
                        case MessageTypeEnum.Error:
                            divBuilder.AddCssClass("pagemessage-error");
                            break;
                        case MessageTypeEnum.Warning:
                            divBuilder.AddCssClass("pagemessage-warning");
                            break;
                        case MessageTypeEnum.Information:
                            divBuilder.AddCssClass("pagemessage-information");
                            break;
                        case MessageTypeEnum.Success:
                            divBuilder.AddCssClass("pagemessage-success");
                            break;
                        default:
                            break;
                    }


                    divBuilder.InnerHtml = message.Text;

                    messagehtml += divBuilder.ToString();

                }
            }
            else
            {
                for (int i = 0; i < messages.Items.Count; i++)
                {
                    TagBuilder divBuilder = new TagBuilder("div");

                    divBuilder.MergeAttributes(htmlAttributes);

                    switch (messages.Items[i].Type)
                    {
                        case MessageTypeEnum.Critial:
                            divBuilder.AddCssClass("pagemessage-critical");
                            break;
                        case MessageTypeEnum.Error:
                            divBuilder.AddCssClass("pagemessage-error");
                            break;
                        case MessageTypeEnum.Warning:
                            divBuilder.AddCssClass("pagemessage-warning");
                            break;
                        case MessageTypeEnum.Information:
                            divBuilder.AddCssClass("pagemessage-information");
                            break;
                        case MessageTypeEnum.Success:
                            divBuilder.AddCssClass("pagemessage-success");
                            break;
                        default:
                            break;
                    }

                    if (messages.Items.Count > 2)
                    {
                        if (i < messages.Items.Count - 1)
                        {
                            divBuilder.InnerHtml = messages.Items[i].Text;
                            messagehtml += divBuilder.ToString();
                        }
                    }
                    else
                    {
                        if( i < messages.Items.Count)
                        {
                            divBuilder.InnerHtml = messages.Items[i].Text;
                            messagehtml += divBuilder.ToString();
                        }
                    }
                   
                }
            }

            return new MvcHtmlString(messagehtml);

        }

    }
}