
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
    
public partial class ProductAccessory
 : IConcurrency 
{

    public long ParentProductId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public int RequirementTypeId { get; set; }

    public System.DateTime ModifiedOn { get; set; }

    public System.DateTime Timestamp { get; set; }



    public virtual RequirementType RequirementType { get; set; }

    public virtual Product ParentProduct { get; set; }

    public virtual Product Product { get; set; }

}

}