
using System.Collections.Generic;
using System.Linq;

namespace DPO.Common
{
    public class QuoteItemsModel : SearchQuote, IActiveQuoteInfoBarModel
    {
        public string ProjectName{ get; set; }

        public byte? ProjectStatusTypeId { get; set; }

        public bool IsTransferred { get; set; }

        public bool WithCostPrice { get; set; }

        public QuoteListModel ActiveQuoteSummary { get; set; }

        public bool ProjectOwnerCommissionSchemeAllowed { get; set; }

        public QuoteItemsModel()
        {
            HasObsoleteProduct = false;

            HasUnavailableProduct = false;
            //InventoryStatusId = (int)ProductInventoryStatusTypeEnum.Available;
            HasObsoleteAndUnavailableProduct = false;

            Items = new PagedList<QuoteItemListModel>(new List<QuoteItemListModel>());

            DiscountRequests = new PagedList<DiscountRequestModel>(new List<DiscountRequestModel>());

            CommissionRequests = new PagedList<CommissionRequestModel>(new List<CommissionRequestModel>());

        }

        public PagedList<QuoteItemListModel> Items { get; set; }

        public PagedList<DiscountRequestModel> DiscountRequests { get; set; }

        public PagedList<CommissionRequestModel> CommissionRequests { get; set; }

        public PagedList<OrderViewModel> QuoteOrders { get; set; }

        //Packag Quote
        public List<DocumentModel> QuotePackage { get; set; }

        public List<DocumentModel> QuotePackageAttachedFiles { get; set; }

        public bool IsDocumentInPackage(DocumentModel document)
        {
            if (QuotePackage == null || document==null || !document.DocumentId.HasValue) return false;
            return QuotePackage.Any(p => p.DocumentId.HasValue && p.DocumentId.Value == document.DocumentId);
        }

        public bool AwaitingDiscountRequest { get; set; }

        public bool DiscountRequestAvailable { get; set; }

        public long? DiscountRequestId { get; set; }
        public long? CommissionRequestId { get; set; }
        public bool HasDAR { get { return (DiscountRequestId != null || AwaitingDiscountRequest); } }
        public bool HasConfiguredItem { get; set; }

        public bool IsCommissionRequest { get; set; }

        public bool AwaitingCommissionRequest { get; set; }
        public bool HasCOM {
            get
            {
                //return ( (CommissionRequestId != null && IsCommission == true) || AwaitingCommissionRequest);
                return (CommissionRequestId != null  || AwaitingCommissionRequest);
            } 
        }

        public bool HasOrder { get; set; }

        public bool IsCommission { get; set; }

        public bool CommissionConvertNo { get; set; }
        public bool CommissionConvertYes { get; set; }

        public bool ShowCommissionConvertPopup { get; set; }

        public byte CommissionRequestStatusTypeId { get; set; }

        public byte DiscountRequestStatusTypeId { get; set; }

        public bool CommissionRequestAvailable { get; set; }

        public long OrderId { get; set; }

        public byte OrderStatusTypeId { get; set; }

        //public int InventoryStatusId { get; set; }
        public bool HasUnavailableProduct { get; set; }
        public bool HasObsoleteProduct { get; set; }
        public bool HasObsoleteAndUnavailableProduct { get; set; }
    }
}
