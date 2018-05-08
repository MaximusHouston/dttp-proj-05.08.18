//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
namespace DPO.Common
{
   public class ContactModel
   {
       public long? ContactId { get; set; }
       public string ContactEmail { get; set; }
       public string WebAddress { get; set; }
       public string OfficeNumber { get; set; }
       public string MobileNumber { get; set; }
       public string FaxNumber { get; set; }
       public string ValidateForCountryCode { get; set; }
   }

}
