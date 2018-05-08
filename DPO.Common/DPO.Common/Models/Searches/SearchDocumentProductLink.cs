using System;

namespace DPO.Common
{
    public class SearchDocumentProductLink : Search
    {
        public Guid DocumentId { get; set; }
        public long? ProductId { get; set; }
        public string ProductNumber { get; set; }
    }
}
