using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Model.Light
{
    public class Filter
    {
        public Filter() {
            Filters = new List<FilterItem>();
        }
        public List<FilterItem> Filters { get; set; }
        public string Logic { get; set; }
        public string Name { get; set; }
    }
}
