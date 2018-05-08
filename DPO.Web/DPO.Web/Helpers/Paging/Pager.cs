using System;
using System.Text;
using System.Linq;
using System.Collections;

using System.Web.Mvc;
using System.Web.Routing;
using DPO.Web.Helpers;
using DPO.Resources;

namespace DPO.Web.Helpers
{
    public class Pager
    {
        private UrlHelper url;
        private readonly int pageSize;
        private readonly int page;
        private readonly int totalRecords;
        private readonly RouteValueDictionary routingValues;
        private readonly int recordStart;
        private readonly int recordEnd;
        private readonly bool usePost;

        public Pager(HtmlHelper htmlHelper, int? pageSize, int? page, int? totalRecords, object routeValues, bool usePost = false)
        {
            pageSize = pageSize.GetValueOrDefault(1);
            page = page.GetValueOrDefault(1);
            totalRecords = totalRecords.GetValueOrDefault(0);

            url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            this.pageSize = pageSize.Value;
            this.page = page.Value;
            this.totalRecords = totalRecords.Value;
            this.recordStart = (this.page - 1) * this.pageSize + 1;
            this.recordEnd = this.recordStart + this.pageSize - 1;
            this.usePost = usePost;

            if (this.recordStart >= this.recordEnd)
            {
                this.recordStart = 1;
            }
            if (this.recordEnd > totalRecords)
            {
                this.recordEnd = totalRecords.Value;
            }

            routingValues = new RouteValueDictionary(routeValues);
            foreach (var v in htmlHelper.ViewContext.Controller.ControllerContext.RouteData.Values)
            {
                routingValues.Add(v.Key, v.Value);
            }

            var queryString = htmlHelper.ViewContext.HttpContext.Request.QueryString;

            var allKeys = queryString.AllKeys.ToList();
            allKeys.ForEach(k =>
                {
                    if (!routingValues.Any(r => r.Key.ToLower() == k.ToLower()))
                    {
                        routingValues.Add(k, queryString[k]);
                    }
                });

        }

        public MvcHtmlString RenderHtml()
        {
            int pageCount = this.totalRecords / this.pageSize;

            if (this.totalRecords % this.pageSize != 0) // if there is a remainder add one to page
            {
                pageCount += 1;
            }
            int noPagesToDisplay = 5; // must be odd

            var sb = new StringBuilder();

            int middle = noPagesToDisplay / 2;

            int startshow = this.page - middle;
            int endshow = this.page + middle;

            if (startshow < 1)
            {
                startshow = 1;
                endshow = noPagesToDisplay;
            }

            if (endshow > pageCount)
            {
                endshow = pageCount;
                startshow = endshow - noPagesToDisplay;
                if (startshow < 1) startshow = 1;
            }

            if (this.totalRecords == 0)
            {
                sb.Append("<div class='ui-widget-content pager pager-norecords'>No records found");
            }
            else
            {
                if (this.totalRecords <= this.pageSize)
                {
                    sb.Append(string.Format("<div class='pager'>Total number of records: {0}", this.totalRecords));
                }
                else
                {
                    sb.Append(string.Format("<div class='pager'>Showing {0} to {1} from a total of {2} ", this.recordStart, this.recordEnd, this.totalRecords));
                }
            }

            string routingName = "page";

            if (this.routingValues.ContainsKey("PageRouteName"))
            {
                if (!string.IsNullOrEmpty(this.routingValues["PageRouteName"] + ""))
                {
                    routingName = this.routingValues["PageRouteName"] + "";

                    this.routingValues.Remove("PageRouteName");
                }
            }


            for (int i = 1; i <= pageCount && pageCount > 1; i++)
            {
                if (i == this.page)
                {
                    sb.Append(GeneratePageLink("pager-page pager-current pager-highlight", i, false, false, routingName));
                }
                else if (i == pageCount)
                {
                    sb.Append(GeneratePageLink("pager-page pager-last", i, false, true, routingName));
                }
                else if (i == 1)
                {
                    sb.Append(GeneratePageLink("pager-page pager-first", i, false, true, routingName));
                }
                else if (i < startshow)
                {

                    sb.Append(GeneratePageLink("pager-page pager-firstgap", (startshow + 1) / 2, true, true, routingName));
                    i = startshow - 1;
                }
                else if (i > endshow)
                {

                    sb.Append(GeneratePageLink("pager-page pager-lastgap", endshow + (pageCount + 1 - i) / 2, true, true, routingName));
                    i = pageCount - 1;
                }
                else
                {
                    sb.Append(GeneratePageLink("pager-page", i, false, true, routingName));
                }
            }

            sb.Append("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        protected virtual string GeneratePageLink(string classes, int page, bool isGap, bool isLink, string pageRouteName)
        {

            if (!isLink)
            {
                return String.Format("<span class='" + classes + "'>{0}</span>", page);
            }


            this.routingValues[pageRouteName] = page;

            var virtualPathData = this.url.RouteUrl(this.routingValues);

            if (virtualPathData != null)
            {
                if (!usePost) { 
                    return String.Format("<a class='" + classes + "' href=\"{0}\" target='_self' title='" + ResourceUI.GoToPage + " {1}' >{2}</a>", virtualPathData, page, (isGap) ? "..." : page.ToString());
                }
                else
                {
                    return String.Format(@"<a id=""Page{0}"" class=""{1}"" href=""#"">{0}</a>
                        <script>
                            $('#Page{0}').on('click', function(event) {{
                                $('#Page').val({0});
                                $(this).parents('form').submit();
                                event.preventDefault();
                                return false;
                            }});
                        </script>",
                        (isGap) ? "Gap" : page.ToString(),
                        classes,
                        (isGap) ? "..." : page.ToString());
                }
            }
            else
            {
                return String.Format("<span class='" + classes + "'>{0}</span>", (isGap) ? "..." : page.ToString());
            }
        }

    }
}