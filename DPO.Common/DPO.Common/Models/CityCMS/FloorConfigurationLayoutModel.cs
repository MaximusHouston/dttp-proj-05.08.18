 
namespace DPO.Common
{
    public class FloorConfigurationLayoutModel
    {
        public int id { get; set; }
        public int? configId { get; set; }
        public string name { get; set; }

        public FloorConfigurationLayoutNodeModel node { get; set; }
    }
}
