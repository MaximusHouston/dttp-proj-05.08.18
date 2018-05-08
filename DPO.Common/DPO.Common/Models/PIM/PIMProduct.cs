using System.Collections.Generic;

namespace DPO.Common
{
    public class PIMProduct
    {
        public PIMProduct()
        {
            Specifications = new Dictionary<string, IPIMProductSpecification>();
            ProductReferences = new List<PIMProductReference>();
        }

        public string ID { get; set; }

        public List<PIMAssetReference> AssetReferences { get; set; }

        public List<PIMProductReference> ProductReferences { get; set; }

        public int ReleaseStatus { get; set; }

        public Dictionary<string, IPIMProductSpecification> Specifications { get; set; }
    }
}