
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
    
public partial class VwProductSystemComponent

{

    public long ProductId { get; set; }

    public decimal ListPrice { get; set; }

    public long ComponentProductId { get; set; }

    public int ComponentQuantity { get; set; }

    public decimal ComponentListPrice { get; set; }

    public int ComponentModelTypeId { get; set; }

    public string ParentProductNumber { get; set; }

    public string ComponentProductNumber { get; set; }

    public Nullable<int> MultiplierTypeId { get; set; }

    public string MultiplierType { get; set; }

}

}