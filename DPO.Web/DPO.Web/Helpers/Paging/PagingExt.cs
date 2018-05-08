using DPO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections;

namespace DPO.Web.Helpers
{
    public static class PagingExt
    {
        #region HtmlHelper extensions

        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, ISearch search, object valuesDictionary, bool usePost = false)
        {
            return Pager(htmlHelper, search.PageSize, search.Page, search.TotalRecords, valuesDictionary, usePost);
        }

        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, ISearch search, bool usePost = false)
        {
            return Pager(htmlHelper, search, null, usePost);
        }

        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, int? pageSize, int? page, int? totalRecords, bool usePost = false)
        {
            return Pager(htmlHelper, pageSize.Value, page.Value, totalRecords.Value, null, usePost);
        }


        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, int? pageSize, int? page, int? totalRecords, object valuesDictionary, bool usePost = false)
        {
            
            var pager = new Pager(htmlHelper, pageSize, page, totalRecords, valuesDictionary, usePost);
            return pager.RenderHtml();
        }

        #endregion

        #region IQueryable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion

        #region IEnumerable<T> extensions

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }

        #endregion
    }
}