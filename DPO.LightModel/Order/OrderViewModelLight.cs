using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DPO.Common;

namespace DPO.Model.Light
{
    public class OrderViewModelLight : PageModel
    {
        public long OrderId { get; set; }
        public string OrderIdStr
        {
            get { return (OrderId != 0) ? OrderId.ToString() : ""; }
        }

        public long QuoteId { get; set; }
        public string QuoteIdStr
        {
            get { return (QuoteId != 0) ? QuoteId.ToString() : ""; }
        }
        public string QuoteTitle { get; set; }

        public long ProjectId { get; set; }
        public string ProjectIdStr
        {
            get { return (ProjectId != 0) ? ProjectId.ToString() : ""; }
        }
        public string ProjectName { get; set; }

        public long? DiscountRequestId { get; set; }
        public string DiscountRequestIdStr
        {
            get { return (DiscountRequestId != 0) ? DiscountRequestId.ToString() : ""; }
        }

        public long? CommissionRequestId { get; set; }
        public string CommissionRequestIdStr
        {
            get { return (CommissionRequestId != 0) ? CommissionRequestId.ToString() : ""; }
        }

        public long? BusinessId { get; set; }
        public string BusinessIdStr
        {
            get { return (BusinessId != 0) ? BusinessId.ToString() : ""; }
        }   

        public long? ShipToAddressId { get; set; }
        public string ShipToAddressIdStr
        {
            get { return (ShipToAddressId != 0) ? ShipToAddressId.ToString() : ""; }
        }
        public string ShipToName { get; set; }
        public Byte PricingTypeId { get; set; }
        public string PONumber { get; set; }
        public HttpPostedFileBase POAttachment { get; set; }
        public string POAttachmentFileName { get; set; }
        public string POAttachmentFileLocation { get; set; }
        public int ConfigOrderNumber { get; set; }
        public string DARAttachmentFileName { get; set; }

        public string COMAttachmentFileName { get; set; }

        public string OrderAttachmentFileName { get; set; }

        public decimal TotalNetPrice { get; set; }
        public decimal TotalDiscountPercent { get; set; }
        public string Comments { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        //public DateTime? EstimatedReleaseDate
        //{
        //    //get { return (EstimatedDeliveryDate != null) ? EstimatedDeliveryDate.AddDays(-10) : DateTime.MinValue; }
        //    //set { EstimatedReleaseDate = value; }

        //    get { return (EstimatedDeliveryDate != null) ? EstimatedDeliveryDate : DateTime.MinValue; }
        //    set { EstimatedReleaseDate = value; }
        //}
        public DateTime? EstimatedReleaseDate { get; set; }
        public bool DeliveryAppointmentRequired { get; set; }
        public string DeliveryContactName { get; set; }
        public string DeliveryContactPhone { get; set; }

        public OrderStatusTypeEnum OrderStatusTypeId { get; set; }
        public string OrderStatusDescription { get { return OrderStatusTypeId.GetDescription(); } }

        public long SubmittedByUserId { get; set; }
        public string SubmittedByUserIdStr
        {
            get { return (SubmittedByUserId != 0) ? SubmittedByUserId.ToString() : ""; }
        }
        public string SubmittedByUserName { get; set; }

        public DateTime SubmitDate { get; set; }
        
        

        public long CreatedByUserId { get; set; }
        public string CreatedByUserIdStr
        {
            get { return (CreatedByUserId != 0) ? CreatedByUserId.ToString() : ""; }
        }

        public long UpdatedByUserId { get; set; }
        public string UpdatedByUserIdStr
        {
            get { return (UpdatedByUserId != 0) ? UpdatedByUserId.ToString() : ""; }
        }
        public string UpdatedByUser { get; set; }

        public string BusinessName { get; set; }

        public HttpPostedFileBase CompetitorQuoteFile { get; set; }

        public string ProjectOwner { get; set; }

        public long ProjectOwnerId { get; set; }

        public DateTime ProjectDate { get; set; }

        public int? ERPPOKey { get; set; }

        public string ERPOrderNumber { get; set; }

        public string ERPStatus { get; set; }

        public string ERPComments { get; set; }

        public DateTime? ERPOrderDate { get; set; }

        public DateTime? ERPInvoiceDate { get; set; }
        public DateTime? ERPShipDate { get; set; }

        public string ERPInvoiceNumber { get; set; }

        public DateTime OrderReleaseDate { get; set; }

        public decimal RequestedDiscountPercentage { get; set; }
        public decimal ApprovedDiscountPercentage { get; set; }

        public string ERPAccountId { get; set; }
        public string AccountID { get; set; } //this one is in BusinessTable equivalent to CRM Active ID in mapics

        public ProjectModel Project { get; set; }
        
        public bool HasConfiguredModel { get; set; }
    }
}
