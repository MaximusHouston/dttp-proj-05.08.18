 

namespace DPO.Common
{
    public class OrderSendEmailModel : SendEmailModel
    {
        public OrderSendEmailModel()
            : base()
        {

        }

        public OrderViewModel order;
        public string AccountManagerEmail { get; set; }
        public string AccountOwnerEmail { get; set; }
        public string AttachmentFile { get; set; }
        public new string DARAttachmentFile { get; set; }
        public new string COMAttachmentFile { get; set; }
    }
}
