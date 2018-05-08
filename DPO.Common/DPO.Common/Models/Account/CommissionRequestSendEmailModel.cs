namespace DPO.Common
{
    public class CommissionRequestSendEmailModel : SendEmailModel
    {
        public CommissionRequestSendEmailModel()
            : base()
        {

        }

        public CommissionRequestModel commissionRequest;
        public string AccountManagerEmail { get; set; }
        public string AccountOwnerEmail { get; set; }
    }
}

