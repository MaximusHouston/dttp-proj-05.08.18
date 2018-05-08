using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Data.Models
{
    public class ProductDefinition
    {
        public Product IndoorProduct { get; set; }
        public Product OutdoorProduct { get; set; }
        public Product Product { get; set; }
        public VwProductSpecification VWProductSpecifications { get; set; }
    }
}
