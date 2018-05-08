//===================================================================================

namespace DPO.Common
{
   public class SendEmailProjectTransfer : SendEmailModel
   {
       public SendEmailProjectTransfer()
           : base()
      {

      }
      public string Email { get; set; }

      public string ProjectName { get; set; }

      public string TransferFrom { get; set; }

   }
}
