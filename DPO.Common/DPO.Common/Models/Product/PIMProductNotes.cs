 
using System.Collections.Generic; 

namespace DPO.Common.Models
{
    // HACK:  Remove this
    public class PIMProductNotesOld
    {
        public string AttributeID { get; set; }
        public string Title { get; set; }

        //public string Value { get; set; }
        public List<string> Value { get; set; }
    }

}
