using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPO.Web.Areas.Apps.Models
{
    public class Event
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please provide Event Name")]
        [Display(Name="Event Name")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Please provide Event Start Date")]
        [Display(Name = "Event Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Event End Date")]
        [Required(ErrorMessage = "Please provide Event End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Event Setup Date")]
        [Required(ErrorMessage = "Please provide Event Setup Date")]
        public DateTime SetupDate { get; set; }

        [Display(Name = "Booth Size")]
        [Required(ErrorMessage = "Please provide Booth Size")]
        [MinLength(2)]
        [MaxLength(20)]
        public string BoothSize { get; set; }

        [Display(Name = "Market Category")]
        [Required(ErrorMessage = "Please Select Market Category")]
        public MarketCategories MarketCategory { get; set; }

        [Display(Name = "Event Venue/Location")]
        [Required(ErrorMessage = "Please provide Event Location")]
        public string Location { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please provide Event Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please provide City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please provide State")]
        [Display(Name="State/Province")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please provide ZipCode")]
        [Display(Name="Zip Code")]
        public string ZipCode { get; set; }


        [MinLength(2)]
        [MaxLength(20)]
        [Display(Name="On-site Contact Person")]
        [Required(ErrorMessage="Please provide Contact Name")]
        public string Attendee { get; set; }

        [Phone]
        [Display(Name = "On-site Contact Phone Number")]
        [Required(ErrorMessage="Please provide Contact Number")]
        public string AttendeePhone { get; set; }

        [Display(Name ="Comments/Special Requests:")]
        public string Comments { get; set; }

    }

    public enum MarketCategories
    {
        VirticalMarket_Office,
        VirticalMarket_Retail,
        VirticalMarket_Hotel,
        VRV_Pro,
        Corporate_Marketing
    }


}