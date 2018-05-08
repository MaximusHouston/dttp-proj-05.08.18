//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 
using System.Collections.Generic;
 

namespace DPO.Common
{
   public class UserBasketModel
   {
       public string Description { get; set; }

       public long QuoteId{ get; set; }

       public int QuoteItemCount { get; set; }

       public long ProjectId { get; set; }

      public List<BasketItemModel> Items { get; set; }
   }
}
