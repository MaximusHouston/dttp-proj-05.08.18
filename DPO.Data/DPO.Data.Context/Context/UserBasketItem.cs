
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DPO.Data
{

using System;
    using System.Collections.Generic;
    using DPO.Common;
    
public partial class UserBasketItem

{

    public long BasketItemId { get; set; }

    public long UserId { get; set; }

    public long ItemId { get; set; }

    public string Description { get; set; }

    public decimal Quantity { get; set; }



    public virtual User User { get; set; }

}

}