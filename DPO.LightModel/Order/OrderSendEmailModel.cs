using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using System.IO;

namespace DPO.Model.Light
{
    public class OrderSendEmailModel : SendEmailModel
    {
        public OrderSendEmailModel()
            : base()
        {

        }

        public OrderViewModelLight order;
        public string AccountManagerEmail { get; set; }
        public string AccountOwnerEmail { get; set; }

        public string POAttachmentFile { get; set; }

        public decimal RequestDiscountPercent { get; set; }
        public decimal ApprovedDiscountPercent { get; set; }

        public new string DARAttachmentFile { get; set; }
        public new string COMAttachmentFile { get; set; }

        public new string OrderAttachmentFile { get; set; }
        public List<string> OtherAttachmentFiles { get; set; }
    }
}
