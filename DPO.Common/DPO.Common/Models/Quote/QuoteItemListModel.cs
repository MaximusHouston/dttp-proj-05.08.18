 
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DPO.Common
{
    public class QuoteItemListModel : ProductModel
    {
        public string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? ExtendedNet
        {
            get
            {
                return this.PriceNet * this.Quantity;
            }
        }

        public bool IsCommissionable { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? PriceList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal? PriceNet { get; set; }

        //TODO: uncomment if something broken
        //public new long? QuoteId { get; set; }
        public long? QuoteItemId { get; set; }
        public DocumentModel GetDocument(DocumentTypeEnum type)
        {
            if (Documents == null) return null;

            return Documents.Where(d => d.DocumentTypeId == (int)type).FirstOrDefault();
        }

        public decimal TotalListPriceUnitary { get; set; }
        public decimal TotalCommissionUnitary { get; set; }
        //public string CodeString { get; set; }
        //public byte? LineItemTypeId { get; set; }
    }
}
