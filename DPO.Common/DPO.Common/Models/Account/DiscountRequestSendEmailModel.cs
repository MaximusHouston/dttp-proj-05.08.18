namespace DPO.Common
{
    public class DiscountRequestSendEmailModel : SendEmailModel
    {
        public DiscountRequestSendEmailModel()
            : base()
        {

        }

        public DiscountRequestModel discountRequest;
        public string AccountManagerEmail { get; set; }
        public string AccountOwnerEmail { get; set; }
    }
}

