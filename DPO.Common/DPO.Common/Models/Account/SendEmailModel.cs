//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;

namespace DPO.Common
{
    public class SendEmailModel
    {
        public SendEmailModel()
        {
            this.To = new List<MailAddress>();
            this.HelpLink = "mailto:daikincity@daikincomfort.com";
        }
        public string ToName
        {
            get
            {
                return String.Join(",", To.Select(m => m.DisplayName));
            }
        }
        public MailAddress From { get; set; }

        public List<MailAddress> To { get; set; }

        public string Subject { get; set; }

        public bool RenderTextVersion { get; set; }

        public string BodyTextVersion { get; set; }
        public string BodyHtmlVersion { get; set; }

        public string HelpLink { get; set; }

        public List<string> OtherAttachmentFiles { get; set; }

        public string DARAttachmentFileName { get; set; }
        public string DARAttachmentFile { get; set; }
        public string COMAttachmentFileName { get; set; }
        public string COMAttachmentFile { get; set; }

        public string OrderAttachmentFile { get; set; }
        public string OrderAttachmentFileName { get; set; }

        public long? ProjectId { get; set; }
        public long? QuoteId { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessTypeDescription { get; set; }
    }

    //To use for Jsonconvert
    public class MailAddressModel
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public string User { get; set; }

    }

    public class SendEmailViewModel
    {
        public SendEmailViewModel()
        {
            this.To = new List<MailAddressModel>();
            this.HelpLink = "mailto:daikincity@daikincomfort.com";
        }
        public string ToName
        {
            get
            {
                return String.Join(",", To.Select(m => m.DisplayName));
            }
        }
        public MailAddressModel From { get; set; }

        public List<MailAddressModel> To { get; set; }

        public string Subject { get; set; }

        public bool RenderTextVersion { get; set; }

        public string BodyTextVersion { get; set; }
        public string BodyHtmlVersion { get; set; }

        public string HelpLink { get; set; }

        public List<string> OtherAttachmentFiles { get; set; }

        public string DARAttachmentFileName { get; set; }
        public string DARAttachmentFile { get; set; }
        public string COMAttachmentFileName { get; set; }
        public string COMAttachmentFile { get; set; }

        public string OrderAttachmentFile { get; set; }
        public string OrderAttachmentFileName { get; set; }

        public long? ProjectId { get; set; }
        public long? QuoteId { get; set; }
    }
}
