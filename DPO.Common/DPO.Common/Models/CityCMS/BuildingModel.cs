using System;

namespace DPO.Common
{
    public class BuildingModel
    {
        public long? id { get; set; }
        public string name { get; set; }
        public int? typeId { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string videoIn { get; set; }
        public string videoInPoster { get; set; }
        public string hotspotX { get; set; }
        public string hotspotY { get; set; }
        public string menuImage { get; set; }
        public string buildingFolderName { get; set; }

        //all buildings
        public BuildingFloorsModel floors { get; set; }

        public String JsonName()
        {
            if (name == null) return "";
            return name.Replace(" ", "_").ToLower();
        }
    }
}
