using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DPO.Common
{
    public class SelectListItemExt : SelectListItem
    {
        private decimal valueDecimal;
        private long valueLong;
        public string DataRequirementLevel { get; set; }
        // public new bool? Selected { get; set; }
        public new bool Disabled { get; set; }
        public object HtmlAttributes { get; set; }
        public decimal ValueDecimal { get { return valueDecimal; } set { Value = value.ToString(); valueDecimal = value; } }
        public long ValueLong { get { return valueLong; } set { Value = value.ToString(); valueLong = value; } }
    }


}
