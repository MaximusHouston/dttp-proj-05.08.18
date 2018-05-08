using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Model.Light
{
    public class ShipToAddressViewModel
    {
        public long? AddressId { get; set; }
        public string AddressIdStr
        {
            get { return (AddressId != null) ? AddressId.ToString() : ""; }
        } 

        public string ShipToName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }
        public int? StateId { get; set; }
        public string CountryCode { get; set; }
        public int? SquareFootage { get; set; }
        public int? NumberOfFloor { get; set; }
       
    }
}
