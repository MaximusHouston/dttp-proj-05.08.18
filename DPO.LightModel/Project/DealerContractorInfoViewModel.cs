using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Model.Light
{
    public class DealerContractorInfoViewModel
    {
        public long? AddressId { get; set; }
        public string AddressIdStr
        {
            get { return (AddressId != null) ? AddressId.ToString() : ""; }
        }

        public string DealerContractorName { get; set; }
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public int? StateId { get; set; }
        public string CountryCode { get; set; }
       
    }
}


