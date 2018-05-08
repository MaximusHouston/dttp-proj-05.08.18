 
using Newtonsoft.Json;

namespace DPO.Common
{
    [JsonObject(IsReference = false)]
    public class ProductAccessoryModel
    {
        public long? ParentProductId { get; set; }
        public ProductModel Accessory { get; set; }
        public int Quantity { get; set; }
    }
}
