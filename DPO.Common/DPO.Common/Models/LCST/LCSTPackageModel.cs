
using System.Collections.Generic;

namespace DPO.Common
{
    //public class LCSTAccessories    //this is used to parse JSON string
    //{
    //    public LCSTAccessories() {
    //        Accessories = new List<string>();
    //    }
    //    public List<String> Accessories { get; set; }
    //}
    public class LCSTPackageModel
    {
        public LCSTPackageModel()
        {
            //Accessories = new List<string>();
            Accessories = new List<LCSTAccessory>();
        }
        public string ConfigType { get; set; }
        public string SubmittalPdf { get; set; }
        public string BaseModel { get; set; }
        public string Model { get; set; }
        public string SystemId { get; set; }
        //public string AccessoriesJSONString { get; set; }
        //public List<String> Accessories { get; set; }

        public List<LCSTAccessory> Accessories { get; set; }
    }
}
