using System;
using DPO.Common;

namespace DPO.Model.Light
{
    public class OrderGridViewModel : Search
    {
        public long orderId { get; set; }
        public string orderIdStr
        {
            get { return (orderId != 0) ? orderId.ToString() : ""; }
        }
        public long quoteId { get; set; }
        public string quoteIdStr
        {
            get { return (quoteId != 0) ? quoteId.ToString() : ""; }
        }
        public string activeQuoteTitle { get; set; }

        public long projectId { get; set; }
        public string projectIdStr
        {
            get { return (projectId != 0) ? projectId.ToString() : ""; }
        }
        public string projectName { get; set; }


        public long? discountRequestId { get; set; }
        public string discountRequestIdStr
        {
            get { return (discountRequestId != 0) ? discountRequestId.ToString() : ""; }
        }

        public long? commissionRequestId { get; set; }
        public string commissionRequestIdStr
        {
            get { return (commissionRequestId != 0) ? commissionRequestId.ToString() : ""; }
        }

        public string darComStatus { get; set; }

        public long? businessId { get; set; }
        public string businessIdStr
        {
            get { return (businessId != 0) ? businessId.ToString() : ""; }
        }
        public string businessName { get; set; }

        public string projectOwnerName { get; set; }

        public string dealerContractorName { get; set; }
      
        public Byte pricingTypeId { get; set; }
        public string pricingTypeDescription { get { return (pricingTypeId == 1) ? "Buy/Sell" : "Commission"; } }

        public string poNumber { get; set; }
        public string erpOrderNumber { get; set; }
        public string poAttachmentName { get; set; }
        public decimal totalListPrice { get; set; }
        public decimal totalNetPrice { get; set; }
        public decimal totalDiscountPercent { get; set; }
        public decimal totalSellPrice { get; set; }
        public int vrvODUcount { get; set; }
        public decimal splitODUcount { get; set; }

        public DateTime estimatedDeliveryDate { get; set; }
        public DateTime? estimatedReleaseDate { get; set; }
        public DateTime? projectDate { get; set; }
        public DateTime? submitDate { get; set; }
        public DateTime? ERPShipDate { get; set; }
       
        public OrderStatusTypeEnum orderStatusTypeId { get; set; }
        public string orderStatusDescription { get { return orderStatusTypeId.GetDescription(); } }

        public long submittedByUserId { get; set; }
        public string submittedByUserIdStr
        {
            get { return (submittedByUserId != 0) ? submittedByUserId.ToString() : ""; }
        }
        public string submittedByUserName { get; set; }

        

        public long createdByUserId { get; set; }
        public string createdByUserIdStr
        {
            get { return (createdByUserId != 0) ? createdByUserId.ToString() : ""; }
        }

        public long updatedByUserId { get; set; }
        public string updatedByUserIdStr
        {
            get { return (updatedByUserId != 0) ? updatedByUserId.ToString() : ""; }
        }

        public DateTime OrderReleaseDate { get; set; }
    }
}


