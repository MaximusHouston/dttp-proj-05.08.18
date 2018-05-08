using System;
using System.Collections.Generic;
using System.Web;

namespace DPO.Common
{
    public class OrderViewModel
    {
        public long OrderId { get; set; }
        public long QuoteId { get; set; }
        public long ProjectId { get; set; }
        public long? DiscountRequestId { get; set; }
        public long? CommissionRequestId { get; set; }
        public long BusinessId { get; set; }
        public long ShipToAddressId { get; set; }
        public Byte PricingTypeId { get; set; }
        public string PONumber { get; set; }
        public string POAttachmentFileName { get; set; }
        public decimal TotalDiscountPercent { get; set; }
        public decimal TotalNetPrice { get; set; }
        public string Comments { get; set; }
        public DateTime? EstimatedReleaseDate { get; set; }
        public bool DeliveryAppointmentRequired { get; set; }
        public string DeliveryContactName { get; set; }
        public string DeliveryContactPhone { get; set; }
        public OrderStatusTypeEnum OrderStatusTypeId { get; set; }
        public string OrderStatusTypeDescription
        {
            get { return OrderStatusTypeId.GetDescription(); }
            set { }
        }

        public long SubmittedByUserId { get; set; }
        public string SubmittedByUser { get; set; }
        public DateTime? SubmitDate { get; set; }

        public long CreatedByUserId { get; set; }

        public long UpdatedByUserId { get; set; }
        public string UpdatedByUser { get; set; }

        public DateTime Timestamp { get; set; }

        public string ProjectOwner { get; set; }

        public long ProjectOwnerId { get; set; }

        public string BusinessName { get; set; }

        public string EmailList { get; set; }

        public bool IsValidEmails { get; set; }

        public bool ShouldSendEmail { get; set; }

        public HttpPostedFileBase CompetitorQuoteFile { get; set; }

        public List<string> InvalidEmails { get; set; }

        public string QuoteTitle { get; set; }

        public string ProjectName { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public DateTime OrderReleaseDate { get; set; }

        public int? ERPPOKey { get; set; }

        public string ERPOrderNumber { get; set; }

        public string ERPStatus { get; set; }

        public string ERPComments { get; set; }

        public DateTime? ERPOrderDate { get; set; }

        public DateTime? ERPInvoiceDate { get; set; }
        public DateTime? ERPShipDate { get; set; }

        public string ERPInvoiceNumber { get; set; }

        public DateTime ProjectDate { get; set; }

        public ProjectModel Project { get; set; }

        public BusinessModel Business { get; set; }

        public UserModel User { get; set; }

        public bool HasQuote { get { return (this.Project != null && this.Project.ActiveQuoteSummary != null); } }

        public QuoteListModel Quote { get { return ((HasQuote) ? this.Project.ActiveQuoteSummary : new QuoteListModel()); } }

        //public List<QuoteItemListModel> QuoteItems { get; set; }

        public List<QuoteItemModel> QuoteItems { get; set; }

        public string ShipToName { get; set; }

        public string SellerName { get; set; }

        public string CustomerName { get; set; }

        public string EngineerName { get; set; }

    }
}
