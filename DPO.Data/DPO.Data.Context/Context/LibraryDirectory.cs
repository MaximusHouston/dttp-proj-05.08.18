
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
    
public partial class LibraryDirectory

{

    public LibraryDirectory()
    {

        this.LibraryDocumentRelationships = new HashSet<LibraryDocumentRelationship>();

    }


    public int LibraryDirectoryId { get; set; }

    public Nullable<int> ParentId { get; set; }

    public string Name { get; set; }

    public bool Protected { get; set; }



    public virtual ICollection<LibraryDocumentRelationship> LibraryDocumentRelationships { get; set; }

}

}
