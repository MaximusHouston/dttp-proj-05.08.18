//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================


namespace DPO.Common
{
    public interface ISearch
    {
        string Filter { get; set; }
        bool IsDesc { get; set; }
        int? Page { get; set; }
        int? PageSize { get; set; }
        string PreviousFilter { get; set; }
        bool ReturnTotals { get; set; }
        string SortColumn { get; set; }
        int TotalRecords { get; set; }
    }
}