
namespace DPO.Common
{
    public class ToolModel
    {
        public long? ToolId { get; set; }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public byte? Order { get; set; }
        public bool? AddToQuote { get; set; }
        public string AccessUrl { get; set; }
    }
}
