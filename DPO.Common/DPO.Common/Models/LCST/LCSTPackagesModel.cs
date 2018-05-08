using System.Collections.Generic;

namespace DPO.Common
{
    public class LCSTPackagesModel
    {
        public LCSTPackagesModel()
        {
            Packages = new List<LCSTPackageModel>();
        }

        public string Token { get; set; }
        public long QuoteId { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }

        //public string OrderType { get; set; }// this will be moved to LCSTPackageModel as ConfigType

        public List<LCSTPackageModel> Packages { get; set; }
        public List<string> ValidProducts = new List<string>();
        public List<string> InValidProducts = new List<string>();
        public string Message { get; set; }
    }
}
