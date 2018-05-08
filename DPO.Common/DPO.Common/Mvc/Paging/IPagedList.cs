//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System.Collections.Generic;

namespace DPO.Common
{
	public interface IPagedList<T> : IList<T>
	{
		int PageCount { get; }
		int TotalRecords { get; }
		int Page { get; }
		int PageSize { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		bool IsFirstPage { get; }
		bool IsLastPage { get; }
	}
}