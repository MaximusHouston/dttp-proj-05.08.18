using DPO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data
{

   public partial class Quote 
   {
       public bool QuoteItemBeingDeleted { get; set; }

       // Standard Totals

       public decimal StandardTotalDiscountAmount { get { return this.TotalList - this.StandardTotalNet; } }

       public decimal StandardTotalNet { get { return (this.TotalNetCommission + this.TotalNetNonCommission); } }

       public decimal StandardTotalCommissionAmount { get { return StandardTotalSell - StandardTotalNet; } }

       public decimal StandardTotalSell { get { return CalculateTotalApplyingDiscount(this.StandardTotalNet, this.DiscountPercentage, this.IsGrossMargin); } }

       public decimal StandardTotalSale { get { return StandardTotalSell + this.TotalFreight + this.TotalMisc + this.TotalService; } }

       // DAR Totals
       public decimal ApprovedTotalDiscountAmount { get { return this.TotalList - this.ApprovedTotalNet; } }

       public decimal ApprovedTotalNet { get { return CalculateTotalApplyingDiscount(this.TotalList, this.ApprovedDiscountPercentage, false); } }

       public decimal ApprovedTotalCommissionAmount { get { return ApprovedTotalNet - ApprovedTotalSell; } }

       public decimal ApprovedTotalSell { get { return CalculateTotalApplyingDiscount(this.ApprovedTotalNet, this.ApprovedCommissionPercentage, false); } }

       public decimal ApprovedTotalSale { get { return ApprovedTotalSell + this.TotalFreight + this.TotalMisc + this.TotalService; } }


       public decimal CalculateTotalApplyingDiscount(decimal total, decimal percentage, bool isGrossMargin)
       {
           if (isGrossMargin)
           {
               return total / (1 - percentage);
           }
           else // Mark up
           {
               return total * (1 + percentage);
           }
       }
   }
}
