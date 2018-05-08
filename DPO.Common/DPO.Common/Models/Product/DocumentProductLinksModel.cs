 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class DocumentProductLinksModel : SearchDocumentProductLink
    {
        public int? ProductStatusTypeId { get; set; }
        public int? ProductFamilyId { get; set; }
        public List<DocumentProductLinkModel> Documents { get; set; }
    }
}
