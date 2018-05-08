using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPO.Web.Areas.Apps.Models
{
    public class Shipping
    {
       [ScaffoldColumn(false)]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       [Key]
       public int ID { get;set;}
       
       [Required(ErrorMessage="Please provide Company")]
       public string Company { get; set;}

       [Required(ErrorMessage="Please provide Contact Name")]
       [Display(Name="Contact Name")]
       public string ContactName { get; set;}

       [Required(ErrorMessage="Please provide Shipping Address")]
       [Display(Name="Shipping Address")]
       public string Address { get; set;}

       [Required(ErrorMessage="Please provide Shipping City")]
       [Display(Name="Shipping City")]
       public string City { get; set;}

       [Required(ErrorMessage="Please provide Shipping State")]
       [Display(Name="Shipping State/Province")]
       public string State { get; set;}

       [Required(ErrorMessage = "Please provide Shipping ZipCode")]
       [Display(Name = "Shipping Zip Code")]
       public string Zip { get; set; }
       
    }

   
 
}