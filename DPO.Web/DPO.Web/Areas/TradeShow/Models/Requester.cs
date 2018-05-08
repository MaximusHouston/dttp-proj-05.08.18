using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPO.Web.Areas.Apps.Models
{
    public class Requester
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        
        [MinLength(2)]
        [MaxLength(50)]
        [Required(ErrorMessage="Please provide Requester Title")]
        public string Title { get; set; }
        
        [Required(ErrorMessage="Please provide First Name")]
        [MinLength(2)]
        [MaxLength(20)]
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage="Please provide Last Name")]
        [MinLength(2)]
        [MaxLength(20)]
        [Display(Name="Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage="Please provide Company Name")]
        public string Company { get; set; }
        
        [Required(ErrorMessage="Please provide Contact Number")]
        [Phone]
        [Display(Name="Contact Number")]
        public string ContactNumber { get; set; }
        
        [Required(ErrorMessage="Please provide Email Address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}