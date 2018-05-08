//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System;
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
   public class SendEmailApprovalModel : SendEmailModel
   {
       public SendEmailApprovalModel()
           : base()
      {

      }
      public bool Approved { get; set; }

      public string Reason { get; set; }

      public long? ProjectId { get; set; }

      public string ProjectName { get; set; }

      public string ProjectOwnerName { get; set; }

      public string ModifierName { get; set; }

      [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
      public DateTime? ProjectDate { get; set; }

      public string BusinessName { get; set; }

      public string QuoteTitle { get; set; }

      public decimal RequestedDiscount { get; set; }

      public decimal ApprovedDiscount { get; set; }

      public decimal? TotalNet { get; set; }

      public decimal? RequestedCommission { get; set; }
      public decimal? ApprovedCommission { get; set; }
      public decimal? ApprovedTotalCommission { get; set; }

      public decimal? RequestedMultiplier { get; set; }
      public decimal? ApprovedMultiplier { get; set; }

      public decimal? RequestedCommissionPercentage { get; set; }
      public decimal? ApprovedCommissionPercentage { get; set; }

      public decimal? ApprovedTotalNet { get; set; }

      public decimal? TotalList { get; set; }

    // these properties need Order Model send Email
      public int ERPPOKey { get; set; }
      public string ERPOrderNumber { get; set; }
      public DateTime ERPOrderDate { get; set; }
      public DateTime? ERPInvoiceDate { get; set; }
      public string ERPInvoiceNumber { get; set; }
      public string PONumber { get; set; }
   }
}
