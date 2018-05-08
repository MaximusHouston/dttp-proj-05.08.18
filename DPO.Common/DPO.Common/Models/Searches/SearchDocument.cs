using System;

namespace DPO.Common
{
    public class SearchDocument : Search
    {
        public int? DocumentTypeId { get; set; }
        public DateTime PIMUploadDate { get; set; }
    }
}
