 

namespace DPO.Common
{
    public class BusinessLinkModel
    {
        public long? BusinessLinkId { get; set; }
        public long BusinessId { get; set; }
        public long ParentBusinessId { get; set; }
    }
}
