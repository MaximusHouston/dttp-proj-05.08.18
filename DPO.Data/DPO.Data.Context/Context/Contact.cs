
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
    
public partial class Contact

{

    public Contact()
    {

        this.Businesses = new HashSet<Business>();

        this.Users = new HashSet<User>();

    }


    public long ContactId { get; set; }

    public string Phone { get; set; }

    public string Mobile { get; set; }

    public string ContactEmail { get; set; }

    public string Website { get; set; }



    public virtual ICollection<Business> Businesses { get; set; }

    public virtual ICollection<User> Users { get; set; }

}

}