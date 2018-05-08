using System;

namespace DPO.Common
{
    public class PIMAsset
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public PIMAssetType AssetType { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public string Size { get; set; }
    }
}
