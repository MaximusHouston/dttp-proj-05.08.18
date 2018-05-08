
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
    
public partial class Address

{

    public Address()
    {

        this.Businesses = new HashSet<Business>();

        this.CustomerProjects = new HashSet<Project>();

        this.EngineerProjects = new HashSet<Project>();

        this.SellerProjects = new HashSet<Project>();

        this.ShipToProjects = new HashSet<Project>();

        this.Users = new HashSet<User>();

        this.Orders = new HashSet<Order>();

    }


    public long AddressId { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string Location { get; set; }

    public Nullable<int> StateId { get; set; }

    public string PostalCode { get; set; }



    public virtual ICollection<Business> Businesses { get; set; }

    public virtual State State { get; set; }

    public virtual ICollection<Project> CustomerProjects { get; set; }

    public virtual ICollection<Project> EngineerProjects { get; set; }

    public virtual ICollection<Project> SellerProjects { get; set; }

    public virtual ICollection<Project> ShipToProjects { get; set; }

    public virtual ICollection<User> Users { get; set; }

    public virtual ICollection<Order> Orders { get; set; }

}

}
