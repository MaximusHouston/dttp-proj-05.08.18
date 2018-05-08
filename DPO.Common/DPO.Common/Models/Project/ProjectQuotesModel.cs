using System;
using System.Collections.Generic;

namespace DPO.Common
{
    public class ProjectQuotesModel : SearchProject, IActiveQuoteInfoBarModel
    {
        public ProjectQuotesModel()
        {
            Items = new PagedList<QuoteListModel>(new List<QuoteListModel>(), 1, 25);
        }
        public bool Active { get { return true; } set { } }
        public QuoteListModel ActiveQuoteSummary { get; set; }
        public bool AwaitingDiscountRequest { get; set; }
        public bool Deleted { get; set; }
        
        public long? DiscountRequestId { get; set; }
        public bool HasDAR { get { return (DiscountRequestId != null || AwaitingDiscountRequest); } }

        public long? CommissionRequestId { get; set; }

        public bool IsTransferred { get; set; }
        public PagedList<QuoteListModel> Items { get; set; }
        public string ProjectName { get; set; }
        public new byte? ProjectStatusTypeId { get; set; }
        //public bool HasPendingDAR { get { return ()} }

        public bool AwaitingCommissionRequest { get; set; }
        public bool HasCOM
        {
            get
            {
                return (AwaitingCommissionRequest);
            }
        }

        public bool IsCommission { get; set; }

        public bool CommissionConvertNo { get; set; }
        public bool CommissionConvertYes { get; set; }

        public byte OrderStatus { get; set; }
    }
}
