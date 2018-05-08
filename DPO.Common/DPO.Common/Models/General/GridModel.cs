 
using System.Collections.Generic;

namespace DPO.Common
{
    public class GridModel
    {
        public int GridId { get; set; }
        public string GridName { get; set; }
        public List<GridColumn> GridColumns { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<GridSort> SortOptions { get; set; }
        public List<GridFilter> FilterOptions { get; set; }
    }
}
