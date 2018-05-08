using System;
using System.Collections.Generic;

namespace DPO.Common
{
    public interface IActiveQuoteInfoBarModel
    {
        bool HasDAR { get; }

        bool Deleted { get; set; }

        bool IsTransferred { get; set; }

        bool Active { get; set; }

        long? ProjectId { get; set; }

        byte? ProjectStatusTypeId { get; set; }

        QuoteListModel ActiveQuoteSummary { get; set; }

        bool IsCommission { get; set; }

        bool CommissionConvertNo { get; set; }
        bool CommissionConvertYes { get; set; }

    }
}
