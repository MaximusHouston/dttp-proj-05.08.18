using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public class PIMAssetReference
    {
        public string ProductId { get; set; }

        public string LinkedId { get; set; }

        public PIMAssetType AssetType { get; set; }

        public PIMAssetCrossReferenceType AssetCrossReferenceType { get; set; }

        public string AssetId { get; set; }

        public PIMAsset Asset { get; set; }

        public PIMAssetReference ShallowCopy()
        {
            return (PIMAssetReference)this.MemberwiseClone();
        }
    }
}
