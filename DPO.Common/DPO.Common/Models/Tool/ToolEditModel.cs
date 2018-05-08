 
using System.Collections.Generic;

namespace DPO.Common
{
    public class ToolEditModel : ToolModel
    {
        public ToolEditModel()
        {
            this.BusinessTypes = new List<BusinessModel>();
        }

        public List<BusinessModel> BusinessTypes { get; set; }
        public int[] PostedBusinessTypeIds { get; set; }
    }
}
