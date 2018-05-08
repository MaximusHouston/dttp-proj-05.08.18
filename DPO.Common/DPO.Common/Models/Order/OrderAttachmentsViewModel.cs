using System;


namespace DPO.Common
{
    public class OrderAttachmentsViewModel
    {
       public long OrderAttachmentId {get;set;}
       public long OrderId {get;set; }
       public string FileName {get;set; }
       public DateTime Timestamp {get;set; }   
    }
}
