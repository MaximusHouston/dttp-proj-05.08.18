using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DPO.Web.Areas.TradeShow.Models;

namespace DPO.Web.Areas.Apps.Models
{
    public class RentingItem
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }

        [MinLength(2)]
        public string Name { get; set; }

        [MinLength(10)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public byte[] Image { get; set; }

        public int  Size { get; set; }

        public bool Selected
        {
            get;
            set;
        }

        //public virtual TradeShowOrder TradeShowOrder { get; set; }
    }
}