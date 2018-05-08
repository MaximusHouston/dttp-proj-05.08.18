using DPO.Common;
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

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, string optionLabel, string emptyText)
        {
            return DropDownListFor(htmlHelper, expression, model, optionLabel, emptyText, null);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, DropDownModel model, string optionLabel, string emptyText, object htmlAttributes, bool disableIfSelectedItemDisabled = false)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            var isSelectedItemDisabled = false;

            if (model != null && model.Items != null && expression != null)
            {
                object val = expression.Compile()(htmlHelper.ViewData.Model);

                if (val is Enum)
                {
                    Type enumType = typeof(TProperty);
                    Type nullableType = Nullable.GetUnderlyingType(enumType);

                    enumType = nullableType ?? enumType;

                    val = Convert.ChangeType((Enum)val, Enum.GetUnderlyingType(enumType));
                }

                if (val != null)
                {
                    foreach (var item in model.Items)
                    {
                        item.Selected = false;
                        var itemVal = item.Value;
                        var itemText = item.Text;

                        if (String.Compare(itemVal, val.ToString(), true) == 0
                            || String.Compare(itemText, val.ToString(), true) == 0)
                        {
                            item.Selected = true;
                            isSelectedItemDisabled = item.Disabled;
                        }
                    }
                }
            }

            if (isSelectedItemDisabled
                && disableIfSelectedItemDisabled)
            {
                var obj = new
                {
                    disabled = "true"
                };

                if (htmlAttributes != null)
                {
                    htmlAttributes = htmlHelper.MergeObjects(htmlAttributes, obj);
                }
                else
                {
                    htmlAttributes = obj;
                }
            }

            return DropDownListFor(htmlHelper, name, model, optionLabel, emptyText, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor(this HtmlHelper htmlHelper, string expression, DropDownModel model, string optionLabel, string emptyText, object htmlAttributes)
        {
            string name = expression;

            if (model == null || model.Items == null || model.Items.Count() == 0)
            {
                TagBuilder tagBuilder = new TagBuilder("select")
                {
                    InnerHtml = ListItemToOption(
                        new SelectListItemExt()
                        {
                            Text = HttpUtility.HtmlEncode(emptyText),
                            Selected = true
                        })
                };

                string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

                tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
                tagBuilder.GenerateId(fullName);
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                tagBuilder.Attributes.Add(new KeyValuePair<string, string>("disabled", "true"));

                return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
            }

            return SelectInternal(htmlHelper, optionLabel, name, model, false /* allowMultiple */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        internal static string ListItemToOption(SelectListItemExt item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }

            if (item.Disabled)
            {
                builder.Attributes["disabled"] = "true";
            }

            if (!String.IsNullOrEmpty(item.DataRequirementLevel))
            {
                builder.Attributes["data-requirementlevel"] = item.DataRequirementLevel;
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));

            return builder.ToString(TagRenderMode.Normal);
        }

        static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        static dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
                return item1 ?? item2 ?? new ExpandoObject();

            dynamic expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item1, null);
            }
            foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item2, null);
            }
            return result;
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, DropDownModel model, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (String.IsNullOrEmpty(fullName))
                throw new ArgumentException("No name");

            var selectList = model.Items;

            bool isListEmpty = (selectList == null || selectList.Count() == 0);

            object defaultValue = (allowMultiple) ? GetModelStateValue(htmlHelper, fullName, typeof(string[])) : GetModelStateValue(htmlHelper, fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (defaultValue == null)
                defaultValue = htmlHelper.ViewData.Eval(fullName);

            //if (defaultValue != null && !isListEmpty)
            //{
            //    IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };

            //    IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);

            //    HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

            //    List<SelectListItemExt> newSelectList = new List<SelectListItemExt>();

            //    foreach (SelectListItemExt item in selectList)
            //    {
            //        item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);

            //        newSelectList.Add(item);
            //    }

            //    selectList = newSelectList;
            //}

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null && selectList.Count > 1)
            {
                if (optionLabel == "SHOWALLOPTION")
                {
                    listItemBuilder.Append(ListItemToOption(new SelectListItemExt() { Text = "Show All...", Value = "", Selected = true, Disabled = false }));
                }
                else
                {
                    listItemBuilder.Append(ListItemToOption(new SelectListItemExt() { Text = optionLabel, Value = "", Selected = !selectList.Any(w => w.Selected), Disabled = true, HtmlAttributes = new { style = "display:none" } }));
                }
            }

            foreach (SelectListItemExt item in selectList)
            {
                listItemBuilder.Append(ListItemToOption(item));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
                tagBuilder.MergeAttribute("multiple", "multiple");

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }



}