using System.Collections.Generic;

namespace DPO.Model.Light
{
    public class FilterItem
    {
        public FilterItem()
        {
            Filters = new List<FilterItem>();
        }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public List<FilterItem> Filters { get; set; }
        public string Logic { get; set; }
        public string Name { get; set; }
    }
}
