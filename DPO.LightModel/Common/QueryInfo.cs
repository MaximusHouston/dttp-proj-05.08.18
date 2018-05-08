using System.Collections.Generic;
namespace DPO.Model.Light
{
    public class QueryInfo
    {
        public QueryInfo() {
            Take = 50;
            Sort = new List<Sort>();
        }
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<Sort> Sort { get; set; }

        public string SortString 
        {
            get
            {
                if (Sort != null && Sort.Count > 0)
                {
                    var sorts = new List<string>();
                    Sort.ForEach(x => sorts.Add(string.Format("{0} {1}", x.Field, x.Dir)));
                    return string.Join(",", sorts.ToArray());
                }
                return string.Empty;
            }
        }

        public Filter Filter { get; set; }
    }
}
