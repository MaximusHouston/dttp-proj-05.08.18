//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace DPO.Common
{
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList()
        {

        }

        public PagedList(IEnumerable<T> source, int page, int pageSize)
            : this(source, page, pageSize, null)
        {
        }
        public PagedList(IEnumerable<T> source, ISearch search)
            : this(source, search.Page, search.PageSize, search.TotalRecords)
        {
        }

        public PagedList(IEnumerable<T> source, int? page, int? pageSize)
            : this(source, page, pageSize, null)
        {
        }

        public PagedList(IEnumerable<T> source, int? page, int? pageSize, int? totalCount)
        {
            int iPage = page.GetValueOrDefault(1);
            int iPageSize = pageSize.GetValueOrDefault(9);

            Initialize(source.AsQueryable(), iPage, iPageSize, totalCount);
        }

        public PagedList(IEnumerable<T> source, int page, int pageSize, int? totalCount)
        {
            Initialize(source, page, pageSize, totalCount);
        }

        public PagedList(IEnumerable<T> source)
        {
            Initialize(source, 1, DPO.Common.Constants.DEFAULT_USER_DISPLAYSETTINGS_PAGESIZE, null);
        }

        #region IPagedList Members

        public bool HasNextPage { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
        public int Page { get; private set; }
        public int PageCount { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRecords { get; set; }
        #endregion

        protected void Initialize(IEnumerable<T> source, int page, int pageSize, int? totalCount)
        {
            //### argument checking
            if (page < 1)
            {
                page = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }

            //### set source to blank list if source is null to prevent exceptions
            if (source == null)
            {
                source = new List<T>();
            }

            //### set properties
            TotalRecords = totalCount ?? source.Count();


            PageSize = pageSize;

            PageCount = (TotalRecords > 0) ? (int)Math.Ceiling(TotalRecords / (double)PageSize) : 1;

            Page = (page < 1) ? 1 : page;

            HasPreviousPage = (page > 1);
            HasNextPage = (page < PageCount + 1);
            IsFirstPage = (page <= 1);
            IsLastPage = (page >= PageCount + 1);

            //### add items to internal list
            base.AddRange(source);

        }
    }
}