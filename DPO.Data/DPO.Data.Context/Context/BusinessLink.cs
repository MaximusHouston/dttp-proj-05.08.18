
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
    
public partial class BusinessLink

{

    public long BusinessLinkId { get; set; }

    public long BusinessId { get; set; }

    public long ParentBusinessId { get; set; }



    public virtual Business Business { get; set; }

    public virtual Business Business1 { get; set; }

}

}
