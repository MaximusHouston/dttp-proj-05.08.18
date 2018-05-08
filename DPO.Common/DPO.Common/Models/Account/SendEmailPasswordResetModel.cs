//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

namespace DPO.Common
{
   public class SendEmailPasswordResetModel : SendEmailModel
   {
      public SendEmailPasswordResetModel() : base()
      {

      }
      public string SecurityKey { get; set; }
   }
}
