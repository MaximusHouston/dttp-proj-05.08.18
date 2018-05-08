//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
//
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DPO.Common
{
    [Serializable]
    [DebuggerDisplay("Page={Page} PageSize={PageSize} TotalRecords={TotalRecords} SortColumn={SortColumn} IsDesc={IsDesc} Filter={Filter}")]
    public class Search : PageModel, ISearch
    {
        public Search()
            : base()
        {
            Page = 1;
            PageSize = Common.Constants.DEFAULT_USER_DISPLAYSETTINGS_PAGESIZE;
            TotalRecords = 0;
            PageSizes = new List<int> { 10, 20, 50, 100 };
        }

        public Search(ISearch search)
            : this()
        {
            this.Page = search.Page;
            this.PageSize = search.PageSize;
            this.TotalRecords = search.TotalRecords;
            this.SortColumn = search.SortColumn;
            this.IsDesc = search.IsDesc;
            this.Filter = search.Filter;
            this.PreviousFilter = search.PreviousFilter;
            this.ReturnTotals = search.ReturnTotals;
        }

        public string Filter { get; set; }
        public bool IsDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public List<int> PageSizes { get; set; }
        public string PreviousFilter { get; set; }
        public bool ReturnTotals { get; set; }
        public string SortColumn { get; set; }
        public int TotalRecords { get; set; }
    }
}