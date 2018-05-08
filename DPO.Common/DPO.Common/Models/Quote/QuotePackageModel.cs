
using System.Collections.Generic;


namespace DPO.Common
{
    public class QuotePackageModel
    {
        public PagedList<QuoteItemListModel> SelectedProducts { get; set; }
        public List<QuotePackageSelectedItemModel> SelectedDocuments { get; set; }
        public List<DocumentModel> AttachedFiles { get; set; }
        public QuoteModel Quote { get; set; }
    }
}
