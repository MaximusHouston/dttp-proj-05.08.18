
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DPO.Common
{
    public class QuoteModel : SearchQuote, IConcurrency, IActiveQuoteInfoBarModel
    {
        public QuoteModel()
        {
            Items = new List<QuoteItemModel>();
        }

        public ProjectModel Project { get; set; }

        public byte? ProjectStatusTypeId { get; set; }

        public string Description { get; set; }

        public bool IsGrossMargin { get; set; }

        public bool IsTransferred { get; set; }

        public bool IsCommissionSchemeAllowed { get; set; }

        public bool IsCommission { get; set; }

        /// <summary>
        /// This needs to be removed as it is old code
        /// </summary>
        public bool IsCommissionScheme { get; set; }

        public bool NewRecordAdded { get; set; }

        public bool AwaitingDiscountRequest { get; set; }

        public bool AwaitingCommissionRequest { get; set; }


        public decimal Multiplier { get; set; }
        public DropDownModel CommissionMultipliersTypes { get; set; }

        public string Notes { get; set; }

        public bool RecalculationRequired { get; set; }

        public decimal ItemCount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalList { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal DiscountPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNet { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalDiscount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedDiscountPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalMisc { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalSell { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalFreight { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal CommissionPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ApprovedCommissionPercentage { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CommissionCost { get { return TotalSell - TotalNet; } }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal Total { get { return TotalSell + TotalFreight; } } 

        public long? DiscountRequestId { get; set; }

        public long? CommissionRequestId { get; set; }

        public decimal AppliedCommissionPercentage { get { return (DiscountRequestId == null) ? CommissionPercentage : ApprovedCommissionPercentage; } }


        public decimal AppliedDiscountPercentage { get { return (DiscountRequestId == null) ? DiscountPercentage : ApprovedDiscountPercentage; } }

        public List<QuoteItemModel> Items { get; set; }

        private QuoteListModel _quoteListModel = new QuoteListModel();

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CommissionAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal TotalNetCommission { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CommissionNetMultiplierValue { get; set; }

        public bool CommissionConvertNo { get; set; }
        public bool CommissionConvertYes { get; set; }

        public byte CommissionRequestStatusTypeId { get; set; }

        public byte DiscountRequestStatusTypeId { get; set; }

        public long OrderId { get; set; }
        

        public QuoteListModel ActiveQuoteSummary
        {
            get
            {
                return new QuoteListModel
                {
                    Active = this.Active,
                    Deleted = this.Deleted,
                    ItemCount = this.ItemCount,
                    ProjectId = this.ProjectId,
                    QuoteId = this.QuoteId,
                    Timestamp = this.Timestamp,
                    Revision = this.Revision,
                    Title = this.Title,
                    TotalList = this.TotalList,
                    TotalMisc = this.TotalMisc,
                    TotalSell = this.TotalSell,
                    HasDAR = this.HasDAR,
                    CommissionAmount = this.CommissionAmount,
                    NetMultiplierValue = this.CommissionNetMultiplierValue,
                    TotalNet = this.TotalNet,
                    IsCommission = this.IsCommission,
                    TotalNetCommission = this.TotalNetCommission,
                    CommissionConvertNo = this.CommissionConvertNo,
                    CommissionRequestStatusTypeId = this.CommissionRequestStatusTypeId,
                    OrderId = this.OrderId,
                    OrderStatusTypeId = this.OrderStatusTypeId
                };
            }
            set
            {

            }

        }

        public bool HasDAR { get { return (DiscountRequestId != null || AwaitingDiscountRequest); } }

        public bool HasCOM
        {
            get
            {
                return ( (CommissionRequestId != null &&  IsCommission == true) ||  AwaitingCommissionRequest);
            }
        }

        public bool HasOrder
        {
            get
            {
                if ((OrderId != null && OrderId != 0))
                {
                    return true;
                }
                else return false;
            }
        }

        public bool ShowCommissionConvertPopup { get; set; }
        public byte OrderStatusTypeId { get; set; }

        public OrderOptionsModel OrderOptions { get; set; }
        public QuoteOptionsModel QuoteOptions { get; set; }
    }
}
