//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
using System.Collections.Generic;

namespace DPO.Common
{
   public class SendEmailContactUsModel : SendEmailModel
   {
       public SendEmailContactUsModel()
           : base()
      {

      }

       //contact us form
       public string ContactUsAddress { get; set; }
       public List<string> ContactUsSubjects { get; set; }
       public string ContactUsTel { get; set; }
       public string Message { get; set; }
       public string UserEmail { get; set; }
       public string UserName { get; set; }
   }
}
